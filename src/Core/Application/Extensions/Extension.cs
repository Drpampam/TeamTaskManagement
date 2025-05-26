using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskManagement.API.Services;

namespace Application.Extensions
{
    public static class Extension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IJwtService, JwtService>();
        }
    }
}