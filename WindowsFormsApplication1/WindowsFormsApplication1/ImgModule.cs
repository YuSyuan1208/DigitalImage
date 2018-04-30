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
                        int Range = counter + y * ((ImgIn.ImgPixelsSize - 1) / Img.Height) + x * 3;
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
        /// UnSharpMasking (Img=原圖, HSMaxPixel1,HSMinPixel1=選取欲銳化像素範圍 ,HSMaxPixel1,HSMinPixel1=選取輸出像素範圍 ,
        /// LowPassMask=LowPass遮罩範圍 ,Threshold=銳化選擇 ,Amount=銳化強度)
        /// </summary>
        public Bitmap UnSharpMasking(Bitmap Img,byte HSMaxPixel1=255,byte HSMinPixel1=0
            ,byte HSMaxPixel2=255, byte HSMinPixel2=0
            ,int Threshold=0 ,double Amount=1 ,int LowPassMask = 3)
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
                if (ChangeValue > Threshold) continue;
                var PixelChange = ImgIn.rgbValues[counter] + ChangeValue * Amount;
                PixelChange = PixelChange > 255 ? 255 : PixelChange < 0 ? 0 : PixelChange;
                ImgIn.rgbValues[counter] = (byte)PixelChange;
                ImgIn.rgbValues[counter + 1] = (byte)PixelChange;
                ImgIn.rgbValues[counter + 2] = (byte)PixelChange;
            }
            Img = ImgIn.ImgChange();
            Img = HistogramProcessing(Img, HSMaxPixel2, HSMinPixel2, Constants.HS);
            return Img;
        }
    }
}
