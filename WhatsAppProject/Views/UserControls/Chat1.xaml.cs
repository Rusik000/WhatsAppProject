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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhatsAppProject.ViewModels;

namespace WhatsAppProject.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Chat1.xaml
    /// </summary>
    public partial class Chat1 : UserControl
    {
        public Chat1()
        {
            InitializeComponent();
            this.DataContext = new MessageViewModel() { messageView = this };
        }

        
    }
}
