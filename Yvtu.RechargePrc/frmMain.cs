using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.RechargePrc.Ops;

namespace Yvtu.RechargePrc
{
    public partial class frmMain : Form
    {
        // Xf-eTZr-T|szAhrlL5zZX@IbKd*Plis%ALrCHnA#6QdsO&HW@M
        public int childFormNumber = 0;
        private string ch;
        private int ChCount = 0;
        private int StartCh = 1;
        private int MaxChs = 0;
        private readonly IAppDbContext db;

        public frmMain(IAppDbContext db)
        {
            InitializeComponent();
            this.db = db;
        }

        void LoadSettings()
        {
            //ch = DB.GetKeyValueFromConfigFile("Channel");
            ChCount =int.Parse( Utility.GetKeyValueFromConfigFile("ChannelCount"));
            StartCh =int.Parse(Utility.GetKeyValueFromConfigFile("StartChannel"));
            MaxChs = int.Parse(Utility.GetKeyValueFromConfigFile("MaxChannels"));
            txtchcnt.Text = ChCount.ToString();
            txtstartch.Text = StartCh.ToString();
            txtMaxch.Text = MaxChs.ToString();

            SharedParams.Load();
            this.lblDaiyReportSendTime.Text = SharedParams.DailyReportSendTime;
            this.Text = "Recharge bg " + SharedParams.Short_Code + "." + SharedParams.Application_Title;
        }

       

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void btn_newodd_Click(object sender, EventArgs e)
        {
            using (Form1 childForm = new Form1(ch, db))
            {
                childForm.MdiParent = this;
                childForm.Text = "W" + childFormNumber++;
                lblCount.Text = "Count: " + childFormNumber + " interface(s)";
                childForm.Show();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadSettings();
            //this.Text += " - " + SharedParams.Short_Code;
            SharedParams.TokenTimeLeft = 0;
            this.timer1.Enabled = true;
        }

        

        private void btn_newall_Click(object sender, EventArgs e)
        {
            int c = StartCh - 1;
            if (childFormNumber >= MaxChs * 2) return;
            for (int i=1;i<=MaxChs;i++)
            {
                c = StartCh-1+i;
                CreateNewProccess(c.ToString(), "all");
            }
        }

        void CreateNewProccess(string ch,string ftype)
        {
            var childForm = new Form1(ch, db)
            {
                MdiParent = this,
                Tag = ch + ftype
            };
            childFormNumber++;
            lblCount.Text = "Count: " + childFormNumber + " interface(s)";
            childForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Form1 childForm in MdiChildren)
            {
                childForm.Start_Processes();
            }
        }

        private void btn_closeall_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Are you sure to close all opened processes?", "close all", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.No) return;
            //foreach (Form childForm in MdiChildren)
            //{
            //    childForm.Close();
            //    childFormNumber--;
            //    lblCount.Text = "Count:" + childFormNumber;
            //}
            MessageBox.Show(Get8Digits());
        }

        public string Get8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }

       
        private void btn_tilevertical_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void btnHorizontal_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }


        private void btn_StopStartFetch_Click(object sender, EventArgs e)
        {

            if (btn_StopStartFetch.Text.Equals("Pause"))
            {
                btn_StopStartFetch.Text = "Resume";
                foreach (Form1 childForm in MdiChildren)
                {
                    childForm.StopStartFetch(true);
                }
            }
            else
            {
                btn_StopStartFetch.Text = "Pause";
                foreach (Form1 childForm in MdiChildren)
                {
                    childForm.StopStartFetch(false);
                }
            }
        }

        private void btn_reloadsettings_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void btn_CreateProc_MouseHover(object sender, EventArgs e)
        {
            lblHelp.Text = "Create interfaces that process queued requests";
        }

        private void btn_CreateProc_MouseLeave(object sender, EventArgs e)
        {
            lblHelp.Text = string.Empty;
        }

        private void btn_startall_MouseHover(object sender, EventArgs e)
        {
            lblHelp.Text = "Start processing ...";
        }

        private void btn_startall_MouseLeave(object sender, EventArgs e)
        {
            lblHelp.Text = string.Empty;
        }

        private void btn_StopLog_MouseHover(object sender, EventArgs e)
        {
            lblHelp.Text = "Stop and resume writting logs";
        }

        private void btn_StopLog_MouseLeave(object sender, EventArgs e)
        {
            lblHelp.Text = string.Empty;
        }

        private void btn_StopStartFetch_MouseHover(object sender, EventArgs e)
        {
            lblHelp.Text = "Stop fetching new requests from database";
        }

        private void btn_StopStartFetch_MouseLeave(object sender, EventArgs e)
        {
            lblHelp.Text = string.Empty;
        }

        private void btn_reloadsettings_MouseHover(object sender, EventArgs e)
        {
            lblHelp.Text = "Reload settings from configuration file and database";
        }

        private void btn_reloadsettings_MouseLeave(object sender, EventArgs e)
        {
            lblHelp.Text = string.Empty;
        }

        private void btn_SuspectProc_MouseHover(object sender, EventArgs e)
        {
            lblHelp.Text = "Start processing the suspected request";
        }

        private void btn_SuspectProc_MouseLeave(object sender, EventArgs e)
        {
            lblHelp.Text = string.Empty;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SharedParams.SendDailyReportPermission && !string.IsNullOrEmpty(SharedParams.DailyReportSendTime))
            {
                var sendingTime  = SharedParams.DailyReportSendTime.Replace(":", string.Empty) + "00";
                var time = DateTime.Now.ToString("Hmmss");
                if (time == sendingTime)
                {
                    SendDailyReport();
                    lblHelp.Text = "Daily report has been sent";
                }
            }
            if (SharedParams.TokenTimeLeft > 0)
            {
                SharedParams.TokenTimeLeft = SharedParams.TokenTimeLeft - 1;
            }
            else
            {
                new Operations(db).Login(SharedParams.OCSApiUser, SharedParams.OCSApiPassword);
            }
            this.txtTokenTimeLeft.Text = SharedParams.TokenTimeLeft.ToString();
            this.txtToken.Text = SharedParams.Token;
        }
        void SendDailyReport()
        {
            //if (SharedParams.DailyStatReportList == null || SharedParams.DailyStatReportList.Count == 0) return;

            //var reportMessage = new Operations().CreateDailyReportMessage();
            //if (string.IsNullOrEmpty(reportMessage)) return;

            //reportMessage = "التقرير اليومي لخدمة ريال موبايل" + Environment.NewLine
            //    + "----------------------------------------------" + Environment.NewLine + reportMessage;


            //foreach (var report in SharedParams.DailyStatReportList)
            //{
            //    sms_outmessage OutMessage = new sms_outmessage();
            //    OutMessage.Short_Code = SharedParams.Short_Code;
            //    OutMessage.Message = reportMessage;
            //    OutMessage.Mobile_No = report.MobileNo;
            //    OutMessage.Create();
            //}
        }
    }
}
