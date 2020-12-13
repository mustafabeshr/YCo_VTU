using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Yvtu.RechargePrc
{
    public class SharedParams
    {
        public static string Short_Code { get; set; }
        public static string Application_Status { get; set; }
        public static string Application_Title { get; set; }
        public static string Application_Stop_Message { get; set; }
        public static string OCSApiUser { get; set; }
        public static string OCSApiPassword { get; set; }
        public static string OCSEndpoint { get; set; }
        public static string CommandsList { get; set; }
        public static string GrappedRequestsCount { get; set; }
        public static string MobileNumberPattern { get; set; }
        public static string Token
        {
            get;
            set;
        }
        public static int TokenTimeLeft
        {
            get;
            set;
        }
        public static int Channel { get; set; }
        public static int redundancyPeriod { get; set; }
        public static string DailyReportSendTime { get; set; }
        public static bool SendDailyReportPermission { get; set; }
        public static int DbTimeoutSec { get; set; }
        public static bool Load()
        {
            try
            {
                Short_Code = Utility.GetKeyValueFromConfigFile("Short_Code");
                Application_Status = Utility.GetKeyValueFromConfigFile("Application_Status");
                Application_Title = Utility.GetKeyValueFromConfigFile("Application_Title");
                Application_Stop_Message = Utility.GetKeyValueFromConfigFile("Application_Stop_Message");
                OCSApiUser = Utility.GetKeyValueFromConfigFile("OCSApiUser");
                OCSApiPassword = Utility.GetKeyValueFromConfigFile("OCSApiPassword");
                OCSEndpoint = Utility.GetKeyValueFromConfigFile("OCSEndpoint");
                CommandsList = Utility.GetKeyValueFromConfigFile("CommandsList");
                GrappedRequestsCount = Utility.GetKeyValueFromConfigFile("GrappedRequestsCount");
                MobileNumberPattern = Utility.GetKeyValueFromConfigFile("MobileNumberPattern");
                redundancyPeriod = int.Parse(Utility.GetKeyValueFromConfigFile("redundancyPeriod").ToString());
                DailyReportSendTime = Utility.GetKeyValueFromConfigFile("DailyReportSendTime");
                SendDailyReportPermission = Utility.GetKeyValueFromConfigFile("SendDailyReportPermission") == "1" ? true : false;
                DbTimeoutSec = int.Parse(Utility.GetKeyValueFromConfigFile("DbTimeoutSec").ToString());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public enum COMMANDS
    {
        RECHARGE         = 1,
        MONEY_TRANSFER   = 2,
        BALANCE_QUERY    = 3
    }
    public class RechargeConstants
    {
        public static class RequestStatus
        {
            public const int PENDING = 0;
            public const int CLOSED = 1;
            public const int FAILED = 2;
        }
        public static class MobileOperatorPrefix
        {
            public const string YemenMobile = "77";
            public const string Sabafone = "71";
            public const string MTN = "73";
            public const string Y = "70";
        }
    }
}
