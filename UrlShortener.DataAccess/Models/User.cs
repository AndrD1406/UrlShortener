using Microsoft.AspNetCore.Identity;

namespace UrlShortener.DataAccess.Models;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpirationDateTime { get; set; }

    public virtual List<Url>? Urls { get; set;}
}
