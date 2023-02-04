using AutoMapper;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Requests.Models;

namespace CoffeeExchange.Mapper;
 
public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<AuthenticationRequest, User>()
            .ForMember(member => member.PasswordHash, 
                configuration => configuration.MapFrom(src => src.Password));
        
        CreateMap<RegistrationWithRoleRequest, User>()
            .ForMember(member => member.PasswordHash, 
                configuration => configuration.MapFrom(src => src.Password));
    }
}