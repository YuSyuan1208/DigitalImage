using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Bitmap p1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //Open Source
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //宣告openFileDialog1 
            openFileDialog1.Filter = "All Files|*.*";
            //設定可以開啟的檔案格式
            //SHOW出圖片囉
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                p1 = new Bitmap(openFileDialog1.FileName);
            //讓p1得到秀出圖片的資訊
            this.pictureBox1.Image = p1;
            //顯示圖片在pictureBox1的框架內
            panel1.Invalidate();
        }
        //Gray
        private void button2_Click(object sender, EventArgs e)
        {
            if (p1 == null)
                return;
                var ImgIn = new ImgModule(p1);
            p1 = ImgIn.gary(p1);
            this.pictureBox1.Image = p1;
        }
        //PepperSalt
        private void button3_Click(object sender, EventArgs e)
        {
            if (p1 == null)
                return;
            var ImgIn = new ImgModule(p1);
            p1 = ImgIn.PepperSalt(p1,5);
            this.pictureBox1.Image = p1;
            panel1.Invalidate();
        }
        //Drawing Histogram
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (p1 == null)
                return;
            var ImgIn = new ImgModule(p1);
            var PixelArray = ImgIn.Histogram(p1);  
            var PixelLength = ImgIn.rgbValues.Length / 3;
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            long MaxPixelNum = 0;
            for (int i = 0; i < 256; i++)
            {
                if (PixelArray[i] > MaxPixelNum) { MaxPixelNum = PixelArray[i]; }
            }
            for (int i = 0; i < 256; i++)
            {
                int LineHight = (int)(((double)PixelArray[i]/ MaxPixelNum) *255);
                e.Graphics.DrawLine(pen, i, panel1.Size.Height, i, panel1.Size.Height - (int)LineHight);
            }
        }
        //HistogramEqulization
        private void button4_Click(object sender, EventArgs e)
        {
            if (p1 == null)
                return;
            var ImgIn = new ImgModule(p1);
            p1 = ImgIn.HistogramProcessing(p1,200,50,Constants.HE);
            this.pictureBox1.Image = p1;
            panel1.Invalidate();
        }
        //LowPassFilter
        private void button5_Click(object sender, EventArgs e)
        {
            if (p1 == null)
                return;
            var ImgIn = new ImgModule(p1);
            p1 = ImgIn.LowPassFilter(p1,3);
            this.pictureBox1.Image = p1;
            panel1.Invalidate();
        }
        //UnSharpMasking
        private void button6_Click(object sender, EventArgs e)
        {
            if (p1 == null)
                return;
            var ImgIn = new ImgModule(p1);
            p1 = ImgIn.UnSharpMasking(p1,0,1);
            this.pictureBox1.Image = p1;
            panel1.Invalidate();
        }
        //Sobel
        private void button7_Click(object sender, EventArgs e)
        {
            var ImgIn = new ImgModule(p1);
            p1 = ImgIn.Sobel(p1);
            this.pictureBox1.Image = p1;
        }
        //Binary
        private void button8_Click(object sender, EventArgs e)
        {
            var ImgIn = new ImgModule(p1);
            p1 = ImgIn.Binary(p1, 200);
            this.pictureBox1.Image = p1;
        }
        //HoughTransform
        private void button9_Click(object sender, EventArgs e)
        {
            var ImgIn = new ImgModule(p1);
            p1 = ImgIn.HoughTransform(p1, 200);
            this.pictureBox1.Image = p1;
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
