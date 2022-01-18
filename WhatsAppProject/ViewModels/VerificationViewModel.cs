using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Twilio.Rest.Verify.V2.Service;
using WhatsAppProject.Command;
using WhatsAppProject.Views;

namespace WhatsAppProject.ViewModels
{
    public class VerificationViewModel : BaseViewModel
    {
        public VerificationView verificationView { get; set; }
        public RelayCommand NextCommand { get; set; }

        public int Count { get; set; } = 0;
        public string Code { get; set; }
        public VerificationViewModel()
        {
            NextCommand = new RelayCommand((sender) =>
            {
                //VerificationCode();
                ChatView chatView = new ChatView();
                verificationView.Hide();
                chatView.Show();

            }, (pred) =>
            {
                if (verificationView.AgreeTglBtn.IsChecked == true &&
                    verificationView.CodeVerifyTxtBx.Text.Length==6)
                {
                    return true;
                }
                return false;
            });
        }
        private void VerificationCode()
        {
            if (Count == 0)
            {
                try
                {
                    Code = verificationView.CodeVerifyTxtBx.Text;
                    VerificationCheckResource.Create(
                        to: RegisterViewModel.Number,
                        code: Code,
                        pathServiceSid: "VAe05df72632429af144c249c8b5ffaaa4"
                        );
                    Count++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("You send before this number !!!");
            }
        }
    }
}
