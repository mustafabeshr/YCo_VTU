using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Yvtu.Sender
{
    public class Configurations
    {
        [Description("SMSC IP address")]
        [Category("General")]
        public string Server { get; set; }
        [Category("General")]
        [Description("Port no")]
        public int Port { get; set; }
        [Category("General")]
        [Description("Short code with country code like 9672000")]
        public string ShortCode { get; set; }
        [Category("General")]
        [Description("SMSC user id")]
        public string SystemId { get; set; }
        [Category("General")]
        [Description("SMSC password")]
        public string Password { get; set; }
        [Category("Application")]
        [Description("How many instances you want to bind with SMSC")]
        public short InstancesCount { get; set; }
        [Category("Application")]
        [Description("Bind type to SMSC (receiver,sender or both")]
        public string ConnectionMode { get; set; }
        [Category("Application")]
        [Description("Application status active or deactive")]
        public string ApplicationStatus { get; set; }
        [Category("Application")]
        [Description("The message replay to sender when application is deactive")]
        public string ApplicationStopMessage { get; set; }
        [Category("Application")]
        [Description("Maximum requests in queue")]
        public short OverloadThreshold { get; set; }
        [Category("Application")]
        [Description("The message replay to sender when queue is full")]
        public string OverLoadMessage { get; set; }
        [Category("Application")]
        public short CheckValidityPeriod { get; set; }
        [Category("Application")]
        [Description("The environment which the application run on")]
        public string Environment { get; set; }
        [Category("Application")]
        public short MaxChannels { get; set; }
        [Category("Sender")]
        [Description("How many message will be sent per a second")]
        public short SendSpeed { get; set; }
        [Category("Sender")]
        [Description("The table name which store messages that should send")]

        public string SenderSource { get; set; }
        public Configurations(string server, int port, string shortcode, string systemid, string password
            , short instancescount, string connectionmode, string applicationstatus, string applicationstopmessage
            , short overloadthreshold, string overLoadmessage, short checkvalidityperiod, string environment
            , short maxchannels, short sendspeed, string sendersource)
        {
            Server = server;
            Port = port;
            ShortCode = shortcode;
            SystemId = systemid;
            Password = password;
            InstancesCount = instancescount;
            ConnectionMode = connectionmode;
            ApplicationStatus = applicationstatus;
            ApplicationStopMessage = applicationstopmessage;
            OverloadThreshold = overloadthreshold;
            OverLoadMessage = overLoadmessage;
            CheckValidityPeriod = checkvalidityperiod;
            Environment = environment;
            MaxChannels = maxchannels;
            SendSpeed = sendspeed;
            SenderSource = sendersource;
        }
    }
}
