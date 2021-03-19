namespace Yvtu.RechargePrcFW
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_start = new System.Windows.Forms.Button();
            this.tbProgress = new System.Windows.Forms.ProgressBar();
            this.txtlog = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.chnotfetch = new System.Windows.Forms.CheckBox();
            this.chwithoutlog = new System.Windows.Forms.CheckBox();
            this.lblfetchtype = new System.Windows.Forms.Label();
            this.lblchannel = new System.Windows.Forms.Label();
            this.lblver = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(8, 12);
            this.btn_start.Margin = new System.Windows.Forms.Padding(4);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(100, 28);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // tbProgress
            // 
            this.tbProgress.Location = new System.Drawing.Point(0, 297);
            this.tbProgress.Margin = new System.Windows.Forms.Padding(4);
            this.tbProgress.Name = "tbProgress";
            this.tbProgress.Size = new System.Drawing.Size(705, 11);
            this.tbProgress.TabIndex = 2;
            this.tbProgress.Visible = false;
            // 
            // txtlog
            // 
            this.txtlog.BackColor = System.Drawing.Color.Black;
            this.txtlog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtlog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtlog.ForeColor = System.Drawing.SystemColors.Info;
            this.txtlog.Location = new System.Drawing.Point(4, 13);
            this.txtlog.Margin = new System.Windows.Forms.Padding(4);
            this.txtlog.Multiline = true;
            this.txtlog.Name = "txtlog";
            this.txtlog.ReadOnly = true;
            this.txtlog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtlog.Size = new System.Drawing.Size(1131, 577);
            this.txtlog.TabIndex = 4;
            this.txtlog.TextChanged += new System.EventHandler(this.txtlog_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "gnome_edit_clear.png");
            // 
            // chnotfetch
            // 
            this.chnotfetch.AutoSize = true;
            this.chnotfetch.Location = new System.Drawing.Point(417, 16);
            this.chnotfetch.Margin = new System.Windows.Forms.Padding(4);
            this.chnotfetch.Name = "chnotfetch";
            this.chnotfetch.Size = new System.Drawing.Size(161, 21);
            this.chnotfetch.TabIndex = 7;
            this.chnotfetch.Text = "Don\'t fetch any more";
            this.chnotfetch.UseVisualStyleBackColor = true;
            this.chnotfetch.CheckedChanged += new System.EventHandler(this.chnotfetch_CheckedChanged);
            // 
            // chwithoutlog
            // 
            this.chwithoutlog.AutoSize = true;
            this.chwithoutlog.Location = new System.Drawing.Point(309, 16);
            this.chwithoutlog.Margin = new System.Windows.Forms.Padding(4);
            this.chwithoutlog.Name = "chwithoutlog";
            this.chwithoutlog.Size = new System.Drawing.Size(101, 21);
            this.chwithoutlog.TabIndex = 8;
            this.chwithoutlog.Text = "Without log";
            this.chwithoutlog.UseVisualStyleBackColor = true;
            this.chwithoutlog.CheckedChanged += new System.EventHandler(this.chwithoutlog_CheckedChanged);
            // 
            // lblfetchtype
            // 
            this.lblfetchtype.AutoSize = true;
            this.lblfetchtype.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblfetchtype.ForeColor = System.Drawing.Color.Black;
            this.lblfetchtype.Location = new System.Drawing.Point(219, 15);
            this.lblfetchtype.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblfetchtype.Name = "lblfetchtype";
            this.lblfetchtype.Size = new System.Drawing.Size(32, 24);
            this.lblfetchtype.TabIndex = 9;
            this.lblfetchtype.Text = "All";
            // 
            // lblchannel
            // 
            this.lblchannel.AutoSize = true;
            this.lblchannel.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblchannel.ForeColor = System.Drawing.Color.Blue;
            this.lblchannel.Location = new System.Drawing.Point(115, 15);
            this.lblchannel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblchannel.Name = "lblchannel";
            this.lblchannel.Size = new System.Drawing.Size(47, 24);
            this.lblchannel.TabIndex = 10;
            this.lblchannel.Text = "CH1";
            // 
            // lblver
            // 
            this.lblver.AutoSize = true;
            this.lblver.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblver.ForeColor = System.Drawing.Color.Black;
            this.lblver.Location = new System.Drawing.Point(599, 17);
            this.lblver.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblver.Name = "lblver";
            this.lblver.Size = new System.Drawing.Size(60, 18);
            this.lblver.TabIndex = 11;
            this.lblver.Text = "Version:";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.lblver);
            this.groupBox1.Controls.Add(this.btn_start);
            this.groupBox1.Controls.Add(this.lblchannel);
            this.groupBox1.Controls.Add(this.lblfetchtype);
            this.groupBox1.Controls.Add(this.chwithoutlog);
            this.groupBox1.Controls.Add(this.chnotfetch);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(33, 1);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtlog, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1.55642F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 98.44358F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1139, 594);
            this.tableLayoutPanel1.TabIndex = 13;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Ivory;
            this.ClientSize = new System.Drawing.Size(1139, 594);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tbProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "RMBG";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.ProgressBar tbProgress;
        private System.Windows.Forms.TextBox txtlog;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.CheckBox chnotfetch;
        private System.Windows.Forms.CheckBox chwithoutlog;
        private System.Windows.Forms.Label lblfetchtype;
        private System.Windows.Forms.Label lblchannel;
        private System.Windows.Forms.Label lblver;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}

