using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace _4Client
{

    public partial class Form1 : Form
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера

        Thread thread;

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
        //создаем сокет
        private Socket _ServS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Form1()
        {
            InitializeComponent();
            // Connect();

            CheckForIllegalCrossThreadCalls = false;
            try
            {
                // подключаемся к удаленному хосту
                _ServS.Connect(ipPoint);
                thread = new Thread(func);
                thread.Start(_ServS);//передача параметра в поток
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            void func(object soc)//Функция потока, передаем параметр всегда object
            {
                Socket handler = soc as Socket;
                try
                {
                    //TextBox text = new TextBox();
                    //text.Location = new System.Drawing.Point(19, 12);
                    //text.Size = new System.Drawing.Size(633, 300);
                    //Controls.Add(text);

                    // готовимся  получать  сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов за 1 раз
                    byte[] data = new byte[255]; // буфер для получаемых данных

                    while (true)
                    {
                        builder.Clear();

                        do
                        {
                            bytes = handler.Receive(data);  // получаем сообщение
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (handler.Available > 0);

                        // chat.Text = builder.ToString();
                        //  SetTextSafe(builder.ToString());
                        listBox1.Items.Add(builder.ToString());
                    }

                    // закрываем сокет

                }
                catch (Exception ex)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Console.WriteLine(ex.Message);
                }
            }
        }
        //private void Connect()
        //{
        //    IPEndPoint Представляет сетевую конечную точку в виде IP-адреса и номер порта.
        //     /*IPEndPointКласс содержит сведения об узле и локальном или удаленном порте, необходимые приложению для подключения к службе на узле.
        //     Объединяя IP-адрес узла и номер порта службы, IPEndPoint класс формирует точку подключения к службе. */

        //    создаем конечную точку
        //    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
        //    создаем сокет(будем получать по протоколу tcp) Сокет помогает принимать и отправлять сообщения
        //    _ServS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    _ClientS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //    подключаемся к удаленному хосту
        //   _ServS.Connect(ipPoint);
        //    _ClientS.Connect(ipPoint);
        //}
        private void button1_Click(object sender, EventArgs e)
        {
            Send();
        }
        private void Send()
        {
            string message = textBox1.Text;
            byte[] data = Encoding.Unicode.GetBytes(message); // кодирует сообщение в байты
            _ServS.Send(data); // send передаект объект в сокет
        }

    }
}
