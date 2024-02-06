using Application;
using Carter;
using Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddApplicationLayer();

builder.Services.AddInfrastructureLayer();
builder.Host.AddInfrastructureLayer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapCarter();
app.UseSerilogRequestLogging();
app.Run();
