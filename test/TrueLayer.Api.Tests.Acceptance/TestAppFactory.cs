using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TrueLayer.Api.Tests.Acceptance
{
    public class TestAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var builderConfigured = builder.UseEnvironment("Test");
            
            base.ConfigureWebHost(builderConfigured);
        }
    }
}