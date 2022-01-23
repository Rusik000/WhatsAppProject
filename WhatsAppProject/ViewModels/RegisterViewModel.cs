using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;
using WhatsAppProject.Command;
using WhatsAppProject.Models;
using WhatsAppProject.Views;

namespace WhatsAppProject.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public RegisterView RegisterView { get; set; }

        private List<string> _cultureList;

        public List<string> CultureList
        {
            get { return _cultureList; }
            set { _cultureList = value; OnPropertyChanged(); }
        }


        public static string Number { get; set; }
        public RegionInfo GetRegionInfo { get; set; }


        public RelayCommand SelectionChanged { get; set; }

        public RelayCommand NextCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public Client client { get; set; }

       
        public RegisterViewModel()
        {
            LoadedCommand = new RelayCommand((sender) =>
            {
                if (File.Exists("Ruslan"))
                {
                    using (var fs = new FileStream("Ruslan", FileMode.OpenOrCreate))
                    {
                        using (var sr = new StreamReader(fs, Encoding.ASCII))
                        {
                            var text = sr.ReadToEnd();
                            if (text == "555280008" || text == "0555280008")
                            {
                                RegisterView.Hide();
                                ChatView chatView = new ChatView();
                                chatView.Show();
                            }
                        }
                    }
                }
            });
            SetData();
            SelectionChanged = new RelayCommand((sender) =>
            {
                GetCode(sender);
            });
            NextCommand = new RelayCommand((sender) =>
            {
                //WriteDataToFile();
                //SendPhoneNumber();
                VerificationView verificationView = new VerificationView();
                RegisterView.Hide();
                verificationView.Show();

            }, (pred) =>
            {
                if (RegisterView.PhoneTxtBx.Text.Length > 0)
                {
                    return true;
                }
                return false;
            });

        }

        private void WriteDataToFile()
        {
            User user = new User()
            {
                Name = RegisterView.FullnameTxtbx.Text,
                PhoneNumber = RegisterView.PhoneTxtBx.Text
            };


            using (FileStream fs = new FileStream($"{user.Name}.txt", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.Write(user.PhoneNumber);
                }
            }

        }
        private void SendPhoneNumber()
        {
            try
            {
                var accountSid = "ACfd0363cdec733b79d54f6cbc1512492a";
                var authToken = "38e5acb498b0cab3b7b1074ad6029b17";
                TwilioClient.Init(accountSid, authToken);

                Number = RegisterView.PhoneCodeTxtBx.Text + RegisterView.PhoneTxtBx.Text;

                VerificationResource.Create(
                    to: Number,
                    channel: "sms",
                    pathServiceSid: "VAe05df72632429af144c249c8b5ffaaa4"
                    );

            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void GetCode(object sender)
        {
            var item = sender as string;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            CultureInfo cInfo = cultures.FirstOrDefault(culture => new RegionInfo(culture.LCID).EnglishName == item);
            var region = new RegionInfo(cInfo.LCID);
            var code = GetCountriesDictionary().FirstOrDefault(x => x.Key == region.TwoLetterISORegionName).Value;
            RegisterView.PhoneCodeTxtBx.Text = "+" + code;
        }
        private void SetData()
        {
            CultureList = new List<string>();
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var getCulture in getCultureInfo)
            {
                GetRegionInfo = new RegionInfo(getCulture.LCID);
                if (!(CultureList.Contains(GetRegionInfo.EnglishName)))
                {
                    CultureList.Add(GetRegionInfo.EnglishName);
                }
            }

            CultureList.Sort();

        }
        public Dictionary<string, string> GetCountriesDictionary()
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("AC", "247");
            dictionary.Add("AD", "376");
            dictionary.Add("AE", "971");
            dictionary.Add("AF", "93");
            dictionary.Add("AG", "1-268");
            dictionary.Add("AI", "1-264");
            dictionary.Add("AL", "355");
            dictionary.Add("AM", "374");
            dictionary.Add("AN", "599");
            dictionary.Add("AO", "244");
            dictionary.Add("AR", "54");
            dictionary.Add("AS", "1-684");
            dictionary.Add("AT", "43");
            dictionary.Add("AU", "61");
            dictionary.Add("AW", "297");
            dictionary.Add("AX", "358-18");
            dictionary.Add("AZ", "994");
            dictionary.Add("BA", "387");
            dictionary.Add("BB", "1-246");
            dictionary.Add("BD", "880");
            dictionary.Add("BE", "32");
            dictionary.Add("BF", "226");
            dictionary.Add("BG", "359");
            dictionary.Add("BH", "973");
            dictionary.Add("BI", "257");
            dictionary.Add("BJ", "229");
            dictionary.Add("BM", "1-441");
            dictionary.Add("BN", "673");
            dictionary.Add("BO", "591");
            dictionary.Add("BR", "55");
            dictionary.Add("BS", "1-242");
            dictionary.Add("BT", "975");
            dictionary.Add("BW", "267");
            dictionary.Add("BY", "375");
            dictionary.Add("BZ", "501");
            dictionary.Add("CA", "1");
            dictionary.Add("CC", "61");
            dictionary.Add("CD", "243");
            dictionary.Add("CF", "236");
            dictionary.Add("CG", "242");
            dictionary.Add("CH", "41");
            dictionary.Add("CI", "225");
            dictionary.Add("CK", "682");
            dictionary.Add("CL", "56");
            dictionary.Add("CM", "237");
            dictionary.Add("CN", "86");
            dictionary.Add("CO", "57");
            dictionary.Add("CR", "506");
            dictionary.Add("CS", "381");
            dictionary.Add("CU", "53");
            dictionary.Add("CV", "238");
            dictionary.Add("CX", "61");
            dictionary.Add("CY", "357");
            dictionary.Add("CZ", "420");
            dictionary.Add("DE", "49");
            dictionary.Add("DJ", "253");
            dictionary.Add("DK", "45");
            dictionary.Add("DM", "1-767");
            dictionary.Add("DO", "1-809");
            dictionary.Add("DZ", "213");
            dictionary.Add("EC", "593");
            dictionary.Add("EE", "372");
            dictionary.Add("EG", "20");
            dictionary.Add("EH", "212");
            dictionary.Add("ER", "291");
            dictionary.Add("ES", "34");
            dictionary.Add("ET", "251");
            dictionary.Add("FI", "358");
            dictionary.Add("FJ", "679");
            dictionary.Add("FK", "500");
            dictionary.Add("FM", "691");
            dictionary.Add("FO", "298");
            dictionary.Add("FR", "33");
            dictionary.Add("GA", "241");
            dictionary.Add("GB", "44");
            dictionary.Add("GD", "1-473");
            dictionary.Add("GE", "995");
            dictionary.Add("GF", "594");
            dictionary.Add("GG", "44");
            dictionary.Add("GH", "233");
            dictionary.Add("GI", "350");
            dictionary.Add("GL", "299");
            dictionary.Add("GM", "220");
            dictionary.Add("GN", "224");
            dictionary.Add("GP", "590");
            dictionary.Add("GQ", "240");
            dictionary.Add("GR", "30");
            dictionary.Add("GT", "502");
            dictionary.Add("GU", "1-671");
            dictionary.Add("GW", "245");
            dictionary.Add("GY", "592");
            dictionary.Add("HK", "852");
            dictionary.Add("HN", "504");
            dictionary.Add("HR", "385");
            dictionary.Add("HT", "509");
            dictionary.Add("HU", "36");
            dictionary.Add("ID", "62");
            dictionary.Add("IE", "353");
            dictionary.Add("IL", "972");
            dictionary.Add("IM", "44");
            dictionary.Add("IN", "91");
            dictionary.Add("IO", "246");
            dictionary.Add("IQ", "964");
            dictionary.Add("IR", "98");
            dictionary.Add("IS", "354");
            dictionary.Add("IT", "39");
            dictionary.Add("JE", "44");
            dictionary.Add("JM", "1-876");
            dictionary.Add("JO", "962");
            dictionary.Add("JP", "81");
            dictionary.Add("KE", "254");
            dictionary.Add("KG", "996");
            dictionary.Add("KH", "855");
            dictionary.Add("KI", "686");
            dictionary.Add("KM", "269");
            dictionary.Add("KN", "1-869");
            dictionary.Add("KP", "850");
            dictionary.Add("KR", "82");
            dictionary.Add("KW", "965");
            dictionary.Add("KY", "1-345");
            dictionary.Add("KZ", "7");
            dictionary.Add("LA", "856");
            dictionary.Add("LB", "961");
            dictionary.Add("LC", "1-758");
            dictionary.Add("LI", "423");
            dictionary.Add("LK", "94");
            dictionary.Add("LR", "231");
            dictionary.Add("LS", "266");
            dictionary.Add("LT", "370");
            dictionary.Add("LU", "352");
            dictionary.Add("LV", "371");
            dictionary.Add("LY", "218");
            dictionary.Add("MA", "212");
            dictionary.Add("MC", "377");
            dictionary.Add("MD", "373");
            dictionary.Add("ME", "382");
            dictionary.Add("MG", "261");
            dictionary.Add("MH", "692");
            dictionary.Add("MK", "389");
            dictionary.Add("ML", "223");
            dictionary.Add("MM", "95");
            dictionary.Add("MN", "976");
            dictionary.Add("MO", "853");
            dictionary.Add("MP", "1-670");
            dictionary.Add("MQ", "596");
            dictionary.Add("MR", "222");
            dictionary.Add("MS", "1-664");
            dictionary.Add("MT", "356");
            dictionary.Add("MU", "230");
            dictionary.Add("MV", "960");
            dictionary.Add("MW", "265");
            dictionary.Add("MX", "52");
            dictionary.Add("MY", "60");
            dictionary.Add("MZ", "258");
            dictionary.Add("NA", "264");
            dictionary.Add("NC", "687");
            dictionary.Add("NE", "227");
            dictionary.Add("NF", "672");
            dictionary.Add("NG", "234");
            dictionary.Add("NI", "505");
            dictionary.Add("NL", "31");
            dictionary.Add("NO", "47");
            dictionary.Add("NP", "977");
            dictionary.Add("NR", "674");
            dictionary.Add("NU", "683");
            dictionary.Add("NZ", "64");
            dictionary.Add("OM", "968");
            dictionary.Add("PA", "507");
            dictionary.Add("PE", "51");
            dictionary.Add("PF", "689");
            dictionary.Add("PG", "675");
            dictionary.Add("PH", "63");
            dictionary.Add("PK", "92");
            dictionary.Add("PL", "48");
            dictionary.Add("PM", "508");
            dictionary.Add("PR", "1-787");
            dictionary.Add("PS", "970");
            dictionary.Add("PT", "351");
            dictionary.Add("PW", "680");
            dictionary.Add("PY", "595");
            dictionary.Add("QA", "974");
            dictionary.Add("RE", "262");
            dictionary.Add("RO", "40");
            dictionary.Add("RS", "381");
            dictionary.Add("RU", "7");
            dictionary.Add("RW", "250");
            dictionary.Add("SA", "966");
            dictionary.Add("SB", "677");
            dictionary.Add("SC", "248");
            dictionary.Add("SD", "249");
            dictionary.Add("SE", "46");
            dictionary.Add("SG", "65");
            dictionary.Add("SH", "290");
            dictionary.Add("SI", "386");
            dictionary.Add("SJ", "47");
            dictionary.Add("SK", "421");
            dictionary.Add("SL", "232");
            dictionary.Add("SM", "378");
            dictionary.Add("SN", "221");
            dictionary.Add("SO", "252");
            dictionary.Add("SR", "597");
            dictionary.Add("ST", "239");
            dictionary.Add("SV", "503");
            dictionary.Add("SY", "963");
            dictionary.Add("SZ", "268");
            dictionary.Add("TA", "290");
            dictionary.Add("TC", "1-649");
            dictionary.Add("TD", "235");
            dictionary.Add("TG", "228");
            dictionary.Add("TH", "66");
            dictionary.Add("TJ", "992");
            dictionary.Add("TK", "690");
            dictionary.Add("TL", "670");
            dictionary.Add("TM", "993");
            dictionary.Add("TN", "216");
            dictionary.Add("TO", "676");
            dictionary.Add("TR", "90");
            dictionary.Add("TT", "1-868");
            dictionary.Add("TV", "688");
            dictionary.Add("TW", "886");
            dictionary.Add("TZ", "255");
            dictionary.Add("UA", "380");
            dictionary.Add("UG", "256");
            dictionary.Add("US", "1");
            dictionary.Add("UY", "598");
            dictionary.Add("UZ", "998");
            dictionary.Add("VA", "379");
            dictionary.Add("VC", "1-784");
            dictionary.Add("VE", "58");
            dictionary.Add("VG", "1-284");
            dictionary.Add("VI", "1-340");
            dictionary.Add("VN", "84");
            dictionary.Add("VU", "678");
            dictionary.Add("WF", "681");
            dictionary.Add("WS", "685");
            dictionary.Add("YE", "967");
            dictionary.Add("YT", "262");
            dictionary.Add("ZA", "27");
            dictionary.Add("ZM", "260");
            dictionary.Add("ZW", "263");
            return dictionary;
        }
    }
}
