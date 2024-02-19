using Application;
using Carter;
using Infrastructure;
using Presentation.Middleware;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddApplicationLayer();

builder.Services.AddInfrastructureLayer();
builder.Host.AddInfrastructureLayer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ClientPolicy",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ClientPolicy");
app.UseHttpsRedirection();
app.UseMiddleware<AmazonExceptionMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.MapCarter();
app.UseSerilogRequestLogging();

app.Run();
