using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace demo_web.HealthChecks
{
    public class StartupHealthCheck : IHealthCheck
    {
        private static DateTime? _firstCall = null;
        
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            _firstCall ??= DateTime.UtcNow;

            var age = DateTime.UtcNow - _firstCall.Value;
            if (age < TimeSpan.FromSeconds(30))
                return Task.FromResult(HealthCheckResult.Unhealthy("Still starting"));

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}