using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Order
{
    /// <summary>
    /// Логика взаимодействия для NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        public NewOrder()
        {
            client client = new client();
            InitializeComponent();
            string command7 = "20|";
            DataTable dataTable5 = client.SendSqlCommandButton_Click(command7);
            foreach (DataRow row in dataTable5.Rows)
            {
                // Добавление значений из трех столбцов каждой строки в список costomer
                costomer.Items.Add(Cripts.Decrypt(row[1].ToString()) + " " + Cripts.Decrypt(row[2].ToString()) + " " + Cripts.Decrypt(row[3].ToString()));
            }
            string command6 = "19|";
            DataTable dataTable4 = client.SendSqlCommandButton_Click(command6);
            foreach (DataRow row in dataTable4.Rows)
            {
                MaterialsCombo.Items.Add(row[1].ToString());
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(MaterialsCombo.Text) | string.IsNullOrEmpty(Number.Text))
            {
                int num1 = (int)MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                string text = MaterialsCombo.Text;
                bool flag = false;
                for (int index = 0; index < Materials.Items.Count; ++index)
                {
                    if ((Materials.Columns[0].GetCellContent((DataGridRow)Materials.ItemContainerGenerator.ContainerFromIndex(index)) as TextBlock).Text == text)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    int num2 = (int)MessageBox.Show("Данный материал уже добавлен, если хотите изменить сначала удалите старую запись", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {

                    Materials.Items.Add(new
                    {
                        MatName = MaterialsCombo.Text,
                        Number = Number.Text
                    });
                    Number.Text = "";
                    MaterialsCombo.Text = "";
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Materials.SelectedItem != null)
            {
                if (MessageBox.Show("Вы действительно хотите удалить данные?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
                    return;
                Materials.Items.Remove(Materials.SelectedItem);
            }
            else
            {
                int num = (int)MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(title.Text) | string.IsNullOrEmpty(costomer.Text) | string.IsNullOrEmpty(finishdata.Text) | string.IsNullOrEmpty(Worker.Text) | string.IsNullOrEmpty(Other.Text) | string.IsNullOrEmpty(TechWork.Text) | Materials.Items.Count == 0)
            {
                int num1 = (int)MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (finishdata.SelectedDate.Value < DateTime.Now)
            {
                int num2 = (int)MessageBox.Show("Нельзя добавлять заказы прошедшим числом", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {

                client client = new client();
                Int128 int128 = (Int128)0;
                for (int index = 0; index < Materials.Items.Count; ++index)
                {
                    DataGridRow dataGridRow = (DataGridRow)Materials.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock cellContent1 = Materials.Columns[0].GetCellContent(dataGridRow) as TextBlock;
                    TextBlock cellContent2 = Materials.Columns[1].GetCellContent(dataGridRow) as TextBlock;
                    string command = "5|" + cellContent1.Text;

                    DataTable dt = client.SendSqlCommandButton_Click(command);
                    string fieldValue = dt.Rows[0].Field<string>(0);
                    int128 += Int128.Parse(fieldValue) * Int128.Parse(cellContent2.Text);
                }

                string[] parts = (costomer.Text).Split(' ');
                string command3 = "12|" + Cripts.Encrypt(parts[0]) + " " + Cripts.Encrypt(parts[1]) + " " + Cripts.Encrypt(parts[2]); ;

                DataTable dataTable1 = client.SendSqlCommandButton_Click(command3);
                Int128 price = int128 + Convert.ToInt64((Worker.Text)) + Convert.ToInt64(TechWork.Text) + Convert.ToInt64(Other.Text);

                string command1 = "7|" + title.Text + "|" + dataTable1.Rows[0].Field<string>(0) + "|" + price + "|" + DateTime.Now.ToString("yyyy-MM-dd") + "|" + finishdata.SelectedDate.Value.ToString("yyyy-MM-dd") + "|" + Worker.Text + "|" + TechWork.Text + "|" + Other.Text;

                client.SendSqlCommandButton_Click(command1);

                string command2 = "8|" + title.Text + "|" + dataTable1.Rows[0].Field<string>(0) + "|" + price + "|" + DateTime.Now.ToString("yyyy-MM-dd") + "|" + finishdata.SelectedDate.Value.ToString("yyyy-MM-dd") + "|" + Worker.Text + "|" + TechWork.Text + "|" + Other.Text;

                DataTable dataTable3 = client.SendSqlCommandButton_Click(command2);
                
                string idOrd = dataTable3.Rows[0].Field<string>(0);
                for (int index = 0; index < Materials.Items.Count; ++index)
                {
                    DataGridRow dataGridRow = (DataGridRow)Materials.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock cellContent3 = Materials.Columns[0].GetCellContent(dataGridRow) as TextBlock;
                    TextBlock cellContent4 = Materials.Columns[1].GetCellContent(dataGridRow) as TextBlock;
                    string command = "11|" + cellContent3.Text;
                    DataTable dataTable5 =client.SendSqlCommandButton_Click(command);
                    string command5 = "18|" + dataTable5.Rows[0].Field<string>(0) + "|" + idOrd + "|" + cellContent4.Text;
                    client.SendSqlCommandButton_Click(command5);
                }

                Hide();
            }
        }

        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
