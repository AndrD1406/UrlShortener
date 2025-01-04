using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UrlShortener.BusinessLogic.Services;
using UrlShortener.BusinessLogic.Util;
using UrlShortener.DataAccess;
using UrlShortener.DataAccess.Models;

var builder = WebApplication.CreateBuilder(args);

// The AppSettings with data needed for Tokens
IConfigurationSection appSettings = builder.Configuration.GetSection("AppSettings");

// Add services to the container.
builder.Services.AddControllers();
var serverVersion = new MySqlServerVersion("8.0.39");
builder.Services.AddDbContext<UrlShortenerDbContext>(
    options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion,
    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null)));

builder.Services.AddIdentityApiEndpoints<User>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<UrlShortenerDbContext>()
.AddDefaultTokenProviders()
.AddUserStore<UserStore<User, IdentityRole<Guid>, UrlShortenerDbContext, Guid>>()
.AddRoleStore<RoleStore<IdentityRole<Guid>, UrlShortenerDbContext, Guid>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the Bearer Authorization: `Bearer Generated-JWT-Token`",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { 
        new OpenApiSecurityScheme
        {
            Reference =  new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        },
        new string[]{}
    }});
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddTransient<IUrlService, UrlService>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
        .WithOrigins("https://localhost:4200", "https://localhost:4300")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appSettings["Issuer"],
        ValidAudience = appSettings["Audience"],
        
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettings["Key"]!)),
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors();
app.UseHsts();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
