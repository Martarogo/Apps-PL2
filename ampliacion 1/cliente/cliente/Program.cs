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
        private readonly String SERVER = "localhost";
        private readonly int SERVERPORT = 23456;
        private int sec = 1, numRec;
        private String strRec;
        private int[] nArray = { 5, 4, 3, 2, 1 };
        private byte[] sent, received = new byte[128];
        BinaryMessageCodec encoding = new BinaryMessageCodec();
        UdpClient client = null;
        Message msg;

        public void Run()
        {
            try
            {
                client = new UdpClient();

                processMessages(nArray);

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


        private void processMessages(int[] nArray) {

            for (int i = 0; i < nArray.Length; i++)
            {
                if (i == nArray.Length - 1)
                {
                    sec = -1;
                    msg = new Message(sec, nArray[i]);
                }
                else
                {
                    msg = new Message(sec, nArray[i]);
                }

                sent = encoding.Encode(msg);

                sendMessage();
            }
        }

        private void sendMessage()
        {
            
            Random rndm = new Random();

            int random = rndm.Next(11);

            if (random > 1)
            {
                client.Send(sent, sent.Length, SERVER, SERVERPORT);
                Console.WriteLine("Mensaje enviado: " + msg.packet);
            }
            
            client.Send(sent, sent.Length, SERVER, SERVERPORT);
            Console.WriteLine("Mensaje enviado: " + msg.packet);

            try
            {
                client.Client.ReceiveTimeout = 1000;

                waitACK();
            }
            catch (Exception e)
            {
                processException(e);
            }
        }

        private void waitACK()
        {
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Recibir información
            received = client.Receive(ref remoteIPEndPoint);

            strRec = encoding.Decode(received);
            numRec = int.Parse(strRec);

            if (sec != numRec)
            {
                sendMessage();
            }
            else if (sec == numRec)
            {
                Console.WriteLine("Numero de secuencia recibido: " + numRec + "\n");
                sec++;
            }
        }


        private void processException(Exception e)
        {
            if (e.InnerException != null)
            {
                if (e.InnerException is SocketException)
                {
                    SocketException se = (SocketException)e.InnerException;
                    if (se.SocketErrorCode == SocketError.TimedOut)
                    {
                        Console.WriteLine("Ha expirado el temporizador, se reenvia el mensaje");
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
                sendMessage();
            }
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
