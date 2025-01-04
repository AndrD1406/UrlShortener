using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.DataAccess;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.BusinessLogic.Services
{
    public class UrlRepository : IUrlRepository
    {
        private readonly UrlShortenerDbContext context;

        public UrlRepository(UrlShortenerDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Url>> GetAll(Guid userId)
        {
            return await context.Urls.Where(x => x.UserId == userId).ToListAsync();
        }
        public async Task<Url> Create(Url url)
        {
            await context.Urls.AddAsync(url).ConfigureAwait(false);
            await context.SaveChangesAsync();
            return url;
        }

        public async Task<bool> Delete(Guid id)
        {
            var toDelete = await context.Urls.FirstOrDefaultAsync(x => x.Id == id);
            context.Urls.Remove(toDelete);
            await context.SaveChangesAsync();

            return toDelete != null;
        }

        public async Task<Url> GetById(Guid id)
        {
            return await context.Urls.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Url url)
        {
            await context.Urls.FirstOrDefaultAsync(x => x == url);
            await context.SaveChangesAsync();
        }
    }
}
