

using UrlShortener.DataAccess.Models;

namespace UrlShortener.BusinessLogic.Services
{
    public interface IUrlRepository
    {
        Task<IEnumerable<Url>> GetAll(Guid userId);

        Task<Url> GetById(Guid id);

        Task<Url> Create(Url url);

        Task Update(Url url);

        Task<bool> Delete(Guid id);

    }
}
