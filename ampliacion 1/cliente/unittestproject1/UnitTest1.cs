using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vocabulary;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            BinaryMessageCodec bmc = new BinaryMessageCodec();
            Message msg = new Message(1, 5);

            Console.WriteLine("Prueba: " + msg.num + " " + msg.packet);

            byte[] enviado = bmc.Encode(msg);

            int recibido = bmc.Decode(enviado);

            Console.WriteLine("Recibido: " + recibido % 10 + " " + recibido);

            Message msg2 = new Message(recibido / 10, recibido % 10);

            Assert.AreEqual(msg, msg2);

            Console.ReadKey();
        }
    }
}
