using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BluetoothPrint.droid
{
    /// <summary>
    /// 打印格式控制
    /// </summary>
    public static class PrintFormatCommand
    {
        public static byte[] Init()
        {
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x1B;
            cmdData[1] = 0x40;
            return cmdData;
        }
        /// <summary>
        /// 设置汉字模式
        /// </summary>
        /// <returns></returns>
        public static byte[] CharacterSet()
        {
            byte[] cmdData = new byte[2];

            //换行
            cmdData[0] = 0x1C;
            cmdData[1] = 0x26;
            return cmdData;
        }
        /// <summary>
        /// 字体大小
        /// </summary>
        /// <returns></returns>
        public static byte[] FontSize(FontSizeEnum fontsize)
        {
            byte[] cmdData = new byte[4];

            //放大
            cmdData[0] = 0x1d;
            cmdData[1] = 0x21;

            switch (fontsize)
            {
                case FontSizeEnum.Normal:
                    cmdData[2] = 0x00;
                    break;
                case FontSizeEnum.Size16:
                    cmdData[2] = 0x11;
                    break;
                case FontSizeEnum.Size32:
                    cmdData[2] = 0x22;
                    break;
            }
            return cmdData;
        }
        /// <summary>
        /// 行间距
        /// </summary>
        /// <returns></returns>
        public static byte[] LineHeight()
        {
            byte[] cmdData = new byte[3];

            //换行
            cmdData[0] = 0x1B;
            cmdData[1] = 0x33;
            cmdData[1] = 0x10;
            return cmdData;
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <returns></returns>
        public static byte[] Wrap()
        {
            byte[] cmdData = new byte[1];
            cmdData[0] = 0x0A;
            return cmdData;
        }
        /// <summary>
        /// 设置对齐方式
        /// </summary>
        /// <param name="alignType">0：左对齐，1：居中，2：右对齐</param>
        /// <returns></returns>
        public static byte[] Align(int alignType = 0)
        {
            byte[] cmdData = new byte[3];
            cmdData[0] = 0x1B;
            cmdData[1] = 0x61;
            switch (alignType)
            {
                case 0:
                    cmdData[2] = 0x30;
                    break;
                case 1:
                    cmdData[2] = 0x31;
                    break;
                case 2:
                    cmdData[2] = 0x32;
                    break;
            }
            return cmdData;
        }

        /// <summary>
        /// 二维码打印
        /// </summary>
        /// <param name="qrcontent"></param>
        /// <returns></returns>
        public static List<byte> PrintQrCode(string qrcontent)
        {
            var qrbyte = System.Text.Encoding.Default.GetBytes(qrcontent);
            List<byte> cmdData = new List<byte>();
            cmdData.Add(0x1d);
            cmdData.Add(0x28);
            cmdData.Add(0x6b);
            cmdData.Add(0x04);
            cmdData.Add(0x00);
            cmdData.Add(0x31);
            cmdData.Add(0x41);
            cmdData.Add(0x31);
            cmdData.Add(0x00);


            cmdData.Add(0x1d);
            cmdData.Add(0x28);
            cmdData.Add(0x6b);
            cmdData.Add(0x03);
            cmdData.Add(0x00);
            cmdData.Add(0x31);
            cmdData.Add(0x43);
            cmdData.Add(0x06);

            cmdData.Add(0x1d);
            cmdData.Add(0x28);
            cmdData.Add(0x6b);
            cmdData.Add(0x03);
            cmdData.Add(0x00);
            cmdData.Add(0x31);
            cmdData.Add(0x45);
            cmdData.Add(0x30);

            cmdData.Add(0x1d);
            cmdData.Add(0x28);
            cmdData.Add(0x6b);

            int pl;
            int ph;
            //（pl+ph*256)-3
            //(0 ≤ pL ≤ 255 , 0 ≤ pH ≤ 27)

            ph = (qrbyte.Length + 3) / 255;
            pl = (qrbyte.Length + 3) % 255;

            cmdData.Add(byte.Parse(pl.ToString()));
            cmdData.Add(byte.Parse(ph.ToString()));

            //cmdData.Add(0x10);
            //cmdData.Add(0x00);

            cmdData.Add(0x31);
            cmdData.Add(0x50);
            cmdData.Add(0x30);

            foreach (var b in qrbyte)
            {
                cmdData.Add(b);
            }

            cmdData.Add(0x1d);
            cmdData.Add(0x28);
            cmdData.Add(0x6b);
            cmdData.Add(0x03);
            cmdData.Add(0x00);
            cmdData.Add(0x31);
            cmdData.Add(0x51);
            cmdData.Add(0x30);

            return cmdData;

        }

    }
    /// <summary>
    /// 字符大小
    /// </summary>
    public enum FontSizeEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 两倍宽
        /// </summary>
        Size16 = 1,
        /// <summary>
        /// 3倍宽
        /// </summary>
        Size32 = 2
    }
}
