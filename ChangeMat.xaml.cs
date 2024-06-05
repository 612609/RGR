using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Order
{
    /// <summary>
    /// Логика взаимодействия для ChangeMat.xaml
    /// </summary>
    public partial class ChangeMat : Window
    {
        private int index;

        public ChangeMat(int id, string title, string price)
        {
            InitializeComponent();
            index = id;
            Name.Text = title;
            Price.Text = price;
        }

        private void Price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Price.Text) | string.IsNullOrEmpty(Name.Text))
            {
                int num = (int)MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                string command = "4|" + Name.Text + "|" + Price.Text + "|" + index;
                client client = new client();
                DataTable dt = client.SendSqlCommandButton_Click(command);
                Hide();

            }
        }
    }
}
