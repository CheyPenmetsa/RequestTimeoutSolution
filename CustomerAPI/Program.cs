using Microsoft.AspNetCore.Http.Timeouts;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseRequestTimeouts();
app.MapControllers();

app.Run();
