namespace SEMI_FINAL
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
            this.pnlSizebar = new Guna.UI2.WinForms.Guna2Panel();
            this.lblRecentHeader = new System.Windows.Forms.Label();
            this.btnDownloadModel = new Guna.UI2.WinForms.Guna2Button();
            this.btnApiKey = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.btnNewChat = new Guna.UI2.WinForms.Guna2Button();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.pnlTopHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.lblAppTitleHeader = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_model = new Guna.UI2.WinForms.Guna2ComboBox();
            this.pnlChat = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlInputBox = new Guna.UI2.WinForms.Guna2Panel();
            this.txtInput = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnOcr = new Guna.UI2.WinForms.Guna2Button();
            this.btnSend = new Guna.UI2.WinForms.Guna2Button();
            this.pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2VScrollBar1 = new Guna.UI2.WinForms.Guna2VScrollBar();
            this.pnlSizebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            this.pnlTopHeader.SuspendLayout();
            this.pnlChat.SuspendLayout();
            this.pnlInputBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSizebar
            // 
            this.pnlSizebar.BackColor = System.Drawing.Color.Transparent;
            this.pnlSizebar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(71)))), ((int)(((byte)(82)))));
            this.pnlSizebar.BorderThickness = 1;
            this.pnlSizebar.Controls.Add(this.lblRecentHeader);
            this.pnlSizebar.Controls.Add(this.btnDownloadModel);
            this.pnlSizebar.Controls.Add(this.btnApiKey);
            this.pnlSizebar.Controls.Add(this.guna2Button1);
            this.pnlSizebar.Controls.Add(this.btnNewChat);
            this.pnlSizebar.Controls.Add(this.lblSubTitle);
            this.pnlSizebar.Controls.Add(this.label1);
            this.pnlSizebar.Controls.Add(this.guna2CirclePictureBox1);
            this.pnlSizebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSizebar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.pnlSizebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSizebar.Name = "pnlSizebar";
            this.pnlSizebar.Size = new System.Drawing.Size(280, 720);
            this.pnlSizebar.TabIndex = 0;
            // 
            // lblRecentHeader
            // 
            this.lblRecentHeader.AutoSize = true;
            this.lblRecentHeader.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblRecentHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(199)))), ((int)(((byte)(212)))));
            this.lblRecentHeader.Location = new System.Drawing.Point(18, 160);
            this.lblRecentHeader.Name = "lblRecentHeader";
            this.lblRecentHeader.Size = new System.Drawing.Size(83, 13);
            this.lblRecentHeader.TabIndex = 10;
            this.lblRecentHeader.Text = "RECENT CHATS";
            // 
            // btnDownloadModel
            // 
            this.btnDownloadModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDownloadModel.Animated = true;
            this.btnDownloadModel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.btnDownloadModel.BorderRadius = 8;
            this.btnDownloadModel.BorderThickness = 1;
            this.btnDownloadModel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(136)))), ((int)(((byte)(29)))));
            this.btnDownloadModel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDownloadModel.ForeColor = System.Drawing.Color.White;
            this.btnDownloadModel.Location = new System.Drawing.Point(16, 560);
            this.btnDownloadModel.Name = "btnDownloadModel";
            this.btnDownloadModel.Size = new System.Drawing.Size(248, 42);
            this.btnDownloadModel.TabIndex = 9;
            this.btnDownloadModel.Text = "📥 Tải Local Model";
            this.btnDownloadModel.Click += new System.EventHandler(this.btnDownloadModel_Click);
            // 
            // btnApiKey
            // 
            this.btnApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApiKey.Animated = true;
            this.btnApiKey.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnApiKey.BorderRadius = 8;
            this.btnApiKey.BorderThickness = 1;
            this.btnApiKey.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.btnApiKey.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnApiKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(201)))), ((int)(((byte)(255)))));
            this.btnApiKey.Location = new System.Drawing.Point(16, 610);
            this.btnApiKey.Name = "btnApiKey";
            this.btnApiKey.Size = new System.Drawing.Size(248, 42);
            this.btnApiKey.TabIndex = 8;
            this.btnApiKey.Text = "🔑 Cấu hình Gemini Key";
            this.btnApiKey.Click += new System.EventHandler(this.btnApiKey_Click);
            // 
            // guna2Button1
            // 
            this.guna2Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.guna2Button1.Animated = true;
            this.guna2Button1.BorderRadius = 8;
            this.guna2Button1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(50)))));
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Location = new System.Drawing.Point(16, 660);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(248, 42);
            this.guna2Button1.TabIndex = 4;
            this.guna2Button1.Text = "🚪 Thoát";
            // 
            // btnNewChat
            // 
            this.btnNewChat.Animated = true;
            this.btnNewChat.BorderRadius = 8;
            this.btnNewChat.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnNewChat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNewChat.ForeColor = System.Drawing.Color.White;
            this.btnNewChat.Location = new System.Drawing.Point(16, 95);
            this.btnNewChat.Name = "btnNewChat";
            this.btnNewChat.Size = new System.Drawing.Size(248, 44);
            this.btnNewChat.TabIndex = 11;
            this.btnNewChat.Text = "+ Cuộc trò chuyện mới";
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(199)))), ((int)(((byte)(212)))));
            this.lblSubTitle.Location = new System.Drawing.Point(74, 46);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(99, 15);
            this.lblSubTitle.TabIndex = 7;
            this.lblSubTitle.Text = "Power User Mode";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(201)))), ((int)(((byte)(255)))));
            this.label1.Location = new System.Drawing.Point(74, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 21);
            this.label1.TabIndex = 6;
            this.label1.Text = "AI Assistant";
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.ErrorImage = null;
            this.guna2CirclePictureBox1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.guna2CirclePictureBox1.InitialImage = null;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(18, 20);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(46, 46);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2CirclePictureBox1.TabIndex = 5;
            this.guna2CirclePictureBox1.TabStop = false;
            // 
            // pnlTopHeader
            // 
            this.pnlTopHeader.BackColor = System.Drawing.Color.Transparent;
            this.pnlTopHeader.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(71)))), ((int)(((byte)(82)))));
            this.pnlTopHeader.BorderThickness = 1;
            this.pnlTopHeader.Controls.Add(this.lblAppTitleHeader);
            this.pnlTopHeader.Controls.Add(this.label2);
            this.pnlTopHeader.Controls.Add(this.cb_model);
            this.pnlTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopHeader.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(19)))), ((int)(((byte)(19)))));
            this.pnlTopHeader.Location = new System.Drawing.Point(280, 0);
            this.pnlTopHeader.Name = "pnlTopHeader";
            this.pnlTopHeader.Size = new System.Drawing.Size(920, 60);
            this.pnlTopHeader.TabIndex = 4;
            // 
            // lblAppTitleHeader
            // 
            this.lblAppTitleHeader.AutoSize = true;
            this.lblAppTitleHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblAppTitleHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(226)))), ((int)(((byte)(225)))));
            this.lblAppTitleHeader.Location = new System.Drawing.Point(20, 18);
            this.lblAppTitleHeader.Name = "lblAppTitleHeader";
            this.lblAppTitleHeader.Size = new System.Drawing.Size(176, 21);
            this.lblAppTitleHeader.TabIndex = 9;
            this.lblAppTitleHeader.Text = "Ollama Hybrid AI Chat";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(199)))), ((int)(((byte)(212)))));
            this.label2.Location = new System.Drawing.Point(620, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "MODEL:";
            // 
            // cb_model
            // 
            this.cb_model.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_model.BackColor = System.Drawing.Color.Transparent;
            this.cb_model.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(71)))), ((int)(((byte)(82)))));
            this.cb_model.BorderRadius = 18;
            this.cb_model.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_model.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_model.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cb_model.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.cb_model.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.cb_model.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.cb_model.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(226)))), ((int)(((byte)(225)))));
            this.cb_model.ItemHeight = 28;
            this.cb_model.Location = new System.Drawing.Point(678, 12);
            this.cb_model.Name = "cb_model";
            this.cb_model.Size = new System.Drawing.Size(220, 34);
            this.cb_model.TabIndex = 7;
            this.cb_model.SelectedIndexChanged += new System.EventHandler(this.cboModel_SelectedIndexChanged);
            // 
            // pnlChat
            // 
            this.pnlChat.Controls.Add(this.pnlInputBox);
            this.pnlChat.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlChat.FillColor = System.Drawing.Color.Black;
            this.pnlChat.Location = new System.Drawing.Point(280, 600);
            this.pnlChat.Name = "pnlChat";
            this.pnlChat.Size = new System.Drawing.Size(920, 120);
            this.pnlChat.TabIndex = 1;
            // 
            // pnlInputBox
            // 
            this.pnlInputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlInputBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(50)))));
            this.pnlInputBox.BorderRadius = 14;
            this.pnlInputBox.BorderThickness = 1;
            this.pnlInputBox.Controls.Add(this.txtInput);
            this.pnlInputBox.Controls.Add(this.btnOcr);
            this.pnlInputBox.Controls.Add(this.btnSend);
            this.pnlInputBox.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(20)))));
            this.pnlInputBox.Location = new System.Drawing.Point(20, 10);
            this.pnlInputBox.Name = "pnlInputBox";
            this.pnlInputBox.Size = new System.Drawing.Size(880, 95);
            this.pnlInputBox.TabIndex = 4;
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.BorderThickness = 0;
            this.txtInput.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtInput.DefaultText = "";
            this.txtInput.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(20)))));
            this.txtInput.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtInput.ForeColor = System.Drawing.Color.White;
            this.txtInput.Location = new System.Drawing.Point(12, 10);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(160)))));
            this.txtInput.PlaceholderText = "Nhập tin nhắn của bạn hoặc thả tệp vào đây...";
            this.txtInput.SelectedText = "";
            this.txtInput.Size = new System.Drawing.Size(730, 75);
            this.txtInput.TabIndex = 2;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // btnOcr
            // 
            this.btnOcr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOcr.Animated = true;
            this.btnOcr.BorderRadius = 8;
            this.btnOcr.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
            this.btnOcr.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnOcr.ForeColor = System.Drawing.Color.White;
            this.btnOcr.Location = new System.Drawing.Point(750, 48);
            this.btnOcr.Name = "btnOcr";
            this.btnOcr.Size = new System.Drawing.Size(55, 38);
            this.btnOcr.TabIndex = 3;
            this.btnOcr.Text = "📎";
            this.btnOcr.Click += new System.EventHandler(this.btnOcr_Click);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Animated = true;
            this.btnSend.BorderRadius = 8;
            this.btnSend.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(812, 48);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(58, 38);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "➔";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            this.btnSend.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnSend_KeyDown);
            // 
            // pnlMain
            // 
            this.pnlMain.AutoScroll = true;
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.Black;
            this.pnlMain.Location = new System.Drawing.Point(280, 60);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(920, 540);
            this.pnlMain.TabIndex = 3;
            // 
            // guna2VScrollBar1
            // 
            this.guna2VScrollBar1.BindingContainer = this.pnlMain;
            this.guna2VScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.guna2VScrollBar1.FillColor = System.Drawing.Color.Black;
            this.guna2VScrollBar1.InUpdate = false;
            this.guna2VScrollBar1.LargeChange = 10;
            this.guna2VScrollBar1.Location = new System.Drawing.Point(1186, 60);
            this.guna2VScrollBar1.Name = "guna2VScrollBar1";
            this.guna2VScrollBar1.ScrollbarSize = 14;
            this.guna2VScrollBar1.Size = new System.Drawing.Size(14, 540);
            this.guna2VScrollBar1.TabIndex = 0;
            this.guna2VScrollBar1.ThumbColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Controls.Add(this.guna2VScrollBar1);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlChat);
            this.Controls.Add(this.pnlTopHeader);
            this.Controls.Add(this.pnlSizebar);
            this.Name = "Form1";
            this.Text = "Ollama Desktop - Chat";
            this.pnlSizebar.ResumeLayout(false);
            this.pnlSizebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.pnlTopHeader.ResumeLayout(false);
            this.pnlTopHeader.PerformLayout();
            this.pnlChat.ResumeLayout(false);
            this.pnlInputBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlSizebar;
        private Guna.UI2.WinForms.Guna2Panel pnlTopHeader;
        private Guna.UI2.WinForms.Guna2Panel pnlChat;
        private Guna.UI2.WinForms.Guna2Panel pnlInputBox;
        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2VScrollBar guna2VScrollBar1;
        private Guna.UI2.WinForms.Guna2TextBox txtInput;
        private Guna.UI2.WinForms.Guna2Button btnSend;
        private Guna.UI2.WinForms.Guna2Button btnOcr;
        private Guna.UI2.WinForms.Guna2Button btnDownloadModel;
        private Guna.UI2.WinForms.Guna2Button btnApiKey;
        private Guna.UI2.WinForms.Guna2Button btnNewChat;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Label lblRecentHeader;
        private System.Windows.Forms.Label lblAppTitleHeader;
        private Guna.UI2.WinForms.Guna2ComboBox cb_model;
        private System.Windows.Forms.Label label2;
    }
}

