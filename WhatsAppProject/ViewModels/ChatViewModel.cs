using System;
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
        public int Count { get; set; } = 0;
        public ChatViewModel()
        {
            GetClients();
            FilterCommand = new RelayCommand((sender) =>
            {

            });
            SelectionChangedCommand = new RelayCommand((sender) =>
            {
                Count++;
                if (Count==1)
                {
                    var item = chatView.ClientsListBx.SelectedItem as Client;
                    CarryItemToUserControl(item);
                }
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

            }
            if (client == null)
            {
                return;
            }
        }
        private void GetClients()
        {
            Clients = new ObservableCollection<Client>
            {
                new Client
                {
                    Id=1,
                    FullName="John Wick",
                    ImagePath="../Images/Profile1.png",
                    Message="How are you?",
                    Time="9:28 AM"
                }
            };
        }
    }
}
