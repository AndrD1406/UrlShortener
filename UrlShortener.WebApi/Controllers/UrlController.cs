using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.BusinessLogic.Models;
using UrlShortener.BusinessLogic.Services;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UrlController : ControllerBase
{
    private readonly IUrlService urlService;
    private readonly IMapper mapper;

    public UrlController(IUrlService urlService, IMapper mapper)
    {
        this.urlService = urlService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authorized.");
        }

        var dtos = await this.urlService.GetAll(Guid.Parse(userId));

        var result = dtos.Select(item => mapper.Map<UrlDto>(item)).ToList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var url = await urlService.GetById(Id);

        var result = mapper.Map<UrlDto>(url);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UrlCreateDto dto)
    {
        string longUrl = dto.LongUrl;
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            return BadRequest("Long URL cannot be null or empty.");
        }

        var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(stringUserId))
        {
            return Unauthorized("User not authorized.");
        }

        Guid userId = Guid.Parse(stringUserId);

        var shortUrl = urlService.GenerateShortUrl(longUrl);

        var url = new Url
        {
            Id = Guid.NewGuid(),
            LongUrl = longUrl,
            ShortUrl = shortUrl,
            UserId = userId,
            CreatedDate = DateTime.Now
        };

        var createdUrl = await urlService.Create(url);

        var result = mapper.Map<UrlDto>(createdUrl);

        return CreatedAtAction(nameof(GetById), new { Id = createdUrl.Id }, result);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UrlDto urlDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

        var existingUrl = await urlService.GetById(id);
        if (existingUrl == null || existingUrl.UserId != userId)
        {
            return NotFound("Url not found or access denied.");
        }

        var updatedUrl = mapper.Map(urlDto, existingUrl);
        await urlService.Update(updatedUrl);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

        var existingUrl = await urlService.GetById(id);
        if (existingUrl == null || existingUrl.UserId != userId)
        {
            return NotFound("Url not found or access denied.");
        }

        await urlService.Delete(id);

        return NoContent();
    }
}
