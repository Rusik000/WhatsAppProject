using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                MessageBox.Show("SendImage");
            });
            SendFileCommand = new RelayCommand((sender) =>
            {
                MessageBox.Show("SendFile");

            });
            SendLocationCommand = new RelayCommand((sender) =>
            {
                MessageBox.Show("SendLocation");
            });
        }
    }
}
