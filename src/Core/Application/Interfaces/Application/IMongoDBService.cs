using Domain.Entities.Payment;

namespace Application.Interfaces.Application
{
    public interface IMongoDBService
    {
        Task<Product> CreateAsync(Product product);
    }
}