using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vocabulary;
using System.Net.Sockets;
using System.Net;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Message msg = new Message(1, 5);
            BinaryMessageCodec bmc = new BinaryMessageCodec();

            Console.WriteLine("Prueba: " + msg.num + " " + msg.packet);

            byte[] bSent = bmc.Encode(msg);

            UdpClient client = new UdpClient();

            client.Send(bSent, bSent.Length, "localhost", 23456);
            
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] bRec = client.Receive(ref remoteIPEndPoint);

            String strRec = bmc.Decode(bRec);

            String[] separador = { " | " };

            String[] mRec = strRec.Split(separador, StringSplitOptions.RemoveEmptyEntries);

            int secRec = int.Parse(mRec[0]);
            int nRec = int.Parse(mRec[1]);

            Assert.Equals(secRec, 1);
            Assert.Equals(nRec, 5);
        }
    }
}
