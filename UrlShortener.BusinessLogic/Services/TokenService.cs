using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PolynomSolver.DataAccess.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.BusinessLogic.Services;

public class TokenService: ITokenService
{
    private readonly IConfiguration configuration;
    private readonly IMapper mapper;

    public TokenService(IConfiguration configuration, IMapper mapper)
    {
        this.configuration = configuration;
        this.mapper = mapper;
    }

    public AuthenticationResponse CreateJwtToken(User user)
    {
        DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["AppSettings:EXPIRATION_MINUTES"]));


        Claim[] claims = new Claim[]
            {
					//JwtRegisteredClaimNames.Sub - user identity
					new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
					//JwtRegisteredClaimNames.Jti - unique id for the token
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					//JwtRegisteredClaimNames.Iat - issued at
					//couldn't authorize if iat is string it must be int
					new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
					// further fields are optional
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Surname , user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
            };

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Key"]!));

        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokenGenerator = new JwtSecurityToken(
            configuration["AppSettings:Issuer"],
            configuration["AppSettings:Audience"],
            claims,
            expires: expiration,
            signingCredentials: signingCredentials
            );

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        string token = null;

        //possible error here if key size is less than 256 bytes 
        token = tokenHandler.WriteToken(tokenGenerator);

        Console.WriteLine(Convert.ToInt32(configuration["AppSettings:EXPIRATION_MINUTES"]));

        return new AuthenticationResponse()
        {
            Token = token,
            Email = user.Email,
            FirstName = user.LastName,
            Expiration = expiration,
            RefreshToken = GenerateRefreshToken(),
            //*here I changed Utc to UtcNow
            RefreshTokenExpirationDateTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["AppSettings:EXPIRATION_MINUTES"]))
        };
    }

    public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true,
            ValidAudience = configuration["AppSettings:Audience"],
            ValidateIssuer = true,
            ValidIssuer = configuration["AppSettings:Issuer"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Appsettings:Key"]!)),

            //method called when token is expired
            ValidateLifetime = false,
        };

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        try
        {
            ClaimsPrincipal principal1 = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken1);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }

        ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    //Creates a refresh token (base 64 string of random numbers)
    private string GenerateRefreshToken()
    {
        byte[] bytes = new byte[64];
        var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
