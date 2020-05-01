using Boozfinder.Providers;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Boozfinder.Services;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos;
using Boozfinder.Services.Interfaces;

namespace Boozfinder
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
            services.AddSingleton<IImageService>(InitializeImageClientInstance(Configuration.GetSection("BlobStorage")));
            services.AddMemoryCache();
            services.AddScoped<ICacheProvider, CacheProvider>();
            services.AddScoped<IUserProvider, UserProvider>();
            services.AddScoped<IBoozeProvider, BoozeProvider>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Boozfinder API",
                    //Description = "A simple API for Boozfinder",
                    //TermsOfService = new Uri("https://example.com/terms"),
                    //Contact = new OpenApiContact
                    //{
                    //    Name = "My name",
                    //    Email = string.Empty,
                    //    Url = new Uri("https://twitter.com/example"),
                    //},
                    //License = new OpenApiLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = new Uri("https://example.com/license"),
                    //}
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Boozfinder API v1");
            });
        }

        /// <summary>
        /// Creates a Cosmos DB database and a container with the specified partition key. 
        /// </summary>
        /// <returns></returns>
        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
            CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
            return cosmosDbService;
        }

        private static ImageService InitializeImageClientInstance(IConfigurationSection configurationSection)
        {
            string connectionString = configurationSection.GetSection("ConnectionString").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            return new ImageService(connectionString, containerName);
        }
    }
}
