using Microsoft.AspNetCore.Http;

namespace Application.Features.Authentication.Commands.Mapper;

public record SignUpDto(
    string ClientId,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    bool TermsAndConditionsAccepted,
    bool IsOver18);

public static class AuthMapper
{
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
}