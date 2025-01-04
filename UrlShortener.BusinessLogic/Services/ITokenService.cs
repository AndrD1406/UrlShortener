using PolynomSolver.DataAccess.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.BusinessLogic.Services
{
    public interface ITokenService
    {
        AuthenticationResponse CreateJwtToken(User user);

        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
