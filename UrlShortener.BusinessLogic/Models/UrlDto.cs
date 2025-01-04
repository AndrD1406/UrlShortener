using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.BusinessLogic.Models
{
    public class UrlDto
    {

        public string LongUrl { get; set; } = string.Empty;

        public string ShortUrl { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public UserDto? CreatedBy { get; set; }
    }
}
