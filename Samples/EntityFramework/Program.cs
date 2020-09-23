using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace EntityFramework
{
    public static class Program
    {
        static Task Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                .AddDbContext<Context>(options => options.UseSqlite("Data Source=Context.db"))
                .AddHostedService<MainService>());
            using var host = hostBuilder.Build();
            return host.StartAsync();
        }
    }
}
