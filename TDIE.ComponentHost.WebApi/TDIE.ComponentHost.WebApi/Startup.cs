using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using TDIE.ComponentHost.Core;
using OnTrac.Utilities.Mappers;
using OnTrac.Utilities.Mappers.Core;
using TDIE.PackageManager.Core;
using TDIE.PackageManager.Basic;

namespace TDIE.ComponentHost.WebApi
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
            services.AddOptions()
                    .AddSwaggerGen(config =>
                    {
                        config.SwaggerDoc("v1", new OpenApiInfo { Title = "ComponentHost Web API", Version = "v1" });
                        config.DescribeAllEnumsAsStrings();
                    })
                    .AddScoped<IPackageManager, BasicManager>()
                    .AddScoped(typeof(Process), x => Process.GetCurrentProcess())
                    .AddSingleton<IComponentHostBackgroundService, ComponentHostBackgroundService>()
                    .AddSingleton<IObjectMapperService, ObjectMapperService>()
                    .AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });

            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = Configuration["IdentityServer:Server"];
                        options.RequireHttpsMetadata = false;

                        options.ApiName = Configuration["IdentityServer:ApiName"];
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();

            // use swagger to show UI describing our API -- this is OK as this is only meant for internal use
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", "ComponentHost Web API");
               });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseHttpsRedirection();
            app.UseMvc();            
        }
    }
}
