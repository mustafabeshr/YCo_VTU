using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.RechargePrc.Ops
{
    public partial class Operations
    {
        public void Login(string user, string password)
        {
            #region Local test
            SharedParams.Token = "local access token";
            SharedParams.TokenTimeLeft = 4000;
            return;
            #endregion
        }
    }
}
