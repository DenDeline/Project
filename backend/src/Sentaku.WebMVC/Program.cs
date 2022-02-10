using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.Infrastructure.Data;

namespace Project.WebMVC
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var host = CreateHostBuilder(args).Build();

      using (var services = host.Services.CreateScope())
      {
        var mock = services.ServiceProvider.GetRequiredService<MockDatabase>();
        await mock.MockLanguagesAsync();
        await mock.MockRolesAsync();
        await mock.MockUsersAsync();
      }

      await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
