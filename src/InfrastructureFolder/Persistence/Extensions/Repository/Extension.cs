using Application.Interfaces;
using Application.Interfaces.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Confgurations;
using Persistence.Extensions.MongoDbSettings;
using Persistence.Repositories;

namespace Persistence.Extensions.Repository
{
    public static class Extension
    {
        public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IMongoDBService, MongoDBService>();
        }
    }
}