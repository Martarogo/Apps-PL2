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
        private byte[] bRec = new byte[128], bACK;
        private String strRec;
        private String[] separador = { " | " }, mRec;
        private int nRec, secRec, sec = 1;

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

        private void waitMessage()
        {
            // Recibir información
            bRec = client.Receive(ref remoteIPEndPoint);

            strRec = encoding.Decode(bRec);

            mRec = strRec.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            
            secRec = int.Parse(mRec[0]);
            nRec = int.Parse(mRec[1]);

            if (sec != secRec)
            {
                if (secRec == -1)
                {
                    Console.WriteLine("Mensaje recibido: " + strRec);

                    sec = -1;

                    sendACK();
                }
                else if (secRec != -1)
                {
                    Console.WriteLine("El numero de secuencia recibido no conicide. Se descarta el paquete");
                    waitMessage();
                }
                
            }
            else if (sec == secRec)
            {
                Console.WriteLine("Mensaje recibido: " + strRec);

                sendACK();
            }
        }

        private void sendACK()
        {
            Message ack = new Message(sec);

            bACK = encoding.Encode(ack);

            client.Send(bACK, bACK.Length, remoteIPEndPoint);

            Console.WriteLine("Numero de secuencia enviado: " + sec);

            Console.WriteLine("\nFin del envio de datos");
            sec++;
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


