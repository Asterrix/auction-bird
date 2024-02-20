using Microsoft.AspNetCore.Http;

namespace Application.Features.Authentication.Commands.Mapper;

public record SignInDto(
    string ClientId,
    string Email,
    string Password);

public record SignUpDto(
    string ClientId,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    bool TermsAndConditionsAccepted,
    bool IsOver18);

public record SignOutDto(string Username);

public static class AuthMapper
{
    public static SignInDto MapToSignInDto(this IFormCollection form)
    {
        return new SignInDto(
            form["clientId"],
            form["email"],
            form["password"]
        );
    }

    public static SignUpDto MapToSignUpDto(this IFormCollection form)
    {
        return new SignUpDto(
            form["clientId"],
            form["firstName"],
            form["lastName"],
            form["email"],
            form["password"],
            bool.Parse(form["termsAndConditions"]),
            bool.Parse(form["isOver18"])
        );
    }
    
    public static SignOutDto MapToSignOutDto(this IFormCollection form)
    {
        return new SignOutDto(
            form["username"]
        );
    }
}