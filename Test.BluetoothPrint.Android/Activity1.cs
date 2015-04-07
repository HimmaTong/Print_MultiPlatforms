using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Bluetooth;
using Android.Graphics;
using Java.IO;
using BluetoothPrint.droid;
using Android.Util;

namespace Test.BluetoothPrint.droid
{
    [Activity(Label = "Test", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        string address = "98:D3:31:30:2A:3C";//"00:02:0A:01:60:71";
        BluetoothHelper blueH = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            DisplayMetrics metric = new DisplayMetrics();
            this.Window.WindowManager.DefaultDisplay.GetMetrics(metric);
            float density = metric.Density;  // 屏幕密度（0.75 / 1.0 / 1.5）
            //Rate = (float)metric.WidthPixels / (float)1280;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            blueH = new BluetoothHelper();
            int err = 0;
            Action<string, string> ConnectedAction = new Action<string, string>((name, address) =>
            {
                System.Console.WriteLine(name);
                System.Console.WriteLine(address);
            });
            Action<string> ConnectingAction = new Action<string>((t) =>
            {

            });
            Action<string> ConnFailedAction = new Action<string>((t) =>
            {

            });



            if (blueH.Init(ConnectedAction, ConnectingAction, ConnFailedAction) == 1)
            {
                //蓝牙已打开
            }
            else
            {
                if (blueH.IsOpen() == 1)
                {
                    //打开蓝牙
                    blueH.Open(this);
                }
            }




            Button btnScan = FindViewById<Button>(Resource.Id.MyButton);
            btnScan.Click += (o, e) =>
            {
                var serverIntent = new Intent(this, typeof(DeviceManager));
                StartActivityForResult(serverIntent, DeviceManager.REQUEST_CONNECT_DEVICE);
            };
            Button Print = FindViewById<Button>(Resource.Id.Print);
            Print.Click += (o, e) =>
            {

                //Java.Lang.String str0 = new Java.Lang.String("时段：晚市    手机：18721636793   人数：1");

                //if (blueH.IsConnected()!=1)
                //{
                //    if (blueH.Connect(address)==1)
                //    {
                //        blueH.SendMessage(str0);
                //    }
                //}
                //else
                //{
                //    blueH.SendMessage(str0);
                //    //Bitmap bm = BitmapFactory.DecodeStream(Resources.Assets.Open("android.png"));
                //    //blueH.SendImg(bm,576,0);
                //    //blueH.WalkPaper(2);
                //    //blueH.CutPage();
                //}
                /*  */
                //if (blueH.IsConnected() != 1)
                //{
                //    blueH.Connect(address);
                //}

                var cmd = Ticket.CreateTicketCMD("星巴克", "晚餐", "18721636753", "5", "2015-01-26 19:12:12", "小桌A001", "之前还有17位客人等待，过号请重新领号，不要走开哦", "021-0000000", "上海市徐汇区商业区", "扫描二维码查看您的排号队列，还可以下载官方APP，获取等多优惠", "", "如所持票已等待30分钟，可凭票销售8.8折优惠（团购不享受此优惠）");
                blueH.SendCommand(cmd.ToArray());

                //Bitmap bm = BitmapFactory.DecodeStream(Resources.Assets.Open("qr.jpg"));
                //blueH.SendImg(bm,576,0);

                //blueH.WalkPaper(6);
                //blueH.CutPage();
                //blueH.SendCommand(SerialPortPrint.PrintCommand.Inite());
                //blueH.SendCommand(PrintFormatCommand.FontSize(FontSizeEnum.Size16));
                //Java.Lang.String str = new Java.Lang.String("猫管家");
                //blueH.SendMessage(str);
                //blueH.SendCommand(PrintFormatCommand.Wrap());
                //blueH.SendCommand(PrintFormatCommand.Wrap());
                //blueH.SendCommand(PrintFormatCommand.FontSize(FontSizeEnum.Normal));
                //Java.Lang.String str0 = new Java.Lang.String("时段：晚市    手机：18721636793   人数：1");
                //blueH.SendMessage(str0);
                //blueH.SendCommand(PrintFormatCommand.Wrap());
                //blueH.SendCommand(PrintFormatCommand.Wrap());

                //Java.Lang.String str1 = new Java.Lang.String("领号时间:2015-01-17 17:14:55");
                //blueH.SendMessage(str1);
                //blueH.SendCommand(PrintFormatCommand.Wrap());

                //blueH.SendCommand(PrintFormatCommand.FontSize(FontSizeEnum.Size32));
                //blueH.SendCommand(PrintFormatCommand.Align(1));
                //Java.Lang.String str3 = new Java.Lang.String("小桌 A001");
                //blueH.SendMessage(str3);
                //blueH.SendCommand(PrintFormatCommand.Wrap());
            };
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case 0:
                    if (resultCode == Result.Ok)
                    {

                    }
                    break;
                case DeviceManager.REQUEST_CONNECT_DEVICE:
                    // When DeviceListActivity returns with a device to connect
                    if (resultCode == Result.Ok)
                    {
                        // Get the device MAC address
                        var address = data.Extras.GetString(DeviceManager.EXTRA_DEVICE_ADDRESS);
                        // Get the BLuetoothDevice object
                        //// Attempt to connect to the device
                        blueH.Connect(address);
                    }
                    break;
                case DeviceManager.REQUEST_ENABLE_BT:
                    // When the request to enable Bluetooth returns
                    if (resultCode == Result.Ok)
                    {
                        // Bluetooth is now enabled, so set up a chat session
                        // SetupChat();
                    }
                    else
                    {
                        // User did not enable Bluetooth or an error occured
                        //Log.Debug(TAG, "BT not enabled");
                        //Toast.MakeText(this, Resource.String.bt_not_enabled_leaving, ToastLength.Short).Show();
                        //Finish();
                    }
                    break;
            }
        }
    }
}

