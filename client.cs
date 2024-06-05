using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Order
{
    internal class client
    {
        private Socket clientSocket;

        public client()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(IPAddress.Parse("217.71.129.139"), 4191);
        }

        public DataTable SendSqlCommandButton_Click(string sqlCommandText)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(sqlCommandText);
            clientSocket.Send(buffer);

            byte[] receiveBuffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(receiveBuffer);
            string responseData = Encoding.UTF8.GetString(receiveBuffer, 0, receivedBytes);

            return ConvertStringToDataTable(responseData);
        }


        private DataTable ConvertStringToDataTable(string data)
        {
            DataTable dataTable = new DataTable();

            // Разделение данных на строки
            string[] rows = data.Split('\n');
            // Разделение первой строки на названия столбцов
            string[] columns = rows[0].Split(',');

            // Добавление столбцов в DataTable
            foreach (string column in columns)
            {
                dataTable.Columns.Add(column.Trim()); // Удаление пробелов в названиях столбцов
            }

            // Добавление строк в DataTable
            for (int i = 1; i < rows.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(rows[i]))
                {
                    string[] values = rows[i].Split(',');

                    DataRow row = dataTable.NewRow();

                    for (int j = 0; j < values.Length; j++)
                    {
                        row[j] = values[j].Trim(); // Удаление пробелов в данных
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }
    }
}

