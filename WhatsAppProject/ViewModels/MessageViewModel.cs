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
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            MessageBox.Show("Connected");
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

        public T FindDescendant<T>(DependencyObject obj) where T : DependencyObject
        {

            if (obj is T)
                return obj as T;


            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (childrenCount < 1)
                return null;


            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return child as T;
            }


            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (child != null && child is T)
                    return child as T;
            }

            return null;
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
                App.Current.Dispatcher.Invoke(() =>
                {
                    SenderCount++;
                    Messages.Add(text);
                    if (SenderCount % 2 == 0)
                    {
                        TextBlock nameBlock = FindDescendant<TextBlock>(messageView.messageList);
                        nameBlock.TextAlignment = TextAlignment.Left;
                    }
                    if (SenderCount % 2 != 0)
                    {
                        TextBlock nameBlock = FindDescendant<TextBlock>(messageView.messageList);
                        nameBlock.TextAlignment = TextAlignment.Right;
                    }
                });
            }
            catch (Exception)
            {

            }
        }

    }
}
