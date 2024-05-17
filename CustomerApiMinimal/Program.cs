using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromMilliseconds(1500)
    };
    options.AddPolicy("threesecondpolicy", TimeSpan.FromSeconds(3));
    options.AddPolicy("customstatuscode", new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(3),
        TimeoutStatusCode = 503
    });
    options.AddPolicy("customdelegatepolicy", new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(3),
        TimeoutStatusCode = 503,
        WriteTimeoutResponse = async (HttpContext context) => {
            context.Response.ContentType = "application/json";
            var errorResponse = new
            {
                error = "Request time out from custome delegate policy",
                status = 503
            };
            var jsonResponse = JsonConvert.SerializeObject(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer Minimal API", Description = "Perform actions on Customer", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Minimal API V1");
});

app.MapGet("/defaultTimeout", async (HttpContext context) =>
{
    await Task.CompletedTask;
    return Results.Ok();
});

app.MapGet("/twosecondtimeout/{waitSeconds:int}", async ([FromRoute] int waitSeconds, HttpContext context) =>
{
    await Task.Delay(TimeSpan.FromSeconds(waitSeconds), context.RequestAborted);
    return Results.Ok();
}).WithRequestTimeout(TimeSpan.FromSeconds(2));

app.MapGet("/threesecondtimeout/{waitSeconds:int}", async ([FromRoute] int waitSeconds, HttpContext context) =>
{
    await Task.Delay(TimeSpan.FromSeconds(waitSeconds), context.RequestAborted);
    return Results.Ok();
}).WithRequestTimeout("threesecondpolicy");

app.MapGet("/customstatuscode/{waitSeconds:int}", [RequestTimeout("customstatuscode")] async ([FromRoute] int waitSeconds, HttpContext context) =>
{
    await Task.Delay(TimeSpan.FromSeconds(waitSeconds), context.RequestAborted);
    return Results.Ok();
});

app.MapGet("/customdelegatepolicy/{waitSeconds:int}", async ([FromRoute] int waitSeconds, HttpContext context) =>
{
    await Task.Delay(TimeSpan.FromSeconds(waitSeconds), context.RequestAborted);
    return Results.Ok();
}).WithRequestTimeout("customdelegatepolicy");

app.MapGet("/disablerequestTimeout/{waitSeconds:int}", async ([FromRoute] int waitSeconds, HttpContext context) =>
{
    await Task.Delay(TimeSpan.FromSeconds(waitSeconds), context.RequestAborted);
    return Results.Ok();
}).DisableRequestTimeout();

app.Run();
