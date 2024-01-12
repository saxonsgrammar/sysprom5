using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
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

        static void Main(string[] args)
        {
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), 8005);
                string dat = Processor(client);
                Console.WriteLine(dat);

                while (true)
                {
                    dat = Processor(client);
                    Console.WriteLine(dat);
                    string log = Console.ReadLine();
                    client.Send(Encoding.UTF8.GetBytes(log));

                    dat = Processor(client);
                    Console.WriteLine(dat);
                    string pass = Console.ReadLine();
                    client.Send(Encoding.UTF8.GetBytes(pass));

                    dat = Processor(client);
                    Console.WriteLine(dat);

                    if (dat.StartsWith("А")) break;
                }

                int count = 0;

                while (dat != "end")
                {
                    dat = Processor(client);
                    Console.WriteLine(dat);
                    string mess = Console.ReadLine();
                    client.Send(Encoding.UTF8.GetBytes(mess));
                    count++;

                    if (count < 10)
                    {
                        dat = Processor(client);
                        Console.WriteLine($"Сообщение сервера: {dat}");
                    }
                    else
                    {
                        dat = Processor(client);
                        break;
                    }
                }
            }
        }
    }
}