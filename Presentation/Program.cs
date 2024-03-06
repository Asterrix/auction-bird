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
    options.AddPolicy("ClientPolicy",
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
app.UseMiddleware<TokenMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseMiddleware<FirebaseStorageExceptionMiddleware>();
app.UseMiddleware<NotFoundExceptionMiddleware>();
app.MapCarter();
app.UseSerilogRequestLogging();

app.Run();