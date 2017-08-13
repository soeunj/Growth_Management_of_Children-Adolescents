using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using OpenCvSharp;

namespace Auxology
{
    public partial class MainForm : Form
    {
        string connStr = "provider = Microsoft.JET.OLEDB.4.0;Data Source={0};";
        PInfoForm pinfo = new PInfoForm();//개인정보등록폼
        Munjin1Form munjin1 = new Munjin1Form();//문진표1폼
        PredictForm predict_form = new PredictForm();//신장예측폼
        
        public MainForm()
        {   
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Global.select_Hinfo();
            Global.Input_Boneage();

            Boolean access = check_lcns();
            if (access)
            {
                panel1.Controls.Clear();
                pinfo.MdiParent = this;
                pinfo.Dock = System.Windows.Forms.DockStyle.Fill;
                pinfo.Show();
                panel1.Controls.Add(pinfo);
            }
            else
            {
                DoctorInfoForm di_Form = new DoctorInfoForm();//병원 정보입력폼
                di_Form.Owner = this;
                di_Form.ShowDialog();
                if (di_Form.DialogResult == DialogResult.OK)
                {
                    panel1.Controls.Clear();
                    pinfo.MdiParent = this;
                    pinfo.Dock = System.Windows.Forms.DockStyle.Fill;
                    pinfo.Show();
                    panel1.Controls.Add(pinfo);
                }
                else
                {
                    DialogResult dr = MessageBox.Show("인증에 실패 하였습니다. 재시작 해주세요.", "알림");
                    if (dr == DialogResult.OK)
                    {
                        Application.ExitThread();
                        Environment.Exit(0);
                    }
                    this.DialogResult = di_Form.DialogResult;
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {/*
            if(this.WindowState == FormWindowState.Maximized)
            {
                pinfo.Hide();
                pinfo.Form_Maximized();
                pinfo.Show();
                pinfo.WindowState = FormWindowState.Maximized;
                
            }
            else
            {
                pinfo.Hide();
                pinfo.Form_Minimized();
                pinfo.Show();
            }*/
        }

        private void 신상정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new_Form(1);
        }

        private void 문진표1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new_Form(2);
        }

        private void 최대신장예측ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new_Form(3);
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();   
        }
        public void new_Form(int num)
        {
            if (num == 1)
            {
                panel1.Controls.Clear();
                pinfo.MdiParent = this;
                pinfo.Dock = System.Windows.Forms.DockStyle.Fill;
                pinfo.Show();
                panel1.Controls.Add(pinfo);
            }
            if (num == 2)
            {   
                panel1.Controls.Clear();
                munjin1.MdiParent = this;
                munjin1.Dock = System.Windows.Forms.DockStyle.Fill;
                munjin1.Show();
                if (Global.search_flag == true && Global.munjin_count1 == 0)
                {
                    munjin1.insert_data();
                    Global.munjin_count1++;
                }
                else if(Global.search_flag == false && Global.munjin_count1 == 1)
                {
                    munjin1.initialize_data();
                    Global.munjin_count1++;
                }
                else if(Global.pinfo_register_click_flag == true && Global.munjin_count2 == 0)
                {
                    munjin1.pinfo_registered_insert_data();
                    Global.munjin_count2++;
                }
                else if (Global.pinfo_register_click_flag == false && Global.munjin_count2 == 0)
                {
                    munjin1.initialize_data();
                    Global.munjin_count2++;
                }
                panel1.Controls.Add(munjin1);
            }
            if (num == 3)
            {
                panel1.Controls.Clear();
                predict_form.MdiParent = this;
                predict_form.Dock = System.Windows.Forms.DockStyle.Fill;
                predict_form.Show();
                if (Global.search_flag == true && Global.predict_count1 == 0)
                {
                    predict_form.insert_data();
                    Global.predict_count1++;
                }
                else if (Global.search_flag == false && Global.predict_count1 == 1)
                {
                    predict_form.initialize_data();
                    Global.predict_count1++;
                }
                else if (Global.pinfo_register_click_flag == true && Global.predict_count2 == 0)
                {
                    predict_form.pinfo_registered_insert_data();
                    Global.predict_count2++;
                }
                else if (Global.pinfo_register_click_flag == false && Global.predict_count2 == 0)
                {
                    predict_form.initialize_data();
                    Global.predict_count2++;
                }
                panel1.Controls.Add(predict_form);
            }
        }
        private Boolean check_lcns()
        {
            Boolean access_flag;
            string dt_end = "";
            connStr = String.Format(connStr, "auxology.mdb");
            OleDbConnection gConn = new OleDbConnection(connStr);
            string e_lcns = Global.Encrypt(Global.pc_lcns);
            string query = @"select dt_end from d_info where dt_mac=@dt_mac";
            
            OleDbCommand cmd = new OleDbCommand(query, gConn);
            cmd.Parameters.AddWithValue("@dt_mac", e_lcns);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
            gConn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                dt_end = dr[0].ToString();
            }
            
            if (dt_end != "")            
                access_flag = true;            
            else
                access_flag = false;
            dr.Close();
            gConn.Close();

            return access_flag;

        }
    }
}
