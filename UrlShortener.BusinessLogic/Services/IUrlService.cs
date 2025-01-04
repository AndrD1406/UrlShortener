using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.BusinessLogic.Services
{
    public interface IUrlService
    {
        Task<IEnumerable<Url>> GetAll(Guid userId);

        Task<Url> GetById(Guid id);

        Task<Url> Create(Url url);

        Task Update(Url url);

        Task<bool> Delete(Guid id);

        string GenerateShortUrl(string longUrl);
    }
}
