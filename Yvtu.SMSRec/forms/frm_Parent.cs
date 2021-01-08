using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.SMSRec
{
    public partial class frm_Parent : Form
    {
        private byte childFormNumber = 0;
        private bool collapseSharedParamPanel = false;

        public IAppDbContext Db { get; }

        public frm_Parent(IAppDbContext db)
        {
            InitializeComponent();
            Db = db;
        }
        public void RefreshInterfacesCount()
        {
            lblInterfaceCount.Text =Convert.ToString(--childFormNumber);
        }
        public  void AddInterfaceCount(int value)
        {
            byte intcnt = byte.Parse(lblInterfaceCount.Text);
            intcnt += byte.Parse(value.ToString());
            lblInterfaceCount.Text = Convert.ToString(value);
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            foreach (KeyValuePair<byte, Interface> interDict in SharedParams.InterfaceDictionary)
            {
                Interface inter = interDict.Value;
                if (childFormNumber < SharedParams.Interfaces_MaxCount)
                {
                    //if (SharedParams.InterfaceDictionary[inter.Interface_No].Status == "closed")
                    //{
                        frm_interface childForm = new frm_interface(inter.Interface_No, Db);
                        childForm.MdiParent = this;
                        childForm.Tag = inter.Interface_No.ToString();
                        childForm.Text = "Interface " + childFormNumber++;
                        lblInterfaceCount.Text = Convert.ToString(childFormNumber);
                        childForm.Show();
                    //}
                }
            }
        }

        private bool InterfaceIsExists(string interfaceTag)
        {
            foreach (var form in     this.MdiChildren)
            {
                if (form.Tag.Equals(interfaceTag))
                    return true;
            }
            return false;
        }
        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
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

        private void frm_Parent_Load(object sender, EventArgs e)
        {
            LoadSettings();
            this.Text += " - " + SharedParams.Short_Code;
        }
        void LoadSettings()
        {
            SharedParams.Load();
            lblAppStatus.Text = SharedParams.Application_Status;
            Util.LoadInterfaceDictionary();
            LoadSharedParameters();
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

        private void btnToggleSharedParams_Click(object sender, EventArgs e)
        {
            if (!collapseSharedParamPanel)
            {
                this.btnToggleSharedParams.ImageKey = "toggle_down_alt.png";
                btnToggleSharedParams.Text = string.Empty;
                this.toolTip.SetToolTip(this.btnToggleSharedParams, "Show panel");
                collapseSharedParamPanel = true;
                panel2.Width = 35;
            }
            else
            {
                this.btnToggleSharedParams.ImageKey = "toggle_right.png";
                this.toolTip.SetToolTip(this.btnToggleSharedParams, "Hide panel");
                btnToggleSharedParams.Text = string.Empty;
                collapseSharedParamPanel = false;
                panel2.Width = 306;
            }
        }

        private void ReloadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }
    }
}
