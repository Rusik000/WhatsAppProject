using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WhatsAppProject.ViewModels;

namespace WhatsAppProject.Views
{
    /// <summary>
    /// Interaction logic for VerificationView.xaml
    /// </summary>
    public partial class VerificationView : Window
    {
        public VerificationView()
        {
            InitializeComponent();
            this.DataContext = new VerificationViewModel() { verificationView = this };
        }
    }
}
