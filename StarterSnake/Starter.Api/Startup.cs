using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starter.Api.Middleware;
using System.Text.Json.Serialization;

namespace Starter.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(o => { o.OutputFormatters.Clear(); })
                .AddNewtonsoftJson(o => { o.UseCamelCasing(processDictionaryKeys: true); })
                .AddJsonOptions(opts =>
                 {
                     opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                     opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                 });

            services.AddApplicationInsightsTelemetry(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

            services.AddTransient<RequestBodyLoggingMiddleware>();
            services.AddTransient<ResponseBodyLoggingMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            if (Configuration["APPLICATIONINSIGHTS_BODY_LOGGING"]?.
                Equals(true.ToString(), System.StringComparison.OrdinalIgnoreCase)
                    ?? false)
            {
                // Enable our custom middleware
                app.UseRequestBodyLogging();
                app.UseResponseBodyLogging();
            }
            
            app.UseHttpsRedirection();
            app.UseStatusCodePages();

            app.UseRouting();

            app.UseCors(o => { o.AllowAnyOrigin(); });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}