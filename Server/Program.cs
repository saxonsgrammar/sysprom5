using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static string Processor(Socket client)
        {
            byte[] buffer = new byte[256];
            var data = new StringBuilder();
            do
            {
                int size = client.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (client.Available > 0);

            return data.ToString();
        }

        struct User
        {
            public string log;
            public string password;
        }

        static void Main(string[] args)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005));
                    listener.Listen(3);
                    Console.WriteLine("Сервер запущен. Ожидание подключений....");

                    int count = 0;

                    while (true)
                    {
                        var client = listener.Accept();
                        if (client.Connected) count++; 

                        if (count < 4)
                        {
                            Console.WriteLine($"Клиент {client.RemoteEndPoint} подключился");
                            client.Send(Encoding.UTF8.GetBytes("Подключение к серверу успешно установлено"));
                            Task task = Task.Run(async () => await ProcessClient(client));
                            count--;
                        }
                        else
                        {
                            client.Send(Encoding.UTF8.GetBytes("Превышен лимит кол-ва пользователей. Попробуйте подключиться позже"));
                            count--;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static async Task ProcessClient(Socket client)
        {
            string[] quotes =
            {
                 "Или не берись, или доводи до конца",
                 "Капля долбит камень не силой, но частым падением",
                 "Счастлив тот, кто умело берет под свою защиту то, что любит",
                 "Бесцветные зелёные идеи спят яростно",
                 "Глокая куздра штеко будланула бокра и кудрячит бокрёнка"
            };
            User user = new User();
            string path1 = $"..\\Debug\\1.txt";
            user.log = "Влад";
            user.password = "Граматик";
            List<User> users = new List<User>();
            users.Add(user);
            Random random = new Random();
            string log, pass, dat;

            using (StreamWriter stream = new StreamWriter(path1, false))
            {
                stream.WriteLine($"Клиент {client.RemoteEndPoint} подключился");

                while (true)
                {
                    client.Send(Encoding.UTF8.GetBytes("Введите логин: "));
                    dat = Processor(client);
                    log = dat;

                    client.Send(Encoding.UTF8.GetBytes("\nВведите пароль: "));
                    dat = Processor(client);
                    pass = dat;

                    if (user.log == log && user.password == pass)
                    {
                        client.Send(Encoding.UTF8.GetBytes("Авторизация успешно пройдена!"));
                        Console.WriteLine($"Клиент {client.RemoteEndPoint} успешно прошёл авторизацию");
                        stream.WriteLine($"Клиент {client.RemoteEndPoint} успешно прошёл авторизацию");
                        break;
                    }
                    else
                    {
                        client.Send(Encoding.UTF8.GetBytes("Неверный логин или пароль. Попробуйте ещё раз"));
                    }
                }

                int count = 0;

                while (dat != "end")
                {
                    client.Send(Encoding.UTF8.GetBytes("\nВведите сообщение: "));
                    dat = Processor(client);
                    count++;

                    if (count < 10 && dat != "end")
                    {
                        Console.WriteLine($"Сообщение клиента {client.RemoteEndPoint}: {dat}");
                        stream.WriteLine($"Сообщение клиента {client.RemoteEndPoint}: {dat}");
                        string cit = quotes[random.Next(0, 5)];
                        client.Send(Encoding.UTF8.GetBytes(cit));
                        stream.WriteLine($"Цитата клиента {client.RemoteEndPoint}: {cit}");
                    }
                    else
                    {
                        client.Send(Encoding.UTF8.GetBytes("\nПревышен лимит по сообщениям!"));
                        break;
                    }
                }

                client.Close();
                Console.ReadKey();
            }
        }
    }
}