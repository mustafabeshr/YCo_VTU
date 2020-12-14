using ArdanStudios.Common.SmppClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Sender
{
    public partial class Form1 : Form
    {
        private static ESMEManager connectionManager = null;
        private BackgroundWorker bw;
        private bool AllowLog = true;
        private bool ContinueSending = true;
        private readonly IAppDbContext db;

        public Form1(IAppDbContext db)
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            this.db = db;
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Entities.sms_outmessage obj = (Entities.sms_outmessage)e.UserState;
            //AddLog(obj.mobile_no, obj.message, "OUT", obj.short_code, Color.Black);
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            SubmitSm submitSm = null;
            SubmitSmResp submitSmResp = null;
            List<SubmitSm> submitSmList = null;
            List<SubmitSmResp> submitSmRespList = null;
            var QMessages = new OutSMSRepo(db).GetPendingSMSQueue();
            //AddLog("dd", "worker work", "OUT", "4003", Color.Black);
            if (!object.ReferenceEquals(QMessages, null) && (QMessages.Count > 0))
            {
                while (QMessages.Count>0)
                {
                    if (ContinueSending)
                    {
                        var OutMsg = QMessages.Dequeue();
                        int retvalue = -1;
                        if (!string.IsNullOrEmpty(OutMsg.Receiver) && !string.IsNullOrEmpty(OutMsg.Message))
                        {
                            if (OutMsg.Message.Length <= 70)
                            {
                                retvalue = connectionManager.SendMessage(OutMsg.Receiver, null, Ton.Unknown, Npi.Unknown, DataCodings.UCS2
                                               , DataCodings.UCS2, OutMsg.Message, out submitSm, out submitSmResp);
                            }
                            else
                            {
                                retvalue = connectionManager.SendMessageLarge(OutMsg.Receiver, null, Ton.Unknown, Npi.Unknown, DataCodings.UCS2
                                              , DataCodings.UCS2, OutMsg.Message, out submitSmList, out submitSmRespList);
                            }
                            AddLog(OutMsg.Receiver, OutMsg.Message, "OUT", OutMsg.Sender, Color.Black);
                            new OutSMSRepo(db).RemoveMessage(OutMsg.Id);
                            //Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Setup();
            //btnRun.Enabled = true;
            //btnPause.Enabled = false;
            //btnDisconnect.Enabled = true;
            //btnConnect.Enabled = false;
            
        }
        private void PrepareLogListView()
        {
            lvLOG.View = View.Details;
            lvLOG.GridLines = true;
            lvLOG.FullRowSelect = true;

            //Add column header
            lvLOG.Columns.Add("Mobile", 200, HorizontalAlignment.Center);
            lvLOG.Columns.Add("Message", 1100, HorizontalAlignment.Left);
            lvLOG.Columns.Add("Type", 50, HorizontalAlignment.Center);
            lvLOG.Columns.Add("Short Code", 150, HorizontalAlignment.Center);
            lvLOG.Columns.Add("Time", 240, HorizontalAlignment.Center);
        }
        private void AddLog(string mobileno, string MessageValue, string TypeValue,
            string shortcode, Color forecolor)
        {
           
                //Add items in the listview
                string[] arr = new string[5];
                ListViewItem itm;

                //Add first item
                arr[0] = mobileno;
                arr[1] = MessageValue;
                arr[2] = TypeValue;
                arr[3] = shortcode;
                arr[4] = DateTime.Now.ToString("ss:mm:H dd/MM/yyyy");
                itm = new ListViewItem(arr);
                itm.ForeColor = forecolor;
                itm.ImageIndex = 1;
                //lvLOG.Items.Add(itm);
                if (lvLOG.InvokeRequired)
                    lvLOG.Invoke((MethodInvoker)delegate ()
                    {
                        lvLOG.Items.Add(itm);
                        lvLOG.EnsureVisible(lvLOG.Items.Count - 1);
                        if (lvLOG.Items.Count > 90)
                        {
                            lvLOG.Items.Clear();
                        }
                    });
        }

        private async Task AddLogAsync(string mobileno, string MessageValue, string TypeValue,
            string shortcode, Color forecolor)
        {

            //Add items in the listview
            string[] arr = new string[5];
            ListViewItem itm;

            //Add first item
            arr[0] = mobileno;
            arr[1] = MessageValue;
            arr[2] = TypeValue;
            arr[3] = shortcode;
            arr[4] = DateTime.Now.ToString("ss:mm:H dd/MM/yyyy");
            itm = new ListViewItem(arr);
            itm.ForeColor = forecolor;
            itm.ImageIndex = 1;
            //lvLOG.Items.Add(itm);
            await Task.Run(() => {
                if (lvLOG.InvokeRequired)
                    lvLOG.Invoke((MethodInvoker)delegate ()
                    {
                        lvLOG.Items.Add(itm);
                        lvLOG.EnsureVisible(lvLOG.Items.Count - 1);
                        if (lvLOG.Items.Count > 90)
                        {
                            lvLOG.Items.Clear();
                        }
                    });
            });
        }
        private void Setup()
        {

            DataCodings dataCoding = DataCodings.UCS2; // The encoding to use if Default is returned in any PDU or encoding request

            // Create a esme manager to communicate with an ESME
            connectionManager = new ESMEManager("Test",
                                               SharedParams.config.ShortCode,
                                                new ESMEManager.CONNECTION_EVENT_HANDLER(ConnectionEventHandler),
                                                new ESMEManager.RECEIVED_MESSAGE_HANDLER(ReceivedMessageHandler),
                                                new ESMEManager.RECEIVED_GENERICNACK_HANDLER(ReceivedGenericNackHandler),
                                                new ESMEManager.SUBMIT_MESSAGE_HANDLER(SubmitMessageHandler),
                                                new ESMEManager.QUERY_MESSAGE_HANDLER(QueryMessageHandler),
                                                new ESMEManager.LOG_EVENT_HANDLER(LogEventHandler),
                                                new ESMEManager.PDU_DETAILS_EVENT_HANDLER(PduDetailsHandler));

            connectionManager.AddConnections(SharedParams.config.InstancesCount,ConnectionModes.Transmitter
                , SharedParams.config.Server, SharedParams.config.Port, SharedParams.config.SystemId, SharedParams.config.Password, "VTU", dataCoding);
        }

        private Guid? PduDetailsHandler(string logKey, PduDirectionTypes pduDirectionType, Header pdu, List<PduPropertyDetail> details)
        {
            throw new NotImplementedException();
        }

        private void LogEventHandler(LogEventNotificationTypes logEventNotificationType, string logKey, string shortLongCode, string message)
        {
            if (AllowLog)
            {
                AddLog(logKey, "log Type=" + logEventNotificationType.ToString() + "|message=" + message
                , "LOG", shortLongCode, Color.Gray);
            }
        }

        private void QueryMessageHandler(string logKey, int sequence, string messageId, DateTime finalDate, int messageState, long errorCode)
        {
            if (AllowLog)
            {
                AddLog(logKey, "sequence=" + sequence.ToString() + "|messageId=" + messageId
               + "|finalDate=" + finalDate.ToString("yyyyMMddHmmss") + "|messageState=" + messageState.ToString()
                + "|errorCode=" + errorCode.ToString()
                , "QRY", SharedParams.config.ShortCode, Color.Gray);

            }
        }

        private void SubmitMessageHandler(string logKey, int sequence, string messageId)
        {
            if (AllowLog)
            {

                AddLog(logKey, "sequence=" + sequence.ToString() + "|messageId=" + messageId, "SMT", SharedParams.config.ShortCode, Color.Gray);
            }
        }

        private void ReceivedGenericNackHandler(string logKey, int sequence)
        {
            if (AllowLog)
            {

                AddLog(logKey, "sequence=" + sequence.ToString(), "NCK", SharedParams.config.ShortCode, Color.Gray);
            }
        }

        private void ReceivedMessageHandler(string logKey, string serviceType, Ton sourceTon, Npi sourceNpi, string shortLongCode, DateTime dateReceived, string phoneNumber, DataCodings dataCoding, string message)
        {
            AddLogAsync(phoneNumber, message, "IN", shortLongCode, Color.Blue);
            
        }

        private void ConnectionEventHandler(string logKey, ConnectionEventTypes connectionEventType, string message)
        {
            if (AllowLog)
            {
                AddLog(logKey, connectionEventType.ToString() + "  " + message, "CON", SharedParams.config.ShortCode, Color.Gray);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "VTU Sender";
           SharedParams.config = new Configurations
                (System.Configuration.ConfigurationManager.AppSettings["Server"].ToString(),
                int.Parse(System.Configuration.ConfigurationManager.AppSettings["Port"].ToString()),
                System.Configuration.ConfigurationManager.AppSettings["ShortCode"].ToString(),
                System.Configuration.ConfigurationManager.AppSettings["SystemId"].ToString(),
                System.Configuration.ConfigurationManager.AppSettings["Password"].ToString(),
                short.Parse(System.Configuration.ConfigurationManager.AppSettings["InstancesCount"].ToString()),
                System.Configuration.ConfigurationManager.AppSettings["ConnectionMode"].ToString(),
                System.Configuration.ConfigurationManager.AppSettings["ApplicationStatus"].ToString(),
                System.Configuration.ConfigurationManager.AppSettings["ApplicationStopMessage"].ToString(),
                short.Parse(System.Configuration.ConfigurationManager.AppSettings["OverloadThreshold"].ToString()),
                System.Configuration.ConfigurationManager.AppSettings["OverLoadMessage"].ToString(),
                short.Parse(System.Configuration.ConfigurationManager.AppSettings["CheckValidityPeriod"].ToString()),
                System.Configuration.ConfigurationManager.AppSettings["Environment"].ToString(),
                short.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxChannels"].ToString()),
                short.Parse(System.Configuration.ConfigurationManager.AppSettings["SendSpeed"].ToString()),
                System.Configuration.ConfigurationManager.AppSettings["SenderSource"].ToString());

            PrepareLogListView();
            pgOptions.SelectedObject = SharedParams.config;
            splitContainer2.Panel1Collapsed = false;
            timer1.Enabled = false;
            ts_btnConnect.Enabled = true;
            ts_btnDisconnect.Enabled = false;
            ts_btnStart.Enabled = true;
            ts_btnStop.Enabled = false;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
                ContinueSending = false;
                //btnRun.Enabled = true;
                //btnPause.Enabled = false;
                timer1.Enabled = false;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            ContinueSending = true;
            //btnRun.Enabled = false;
            //btnPause.Enabled = true;
            timer1.Enabled = true;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //btnRun.Enabled = false;
            //btnPause.Enabled = false;
            //btnDisconnect.Enabled = false;
            //btnConnect.Enabled = true;
        }

        private void txtMsg_TextChanged(object sender, EventArgs e)
        {
            //lblLen.Text = txtMsg.Text.Length.ToString();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //List<SubmitSm> submitSmList = null;
            //List<SubmitSmResp> submitSmRespList = null;
            //int retvalue = connectionManager.SendMessageLarge(txtTelnum.Text, null, Ton.Unknown, Npi.Unknown, DataCodings.UCS2
            //                              , DataCodings.UCS2, txtMsg.Text, out submitSmList, out submitSmRespList);
            //AddLog(txtTelnum.Text, txtMsg.Text, "OUT", SharedParams.config.ShortCode, Color.Black);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var itm = e.ClickedItem;
            if (itm.Name == "ts_btnConnect")
            {
                Setup();
                ts_btnDisconnect.Enabled = true;
                ts_btnConnect.Enabled = false;
            }
            else if (itm.Name == "ts_btnDisconnect")
            {
                timer1.Enabled = false;
                connectionManager.Dispose();
                ts_btnDisconnect.Enabled = false;
                ts_btnConnect.Enabled = true;
            }
            if (itm.Name == "ts_btnStart")
            {
                ContinueSending = true;
                timer1.Enabled = true;
                ts_btnStop.Enabled = true;
                ts_btnStart.Enabled = false;
            } else if (itm.Name == "ts_btnStop")
            {
                timer1.Enabled = false;
                ts_btnStart.Enabled = true;
                ts_btnStop.Enabled = false;
            }
            else if (itm.Name == "ts_btnOptions")
            {
                splitContainer2.Panel1Collapsed = !splitContainer2.Panel1Collapsed;
            }
            else if (itm.Name == "ts_btnLog")
            {
                AllowLog = !AllowLog;
            }
            else if (itm.Name == "ts_btnClear")
            {
                lvLOG.Items.Clear();
            }
        }
    }
}
