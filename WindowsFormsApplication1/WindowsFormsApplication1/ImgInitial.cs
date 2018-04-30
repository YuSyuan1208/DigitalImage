using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class ImgInitial
    {
        /// <summary>
        /// Img Initial
        /// </summary>
        public ImgInitial(Bitmap imgIn)
        {
            Rectangle rect_img = new Rectangle(0, 0, imgIn.Width, imgIn.Height);
            System.Drawing.Imaging.BitmapData bmpData =
            imgIn.LockBits(rect_img, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            imgIn.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * imgIn.Height;
            byte[] rgbValuesIn = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValuesIn, 0, bytes);
            rgbValues = rgbValuesIn;
            ImgPixelsSize = bytes;
            img = imgIn;
            imgIn.UnlockBits(bmpData);
        }
        /// <summary>
        /// To Change Img RGB Color
        /// </summary>
        public Bitmap ImgChange  ()
        {
            Rectangle rect_img = new Rectangle(0, 0, img.Width, img.Height);
            System.Drawing.Imaging.BitmapData bmpData =
            img.LockBits(rect_img, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            img.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * img.Height;
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            img.UnlockBits(bmpData);
            return img;
        }
        /// <summary>
        /// Bitmap Clone
        /// </summary>
        public Bitmap Copy(Bitmap ImgIn)
        {
            Rectangle rect_img = new Rectangle(0, 0, img.Width, img.Height);
            Bitmap ImgOut = ImgIn.Clone(rect_img, ImgIn.PixelFormat);
            return ImgOut;
        }
        /// <summary>
        /// Total Img Pixel
        /// </summary>
        public int ImgPixelsSize { get; set; }
        
        public Bitmap img { get; set; }
        public byte[] rgbValues { get; set; }
    }
}
