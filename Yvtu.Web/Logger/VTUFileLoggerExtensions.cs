using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Yvtu.Web.Logger
{
    public static class VTUFileLoggerExtensions
    {
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<VTUFileLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, VTUFileLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
