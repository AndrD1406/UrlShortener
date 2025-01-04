using AutoMapper;
using UrlShortener.BusinessLogic.Models;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.BusinessLogic.Util;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<UrlDto, Url>();

        CreateMap<Url, UrlDto>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User != null
                ? new UserDto
                {
                    Id = src.User.Id.ToString(),
                    FirstName = src.User.FirstName,
                    LastName = src.User.LastName
                }
                : null));
    }
}
