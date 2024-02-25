using System.IdentityModel.Tokens.Jwt;
using Application.Features.Authentication.Commands.Mapper;
using Application.Features.Authentication.Commands.SignInRefreshToken;
using LanguageExt;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.Authentication.Commands.ValidateToken;

public record ValidateTokenCommand(string ClientId, string IdToken, Option<string> RefreshToken) : IRequest<bool>;

public sealed class ValidateTokenCommandHandler(ISender sender) : IRequestHandler<ValidateTokenCommand, bool>
{
    public async Task<bool> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
    {
        string jwksUrl = $"https://cognito-idp.eu-west-3.amazonaws.com/{CognitoConstant.PoolId}/.well-known/jwks.json";
        HttpClient httpClient = new();
        HttpResponseMessage jwksResponse = await httpClient.GetAsync(jwksUrl, cancellationToken);
        string jwksString = await jwksResponse.Content.ReadAsStringAsync(cancellationToken);
        IList<SecurityKey>? issuerSigningKeys = new JsonWebKeySet(jwksString).GetSigningKeys();

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = issuerSigningKeys.First()
        };

        JwtSecurityTokenHandler tokenHandler = new();

        try
        {
            tokenHandler.ValidateToken(request.IdToken, tokenValidationParameters, out SecurityToken _);

            return true;
        }
        catch (SecurityTokenExpiredException)
        {
            return await request.RefreshToken.MatchAsync(
                Some: async refreshToken =>
                {
                    SignInRefreshTokenDto signInRefreshTokenDto = new(request.ClientId, refreshToken);

                    await sender.Send(new SignInRefreshTokenCommand(signInRefreshTokenDto), cancellationToken);

                    return true;
                },
                None: () => false
            );
        }
        catch (Exception)
        {
            return false;
        }
    }
}