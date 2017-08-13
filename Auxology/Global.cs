using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.IO;
using System.Data.OleDb;

namespace Auxology
{
    class Global
    {
        public static string SERVER = "http://contentsbox.co.kr";
        public static bool search_flag = false;
        public static bool pinfo_register_click_flag = false;
        
        public static int munjin_count1 = 0;
        public static int munjin_count2 = 0;

        public static int predict_count1 = 0;
        public static int predict_count2 = 0;

        public static string pc_lcns = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();//mac address
        public static string chart_num = "";
        public static string current_chart_num = "";
        public static string h_name = "";
        public static string d_lcns = "";
        static byte[] Skey = ASCIIEncoding.ASCII.GetBytes("50457988");

        public static int[] RUS_RADIUS = new int[] { 0, 16, 21, 30, 39, 59, 87, 138, 213 };
        public static int[] RUS_ULNA = new int[] { 0, 27, 30, 32, 40, 58, 107, 181 };
        public static int[] RUS_METACARPAL1 = new int[] { 0, 6, 9, 14, 21, 26, 36, 49, 67 };
        public static int[] RUS_METACARPAL3 = new int[] { 0, 4, 5, 9, 12, 19, 31, 43, 52 };
        public static int[] RUS_METACARPAL5 = new int[] { 0, 4, 6, 9, 14, 18, 29, 43, 52 };
        public static int[] RUS_PROXIMAL1 = new int[] { 0, 7, 8, 11, 17, 26, 38, 52, 67 };
        public static int[] RUS_PROXIMAL3 = new int[] { 0, 4, 4, 9, 15, 23, 31, 40, 53 };
        public static int[] RUS_PROXIMAL5 = new int[] { 0, 4, 5, 9, 15, 21, 30, 39, 51 };
        public static int[] RUS_MIDDLE3 = new int[] { 0, 4, 6, 9, 15, 22, 32, 43, 52 };
        public static int[] RUS_MIDDLE5 = new int[] { 0, 6, 7, 9, 15, 23, 32, 42, 49 };
        public static int[] RUS_DISTAL1 = new int[] { 0, 5, 6, 11, 17, 26, 38, 46, 66 };
        public static int[] RUS_DISTAL3 = new int[] { 0, 4, 6, 8, 13, 18, 28, 34, 49 };
        public static int[] RUS_DISTAL5 = new int[] { 0, 5, 6, 9, 13, 18, 27, 34, 48 };


        public static Dictionary<int, double> RUS_BONAGE = new Dictionary<int, double>();
        
        public static void Input_Boneage()
        {
            RUS_BONAGE.Add(42, 2.0);
            RUS_BONAGE.Add(46, 2.1);
            RUS_BONAGE.Add(50, 2.2);
            RUS_BONAGE.Add(55, 2.3);
            RUS_BONAGE.Add(60, 2.4);
            RUS_BONAGE.Add(66, 2.5);
            RUS_BONAGE.Add(70, 2.6);
            RUS_BONAGE.Add(75, 2.7);
            RUS_BONAGE.Add(80, 2.8);
            RUS_BONAGE.Add(86, 2.9);
            RUS_BONAGE.Add(91, 3.0);
            RUS_BONAGE.Add(94, 3.1);
            RUS_BONAGE.Add(98, 3.2);
            RUS_BONAGE.Add(101, 3.3);
            RUS_BONAGE.Add(105, 3.4);
            RUS_BONAGE.Add(108, 3.5);
            RUS_BONAGE.Add(112, 3.6);
            RUS_BONAGE.Add(116, 3.7);
            RUS_BONAGE.Add(120, 3.8);
            RUS_BONAGE.Add(124, 3.9);
            RUS_BONAGE.Add(129, 4.0);
            RUS_BONAGE.Add(132, 4.1);
            RUS_BONAGE.Add(134, 4.2);
            RUS_BONAGE.Add(137, 4.3);
            RUS_BONAGE.Add(140, 4.4);
            RUS_BONAGE.Add(143, 4.5);
            RUS_BONAGE.Add(146, 4.6);
            RUS_BONAGE.Add(149, 4.7);
            RUS_BONAGE.Add(152, 4.8);
            RUS_BONAGE.Add(155, 4.9);
            RUS_BONAGE.Add(158, 5.0);
            RUS_BONAGE.Add(161, 5.1);
            RUS_BONAGE.Add(164, 5.2);
            RUS_BONAGE.Add(166, 5.3);
            RUS_BONAGE.Add(169, 5.4);
            RUS_BONAGE.Add(172, 5.5);
            RUS_BONAGE.Add(175, 5.6);
            RUS_BONAGE.Add(177, 5.7);
            RUS_BONAGE.Add(180, 5.8);
            RUS_BONAGE.Add(183, 5.9);
            RUS_BONAGE.Add(186, 6.0);
            RUS_BONAGE.Add(189, 6.1);
            RUS_BONAGE.Add(191, 6.2);
            RUS_BONAGE.Add(194, 6.3);
            RUS_BONAGE.Add(197, 6.4);
            RUS_BONAGE.Add(200, 6.5);
            RUS_BONAGE.Add(202, 6.6);
            RUS_BONAGE.Add(205, 6.7);
            RUS_BONAGE.Add(208, 6.8);
            RUS_BONAGE.Add(211, 6.9);
            RUS_BONAGE.Add(214, 7.0);
            RUS_BONAGE.Add(216, 7.1);
            RUS_BONAGE.Add(219, 7.2);
            RUS_BONAGE.Add(222, 7.3);
            RUS_BONAGE.Add(225, 7.4);
            RUS_BONAGE.Add(228, 7.5);
            RUS_BONAGE.Add(231, 7.6);
            RUS_BONAGE.Add(234, 7.7);
            RUS_BONAGE.Add(237, 7.8);
            RUS_BONAGE.Add(240, 7.9);
            RUS_BONAGE.Add(243, 8.0);
            RUS_BONAGE.Add(246, 8.1);
            RUS_BONAGE.Add(250, 8.2);
            RUS_BONAGE.Add(253, 8.3);
            RUS_BONAGE.Add(256, 8.4);
            RUS_BONAGE.Add(259, 8.5);
            RUS_BONAGE.Add(262, 8.6);
            RUS_BONAGE.Add(265, 8.7);
            RUS_BONAGE.Add(268, 8.8);
            RUS_BONAGE.Add(272, 8.9);
            RUS_BONAGE.Add(275, 9.0);
            RUS_BONAGE.Add(279, 9.1);
            RUS_BONAGE.Add(283, 9.2);
            RUS_BONAGE.Add(287, 9.3);
            RUS_BONAGE.Add(291, 9.4);
            RUS_BONAGE.Add(295, 9.5);
            RUS_BONAGE.Add(299, 9.6);
            RUS_BONAGE.Add(303, 9.7);
            RUS_BONAGE.Add(308, 9.8);
            RUS_BONAGE.Add(312, 9.9);
            RUS_BONAGE.Add(316, 10.0);
            RUS_BONAGE.Add(321, 10.1);
            RUS_BONAGE.Add(325, 10.2);
            RUS_BONAGE.Add(330, 10.3);
            RUS_BONAGE.Add(334, 10.4);
            RUS_BONAGE.Add(339, 10.5);
            RUS_BONAGE.Add(344, 10.6);
            RUS_BONAGE.Add(348, 10.7);
            RUS_BONAGE.Add(353, 10.8);
            RUS_BONAGE.Add(358, 10.9);
            RUS_BONAGE.Add(363, 11.0);
            RUS_BONAGE.Add(369, 11.1);
            RUS_BONAGE.Add(375, 11.2);
            RUS_BONAGE.Add(381, 11.3);
            RUS_BONAGE.Add(387, 11.4);
            RUS_BONAGE.Add(394, 11.5);
            RUS_BONAGE.Add(400, 11.6);
            RUS_BONAGE.Add(406, 11.7);
            RUS_BONAGE.Add(413, 11.8);
            RUS_BONAGE.Add(420, 11.9);
            RUS_BONAGE.Add(427, 12.0);
            RUS_BONAGE.Add(434, 12.1);
            RUS_BONAGE.Add(441, 12.2);
            RUS_BONAGE.Add(448, 12.3);
            RUS_BONAGE.Add(455, 12.4);
            RUS_BONAGE.Add(462, 12.5);
            RUS_BONAGE.Add(470, 12.6);
            RUS_BONAGE.Add(478, 12.7);
            RUS_BONAGE.Add(485, 12.8);
            RUS_BONAGE.Add(493, 12.9);
            RUS_BONAGE.Add(501, 13.0);
            RUS_BONAGE.Add(511, 13.1);
            RUS_BONAGE.Add(520, 13.2);
            RUS_BONAGE.Add(530, 13.3);
            RUS_BONAGE.Add(540, 13.4);
            RUS_BONAGE.Add(550, 13.5);
            RUS_BONAGE.Add(560, 13.6);
            RUS_BONAGE.Add(570, 13.7);
            RUS_BONAGE.Add(581, 13.8);
            RUS_BONAGE.Add(592, 13.9);
            RUS_BONAGE.Add(603, 14.0);
            RUS_BONAGE.Add(615, 14.1);
            RUS_BONAGE.Add(628, 14.2);
            RUS_BONAGE.Add(641, 14.3);
            RUS_BONAGE.Add(655, 14.4);
            RUS_BONAGE.Add(668, 14.5);
            RUS_BONAGE.Add(682, 14.6);
            RUS_BONAGE.Add(697, 14.7);
            RUS_BONAGE.Add(711, 14.8);
            RUS_BONAGE.Add(726, 14.9);
            RUS_BONAGE.Add(741, 15.0);
            RUS_BONAGE.Add(755, 15.1);
            RUS_BONAGE.Add(769, 15.2);
            RUS_BONAGE.Add(783, 15.3);
            RUS_BONAGE.Add(798, 15.4);
            RUS_BONAGE.Add(813, 15.5);
            RUS_BONAGE.Add(828, 15.6);
            RUS_BONAGE.Add(843, 15.7);
            RUS_BONAGE.Add(859, 15.8);
            RUS_BONAGE.Add(875, 15.9);
            RUS_BONAGE.Add(891, 16.0);
            RUS_BONAGE.Add(912, 16.1);
            RUS_BONAGE.Add(933, 16.2);
            RUS_BONAGE.Add(955, 16.3);
            RUS_BONAGE.Add(977, 16.4);
            RUS_BONAGE.Add(1000, 16.5);
        }

        public static void select_Hinfo()
        {
            string connStr = "provider = Microsoft.JET.OLEDB.4.0;Data Source={0};";
            connStr = String.Format(connStr, "auxology.mdb");
            OleDbConnection gConn = new OleDbConnection(connStr);
            string query = "select * from d_info";
            OleDbCommand cmd = new OleDbCommand(query, gConn);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
            gConn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                h_name = dr[1].ToString();
                d_lcns = Decrypt(dr[5].ToString());
                for(int i = 0; i<d_lcns.Length; i++)
                {
                    if (d_lcns[i] == '\0')
                    {
                        d_lcns = d_lcns.Substring(0, i);
                        break;
                    }
                }
            }
            h_name = Encrypt(h_name);
            dr.Close();
            gConn.Close();
        }

        public static string spilitDate(string date)
        {
            string[] sd = new string[3];
            string sd_sum = "";
            sd[0] = date.Substring(0, 4);
            sd[1] = date.Substring(date.IndexOf('월')-2, 2);
            if(sd[1][0]==' ')
            {
                sd[1] = "0" + sd[1][1];
            }            
            sd[2] = date.Substring(date.IndexOf('일')-2, 2);
            if (sd[2][0] == ' ')
            {
                sd[2] = "0" + sd[2][1];
            }
            sd_sum = sd[0] + "-" + sd[1] + "-" + sd[2];
            return sd_sum;
        }
        
        public static string Encrypt(string p_data)
        {
            // 암호화 알고리즘중 RC2 암호화를 하려면 RC를
            // DES알고리즘을 사용하려면 DESCryptoServiceProvider 객체를 선언한다.
            //RC2 rc2 = new RC2CryptoServiceProvider();
            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            // 대칭키 배치
            rc2.Key = Skey;
            rc2.IV = Skey;

            // 암호화는 스트림(바이트 배열)을
            // 대칭키에 의존하여 암호화 하기때문에 먼저 메모리 스트림을 생성한다.
            MemoryStream ms = new MemoryStream();

            //만들어진 메모리 스트림을 이용해서 암호화 스트림 생성 
            CryptoStream cryStream =
                              new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);

            // 데이터를 바이트 배열로 변경
            byte[] data = Encoding.UTF8.GetBytes(p_data.ToCharArray());

            // 암호화 스트림에 데이터 씀
            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();

            // 암호화 완료 (string으로 컨버팅해서 반환)
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string p_data)
        {        
            // 암호화 알고리즘중 RC2 암호화를 하려면 RC를
            // DES알고리즘을 사용하려면 DESCryptoServiceProvider 객체를 선언한다.
            //RC2 rc2 = new RC2CryptoServiceProvider();
            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            // 대칭키 배치
            rc2.Key = Skey;
            rc2.IV = Skey;

            // 암호화는 스트림(바이트 배열)을
            // 대칭키에 의존하여 암호화 하기때문에 먼저 메모리 스트림을 생성한다.
            MemoryStream ms = new MemoryStream();

            //만들어진 메모리 스트림을 이용해서 암호화 스트림 생성 
            CryptoStream cryStream =
                              new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);

            //데이터를 바이트배열로 변경한다.
            byte[] data = Convert.FromBase64String(p_data);

            //변경된 바이트배열을 암호화 한다.
            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();

            //암호화 한 데이터를 스트링으로 변환해서 리턴
            return Encoding.UTF8.GetString(ms.GetBuffer());
        }
    }
}
