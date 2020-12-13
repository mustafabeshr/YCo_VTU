namespace Yvtu.RechargePrc
{
    partial class frmMain
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblHelp = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btn_CreateProc = new System.Windows.Forms.Button();
            this.btn_startall = new System.Windows.Forms.Button();
            this.btn_closeall = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTokenTimeLeft = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxch = new System.Windows.Forms.TextBox();
            this.txtstartch = new System.Windows.Forms.TextBox();
            this.txtchcnt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_tilevertical = new System.Windows.Forms.Button();
            this.btnHorizontal = new System.Windows.Forms.Button();
            this.pnl_btns = new System.Windows.Forms.Panel();
            this.lblDaiyReportSendTime = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_SuspectProc = new System.Windows.Forms.Button();
            this.btn_reloadsettings = new System.Windows.Forms.Button();
            this.btn_StopStartFetch = new System.Windows.Forms.Button();
            this.btn_StopLog = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnl_btns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCount,
            this.lblHelp});
            this.statusStrip.Location = new System.Drawing.Point(0, 824);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(1358, 26);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "StatusStrip";
            // 
            // lblCount
            // 
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(141, 20);
            this.lblCount.Text = "Count: 0 interface(s)";
            // 
            // lblHelp
            // 
            this.lblHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(36, 20);
            this.lblHelp.Text = "N/A";
            // 
            // btn_CreateProc
            // 
            this.btn_CreateProc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CreateProc.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_CreateProc.FlatAppearance.BorderSize = 0;
            this.btn_CreateProc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btn_CreateProc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_CreateProc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_CreateProc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btn_CreateProc.Location = new System.Drawing.Point(0, 0);
            this.btn_CreateProc.Margin = new System.Windows.Forms.Padding(5);
            this.btn_CreateProc.Name = "btn_CreateProc";
            this.btn_CreateProc.Size = new System.Drawing.Size(109, 76);
            this.btn_CreateProc.TabIndex = 7;
            this.btn_CreateProc.Text = "Create Processes";
            this.btn_CreateProc.UseVisualStyleBackColor = true;
            this.btn_CreateProc.Click += new System.EventHandler(this.btn_newall_Click);
            this.btn_CreateProc.MouseLeave += new System.EventHandler(this.btn_CreateProc_MouseLeave);
            this.btn_CreateProc.MouseHover += new System.EventHandler(this.btn_CreateProc_MouseHover);
            // 
            // btn_startall
            // 
            this.btn_startall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_startall.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_startall.FlatAppearance.BorderSize = 0;
            this.btn_startall.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btn_startall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_startall.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_startall.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btn_startall.Location = new System.Drawing.Point(0, 76);
            this.btn_startall.Margin = new System.Windows.Forms.Padding(5);
            this.btn_startall.Name = "btn_startall";
            this.btn_startall.Size = new System.Drawing.Size(109, 76);
            this.btn_startall.TabIndex = 8;
            this.btn_startall.Text = "Start Processes";
            this.btn_startall.UseVisualStyleBackColor = true;
            this.btn_startall.Click += new System.EventHandler(this.button1_Click);
            this.btn_startall.MouseLeave += new System.EventHandler(this.btn_startall_MouseLeave);
            this.btn_startall.MouseHover += new System.EventHandler(this.btn_startall_MouseHover);
            // 
            // btn_closeall
            // 
            this.btn_closeall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_closeall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_closeall.Location = new System.Drawing.Point(827, 8);
            this.btn_closeall.Margin = new System.Windows.Forms.Padding(5);
            this.btn_closeall.Name = "btn_closeall";
            this.btn_closeall.Size = new System.Drawing.Size(141, 35);
            this.btn_closeall.TabIndex = 10;
            this.btn_closeall.Text = "Close all";
            this.btn_closeall.UseVisualStyleBackColor = true;
            this.btn_closeall.Visible = false;
            this.btn_closeall.Click += new System.EventHandler(this.btn_closeall_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtToken);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtTokenTimeLeft);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtMaxch);
            this.groupBox1.Controls.Add(this.txtstartch);
            this.groupBox1.Controls.Add(this.txtchcnt);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 784);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(1358, 40);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // txtToken
            // 
            this.txtToken.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtToken.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtToken.ForeColor = System.Drawing.Color.Maroon;
            this.txtToken.Location = new System.Drawing.Point(861, 9);
            this.txtToken.Margin = new System.Windows.Forms.Padding(5);
            this.txtToken.Name = "txtToken";
            this.txtToken.ReadOnly = true;
            this.txtToken.Size = new System.Drawing.Size(479, 24);
            this.txtToken.TabIndex = 9;
            this.txtToken.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(802, 12);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "Token:";
            // 
            // txtTokenTimeLeft
            // 
            this.txtTokenTimeLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTokenTimeLeft.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTokenTimeLeft.ForeColor = System.Drawing.Color.Maroon;
            this.txtTokenTimeLeft.Location = new System.Drawing.Point(717, 9);
            this.txtTokenTimeLeft.Margin = new System.Windows.Forms.Padding(5);
            this.txtTokenTimeLeft.Name = "txtTokenTimeLeft";
            this.txtTokenTimeLeft.ReadOnly = true;
            this.txtTokenTimeLeft.Size = new System.Drawing.Size(82, 24);
            this.txtTokenTimeLeft.TabIndex = 7;
            this.txtTokenTimeLeft.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(619, 14);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Token time left:";
            // 
            // txtMaxch
            // 
            this.txtMaxch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxch.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMaxch.ForeColor = System.Drawing.Color.Maroon;
            this.txtMaxch.Location = new System.Drawing.Point(541, 9);
            this.txtMaxch.Margin = new System.Windows.Forms.Padding(5);
            this.txtMaxch.Name = "txtMaxch";
            this.txtMaxch.ReadOnly = true;
            this.txtMaxch.Size = new System.Drawing.Size(71, 24);
            this.txtMaxch.TabIndex = 5;
            this.txtMaxch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtstartch
            // 
            this.txtstartch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtstartch.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtstartch.ForeColor = System.Drawing.Color.Maroon;
            this.txtstartch.Location = new System.Drawing.Point(369, 8);
            this.txtstartch.Margin = new System.Windows.Forms.Padding(5);
            this.txtstartch.Name = "txtstartch";
            this.txtstartch.ReadOnly = true;
            this.txtstartch.Size = new System.Drawing.Size(71, 24);
            this.txtstartch.TabIndex = 4;
            this.txtstartch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtchcnt
            // 
            this.txtchcnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtchcnt.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtchcnt.ForeColor = System.Drawing.Color.Maroon;
            this.txtchcnt.Location = new System.Drawing.Point(191, 8);
            this.txtchcnt.Margin = new System.Windows.Forms.Padding(5);
            this.txtchcnt.Name = "txtchcnt";
            this.txtchcnt.ReadOnly = true;
            this.txtchcnt.Size = new System.Drawing.Size(71, 24);
            this.txtchcnt.TabIndex = 3;
            this.txtchcnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(443, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Max Channel :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(267, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Start Channel :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(75, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Channels Count :";
            // 
            // btn_tilevertical
            // 
            this.btn_tilevertical.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_tilevertical.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_tilevertical.FlatAppearance.BorderSize = 0;
            this.btn_tilevertical.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btn_tilevertical.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_tilevertical.Location = new System.Drawing.Point(0, 746);
            this.btn_tilevertical.Margin = new System.Windows.Forms.Padding(5);
            this.btn_tilevertical.Name = "btn_tilevertical";
            this.btn_tilevertical.Size = new System.Drawing.Size(109, 38);
            this.btn_tilevertical.TabIndex = 17;
            this.btn_tilevertical.Text = "Tile Vertical";
            this.btn_tilevertical.UseVisualStyleBackColor = true;
            this.btn_tilevertical.Click += new System.EventHandler(this.btn_tilevertical_Click);
            // 
            // btnHorizontal
            // 
            this.btnHorizontal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnHorizontal.FlatAppearance.BorderSize = 0;
            this.btnHorizontal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnHorizontal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHorizontal.Location = new System.Drawing.Point(0, 708);
            this.btnHorizontal.Margin = new System.Windows.Forms.Padding(5);
            this.btnHorizontal.Name = "btnHorizontal";
            this.btnHorizontal.Size = new System.Drawing.Size(109, 38);
            this.btnHorizontal.TabIndex = 18;
            this.btnHorizontal.Text = "Tile Horizontal";
            this.btnHorizontal.UseVisualStyleBackColor = true;
            this.btnHorizontal.Click += new System.EventHandler(this.btnHorizontal_Click);
            // 
            // pnl_btns
            // 
            this.pnl_btns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnl_btns.Controls.Add(this.lblDaiyReportSendTime);
            this.pnl_btns.Controls.Add(this.pictureBox1);
            this.pnl_btns.Controls.Add(this.btn_SuspectProc);
            this.pnl_btns.Controls.Add(this.btn_reloadsettings);
            this.pnl_btns.Controls.Add(this.btn_StopStartFetch);
            this.pnl_btns.Controls.Add(this.btn_StopLog);
            this.pnl_btns.Controls.Add(this.btn_startall);
            this.pnl_btns.Controls.Add(this.btn_CreateProc);
            this.pnl_btns.Controls.Add(this.btnHorizontal);
            this.pnl_btns.Controls.Add(this.btn_tilevertical);
            this.pnl_btns.Controls.Add(this.btn_closeall);
            this.pnl_btns.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnl_btns.Location = new System.Drawing.Point(1249, 0);
            this.pnl_btns.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnl_btns.Name = "pnl_btns";
            this.pnl_btns.Size = new System.Drawing.Size(109, 784);
            this.pnl_btns.TabIndex = 20;
            // 
            // lblDaiyReportSendTime
            // 
            this.lblDaiyReportSendTime.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDaiyReportSendTime.Location = new System.Drawing.Point(3, 489);
            this.lblDaiyReportSendTime.Name = "lblDaiyReportSendTime";
            this.lblDaiyReportSendTime.Size = new System.Drawing.Size(105, 35);
            this.lblDaiyReportSendTime.TabIndex = 24;
            this.lblDaiyReportSendTime.Text = "-";
            this.lblDaiyReportSendTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureBox1.Location = new System.Drawing.Point(0, 594);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(109, 114);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // btn_SuspectProc
            // 
            this.btn_SuspectProc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SuspectProc.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_SuspectProc.FlatAppearance.BorderSize = 0;
            this.btn_SuspectProc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btn_SuspectProc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SuspectProc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_SuspectProc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btn_SuspectProc.Location = new System.Drawing.Point(0, 370);
            this.btn_SuspectProc.Margin = new System.Windows.Forms.Padding(5);
            this.btn_SuspectProc.Name = "btn_SuspectProc";
            this.btn_SuspectProc.Size = new System.Drawing.Size(109, 90);
            this.btn_SuspectProc.TabIndex = 22;
            this.btn_SuspectProc.Text = "Process Suspected";
            this.btn_SuspectProc.UseVisualStyleBackColor = true;
            this.btn_SuspectProc.MouseLeave += new System.EventHandler(this.btn_SuspectProc_MouseLeave);
            this.btn_SuspectProc.MouseHover += new System.EventHandler(this.btn_SuspectProc_MouseHover);
            // 
            // btn_reloadsettings
            // 
            this.btn_reloadsettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_reloadsettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_reloadsettings.FlatAppearance.BorderSize = 0;
            this.btn_reloadsettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btn_reloadsettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_reloadsettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_reloadsettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btn_reloadsettings.Location = new System.Drawing.Point(0, 294);
            this.btn_reloadsettings.Margin = new System.Windows.Forms.Padding(5);
            this.btn_reloadsettings.Name = "btn_reloadsettings";
            this.btn_reloadsettings.Size = new System.Drawing.Size(109, 76);
            this.btn_reloadsettings.TabIndex = 21;
            this.btn_reloadsettings.Text = "Reload Settings";
            this.btn_reloadsettings.UseVisualStyleBackColor = true;
            this.btn_reloadsettings.Click += new System.EventHandler(this.btn_reloadsettings_Click);
            this.btn_reloadsettings.MouseLeave += new System.EventHandler(this.btn_reloadsettings_MouseLeave);
            this.btn_reloadsettings.MouseHover += new System.EventHandler(this.btn_reloadsettings_MouseHover);
            // 
            // btn_StopStartFetch
            // 
            this.btn_StopStartFetch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_StopStartFetch.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_StopStartFetch.FlatAppearance.BorderSize = 0;
            this.btn_StopStartFetch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btn_StopStartFetch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StopStartFetch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_StopStartFetch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btn_StopStartFetch.Location = new System.Drawing.Point(0, 228);
            this.btn_StopStartFetch.Margin = new System.Windows.Forms.Padding(5);
            this.btn_StopStartFetch.Name = "btn_StopStartFetch";
            this.btn_StopStartFetch.Size = new System.Drawing.Size(109, 66);
            this.btn_StopStartFetch.TabIndex = 20;
            this.btn_StopStartFetch.Text = "Pause";
            this.btn_StopStartFetch.UseVisualStyleBackColor = true;
            this.btn_StopStartFetch.Click += new System.EventHandler(this.btn_StopStartFetch_Click);
            this.btn_StopStartFetch.MouseLeave += new System.EventHandler(this.btn_StopStartFetch_MouseLeave);
            this.btn_StopStartFetch.MouseHover += new System.EventHandler(this.btn_StopStartFetch_MouseHover);
            // 
            // btn_StopLog
            // 
            this.btn_StopLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_StopLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_StopLog.FlatAppearance.BorderSize = 0;
            this.btn_StopLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btn_StopLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StopLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_StopLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btn_StopLog.Location = new System.Drawing.Point(0, 152);
            this.btn_StopLog.Margin = new System.Windows.Forms.Padding(5);
            this.btn_StopLog.Name = "btn_StopLog";
            this.btn_StopLog.Size = new System.Drawing.Size(109, 76);
            this.btn_StopLog.TabIndex = 19;
            this.btn_StopLog.Text = "Stop Log";
            this.btn_StopLog.UseVisualStyleBackColor = true;
            this.btn_StopLog.MouseLeave += new System.EventHandler(this.btn_StopLog_MouseLeave);
            this.btn_StopLog.MouseHover += new System.EventHandler(this.btn_StopLog_MouseHover);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1358, 850);
            this.Controls.Add(this.pnl_btns);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip);
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmMain";
            this.Text = "Recharge (BG)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnl_btns.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btn_CreateProc;
        private System.Windows.Forms.Button btn_startall;
        private System.Windows.Forms.Button btn_closeall;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtstartch;
        private System.Windows.Forms.TextBox txtMaxch;
        private System.Windows.Forms.Button btn_tilevertical;
        private System.Windows.Forms.Button btnHorizontal;
        private System.Windows.Forms.Panel pnl_btns;
        private System.Windows.Forms.Button btn_StopLog;
        private System.Windows.Forms.Button btn_StopStartFetch;
        private System.Windows.Forms.Button btn_reloadsettings;
        private System.Windows.Forms.Button btn_SuspectProc;
        private System.Windows.Forms.ToolStripStatusLabel lblHelp;
        private System.Windows.Forms.TextBox txtchcnt;
        private System.Windows.Forms.ToolStripStatusLabel lblCount;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtTokenTimeLeft;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Label lblDaiyReportSendTime;
    }
}



