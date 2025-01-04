using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.BusinessLogic.Services
{
    public class UrlService : IUrlService
    {
        private const string BaseUrl = "https://localhost:4200";
    
        private readonly IUrlRepository repository;
        private readonly IMapper mapper;

        public UrlService(IUrlRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Url>> GetAll(Guid userId)
        {
            return await repository.GetAll(userId);
        }
        public async Task<Url> Create(Url url)
        {
            return await repository.Create(url);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await repository.Delete(id);
        }

        public async Task<Url> GetById(Guid id)
        {
            return await repository.GetById(id);
        }

        public async Task Update(Url url)
        {
            await repository.Update(url);
        }

        public string GenerateShortUrl(string longUrl)
        {
            if (string.IsNullOrEmpty(longUrl))
            {
                throw new ArgumentException("The URL cannot be null or empty.", nameof(longUrl));
            }

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(longUrl));

                var hashString = Convert.ToBase64String(hashBytes);

                var shortKey = hashString.Replace("/", "").Replace("+", "").Substring(0, 8);

                return $"{BaseUrl}{shortKey}";
            }
        }
    }
}
