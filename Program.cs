using System.Data;
using System.Net.Sockets;
using System.Net;
using System.Text;
using MySql.Data.MySqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Net.Mail;

class Program
{
    static void Main(string[] args)
    {
        MultiClientServer server = new MultiClientServer(8888);
    }

}
class MultiClientServer
{
    MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=Roman1234;database=usersdb");
    public void openConectiuon()
    {


        if (connection.State != ConnectionState.Closed)
            return;
        connection.Open();


    }
    private void SendEmail(string body, string login)
    {
        SmtpClient client = new SmtpClient();
        client.Credentials = new NetworkCredential("roma.lazarenko2024@mail.ru", "kp2sv5ZJ2n1qASzFMvvv");
        client.Host = "smtp.mail.ru";
        client.Port = 587;
        client.EnableSsl = true;
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("roma.lazarenko2024@mail.ru");
        mail.To.Add(new MailAddress(login));
        mail.Subject = "Код для входа";
        mail.Body = body;
        client.Send(mail);
        
    }

    private string GenerateRandomCode()
    {
        // Создаем экземпляр Random для генерации случайных чисел
        Random random = new Random();

        // Строка с символами, из которых будет формироваться код
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        // Создаем объект StringBuilder для добавления символов в код
        StringBuilder code = new StringBuilder();

        // Генерируем 6 случайных символов
        for (int i = 0; i < 8; i++)
        {
            code.Append(chars[random.Next(chars.Length)]);
        }
        
        return code.ToString();
    }
    public MySqlConnection getConnection() => connection;
    private TcpListener _server;
    private bool _isRunning;

    public MultiClientServer(int port)
    {
        _server = new TcpListener(IPAddress.Any, port);
        _server.Start();

        _isRunning = true;
        Console.WriteLine("Сервер запущен и ожидает подключений...");

        while (_isRunning)
        {
            TcpClient client = _server.AcceptTcpClient();
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
            clientThread.Start(client);
        }
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        openConectiuon();

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] parts = message.Split('|'); // Разделение строки
                string sqlCommandText = "";
                DataTable dataTable = new DataTable();
                MySqlCommand command = new MySqlCommand(); 
                MySqlDataAdapter adapter = new MySqlDataAdapter();

                if (parts.Length > 0 && int.TryParse(parts[0], out int number))
                {
                    switch (number)
                    {
                        case 1:
                            sqlCommandText = "SELECT * FROM usersdb.users WHERE Login=@login AND Password=@password";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@login", parts[1]);
                            command.Parameters.AddWithValue("@password", parts[2]);
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 2:
                            sqlCommandText = "insert into usersdb.users(Login, Password) values (@login, @password)";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@login", parts[1]);
                            command.Parameters.AddWithValue("@password", parts[2]);
                            command.ExecuteNonQuery();
                            break;
                        case 3:
                            sqlCommandText = "insert into usersdb.materials(MatName, MatCost) values (@name, @price)";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@name", parts[1]);
                            command.Parameters.AddWithValue("@price", parts[2]);
                            command.ExecuteNonQuery();
                            break;
                        case 4:
                            sqlCommandText = "UPDATE usersdb.materials SET MatName =@name, MatCost=@price WHERE idMaterials=@id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@name", parts[1]);
                            command.Parameters.AddWithValue("@price", parts[2]);
                            command.Parameters.AddWithValue("@id", parts[3]);
                            command.ExecuteNonQuery();
                            break;
                        case 5:
                            sqlCommandText = "SELECT MatCost FROM usersdb.materials WHERE MatName=@name";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@name", parts[1]);
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 6:
                            sqlCommandText = "UPDATE usersdb.orders SET title = @title, customer=@customer, price=@price,begindata=@begin, finishdata=@finish, Worker=@worker, TechWork=@tech, Other=@other WHERE idorders=@id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@title", parts[1]);
                            command.Parameters.AddWithValue("@customer", parts[2]);
                            command.Parameters.AddWithValue("@price", parts[3]);
                            command.Parameters.AddWithValue("@finish", parts[4]);
                            command.Parameters.AddWithValue("@worker", parts[5]);
                            command.Parameters.AddWithValue("@tech", parts[6]);
                            command.Parameters.AddWithValue("@other", parts[7]);
                            command.Parameters.AddWithValue("@id", parts[8]);
                            command.ExecuteNonQuery();

                            break;
                        case 7:
                            sqlCommandText = "insert into usersdb.orders(title, customer, price,begindata ,finishdata, Worker, TechWork, Other, readiness) values (@title,@customer,@price,@begin,@finish,@worker,@tech,@other,0)";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@title", parts[1]);
                            command.Parameters.AddWithValue("@customer", parts[2]);
                            command.Parameters.AddWithValue("@price", parts[3]);
                            command.Parameters.AddWithValue("@begin", parts[4]);
                            command.Parameters.AddWithValue("@finish", parts[5]);
                            command.Parameters.AddWithValue("@worker", parts[6]);
                            command.Parameters.AddWithValue("@tech", parts[7]);
                            command.Parameters.AddWithValue("@other", parts[8]);
                            command.ExecuteNonQuery();
                            break;
                        case 8:
                            sqlCommandText = "SELECT idorders FROM usersdb.orders WHERE usersdb.orders.title = @title AND usersdb.orders.customer=@customer AND usersdb.orders.price=@price AND usersdb.orders.begindata=@begin AND usersdb.orders.finishdata=@finishdata AND usersdb.orders.Worker=@worker AND usersdb.orders.TechWork=@tech AND usersdb.orders.Other=@other";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@title", parts[1]);
                            command.Parameters.AddWithValue("@customer", parts[2]);
                            command.Parameters.AddWithValue("@price", parts[3]);
                            command.Parameters.AddWithValue("@begin", parts[4]);
                            command.Parameters.AddWithValue("@finishdata", parts[5]);
                            command.Parameters.AddWithValue("@worker", parts[6]);
                            command.Parameters.AddWithValue("@tech", parts[7]);
                            command.Parameters.AddWithValue("@other", parts[8]);
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 9:
                            sqlCommandText = "SELECT title, CONCAT(usersdb.clients.Name, ' ', usersdb.clients.Surname, ' ', usersdb.clients.Patronimic) AS customer, finishdata, Worker, TechWork, Other FROM usersdb.clients, usersdb.orders WHERE usersdb.clients.idClients = usersdb.orders.customer AND idorders = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 10:
                            sqlCommandText = "SELECT usersdb.materials.MatName, Number FROM usersdb.materials, usersdb.matord WHERE usersdb.materials.idMaterials = usersdb.matord.idMat AND idorders = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 11:
                            sqlCommandText = "SELECT idMaterials FROM usersdb.materials WHERE MatName = @customer";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@customer", parts[1]);
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 12:
                            sqlCommandText = "SELECT idClients FROM usersdb.clients WHERE CONCAT(usersdb.clients.Name,' ', usersdb.clients.Surname,' ', usersdb.clients.Patronimic) = @customer";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@customer", parts[1]);
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);

                            break;
                        case 13:
                            sqlCommandText = "UPDATE usersdb.clients SET Name=@name, Surname=@famili,Patronimic=@patronimic, Number=@number,Note=@note WHERE idClients=@id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@name", parts[1]);
                            command.Parameters.AddWithValue("@famili", parts[2]);
                            command.Parameters.AddWithValue("@patronimic", parts[3]);
                            command.Parameters.AddWithValue("@number", parts[4]);
                            command.Parameters.AddWithValue("@note", parts[5]);
                            command.Parameters.AddWithValue("@id", parts[6]);
                            command.ExecuteNonQuery();
                            break;
                        case 14:
                            sqlCommandText = "insert into usersdb.clients(  Name, Surname,Patronimic, Number,Note) values (@name, @famili,@patronimic,@numver,@note)";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@name", parts[1]);
                            command.Parameters.AddWithValue("@famili", parts[2]);
                            command.Parameters.AddWithValue("@patronimic", parts[3]);
                            command.Parameters.AddWithValue("@numver", parts[4]);
                            command.Parameters.AddWithValue("@note", parts[5]);
                            command.ExecuteNonQuery();
                            break;
                        case 15:
                            sqlCommandText = "SELECT * FROM usersdb.clients WHERE CONCAT ( Name, Surname,Patronimic, Number, Note) LIKE @search";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@search", "%" + parts[1] + "%");
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 16:
                            sqlCommandText = "SELECT usersdb.orders.idorders, usersdb.orders.title, CONCAT(usersdb.clients.Name,' ', usersdb.clients.Surname,' ', usersdb.clients.Patronimic) AS customer, usersdb.orders.price, usersdb.orders.begindata, usersdb.orders.finishdata FROM usersdb.orders, usersdb.clients WHERE CONCAT ( usersdb.orders.title, CONCAT(usersdb.clients.Name,' ', usersdb.clients.Surname,' ', usersdb.clients.Patronimic), usersdb.orders.price, usersdb.orders.begindata, usersdb.orders.finishdata) LIKE @search";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@search", "%" + parts[1] + "%");
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 17:
                            sqlCommandText = "SELECT * FROM usersdb.materials WHERE CONCAT ( MatName, MatCost) LIKE @search";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@search", "%" + parts[1] + "%");
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 18:
                            sqlCommandText = "insert into usersdb.matord(idMat, idorders, Number) values (@idMat,@idOrd,@number)";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@idMat", parts[1]);
                            command.Parameters.AddWithValue("@idOrd", parts[2]);
                            command.Parameters.AddWithValue("@number", parts[3]);
                            command.ExecuteNonQuery();
                            break;
                        case 19:
                            sqlCommandText = "SELECT * FROM usersdb.materials";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 20:
                            sqlCommandText = "SELECT * FROM usersdb.clients";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 21:
                            sqlCommandText = "DELETE FROM usersdb.materials WHERE idMaterials = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            command.ExecuteNonQuery();
                            break;
                        case 22:
                            sqlCommandText = "UPDATE usersdb.orders SET readiness = 0 WHERE idorders = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            command.ExecuteNonQuery();
                            break;
                        case 23:
                            sqlCommandText = "UPDATE usersdb.orders SET readiness = 1 WHERE idorders = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            command.ExecuteNonQuery();
                            break;
                        case 24:
                            sqlCommandText = "DELETE FROM usersdb.orders WHERE idorders = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            command.ExecuteNonQuery();
                            break;
                        case 25:
                            sqlCommandText = "DELETE FROM usersdb.matord WHERE idorders = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            command.ExecuteNonQuery();
                            break;
                        case 26:
                            sqlCommandText = "DELETE FROM usersdb.clients WHERE idClients = @id";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            command.Parameters.AddWithValue("@id", parts[1]);
                            command.ExecuteNonQuery();
                            break;
                        case 27:
                            sqlCommandText = "SELECT * FROM usersdb.materials";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 28:
                            sqlCommandText = "SELECT * FROM usersdb.clients";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 29:
                            sqlCommandText = "SELECT idorders, title, price, CONCAT(usersdb.clients.Name,' ', usersdb.clients.Surname,' ', usersdb.clients.Patronimic) AS customer, begindata, finishdata, readiness FROM usersdb.clients, usersdb.orders WHERE usersdb.clients.idClients=usersdb.orders.customer";
                            command.CommandText = sqlCommandText;
                            command.Connection = getConnection();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                            break;
                        case 30:
                            int atIndex = parts[1].IndexOf('@');
                            string code1 = "";
                            DataColumn newColumn = new DataColumn("Code", typeof(string));
                            if (atIndex != -1 && atIndex < parts[1].Length - 1)
                            {
                                string domen = parts[1].Substring(atIndex + 1);
                                if (domen == "mail.ru")
                                {
                                    code1 = GenerateRandomCode();

                                    string emailBody = "Введите данный код для регистрации в приложении: " + code1;
                                    try
                                    {
                                        SendEmail(emailBody, parts[1]);
                                    }
                                    catch (Exception ex)
                                    {
                                        code1 = "Не верно введена почта";
                                        dataTable.Columns.Add(newColumn);

                                        dataTable.Rows.Add(0);

                                        dataTable.Rows[0]["Code"] = code1;

                                       
                                        break;
                                    }
                                }
                                else
                                {
                                    code1 = "Введите корпоративную почту";
                                }
                            }
                            else
                            {
                                code1 = "Введите корпоративную почту";
                            }
                            

                            // Добавляем новую колонку в datatable
                            dataTable.Columns.Add(newColumn);

                            dataTable.Rows.Add(0);

                            dataTable.Rows[0]["Code"] = code1;
                            break;
                    }
                }

                

                StringBuilder responseBuilder = new StringBuilder();

                // Добавление названий столбцов в начало строки
                string[] columnNames = dataTable.Columns.Cast<DataColumn>()
                                             .Select(column => column.ColumnName)
                                             .ToArray();
                responseBuilder.AppendLine(string.Join(",", columnNames));

                // Добавление данных строк
                foreach (DataRow row in dataTable.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    responseBuilder.AppendLine(string.Join(",", fields));
                }

                byte[] buffer1 = Encoding.UTF8.GetBytes(responseBuilder.ToString());

                stream.Write(buffer1, 0, buffer1.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
        }
        finally
        {
            client.Close();
        }
    }

}
