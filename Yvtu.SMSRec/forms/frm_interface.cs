using System;
using System.Windows.Forms;
using SMPPSocket;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yvtu.Core.Entities;
using Yvtu.SMSRec.Repo;

namespace Yvtu.SMSRec
{
    public partial class frm_interface : Form
    {

        class CurrentLogMessage
        {
            private  string _trafficType;
            private  string _shortCode;
            private  string _mobile;
            private  string _message;
            private  string _type;
            public  string TrafficType
            {
                get
                {
                    return _trafficType;
                }

                set
                {
                    _trafficType = value;
                }
            }
            public  string ShortCode
            {
                get
                {
                    return _shortCode;
                }

                set
                {
                    _shortCode = value;
                }
            }
            public  string Mobile
            {
                get
                {
                    return _mobile;
                }

                set
                {
                    _mobile = value;
                }
            }
            public  string Message
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
            public  string Type
            {
                get
                {
                    return _type;
                }

                set
                {
                    _type = value;
                }
            }

            public CurrentLogMessage(string traffictype,string shortcode,string mobile,string message,string type)
            {
                _trafficType = traffictype;
                _shortCode = shortcode;
                _mobile = mobile;
                _message = message;
                _type = type;
            }
        }
        private BackgroundWorker bw_Delivery;
        private BackgroundWorker bw_Sender = new BackgroundWorker();
        
        private bool SMSCConnected = false;
        private byte currentChannel=0;
        //-----------------------------------------------------------
        byte _interface_no;
        frm_Parent _parentForm;
        Interface CurrentInterface;
        bool StopLog = false;
        int _bindtype;
        private readonly IRecDbContext db;

        //-----------------------------------------------------------

        private byte getCurrentChannelNo()
        {
            try
            {
                currentChannel = byte.Parse(((currentChannel % SharedParams.ChannelsCount)+1).ToString());
                return currentChannel;
             }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage(string.Empty, SharedParams.Short_Code.ToString(), "", "Could not get channel no", "error");
                WriteLog(c);
                return 1;
            }
        }
        public frm_interface(IRecDbContext db)
        {
            InitializeComponent();
            this.db = db;
            //bw_Delivery.WorkerReportsProgress = true;
            //bw_Delivery.WorkerSupportsCancellation = true;
            //bw_Delivery.DoWork += new DoWorkEventHandler(bw_Delivery_DoWork);
            //bw_Delivery.ProgressChanged += new ProgressChangedEventHandler(bw_Delivery_ProgressChanged);
            //bw_Delivery.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Delivery_RunWorkerCompleted);

        }
        public frm_interface(byte Interface_No, IRecDbContext db)
        {
            _interface_no = Interface_No;
            this.db = db;
            InitializeComponent();
            //bw_Delivery.WorkerReportsProgress = true;
            //bw_Delivery.WorkerSupportsCancellation = true;
            //bw_Delivery.DoWork += new DoWorkEventHandler(bw_Delivery_DoWork);
            //bw_Delivery.ProgressChanged += new ProgressChangedEventHandler(bw_Delivery_ProgressChanged);
            //bw_Delivery.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Delivery_RunWorkerCompleted);

           
        }
        public frm_interface(byte Interface_No,frm_Parent ParentForm, IRecDbContext db)
        {
            _interface_no = Interface_No;
            _parentForm = ParentForm;
            this.db = db;
            InitializeComponent();
        }
        private void bw_Delivery_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try {
                WriteLog((CurrentLogMessage)e.UserState);
            }
            catch (Exception ex)
            {
                CurrentLogMessage c1 = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), "", ex.Message, "error");
                WriteLog(c1);
            }
        }
        private void bw_Delivery_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Result!=null) WriteLog((CurrentLogMessage)e.Result);
                bw_Delivery.Dispose();
            }
            catch (Exception ex)
            {
                CurrentLogMessage c1 = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), "", ex.Message, "error");
                WriteLog(c1);
            }
        }
        private void bw_Delivery_DoWork(object sender, DoWorkEventArgs e)
        {       // Delivered message start parsing ...
            try {
                bool res;
                
                CurrentLogMessage c2 = null;
                DeliverMessage.Delivered_Message delmsg = (DeliverMessage.Delivered_Message)e.Argument;
                if (delmsg.Short_code == SharedParams.Short_Code.ToString())
                {
                DeliverMessage DeliverMsg = new DeliverMessage(db);
                DeliverMessage.RequestReturnValue RQretuenvalue = new DeliverMessage.RequestReturnValue();
                    var RQpack = new PartnerRequest();
                    var queueNo = getCurrentChannelNo();
                    RQretuenvalue = DeliverMsg.Parse_Request(delmsg, queueNo, out RQpack);
                    if (RQretuenvalue.Ret_Status == true)
                    {
                        c2 = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), RQpack.MobileNo,
                            RQpack.RequestName +" - " + RQpack.Shortcode + " - " + RQpack.ReplayDesc +" success ("+ RQpack .Id+ ") Q("+ queueNo + ")", "partnerok");
                        bw_Delivery.ReportProgress(0, c2);
                        if (RQpack.RequestId == 3)
                        {
                            var result = new Repo.PartnerRequestRepo(db).Create(RQpack);
                        }
                    }
                    else
                    {
                        if (RQretuenvalue.Ret_ID == -1)
                        {
                            CurrentLogMessage c = new CurrentLogMessage("S",SharedParams.Short_Code.ToString(), RQpack.MobileNo, RQretuenvalue.Ret_Message_to_Client,"partnererror");
                            e.Result = c;
                            var BadRequest = new PartnerRequest();
                            BadRequest.MobileNo = RQpack.MobileNo;
                            BadRequest.Shortcode = SharedParams.Short_Code.ToString();
                            BadRequest.Content = RQpack.Content;
                            BadRequest.Error = RQretuenvalue.Ret_Message;
                            BadRequest.AccessChannel = "sms";
                            BadRequest.QueueNo = 1;
                            BadRequest.RequestId = RQpack.RequestId;
                            BadRequest.ReplayDesc = RQretuenvalue.Ret_Message_to_Client;
                            BadRequest.ReplayTime = DateTime.Now;
                            BadRequest.Status = 2;

                            res = (new PartnerRequestRepo(db).Create(BadRequest).Success);
                            
                            if (res == false)
                            {
                                 c2 = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), RQpack.MobileNo, "failed to create bad request [" + RQretuenvalue.Ret_Message_to_Client + "]", "error");
                                e.Result = c2;
                            }
                            if (!string.IsNullOrEmpty(RQretuenvalue.Ret_Message_to_Client))
                            {
                                var outmessage = new SMSOut();
                                outmessage.Message = RQretuenvalue.Ret_Message_to_Client;
                                outmessage.Receiver = RQpack.MobileNo;
                                res = (new OutSMSRepo(db).Create(outmessage).Success);
                                if (res == false)
                                {
                                     c2 = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), RQpack.MobileNo, "failed to create sms to client [" + RQretuenvalue.Ret_Message_to_Client + "]", "error");
                                    e.Result = c2;
                                }
                            }
                        }
                        else if (RQretuenvalue.Ret_ID == -2)
                        {
                            CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),RQpack.MobileNo,RQretuenvalue.Ret_Message,"partnererror");
                            e.Result = c;
                        }
                    }//res==true
            }
                delmsg = null;
        }
            catch (Exception ex)
            {
                CurrentLogMessage c1 = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), "", ex.Message, "error");
                WriteLog(c1);
            }
        } // bw_Delivery_DoWork


        private void frm_interface_Load(object sender, EventArgs e)
        {
            CurrentInterface = SharedParams.InterfaceDictionary[_interface_no];
            CurrentInterface.Status = "running";
            LoadSharedParameters();
            LoadPrivateParameters();
            this.Text = "Interface" + _interface_no.ToString();
            //-----------------------------------------------------------------------
            btnStartStopLog.Text = "Stop Log";
            if (CurrentInterface.Auto_Start)
            {
                if (_bindtype == (int)smpp.SMPPBindType.BINDReciave)
                { _bindtype = (int)smpp.SMPPBindType.BINDReciave;
                    pnl_bindtypecolor.BackColor = Color.Blue;
                }
                smppclient.Connect(SharedParams.SMSC_IP, SharedParams.Port_No);
                if (SMSCConnected) smppclient.SMPPBind(SharedParams.Account_ID, SharedParams.Password, "EXT_SME",(smpp.SMPPBindType) _bindtype);
            }
            ch_checkconnection.Checked = true;
        }
        void GoToLastRow(DataGridView dg)
        {
            if (dg.Rows.Count > 0)
            {
                try
                {
                    dg.Rows[dg.Rows.Count - 1].Selected = true;
                    dg.CurrentCell = dg.Rows[dg.Rows.Count - 1].Cells[1];
                }
                catch (IndexOutOfRangeException)
                { }
                catch (ArgumentOutOfRangeException)
                { }
            }
        }
        void WriteLog(CurrentLogMessage clm)
        {
            if (!StopLog)
            {
                try
                {
                    if (dgv.Rows.Count > 25) dgv.Rows.Clear();
                    dgv.Rows.Add(DateTime.Now.ToString("HH:mm:ss"), clm.TrafficType, clm.ShortCode, clm.Mobile, clm.Message);
                    
                    switch (clm.Type)
                    {
                        case "sys": dgv.Rows[dgv.Rows.Count - 2].DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64); break;
                        case "partnererror": dgv.Rows[dgv.Rows.Count - 2].DefaultCellStyle.ForeColor = Color.FromArgb(192, 0, 192); break;
                        case "partnerok": dgv.Rows[dgv.Rows.Count - 2].DefaultCellStyle.ForeColor = Color.Green; break;
                        case "error": dgv.Rows[dgv.Rows.Count - 2].DefaultCellStyle.ForeColor = Color.Red; break;
                        case "deliver": dgv.Rows[dgv.Rows.Count - 2].DefaultCellStyle.ForeColor = Color.Blue; break;
                        case "invaliddeliver": dgv.Rows[dgv.Rows.Count - 2].DefaultCellStyle.ForeColor = Color.Red; break;
                        case "sender": dgv.Rows[dgv.Rows.Count - 2].DefaultCellStyle.ForeColor = Color.DarkGreen; break;
                    }
                    if (clm.Type=="sys" || clm.Type == "error")
                    {
                        if (dgv_Warnings.Rows.Count > 200) dgv.Rows.Clear();
                        dgv_Warnings.Rows.Add(DateTime.Now.ToString("dd HH:mm:ss"), clm.TrafficType, clm.ShortCode, clm.Mobile, clm.Message);
                        dgv_Warnings.DefaultCellStyle.ForeColor = Color.Red;
                    }
                    GoToLastRow(dgv);
                }
                catch (Exception ex)
                {
                    CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Exception in WriteLog [" + ex.Message + "]","error");
                    WriteLog(c);
                }
                
            }
        }
        void LoadSharedParameters()
        {
            txtSMSCIP.Text = SharedParams.SMSC_IP;
            txtPort.Text = SharedParams.Port_No.ToString();
            txtAcc.Text = SharedParams.Account_ID;
            txtPass.Text = SharedParams.Password;
            txtShortCode.Text = SharedParams.Short_Code.ToString();
            txtAppStatus.Text = SharedParams.Application_Status;
            txtOverThershold.Text = SharedParams.Overload_Threshold.ToString();
            txtStopMsg.Text = SharedParams.Application_Stop_Message;
            txtOverMsg.Text = SharedParams.OverLoad_Message;
            txtValPeriod.Text = SharedParams.CheckValidityPeriod.ToString();
            txtEnvironment.Text = SharedParams.Environment_Name;
        }
        void LoadPrivateParameters()
        {
            txtInterfaceNo.Text = CurrentInterface.Interface_No.ToString();
            txtAutoStart.Text = CurrentInterface.Auto_Start.ToString();
            txtAutoSend.Text = CurrentInterface.Auto_Sending.ToString();
            txtSendingTable.Text = CurrentInterface.SendingData_Table;
            txtSendingSpeed.Text = CurrentInterface.Sending_Speed.ToString();
            txtBroadQueueing.Text = CurrentInterface.Braodcast_Queueing.ToString();
            txtBroadTable.Text = CurrentInterface.Braodcast_Table;
            txtChannel.Text = CurrentInterface.Channel.ToString();
            txtSendDailyReport.Text = CurrentInterface.Sending_Daily_Report.ToString();
            txtDailyReportTime.Text = CurrentInterface.Sending_Daily_Report_Time;
            txtDailyReportLang.Text = CurrentInterface.Daily_Report_Lang;
            txtInterfaceStatus.Text = CurrentInterface.Status;
        }
        private void frm_interface_FormClosed(object sender, FormClosedEventArgs e)
        {
            //SharedParams.InterfaceDictionary[_interface_no].Status = "closed";
            //_parentForm.RefreshInterfacesCount();
        }
        private void smppclient_BindEvent(bool bindstatus)
        {
            try
            {
                if (bindstatus)
                {
                    CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Bind succeeded","sys");
                    WriteLog(c);
                }
                else
                {
                    CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Bind failed","sys");
                    WriteLog(c);
                }
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Exception in bind event [" + ex.Message + "]","error");
                WriteLog(c);
            }
        }
        void SMPPDisconnect()
        {
            try
            {
                smppclient.Disconnect();
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), "", "Exception in disconnect method [" + ex.Message + "]", "error");
                WriteLog(c);
            }
        }
        void SMPPConnect()
        {
            try
            {
                smppclient.Connect(SharedParams.SMSC_IP, SharedParams.Port_No);

            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), "", "Exception in connect method [" + ex.Message + "]", "error");
                WriteLog(c);
            }
        }
        private void smppclient_Connected(bool connected)
        {
            try {
                if (connected)
                {
                    SMSCConnected = true;
                    btnConnect.Text = "Disconnect";
                    timer_QueryLink.Enabled = true;
                    CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Connected","sys");
                    WriteLog(c);
                    
                }
                else
                {
                    btnConnect.Text = "Connect";
                    SMSCConnected = false;
                    timer_checkConnection.Enabled = true;
                    timer_checkConnection.Interval = 1000;
                }
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), "", "Exception in connect event [" + ex.Message + "]", "error");
                WriteLog(c);
            }
        }
        private void smppclient_DeliverEvent(string mobile_no, string short_no, string text_message, string text_language)
        {
            if (mobile_no.StartsWith("967")) mobile_no = mobile_no.Substring(3);
            if (short_no.StartsWith("967")) short_no = short_no.Substring(3);

            // Commit comming message ...
            try
            {

                Regex re = new Regex("[;\\/:*?\"<>|&']");
                text_message = re.Replace(text_message, string.Empty);

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                text_message = regex.Replace(text_message, " ");

                

                var SMSReception = new SMSIn();
                SMSReception.Sender = mobile_no;
                SMSReception.Receiver = short_no;
                SMSReception.Message = text_message;
                SMSReception.Lang = text_language;
                new SMSInRepo(db).Create(SMSReception);
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("", SharedParams.Short_Code.ToString(), "", "Exception in deliver event when trying commit comming message[" + ex.Message + "]", "error");
                WriteLog(c);
            }
            //---------------------------------------------------------
            try
            {
                if (short_no == SharedParams.Short_Code.ToString() && mobile_no.StartsWith("70"))
                {
                    DeliverMessage.Delivered_Message DelMsg = new DeliverMessage.Delivered_Message(mobile_no, short_no, text_message, text_language, CurrentInterface.Channel);
                    CurrentLogMessage c = new CurrentLogMessage("R", short_no, mobile_no, text_message, "deliver");
                    WriteLog(c);
                    if (SharedParams.Application_Status == "stop")
                    {
                        CurrentLogMessage c1 = new CurrentLogMessage("S", short_no, mobile_no, SharedParams.Application_Stop_Message, "sender");
                        WriteLog(c1);

                        var ReplayMessage = new SMSOut();
                        ReplayMessage.Message = SharedParams.Application_Stop_Message;
                        ReplayMessage.Receiver = mobile_no;
                        bool res = new OutSMSRepo(db).Create(ReplayMessage).Success;
                        if (res == false)
                        {
                            CurrentLogMessage c2 = new CurrentLogMessage("", short_no, mobile_no, "could NOT create message [" + SharedParams.Application_Stop_Message + "]", "error");
                            WriteLog(c2);
                        }
                    }
                    else
                    {
                        bw_Delivery = new BackgroundWorker();
                        bw_Delivery.WorkerReportsProgress = true;
                        bw_Delivery.WorkerSupportsCancellation = true;
                        bw_Delivery.DoWork += new DoWorkEventHandler(bw_Delivery_DoWork);
                        bw_Delivery.ProgressChanged += new ProgressChangedEventHandler(bw_Delivery_ProgressChanged);
                        bw_Delivery.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Delivery_RunWorkerCompleted);
                        if (bw_Delivery.IsBusy != true)
                        {
                            bw_Delivery.RunWorkerAsync(DelMsg);
                        }
                        else
                        {

                            string Msg = "عذرا النظام مشغول بمعالجة طلب اخر يرجى المحاولة لاحقا";
                            CurrentLogMessage c1 = new CurrentLogMessage("S", short_no, mobile_no, Msg, "sender");
                            WriteLog(c1);
                            var ReplayMessage = new SMSOut();
                            ReplayMessage.Message = Msg;
                            ReplayMessage.Receiver = mobile_no;
                            bool res = new OutSMSRepo(db).Create(ReplayMessage).Success;
                            if (res == false)
                            {
                                CurrentLogMessage c2 = new CurrentLogMessage("", short_no, mobile_no, "could NOT create message [" + Msg + "]", "error");
                                WriteLog(c2);
                            }

                        }
                    }
                }
                else
                {
                    CurrentLogMessage c = new CurrentLogMessage("R", short_no, mobile_no, text_message, "invaliddeliver");
                    WriteLog(c);
                }
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Exception in deliver event [" + ex.Message + "]","error");
                WriteLog(c);
            }
        }
        private void smppclient_Exception(Exception ex)
        {
            CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Exception [" + ex.Message + "]","error");
            WriteLog(c);
           
        }
        private void smppclient_QueryLink(bool querystatus)
        {

        }
        private void smppclient_Receive(string receiveData)
        {
            //try
            //{ 
            //WriteLog( "Receive [" + receiveData + "]");
            //}
            //catch (Exception ex)
            //{
            //    WriteLog("Exception in receive event [" + ex.Message + "]");
            //}
        }
        private void smppclient_SumbitSM(string msg_id)
        {
            try
            {
                CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Submit [" + msg_id + "]", "partnerok");
                WriteLog(c);
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("", SharedParams.Short_Code.ToString(),"","Exception in submit event [" + ex.Message + "]","error");
                WriteLog(c);
            }

        }
        private void btnStartStopLog_Click(object sender, EventArgs e)
        {
            if (StopLog)
            {
                StopLog = false;
                btnStartStopLog.Text = "Stop Log";
                
            }
            else
            {
                StopLog = true;
                btnStartStopLog.Text = "Resume Log";
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text.Equals("Connect"))
            {
                SMPPConnect();
            }
            else if (btnConnect.Text.Equals("Disconnect"))
            {
                SMPPDisconnect();
            }
        }
        private void btnBind_Click(object sender, EventArgs e)
        {
            try
            { 
            smppclient.SMPPBind(SharedParams.Account_ID, SharedParams.Password, "EXT_SME", (smpp.SMPPBindType)_bindtype);

            if (_bindtype == (int)smpp.SMPPBindType.BINDReciave)
            {pnl_bindtypecolor.BackColor = Color.Blue; }
            else if (_bindtype == (int)smpp.SMPPBindType.BINDSender)
            {pnl_bindtypecolor.BackColor = Color.Yellow; }
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Exception in bind method [" + ex.Message + "]","error");
                WriteLog(c);
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            dgv.Rows.Clear();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {

        }
        private void timer_checkConnection_Tick(object sender, EventArgs e)
        {
            if (SMSCConnected)
            {
                timer_checkConnection.Interval = 10000;
            }
            else
            {
                timer_checkConnection.Interval = 1000;
                CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Try to connect...","sys");
                WriteLog(c);
                try
                {
                    smppclient.Connect(SharedParams.SMSC_IP, SharedParams.Port_No);
                    if (SMSCConnected) btnBind_Click(sender, e);
                    
                }
                catch (Exception ex)
                {
                    CurrentLogMessage c1 = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"",ex.Message,"error");
                    WriteLog(c1);
                }
            }
        }
        private void smppclient_SMSCError_1(string err_no, string err_desc)
        {
            try
            {
                CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","SMSC Error [" + err_no + "] " + "[" + err_desc + "]","error");
                WriteLog(c);
            }
            catch (Exception ex)
            {
                CurrentLogMessage c = new CurrentLogMessage("",SharedParams.Short_Code.ToString(),"","Exception in SMSC Error event [" + ex.Message + "]","error");
                WriteLog(c);
            }
        }
        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled =Util.OnlyNumber(e.KeyChar, false, sender);
        }
        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                string rtxt = string.Empty;
                for (int i = 0; i < dgv.Columns.Count; i++)
                    rtxt += dgv[i, e.RowIndex].Value.ToString() + "    ";
                Clipboard.SetText(rtxt);

            }
            catch (NullReferenceException)
            {

            }
            catch
            { }
        }
    
        private void timer_QueryLink_Tick(object sender, EventArgs e)
        {
            smppclient.QuerySMPPLink();
        }

        private void ch_checkconnection_CheckedChanged(object sender, EventArgs e)
        {
            if (ch_checkconnection.Checked)
            {
                timer_checkConnection.Enabled = true;
                timer_checkConnection.Interval = 10000;
            }
            else
            {
                timer_checkConnection.Enabled = false;
            }
        }

        private void btn_startbook_Click(object sender, EventArgs e)
        {
        }

        private void btnTestIncome_Click(object sender, EventArgs e)
        {
            smppclient_DeliverEvent(txtInMobile.Text, "8000", txtIncoming.Text, "A");
        }
    }
}
