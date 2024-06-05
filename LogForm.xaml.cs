using System.Data;
using System.Windows;

namespace Order
{
    /// <summary>
    /// Логика взаимодействия для LogForm.xaml
    /// </summary>
    public partial class LogForm : Window
    {
        public LogForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cripts cripts = new Cripts();
            string command = "1|" + login.Text + "|" + cripts.ComputeSha256Hash(password.Password);
            client client = new client();
            DataTable dt = client.SendSqlCommandButton_Click(command);
            
            if (dt.Rows.Count > 0)
            {
                Hide();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Не верно введены логин или пароль");
            }
        }

        private void registr_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Registr registr = new Registr();
            registr.Show();
        }

       
    }
}
