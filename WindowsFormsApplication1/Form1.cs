﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ss = axOPOSPOSPrinter1.Open("print2");
            axOPOSPOSPrinter1.ClaimDevice(1000);
            axOPOSPOSPrinter1.DeviceEnabled = true;
            //var s = axOPOSPOSPrinter1.PrintNormal(2, "1231321313212333333333334444444444444444444444444444444444444444444");
             var aa = axOPOSPOSPrinter1.PrintBitmap(2, @"F:\QBWrokSpace\Print_MultiPlatforms\OCXTest\test.jpg", -11, -1);
        }
    }
}
