using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace Order
{
    /// <summary>
    /// Логика взаимодействия для ChangeClients.xaml
    /// </summary>
    public partial class ChangeClients : Window
    {
        
        int index = 0;
        public ChangeClients(int id, string name, string famili, string patronimic, string numver, string note)
        {
            InitializeComponent();
            NameC.Text = name;
            FamiliC.Text = famili;
            PatronimicC.Text = patronimic;
            NumberC.Text = numver;
            NoteC.Text = note;
            index = id;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameC.Text) | string.IsNullOrEmpty(FamiliC.Text) | NumberC.Text.Length != 18)
            {
                MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string command = "13|" + Cripts.Encrypt(NameC.Text) + "|" + Cripts.Encrypt(FamiliC.Text) + "|" + Cripts.Encrypt(PatronimicC.Text) + "|" + Cripts.Encrypt(NumberC.Text) + "|" + NoteC.Text + "|" + index;
                client client = new client();
                client.SendSqlCommandButton_Click(command);
                Hide();
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
