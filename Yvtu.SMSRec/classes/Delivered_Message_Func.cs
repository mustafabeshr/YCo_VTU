using System.Collections.Generic;

namespace Yvtu.SMSRec
{
    partial class DeliverMessage
    {
        public enum Request_Name{Result=0}

        public struct Request_Packet
        {
            public Request_Name request_name { get; set; }
            public string Short_Code { get; set; }
            public string Message { get; set; }
            public string Mobile_no { get; set; }
            public byte Channel { get; set; }
        }

        public struct RequestReturnValue
        {
            public int Ret_ID { get; set; }
            public string Ret_Message { get; set; }
            public string Ret_Message_to_Client { get; set; }
            public bool Ret_Status {get; set;}

        }

        public class Delivered_Message
        {
            private string _short_code;
            private string _message;
            private string _lang;
            private string _mobile_no;
            private byte _channel;
            public string Short_code
            {
                get
                {
                    return _short_code;
                }

                set
                {
                    _short_code = value;
                }
            }
            public string Message
            {
                get
                {
                    return _message;
                }

                set
                {
                    _message = value;
                }
            }
            public string Lang
            {
                get
                {
                    return _lang;
                }

                set
                {
                    _lang = value;
                }
            }
            public string Mobile_No
            {
                get
                {
                    return _mobile_no;
                }

                set
                {
                    _mobile_no = value;
                }
            }

            public byte Channel
            {
                get
                {
                    return _channel;
                }

                set
                {
                    _channel = value;
                }
            }

            public Delivered_Message(string mobileno, string shortcode, string message, string lang,byte channel)
            {
                Short_code = shortcode;
                Message = message;
                Lang = lang;
                Mobile_No = mobileno;
                Channel = channel;
            }
        }

        public class DeliveryWorkerDoArgs
        {
            public Delivered_Message DMsg { get; set; }
            public RequestReturnValue RV { get; set; }
        }
        public static class WrongRequestMessags
        {
            public static string REQUEST_TOO_SHORT = "عذرا طلب غير صحيح ، يرجى التأكد من صيغة الطلب و المحاولة لاحقا";
            public static string UNKNOWN_COMMAND = "عذرا رقم الطلب غير صحيح";
            public static string WRONG_MOBILE = "رقم موبايل خاطئ يرجى التاكد و المحاولة لاحقا";
            public static string Wrong_password = "الرقم السري غير صحيح يرجى التأكد و المحاولة لاحقا";
            public static string Balance_not_enough = "عذرا رصيدك غير كافي لاجراء العملية";
            public static string SendingCardFailed = "فشلت عملية ارسال كرت للرقم {0} رصيدك {1} يرجى المحاولة لاحقا";
        }
        public delegate void DoWorkEventHandler(object sender, DeliverMessage.DeliveryWorkerDoArgs e);
    }
}
