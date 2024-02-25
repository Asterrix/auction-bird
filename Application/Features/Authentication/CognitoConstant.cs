namespace Application.Features.Authentication;

public static class CognitoConstant
{
    public const string DefaultGroupName = "User"; // Default group name to which the user will be added.
    
    public static readonly string PoolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID") 
                                           ?? throw new InvalidOperationException("COGNITO_USER_POOL_ID is not set");
    
}