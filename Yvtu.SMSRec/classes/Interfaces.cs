using System;

namespace Yvtu.SMSRec
{
    [Serializable]
    public class Interface
    {
        public bool Auto_Start { get; set; }
        public bool Auto_Sending { get; set; }
        public byte Interface_No { get; set; }
        public string Bind_Type { get; set; }
        public string SendingData_Table { get; set; }
        public bool Braodcast_Queueing { get; set; }
        public string Braodcast_Table { get; set; }
        public byte Channel { get; set; }
        public byte Sending_Speed { get; set; }
        public bool Sending_Daily_Report { get; set; }
        public string Sending_Daily_Report_Time { get; set; }
        public string Daily_Report_Lang { get; set; }
        public string Status { get; set; }
    }

     
}
