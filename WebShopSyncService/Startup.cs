using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delos.Contexts;
using Delos.Model;
using Delos.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Model;

namespace WebShopSyncService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var syncServices = Configuration.GetSection("Services").Get<List<ImportServiceConfig>>();
            if (syncServices != null)
            {
                foreach (var s in syncServices)
                {
                    string objectToInstantiate = s.Implementation + ", WebShopSyncService";
                    var objectType = Type.GetType(objectToInstantiate);
                    try
                    {
                        var serviceInstance = Activator.CreateInstance(objectType) as IImportService;
                        serviceInstance.Config = s;

                        var hostService = new ImportHostService(serviceInstance, new DelosDbContext(Configuration.GetConnectionString("Delos")));
                        hostService.StartAsync(new System.Threading.CancellationToken());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            var exportServices = Configuration.GetSection("ExportServices").Get<List<ExportServiceConfig>>();
            if (exportServices != null)
            {
                foreach (var s in exportServices)
                {
                    string objectToInstantiate = s.Implementation + ", WebShopSyncService";
                    var objectType = Type.GetType(objectToInstantiate);
                    try
                    {
                        var serviceInstance = Activator.CreateInstance(objectType) as IExportService;
                        serviceInstance.Config = s;
                        serviceInstance.ConnectionString = Configuration.GetConnectionString("Delos");

                        var hostService = new ExportHostService(serviceInstance);
                        hostService.StartAsync(new System.Threading.CancellationToken());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
