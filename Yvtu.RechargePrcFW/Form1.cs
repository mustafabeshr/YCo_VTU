using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using Yvtu.RechargePrcFW.lib;

namespace Yvtu.RechargePrcFW
{
    public partial class Form1 : Form
    {
        Queue<GrappedRecharge> QueuedRequests = null;
        private BackgroundWorker bw;
        private string fetchtype = string.Empty;
        private string ch = string.Empty;
        private string fetchtable = string.Empty;
        private int PayTimeout = 0;
        private int NoRowsWaitTimeout = 0;
        private string SystemVersion = string.Empty;
        private string logmessage = string.Empty;
        private string AttachMessage = string.Empty;

        public Form1()
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }
        public Form1(string Channel)
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            ch = Channel;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            ch = Channel;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }
        void LoadSettings()
        {
            //fetchtype = DB.GetKeyValueFromConfigFile("FetchType");
            lblfetchtype.Text = fetchtype;
            PayTimeout = int.Parse(Utility.GetKeyValueFromConfigFile("payment_timeout"));
            NoRowsWaitTimeout = int.Parse(Utility.GetKeyValueFromConfigFile("NoRowsWaitTimeout"));
            timer1.Interval = int.Parse(Utility.GetKeyValueFromConfigFile("pay_timer_interval"));
            //ch = DB.GetKeyValueFromConfigFile("Channel");
            lblchannel.Text = "CH" + ch;
            SystemVersion = Utility.GetKeyValueFromConfigFile("sysver");
            lblver.Text = "Version:" + SystemVersion;
            this.Text = "Recharge [bg] : CH " + ch ;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
                btn_start.Enabled = true;
                //btn_cancel.Enabled = false;
                timer1.Enabled = false;
            }
        }
        private void btn_start_Click(object sender, EventArgs e)
        {
            Start_Processes();
        }
        public void StopStartRecordLog(bool newState)
        {
            chwithoutlog.Checked = newState;
        }
        public void StopStartFetch(bool newState)
        {
            chnotfetch.Checked = newState;
        }
        public void Start_Processes()
        {
            if (btn_start.Enabled)
            {
                btn_start.Enabled = false;
                //btn_cancel.Enabled = true;
                timer1.Enabled = true;
            }
            if (chnotfetch.Checked)
                chnotfetch.Checked = false;
        }
        public void Stop_Processes()
        {
            if (!chnotfetch.Checked)
                chnotfetch.Checked = true;

        }
        public void Start_Process()
        {
            btn_start.Enabled = false;
            //btn_cancel.Enabled = true;
            timer1.Enabled = true;
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            GrappedRecharge GrabbedRequest = null;
            string msg = string.Empty;
            for (int i = 1; (i <= 10); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    if (!chnotfetch.Checked)
                    {
                        QueuedRequests = new RechargeRepo().GetPendingRechargeQueue(int.Parse(ch));
                        if (!object.ReferenceEquals(QueuedRequests, null) && (QueuedRequests.Count > 0))
                        {
                            msg = Environment.NewLine + DateTime.Now.ToString("dd HH:mm:ss") + " Load "  + QueuedRequests.Count.ToString() + " request(s) from database";
                            logmessage = msg;
                            worker.ReportProgress(40, msg);
                            if (!chwithoutlog.Checked) WritingLOGToFILE(logmessage);
                            while (QueuedRequests.Count > 0 && !chnotfetch.Checked)
                            {
                                //Thread.Sleep(2000);
                                
                                int RequestsLeft = QueuedRequests.Count;
                                GrabbedRequest = QueuedRequests.Dequeue();
                                
                                #region ...solve redundancy message
                                //if (GrabbedRequest.RequestQueueCommandId != (int)COMMANDS.PAYMENT_ROLLBACK
                                //    && GrabbedRequest.RequestQueueCommandId != (int)COMMANDS.YM_BONUS
                                //    && Utility.isRedundancyMessage(GrabbedRequest.Client_Mobile, GrabbedRequest.Client_Message, GrabbedRequest.Req_Time))
                                //{
                                //    ////////////////////////////////////////////////////////////
                                //    sms_outmessage OutMessage = new sms_outmessage();
                                //    OutMessage.Short_Code = SharedParams.Short_Code;
                                //    string Msg = SharedParams.System_Messages["REDUNDANCY_CMD"];
                                //    OutMessage.Message = Msg;
                                //    OutMessage.Mobile_No = GrabbedRequest.Client_Mobile;
                                //    OutMessage.Create();
                                //    GrabbedRequest.Debug_Info = SharedParams.System_Messages["REDUNDANCY_CMD"];
                                //    GrabbedRequest.Lifecycle = "REDUNDANCY_CMD";
                                //    GrabbedRequest.CloseRequestWithFail();
                                //    //////////////////////////////////////////////////////////// 
                                //    worker.ReportProgress(40, "REDUNDANCY_CMD");
                                //    return;
                                //}
                                #endregion

                                //Utility.WritingLOGToFILE("-----------------------------------------------------------------");
                                //Utility.WritingLOGToFILE("ReqId[ "+GrabbedRequest.Id.ToString()+" ]");
                                msg = Environment.NewLine + DateTime.Now.ToString("dd HH:mm:ss") + " No[" + RequestsLeft + "] [" +
                                      GrabbedRequest.MasterId.ToString() + "] Recharge "+ GrabbedRequest.SubscriberNo  + "("+ GrabbedRequest.Amount.ToString("N0") 
                                      + ") ...";
                                logmessage = msg;
                                worker.ReportProgress(40, msg);
                                 #region Recharge
                                    var watch = System.Diagnostics.Stopwatch.StartNew();
                                    AttachMessage = string.Empty;
                                    Operations operation = new Operations();
                                    var result = operation.DoRecharge(GrabbedRequest);
                                    watch.Stop();
                                    double elapsedMs = watch.ElapsedMilliseconds;
                                    msg = operation.resultDisplayMsg + "  within " + Math.Round(elapsedMs / 1000, 2) + " sec " ;
                                    logmessage += msg;
                                    worker.ReportProgress(40, msg);
                                    if (!chwithoutlog.Checked) WritingLOGToFILE(logmessage + " charge result = "+ result.ResultCode + "|"+ result.ResultDesc);
                                    #endregion
                            }
                        } //(GrabbedRequest != null)
                        else
                        {
                            msg = Environment.NewLine + DateTime.Now.ToString("dd HH:mm:ss") + " Loading 0 request(s) from database";
                            logmessage += msg;
                            if (!chwithoutlog.Checked) WritingLOGToFILE(logmessage);
                            worker.ReportProgress(0, msg);
                            Thread.Sleep(NoRowsWaitTimeout);
                        }
                        //System.Threading.Thread.Sleep(2000);
                        //worker.ReportProgress((i * 10));
                    }
                }
            }
        }
       
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.tbProgress.Value = 100;

            if ((e.Cancelled == true))
            {
                this.tbProgress.Text = "Canceled!";
            }
            else if (!(e.Error == null))
            {
                this.tbProgress.Text = ("Error: " + e.Error.Message);
            }
            else
            {
                this.tbProgress.Text = "Done!";
            }
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                string strlog = e.UserState.ToString();
                txtlog.AppendText(strlog);

            }
            catch (NullReferenceException)
            { }
            catch
            { }
            this.tbProgress.Value = e.ProgressPercentage;
            this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
        }
        void WriteLog(string msg)
        {
            txtlog.AppendText(msg);
            txtlog.AppendText("\n");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }
        private void btn_clear_Click(object sender, EventArgs e)
        {
            txtlog.Text = string.Empty;
        }
        private void txtlog_TextChanged(object sender, EventArgs e)
        {
            if (txtlog.Lines.Count() == 30)
            {
                txtlog.Text = string.Empty;
            }
        }
        private void btn_reload_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }
        void WritingLOGToFILE(string strline)
        {
            strline = strline.Replace("\r\n", string.Empty);
            string folderpath = AppDomain.CurrentDomain.BaseDirectory + @"LOG\" + DateTime.Now.ToString("yyyyMMdd");
            bool exists = System.IO.Directory.Exists(folderpath);
            if (!exists) System.IO.Directory.CreateDirectory(folderpath);
            folderpath = folderpath + @"\RM_" +DateTime.Now.ToString("yyyyMMdd")+"_"+SharedParams.Short_Code+"_" + fetchtype + "_" + "CH" + ch  + ".log";
            string lc = strline.Substring(strline.Length - 1, 1);
            if (!File.Exists(folderpath))
            {
                using (StreamWriter sw = File.CreateText(folderpath))
                {
                    //if ( lc== @"\n")
                    sw.WriteLine(strline);
                    logmessage = string.Empty;
                    //else
                    //   sw.Write(strline);
                }
            }
            else
            {
                StreamWriter sw = File.AppendText(folderpath);
                sw.WriteLine(strline);
                logmessage = string.Empty;
                sw.Close();
            }
        }
        private void chnotfetch_CheckedChanged(object sender, EventArgs e)
        {
            if (!chnotfetch.Checked)
            {
                WritingLOGToFILE(DateTime.Now.ToString("dd HH:mm:ss") + " Resume Processing");
                txtlog.AppendText("\r\n" + DateTime.Now.ToString("dd HH:mm:ss") + " Resume Processing");
            }
            else
            {
                WritingLOGToFILE(DateTime.Now.ToString("dd HH:mm:ss") + " Pause Processing");
                txtlog.AppendText("\r\n" + DateTime.Now.ToString("dd HH:mm:ss") + " Pause Processing");
            }
            chChangeColor();
        }
        void chChangeColor()
        {
            if (chnotfetch.Checked && chwithoutlog.Checked)
            {
                this.BackColor = Color.Red;
            }
            else if (chnotfetch.Checked && !chwithoutlog.Checked)
            {
                this.BackColor = Color.Red;
            }
            else if (!chnotfetch.Checked && chwithoutlog.Checked)
            {
                this.BackColor = Color.Yellow;
            }
            else if (!chnotfetch.Checked && !chwithoutlog.Checked)
            {
                this.BackColor = Color.White;
            }
        }
        private void chwithoutlog_CheckedChanged(object sender, EventArgs e)
        {
            ChangeLogChecking();
        }
        private void ChangeLogChecking()
        {
            if (!chwithoutlog.Checked)
            {
                WritingLOGToFILE(DateTime.Now.ToString("dd HH:mm:ss") +  " Resume Log");
                txtlog.AppendText("\r\n"+DateTime.Now.ToString("dd HH:mm:ss") +" Resume Log");
            }
            else
            {
                WritingLOGToFILE(DateTime.Now.ToString("dd HH:mm:ss") +  " Stop Log");
                txtlog.AppendText("\r\n" + DateTime.Now.ToString("dd HH:mm:ss") +  " Stop Log");
            }
            chChangeColor();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
