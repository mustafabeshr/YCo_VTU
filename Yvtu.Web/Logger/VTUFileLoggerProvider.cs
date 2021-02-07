using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Yvtu.Web.Logger
{
    [ProviderAlias("VTUWeb")]
    public class VTUFileLoggerProvider : ILoggerProvider
    {
        public readonly VTUFileLoggerOptions Options;
        public SemaphoreSlim WriteFileLock;

        public VTUFileLoggerProvider(IOptions<VTUFileLoggerOptions> _options)
        {
            WriteFileLock = new SemaphoreSlim(1, 1);
            Options = _options.Value;

            if (!Directory.Exists(Options.FolderPath))
            {
                Directory.CreateDirectory(Options.FolderPath);
            }
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new VTUFileLogger(this);
        }

        public void Dispose()
        {
        }
    }
}
