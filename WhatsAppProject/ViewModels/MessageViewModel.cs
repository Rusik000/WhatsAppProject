using ServerSide;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WhatsAppProject.Command;
using WhatsAppProject.Views.UserControls;

namespace WhatsAppProject.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public static Socket ClientSocket =
        new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 27001;
        public Chat1 messageView { get; set; }
        public RelayCommand AttachCommand { get; set; }
        public RelayCommand SendTextCommand { get; set; }

        public RelayCommand SendVoiceCommand { get; set; }

        public RelayCommand MouseEnterCommand { get; set; }
        public RelayCommand MouseLeaveCommand { get; set; }

        public RelayCommand LoadedCommand { get; set; }
        public int Count { get; set; } = 0;

        public static bool isFile { get; set; } = true;
        public static bool isText { get; set; } = false;
        public static bool isImage { get; set; } = false;

        public static int CountFile { get; set; } = 0;
        public bool RecordVoice { get; set; } = true;

        public string FilePath { get; set; } = String.Empty;


        Timer timer1 = new Timer();

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int record(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        public MessageViewModel()
        {
            LoadedCommand = new RelayCommand((sender) =>
            {
                ConnectToServer();
            });

            RequestLoop();

            AttachCommand = new RelayCommand((sender) =>
            {
                AddAttachUserControl();
            });

            MouseEnterCommand = new RelayCommand((sender) =>
            {
                if (messageView.MessageTxtbx.Text == "Type a message")
                {
                    messageView.MessageTxtbx.Text = String.Empty;
                    messageView.MessageTxtbx.Foreground = Brushes.Gray;
                }

            });
            MouseLeaveCommand = new RelayCommand((sender) =>
            {
                if (messageView.MessageTxtbx.Text == String.Empty)
                {
                    messageView.MessageTxtbx.Foreground = Brushes.Gray;
                    messageView.MessageTxtbx.Text = "Type a message";
                }
                if (messageView.MessageTxtbx.Text != String.Empty)
                {
                    messageView.MessageTxtbx.Foreground = Brushes.Black;
                }
                if (messageView.MessageTxtbx.Text == "Type a message")
                {
                    messageView.MessageTxtbx.Foreground = Brushes.Gray;
                }
            });

            SendTextCommand = new RelayCommand((sender) =>
            {
                SendRequest();
            });
            SendVoiceCommand = new RelayCommand((sender) =>
            {

            });
        }

        private void EndToRecord()
        {
            timer1.Stop();
            timer1.Enabled = false;
            record("save recsound C:/Users/rusla/OneDrive", "", 0, 0);
            record("close recsound", "", 0, 0);
        }
        private void BeginToRecord()
        {
            timer1.Enabled = true;
            timer1.Start();
            record("open new Type waveaudio Alias recsound", "", 0, 0);
            record("record recsound", "", 0, 0);
        }
        private void AddAttachUserControl()
        {
            if (Count == 0)
            {
                AttachUserControl UserCtrl = new AttachUserControl();
                messageView.HiddenGrid.Children.Add(UserCtrl);
            }
            Count++;
            if (Count % 2 != 0)
            {
                messageView.HiddenGrid.Visibility = Visibility.Visible;
            }
            if (Count % 2 == 0)
            {
                messageView.HiddenGrid.Visibility = Visibility.Hidden;
            }
        }
        private void SendRequest()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                string text = messageView.MessageTxtbx.Text;
                SendString(text);
            });
        }
        private void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

        }
        private void ConnectToServer()
        {
            int attempts = 0;
            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    ClientSocket.Connect(IPAddress.Loopback, PORT);
                }
                catch (SocketException)
                {

                }
            }
            MessageBox.Show("Connected to Server");

        }
        private void RequestLoop()
        {
            var receiver = Task.Run(() =>
            {
                while (true)
                {
                    ReceiveResponse();
                }
            });

        }
        public Image ByteArrayToImage(byte[] buffer)
        {
            Image returnImage = null;
            string path = Encoding.UTF8.GetString(buffer);
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    returnImage = new Image();
                    returnImage.Source = new BitmapImage(new Uri(path));
                });
                return returnImage;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void ReceiveResponse()
        {
            try
            {
                var buffer = new byte[2048];
                int received = ClientSocket.Receive(buffer, SocketFlags.None);
                if (received == 0) return;
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                var checkedText = Encoding.ASCII.GetString(data);
                App.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        string[] sub = checkedText.Split('.');

                        if (sub[1] == "txt" || sub[1] == "docx" || sub[1] == "pdf")
                        {
                            ReceiveFile(data);
                        }
                        else if (sub[1] == "jpg" || sub[1] == "png" || sub[1] == "PNG" || sub[1] == "JPG")
                        {
                            ReceiveImage(data);
                        }
                    }
                    catch (Exception)
                    {
                        ReceiveText(data);
                    }




                });
            }
            catch (Exception)
            {

            }
        }

        private void ReceiveFile(byte[] data)
        {

            string text = Encoding.ASCII.GetString(data);
            FilePath = text;
            var stackpanel = new StackPanel();
            stackpanel.Width = 650;
            stackpanel.Height = 150;
            stackpanel.Orientation = Orientation.Horizontal;
            Button button = new Button();
            Thickness margin1 = button.Margin;
            margin1.Left = 400;
            button.Margin = margin1;
            button.Width = 100;
            button.Height = 50;
            button.Click += Button_Click;
            button.Content = "Open File";
            button.FontSize = 15;
            button.Background = Brushes.DarkGreen;
            Image pdfimage = null;
            pdfimage = new Image();
            pdfimage.Width = 100;
            pdfimage.Height = 100;
            pdfimage.Source = new BitmapImage(new Uri("C:/Users/rusla/Source/Repos/WhatsAppProject/WhatsAppProject/Images/File.png"));
            Thickness margin = pdfimage.Margin;
            margin.Left = 10;
            pdfimage.Margin = margin;
            pdfimage.HorizontalAlignment = HorizontalAlignment.Right;
            pdfimage.Stretch = Stretch.Fill;
            stackpanel.Children.Add(button);
            stackpanel.Children.Add(pdfimage);
            messageView.messageList.Items.Add(stackpanel);

        }

        private void ReceiveImage(byte[] data)
        {
            try
            {
                Image img = ByteArrayToImage(data);
                var stackpanel = new StackPanel();
                stackpanel.Width = 650;
                stackpanel.Height = 150;
                img.Width = 170;
                img.Height = 150;
                Thickness margin = img.Margin;
                margin.Left = 500;
                img.Margin = margin;
                img.HorizontalAlignment = HorizontalAlignment.Right;
                img.Stretch = Stretch.Fill;
                stackpanel.Children.Add(img);
                messageView.messageList.Items.Add(stackpanel);
            }
            catch (Exception)
            {

            }
        }

        private void ReceiveText(byte[] data)
        {
            string text = Encoding.ASCII.GetString(data);
            var stackpanel = new StackPanel();
            stackpanel.Width = 650;
            stackpanel.Height = 60;
            TextBlock textBlock = new TextBlock();
            textBlock.Height = 60;
            textBlock.FontSize = 25;
            textBlock.Text = text;
            textBlock.HorizontalAlignment = HorizontalAlignment.Right;
            stackpanel.Children.Add(textBlock);
            messageView.messageList.Items.Add(stackpanel);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(FilePath);
        }
    }
}
