using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using TDIE.NodeApi.Data;
using TDIE.NodeApi.Data.Entities;
using TDIE.NodeApi.Data.LiteDb;
using TDIE.NodeApi.Models;
using TDIE.NodeApi.ProcessManagement;
using TDIE.PackageManager.Basic;
using TDIE.PackageManager.Core;
using OnTrac.Utilities.Mappers;
using OnTrac.Utilities.Mappers.Core;

namespace TDIE.NodeApi
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
                        config.SwaggerDoc("v1", new OpenApiInfo { Title = "Node API", Version = "v1" });
                        config.DescribeAllEnumsAsStrings();
                    })
                    .AddScoped<IPackageManager, BasicManager>()                    
                    .AddSingleton<IObjectMapperService, ObjectMapperService>()
                    .AddSingleton<IProcessManager, ProcessManager>()
                    .AddSingleton<IProcessStoreAccess, LiteDbProcessStoreAccess>(x => new LiteDbProcessStoreAccess(Configuration["Configuration:ProcessStore:Path"]))
                    .AddHostedService<ProcessStoreSystemSyncBackgroundService>()
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IObjectMapperService objectMapperService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            // use swagger to show UI describing our API -- this is OK as this is only meant for internal use
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Node API v1");
               });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
