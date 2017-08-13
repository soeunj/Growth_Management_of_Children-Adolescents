namespace Auxology
{
    partial class DoctorInfoForm
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
            this.panel_docInfo = new System.Windows.Forms.Panel();
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_del = new System.Windows.Forms.Button();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.doc_lcns = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.end_date = new System.Windows.Forms.TextBox();
            this.mac_adrs = new System.Windows.Forms.TextBox();
            this.hp_doctor = new System.Windows.Forms.TextBox();
            this.hp_address = new System.Windows.Forms.TextBox();
            this.hp_hp = new System.Windows.Forms.TextBox();
            this.hp_name = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_docInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_docInfo
            // 
            this.panel_docInfo.Controls.Add(this.btn_start);
            this.panel_docInfo.Controls.Add(this.btn_del);
            this.panel_docInfo.Controls.Add(this.btn_confirm);
            this.panel_docInfo.Controls.Add(this.doc_lcns);
            this.panel_docInfo.Controls.Add(this.btn_save);
            this.panel_docInfo.Controls.Add(this.end_date);
            this.panel_docInfo.Controls.Add(this.mac_adrs);
            this.panel_docInfo.Controls.Add(this.hp_doctor);
            this.panel_docInfo.Controls.Add(this.hp_address);
            this.panel_docInfo.Controls.Add(this.hp_hp);
            this.panel_docInfo.Controls.Add(this.hp_name);
            this.panel_docInfo.Controls.Add(this.label7);
            this.panel_docInfo.Controls.Add(this.label4);
            this.panel_docInfo.Controls.Add(this.label5);
            this.panel_docInfo.Controls.Add(this.label6);
            this.panel_docInfo.Controls.Add(this.label3);
            this.panel_docInfo.Controls.Add(this.label2);
            this.panel_docInfo.Controls.Add(this.label1);
            this.panel_docInfo.Location = new System.Drawing.Point(3, 2);
            this.panel_docInfo.Name = "panel_docInfo";
            this.panel_docInfo.Size = new System.Drawing.Size(577, 340);
            this.panel_docInfo.TabIndex = 0;
            // 
            // btn_start
            // 
            this.btn_start.Enabled = false;
            this.btn_start.Location = new System.Drawing.Point(484, 230);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 27);
            this.btn_start.TabIndex = 118;
            this.btn_start.Text = "시작하기";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_del
            // 
            this.btn_del.Location = new System.Drawing.Point(484, 42);
            this.btn_del.Name = "btn_del";
            this.btn_del.Size = new System.Drawing.Size(75, 23);
            this.btn_del.TabIndex = 114;
            this.btn_del.Text = "삭제";
            this.btn_del.UseVisualStyleBackColor = true;
            // 
            // btn_confirm
            // 
            this.btn_confirm.Location = new System.Drawing.Point(407, 230);
            this.btn_confirm.Name = "btn_confirm";
            this.btn_confirm.Size = new System.Drawing.Size(75, 27);
            this.btn_confirm.TabIndex = 115;
            this.btn_confirm.Text = "PC인증";
            this.btn_confirm.UseVisualStyleBackColor = true;
            this.btn_confirm.Click += new System.EventHandler(this.btn_confirm_Click);
            // 
            // doc_lcns
            // 
            this.doc_lcns.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.doc_lcns.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.doc_lcns.Location = new System.Drawing.Point(314, 187);
            this.doc_lcns.Name = "doc_lcns";
            this.doc_lcns.Size = new System.Drawing.Size(87, 27);
            this.doc_lcns.TabIndex = 108;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(407, 42);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 111;
            this.btn_save.Text = "저장";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // end_date
            // 
            this.end_date.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.end_date.Enabled = false;
            this.end_date.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.end_date.Location = new System.Drawing.Point(135, 272);
            this.end_date.Name = "end_date";
            this.end_date.Size = new System.Drawing.Size(266, 27);
            this.end_date.TabIndex = 110;
            this.end_date.TextChanged += new System.EventHandler(this.end_date_TextChanged);
            // 
            // mac_adrs
            // 
            this.mac_adrs.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mac_adrs.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mac_adrs.Location = new System.Drawing.Point(135, 229);
            this.mac_adrs.Name = "mac_adrs";
            this.mac_adrs.ReadOnly = true;
            this.mac_adrs.Size = new System.Drawing.Size(266, 27);
            this.mac_adrs.TabIndex = 117;
            // 
            // hp_doctor
            // 
            this.hp_doctor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.hp_doctor.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.hp_doctor.Location = new System.Drawing.Point(135, 187);
            this.hp_doctor.Name = "hp_doctor";
            this.hp_doctor.Size = new System.Drawing.Size(87, 27);
            this.hp_doctor.TabIndex = 107;
            // 
            // hp_address
            // 
            this.hp_address.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.hp_address.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.hp_address.Location = new System.Drawing.Point(135, 127);
            this.hp_address.Name = "hp_address";
            this.hp_address.Size = new System.Drawing.Size(266, 27);
            this.hp_address.TabIndex = 106;
            // 
            // hp_hp
            // 
            this.hp_hp.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.hp_hp.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.hp_hp.Location = new System.Drawing.Point(135, 83);
            this.hp_hp.Name = "hp_hp";
            this.hp_hp.Size = new System.Drawing.Size(266, 27);
            this.hp_hp.TabIndex = 105;
            // 
            // hp_name
            // 
            this.hp_name.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.hp_name.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.hp_name.Location = new System.Drawing.Point(135, 42);
            this.hp_name.Name = "hp_name";
            this.hp_name.Size = new System.Drawing.Size(266, 27);
            this.hp_name.TabIndex = 102;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(17, 275);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 17);
            this.label7.TabIndex = 116;
            this.label7.Text = "인증만료일";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(17, 229);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 17);
            this.label4.TabIndex = 113;
            this.label4.Text = "인증PC";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(232, 190);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 17);
            this.label5.TabIndex = 112;
            this.label5.Text = "면허번호";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(17, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 17);
            this.label6.TabIndex = 109;
            this.label6.Text = "병원장 성명";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(17, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 17);
            this.label3.TabIndex = 104;
            this.label3.Text = "병원 주소";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(17, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 103;
            this.label2.Text = "병원 전화번호";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(17, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 17);
            this.label1.TabIndex = 101;
            this.label1.Text = "병원명";
            // 
            // DoctorInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 343);
            this.ControlBox = false;
            this.Controls.Add(this.panel_docInfo);
            this.Name = "DoctorInfoForm";
            this.Text = "DoctorInfoForm";
            this.Load += new System.EventHandler(this.DoctorInfoForm_Load);
            this.panel_docInfo.ResumeLayout(false);
            this.panel_docInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_docInfo;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_del;
        private System.Windows.Forms.Button btn_confirm;
        private System.Windows.Forms.TextBox doc_lcns;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.TextBox end_date;
        private System.Windows.Forms.TextBox mac_adrs;
        private System.Windows.Forms.TextBox hp_doctor;
        private System.Windows.Forms.TextBox hp_address;
        private System.Windows.Forms.TextBox hp_hp;
        private System.Windows.Forms.TextBox hp_name;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}