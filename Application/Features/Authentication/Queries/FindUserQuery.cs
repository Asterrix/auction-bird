using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using LanguageExt;
using MediatR;

namespace Application.Features.Authentication.Queries;

public record FindUserQuery(string Username) : IRequest<Option<UserType>>;

public sealed class FindUserQueryHandler : IRequestHandler<FindUserQuery, Option<UserType>>
{
    private readonly IAmazonCognitoIdentityProvider _cognitoService;

    public FindUserQueryHandler(IAmazonCognitoIdentityProvider cognitoService)
    {
        _cognitoService = cognitoService;
    }

    public async Task<Option<UserType>> Handle(FindUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            AdminGetUserRequest getUserRequest = new()
            {
                Username = request.Username,
                UserPoolId = CognitoConstant.PoolId
            };

            AdminGetUserResponse response = await _cognitoService.AdminGetUserAsync(getUserRequest, cancellationToken);

            UserType user = new()
            {
                Username = response.Username,
                Attributes = response.UserAttributes,
            };

            return user;
        }
        catch (UserNotFoundException)
        {
            return Option<UserType>.None;
        }
    }
}