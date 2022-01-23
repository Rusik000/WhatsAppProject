using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsAppProject.Command;
using WhatsAppProject.Views.UserControls;

namespace WhatsAppProject.ViewModels
{
    public class AttachUserControlViewModel : BaseViewModel
    {
        public AttachUserControl attachView { get; set; }

        public RelayCommand SendImageCommand { get; set; }
        public RelayCommand SendFileCommand { get; set; }
        public RelayCommand SendLocationCommand { get; set; }
        public AttachUserControlViewModel()
        {
            SendImageCommand = new RelayCommand((sender) =>
            {
                SendImage();            
            });
            SendFileCommand = new RelayCommand((sender) =>
            {
                SendFile();
                MessageViewModel.isFile = true;                
            });
            SendLocationCommand = new RelayCommand((sender) =>
            {
                
            });
        }

        private void SendFile()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "TXT (*.txt)|*.txt|PDF (*.pdf)|*.pdf|DOCX (*.docx)|*.docx";
            open.FilterIndex = 1;
            open.Multiselect = false;
            if (Convert.ToBoolean(open.ShowDialog()) == true)
            {
                string text = open.FileName;
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                MessageViewModel.ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
        }

        private void SendImage()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "PNG (*.png)|*.png|JPEG (*.jpeg)|*.jpeg|JPG (*.jpg)|*.jpg";
            open.FilterIndex = 1;
            open.Multiselect = false;
            if (Convert.ToBoolean(open.ShowDialog()) == true)
            {
                string text = open.FileName;
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                MessageViewModel.ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
        }
    }
}
