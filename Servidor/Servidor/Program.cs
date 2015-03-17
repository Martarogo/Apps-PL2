using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vocabulary;

namespace Servidor
{
    class Server
    {
        private static int PORT = 23456;
        private byte[] brec = new byte[128], back;
        private String crec;
        private String[] separador = { " | " }, mrec;
        private int nrec, secrec, sec = 0;

        private UdpClient client = null;
        BinaryMessageCodec encoding = new BinaryMessageCodec();
        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
        
        
        public void Run()
        {
            Console.WriteLine("Servidor en ejecución...");

            try
            {
                client = new UdpClient(PORT);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                return;
            }
          
            
            // El servidor se ejecuta infinitamente
            for (; ; )
            {
                try
                {
                    waitMessage();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                }

            }
        }

        public void waitMessage()
        {
            // Recibir información
            brec = client.Receive(ref remoteIPEndPoint);

            crec = encoding.Decode(brec);

            mrec = crec.Split(separador, StringSplitOptions.RemoveEmptyEntries);

            secrec = int.Parse(mrec[0]);
            nrec = int.Parse(mrec[1]);

            if (sec != secrec)
            {
                Console.WriteLine("El numero de secuencia recibido no conicide. Se descarta el paquete");
            }
            else if (sec == secrec)
            {
                Message ack = new Message(sec);

                back = encoding.Encode(ack);

                client.Send(back, back.Length, remoteIPEndPoint);
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Server serv = new Server();
            Thread hilo = new Thread(new ThreadStart(serv.Run));
            hilo.Start();
            hilo.Join();
        }
    }
}


