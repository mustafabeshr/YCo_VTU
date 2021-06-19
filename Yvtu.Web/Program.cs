using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Yvtu.Web.Logger;

namespace Yvtu.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               })
               .ConfigureLogging((hostBuilderContext, logging) =>
               {
                   logging.AddFileLogger(options =>
                   {
                       hostBuilderContext.Configuration.GetSection("Logging").GetSection("VTUWeb").GetSection("Options").Bind(options);
                   });
               });
    }
}
