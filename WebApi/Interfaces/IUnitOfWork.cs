using WebAPI.Interfaces;

namespace WebApi.Interfaces
{
    public interface IUnitOfWork
    {
        ICityRepository CityRepository { get; }
        IUserRepository UserRepository { get; } 
        IPropertyRepository PropertyRepository { get; } 
        IPropertyTypeRepository PropertyTypeRepository { get; }
        IFurnishingTypeRepository FurnishingTypeRepository { get; }
        Task<bool> SaveAsync();     
    }
}
