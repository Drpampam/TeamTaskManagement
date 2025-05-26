﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence.Confgurations;
using Persistence.Extensions.MongoDbSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Extensions.Persistence
{
    public static class Extension
    {
        public static void AddPersistence(this IServiceCollection services,  IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton(provider => provider.GetRequiredService<IOptions<Settings>>().Value);

        }
    }
}
