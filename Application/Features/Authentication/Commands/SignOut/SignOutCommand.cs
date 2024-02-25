using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Commands.Mapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Features.Authentication.Commands.SignOut;

public record SignOutCommand(SignOutDto User) : IRequest<bool>;

public sealed class SignOutCommandHandler : IRequestHandler<SignOutCommand, bool>
{
    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    private readonly IHttpContextAccessor _contextAccessor;

    public SignOutCommandHandler(IAmazonCognitoIdentityProvider cognitoService, IHttpContextAccessor contextAccessor)
    {
        _cognitoService = cognitoService;
        _contextAccessor = contextAccessor;
    }

    public async Task<bool> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        AdminUserGlobalSignOutRequest globalSignOutRequest = new()
        {
            Username = request.User.Username,
            UserPoolId = CognitoConstant.PoolId
        };

        try
        {
            Log.Information("Signing out user: {Username}", request.User.Username);
            
            await _cognitoService.AdminUserGlobalSignOutAsync(globalSignOutRequest, cancellationToken);
            _contextAccessor.HttpContext?.Response.Cookies.Delete("idToken");

            Log.Information("User signed out: {Username}", request.User.Username);
            
            return true;
        }
        catch (Exception e)
        {
            Log.Warning("Failed to sign out user: {Username}", request.User.Username);
            
            throw new AmazonServiceException(e.Message);
        }
    }
}