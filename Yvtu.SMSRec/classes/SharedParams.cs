using System;
using System.Collections.Generic;

namespace Yvtu.SMSRec
{
    public class RMConstants
    {
        public static class RequestStatus
        {
            public const string PENDING = "pending";
            public const string CLOSED = "closed";
            public const string FAILED = "failed";
            public const string MIFOS_SUCCESS_YM_FAILED = "MifosOkYMNok";

        }
        public static class MobileOperatorPrefix
        {
            public const string YemenMobile = "77";
            public const string Sabafone = "71";
            public const string MTN = "73";
            public const string Y = "70";
        }
    }

    public class SharedParams
    {
        public static string SMSC_IP { get; set; }
        public static int Port_No { get; set; }
        public static string Account_ID { get; set; }
        public static string Password { get; set; }
        public static int Short_Code { get; set; }
        public static string Application_Status { get; set; }
        public static string Application_Stop_Message { get; set; }
        public static byte Overload_Threshold { get; set; }
        public static string OverLoad_Message { get; set; }
        public static bool CheckValidityPeriod { get; set; }
        public static string Environment_Name { get; set; }
        public static string Interfaces_Path { get; set; }
        public static byte Interfaces_MaxCount { get; set; }
        public static byte ChannelsCount { get; set; }
        public static char TokensDelimiter { get; set; }

        // Valid Command Tokens
        public static string MobileNumberPattern { get; set; }
        public static string FixedNetworkPattern { get; set; }
        public static  string ElectricitySubscriberPattern { get; set; }
        public static string ElectricityCityPattern { get; set; }
        public static string ElectricityRegionPattern { get; set; }
        public static string CommandIdPattern { get; set; }
        public static string AmountPattern { get; set; }
        public static string PINCodePattern { get; set; }
        public static string BankCodePattern { get; set; }
        public static string BankAccountPattern { get; set; }
        public static string AgentIdPattern     { get; set; }
        public static string MarchantIdPattern  { get; set; }
        public static string CompanyIdPattern { get; set; }
        public static string BankSwiftPattern { get; set; }
        public static string VerificationCodePattern { get; set; }
        
        public static string DBName { get; set; }
        public static string DBServer { get; set; }
        public static string DBUser { get; set; }
        public static string DBPass { get; set; }

        public static byte WrongPasswordMaxAttempts { get; set; }
        public static string LockedDuration { get; set; }

        public static bool Load()
        {
            try
            {
                SMSC_IP = Util.GetKeyValueFromConfigFile("SMSC_IP");
                Port_No =Convert.ToInt32( Util.GetKeyValueFromConfigFile("Port_No"));
                Account_ID = Util.GetKeyValueFromConfigFile("Account_ID");
                Password = Util.GetKeyValueFromConfigFile("Password");
                Short_Code = Convert.ToInt32(Util.GetKeyValueFromConfigFile("Short_Code"));
                Application_Status = Util.GetKeyValueFromConfigFile("Application_Status");
                Application_Stop_Message = Util.GetKeyValueFromConfigFile("Application_Stop_Message");
                Overload_Threshold = byte.Parse(Util.GetKeyValueFromConfigFile("Overload_Threshold"));
                OverLoad_Message = Util.GetKeyValueFromConfigFile("OverLoad_Message");
                CheckValidityPeriod =(Util.GetKeyValueFromConfigFile("CheckValidityPeriod")=="1"? true :false);
                Environment_Name = Util.GetKeyValueFromConfigFile("Environment_Name");
                TokensDelimiter =char.Parse(Util.GetKeyValueFromConfigFile("TokensDelimiter"));
                //DBName = Utility.GetKeyValueFromConfigFile("DBName");
                //DBServer = Utility.GetKeyValueFromConfigFile("DBServer");
                //DBUser = Utility.GetKeyValueFromConfigFile("DBUser");
                //DBPass = Utility.GetKeyValueFromConfigFile("DBPass");
                //DBPass = Utility.Decrypt(Utility.GetKeyValueFromConfigFile("DBPass"),true, "YmRes@1415");
                Interfaces_Path = Util.GetKeyValueFromConfigFile("Interfaces_Path");
                ChannelsCount = byte.Parse(Util.GetKeyValueFromConfigFile("ChannelsCount"));
                MobileNumberPattern= Util.GetKeyValueFromConfigFile("MobileNumberPattern");
                
                AmountPattern = Util.GetKeyValueFromConfigFile("AmountPattern");
                PINCodePattern = Util.GetKeyValueFromConfigFile("PINCodePattern");
                
                WrongPasswordMaxAttempts = byte.Parse(Util.GetKeyValueFromConfigFile("WrongPasswordMaxAttempts"));
                LockedDuration = Util.GetKeyValueFromConfigFile("LockedDuration");
                if (ChannelsCount > 20) ChannelsCount = 20;
                Interfaces_MaxCount = 0;
                return true;
            }
            catch (Exception ex)
            {
                throw(ex);
            }
        }

        public static Dictionary<byte, Interface> InterfaceDictionary;
        public static Dictionary<string, string> System_Messages;
    }

    public enum COMMANDS
    {
        CHECK_BALANCE                = 11,
        QUERY_LAST_3_TRANSACTIONS    = 12,
        INFO_COMMAND                 = 13,
        INFO_COMMISSIONS             = 14,
        TO_SUBSCRIBER                = 21,
        TO_BANK_ACCOUNT              = 22,
        TO_MONEY_AGENT               = 23,
        DEPOSIT                      = 24,
        TOPUP_TO_MOBILE              = 31,
        PAYMENT_TO_FIXED             = 32,
        PAYMENT_TO_ADSL              = 33,
        PAYMENT_TO_INTERNATIONAL     = 34,
        TOPUP_TO_OTHERS              = 52,
        PAYMENTS_ELECTRICITY         = 41,
        PAYMENTS_WATER               = 42,
        PAYMENTS_MERCHANT            = 51,
        ACCOUNT_ACTIVATION           = 65,
        CHANGE_PIN                   = 61,
        PINCODE_RESET                = 62,
        PAUSE_CLIENT                 = 63,
        REACTIVE_CLIENT              = 64,
        PAYMENTS_UTILITIES           = 90,
        VERIFICATION_CODE            = 99
    }
}
