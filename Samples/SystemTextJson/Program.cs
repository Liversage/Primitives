using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemTextJson
{
    static class Program
    {
        static Task Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddHostedService<MainService>()
                    .AddSingleton(new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    }));
            using var host = hostBuilder.Build();
            return host.StartAsync();
        }
    }
}
