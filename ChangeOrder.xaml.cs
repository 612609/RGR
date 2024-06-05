using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Order
{
    /// <summary>
    /// Логика взаимодействия для ChangeOrder.xaml
    /// </summary>
    public partial class ChangeOrder : Window
    {
        int index = 0;
        public ChangeOrder(int id)
        {
            InitializeComponent();           
            client client = new client();
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
            string command = "9|" + id.ToString();
 
            DataTable dt = client.SendSqlCommandButton_Click(command);
            
            foreach (DataRow row in dt.Rows)
            {
                string finishdata = row["finishdata"].ToString();

                // Изменяем формат даты
                finishdata = Regex.Replace(finishdata, @"(\d{2})/(\d{2})/(\d{4})", "$2.$1.$3");

                // Помещаем измененное значение обратно в таблицу
                row["finishdata"] = finishdata;
            }
            index = id;
            title.Text = dt.Rows[0].Field<string>(0);
            string[] parts = (dt.Rows[0].Field<string>(1)).Split(' ');
            costomer.Text = Cripts.Decrypt(parts[0]) + " " + Cripts.Decrypt(parts[1]) + " " + Cripts.Decrypt(parts[2]);
            finishdata.SelectedDate = (DateTime.Parse(dt.Rows[0].Field<string>(2))).Date;
            Worker.Text = dt.Rows[0].Field<string>(3);
            TechWork.Text = dt.Rows[0].Field<string>(4);
            Other.Text = dt.Rows[0].Field<string>(5);
            string command1 = "10|" + id.ToString();
            DataTable dataTable3 = client.SendSqlCommandButton_Click(command1);
            Materials.ItemsSource = (IEnumerable)dataTable3.DefaultView;
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
                    DataTable table = ((DataView)Materials.ItemsSource).ToTable();
                    table.Rows.Add(MaterialsCombo.Text, Number.Text);
                    Materials.ItemsSource = (IEnumerable)null;
                    Materials.ItemsSource = (IEnumerable)table.DefaultView;
                    Number.Text = "";
                    MaterialsCombo.Text = "";
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Materials.SelectedItem is DataRowView)
            {
                if (MessageBox.Show("Вы действительно хотите удалить данные?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
                    return;
                DataTable dataTable = new DataTable();
                DataTable table = ((DataView)Materials.ItemsSource).ToTable();
                DataRowView selectedItem = Materials.SelectedItem as DataRowView;
                Materials.ItemsSource = (IEnumerable)null;
                for (int index = table.Rows.Count - 1; index >= 0; --index)
                {
                    DataRow row = table.Rows[index];
                    if (row[0] == (object)selectedItem.Row[0].ToString())
                        row.Delete();
                }
                Materials.ItemsSource = (IEnumerable)table.DefaultView;
            }
            else
            {
                int num = (int)MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(title.Text) | string.IsNullOrEmpty(costomer.Text) | string.IsNullOrEmpty(finishdata.Text) | string.IsNullOrEmpty(Worker.Text) | string.IsNullOrEmpty(Other.Text) | string.IsNullOrEmpty(TechWork.Text) | Materials.Items.Count == 0)
            {
                int num1 = (int)MessageBox.Show("Не введены все значения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (finishdata.SelectedDate.Value < DateTime.Now)
            {
                int num2 = (int)MessageBox.Show("Нельзя добавлять заказ прошедшим числом", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                string command2 = "12|" + Cripts.Encrypt(parts[0]) + " " + Cripts.Encrypt(parts[1]) + " " + Cripts.Encrypt(parts[2]); ;
                DataTable dataTable1 = client.SendSqlCommandButton_Click(command2);

                string command1 = "6|" + title.Text + "|" + int.Parse(dataTable1.Rows[0].Field<string>(0)) + "|" + int128 + Int128.Parse(Worker.Text) + Int128.Parse(TechWork.Text) + Int128.Parse(Other.Text)+ "|" + finishdata.SelectedDate.Value.ToString("yyyy-MM-dd") + "|" + Worker.Text + "|" + TechWork.Text + "|" + Other.Text+ "|"+index.ToString();
                client.SendSqlCommandButton_Click(command1);

                string command8 = "25|" + index.ToString();
                client.SendSqlCommandButton_Click(command8);
                for (int index = 0; index < Materials.Items.Count; ++index)
                {
                    DataGridRow dataGridRow = (DataGridRow)Materials.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock cellContent3 = Materials.Columns[0].GetCellContent(dataGridRow) as TextBlock;
                    TextBlock cellContent4 = Materials.Columns[1].GetCellContent(dataGridRow) as TextBlock;
                    string command = "11|" + cellContent3.Text;
                    DataTable dataTable3 =client.SendSqlCommandButton_Click(command);
                    string command5 = "18|" + dataTable3.Rows[0].Field<string>(0)+ "|" +index.ToString()+ "|" + cellContent4.Text;
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
