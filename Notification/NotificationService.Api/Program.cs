using LoggingService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotificationService.Api.Consumers;
using NotificationService.Api.Data;
using NotificationService.Api.Extensions;
using NotificationService.Api.Services;
using NotificationService.Api.Services.IServices;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("LogDatabase");
builder.Services.AddSerilogLogging(connectionString);

builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddSingleton<ISmsService, SmsService>();
builder.Services.AddScoped<INotification, Notification>();
builder.Services.AddDbContext<NotificationDbContext>(optiion =>
{
    optiion.UseSqlServer(builder.Configuration.GetConnectionString("NotificationConnection"));
});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHostedService<RegisterConsumer>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
            ValidAudience = builder.Configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtOptions:Secret"]))
        };
    });

builder.Services.AddAuthorizationBuilder()
                .AddPolicy("AdminPolicy", policy => policy.RequireRole("ADMIN", "USER"))
                .AddPolicy("UserPolicy", policy => policy.RequireRole("USER"));


var app = builder.Build();

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(c => c.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
ApplayMigration();
app.Run();

void ApplayMigration()
{
    using var scope = app.Services.CreateScope();
    var _context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    if (_context.Database.GetPendingMigrations().Any())
    {
        _context.Database.Migrate();
    }
}
