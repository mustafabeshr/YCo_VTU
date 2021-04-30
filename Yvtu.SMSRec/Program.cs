using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.SMSRec.logger;

namespace Yvtu.SMSRec
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = Host.CreateDefaultBuilder()
             .ConfigureAppConfiguration((context, builder) =>
             {
                 // Add other configuration files...
                 builder.AddJsonFile("appsettings.json", optional: true);
             })
             .ConfigureServices((context, services) =>
             {
                 ConfigureServices(context.Configuration, services);
             })
             .ConfigureLogging(logging =>
             {
                 // Add other loggers...
             }).Build();

            var services = host.Services;
            var mainForm = services.GetRequiredService<frm_Parent>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(IConfiguration configuration,
            IServiceCollection services)
        {
            services.AddSingleton<IAppDbContext, AppDbContext>();
            services.AddScoped<IPartnerManager, PartnerManager>();
            services.AddScoped<IPartnerActivityRepo, PartnerActivityRepo>();
            services.AddLogging(option =>
            {
                option.SetMinimumLevel(LogLevel.Information);
                option.AddNLog("nlog.config");
            });
            services.AddSingleton<frm_Parent>();
            services.AddScoped<frm_interface>();
        }
        
    }
}
