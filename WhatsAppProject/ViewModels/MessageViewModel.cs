using ServerSide;
using System;
using System.Collections.Generic;
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

        public int Count { get; set; } = 0;

        public MessageViewModel()
        {
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
                RequestLoop();
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

        private void RequestLoop()
        {
            var sender = Task.Run(() =>
            {
                while (true)
                {
                    SendRequest();
                }
            });
            var receiver = Task.Run(() =>
            {
                while (true)
                {
                    ReceiveResponse();
                }
            });
            Task.WaitAll(sender, receiver);
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

        private void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            messageView.ExampleTxtBx.Text = text;
        }
    }
}
