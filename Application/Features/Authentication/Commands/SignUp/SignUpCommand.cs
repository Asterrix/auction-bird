using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Commands.Mapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Serilog;

namespace Application.Features.Authentication.Commands.SignUp;

public sealed class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    private const string NameRegex = @"^\p{L}+$";
    private const string EmailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$";

    // First Name
    private const byte FirstNameMinLength = 3;
    private const byte FirstNameMaxLength = 20;

    // Last Name
    private const byte LastNameMinLength = 3;
    private const byte LastNameMaxLength = 32;

    // Email
    private const byte EmailMinLength = 7;
    private const byte EmailMaxLength = 64;

    // Password
    private const byte PasswordMinLength = 8;
    private const byte PasswordMaxLength = 32;
    
    public SignUpCommandValidator()
    {
        RuleFor(u => u.User.FirstName)
            .NotEmpty()
            .WithMessage("First Name is required.")
            .MinimumLength(FirstNameMinLength)
            .WithMessage("First Name must be at least 3 characters long.")
            .MaximumLength(FirstNameMaxLength)
            .WithMessage("First Name must not exceed 20 characters.")
            .Matches(NameRegex)
            .WithMessage("First Name must consist of only alphabetical characters.");

        RuleFor(u => u.User.LastName)
            .NotEmpty()
            .WithMessage("Last Name is required.")
            .MinimumLength(LastNameMinLength)
            .WithMessage("Last Name must be at least 3 characters long.")
            .MaximumLength(LastNameMaxLength)
            .WithMessage("Last Name must not exceed 32 characters.")
            .Matches(NameRegex)
            .WithMessage("Last Name must consist of only alphabetical characters.");

        RuleFor(u => u.User.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email is not in the correct format.")
            .MinimumLength(EmailMinLength)
            .WithMessage("Email must be at least 7 characters long.")
            .MaximumLength(EmailMaxLength)
            .WithMessage("Email must not exceed 64 characters.")
            .Matches(EmailRegex)
            .WithMessage("Email is not in the correct format.");

        RuleFor(u => u.User.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(PasswordMinLength)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(PasswordMaxLength)
            .WithMessage("Password must not exceed 32 characters.")
            .Matches(PasswordRegex)
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one digit and one special character.");

        RuleFor(u => u.User.TermsAndConditionsAccepted)
            .Equal(true)
            .WithMessage("You must accept the terms and conditions to sign up.");
        
        RuleFor(u => u.User.IsOver18)
            .Equal(true)
            .WithMessage("You must be at least 18 years old to sign up.");
    }
}

public record SignUpCommand(SignUpDto User) : IRequest<bool>;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, bool>
{
    private const string DefaultGroupName = "User"; // Default group name to which the user will be added.

    private readonly string _poolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID") ??
                                      throw new InvalidOperationException("COGNITO_USER_POOL_ID is not set");

    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    private readonly IValidator<SignUpCommand> _validator;

    public SignUpCommandHandler(IAmazonCognitoIdentityProvider cognitoService, IValidator<SignUpCommand> validator)
    {
        _cognitoService = cognitoService;
        _validator = validator;
    }

    public async Task<bool> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        bool isUserSignedUp = await SignUpUser(request.User, cancellationToken);
        bool isUserConfirmed = await ConfirmUser(request.User.Email, cancellationToken);
        bool isUserAddedToGroup = await AddUserToGroup(request.User.Email, cancellationToken);

        return isUserSignedUp && isUserConfirmed && isUserAddedToGroup;
    }

    /// <summary>
    /// Signs up the user with the provided details.
    /// </summary>
    /// <param name="request">
    /// Details of the user to be signed up.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Returns true if the user is signed up successfully.
    /// </returns>
    /// <exception cref="AmazonServiceException">
    /// Throws AmazonServiceException if any exception occurs during the sign-up process.
    /// </exception>
    private async Task<bool> SignUpUser(SignUpDto request, CancellationToken cancellationToken)
    {
        AttributeType firstName = new() { Name = "given_name", Value = request.FirstName };
        AttributeType lastName = new() { Name = "family_name", Value = request.LastName };
        AttributeType email = new() { Name = "email", Value = request.Email };

        List<AttributeType> userAttributes = [firstName, lastName, email];

        SignUpRequest signUpRequest = new()
        {
            ClientId = request.ClientId,
            Password = request.Password,
            Username = request.Email,
            UserAttributes = userAttributes
        };

        try
        {
            Log.Information("Signing up user with email: {Email}", request.Email);
            await _cognitoService.SignUpAsync(signUpRequest, cancellationToken);
            return true;
        }
        catch (UsernameExistsException)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("Email", "Email address is already in use.")
            });
        }
        catch (Exception e)
        {
            throw new AmazonServiceException(e.Message);
        }
    }

    /// <summary>
    /// Confirms user account.
    /// </summary>
    /// <param name="email">
    /// Identifier of the user to be confirmed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Returns true if the user is confirmed successfully.
    /// </returns>
    /// <exception cref="AmazonServiceException">
    /// Throws AmazonServiceException if any exception occurs during the sign-up process.
    /// </exception>
    private async Task<bool> ConfirmUser(string email, CancellationToken cancellationToken)
    {
        AdminConfirmSignUpRequest confirmSignUpRequest = new()
        {
            Username = email,
            UserPoolId = _poolId
        };

        try
        {
            Log.Information("Confirming user with email: {Email}", email);
            await _cognitoService.AdminConfirmSignUpAsync(confirmSignUpRequest, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            await DeleteUserFallback(email, cancellationToken);
            throw new AmazonServiceException(e.Message);
        }
    }

    /// <summary>
    /// Adds user to the default group.
    /// </summary>
    /// <param name="email">
    /// Identifier of the user to be added to the group.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Returns true if the user is added to the group successfully.
    /// </returns>
    /// <exception cref="AmazonServiceException">
    /// Throws AmazonServiceException if any exception occurs during the sign-up process.
    /// </exception>
    private async Task<bool> AddUserToGroup(string email, CancellationToken cancellationToken)
    {
        AdminAddUserToGroupRequest addUserToGroupRequest = new()
        {
            GroupName = DefaultGroupName,
            Username = email,
            UserPoolId = _poolId
        };

        try
        {
            Log.Information("Adding user with email: {Email} to group: {GroupName}", email, DefaultGroupName);
            await _cognitoService.AdminAddUserToGroupAsync(addUserToGroupRequest, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            await DeleteUserFallback(email, cancellationToken);
            throw new AmazonServiceException(e.Message);
        }
    }

    /// <summary>
    /// Fallback method to delete user if any exception occurs during the sign-up process.
    /// </summary>
    /// <param name="email">
    /// Identifier of the user to be deleted
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <exception cref="AmazonServiceException">
    /// Throws AmazonServiceException if any exception occurs during the sign-up process.
    /// </exception>
    private async Task<bool> DeleteUserFallback(string email, CancellationToken cancellationToken)
    {
        AdminDeleteUserRequest deleteUserRequest = new()
        {
            Username = email,
            UserPoolId = _poolId
        };

        try
        {
            Log.Information("Deleting user with email: {Email}", email);
            await _cognitoService.AdminDeleteUserAsync(deleteUserRequest, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            throw new AmazonServiceException(e.Message);
        }
    }
}