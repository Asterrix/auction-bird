using Application.Features.Authentication.Commands.Mapper;
using Application.Features.Authentication.Commands.SignIn;
using Application.Features.Authentication.Commands.SignUp;
using Carter;
using MediatR;

namespace Presentation.Endpoints;

public sealed class AuthenticationModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("signin", SignIn);
        app.MapPost("signup", SignUp);
    }

    private static async Task<bool> SignIn(ISender sender, HttpRequest httpRequest)
    {
        IFormCollection form = await httpRequest.ReadFormAsync();
        SignInDto signInDto = form.MapToSignInDto();

        return await sender.Send(new SignInCommand(signInDto));
    }

    private static async Task SignUp(ISender sender, HttpRequest httpRequest)
    {
        IFormCollection form = await httpRequest.ReadFormAsync();
        SignUpDto signUpDto = form.MapToSignUpDto();

        await sender.Send(new SignUpCommand(signUpDto));
    }
}