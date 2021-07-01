using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace demo_web.HealthChecks
{
    public class RandomFailingCheck : IHealthCheck
    {
        private static readonly Random _rng = new Random();
        
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = _rng.Next(10) == 0
                ? HealthCheckResult.Unhealthy("Failed random")
                : HealthCheckResult.Healthy();

            return Task.FromResult(result);
        }
    }
}