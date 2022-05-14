using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpServer
{

    class myThread
    {
        Thread thread;
        public myThread(string name, Socket handler) //Конструктор получает имя функции и номер,

        {
            thread = new Thread(this.func);
            thread.Name = name;
            thread.Start(handler);//передача параметра в поток
        }

        void func(object num)//Функция потока, передаем параметр всегда object
        {
            Socket handler = (Socket)num;


            Program.SendAll(thread.Name + " присоединился к чату");

            while (true)
            {
                StringBuilder builder = new StringBuilder(); //StringBuilder изменяемая строка
                int bytes = 0; // количество полученных байтов за 1 раз
                int kol_bytes = 0;//количество полученных байтов
                byte[] data = new byte[255]; // буфер для получаемых данных

                builder.Clear();

                do
                {
                    bytes = handler.Receive(data);  // получаем сообщение
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    kol_bytes += bytes;
                }
                while (handler.Available > 0);


                Program.SendAll(DateTime.Now.ToShortTimeString() + ": " + thread.Name + ": " + builder.ToString() + '\n');
                /*Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                Console.WriteLine(kol_bytes + "bytes\n");
                // отправляем ответ
                string message = "ваше сообщение доставлено";
                data = Encoding.Unicode.GetBytes(message); //Преобразуем сообщение в байты
                handler.Send(data); //пересылаем сообщение в байтах в сокет*/
            }
            // закрываем сокет
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();



        }


    }
    class Program
    {
        static int port = 8005; // порт для приема входящих запросов
        static List<Socket> clients = new List<Socket>();
        static List<string> chat = new List<string>();

        public static void SendAll(String message)
        {
            chat.Add(message + Environment.NewLine);

            for (int i = 0; i < clients.Count; i++)
                clients[i].Send(Encoding.Unicode.GetBytes(message + Environment.NewLine));
        }

        static void Main(string[] args)
        {



            String Host = Dns.GetHostName(); //Возвращает имя узла локального компьютера
            Console.WriteLine("Comp name = " + Host);
            IPAddress[] IPs;
            IPs = Dns.GetHostAddresses(Host); // возвращает айпи адрес для указаного узла
            foreach (IPAddress ip1 in IPs) // выводим все адреса перебирая их
                Console.WriteLine(ip1);

            //IPEndPoint Представляет сетевую конечную точку в виде IP-адреса и номер порта.
            /*IPEndPointКласс содержит сведения об узле и локальном или удаленном порте, необходимые приложению для подключения к службе на узле.
            Объединяя IP-адрес узла и номер порта службы, IPEndPoint класс формирует точку подключения к службе. */

            //получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет сервера
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                
                while (true)
                {
                    Socket handler = listenSocket.Accept();  // сокет для связи с клиентом
                                                             // готовимся  получать  сообщение
                    clients.Add(handler);

                    myThread t1 = new myThread(clients.Count.ToString(), handler);
                    //Console.Read();


                }
            }
            catch (Exception ex) // Если ошибка
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
