using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace SobelFilter
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("sobel");

            UserParams userParams = new UserParams();
            if(!userParams.ReadArguments(args))
            {
                return 1;
            }

            
            Bitmap inputData = new Bitmap(userParams.InputFileName);
            int w = inputData.Width;
            int h = inputData.Height;

            Console.WriteLine("w = {0} h = {1}", w, h);

            BitmapData bmpData = inputData.LockBits(new Rectangle(Point.Empty, inputData.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            byte[] buf = new byte[inputData.Width * inputData.Height * 3];
            // データの吸出し
            Marshal.Copy(bmpData.Scan0, buf, 0, buf.Length);
            // 不要なので入力画像は閉じる
            inputData.UnlockBits(bmpData);

            byte[] sobelBuf_H = new byte[inputData.Width * inputData.Height * 3];
            byte[] sobelBuf_V = new byte[inputData.Width * inputData.Height * 3];
            byte[] sobelBuf_VH = new byte[inputData.Width * inputData.Height * 3];


            // 上下左右の端画素は対象外とする
            // 0クリアされているのでRGB(0,0,0)になっているので気にしない

            /* 水平方向のフィルタ　縦の線を検出しやすい
             * -1 0 1 
             * -2 0 2
             * -1 0 1
             */

            /* 垂直方向のフィルタ　横の線を検出しやすい
             * -1 -2 -1
             *  0  0  0
             *  1  2  1
             */

            for (int y = 1; y < h - 1; y++)
            {
                for(int x = 1; x < w - 1; x++)
                {
                    // グレー画像なのでbだけ使う
                    int b = buf[(y * w + x) * 3 + 0];
                    //int g = buf[(y * w + x) * 3 + 1];
                    //int r = buf[(y * w + x) * 3 + 2];

                    // 水平方向フィルタ
                    int value_H = 
                        buf[((y - 1) * w + x - 1) * 3] * -1 +
                        buf[((y - 1) * w + x    ) * 3] *  0 +
                        buf[((y - 1) * w + x + 0) * 3] *  1 +
                        buf[(y       * w + x - 1) * 3] * -2 +
                        buf[(y       * w + x    ) * 3] *  0 +
                        buf[(y       * w + x + 1) * 3] *  2 +
                        buf[((y + 1) * w + x - 1) * 3] * -1 +
                        buf[((y + 1) * w + x    ) * 3] *  0 +
                        buf[((y + 1) * w + x + 1) * 3] *  1;

                    byte filteredValue_H = (byte)(Math.Min(Math.Max(0, value_H), 255));
                    sobelBuf_H[(y * w + x) * 3 + 0] = filteredValue_H;
                    sobelBuf_H[(y * w + x) * 3 + 1] = filteredValue_H;
                    sobelBuf_H[(y * w + x) * 3 + 2] = filteredValue_H;

                    int value_V =
                        buf[((y - 1) * w + x - 1) * 3] * -1 +
                        buf[((y - 1) * w + x    ) * 3] * -2 +
                        buf[((y - 1) * w + x + 0) * 3] * -1 +
                        buf[(y       * w + x - 1) * 3] *  0 +
                        buf[(y       * w + x)     * 3] *  0 +
                        buf[(y       * w + x + 1) * 3] *  0 +
                        buf[((y + 1) * w + x - 1) * 3] *  1 +
                        buf[((y + 1) * w + x)     * 3] *  2 +
                        buf[((y + 1) * w + x + 1) * 3] *  1;

                    byte filteredValue_V = (byte)(Math.Min(Math.Max(0, value_V), 255));
                    sobelBuf_V[(y * w + x) * 3 + 0] = filteredValue_V;
                    sobelBuf_V[(y * w + x) * 3 + 1] = filteredValue_V;
                    sobelBuf_V[(y * w + x) * 3 + 2] = filteredValue_V;

                    double valueVH = Math.Sqrt(Math.Pow(filteredValue_H, 2) + Math.Pow(filteredValue_V, 2));
                    byte filteredValue_VH = (byte)(Math.Min(Math.Max(0, valueVH), 255));
                    sobelBuf_VH[(y * w + x) * 3 + 0] = filteredValue_VH;
                    sobelBuf_VH[(y * w + x) * 3 + 1] = filteredValue_VH;
                    sobelBuf_VH[(y * w + x) * 3 + 2] = filteredValue_VH;

                }
            }

            string fileName = Path.GetFileNameWithoutExtension(userParams.InputFileName);

            Bitmap destBmp_H = new Bitmap(w, h);
            BitmapData destData_H= destBmp_H.LockBits(new Rectangle(Point.Empty, destBmp_H.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(sobelBuf_H, 0, destData_H.Scan0, sobelBuf_H.Length);
            destBmp_H.UnlockBits(destData_H);
            destBmp_H.Save(fileName + "_H.bmp", ImageFormat.Bmp);

            Bitmap destBmp_V = new Bitmap(w, h);
            BitmapData destData_V = destBmp_V.LockBits(new Rectangle(Point.Empty, destBmp_V.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(sobelBuf_V, 0, destData_V.Scan0, sobelBuf_V.Length);
            destBmp_V.UnlockBits(destData_V);
            destBmp_V.Save(fileName + "_V.bmp", ImageFormat.Bmp);

            Bitmap destBmp_VH = new Bitmap(w, h);
            BitmapData destData_VH = destBmp_VH.LockBits(new Rectangle(Point.Empty, destBmp_VH.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(sobelBuf_VH, 0, destData_VH.Scan0, sobelBuf_VH.Length);
            destBmp_VH.UnlockBits(destData_VH);
            destBmp_VH.Save(fileName + "_VH.bmp", ImageFormat.Bmp);

            return 0;
        }
    }
}