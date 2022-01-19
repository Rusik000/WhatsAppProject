﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsAppProject.Command;
using WhatsAppProject.Models;
using WhatsAppProject.Views;
using WhatsAppProject.Views.UserControls;

namespace WhatsAppProject.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public ChatView chatView { get; set; }

        private ObservableCollection<Client> _clients;

        public ObservableCollection<Client> Clients
        {
            get { return _clients; }
            set { _clients = value; OnPropertyChanged(); }
        }

        public RelayCommand MouseEnterCommand { get; set; }
        public RelayCommand MouseLeaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand FilterCommand { get; set; }
        public RelayCommand SelectionChangedCommand { get; set; }



        private static readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 27001;
        public ChatViewModel()
        {
            GetClients();
            FilterCommand = new RelayCommand((sender) =>
            {

            });
            SelectionChangedCommand = new RelayCommand((sender) =>
            {
                var item = chatView.ClientsListBx.SelectedItem as Client;
                CarryItemToUserControl(item);
            });
            MouseEnterCommand = new RelayCommand((sender) =>
            {
                if (chatView.SearchTxtbx.Text == "Search...")
                {
                    chatView.SearchTxtbx.Text = String.Empty;
                    chatView.SearchTxtbx.Foreground = Brushes.Gray;
                }
            });
            MouseLeaveCommand = new RelayCommand((sender) =>
            {
                if (chatView.SearchTxtbx.Text == String.Empty)
                {
                    chatView.SearchTxtbx.Foreground = Brushes.Gray;
                    chatView.SearchTxtbx.Text = "Search...";
                }
                if (chatView.SearchTxtbx.Text != String.Empty)
                {
                    chatView.SearchTxtbx.Foreground = Brushes.Black;
                }
                if (chatView.SearchTxtbx.Text == "Search...")
                {
                    chatView.SearchTxtbx.Foreground = Brushes.Gray;
                }
            });
            CancelCommand = new RelayCommand((sender) =>
            {
                chatView.Hide();
            });
        }

        private void CarryItemToUserControl(Client client)
        {
            string imagePath = client.ImagePath;
            Chat1 chat1 = new Chat1();
            if (client != null)
            {
                var converter = new ImageSourceConverter();
                chat1.ProfileImage.ImageSource =
                    (ImageSource)converter.ConvertFromString($"pack://application:,,,{imagePath}");

                chat1.FullnameTxtBlck.Text = client.FullName;
                chatView.MessageGrid.Children.Add(chat1);
                //ConnectToServer();
            }
            if (client == null)
            {
                return;
            }
        }

        private void ConnectToServer()
        {
            int attempts = 0;
            while (!MessageViewModel.ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    MessageViewModel.ClientSocket.Connect(IPAddress.Loopback, PORT);
                }
                catch (SocketException)
                {

                }
            }
            MessageBox.Show("Connected");
        }
        private void GetClients()
        {
            Clients = new ObservableCollection<Client>
            {
                new Client
                {
                    Id=1,
                    FullName="Racon Ziqi",
                    ImagePath="../Images/Ziqi.jpg",
                    Message="Nagarirsan Kele?",
                    Time="8:28 AM"
                },
                new Client
                {
                    Id=2,
                    FullName="Kamran Step",
                    ImagePath="../Images/Kamran.jpg",
                    Message="Derse gel",
                    Time="4:25 AM"
                },
                new Client
                {
                    Id=3,
                    FullName="Huseyn Step",
                    ImagePath="../Images/Huseyn.jpg",
                    Message="Gotur gor kimdi?",
                    Time="2:25 PM"
                },
                new Client
                {
                    Id=4,
                    FullName="Refael Step",
                    ImagePath="../Images/Refeal.jpg",
                    Message="Memati zor oynayir ee",
                    Time="03:40 AM"
                },
                new Client
                {
                    Id=5,
                    FullName="Murad Mati",
                    ImagePath="../Images/Murad.jpg",
                    Message="KFC-nin qabagindayam gel",
                    Time="  5:40 PM"
                },
                new Client
                {
                    Id=6,
                    FullName="0513861009",
                    ImagePath="../Images/Kenan.jpg",
                    Message="teamsden kod atmisam baxa bilersen?",
                    Time="7:40 PM"
                }
            };
        }
    }
}
