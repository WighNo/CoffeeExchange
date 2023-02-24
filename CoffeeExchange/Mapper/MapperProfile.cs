using AutoMapper;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Requests.Models;

namespace CoffeeExchange.Mapper;
 
/// <summary>
/// Профиль маппера данных
/// </summary>
public class MapperProfile : Profile
{
    /// <summary>
    /// Конструктор класса с инициализацией маршрутов
    /// </summary>
    public MapperProfile()
    {
        CreateMap<AuthenticationRequest, User>()
            .ForMember(member => member.PasswordHash, 
                configuration => configuration.MapFrom(src => src.Password));
        
        CreateMap<RegistrationWithRoleRequest, User>()
            .ForMember(member => member.PasswordHash, 
                configuration => configuration.MapFrom(src => src.Password));

        CreateMap<CreateProductRequest, Product>();

        CreateMap<AddCoffeeHouseRequest, CoffeeHouse>();
    }
}