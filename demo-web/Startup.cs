using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_web.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace demo_web
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
            services.AddHealthChecks()
                .AddCheck<StartupHealthCheck>("startup_check", tags: new[] {"Startup"})
                .AddCheck<RandomFailingCheck>("readiness", tags: new[] {"Readiness"})
                .AddCheck<RandomFailingCheck>("liveliness", tags: new[] {"Liveliness"});
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/startup", new HealthCheckOptions {Predicate = check => check.Tags.Contains("Startup")});
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions {Predicate = check => check.Tags.Contains("Liveliness")});
                endpoints.MapHealthChecks("/ready", new HealthCheckOptions {Predicate = check => check.Tags.Contains("Readiness")});
                endpoints.MapControllers();
            });
        }
    }
}
