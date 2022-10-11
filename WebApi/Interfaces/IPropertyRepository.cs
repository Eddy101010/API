using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetPropertiesAsync(int sellRent);

        void AddProperty(Property property);
        void DeleteProperty(int id); 
    }
}
