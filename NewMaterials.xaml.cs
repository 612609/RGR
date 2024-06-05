using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Order
{
    /// <summary>
    /// Логика взаимодействия для NewMaterials.xaml
    /// </summary>
    public partial class NewMaterials : Window
    {
        public NewMaterials()
        {
            InitializeComponent();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MatName.Text) | string.IsNullOrEmpty(MatCost.Text))
            {
                int num = (int)MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                string command = "3|" + MatName.Text + "|" + MatCost.Text;
                client client = new client();
                client.SendSqlCommandButton_Click(command);
                Hide();
            }
        }

        private void MatCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
