using System;
using System.Linq;
using AutoMapper;
using MELI.Challenge.API.Mappers;
using MELI.Challenge.Domain;
using MELI.Challenge.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace MELI.Desafio.API
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
            services.AddControllers();
            services.AddScoped<ICommunicationService, CommunicationService>();

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            Satellite[] satellites = Configuration.GetSection("satellites").Get<Satellite[]>();

            services.Configure<SatelliteOptions>(options => {
                options.Satellites = satellites.ToList();
            });
            AddSwagger(services);
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCache cache)
        {
            var entryOptions = new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions().
                SetPriority(CacheItemPriority.NeverRemove);

            cache.Set("kenobi", new CacheValue { Distance = "", message = new string[] { } }, entryOptions);
            cache.Set("skywalker", new CacheValue { Distance = "", message = new string[] { } }, entryOptions);
            cache.Set("sato", new CacheValue { Distance = "", message = new string[] { } }, entryOptions);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MELI Challenge API V1");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var groupName = "v1";

                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"MELI Challenge {groupName}",
                    Version = groupName,
                    Description = "MELI Challenge API",
                    Contact = new OpenApiContact
                    {
                        Name = "MELI Company",
                        Email = string.Empty,
                    }
                });
            });
        }
    }

    public class CacheValue
    {
        public string Distance { get; set; }
        public string[] message { get; set; }
    }
}
