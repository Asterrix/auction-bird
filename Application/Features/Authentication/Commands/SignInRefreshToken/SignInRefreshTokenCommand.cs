using System.Net;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Mapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Features.Authentication.Commands.SignInRefreshToken;

public record SignInRefreshTokenCommand(SignInRefreshTokenDto User) : IRequest<bool>;

public sealed class SignInRefreshTokenCommandHandler : IRequestHandler<SignInRefreshTokenCommand, bool>
{
    private readonly IAmazonCognitoIdentityProvider _cognitoIdentityProvider;
    private readonly IHttpContextAccessor _contextAccessor;

    public SignInRefreshTokenCommandHandler(IAmazonCognitoIdentityProvider cognitoIdentityProvider, IHttpContextAccessor contextAccessor)
    {
        _cognitoIdentityProvider = cognitoIdentityProvider;
        _contextAccessor = contextAccessor;
    }

    public async Task<bool> Handle(SignInRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, string> authParams = new()
        {
            { "REFRESH_TOKEN", request.User.RefreshToken }
        };

        InitiateAuthRequest initiateAuthRequest = new()
        {
            AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
            AuthParameters = authParams,
            ClientId = request.User.ClientId
        };

        try
        {
            Log.Information("Initiating refresh token flow.");
            
            InitiateAuthResponse initiateAuthResponse = await _cognitoIdentityProvider.InitiateAuthAsync(initiateAuthRequest, cancellationToken);
            
            if (initiateAuthResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                if(_contextAccessor.HttpContext is null) throw new ArgumentNullException(nameof(_contextAccessor.HttpContext));
                
                _contextAccessor.HttpContext.Response.Cookies.Delete("idToken");
                _contextAccessor.HttpContext.Response.Cookies.Delete("refreshToken");
                
                _contextAccessor.HttpContext.Response.Cookies.Append("idToken",
                    initiateAuthResponse.AuthenticationResult.IdToken, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
            
                return true;
            }
        }
        catch (Exception e)
        {
            Log.Warning("Error occurred while refreshing token: {Error}", e.Message);
            
            throw new AmazonServiceException(e.Message);
        }
        
        return false;
    }
}