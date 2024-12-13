using IdentityService.Api.Data;
using IdentityService.Api.Models;
using IdentityService.Api.Service;
using IdentityService.Api.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<IdentityServiceDbContext>(optiion =>
{
    optiion.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection"));
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("AppSettings:JwtOptions"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                          .AddEntityFrameworkStores<IdentityServiceDbContext>()
                          .AddDefaultTokenProviders();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API");
        c.RoutePrefix = string.Empty;
    });
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplayMigration();
app.Run();

void ApplayMigration()
{
    using var scope = app.Services.CreateScope();
    var _db = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
    if (_db.Database.GetPendingMigrations().Any())
    {
        _db.Database.Migrate();
    }
}
