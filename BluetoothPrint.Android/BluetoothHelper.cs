using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using PrintBase;
using SerialPortPrint;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothPrint.droid
{
    /// <summary>
    /// 蓝牙数据操作
    /// </summary>
    public class BluetoothHelper : IDisposable
    {
        private const string TAG = "BluetoothHelper";
        private string address;
        private bool isopen = false;
        // Track whether Dispose has been called.
        private bool disposed = false;
        // Message types sent from the BluetoothService Handler
        // TODO: Make into Enums
        public const int MESSAGE_STATE_CHANGE = 1;
        public const int MESSAGE_READ = 2;
        public const int MESSAGE_WRITE = 3;
        public const int MESSAGE_DEVICE_NAME = 4;
        public const int MESSAGE_TOAST = 5;
        public const int MESSAGE_CONNCETFAILED = 6;


        /// <summary>
        /// 没有打开蓝牙
        /// </summary>
        public const int ERROR_NOTOPEN = 2;
        /// <summary>
        /// 没有连接蓝牙打印机
        /// </summary>
        public const int ERROR_NOTCONNCETED = 3;

        // Key names received from the BluetoothService Handler
        public const string DEVICE_NAME = "";
        public const string DEVICE_ADDRESS = "";
        public const string TOAST = "toast";

        // Intent request codes
        // TODO: Make into Enums
        private const int REQUEST_CONNECT_DEVICE = 1;
        private const int REQUEST_ENABLE_BT = 2;

        // Name of the connected device
        internal string connectedDeviceName = null;
        internal string connectedDeviceAddress = null;


        private BluetoothAdapter bluetoothAdapter = null;
        private BluetoothService chatService = null;
        public BluetoothHelper()
        {
            // Initialize the BluetoothService to perform bluetooth connections

        }
        public void Stop()
        {
            if (chatService != null)
            {
                chatService.Stop();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectedAction">连接成功</param>
        /// <param name="ConnectingAction">连接中</param>
        /// <param name="ConnFailedAction">断开链接（用户UI提示）</param>
        /// <returns></returns>
        public int Init(Action<string, string> ConnectedAction, Action<string> ConnectingAction, Action<string> ConnFailedAction)
        {
            //断开重连
            Action<string> connLost = (mess) =>
            {
                if (GetState() == 1)
                {
                    if (!string.IsNullOrEmpty(address))
                    {
                        Log.Debug(TAG, mess);
                        Stop();
                        System.Threading.Thread.Sleep(2000);
                        Connect(address);
                    }
                }
            };
            int err = 0;
            Stop();
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter == null)
            {
                err = (int)PrintError.NotSupportBluetooth;
                return err;
            }
            else
            {
                chatService = new BluetoothService(new MyHandler(this, ConnectedAction, ConnectingAction, ConnFailedAction, connLost));
                isopen = true;
                err = 1;
                return err;
            }
        }
        /// <summary>
        /// 判断蓝牙是否打开
        /// </summary>
        /// <returns></returns>
        public int IsOpen()
        {

            int err = 0;
            try
            {
                if (this.bluetoothAdapter.IsEnabled)
                {
                    err = 1;
                }
                else
                {
                    err = (int)PrintError.NotOpenBluetooth;
                }
            }
            catch
            {
                err = (int)PrintError.NotSupportBluetooth;
            }
            return err;
        }
        public int IsConnected()
        {
            int err = 0;
            if (this.chatService != null && this.chatService.GetState() == BluetoothService.STATE_CONNECTED)
            {
                err = 1;
                return err;
            }
            else
            {
                err = (int)PrintError.ConnectedFailure;
                return err;
            }
        }
        /// <summary>
        ///开启蓝牙
        /// </summary>
        /// <returns></returns>
        public bool Open(Activity activity)
        {
            Intent enableBtIntent = new Intent(
                BluetoothAdapter.ActionRequestEnable);
            activity.StartActivityForResult(enableBtIntent, 0);
            return true;
        }

        /// <summary>
        /// 关闭蓝牙
        /// </summary>
        public void closeBluetooth()
        {
            this.bluetoothAdapter.Disable();
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public int Connect(string address)
        {
            int err = 0;
            if (!isopen)
            {
                err = (int)PrintError.OpenFailure;
                return err;
            }
            this.address = address;
            BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(address);
            chatService.Connect(device);
            err = 1;
            return err;
        }

        public byte[] Init()
        {
            return PrintCommand.Inite();
        }
        /// <summary>
        /// 获取打印机状态
        /// </summary>
        /// <returns>3:STATE_CONNECTED,2:STATE_CONNECTING,1:STATE_LISTEN,0：STATE_NONE</returns>
        public int GetState()
        {
            return (int)this.chatService.GetState();
        }
        public int SendCommand(byte[] cmd)
        {
            int err = 0;
            try
            {
                if (!isopen)
                {
                    err = (int)PrintError.OpenFailure;
                    return err;
                }
                if (this.chatService.GetState() != BluetoothService.STATE_CONNECTED)
                {
                    err = (int)PrintError.ConnectedFailure;
                    return err;
                }

                if (cmd.Length <= 0)
                {
                    err = (int)PrintError.SendNull;
                    return err;
                }


                if (chatService.Write(cmd))
                {
                    err = 1;
                }
                else
                {
                    err = (int)PrintError.SendFailure;
                }

                return err;
            }
            catch
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
        }
        /// <summary>
        /// 打印文本内容
        /// </summary>
        /// <param name="message">打印的内容</param>
        /// <param name="err">2:没有开启蓝牙，3：没有连接打印机，4：butmap为null,5:程序出现异常</param>
        /// <returns></returns>
        public int SendMessage(Java.Lang.String message)
        {
            int err = 0;
            if (!isopen)
            {
                err = (int)PrintError.OpenFailure;
                return err;
            }
            if (this.chatService.GetState() != BluetoothService.STATE_CONNECTED)
            {
                err = (int)PrintError.ConnectedFailure;
                return err;
            }

            if (message.Length() <= 0)
            {
                err = (int)PrintError.SendNull; ;
                return err;
            }
            try
            {
                byte[] send = message.GetBytes("GB2312");
                chatService.Write(send);
                err = 1;
                return err;
            }
            catch
            {
                err = (int)PrintError.SendFailure; ;
                return err;
            }

            // Get the message bytes and tell the BluetoothService to write

        }
        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="err">2:没有开启蓝牙，3：没有连接打印机，4：butmap为null,5:程序出现异常</param>
        /// <returns></returns>
        public int SendImg(Bitmap bitmap, int dpiWidth, int pt)
        {
            int err = 0;
            if (!isopen)
            {
                err = (int)PrintError.OpenFailure;
                return err;
            }
            if (this.chatService.GetState() != BluetoothService.STATE_CONNECTED)
            {
                err = (int)PrintError.ConnectedFailure;
                return err;
            }

            if (bitmap == null)
            {
                err = (int)PrintError.SendNull;
                return err;
            }

            try
            {
                var newbit = ResizeBitmap(bitmap, dpiWidth, bitmap.Height);


                byte[] data = Pos.POS_PrintPicture(newbit, dpiWidth, 0, (PrinterType)pt);
                byte[] cmdData = new byte[data.Length + 6];
                cmdData[0] = 0x1B;
                cmdData[1] = 0x2A;
                cmdData[2] = 0x32;
                cmdData[3] = 0x20;
                cmdData[4] = 0x2;
                cmdData[5] = 0x50;
                for (int i = 0; i < data.Length; i++)
                {
                    cmdData[6 + i] = data[i];
                }
                chatService.Write(data);
                //chatService.Write(cmdData);
                err = 1;
                return err;
            }
            catch
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
        }
        /// <summary>
        ///切纸
        /// </summary>
        public int CutPage()
        {
            int err = 0;
            try
            {
                byte[] cmdData = PrintCommand.Cut();
                chatService.Write(cmdData);
                err = 1;
                return err;
            }
            catch
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
        }
        /// <summary>
        /// 走纸
        /// </summary>
        /// <param name="row"></param>
        public int WalkPaper(int row)
        {
            int err = 0;
            try
            {
                byte[] cmdData = PrintCommand.WalkPaper(row);
                chatService.Write(cmdData);
                err = 1;
                return err;
            }
            catch
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
        }
      
        public Bitmap ResizeBitmap(Bitmap bitmap, int newWidth, int newHeight)
        {
            Bitmap newbit = Bitmap.CreateBitmap(newWidth, newHeight, Bitmap.Config.Rgb565);
            Android.Graphics.Canvas canvas = new Canvas(newbit);
            canvas.DrawColor(Color.White);
            Paint pt = new Paint();
            pt.Color = Color.White;

            //Rect bgR = new Rect(0, 0, width, height);
            Matrix matrix1 = new Matrix();
            matrix1.PostScale(1, 1);
            //canvas.DrawRect(bgR, pt);
            Android.Graphics.Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            canvas.DrawBitmap(bitmap, rect, rect, pt);
            return newbit;
        }


        // The Handler that gets information back from the BluetoothService
        private class MyHandler : Handler
        {
            BluetoothHelper bluetoothChat;
            Action<string, string> ConnectedAction;
            Action<string> ConnectingAction;
            Action<string> ConnFailedAction;
            Action<string> ConnLost;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="chat"></param>
            /// <param name="ConnectedAction">连接成功</param>
            /// <param name="ConnectingAction">连接中</param>
            /// <param name="ConnFailedAction">断开链接（用户UI提示）</param>
            /// <param name="ConnLost">断开连接（触发断线重连）</param>
            public MyHandler(BluetoothHelper chat, Action<string, string> ConnectedAction, Action<string> ConnectingAction, Action<string> ConnFailedAction, Action<string> ConnLost)
            {
                bluetoothChat = chat;
                this.ConnectedAction = ConnectedAction;
                this.ConnectingAction = ConnectingAction;
                this.ConnFailedAction = ConnFailedAction;
                this.ConnLost = ConnLost;
            }
            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    case BluetoothHelper.MESSAGE_STATE_CHANGE:
                        switch (msg.Arg1)
                        {
                            case BluetoothService.STATE_CONNECTED:
                                if (ConnectedAction != null)
                                {
                                    string deviceName = bluetoothChat.connectedDeviceName;
                                    string address = bluetoothChat.connectedDeviceAddress;
                                    ConnectedAction("成功连接至" + deviceName, address);
                                }
                                //连接成功
                                break;
                            case BluetoothService.STATE_CONNECTING:
                                //正在连接
                                if (ConnectingAction != null)
                                {
                                    ConnectingAction("正在连接");
                                }
                                break;
                            case BluetoothService.STATE_LISTEN:
                            case BluetoothService.STATE_NONE:
                                //连接失败
                                if (ConnFailedAction != null)
                                {
                                    ConnFailedAction("连接失败");
                                }
                                break;
                        }
                        break;
                    case BluetoothHelper.MESSAGE_WRITE:
                        byte[] writeBuf = (byte[])msg.Obj;
                        // construct a string from the buffer
                        var writeMessage = new Java.Lang.String(writeBuf);
                        //bluetoothChat.conversationArrayAdapter.Add("も诀: " + writeMessage);
                        break;
                    case BluetoothHelper.MESSAGE_READ:
                        byte[] readBuf = (byte[])msg.Obj;
                        // construct a string from the valid bytes in the buffer
                        var readMessage = new Java.Lang.String(readBuf, 0, msg.Arg1);
                        //bluetoothChat.conversationArrayAdapter.Add(bluetoothChat.connectedDeviceName + ":  " + readMessage);
                        break;
                    case BluetoothHelper.MESSAGE_DEVICE_NAME:
                        // save the connected device's name
                        bluetoothChat.connectedDeviceName = msg.Data.GetString(BluetoothHelper.DEVICE_NAME);
                        bluetoothChat.connectedDeviceAddress = msg.Data.GetString(BluetoothHelper.DEVICE_ADDRESS);
                        //Toast.MakeText(Application.Context, "Connected to " + bluetoothChat.connectedDeviceName, ToastLength.Short).Show();
                        break;
                    case BluetoothHelper.MESSAGE_TOAST:
                        //Toast.MakeText(Application.Context, msg.Data.GetString(TOAST), ToastLength.Short).Show();
                        //蓝牙断开
                        //连接失败
                        if (ConnLost != null)
                        {
                            ConnLost("连接断开");
                        }
                        if (ConnFailedAction != null)
                        {
                            ConnFailedAction("连接断开");
                        }
                        break;
                    case BluetoothHelper.MESSAGE_CONNCETFAILED:
                        if (ConnLost != null)
                        {
                            ConnLost("连接断开");
                        }
                        if (ConnFailedAction != null)
                        {
                            ConnFailedAction("连接断开");
                        }
                        break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.Dispose();
            }
            if (chatService != null)
                chatService.Stop();
            disposed = true;
        }
        ~BluetoothHelper()
        {
            Dispose(false);
        }
    }

}
