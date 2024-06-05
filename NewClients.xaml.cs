using System.Text.RegularExpressions;
using System.Windows;
namespace Order
{
    /// <summary>
    /// Логика взаимодействия для NewClients.xaml
    /// </summary>
    public partial class NewClients : Window
    {
        public NewClients()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameC.Text) | string.IsNullOrEmpty(FamiliC.Text) | NumberC.Text.Length != 18)
            {
                MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string command = "14|" + Cripts.Encrypt(NameC.Text) + "|" + Cripts.Encrypt(FamiliC.Text) + "|" + Cripts.Encrypt(PatronimicC.Text) + "|" + Cripts.Encrypt(NumberC.Text) + "|" + NoteC.Text;
                client client = new client();
                client.SendSqlCommandButton_Click(command);
                Hide();
            }
        }

        public async Task PhoneMask()
        {
            int PhoneLength = 11;
            var newVal = Regex.Replace(NumberC.Text, @"[^0-9]", "");
            if (PhoneLength != newVal.Length && !string.IsNullOrEmpty(newVal))
            {
                NumberC.Text = string.Empty;

                if (newVal.Length <= 1)
                {
                    NumberC.Text = Regex.Replace(newVal, @"(\d{1})", "+$1");
                }
                else if (newVal.Length <= 4)
                {
                    NumberC.Text = Regex.Replace(newVal, @"(\d{1})(\d{0,3})", "+$1($2)");
                }
                else if (newVal.Length <= 7)
                {
                    NumberC.Text = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})", "+$1($2)$3");
                }
                else if (newVal.Length <= 9)
                {
                    NumberC.Text = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})", "+$1($2)$3-$4");
                }
                else if (newVal.Length > 9)
                {
                    NumberC.Text = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})(\d{0,2})", "+$1($2)$3-$4-$5");
                }
            }
        }

        private void NumberC_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string input = e.Text;

            if (!Regex.IsMatch(input, @"[0-9]") || NumberC.Text.Length >= 18)
            {
                e.Handled = true;
                return;
            }

            if (NumberC.Text.Length == 0)
            {
                NumberC.Text = "+7 (";
                NumberC.SelectionStart = NumberC.Text.Length;
            }

            if (NumberC.Text.Length == 7)
            {
                NumberC.Text += ") ";
                NumberC.SelectionStart = NumberC.Text.Length;
            }

            if (NumberC.Text.Length == 12 || NumberC.Text.Length == 15)
            {
                NumberC.Text += "-";
                NumberC.SelectionStart = NumberC.Text.Length;
            }
        }
    }
}
