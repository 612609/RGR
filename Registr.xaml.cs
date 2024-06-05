using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows;
namespace Order
{
    /// <summary>
    /// Логика взаимодействия для Registr.xaml
    /// </summary>
    public partial class Registr : Window
    {
        client client = new client();
        public Registr()
        {
            InitializeComponent();

        }
        private string code1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(login.Text) | string.IsNullOrEmpty(password.Password) | string.IsNullOrEmpty(AdminCode.Password))
            {
                MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (AdminCode.Password == code1)
                   
                {
                    Cripts cripts = new Cripts();
                    string command = "2|" + login.Text + "|" + cripts.ComputeSha256Hash(password.Password);
                    
                    client.SendSqlCommandButton_Click(command);
                    Hide();
                    LogForm logForm = new LogForm();
                    logForm.Show();
                }
                else
                {
                    MessageBox.Show("Не верный код, регистрация не возможна");
                }
            }
        }

        private void registr_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            LogForm logForm = new LogForm();
            logForm.Show();
        }
       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string command = "30|" + login.Text;

            DataTable  dt =client.SendSqlCommandButton_Click(command);
            if (dt.Rows[0].Field<string>(0)== "Введите корпоративную почту" || dt.Rows[0].Field<string>(0) == "Не верно введена почта")
            {
                MessageBox.Show(dt.Rows[0].Field<string>(0), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else code1 = dt.Rows[0].Field<string>(0);
        }
    }
}
