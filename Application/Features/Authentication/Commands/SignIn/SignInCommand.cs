using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Commands.Mapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Features.Authentication.Commands.SignIn;

public record SignInCommand(SignInDto User) : IRequest<bool>;

public sealed class SignInCommandHandler : IRequestHandler<SignInCommand, bool>
{
    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    private readonly IHttpContextAccessor _contextAccessor;

    public SignInCommandHandler(IAmazonCognitoIdentityProvider cognitoService, IHttpContextAccessor contextAccessor)
    {
        _cognitoService = cognitoService;
        _contextAccessor = contextAccessor;
    }

    public async Task<bool> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, string> authParameters = new()
        {
            { "USERNAME", request.User.Email },
            { "PASSWORD", request.User.Password }
        };

        InitiateAuthRequest initiateAuthRequest = new()
        {
            AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
            AuthParameters = authParameters,
            ClientId = request.User.ClientId
        };

        try
        {
            InitiateAuthResponse initiateAuthResponse = await _cognitoService.InitiateAuthAsync(initiateAuthRequest, cancellationToken);

            string idToken = initiateAuthResponse.AuthenticationResult.IdToken;

            if (_contextAccessor.HttpContext == null)
            {
                throw new ArgumentNullException(nameof(_contextAccessor.HttpContext));
            }

            _contextAccessor.HttpContext.Response.Cookies.Append("idToken", idToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            
            _contextAccessor.HttpContext.Response.Cookies.Append("refreshToken", initiateAuthResponse.AuthenticationResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return true;
        }
        catch (UserNotFoundException e)
        {
            Log.Information("User not found: {0}", e.Message);

            throw new AmazonServiceException("Invalid username or password.");
        }
        catch (Exception e)
        {
            Log.Information("Error occured while signing in: {0}", e.Message);

            throw new AmazonServiceException(e.Message);
        }
    }
}