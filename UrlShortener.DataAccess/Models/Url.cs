using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.DataAccess.Models
{
    public class Url
    {
        [Key]
        public Guid Id { get; set; }
        public string LongUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public User? User { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
    }
}
