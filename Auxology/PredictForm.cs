using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Blob;
using System.Net.Http;
using Newtonsoft.Json;

using System.Data.OleDb;


namespace Auxology
{
    public partial class PredictForm : Form
    {

        string connStr = "provider = Microsoft.JET.OLEDB.4.0;Data Source={0};";
        const int hist_height = 200;
        string regday = "";
        string birthday = "";
        string path_date = "";
        string[] img_path = new string[14];
        string saveFolder;
        private int temp = 0;
        private int rus = 0;
        private string[] bone_matched = new string[13];
        private double bone_age = 0;
        private CvPoint2D32f line_point1 = new CvPoint2D32f();//손목 자르기위한 위치 저장
        private CvPoint2D32f line_point2 = new CvPoint2D32f();//손목 자르기위한 위치 저장

        List<CvPoint2D32f> c_point = new List<CvPoint2D32f>();//Covexhull point
        List<CvPoint2D32f> finger1 = new List<CvPoint2D32f>();//엄지 point
        List<CvPoint2D32f> finger2 = new List<CvPoint2D32f>();//검지 point
        List<CvPoint2D32f> finger3 = new List<CvPoint2D32f>();//중지 point
        List<CvPoint2D32f> finger4 = new List<CvPoint2D32f>();//약지 point
        List<CvPoint2D32f> finger5 = new List<CvPoint2D32f>();//새끼 point
        List<CvPoint2D32f> defect_point = new List<CvPoint2D32f>();//새끼 point

        private IplImage radius = new IplImage(100, 120, BitDepth.U8, 3);//요골                
        private IplImage ulna = new IplImage(100, 120, BitDepth.U8, 3);//척골
        private IplImage met1 = new IplImage(55, 55, BitDepth.U8, 3);//Metacarpal1
        private IplImage met3 = new IplImage(55, 55, BitDepth.U8, 3);//Metacarpal3
        private IplImage met5 = new IplImage(55, 55, BitDepth.U8, 3);//Metacarpal5
        private IplImage pph1 = new IplImage(55, 55, BitDepth.U8, 3);//P.Phalanges1
        private IplImage pph3 = new IplImage(55, 55, BitDepth.U8, 3);//P.Phalanges3
        private IplImage pph5 = new IplImage(55, 55, BitDepth.U8, 3);//P.Phalanges5
        private IplImage mph3 = new IplImage(55, 55, BitDepth.U8, 3);//M.Phalanges3
        private IplImage mph5 = new IplImage(55, 55, BitDepth.U8, 3);//M.Phalanges5
        private IplImage dph1 = new IplImage(55, 55, BitDepth.U8, 3);//D.Phalanges1
        private IplImage dph5 = new IplImage(55, 55, BitDepth.U8, 3);//D.Phalanges3
        private IplImage dph3 = new IplImage(55, 55, BitDepth.U8, 3);//D.Phalanges5

        string openstrFilename = null;

        public PredictForm()
        {
            InitializeComponent();
        }

        private void PredictForm_Load(object sender, EventArgs e)
        {
            timer1.Interval = 100;

            chartnum_tb.Text = Global.chart_num;
            lab_B.Text = "";
            lab_C.Text = "";
            lab_D.Text = "";
            lab_E.Text = "";
            lab_F.Text = "";
            lab_G.Text = "";
            lab_H.Text = "";
            lab_I.Text = "";
            regday = Global.spilitDate(reg_date.Text);
            saveFolder = "imagesave/" + Global.h_name + "/" + Global.d_lcns + "/" + reg_date.Value.ToString().Substring(0, 10) + "/" + Global.chart_num;
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "영상파일 열기";
            openFileDialog1.Filter = "All Files(*.*)|*.*| Bitmap File(*.bmp)|*.bmp|GIF File(*.gif)|*.gif|JPEG File(*.jpg)|*.jpg|PNG file(*.png)|*.png|TIFF(*.tif)|*.tif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openstrFilename = openFileDialog1.FileName;
                image_path.Text = openstrFilename;
                IplImage src = IplImage.FromFile(openstrFilename);
                
                src = Crop_image(src);
                DirectoryInfo di = new DirectoryInfo(saveFolder);
                if (di.Exists == false)
                {
                    di.Create();
                }
                R_U_Find(src);//요골, 척골 추출
                Find_Finger(src);
                Cal_Rus();
                pB_radius.Image = BitmapConverter.ToBitmap(radius);
                pB_ulna.Image = BitmapConverter.ToBitmap(ulna);
                pB_Met1.Image = BitmapConverter.ToBitmap(met1);
                pB_Met3.Image = BitmapConverter.ToBitmap(met3);
                pB_Met5.Image = BitmapConverter.ToBitmap(met5);
                pB_Pph1.Image = BitmapConverter.ToBitmap(pph1);
                pB_Pph3.Image = BitmapConverter.ToBitmap(pph3);
                pB_Pph5.Image = BitmapConverter.ToBitmap(pph5);
                pB_Mph3.Image = BitmapConverter.ToBitmap(mph3);
                pB_Mph5.Image = BitmapConverter.ToBitmap(mph5);
                pB_Dph1.Image = BitmapConverter.ToBitmap(dph1);
                pB_Dph3.Image = BitmapConverter.ToBitmap(dph3);
                pB_Dph5.Image = BitmapConverter.ToBitmap(dph5);
                xray_preview.ImageIpl = src;
                src.SaveImage(saveFolder + "/hand.png");
            }
            
            ///////////////////////////////////           
            //수골 및 지골 분할 및 특징 추출
            //손목 : 요골 및 척골 (2곳)
            //손바닥 : 제1,3,5지 중수골 (3곳)
            //손가락 : 제1,3,5지 기절골 및 말절골 (6곳)
            //손가락 : 제3. 5지 중수골 (2곳)
            ///////////////////////////////
        }

        /////////히스토그램 계산
        private static double CalcHist(IplImage img, CvHistogram hist)
        {
            float minValue, maxValue;
            hist.Calc(img);  //히스토그램 계산
            hist.GetMinMaxValue(out minValue, out maxValue); //히스토그램의 최대, 최소값 산출
            //Console.WriteLine("min : " + minValue + ",max : " + maxValue);
            Cv.Scale(hist.Bins, hist.Bins, (double)(hist_height / maxValue), 0); //최대값이 히스토그램 높이에 딱 맞게 출력되도록 조정

            return maxValue;
        }

        //////////히스토그램 그리기
        private static void DrawHist(IplImage img, CvHistogram hist, int histSize, double maxvalue)
        {
            img.Set(CvColor.White);
            int binW = Cv.Round((double)img.Width / histSize);
            double sum = 0;
            for (int i = 0; i < histSize; i++)
            {
                //Console.WriteLine(i + " : " + Cv.Round((double)hist.Bins[i] * (maxvalue / hist_height))); //해당 픽셀값(0~255)당 실제 픽셀 갯수 출력
                sum += Cv.Round((double)hist.Bins[i] * (maxvalue / hist_height)); //값의 총합 = 이미지의 픽셀 갯수(이미지 가로*세로)
                img.Rectangle(
                        new CvPoint((i * binW), img.Height),
                        new CvPoint(((i + 1) * binW), img.Height - Cv.Round(hist.Bins[i])),
                        CvColor.Black, -1, LineType.AntiAlias, 0
                        );
            }
            //Console.WriteLine(sum); //값의 총합 출력
        }

        private void Find_Finger(IplImage src)
        {
            IplImage dst = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage rotate = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage dst2 = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage dst3 = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage dst4 = new IplImage(450, 450, BitDepth.U8, 1);
            Cv.CvtColor(src, dst, ColorConversion.BgrToGray);
            dst2 = dst.Clone();
            dst3 = dst.Clone();
            dst4 = dst.Clone();

            Cv.SubS(dst, 30, dst);
            //이진화
            //Cv.Threshold(dst2, dst2, 20, 255, ThresholdType.Binary);
            Cv.AdaptiveThreshold(dst, dst, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 255, -1);

            Cv.Dilate(dst, dst);
            Cv.Dilate(dst, dst);
            Cv.Dilate(dst, dst);
            Cv.Dilate(dst, dst);
            //손바닥 중심점, 손가락 끝점 원 그리기
            int zero_count = 0;
            int i_sum = 0;
            int j_sum = 0;

            for (int i = 0; i < dst.Height; i++)
            {
                for (int j = 0; j < dst.Width; j++)
                {
                    if (dst[i, j] == 255)
                    {
                        zero_count++;
                        i_sum += i;
                        j_sum += j;
                    }
                }
            }

            double first_angle = CalAngleFromPoints(finger1[0], defect_point[4]);
            double third_angle = CalAngleFromPoints(finger3[0], new CvPoint2D32f(j_sum / zero_count, i_sum / zero_count));
            double fifth_angle = CalAngleFromPoints(finger5[0], new CvPoint2D32f(defect_point[3].X - 40, defect_point[3].Y));

            CvPoint2D32f first_rotate1 = rotate_coor(first_angle, finger1[0], src.Width / 2, src.Height / 2);
            CvPoint2D32f first_rotate2 = rotate_coor(first_angle, defect_point[4], src.Width / 2, src.Height / 2);

            CvMat mat = Cv.GetRotationMatrix2D(new CvPoint2D32f(src.Width / 2, src.Height / 2), first_angle, 1);
            Cv.WarpAffine(dst2, dst2, mat);

            Cv.SetImageROI(dst2, new CvRect((int)first_rotate2.X - 50, (int)first_rotate1.Y, 50, (int)(first_rotate2.Y + 40 - first_rotate1.Y)));

            CvPoint2D32f third_rotate1 = rotate_coor(third_angle, finger3[0], src.Width / 2, src.Height / 2);
            CvPoint2D32f third_rotate2 = rotate_coor(third_angle, defect_point[2], src.Width / 2, src.Height / 2);

            CvMat mat2 = Cv.GetRotationMatrix2D(new CvPoint2D32f(src.Width / 2, src.Height / 2), third_angle, 1);
            Cv.WarpAffine(dst3, dst3, mat2);

            Cv.SetImageROI(dst3, new CvRect((int)third_rotate1.X - 15, (int)third_rotate1.Y, 58, (int)third_rotate2.Y + 45 - (int)third_rotate1.Y));

            CvPoint2D32f fifth_rotate1 = rotate_coor(fifth_angle, finger5[0], src.Width / 2, src.Height / 2 + 150);
            CvPoint2D32f fifth_rotate2 = rotate_coor(fifth_angle, defect_point[3], src.Width / 2, src.Height / 2 + 150);

            CvMat mat3 = Cv.GetRotationMatrix2D(new CvPoint2D32f(src.Width / 2, src.Height / 2 + 150), fifth_angle, 1);
            Cv.WarpAffine(dst4, dst4, mat3);
            Cv.SetImageROI(dst4, new CvRect((int)fifth_rotate1.X - 13, (int)fifth_rotate1.Y, 55, (int)fifth_rotate2.Y + 40 - (int)fifth_rotate1.Y));

            IplImage first_finger = new IplImage(55, (int)fifth_rotate2.Y + 40 - (int)fifth_rotate1.Y, BitDepth.U8, 1);
            IplImage third_finger = new IplImage(58, (int)third_rotate2.Y + 45 - (int)third_rotate1.Y, BitDepth.U8, 1);
            IplImage fifth_finger = new IplImage(50, (int)(first_rotate2.Y - first_rotate1.Y), BitDepth.U8, 1);

            Cv.Resize(dst2, first_finger);
            Cv.Resize(dst3, third_finger);
            Cv.Resize(dst4, fifth_finger);

            Seperate_Joint(first_finger, 1);
            Seperate_Joint(third_finger, 3);
            Seperate_Joint(fifth_finger, 5);
        }

        private void Seperate_Joint(IplImage _finger, int index)
        {
            IplImage original = new IplImage(_finger.Width, _finger.Height, BitDepth.U8, 1);
            original = _finger.Clone();
            IplImage finger = ContrastStretch(_finger);
            double[] pixel_avr = new double[finger.Height];
            int count = 0;
            for (int i = 0; i < finger.Height; i++)
            {
                count = 0;
                for (int j = 0; j < finger.Width; j++)
                {
                    if (finger[i, j] > 10)
                    {
                        pixel_avr[i] += finger[i, j];
                        count++;
                    }
                }
                pixel_avr[i] = pixel_avr[i] / count;
            }

            IplImage histogram = new IplImage(255, finger.Height, BitDepth.U8, 1);
            IplImage histogram2 = new IplImage(255, finger.Height, BitDepth.U8, 3);
            histogram.Zero();
            histogram2.Zero();
            for (int i = 0; i < histogram.Height; i++)
            {
                for (int j = 0; j < (int)pixel_avr[i]; j++)
                {
                    histogram[i, j] = 255;
                }
            }
            for (int i = 0; i < histogram.Height; i++)
            {
                for (int j = 0; j < histogram.Width; j++)
                {
                    if (histogram[i, j] != 255)
                        histogram[i, j] = 0;
                }
            }

            CvSeq<CvPoint> contours;
            CvMemStorage storage = new CvMemStorage();
            //윤곽선 추출
            Cv.FindContours(histogram, storage, out contours, CvContour.SizeOf, ContourRetrieval.External, ContourChain.ApproxSimple);
            contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 3, true);
            //윤곽선 그리기
            Cv.DrawContours(histogram2, contours, CvColor.Green, CvColor.Red, 3);

            int[] temp = new int[histogram2.Height];
            List<int> madi_point = new List<int>();
            List<CvPoint> madi_point1 = new List<CvPoint>();
            List<CvPoint> madi_point2 = new List<CvPoint>();
            List<CvPoint> madi_point3 = new List<CvPoint>();
            for (int i = 0; i < histogram2.Height; i++)
            {
                for (int j = 5; j < histogram2.Width; j++)
                {
                    if (histogram2[i, j] != 0)
                    {
                        temp[i] = j;
                        break;
                    }
                }
            }            

            for (int i = 0; i < histogram2.Height/3; i++)
            {
                if (i > 1 && i < histogram2.Height/3)
                {
                    if (temp[i - 1] - temp[i] > 0 && temp[i] - temp[i + 1] < 0)
                    {
                        CvPoint point = new CvPoint(i, temp[i]);
                        madi_point.Add(i);
                        madi_point1.Add(point);
                    }
                }
            }
            
            for (int i = histogram2.Height / 3; i < histogram2.Height / 3*2; i++)
            {
                if (i > 1 && i < histogram2.Height / 3*2)
                {
                    if (temp[i - 1] - temp[i] > 0 && temp[i] - temp[i + 1] < 0)
                    {
                        CvPoint point = new CvPoint(i, temp[i]);
                        madi_point.Add(i);
                        madi_point2.Add(point);
                    }
                }
            }

            for (int i = histogram2.Height / 3*2; i < histogram2.Height -1; i++)
            {
                if (i > 1 && i < histogram2.Height - 1)
                {
                    if (temp[i - 1] - temp[i] > 0 && temp[i] - temp[i + 1] < 0)
                    {
                        CvPoint point = new CvPoint(i, temp[i]);
                        madi_point.Add(i);
                        madi_point3.Add(point);
                    }
                }
            }

            madi_point1.Sort((a, b) => a.Y > b.Y ? 1 : -1);//y축정렬
            madi_point2.Sort((a, b) => a.Y > b.Y ? 1 : -1);//y축정렬
            madi_point3.Sort((a, b) => a.Y > b.Y ? 1 : -1);//y축정렬            
            
            if (histogram.Height / 3 * 2 - madi_point2[0].X < 10)
            {
                madi_point2.RemoveAt(0);
            }

            Cv.Circle(histogram2, new CvPoint(madi_point1[0].Y, madi_point1[0].X), 5, CvColor.Red);
            Cv.Circle(histogram2, new CvPoint(madi_point2[0].Y, madi_point2[0].X), 5, CvColor.Red);
            Cv.Circle(histogram2, new CvPoint(madi_point3[0].Y, madi_point3[0].X), 5, CvColor.Red);

            Cv.Circle(histogram2, new CvPoint(madi_point1[0].Y, histogram.Height/3), 5, CvColor.Blue);
            Cv.Circle(histogram2, new CvPoint(madi_point2[0].Y, histogram.Height / 3*2), 5, CvColor.Blue);
            Cv.Circle(histogram2, new CvPoint(madi_point3[0].Y, histogram.Height), 5, CvColor.Blue);
            /*
            IplImage finger_madi1 = new IplImage(finger.Width, 40, BitDepth.U8, 1);
            IplImage finger_madi2 = new IplImage(finger.Width, 40, BitDepth.U8, 1);
            IplImage finger_madi3 = new IplImage(finger.Width, finger.Height - madi_point3[0].X + 20, BitDepth.U8, 1);*/
            IplImage finger_madi1 = new IplImage(55, 55, BitDepth.U8, 1);
            IplImage finger_madi2 = new IplImage(55, 55, BitDepth.U8, 1);
            IplImage finger_madi3 = new IplImage(55, 55, BitDepth.U8, 1);
            Cv.SetImageROI(original, new CvRect(0, madi_point1[0].X - 20, finger.Width, 40));
            Cv.Resize(original, finger_madi1);
            Cv.ResetImageROI(original);
            
            Cv.SetImageROI(original, new CvRect(0, madi_point2[0].X - 20, finger.Width, 40));
            Cv.Resize(original, finger_madi2);
            Cv.ResetImageROI(original);

            Cv.SetImageROI(original, new CvRect(0, madi_point3[0].X - 20, finger.Width, finger.Height-madi_point3[0].X+20));
            Cv.Resize(original, finger_madi3);
            Cv.ResetImageROI(original);

            const int histSize = 256;
            float[] range = { 0, 256 };
            float[][] ranges = { range };

            IplImage met1_histimg = new IplImage(new CvSize(255,hist_height),BitDepth.U8,1);
            IplImage pph1_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage dph1_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage met3_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage pph3_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage mph3_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage dph3_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage met5_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage pph5_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage mph5_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage dph5_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);

            CvHistogram met1_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram pph1_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram dph1_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram met3_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram pph3_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram mph3_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram dph3_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram met5_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram pph5_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram mph5_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram dph5_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);

            if (index == 1)
            {
                met1 = finger_madi3.Clone();
                pph1 = finger_madi2.Clone();
                dph1 = finger_madi1.Clone();
                /*
                DrawHist(met1_histimg, met1_hist, histSize, CalcHist(met1, met1_hist));
                DrawHist(pph1_histimg, pph1_hist, histSize, CalcHist(pph1, pph1_hist));
                DrawHist(dph1_histimg, dph1_hist, histSize, CalcHist(dph1, dph1_hist));
                
                met1_histimg.SaveImage("met1_hist.png");
                pph1_histimg.SaveImage("pph1_hist.png");
                dph1_histimg.SaveImage("dph1_hist.png");
                */
                finger_madi1.SaveImage(saveFolder + "/dph1.png");
                finger_madi2.SaveImage(saveFolder + "/pph1.png");
                finger_madi3.SaveImage(saveFolder + "/met1.png");
            }
            if(index == 3)
            {
                met3 = finger_madi3.Clone();
                pph3 = finger_madi3.Clone();
                mph3 = finger_madi2.Clone();
                dph3 = finger_madi1.Clone();
                /*
                DrawHist(met3_histimg, met3_hist, histSize, CalcHist(met3, met3_hist));
                DrawHist(pph3_histimg, pph3_hist, histSize, CalcHist(pph3, pph3_hist));
                DrawHist(mph3_histimg, dph3_hist, histSize, CalcHist(mph3, dph3_hist));
                DrawHist(dph3_histimg, dph3_hist, histSize, CalcHist(dph3, dph3_hist));
                
                met3_histimg.SaveImage("met3_hist.png");
                pph3_histimg.SaveImage("pph3_hist.png");
                mph3_histimg.SaveImage("mph3_hist.png");
                dph3_histimg.SaveImage("dph3_hist.png");
                */
                finger_madi1.SaveImage(saveFolder + "/dph3.png");
                finger_madi2.SaveImage(saveFolder + "/mph3.png");
                finger_madi3.SaveImage(saveFolder + "/pph3.png");
                finger_madi3.SaveImage(saveFolder + "/met3.png");
            }
            if (index == 5)
            {
                met5 = finger_madi3.Clone();
                pph5 = finger_madi3.Clone();
                mph5 = finger_madi2.Clone();
                dph5 = finger_madi1.Clone();
                /*
                DrawHist(met5_histimg, met5_hist, histSize, CalcHist(met5, met5_hist));
                DrawHist(pph5_histimg, pph5_hist, histSize, CalcHist(pph5, pph5_hist));
                DrawHist(mph5_histimg, dph5_hist, histSize, CalcHist(mph5, dph5_hist));
                DrawHist(dph5_histimg, dph5_hist, histSize, CalcHist(dph5, dph5_hist));
                
                met5_histimg.SaveImage("met5_hist.png");
                pph5_histimg.SaveImage("pph5_hist.png");
                mph5_histimg.SaveImage("mph5_hist.png");
                dph5_histimg.SaveImage("dph5_hist.png");
                */
                finger_madi1.SaveImage(saveFolder + "/dph5.png");
                finger_madi2.SaveImage(saveFolder + "/mph5.png");
                finger_madi3.SaveImage(saveFolder + "/pph5.png");
                finger_madi3.SaveImage(saveFolder + "/met5.png");
            }
            
        }
        
        private IplImage ContrastStretch(IplImage img)
        {
            //contrast stretching 할 때 로워바운드 어퍼바운드 비율 
            double LOWER_PERCENT = 0.70; //60 -> 작아져야함
            double UPPER_PERCENT = 0.99; //99

            int lower_bound = 0;
            int upper_bound = 0;
            int sum = 0;
            int[] hist_tot = new int[256];
            int[] hist_tmp = new int[256];

            IplImage stretching = new IplImage(img.WidthStep, img.Height * 50, BitDepth.U8, 1);
            IplImage stretched = new IplImage(img.WidthStep, img.Height, BitDepth.U8, 1);
            stretched.Zero();
            stretching.Zero();
            Cv.Resize(img, stretching);
            int width = stretching.Width;
            int height = stretching.Height;
            double num_px = width * height / 50; //50 조각으로 나눠 각각 대비처리
            //히스토그램 생성
            int[] hist_arr = new int[256];

            for (int p = 1; p < 51; p++)
            {
                ///////////////////////////// 대비 세부조절
                if (p > 20 && p <= 35)
                    // UPPER_PERCENT -= 0.007;
                    LOWER_PERCENT -= 0.001;
                if (p > 35)
                    LOWER_PERCENT += 0.02; ; //0.02
                ////////////////////////////

                for (int i = (p - 1) * (height / 50); i < p * (height / 50); ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        hist_arr[(int)Cv.GetReal2D(stretching, i, j)]++;
                        sum += (int)Cv.GetReal2D(stretching, i, j);
                    }
                }
                double avg = sum / num_px;
                //히스토그램 cdf 만들기
                int[] hist_cdf = new int[256];  //누적함수
                hist_cdf[0] = hist_arr[0];

                for (int k = 1; k < 256; ++k)
                {
                    hist_cdf[k] = hist_arr[k] + hist_cdf[k - 1];
                }

                for (int k = 0; k < 256; ++k) //lower bound
                {
                    if ((double)hist_cdf[k] / (width * height / 50) > LOWER_PERCENT)
                    {
                        lower_bound = k;
                        break;
                    }

                }
                for (int k = 255; k >= 0; --k) //upper bound
                {
                    if ((double)hist_cdf[k] / (width * height / 50) < UPPER_PERCENT)
                    {
                        upper_bound = k;
                        break;
                    }
                }
                for (int i = (p - 1) * (height / 50); i < p * (height / 50); ++i)
                {
                    for (int k = 0; k < width; ++k)
                    {
                        int temp_val;
                        temp_val = (int)((double)(Cv.GetReal2D(stretching, i, k) - lower_bound) / (double)(upper_bound - lower_bound) * 255.0);

                        if (temp_val < 0)
                            temp_val = 0;
                        if (temp_val > 255)
                            temp_val = 255;
                        if (avg < 50)
                            temp_val -= (int)(temp_val * 1.5);
                        Cv.SetReal2D(stretching, i, k, temp_val);
                    }
                }
                Array.Clear(hist_arr, 0, hist_arr.Length);
                Array.Clear(hist_cdf, 0, hist_cdf.Length);
                LOWER_PERCENT -= 0.01; // 대비 세부조절
                UPPER_PERCENT -= 0.002;// 대비 세부조절
            }
            Cv.Resize(stretching, stretched);
            //stretched = img;

            //Cv.Smooth(stretched, stretched, SmoothType.Gaussian);
            //Cv.Smooth(stretched, stretched, SmoothType.Median);
            // Cv.AdaptiveThreshold(stretched, stretched, 250, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 7, 0); //이진화
            Cv.NamedWindow("contrast");
            Cv.ShowImage("contrast", stretched);
            return stretched;
        }

        private CvPoint2D32f rotate_coor(double angle, CvPoint2D32f coor, double center1, double center2)
        {
            double q = -angle * Math.PI / 180;
            double cosq = Math.Cos(q);
            double sinq = Math.Sin(q);
            double tempx = coor.X;
            double tempy = coor.Y;
            CvPoint2D32f rotate;
            tempx = tempx - center1;
            tempy = tempy - center2;
            rotate.X = (float)(tempx * cosq - tempy * sinq);
            rotate.Y = (float)(tempy * cosq + tempx * sinq);
            rotate.X = (float)(rotate.X + center1);
            rotate.Y = (float)(rotate.Y + center2);
            return rotate;
        }

        private double CalAngleFromPoints(CvPoint2D32f firstpos, CvPoint2D32f secondpos)
        {
            double fangle;
            double fdx = firstpos.X - secondpos.X;
            double fdy = firstpos.Y - secondpos.Y;
            double drad = Math.Atan2(fdy, fdx);
            fangle = (drad * 180) / 3.14159265;
            return 90+fangle+5;
        }

        //요골,척골 분리 알고리즘
        private void R_U_Find(IplImage src)
        {
            IplImage dst = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage dst2 = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage dst3 = new IplImage(450, 450, BitDepth.U8, 3);
            Cv.CvtColor(src, dst, ColorConversion.BgrToGray);

            //Cv.SubS(dst, 40, dst);
            Cv.Smooth(dst, dst, SmoothType.Gaussian);
            Cv.Threshold(dst, dst2, 0, 255, ThresholdType.Otsu);
            Cv.Dilate(dst2, dst2);
            Cv.Dilate(dst2, dst2);
            Cv.Dilate(dst2, dst2);
            CvSeq<CvPoint> contours;
            CvMemStorage storage = new CvMemStorage();
            //윤곽선 추출
            Cv.FindContours(dst2, storage, out contours, CvContour.SizeOf, ContourRetrieval.External, ContourChain.ApproxSimple);
            contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 3, true);
            //윤곽선 그리기
            Cv.DrawContours(dst3, contours, CvColor.Green, CvColor.Red, 3);

            //roi 좌표 값 저장
            int x = 20;
            int y = (int)(line_point1.Y + (450 - line_point1.Y) / 4);
            //roi 넓이, 높이 값 저장
            int width = (int)line_point1.X - x;
            int height = 450 - y;
            IplImage dst4 = new IplImage(width, height, BitDepth.U8, 3);
            Cv.SetImageROI(dst3, new CvRect(x, y, width, height));
            Cv.Resize(dst3, dst4);

            IplImage dst5 = new IplImage(width, height, BitDepth.U8, 1);
            IplImage dst6 = new IplImage(width, height, BitDepth.U8, 3);
            IplImage clone_dst = new IplImage(width, height, BitDepth.U8, 3);

            IplImage eigimg = new IplImage(width, height, BitDepth.F32, 1);
            IplImage tempimg = new IplImage(width, height, BitDepth.F32, 1);

            CvPoint2D32f[] corners;//코너 저장 변수
            int cornercount = 150;//코너 갯수

            Cv.CvtColor(dst4, dst5, ColorConversion.BgrToGray);// 그레이컬러 변경

            Cv.GoodFeaturesToTrack(dst5, eigimg, tempimg, out corners, ref cornercount, 0.001, 1, null, 3, true, 0.001);
            Cv.FindCornerSubPix(dst5, corners, cornercount, new CvSize(3, 3), new CvSize(-1, -1), new CvTermCriteria(20, 0.03));
            float xsum = 0;
            Cv.SetImageROI(src, new CvRect(x, y, width, height));//관심영역 추출
            Cv.Resize(src, dst6);
            Cv.ResetImageROI(src);
            for (int i = 0; i < cornercount; i++)
            {
                xsum = corners[i].X + xsum;
                Cv.Circle(dst5, corners[i], 3, new CvColor(255, 255, 255));//코너 그리기
            }
            float avr_x = 0;//추출된 코너의 x좌표 평균을 구하여 왼쪽, 오른쪽 나누기 위한 변수
            List<CvPoint2D32f> left_point = new List<CvPoint2D32f>();//왼쪽코너
            List<CvPoint2D32f> right_point = new List<CvPoint2D32f>();//오른쪽코너

            avr_x = xsum / cornercount;
            for (int i = 0; i < cornercount; i++)
            {
                if (corners[i].X < avr_x)
                {
                    left_point.Add(corners[i]);
                }
                else
                {
                    right_point.Add(corners[i]);
                }
            }

            left_point.Sort((a, b) => a.Y > b.Y ? 1 : -1);//y축정렬
            right_point.Sort((a, b) => a.Y > b.Y ? 1 : -1);//y축정렬
            Cv.Line(dst5, line_point1, line_point2, new CvColor(255, 255, 255));//손목점을 구하기 위한 라인 그리기
            double max_distance = 0;//라인에서 가장 먼 거리의 점이 손목점
            
            int max_i = 0;
            for (int i = 1; i < right_point.Count - 1; i++)
            {
                //점과 직선의 방정식을 이용하여 거리를 계산하는 식을 이용하여 거리 계산
                double div1 = Math.Sqrt(Math.Pow((line_point2.Y - line_point1.Y), 2) + Math.Pow((line_point1.X - line_point2.X), 2));
                double div2 = Math.Abs((line_point2.Y - line_point1.Y) * right_point[i].X + (line_point1.X - line_point2.X) * right_point[i].Y
                    + (line_point1.Y - line_point2.Y) * line_point1.X + (line_point2.X - line_point1.X) * line_point1.Y);
                double distance = div2 / div1;
                if (max_distance < distance)
                {
                    max_distance = distance;
                    max_i = i;
                }
            }

            clone_dst = dst6.Clone();
            Cv.SetImageROI(dst6, new CvRect(0, (int)right_point[max_i].Y, width, 120));
            Cv.SetImageROI(clone_dst, new CvRect(0, (int)right_point[max_i].Y - 40, width, 120));
            IplImage dst7 = new IplImage(width, 120, BitDepth.U8, 3);
            IplImage resize_clone = new IplImage(width, 120, BitDepth.U8, 3);
            IplImage resize_clone2 = new IplImage(width, 120, BitDepth.U8, 3);
            IplImage dst8 = new IplImage(width, 120, BitDepth.U8, 1);
            IplImage label = new IplImage(width, 120, BitDepth.U8, 3);

            Cv.Resize(dst6, dst7);
            Cv.Resize(clone_dst, resize_clone);
            Cv.Resize(clone_dst, resize_clone2);

            Cv.CvtColor(dst7, dst8, ColorConversion.BgrToGray);
            Cv.SubS(dst8, 80, dst8);
            Cv.AdaptiveThreshold(dst8, dst8, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 105, -5);

            Cv.Erode(dst8, dst8);
            Cv.Erode(dst8, dst8);
            CvBlobs blobs = new CvBlobs();
            blobs.Label(dst8);//레이블링
            int nofilter_count = blobs.Count;
            blobs.FilterByArea(1000, 10000);//레이블의 영역제한
            blobs.RenderBlobs(dst8, label);//레이블 그리기
            List<int> bone = new List<int>();
            for (int i = 1; i <= nofilter_count; i++)
            {
                try
                {
                    Console.WriteLine(blobs[i].Centroid);//일부로 catch문으로 가게하는 출력문
                    bone.Add(i);//예외발생하지 않을 시 해당 i에 레이블이 있으므로 저장
                }
                catch (KeyNotFoundException)
                {

                }
            }

            //radius = new IplImage(blobs[bone[0]].MaxX - blobs[bone[0]].MinX + 25, blobs[bone[0]].MaxY - blobs[bone[0]].MinY, BitDepth.U8, 3);//요골
            //ulna = new IplImage(blobs[bone[0]].MaxX - blobs[bone[0]].MinX + 25, blobs[bone[0]].MaxY - blobs[bone[0]].MinY, BitDepth.U8, 3);//척골
            
            if (blobs[bone[0]].MinX < blobs[bone[1]].MinX)//첫번째 점이 두번째점 보다 작으면 왼쪽이므로 왼쪽이 척골
            {
                Cv.SetImageROI(resize_clone, new CvRect(blobs[bone[0]].MinX-15, blobs[bone[0]].MinY, blobs[bone[0]].MaxX - blobs[bone[0]].MinX+25, blobs[bone[0]].MaxY - blobs[bone[0]].MinY));//작은점이 척골
                Cv.SetImageROI(resize_clone2, new CvRect(blobs[bone[1]].MinX-15, blobs[bone[1]].MinY, blobs[bone[1]].MaxX - blobs[bone[1]].MinX+30, blobs[bone[1]].MaxY - blobs[bone[1]].MinY));//큰점이 요골  
            }
            else// 두번째점이 첫번쨈보다 더 작으면 두번째점이 척골 첫번째점이 요골
            {
                Cv.SetImageROI(resize_clone2, new CvRect(blobs[bone[0]].MinX-15, blobs[bone[0]].MinY, blobs[bone[0]].MaxX - blobs[bone[0]].MinX+20, blobs[bone[0]].MaxY - blobs[bone[0]].MinY));//작은점이 요골
                Cv.SetImageROI(resize_clone, new CvRect(blobs[bone[1]].MinX-15, blobs[bone[1]].MinY, blobs[bone[1]].MaxX - blobs[bone[1]].MinX+30, blobs[bone[1]].MaxY - blobs[bone[1]].MinY)); //큰점이 척골
            }
            //이미지 리사이즈
            Cv.Resize(resize_clone, ulna);
            Cv.Resize(resize_clone2, radius);
            IplImage raidus_gray = new IplImage(new CvSize(radius.Width, radius.Height), BitDepth.U8, 1);
            IplImage ulna_gray = new IplImage(new CvSize(ulna.Width, ulna.Height), BitDepth.U8, 1);
            Cv.CvtColor(radius, raidus_gray, ColorConversion.BgrToGray);
            Cv.CvtColor(ulna, ulna_gray, ColorConversion.BgrToGray);
            const int histSize = 256;
            float[] range = { 0, 256 };
            float[][] ranges = { range };

            IplImage radius_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            IplImage ulna_histimg = new IplImage(new CvSize(255, hist_height), BitDepth.U8, 1);
            CvHistogram radius_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);
            CvHistogram ulna_hist = new CvHistogram(new int[] { histSize }, HistogramFormat.Array, ranges, true);

            DrawHist(radius_histimg, radius_hist, histSize, CalcHist(raidus_gray, radius_hist));
            DrawHist(ulna_histimg, ulna_hist, histSize, CalcHist(ulna_gray, ulna_hist));

            //radius_histimg.SaveImage("radius_histimg.png");
            //ulna_histimg.SaveImage("ulna_histimg.png");
            ulna.SaveImage(saveFolder+"/ulna.png");
            radius.SaveImage(saveFolder + "/radius.png");
            /*
            Cv.NamedWindow("ulna");
            Cv.ShowImage("ulna", ulna);
            Cv.NamedWindow("raidus");
            Cv.ShowImage("raidus", radius);
            */
        }
        
        //서버, PC에 이미지 파일 저장
        private void SaveFile(IplImage dst)
        {
            DirectoryInfo di = new DirectoryInfo("E:/" + saveFolder);
            if (di.Exists == false)
            {
                di.Create();
            }
            string fileName = "asdf.png";
            dst.SaveImage("E:/" + saveFolder + "/" + fileName);

            NameValueCollection param = new NameValueCollection();
            param.Add("path", saveFolder);
            System.Net.WebClient Client = new System.Net.WebClient();
            Client.Headers.Add("Content-Type", "binary/octet-stream"); Client.QueryString = param;
            byte[] result = Client.UploadFile("http://contentsbox.co.kr" + "/au/uploadfile.php", "POST", "E:/" + saveFolder + "/" + fileName);
            string str = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
        }
        //이미지에서 손바닥만 추출하기
        private IplImage Crop_image(IplImage src)
        {
            IplImage dst = new IplImage(450, 450, BitDepth.U8, 3);
            IplImage dst2 = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage dst3 = new IplImage(450, 450, BitDepth.U8, 1);
            Cv.Resize(src, dst);
            IplImage result = new IplImage(450, 450, BitDepth.U8, 3);
            IplImage result2 = new IplImage(450, 450, BitDepth.U8, 3);
            IplImage result3 = new IplImage(450, 450, BitDepth.U8, 1);
            IplImage result4 = new IplImage(450, 450, BitDepth.U8, 3);
            //graycolor 변환
            Cv.CvtColor(dst, dst2, ColorConversion.BgrToGray);
            Cv.SubS(dst2, 40, dst2);
            //이진화
            Cv.AdaptiveThreshold(dst2, dst2, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 255, -1);
            Cv.Dilate(dst2, dst2);
            Cv.Dilate(dst2, dst2);
            Cv.Dilate(dst2, dst2);
            Cv.Dilate(dst2, dst2);
            Cv.Dilate(dst2, dst2);

            dst3 = dst2.Clone();
            CvSeq<CvPoint> contours, contours2, contours3;
            CvMemStorage storage = new CvMemStorage();
            //윤곽선 추출
            Cv.FindContours(dst2, storage, out contours, CvContour.SizeOf, ContourRetrieval.External, ContourChain.ApproxSimple);
            contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 3, true);
            //for문을 위한 복사
            contours2 = contours;
            contours3 = contours;
            //윤곽선 그리기
            Cv.DrawContours(result, contours, CvColor.Green, CvColor.Red, 3);
            CvSeq<CvPoint> first_contour;

            int i;
            double contour_area = 0;
            int contour_max = 0;
            for (first_contour = contours; contours != null; contours = contours.HNext)
            {
                //윤곽선의 최대값 구하기
                if (contour_area > contours.ContourArea())
                {
                    contour_max = contours.Total;
                    contour_area = contours.ContourArea();
                }
            }

            CvPoint[] ptseq = new CvPoint[contour_max];

            for (first_contour = contours2; contours2 != null; contours2 = contours2.HNext)
            {
                //최대값이면 컨벡스헐을 구하기 위한 좌표 저장
                if (contour_area == contours2.ContourArea())
                {
                    for (i = 0; i < contours2.Total; i++)
                    {
                        CvPoint? pt = Cv.GetSeqElem<CvPoint>(contours2, i);
                        ptseq[i] = new CvPoint { X = pt.Value.X, Y = pt.Value.Y };
                    }
                }
            }

            CvPoint[] hull;
            int[] hulls;
            
            Cv.ConvexHull2(ptseq, out hull, ConvexHullOrientation.Counterclockwise);
            Cv.ConvexHull2(ptseq, out hulls, ConvexHullOrientation.Counterclockwise);   
                     
            CvPoint pt0 = hull[hull.Length - 1];
            CvSeq<CvConvexityDefect> defect =  Cv.ConvexityDefects(contours3, hulls);
            
            DarwDefects(dst, defect);
            //사각형으로 자르기 위한 pt저장변수선언
            CvPoint rec_pt_max = new CvPoint(0, 0);
            CvPoint rec_pt_min = new CvPoint(1000, 1000);
            c_point.Clear();
            foreach (CvPoint pt in hull)
            {
                //컨벡스헐 라인 그리기
                if(pt.Y<450/7*4)
                    c_point.Add(pt);
                
                /*
                Cv.Line(dst, pt0, pt, CvColor.Green);
                pt0 = pt;
                */
                //사각형으로 자르기 위한 pt저장
                if (rec_pt_max.X < pt.X)
                {
                    rec_pt_max.X = pt.X;
                    line_point1 = pt;
                }                    
                if (rec_pt_max.Y < pt.Y)
                {
                    rec_pt_max.Y = pt.Y;
                    line_point2 = pt;
                }
                    
                if (rec_pt_min.X > pt.X)
                    rec_pt_min.X = pt.X;
                if (rec_pt_min.Y > pt.Y)
                    rec_pt_min.Y = pt.Y;
            }
            rec_pt_min.Y = rec_pt_min.Y - 5;
            temp = rec_pt_min.Y;
            List<int> remove_point = new List<int>();
            c_point.Sort((a, b) => a.X > b.X ? 1 : -1);//x축정렬
            
            //쓰레기값 제거
            for(int k = c_point.Count-1; k>=0; k--)
            {                   
                if (c_point[k].Y > c_point[c_point.Count - 1].Y)
                    c_point.RemoveAt(k);
            }
            //손가락 끝점 클리어
            finger1.Clear();
            finger2.Clear();
            finger3.Clear();
            finger4.Clear();
            finger5.Clear();
            double distance = 0;
            double x_distance = 0;
            double y_distance = 0;
            int finger_count = 1;//손가락의 번째수 카운트 
            //손가락 끝점 추출
            for (int k = c_point.Count - 1; k >= 0; k--)
            {
                if (k != 0)
                {
                    x_distance = Math.Pow(c_point[k].X - c_point[k - 1].X, 2);
                    y_distance = Math.Pow(c_point[k].Y - c_point[k - 1].Y, 2);
                    distance = Math.Sqrt(x_distance + y_distance);//점과 점사이 거리계산                    
                }

                if (distance <= 35)//거리가 35보다 작은경우
                {
                    if (finger_count == 5)//새끼손가락일 경우
                    {
                        finger5.Add(c_point[k]);
                        break;
                    }
                        
                }
                else//거리가 35보다 큰경우이면 다른손가락이라고 생각
                {
                    if (finger_count == 1)//첫번째
                        finger1.Add(c_point[k]);
                    else if (finger_count == 2)//두번째
                        finger2.Add(c_point[k]);
                    else if (finger_count == 3)//세번째
                        finger3.Add(c_point[k]);
                    else if (finger_count == 4)//네번째
                        finger4.Add(c_point[k]);
                    else if (finger_count == 5)//다섯번째
                    {
                        finger5.Add(c_point[k]);                        
                        break;//다섯번째까지 모두 추가하면 for문 break
                    }   
                    finger_count++;
                }
            }
            for(int k = 0; k<defect_point.Count; k++)
            {
                defect_point[k] = coor_trans(defect_point[k], rec_pt_min, rec_pt_max);
            }

            line_point1 = coor_trans(line_point1, rec_pt_min, rec_pt_max);
            line_point2 = coor_trans(line_point2, rec_pt_min, rec_pt_max);

            finger1[0] = coor_trans(finger1[0], rec_pt_min, rec_pt_max);
            finger2[0] = coor_trans(finger2[0], rec_pt_min, rec_pt_max);
            finger3[0] = coor_trans(finger3[0], rec_pt_min, rec_pt_max);
            finger4[0] = coor_trans(finger4[0], rec_pt_min, rec_pt_max);
            finger5[0] = coor_trans(finger5[0], rec_pt_min, rec_pt_max);

            Cv.SetImageROI(dst, new CvRect(rec_pt_min, new CvSize(rec_pt_max.X - rec_pt_min.X, rec_pt_max.Y - rec_pt_min.Y)));
            Cv.Resize(dst, result2);

            return result2;
        }

        private void DarwDefects(IplImage img, CvSeq<CvConvexityDefect> defect)
        {
            defect_point.Clear();
            int count = 0;
            foreach(CvConvexityDefect item in defect)
            {
                CvPoint p1 = item.Start, p2 = item.End;
                double dist = GetDistance(p1, p2);
                CvPoint2D64f mid = GetMidpoint(p1, p2);
                //img.DrawLine(p1, p2, CvColor.White, 3);
                //img.DrawCircle(item.DepthPoint, 5, CvColor.Green, -1);
                if (count < 4)
                    defect_point.Add(item.DepthPoint);
                if(count==5)
                    defect_point.Add(item.DepthPoint);
                //img.DrawLine(mid, item.DepthPoint, CvColor.White, 1);
                count++;
            }
        }

        private double GetDistance(CvPoint p1, CvPoint p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private CvPoint2D64f GetMidpoint(CvPoint p1, CvPoint p2)
        {
            return new CvPoint2D64f
            {
                X = (p1.X + p2.X) / 2.0,
                Y = (p1.Y + p2.Y) / 2.0
            };

        }

        private CvPoint2D32f coor_trans(CvPoint2D32f coord, CvPoint rect_min, CvPoint rect_max)
        {
            CvPoint2D32f trans_coor = new CvPoint2D32f((coord.X - rect_min.X) * 450 / (rect_max.X - rect_min.X), (coord.Y - rect_min.Y) * 450 / (rect_max.Y - rect_min.Y));
            return trans_coor;
        }

        private void tw3picturInit()  //TW3 이미지박스 초기화
        {
            pB_TW3B.Image = null;
            pB_TW3C.Image = null;
            pB_TW3D.Image = null;
            pB_TW3E.Image = null;
            pB_TW3F.Image = null;
            pB_TW3G.Image = null;
            pB_TW3H.Image = null;
            pB_TW3I.Image = null;
            
        }

        /// <summary>
        /// TW3 이미지 불러오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pB_radius_Click(object sender, EventArgs e) //1. 요골
        {
            if(temp == 84 || Global.search_flag == true)
            {
                Console.WriteLine("a");
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if(temp == 72 || Global.search_flag == true)
            {
                Console.WriteLine("b");
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = true;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();

            //const string imageCase = @"Image\temp.jpg";

            const string imageCase1 = "image/F/1 ra/F-b_ra16.jpg";
            const string imageCase2 = "image/F/1 ra/F-b_ra21.jpg";
            const string imageCase3 = "image/F/1 ra/F-b_ra30.jpg";
            const string imageCase4 = "image/F/1 ra/F-b_ra39.jpg";
            const string imageCase5 = "image/F/1 ra/F-b_ra59.jpg";
            const string imageCase6 = "image/F/1 ra/F-b_ra87.jpg";
            const string imageCase7 = "image/F/1 ra/F-b_ra138.jpg";
            const string imageCase8 = "image/F/1 ra/F-b_ra213.jpg";

            this.pB_TW3B.Image = System.Drawing.Image.FromFile(imageCase1);
            this.pB_TW3C.Image = System.Drawing.Image.FromFile(imageCase2);
            this.pB_TW3D.Image = System.Drawing.Image.FromFile(imageCase3);
            this.pB_TW3E.Image = System.Drawing.Image.FromFile(imageCase4);
            this.pB_TW3F.Image = System.Drawing.Image.FromFile(imageCase5);
            this.pB_TW3G.Image = System.Drawing.Image.FromFile(imageCase6);
            this.pB_TW3H.Image = System.Drawing.Image.FromFile(imageCase7);
            this.pB_TW3I.Image = System.Drawing.Image.FromFile(imageCase8);
            
        }

        private void pB_ulna_Click(object sender, EventArgs e) //2. 척골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }

            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("image/F/2 ul/F-b_ul27.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul30.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul32.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul40.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul58.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul107.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            
        }

        private void pB_Met1_Click(object sender, EventArgs e)//3. 제1지 중수골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi6.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi9.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi14.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi21.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi26.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi36.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi49.jpg");
            this.pB_TW3I.Image = System.Drawing.Image.FromFile("Image/F/3 fi/F-b_fi67.jpg");
            
        }

        private void pB_Met3_Click(object sender, EventArgs e)  //4. 제3지 중수골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th4.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th5.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th9.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th12.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th19.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th31.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th43.jpg");
            this.pB_TW3I.Image = System.Drawing.Image.FromFile("Image/F/4 th/F-b_th52.jpg");

        }

        private void pB_Met5_Click(object sender, EventArgs e) //5. 제5지 중수골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");

        }

        private void pB_Pph1_Click(object sender, EventArgs e) //6. 제1기절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr7.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr8.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr11.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr17.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr26.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr38.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr52.jpg");
            this.pB_TW3I.Image = System.Drawing.Image.FromFile("Image/F/6 pr/F-b_pr67.jpg");

        }

        private void pB_Pph3_Click(object sender, EventArgs e) //7. 제3지 기절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3I.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
        }

        private void pB_Pph5_Click(object sender, EventArgs e)//8. 제3지 기절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");

        }

        private void pB_Mph3_Click(object sender, EventArgs e)//9. 제3지 중절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3I.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");

        }

        private void pB_Mph5_Click(object sender, EventArgs e)//10.제5지 중절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
        }

        private void pB_Dph1_Click(object sender, EventArgs e)//11.제5지 중절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3I.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
        }

        private void pB_Dph3_Click(object sender, EventArgs e)//12.제5지 중절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3D.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3E.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3F.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3G.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3H.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3I.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");

        }

        private void pB_Dph5_Click(object sender, EventArgs e)//13.제5지 중절골
        {
            if (temp == 84 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else if (temp == 72 || Global.search_flag == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            tw3picturInit();
            this.pB_TW3B.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");
            this.pB_TW3C.Image = System.Drawing.Image.FromFile("Image/F/2 ul/F-b_ul181.jpg");

        }

        private static System.Drawing.Size GetSize(System.Drawing.Size maxSize, System.Drawing.Size size)
        {
            double ratioWidth = (double)maxSize.Width / size.Width;
            double ratioHeight = (double)maxSize.Height / size.Height;
            double ratio = Math.Min(ratioWidth, ratioHeight);
            return new System.Drawing.Size((int)Math.Floor(size.Width * ratio), (int)Math.Floor(size.Height * ratio));

        }
        

        private void cal_maxheight_Click(object sender, EventArgs e)
        {
            string[] sub_day = (reg_date.Value - birth_date.Value).ToString().Split('.');
            double isub_day = Convert.ToDouble(sub_day[0]);
            double man_age = Math.Round(isub_day / 365.0, 1);
            this.man_age.Text = man_age.ToString();
            man_bone.Text = Math.Round(man_age - bone_age,1).ToString();
            if (child_cb.SelectedItem.ToString() == "남")
            {
                if (man_age >= 4 && man_age <= 9)
                {
                    double pheight = Convert.ToDouble(p_height.Text.ToString());
                    double am6 = man_age * 6;
                    double m_height = pheight + 97 - am6;
                    max_height.Text = m_height.ToString();
                }
                else if (man_age >= 10 && man_age <= 17)
                {
                    double a = -(0.0402 - 0.00632 * (man_age - 14) - 0.00155 * Math.Pow(man_age - 14, 2) + 0.00019 * Math.Pow(man_age - 14, 2) * (man_age - 14));
                    double b = 37.67 - 5.50 * (man_age - 14) - 0.799 * Math.Pow(man_age - 14, 2);

                    double pheight = Convert.ToDouble(p_height.Text.ToString());
                    double amr = a * Convert.ToDouble(rus_text.Text.ToString());//rus
                    double m_height = pheight + amr + b;
                    m_height = Math.Round(m_height, 1);
                    max_height.Text = m_height.ToString();//최대신장 출력
                }

            }
            else if (child_cb.SelectedItem.ToString() == "여")
            {
                if (man_age >= 4 && man_age <= 6)
                {
                    double pheight = Convert.ToDouble(p_height.Text.ToString());
                    double am6 = man_age * 6;
                    double m_height = pheight + 85 - am6;
                    max_height.Text = m_height.ToString();
                }
                else if (man_age >= 7 && man_age <= 15)
                {
                    if (mer_false.Checked == true)
                    {
                        double a = -(0.0436 - 0.00379 * (man_age - 11));
                        double b = 44.02 - 3.784 * (man_age - 11) - 0.0247 * Math.Pow(man_age - 11, 2) - 0.0365 * Math.Pow(man_age - 11, 2) * (man_age - 11);

                        double pheight = Convert.ToDouble(p_height.Text.ToString());
                        double amr = a * Convert.ToDouble(rus_text.Text.ToString());//rus
                        double m_height = pheight + amr + b;
                        m_height = Math.Round(m_height, 1);
                        max_height.Text = m_height.ToString();
                    }
                    else if (mer_true.Checked == true || man_age > 15)
                    {
                        double d = 16.54 - 1.94 * (man_age - 11) + 0.230 * Math.Pow(man_age - 11, 2);

                        double pheight = Convert.ToDouble(p_height.Text.ToString());
                        double amr = -0.011 * Convert.ToDouble(rus_text.Text.ToString());
                        double m_height = pheight + amr + d;
                        m_height = Math.Round(m_height, 1);
                        max_height.Text = m_height.ToString();//최대신장 출력
                    }
                }
            }
        }        

        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = true;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = true;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = true;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = true;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = true;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = true;
            radioButton8.Checked = false;
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = true;
        }

        private void Cal_Rus()
        {
            if (temp == 84)
            {
                bone_matched[0] = "E";
                bone_matched[1] = "C";
                bone_matched[2] = "C";
                bone_matched[3] = "D";
                bone_matched[4] = "D";
                bone_matched[5] = "D";
                bone_matched[6] = "D";
                bone_matched[7] = "D";
                bone_matched[8] = "D";
                bone_matched[9] = "D";
                bone_matched[10] = "D";
                bone_matched[11] = "D";
                bone_matched[12] = "D";
                for(int i = 0; i<13; i++)
                {
                    if (bone_matched[i] == "A")
                    {
                        bone_matched[i] = "0";
                    }
                    else if(bone_matched[i] == "B")
                    {
                        bone_matched[i] = "1";
                    }
                    else if (bone_matched[i] == "C")
                    {
                        bone_matched[i] = "2";
                    }
                    else if (bone_matched[i] == "D")
                    {
                        bone_matched[i] = "3";
                    }
                    else if (bone_matched[i] == "E")
                    {
                        bone_matched[i] = "4";
                    }
                    else if (bone_matched[i] == "F")
                    {
                        bone_matched[i] = "5";
                    }
                    else if (bone_matched[i] == "G")
                    {
                        bone_matched[i] = "6";
                    }
                    else if (bone_matched[i] == "H")
                    {
                        bone_matched[i] = "7";
                    }
                    else if (bone_matched[i] == "I")
                    {
                        bone_matched[i] = "8";
                    }
                }
                rus = Global.RUS_RADIUS[Convert.ToInt16(bone_matched[0])]+Global.RUS_ULNA[Convert.ToInt16(bone_matched[1])] + Global.RUS_METACARPAL1[Convert.ToInt16(bone_matched[2])] + Global.RUS_METACARPAL3[Convert.ToInt16(bone_matched[3])]
                    + Global.RUS_METACARPAL5[Convert.ToInt16(bone_matched[4])] + Global.RUS_PROXIMAL1[Convert.ToInt16(bone_matched[5])] + Global.RUS_PROXIMAL3[Convert.ToInt16(bone_matched[6])] + Global.RUS_PROXIMAL5[Convert.ToInt16(bone_matched[7])]
                    + Global.RUS_MIDDLE3[Convert.ToInt16(bone_matched[8])] + Global.RUS_MIDDLE3[Convert.ToInt16(bone_matched[9])] + Global.RUS_DISTAL1[Convert.ToInt16(bone_matched[10])] + Global.RUS_DISTAL3[Convert.ToInt16(bone_matched[11])]
                    + Global.RUS_DISTAL5[Convert.ToInt16(bone_matched[12])];
                Console.WriteLine(rus);
            }
            else if(temp == 72)
            {
                bone_matched[0] = "F";
                bone_matched[1] = "C";
                bone_matched[2] = "D";
                bone_matched[3] = "E";
                bone_matched[4] = "E";
                bone_matched[5] = "E";
                bone_matched[6] = "E";
                bone_matched[7] = "E";
                bone_matched[8] = "D";
                bone_matched[9] = "D";
                bone_matched[10] = "E";
                bone_matched[11] = "E";
                bone_matched[12] = "D";

                for (int i = 0; i < 13; i++)
                {
                    if (bone_matched[i] == "A")
                    {
                        bone_matched[i] = "0";
                    }
                    else if (bone_matched[i] == "B")
                    {
                        bone_matched[i] = "1";
                    }
                    else if (bone_matched[i] == "C")
                    {
                        bone_matched[i] = "2";
                    }
                    else if (bone_matched[i] == "D")
                    {
                        bone_matched[i] = "3";
                    }
                    else if (bone_matched[i] == "E")
                    {
                        bone_matched[i] = "4";
                    }
                    else if (bone_matched[i] == "F")
                    {
                        bone_matched[i] = "5";
                    }
                    else if (bone_matched[i] == "G")
                    {
                        bone_matched[i] = "6";
                    }
                    else if (bone_matched[i] == "H")
                    {
                        bone_matched[i] = "7";
                    }
                    else if (bone_matched[i] == "I")
                    {
                        bone_matched[i] = "8";
                    }
                }
                rus = Global.RUS_RADIUS[Convert.ToInt16(bone_matched[0])] + Global.RUS_ULNA[Convert.ToInt16(bone_matched[1])] + Global.RUS_METACARPAL1[Convert.ToInt16(bone_matched[2])] + Global.RUS_METACARPAL3[Convert.ToInt16(bone_matched[3])]
                    + Global.RUS_METACARPAL5[Convert.ToInt16(bone_matched[4])] + Global.RUS_PROXIMAL1[Convert.ToInt16(bone_matched[5])] + Global.RUS_PROXIMAL3[Convert.ToInt16(bone_matched[6])] + Global.RUS_PROXIMAL5[Convert.ToInt16(bone_matched[7])]
                    + Global.RUS_MIDDLE3[Convert.ToInt16(bone_matched[8])] + Global.RUS_MIDDLE3[Convert.ToInt16(bone_matched[9])] + Global.RUS_DISTAL1[Convert.ToInt16(bone_matched[10])] + Global.RUS_DISTAL3[Convert.ToInt16(bone_matched[11])]
                    + Global.RUS_DISTAL5[Convert.ToInt16(bone_matched[12])];
            }
        }
        
        private void cal_bone_age_Click(object sender, EventArgs e)
        {
            int min_gap = 255;
            int min_gap_rus = 0;
            
            if (Global.RUS_BONAGE.ContainsKey(rus) == true)
            {
                bone_age = Global.RUS_BONAGE[rus];
            }
            else
            {
                foreach(KeyValuePair<int,double> RUS_BONAGE in Global.RUS_BONAGE)
                {
                    if(Math.Abs(rus-RUS_BONAGE.Key)<min_gap)
                    {
                        min_gap = Math.Abs(rus - RUS_BONAGE.Key);
                        min_gap_rus = RUS_BONAGE.Key;
                    }
                        
                }
                bone_age = Global.RUS_BONAGE[min_gap_rus];
            }
            Console.WriteLine(bone_age);
            rus_text.Text = rus.ToString();
            ba_text.Text = bone_age.ToString();
        }

        private void btn_chart_search_Click(object sender, EventArgs e)
        {
            ChartSearchForm csf = new ChartSearchForm();
            csf.ShowDialog();
            if (csf.DialogResult == DialogResult.OK)
            {
                insert_data();
            }
            else
            {
                this.DialogResult = csf.DialogResult;
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            connStr = String.Format(connStr, "auxology.mdb");
            OleDbConnection gConn = new OleDbConnection(connStr);
            
            string field1 = "chart_num, child_name, reg_date, child_birth, child_sex, mer_check, child_height, child_weight, image_path";
            string field2 = ", radius_path, ulna_path, met1_path,met3_path, met5_path, pph1_path, pph3_path, pph5_path, mph3_path, mph5_path, dph1_path, dph3_path, dph5_path";
            string field3 = ", radius_checked, ulna_checked, met1_checked, met3_checked, met5_checked, pph1_checked, pph3_checked, pph5_checked, mph3_checked, mph5_checked";
            string field4 = ",dph1_checked, dph3_checked, dph5_checked, rus_score, bone_age, man_age, man_bone, max_height, hand_path";

            string value1 = "@chart_num, @child_name, @reg_date, @child_birth, @child_sex, @mer_check, @child_height, @child_weight, @image_path";
            string value2 = ", @radius_path, @ulna_path, @met1_path,@met3_path, @met5_path, @pph1_path, @pph3_path, @pph5_path, @mph3_path, @mph5_path, @dph1_path, @dph3_path, @dph5_path";
            string value3 = ", @radius_checked, @ulna_checked, @met1_checked, @met3_checked, @met5_checked, @pph1_checked, @pph3_checked, @pph5_checked, @mph3_checked, @mph5_checked";
            string value4 = ", @dph1_checked, @dph3_checked, @dph5_checked, @rus_score, @bone_age, @man_age, @man_bone, @max_height, hand_path";

            string field = field1 + field2 + field3 + field4;
            string value = value1 + value2 + value3 + value4;
            string t_name = "PREDICT_INFO";
            string query = "insert into {0} ({1}) values({2})";
            string mer = "";
            if (mer_false.Checked == true)
            {
                mer = "false";
            }
            else if (mer_true.Checked == true)
            {
                mer = "true";
            }
            else if(mer_false.Checked==false && mer_true.Checked == false)
            {
                mer = "boy";
            }
            query = String.Format(query, t_name, field, value);
            gConn.Open();
            OleDbCommand cmd = new OleDbCommand(query, gConn);
            cmd.Parameters.Add("@chart_num", OleDbType.VarWChar).Value = chartnum_tb.Text;
            cmd.Parameters.Add("@child_name", OleDbType.VarWChar).Value = pname_tb.Text;
            cmd.Parameters.Add("@reg_date", OleDbType.VarWChar).Value = regday;
            cmd.Parameters.Add("@child_birth", OleDbType.VarWChar).Value = birthday;
            cmd.Parameters.Add("@child_sex", OleDbType.VarWChar).Value = child_cb.Text;
            cmd.Parameters.Add("@mer_check", OleDbType.VarWChar).Value = mer;
            cmd.Parameters.Add("@child_height", OleDbType.VarWChar).Value = p_height.Text;
            cmd.Parameters.Add("@child_weight", OleDbType.VarWChar).Value = p_weight.Text;
            cmd.Parameters.Add("@image_path", OleDbType.VarWChar).Value = image_path.Text;
            cmd.Parameters.Add("@radius_path", OleDbType.VarWChar).Value = saveFolder + "/radius.png";
            cmd.Parameters.Add("@ulna_path", OleDbType.VarWChar).Value = saveFolder + "/ulna.png";
            cmd.Parameters.Add("@met1_path", OleDbType.VarWChar).Value = saveFolder + "/met1.png";
            cmd.Parameters.Add("@met3_path", OleDbType.VarWChar).Value = saveFolder + "/met3.png";
            cmd.Parameters.Add("@met5_path", OleDbType.VarWChar).Value = saveFolder + "/met5.png";
            cmd.Parameters.Add("@pph1_path", OleDbType.VarWChar).Value = saveFolder + "/pph1.png";
            cmd.Parameters.Add("@pph3_path", OleDbType.VarWChar).Value = saveFolder + "/pph3.png";
            cmd.Parameters.Add("@pph5_path", OleDbType.VarWChar).Value = saveFolder + "/pph5.png";
            cmd.Parameters.Add("@mph3_path", OleDbType.VarWChar).Value = saveFolder + "/mph3.png";
            cmd.Parameters.Add("@mph5_path", OleDbType.VarWChar).Value = saveFolder + "/mph5.png";
            cmd.Parameters.Add("@dph1_path", OleDbType.VarWChar).Value = saveFolder + "/dph1.png";
            cmd.Parameters.Add("@dph3_path", OleDbType.VarWChar).Value = saveFolder + "/dph3.png";
            cmd.Parameters.Add("@dph5_path", OleDbType.VarWChar).Value = saveFolder + "/dph5.png";
            cmd.Parameters.Add("@radius_checked", OleDbType.VarWChar).Value = bone_matched[0];
            cmd.Parameters.Add("@ulna_checked", OleDbType.VarWChar).Value = bone_matched[1];
            cmd.Parameters.Add("@met1_checked", OleDbType.VarWChar).Value = bone_matched[2];
            cmd.Parameters.Add("@met3_checked", OleDbType.VarWChar).Value = bone_matched[3];
            cmd.Parameters.Add("@met5_checked", OleDbType.VarWChar).Value = bone_matched[4];
            cmd.Parameters.Add("@pph1_checked", OleDbType.VarWChar).Value = bone_matched[5];
            cmd.Parameters.Add("@pph3_checked", OleDbType.VarWChar).Value = bone_matched[6];
            cmd.Parameters.Add("@pph5_checked", OleDbType.VarWChar).Value = bone_matched[7];
            cmd.Parameters.Add("@mph3_checked", OleDbType.VarWChar).Value = bone_matched[8];
            cmd.Parameters.Add("@mph5_checked", OleDbType.VarWChar).Value = bone_matched[9];
            cmd.Parameters.Add("@dph1_checked", OleDbType.VarWChar).Value = bone_matched[10];
            cmd.Parameters.Add("@dph3_checked", OleDbType.VarWChar).Value = bone_matched[11];
            cmd.Parameters.Add("@dph5_checked", OleDbType.VarWChar).Value = bone_matched[12];
            cmd.Parameters.Add("@rus_score", OleDbType.VarWChar).Value = rus_text.Text;
            cmd.Parameters.Add("@bone_age", OleDbType.VarWChar).Value = ba_text.Text;
            cmd.Parameters.Add("@man_age", OleDbType.VarWChar).Value = man_age.Text;
            cmd.Parameters.Add("@man_bone", OleDbType.VarWChar).Value = man_bone.Text;
            cmd.Parameters.Add("@max_height", OleDbType.VarWChar).Value = max_height.Text;
            cmd.Parameters.Add("@hand_path", OleDbType.VarWChar).Value = saveFolder + "/hand.png";
            cmd.ExecuteNonQuery();         
            
        }

        public void insert_data()
        {
            
            connStr = String.Format(connStr, "auxology.mdb");
            OleDbConnection gConn = new OleDbConnection(connStr);
            string query = "select * from PREDICT_INFO where chart_num=@chart_num";
            OleDbCommand cmd = new OleDbCommand(query, gConn);
            cmd.Parameters.AddWithValue("@chart_num", Global.current_chart_num);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
            gConn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                chartnum_tb.Text = dr[0].ToString();
                pname_tb.Text = dr[1].ToString();
                reg_date.Text = dr[2].ToString();
                birth_date.Text = dr[3].ToString();
                child_cb.Text = dr[4].ToString();
                if (dr[5].ToString() == "boy")
                {
                    mer_false.Checked = false;
                    mer_true.Checked = false;
                }
                else if (dr[5].ToString() == "true")
                {
                    mer_true.Checked = true;
                }
                else if (dr[5].ToString() == "false")
                {
                    mer_false.Checked = true;
                }
                p_height.Text = dr[6].ToString();
                p_weight.Text = dr[7].ToString();
                image_path.Text = dr[8].ToString();
                img_path[0] = dr[9].ToString();
                img_path[1] = dr[10].ToString();
                img_path[2] = dr[11].ToString();
                img_path[3] = dr[12].ToString();
                img_path[4] = dr[13].ToString();
                img_path[5] = dr[14].ToString();
                img_path[6] = dr[15].ToString();
                img_path[7] = dr[16].ToString();
                img_path[8] = dr[17].ToString();
                img_path[9] = dr[18].ToString();
                img_path[10] = dr[19].ToString();
                img_path[11] = dr[20].ToString();
                img_path[12] = dr[21].ToString();

                bone_matched[0] = dr[22].ToString();
                bone_matched[1] = dr[23].ToString();
                bone_matched[2] = dr[24].ToString();
                bone_matched[3] = dr[25].ToString();
                bone_matched[4] = dr[26].ToString();
                bone_matched[5] = dr[27].ToString();
                bone_matched[6] = dr[28].ToString();
                bone_matched[7] = dr[29].ToString();
                bone_matched[8] = dr[30].ToString();
                bone_matched[9] = dr[31].ToString();
                bone_matched[10] = dr[32].ToString();
                bone_matched[11] = dr[33].ToString();
                bone_matched[12] = dr[34].ToString();
                rus_text.Text = dr[35].ToString();
                ba_text.Text = dr[36].ToString();
                man_age.Text = dr[37].ToString();
                man_bone.Text = dr[38].ToString();
                max_height.Text = dr[39].ToString();
                img_path[13] = dr[40].ToString();

            }

            regday = Global.spilitDate(reg_date.Text);
            birthday = Global.spilitDate(birth_date.Text);

            for (int i = 0; i < 13; i++)
            {
                Console.WriteLine(img_path[i]);
            }

            pB_radius.Image = new Bitmap(img_path[0]);
            pB_ulna.Image = new Bitmap(img_path[1]);
            pB_Met1.Image = new Bitmap(img_path[2]);
            pB_Met3.Image = new Bitmap(img_path[3]);
            pB_Met5.Image = new Bitmap(img_path[4]);
            pB_Pph1.Image = new Bitmap(img_path[5]);
            pB_Pph3.Image = new Bitmap(img_path[6]);
            pB_Pph5.Image = new Bitmap(img_path[7]);
            pB_Mph3.Image = new Bitmap(img_path[8]);
            pB_Mph5.Image = new Bitmap(img_path[9]);
            pB_Dph1.Image = new Bitmap(img_path[10]);
            pB_Dph3.Image = new Bitmap(img_path[11]);
            pB_Dph5.Image = new Bitmap(img_path[12]);
            xray_preview.ImageIpl = IplImage.FromFile(img_path[13]);
            rus = Convert.ToInt16(rus_text.Text);
            bone_age = Convert.ToDouble(ba_text.Text);
            dr.Close();
            gConn.Close();
        }

        public void initialize_data()
        {
            chartnum_tb.Text = Global.chart_num;
            pname_tb.Text = "";
            reg_date.Text = "";
            birth_date.Text = "";
            child_cb.Text = "";
            mer_false.Checked = false;
            mer_true.Checked = false;
            p_height.Text = "";
            p_weight.Text = "";
            image_path.Text = "";
            pB_radius.Image = null;
            pB_ulna.Image = null;
            pB_Met1.Image = null;
            pB_Met3.Image = null;
            pB_Met5.Image = null;
            pB_Pph1.Image = null;
            pB_Pph3.Image = null; 
            pB_Pph5.Image = null;
            pB_Mph3.Image = null;
            pB_Mph5.Image = null;
            pB_Dph1.Image = null;
            pB_Dph3.Image = null;
            pB_Dph5.Image = null;
            xray_preview.ImageIpl = BitmapConverter.ToIplImage(Properties.Resources.temp);
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            rus_text.Text = "";
            ba_text.Text = "";
            man_age.Text = "";
            man_bone.Text = "";
            max_height.Text = "";
        }

        private void birth_date_CloseUp(object sender, EventArgs e)
        {
            birthday = Global.spilitDate(birth_date.Text);
        }

        private void reg_date_CloseUp(object sender, EventArgs e)
        {
            regday = Global.spilitDate(reg_date.Text);
        }

        public void pinfo_registered_insert_data()
        {
            connStr = String.Format(connStr, "auxology.mdb");
            OleDbConnection gConn = new OleDbConnection(connStr);
            string query = "select chart_num, child_name, reg_date, child_birth, child_sex, child_height, child_weight from p_info where chart_num=@chart_num";
            int temp = Convert.ToInt16(Global.chart_num);
            temp--;
            OleDbCommand cmd = new OleDbCommand(query, gConn);
            cmd.Parameters.AddWithValue("@chart_num", temp.ToString());
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
            gConn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                chartnum_tb.Text = dr[0].ToString();
                pname_tb.Text = dr[1].ToString();
                //regday = dr[2].ToString();
                reg_date.Text = dr[2].ToString();
                birth_date.Text = dr[3].ToString();
                child_cb.Text = dr[4].ToString();
                p_height.Text = dr[5].ToString();
                p_weight.Text = dr[6].ToString();
            }

            //regday = Global.spilitDate(reg_date.Text);
            //reg_date.Text = regday;
            //birthday = Globalfunction.spilitDate(birth_date.Text);

            dr.Close();
            gConn.Close();
        }

        
        ////////////////////////돋보기용 함수들/////////////////////////////

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(magnifier_check.Checked == true && xray_preview.Image != null)
            {
                magnify_picturebox.Height = magnify_picturebox.Width;
                magnify();
                //magnifier();
            }
        }

        private void xray_preview_MouseEnter(object sender, EventArgs e)
        {
            if (magnifier_check.Checked == true && xray_preview.Image != null)
            {
                magnify_picturebox.Visible = true;
                magnify_picturebox.BringToFront();
                timer1.Enabled = true;
            }
        }

        private void xray_preview_MouseLeave(object sender, EventArgs e)
        {
            magnify_picturebox.Visible = false;
            timer1.Enabled = false;
        }

        private void magnify()
        {
            Graphics g;
            Bitmap bmp;

            bmp = new Bitmap(100, 100);
            g = this.CreateGraphics();
            g = Graphics.FromImage(bmp);
            Point pos;
            int posx = MousePosition.X - 50;
            int posy = MousePosition.Y - 50;

            pos = xray_preview.PointToScreen(new Point());

            if (posx < pos.X) posx = pos.X;
            else if (posx > pos.X + xray_preview.Width - 100) posx = pos.X + xray_preview.Width - 100;
            if (posy < pos.Y) posy = pos.Y;
            else if (posy > pos.Y + xray_preview.Height - 100) posy = pos.Y + xray_preview.Height - 100;

            g.CopyFromScreen(posx, posy, 0, 0, new Size(100, 100));
            magnify_picturebox.Image = bmp;
        }
    }
}
