using ServerSide;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
        public static Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public Chat1 messageView { get; set; }
        public RelayCommand AttachCommand { get; set; }
        public RelayCommand SendTextCommand { get; set; }

        public RelayCommand SendVoiceCommand { get; set; }

        public RelayCommand MouseEnterCommand { get; set; }
        public RelayCommand MouseLeaveCommand { get; set; }

        public RelayCommand LoadedCommand { get; set; }

        private ObservableCollection<string> _messages;

        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); }
        }

        private static readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 27001;
        public int Count { get; set; } = 0;

        public int SenderCount { get; set; } = 0;

        public MessageViewModel()
        {
            Messages = new ObservableCollection<string>();
            LoadedCommand = new RelayCommand((sender) =>
            {
                ConnectToServer();
                RequestLoop();
            });
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
                MessageBox.Show("SendVoice");

            });
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
                string text = Encoding.ASCII.GetString(data);
                Image img = ByteArrayToImage(data);
                
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (img!= null)
                    {
                        var stackpanel = new StackPanel();
                        stackpanel.Width = 650;
                        stackpanel.Height = 150;
                        stackpanel.Orientation = Orientation.Horizontal;
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
                    if (img==null)
                    {
                        var stackpanel = new StackPanel();
                        stackpanel.Width = 650;
                        stackpanel.Height = 60;
                        stackpanel.Orientation = Orientation.Horizontal;
                        TextBlock textBlock = new TextBlock();                
                        textBlock.Height = 60;
                        textBlock.FontSize = 25;
                        textBlock.Text = text;
                        textBlock.HorizontalAlignment = HorizontalAlignment.Right;                    
                        stackpanel.Children.Add(textBlock);
                        messageView.messageList.Items.Add(stackpanel);
                    }
                });
            }
            catch (Exception)
            {

            }
        }

    }
}
