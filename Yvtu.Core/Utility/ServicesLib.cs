using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Utility
{
    public static class ServicesLib
    {
        public static string ConvertToTimeFormatter(int value)
        {
            TimeSpan time = TimeSpan.FromSeconds(value);
            return time.ToString(@"hh\:mm\:ss");
        }
    }
}
