using Application.Interfaces.Application;
using Application.Services;
using Domain.DTOs;
using Domain.Entities.Collaterals;
using Domain.Entities.EmploymentAnalysis;
using Domain.Entities.FamilyAndFriends;
using Domain.Entities.Payment;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class Extension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDemoService<DemoDTO>, DemoService>();
            services.AddScoped<IProductService<Product>, ProductService>();
            services.AddScoped<ICollacteralService<Collateral>, CollacteralService>();
            services.AddScoped<IFamilyAndFriendService<PersonInfo>, FamilyAndFriendService>();
            services.AddScoped<IEmploymentService<EmploymentInfo>, EmploymentService>();
        }
    }
}