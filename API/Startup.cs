using API.Config;
using API.Services;
using CoreDomain;
using Infrastructure;
using Infrastructure.Implementation.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"));
            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRouting();

            services.AddSingleton(provider =>
            {
                var setting = new MongoConnectionSetting();
                Configuration.GetSection("MongoConnectionSetting").Bind(setting);
                return new MongoClient(setting.ConnectionUri).GetDatabase(setting.Database);
            });

            services.AddSingleton<IUriMappingRepository, UriMappingRepository>();
            services.AddSingleton<ISequenceGenerator, MongoSequenceGenerator>();
            services.AddSingleton<ApiService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var commentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                c.IncludeXmlComments(commentsFile);
                c.IncludeXmlComments(commentsFile);
            });
        }
    }
}