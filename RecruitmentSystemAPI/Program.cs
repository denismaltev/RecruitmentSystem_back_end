using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Services;

namespace RecruitmentSystemAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var dbContext = serviceProvider.GetService<RecruitmentSystemContext>();
                    var userManager = serviceProvider.GetRequiredService<UserManager<SystemUser>>();
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    Seeder.Initialize(dbContext, userManager, roleManager);
                }
                catch
                {
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
        }
    }
}
