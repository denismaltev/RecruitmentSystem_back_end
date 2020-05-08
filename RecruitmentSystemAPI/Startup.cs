using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Repositories;
using RecruitmentSystemAPI.Services;

namespace RecruitmentSystemAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RecruitmentSystemContext>(options => options.UseSqlite(Configuration.GetConnectionString("RecruitmentSystemConnection")));
            services.AddDefaultIdentity<SystemUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<RecruitmentSystemContext>();

            services.AddScoped<CompanyRepo>();
            services.AddScoped<JobRepo>();
            services.AddScoped<LabourerJobsRepo>();
            services.AddScoped<LabourerRepo>();
            services.AddScoped<SkillsRepo>();
            services.AddScoped<SystemUserRepo>();
            services.AddScoped<IncidentReportsRepo>();

            services.AddHostedService<ScheduledJob>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Recruitment System API",
                    Description = "British Columbia Institute of Technology Industry Project",
                    TermsOfService = new Uri("https://www.bcit.ca/study/programs/699ccertt"),
                    Contact = new OpenApiContact
                    {
                        Name = "Anna Berman, Denis Maltev, Gina Carpenter, Sara Banaeirad, and Tasnuva Haque.",
                        Email = string.Empty,
                        Url = new Uri("https://www.bcit.ca/study/programs/699ccertt#staff"),
                    }
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseSwagger(); app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recruitment System API V1"); c.RoutePrefix = string.Empty;
            });
            app.UseMvc();
        }
    }
}
