using Application.Features.Authentication.Commands.Mapper;
using Application.Features.Authentication.Commands.SignUp;
using Carter;
using MediatR;

namespace Presentation.Endpoints;

public sealed class AuthenticationModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("signup", SignUp);
    }

    private static async Task<bool> SignUp(ISender sender, HttpRequest httpRequest)
    {
        IFormCollection form = await httpRequest.ReadFormAsync();
        SignUpDto signUpDto = form.MapToSignUpDto();
        
        return await sender.Send(new SignUpCommand(signUpDto));
    }
}