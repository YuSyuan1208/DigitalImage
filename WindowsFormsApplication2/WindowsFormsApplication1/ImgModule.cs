using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication1
{
    static class Constants
    {
        public const int HS = 1;
        public const int HE = 2;
    }
    class ImgModule : ImgInitial
    {  
        public ImgModule(Bitmap imgIn) : base(imgIn)
        {
        }
        /// <summary>
        /// Img to Gray
        /// </summary>
        public Bitmap gary(Bitmap Img)
        {
            var ImgIn = new ImgInitial(Img);
            for (int counter = 0; counter < ImgIn.rgbValues.Length - 1; counter += 3)
            {
                int g = ((ImgIn.rgbValues[counter] + ImgIn.rgbValues[counter + 1] + ImgIn.rgbValues[counter + 2]) / 3);
                ImgIn.rgbValues[counter] = (Byte)g;
                ImgIn.rgbValues[counter + 1] = (Byte)g;
                ImgIn.rgbValues[counter + 2] = (Byte)g;
            }
            Img = ImgIn.ImgChange();
            return Img;
        }
        /// <summary>
        /// Img to PepperSalt
        /// </summary>
        public Bitmap PepperSalt(Bitmap Img ,int Ratio)
        {
            var ImgIn = new ImgInitial(Img);
            Random rnd = new Random();
            var PixelChange = new Byte();
            for (int counter = 0; counter < ImgIn.rgbValues.Length - 1; counter += 3)
            {
                int Total = rnd.Next(0, 100 / Ratio );
                int PepperSalt = rnd.Next(0,2);
                if (Total == 1)
                {
                    if (PepperSalt == 0)
                        PixelChange = 255;
                    else if (PepperSalt == 1)
                        PixelChange = 0;
                }
                else
                    PixelChange = ImgIn.rgbValues[counter];
                ImgIn.rgbValues[counter] = PixelChange;
                ImgIn.rgbValues[counter + 1] = PixelChange;
                ImgIn.rgbValues[counter + 2] = PixelChange;
            }
            Img = ImgIn.ImgChange();
            return Img;
        }
        /// <summary>
        /// Img to HistogramPricessing "HE" or "HS"
        /// </summary>
        public Bitmap HistogramProcessing(Bitmap Img, int MaxPixel=255, int MinPixel=0,int SelectProcessing=2)
        {
            var ImgIn = new ImgInitial(Img);
            var PixelArray = Histogram(Img);
            var ImgMaxPixel = PixelArray[256];
            var ImgMinPixel = PixelArray[257];
            double PixelChange;
            for (int counter = 0; counter < PixelArray.Length - 3; counter++)
            {
                PixelArray[counter + 1] += PixelArray[counter];
            }
            long CdfMin = PixelArray[ImgMinPixel];
            long CdfMax = PixelArray[ImgMaxPixel];
            for (int counter = 0; counter < ImgIn.rgbValues.Length - 1; counter += 3)
            {
                if (SelectProcessing == Constants.HS) 
                    PixelChange = (double)((ImgIn.rgbValues[counter] - ImgMinPixel) / (double)(ImgMaxPixel - ImgMinPixel)) * (MaxPixel - MinPixel) + MinPixel;
                else if (SelectProcessing == Constants.HE) 
                    PixelChange = (double)((PixelArray[ImgIn.rgbValues[counter]] - CdfMin) / (double)(CdfMax - CdfMin)) * (MaxPixel - MinPixel) + MinPixel;
                else
                    break;
                ImgIn.rgbValues[counter] = (byte)PixelChange;         
                ImgIn.rgbValues[counter + 1] = (byte)PixelChange;
                ImgIn.rgbValues[counter + 2] = (byte)PixelChange;
            }
            Img = ImgIn.ImgChange();
            return Img;
        }
        /// <summary>
        /// Histogram PixelsArray
        /// </summary>
        public long[] Histogram(Bitmap Img)
        {
            var ImgIn = new ImgInitial(Img);
            var PixelArray = new long[258];
            int ImgMaxPixel = 0, ImgMinPixel = 255;
            for (int i = 0; i < PixelArray.Length; i++)
                PixelArray[i] = 0;
            for (int counter = 0; counter < ImgIn.rgbValues.Length; counter += 3)
            {
                if (ImgIn.rgbValues[counter] > ImgMaxPixel) ImgMaxPixel = ImgIn.rgbValues[counter];
                if (ImgIn.rgbValues[counter] < ImgMinPixel) ImgMinPixel = ImgIn.rgbValues[counter];
                PixelArray[ImgIn.rgbValues[counter]]++;
            }
            PixelArray[256] = ImgMaxPixel;
            PixelArray[257] = ImgMinPixel;
            return PixelArray;
        }
        /// <summary>
        /// LassPass Filter
        /// </summary>
        public Bitmap LowPassFilter(Bitmap Img, int Mask=3) {
            var ImgIn = new ImgInitial(Img);
            Bitmap Img2 = ImgIn.Copy(Img);
            var ImgIn2 = new ImgInitial(Img2);
            var Block = Mask / 2;
            for (int counter = 0; counter < ImgIn.rgbValues.Length - 1; counter += 3)
            {
                int PixelChange = 0;
                for (int x = -Block; x <= Block; x++)
                {

                    for (int y = -Block; y <= Block; y++)
                    {
                        int Range = counter + y * (Img.Width*3) + x * 3;
                        if (Range > (ImgIn.ImgPixelsSize - 1)) Range = (ImgIn.ImgPixelsSize - 1) * 2 - Range;
                        PixelChange = PixelChange + (int)ImgIn.rgbValues[Math.Abs(Range)];
                    }
                }
                PixelChange = PixelChange / Mask / Mask;
                ImgIn2.rgbValues[counter] = (byte)PixelChange;
                ImgIn2.rgbValues[counter + 1] = (byte)PixelChange;
                ImgIn2.rgbValues[counter + 2] = (byte)PixelChange;
            }
            Img2 = ImgIn2.ImgChange();
            return Img2;
        }
        /// <summary>
        /// UnSharpMasking (Img=原圖,Threshold=銳化選擇 ,Amount=銳化強度 ,
        /// HSMaxPixel1,HSMinPixel1=選取欲銳化像素範圍 ,HSMaxPixel1,HSMinPixel1=選取輸出像素範圍 ,
        /// LowPassMask=LowPass遮罩範圍)
        /// </summary>
        public Bitmap UnSharpMasking(Bitmap Img ,int Threshold=0 ,double Amount=1 ,
            byte HSMaxPixel1 = 255, byte HSMinPixel1 = 0, byte HSMaxPixel2 = 255, byte HSMinPixel2 = 0,
            int LowPassMask = 3)
        {
            var ImgIn = new ImgInitial(Img);
            Bitmap Img2;
            Img2 = ImgIn.Copy(Img);
            Img2 = LowPassFilter(Img2, LowPassMask);
            Img2 = HistogramProcessing(Img2, HSMaxPixel1, HSMinPixel1, Constants.HS);
            var ImgIn2 = new ImgInitial(Img2);
            for (int counter = 0; counter < ImgIn.rgbValues.Length - 1; counter += 3)
            {
                var ChangeValue = ImgIn.rgbValues[counter] - ImgIn2.rgbValues[counter];
                if (ChangeValue < Threshold) continue;
                var PixelChange = ChangeValue * Amount + ImgIn.rgbValues[counter];
                PixelChange =PixelChange > 255 ? 255 : PixelChange < 0 ? 0 : PixelChange;
                ImgIn.rgbValues[counter] = (byte)PixelChange;
                ImgIn.rgbValues[counter + 1] = (byte)PixelChange;
                ImgIn.rgbValues[counter + 2] = (byte)PixelChange;
            }
            Img = ImgIn.ImgChange();
            Img = HistogramProcessing(Img, HSMaxPixel2, HSMinPixel2, Constants.HS);
            return Img;
        }
        /// <summary>
        /// Sobel
        /// </summary>
        public Bitmap Sobel(Bitmap Img)
        {
            var ImgIn = new ImgInitial(Img);
            Bitmap Img2;
            Img2 = ImgIn.Copy(Img);
            var ImgIn2 = new ImgInitial(Img2);
            for (int counter = 0; counter < ImgIn.rgbValues.Length - 1; counter += 3)
            {
                int SobelGx = 0, SobelGy = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y += 2) {
                        
                        int RangeGx = counter + y * ((ImgIn.ImgPixelsSize - 1) / Img.Height) + x * 3;
                        int RangeGy = counter + x * ((ImgIn.ImgPixelsSize - 1) / Img.Height) + y * 3;
                        if (x == 0) { RangeGx *= 2; RangeGy *= 2; }
                        if (RangeGx > ImgIn.rgbValues.Length - 1 || RangeGx < 0) break;
                        if (RangeGy > ImgIn.rgbValues.Length - 1 || RangeGx < 0) break; 
                        SobelGx = SobelGx + x*ImgIn.rgbValues[RangeGx];
                        SobelGy = SobelGy + x*ImgIn.rgbValues[RangeGy];
                    }
                }

                var PixelChange = Math.Abs(SobelGx) + Math.Abs(SobelGy);
                PixelChange = PixelChange > 255 ? 255 : PixelChange < 0 ? 0 : PixelChange;
                ImgIn2.rgbValues[counter] = (byte)PixelChange;
                ImgIn2.rgbValues[counter + 1] = (byte)PixelChange;
                ImgIn2.rgbValues[counter + 2] = (byte)PixelChange;
            }
            Img2 = ImgIn2.ImgChange();
            return Img2;
        }
        /// <summary>
        /// Binary
        /// </summary>
        public Bitmap Binary(Bitmap Img, byte Threshold)
        {
            var ImgIn = new ImgInitial(Img);
            for (int counter = 0; counter < ImgIn.rgbValues.Length - 1; counter += 3)
            {
                var PixelChange = ImgIn.rgbValues[counter] > Threshold ? 255 : 0;
                ImgIn.rgbValues[counter] = (byte)PixelChange;
                ImgIn.rgbValues[counter + 1] = (byte)PixelChange;
                ImgIn.rgbValues[counter + 2] = (byte)PixelChange;
            }
            Img = ImgIn.ImgChange();
            return Img;
        }
        /// <summary>
        /// HoughTransform
        /// </summary>
        public Bitmap HoughTransform(Bitmap Img, byte BinaryThreshold)
        {
            var ImgIn = new ImgInitial(Img);
            var Img2 = ImgIn.Copy(Img);
            Img2 = Sobel(Img2);
            Img2 = Binary(Img2, BinaryThreshold);
            var ImgIn2 = new ImgInitial(Img2);
            List<List<int>> Point = new  List<List<int>>();
            for (int counter = 0; counter < ImgIn2.rgbValues.Length - 1; counter += 3)
            {
                List<int>LinePoint = new List<int>();
                if (ImgIn2.rgbValues[counter] != 255) continue;
                for (int Angle = 0; Angle < 180; Angle++)
                {
                    var ChangeX = Math.Cos(Angle);
                    var ChangeY = Math.Sin(Angle);
                    int TotalPoint = 0;
                    bool e1 = false, e2 = false;
                    while (true) {
                        var CounterChange = ChangeX * 3 + ChangeY * Img2.Width * 3;
                        if ((counter / Img2.Height + ChangeX * 3) < Img2.Width * 3 && (counter / Img2.Height + ChangeX * 3) > 0 && (counter + ChangeY * Img2.Width * 3) > 0)
                        {
                            if (ImgIn2.rgbValues[counter + (int)CounterChange] == 255) TotalPoint++;
                        }
                        else { e1 = true; }
                        if ((counter / Img2.Height - ChangeX * 3) < Img2.Width * 3 && (counter / Img2.Height - ChangeX * 3) > 0 && (counter + ChangeY * Img2.Width * 3) < ImgIn2.rgbValues.Length - 1)
                        {
                            if (ImgIn2.rgbValues[counter - (int)CounterChange] == 255) TotalPoint++;
                        }
                        else { e2 = true; }
                        if (e1 == true && e2 == true) { break; }
                    }
                    if (TotalPoint > 100) LinePoint.Add(counter);
                }
                if(LinePoint != null)Point.Add(LinePoint);
            }
            Img2 = ImgIn2.ImgChange();
            return Img2;
        }
    }
}
