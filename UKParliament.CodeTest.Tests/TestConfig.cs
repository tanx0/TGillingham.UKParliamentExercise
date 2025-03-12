using Microsoft.Extensions.Configuration;


namespace UKParliament.CodeTest.Tests
{


    public static class TestConfig
    {
        private static IConfigurationRoot GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();
        }

        public static string BaseUrl => GetConfiguration().GetSection("TestingConfig:BaseUrl").Value;

        public static string Headless => GetConfiguration().GetSection("TestingConfig:Headless").Value;
    }

}
