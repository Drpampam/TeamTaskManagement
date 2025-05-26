using Application.Interfaces.Application;
using Domain.Entities.Payment;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Persistence.Extensions.MongoDbSettings
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoCollection<Product> _productCollection;

        public MongoDBService(IOptions<Settings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _productCollection = database.GetCollection<Product>(mongoDBSettings.Value.CollectionName);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productCollection.InsertOneAsync(product);
            return product;
        }
    }
}
