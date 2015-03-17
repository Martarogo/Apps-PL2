using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vocabulary;

namespace Cliente
{
    class Client
    {
        private static String SERVER = "localhost";
        private static int SERVERPORT = 23456;
        private static int sec = 0, nrec;
        private String srec;
        private int[] array_num = { 5, 4, 3, 2, 1 };
        private byte[] sent, received = new byte[128];
        BinaryMessageCodec encoding = new BinaryMessageCodec();
        UdpClient client = null;
        Message msg;

        public void Run()
        {
            try
            {
                client = new UdpClient();

                msg = new Message(sec, array_num[0]);

                sendMessage(msg);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {
                client.Close();
            }
            Console.ReadKey();
        }


        private void sendMessage(Message m) {
            sent = encoding.Encode(msg);

            client.Send(sent, sent.Length, SERVER, SERVERPORT);

            try
            {
                client.Client.ReceiveTimeout = 1000;

                waitACK();
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException is SocketException)
                    {
                        SocketException se = (SocketException)e.InnerException;
                        if (se.SocketErrorCode == SocketError.TimedOut)
                        {
                            Console.WriteLine("Ha expirado el temporizador, se reenvia el mensaje");
                            sendMessage(msg);
                        }
                        else
                        {
                            Console.WriteLine("Error: " + se.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        private void waitACK()
        {
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Recibir información
            received = client.Receive(ref remoteIPEndPoint);

            srec = encoding.Decode(received);
            nrec = int.Parse(srec);

            if (sec != (nrec / 10))
            {
                //Enviar mensaje
            }
            else if (sec == (nrec / 10))
            {
                sec++;
            }
            Console.WriteLine("Sec: " + sec);
        }
    }

    


    class Program
    {
        static void Main(string[] args)
        {
            const int N = 1;

            Thread[] threads = new Thread[N];
            for (int i = 0; i < N; i++)
            {
                Client client = new Client();
                threads[i] = new Thread(new ThreadStart(client.Run));
                threads[i].Start();
            }

            for (int i = 0; i < N; i++)
            {
                threads[i].Join();
            }

        }
    }
}
