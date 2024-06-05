using StackExchange.Redis;
using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
namespace Order
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dtclient = new DataTable();
        DataTable dtorder = new DataTable();
        DataTable dtmaterials = new DataTable();
        private void ubdate2()
        {
            client client = new client();
            string command = "29|";
            
            DataTable dt = client.SendSqlCommandButton_Click(command);
            
            foreach (DataRow row in dt.Rows)
            {
                string BeginData = row["begindata"].ToString();
                BeginData = Regex.Replace(BeginData, @"(\d{2})/(\d{2})/(\d{4})", "$2.$1.$3");
                row["begindata"] = BeginData;
            }
            foreach (DataRow row in dt.Rows)
            {
                string finishdata = row["finishdata"].ToString();

                // Изменяем формат даты
                finishdata = Regex.Replace(finishdata, @"(\d{2})/(\d{2})/(\d{4})", "$2.$1.$3");

                // Помещаем измененное значение обратно в таблицу
                row["finishdata"] = finishdata;
            }

            foreach (DataRow row in dt.Rows)
            {
                if (row["idorders"] == DBNull.Value) break;
                // Преобразование типа данных столбца idordes из string в int
                row["idorders"] = Convert.ToInt32(row["idorders"]);
                string[] parts = (row["customer"].ToString()).Split(' ');
                row["customer"] = Cripts.Decrypt(parts[0])+" "+ Cripts.Decrypt(parts[1])+" "+ Cripts.Decrypt(parts[2]);
                // Преобразование типа данных столбца price из string в int
                row["price"] = Convert.ToInt64(row["price"]);

                // Преобразование типа данных столбца begindata из string в DateTime
                row["begindata"] = (DateTime.Parse(row["begindata"].ToString())).Date;

                // Преобразование типа данных столбца finishdata из string в DateTime
                row["finishdata"] = (DateTime.Parse(row["finishdata"].ToString())).Date;

                // Преобразование типа данных столбца readliness из string в sbyte
                row["readiness"] = (row["readiness"].ToString() == "1");
            }
            dtorder = dt;
            // Установка источника данных для элемента order
            order.ItemsSource = dt.DefaultView;

        }
        private void ubdate1()
        {
            client client = new client();
            string command = "28|";
            DataTable dt =client.SendSqlCommandButton_Click(command);
           
            foreach (DataRow row in dt.Rows)
            {
                if (row["idClients"] == DBNull.Value) break;
                // Преобразование типа данных столбца idordes из string в int
                row["Name"] = Cripts.Decrypt(row["Name"].ToString());
                row["Surname"] = Cripts.Decrypt(row["Surname"].ToString());
                row["Patronimic"] = Cripts.Decrypt(row["Patronimic"].ToString());
                row["Number"] = Cripts.Decrypt(row["Number"].ToString());
                row["idClients"] = Convert.ToInt32(row["idClients"]);
            }
            dtclient = dt;
                clients.ItemsSource = dt.DefaultView;
            
        }
        private void ubdate3()
        {
            client client = new client();
            string command = "27|";
            
            DataTable dataTable1 =client.SendSqlCommandButton_Click(command);
           
            foreach (DataRow row in dataTable1.Rows)
            {
                if (row["idMaterials"] == DBNull.Value) break;
                // Преобразование типа данных столбца idordes из string в int
                row["idMaterials"] = Convert.ToInt32(row["idMaterials"]);

                // Преобразование типа данных столбца price из string в int
                row["MatCost"] = Convert.ToInt32(row["MatCost"]);
            }

            dtmaterials = dataTable1;
                Materials.ItemsSource = (IEnumerable)dataTable1.DefaultView;

        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.OriginalSource as CheckBox;
            if (order.SelectedItem is DataRowView row)
            {
                if (checkBox != null)
                {
                    int id = int.Parse(row[0].ToString());

                    client client = new client();
                    string command = "23|" + id.ToString();
                    client.SendSqlCommandButton_Click(command);
                }
            }
        }

        // Обработчик события Unchecked
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.OriginalSource as CheckBox;
            if (order.SelectedItem is DataRowView row)
            {
                if (checkBox != null)
                {
                    int id = int.Parse(row[0].ToString());
                    client client = new client();
                    string command = "22|" + id.ToString();
                    client.SendSqlCommandButton_Click(command);
                    
                }
            }
        }
            public MainWindow()
            {
            
            InitializeComponent();
            client client = new client();

            string command = "29|";

            DataTable dt = client.SendSqlCommandButton_Click(command);

            foreach (DataRow row in dt.Rows)
            {
                string finishdata = row["finishdata"].ToString();

                // Изменяем формат даты
                finishdata = Regex.Replace(finishdata, @"(\d{2})/(\d{2})/(\d{4})", "$2.$1.$3");
                
               

                // Помещаем измененное значение обратно в таблицу
                row["finishdata"] = finishdata;
            }

            foreach (DataRow row in dt.Rows)
            {
                if (row["idorders"] == DBNull.Value) break;
                // Преобразование типа данных столбца idordes из string в int
                row["idorders"] = Convert.ToInt32(row["idorders"]);

                var filteredRows = dt.AsEnumerable()
                    .Where(row => row.Field<string>("readiness") == "0" && // Фильтрация readiness по строковому значению "0"
                                  (DateTime.Parse(row.Field<string>("finishdata")) - DateTime.Today).TotalDays < 7 && // Преобразование finishdata из строки в DateTime
                                  (DateTime.Parse(row.Field<string>("finishdata")) - DateTime.Today).TotalDays > 0) // Преобразование finishdata из строки в DateTime
                    .Select(row => new { Id = row.Field<string>("idorders"), Title = row.Field<string>("title") });

                // Показать предупреждение для каждой строки, удовлетворяющей условиям фильтрации
                foreach (var raw in filteredRows)
                {
                    if (MessageBox.Show("Приближается срок сдачи заказа\n" + raw.Title + "\nОтметить как завершенный?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        string command1 = "23|" + raw.Id.ToString();
                        client.SendSqlCommandButton_Click(command1);
                    }
                }
            }
            // Фильтрация строк для изменения заказа
            var filteredRows1 = dt.AsEnumerable()
                .Where(row => row.Field<string>("readiness") == "0" && // Фильтрация readiness по строковому значению "0"
                              DateTime.Parse(row.Field<string>("finishdata")) < DateTime.Today) // Преобразование finishdata из строки в DateTime
                .Select(row => row.Field<int>("idorders"));

            // Открытие формы изменения заказа для каждого id заказа
            foreach (var id in filteredRows1)
            {
                ChangeOrder changeOrder = new ChangeOrder(id);
                changeOrder.Show();
            }

            order.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(CheckBox_Checked));
            order.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(CheckBox_Unchecked));
            ubdate1();
            ubdate2();
            ubdate3();
        }

        private void AddC_Click(object sender, RoutedEventArgs e)
        {
            NewClients newClients = new NewClients();
            newClients.Show();
        }

        private void UbdateC_Click(object sender, RoutedEventArgs e)
        {
            ubdate1();
        }

        private void ChangeC_Click(object sender, RoutedEventArgs e)
        {

            if (clients.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row[0]);
                string name = (string)row[1];
                string famili = (string)row[2];
                string patronimic = (string)row[3];
                string numver = (string)row[4];
                string note = (string)row[5];
                ChangeClients changeClients = new ChangeClients(id, name, famili, patronimic, numver, note);
                changeClients.Show();
                ubdate1();
            }
            else
            {
                MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

            }


        }

        private void SearchC_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchC.Text.ToLower(); // Получаем текст запроса и приводим его к нижнему регистру

            if (!string.IsNullOrEmpty(searchText))
            {
                // Применяем фильтрацию строк в DataTable
                dtclient.DefaultView.RowFilter = $"Name LIKE '%{searchText}%' OR Surname LIKE '%{searchText}%' OR Patronimic LIKE '%{searchText}%' OR Number LIKE '%{searchText}%' OR Note LIKE '%{searchText}%'";
            }
            else
            {
                // Если строка запроса пустая, отображаем все строки
                dtclient.DefaultView.RowFilter = "";
            }
        }

        private void DeleteC_Click(object sender, RoutedEventArgs e)
        {
            if (clients.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Вы действительно хотите удалить данные?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
                int id = Convert.ToInt32(row[0]);

               

                client client = new client();
                string command = "26|" + id.ToString() ;
                client.SendSqlCommandButton_Click(command);
                ubdate1();
            }
            else
            {
                MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        private void AddO_Click(object sender, RoutedEventArgs e)
        {
            NewOrder newOrder = new NewOrder();
            newOrder.Show();
            ubdate2();
        }

        private void UbdateO_Click(object sender, RoutedEventArgs e)
        {
            ubdate2();
        }

        private void SearchO_TextChanged(object sender, TextChangedEventArgs e)
        {

            string searchText = SearchO.Text.ToLower(); // Получаем текст запроса и приводим его к нижнему регистру

            if (!string.IsNullOrEmpty(searchText))
            {
                // Применяем фильтрацию строк в DataTable
                dtorder.DefaultView.RowFilter = $"title LIKE '%{searchText}%' OR customer LIKE '%{searchText}%' OR price LIKE '%{searchText}%' OR CONVERT(begindata, 'System.String') LIKE '%{searchText}%' OR CONVERT(finishdata, 'System.String') LIKE '%{searchText}%'";
            }
            else
            {
                // Если строка запроса пустая, отображаем все строки
                dtorder.DefaultView.RowFilter = "";
            }
        }

        private void DeleteO_Click(object sender, RoutedEventArgs e)
        {
            if (order.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Вы действительно хотите удалить данные?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
                int id = Convert.ToInt32(row[0]);

                client client = new client();
                string command = "24|" + id.ToString();
                client.SendSqlCommandButton_Click(command);
                ubdate2();
            }
            else
            {
                MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        private void ChangeO_Click(object sender, RoutedEventArgs e)
        {
            if (order.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row[0]);
                ChangeOrder changeOrder = new ChangeOrder(id);
                changeOrder.Show();
                ubdate2();
            }
            else
            {
                MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }


        private void UbdateM_Click(object sender, RoutedEventArgs e) => ubdate3();

        private void SearchM_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchM.Text.ToLower(); // Получаем текст запроса и приводим его к нижнему регистру

            if (!string.IsNullOrEmpty(searchText))
            {
                // Применяем фильтрацию строк в DataTable
                dtmaterials.DefaultView.RowFilter = $"MatName LIKE '%{searchText}%' OR MatCost LIKE '%{searchText}%'";
            }
            else
            {
                // Если строка запроса пустая, отображаем все строки
                dtmaterials.DefaultView.RowFilter = "";
            }

        }

        private void AddM_Click(object sender, RoutedEventArgs e) => new NewMaterials().Show();

        private void DeleteM_Click(object sender, RoutedEventArgs e)
        {
            client client = new client();
            if (Materials.SelectedItem is DataRowView selectedItem)
            {
                if (MessageBox.Show("Вы действительно хотите удалить данные?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
                    return;
                int num = Convert.ToInt32(selectedItem[0]);
                string command = "21|" + num.ToString();
                client.SendSqlCommandButton_Click(command);
     
                ubdate2();
            }
            else
            {
                int num1 = (int)MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ChangeM_Click(object sender, RoutedEventArgs e)
        {
            if (Materials.SelectedItem is DataRowView selectedItem)
            {
                new ChangeMat(Convert.ToInt32(selectedItem[0]), (string)selectedItem[1], (Convert.ToInt32(selectedItem[2])).ToString()).Show();
            }
            else
            {
                int num = (int)MessageBox.Show("Выберете строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

    }
}