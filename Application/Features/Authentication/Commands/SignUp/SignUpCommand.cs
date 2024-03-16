using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Mapper;
using Application.Features.Payment.Commands;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Serilog;
using Stripe;

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
    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    private readonly IValidator<SignUpCommand> _validator;
    private readonly ISender _sender;

    public SignUpCommandHandler(IAmazonCognitoIdentityProvider cognitoService, IValidator<SignUpCommand> validator,
        ISender sender)
    {
        _cognitoService = cognitoService;
        _validator = validator;
        _sender = sender;
    }

    public async Task<bool> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Customer customer = await CreateCustomer(request.User.Email, cancellationToken);
        await SignUpUser(request.User, customer.Id, cancellationToken);
        await ConfirmUser(request.User.Email, cancellationToken);
        await AddUserToGroup(request.User.Email, cancellationToken);

        return true;
    }

    /// <summary>
    /// Signs up the user with the provided details.
    /// </summary>
    /// <param name="request">
    /// Details of the user to be signed up.
    /// </param>
    /// <param name="customerId">String representation of the customer id.</param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Returns true if the user is signed up successfully.
    /// </returns>
    /// <exception cref="AmazonServiceException">
    /// Throws AmazonServiceException if any exception occurs during the sign-up process.
    /// </exception>
    private async Task<bool> SignUpUser(SignUpDto request, string customerId, CancellationToken cancellationToken)
    {
        AttributeType firstName = new() { Name = "given_name", Value = request.FirstName };
        AttributeType lastName = new() { Name = "family_name", Value = request.LastName };
        AttributeType email = new() { Name = "email", Value = request.Email };
        AttributeType customer = new() { Name = "custom:customer_id", Value = customerId };

        List<AttributeType> userAttributes = [firstName, lastName, email, customer];

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
            UserPoolId = CognitoConstant.PoolId
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
            GroupName = CognitoConstant.DefaultGroupName,
            Username = email,
            UserPoolId = CognitoConstant.PoolId
        };

        try
        {
            Log.Information("Adding user with email: {Email} to group: {GroupName}", email,
                CognitoConstant.DefaultGroupName);
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
    /// Creates Stripe customer for the user.
    /// Deletes User if any exception occurs during the sign-up process.
    /// </summary>
    /// <param name="email">Identifier for the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Returns true if the customer is created successfully.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Throws ArgumentException if any exception occurs during the sign-up process.
    /// </exception>
    private async Task<Customer> CreateCustomer(string email, CancellationToken cancellationToken)
    {
        Customer customer = await _sender.Send(new CreateCustomerCommand(email), cancellationToken);
        return customer;
    }

    /// <summary>
    /// Fallback method to delete customer if any exception occurs during the sign-up process.
    /// </summary>
    /// <param name="email">Identifier for the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Returns true if the customer is deleted successfully.
    /// </returns>
    private async Task<bool> DeleteCustomerFallback(string email, CancellationToken cancellationToken)
    {
        bool deleted = await _sender.Send(new DeleteCustomerCommand(email), cancellationToken);
        return deleted;
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
            UserPoolId = CognitoConstant.PoolId
        };

        try
        {
            Log.Information("Deleting user with email: {Email}", email);
            await _cognitoService.AdminDeleteUserAsync(deleteUserRequest, cancellationToken);
            await DeleteCustomerFallback(email, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            throw new AmazonServiceException(e.Message);
        }
    }
}