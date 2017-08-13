namespace Auxology
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanel_main = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.파일ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.불러오기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.저장하기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.개인정보등록ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.신상정보ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.문진표1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.문진표2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.최대신장예측ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.환경설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.종료ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel_main.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_main
            // 
            this.tableLayoutPanel_main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_main.AutoScroll = true;
            this.tableLayoutPanel_main.AutoSize = true;
            this.tableLayoutPanel_main.ColumnCount = 1;
            this.tableLayoutPanel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel_main.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel_main.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel_main.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tableLayoutPanel_main.Name = "tableLayoutPanel_main";
            this.tableLayoutPanel_main.RowCount = 2;
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_main.Size = new System.Drawing.Size(1341, 706);
            this.tableLayoutPanel_main.TabIndex = 22;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Location = new System.Drawing.Point(2, 38);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1337, 665);
            this.panel1.TabIndex = 19;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일ToolStripMenuItem,
            this.개인정보등록ToolStripMenuItem,
            this.최대신장예측ToolStripMenuItem,
            this.환경설정ToolStripMenuItem,
            this.종료ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1341, 35);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            this.파일ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.불러오기ToolStripMenuItem,
            this.저장하기ToolStripMenuItem});
            this.파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            this.파일ToolStripMenuItem.Size = new System.Drawing.Size(43, 31);
            this.파일ToolStripMenuItem.Text = "파일";
            // 
            // 불러오기ToolStripMenuItem
            // 
            this.불러오기ToolStripMenuItem.Name = "불러오기ToolStripMenuItem";
            this.불러오기ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.불러오기ToolStripMenuItem.Text = "불러오기";
            // 
            // 저장하기ToolStripMenuItem
            // 
            this.저장하기ToolStripMenuItem.Name = "저장하기ToolStripMenuItem";
            this.저장하기ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.저장하기ToolStripMenuItem.Text = "저장하기";
            // 
            // 개인정보등록ToolStripMenuItem
            // 
            this.개인정보등록ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.신상정보ToolStripMenuItem,
            this.문진표1ToolStripMenuItem,
            this.문진표2ToolStripMenuItem});
            this.개인정보등록ToolStripMenuItem.Name = "개인정보등록ToolStripMenuItem";
            this.개인정보등록ToolStripMenuItem.Size = new System.Drawing.Size(91, 31);
            this.개인정보등록ToolStripMenuItem.Text = "개인정보등록";
            // 
            // 신상정보ToolStripMenuItem
            // 
            this.신상정보ToolStripMenuItem.Name = "신상정보ToolStripMenuItem";
            this.신상정보ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.신상정보ToolStripMenuItem.Text = "신상정보";
            this.신상정보ToolStripMenuItem.Click += new System.EventHandler(this.신상정보ToolStripMenuItem_Click);
            // 
            // 문진표1ToolStripMenuItem
            // 
            this.문진표1ToolStripMenuItem.Name = "문진표1ToolStripMenuItem";
            this.문진표1ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.문진표1ToolStripMenuItem.Text = "문진표1";
            this.문진표1ToolStripMenuItem.Click += new System.EventHandler(this.문진표1ToolStripMenuItem_Click);
            // 
            // 문진표2ToolStripMenuItem
            // 
            this.문진표2ToolStripMenuItem.Name = "문진표2ToolStripMenuItem";
            this.문진표2ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.문진표2ToolStripMenuItem.Text = "문진표2";
            // 
            // 최대신장예측ToolStripMenuItem
            // 
            this.최대신장예측ToolStripMenuItem.Name = "최대신장예측ToolStripMenuItem";
            this.최대신장예측ToolStripMenuItem.Size = new System.Drawing.Size(91, 31);
            this.최대신장예측ToolStripMenuItem.Text = "최대신장예측";
            this.최대신장예측ToolStripMenuItem.Click += new System.EventHandler(this.최대신장예측ToolStripMenuItem_Click);
            // 
            // 환경설정ToolStripMenuItem
            // 
            this.환경설정ToolStripMenuItem.Name = "환경설정ToolStripMenuItem";
            this.환경설정ToolStripMenuItem.Size = new System.Drawing.Size(67, 31);
            this.환경설정ToolStripMenuItem.Text = "환경설정";
            // 
            // 종료ToolStripMenuItem
            // 
            this.종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            this.종료ToolStripMenuItem.Size = new System.Drawing.Size(43, 31);
            this.종료ToolStripMenuItem.Text = "종료";
            this.종료ToolStripMenuItem.Click += new System.EventHandler(this.종료ToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1090, 661);
            this.Controls.Add(this.tableLayoutPanel_main);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "소아, 청소년 성장관리 보조시스템_V1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel_main.ResumeLayout(false);
            this.tableLayoutPanel_main.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_main;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 불러오기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 저장하기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 개인정보등록ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 신상정보ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 문진표1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 문진표2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 최대신장예측ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 환경설정ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 종료ToolStripMenuItem;
    }
}

