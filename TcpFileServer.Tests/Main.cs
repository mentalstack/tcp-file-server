﻿namespace FileServer.Tests
{
    using FileServer;
    using FileServer.Handlers;

    using Microsoft.VisualStudio.TestTools;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.IO;
    using System.Threading;
    using System.Net;

    /// <summary>
    /// Main unit test.
    /// </summary>
    [TestClass]
    public class Main
    {
        #region Public Methods

        /// <summary>
        /// Send file test.
        /// </summary>
        [TestMethod]
        public void SendFile()
        {
            TcpFileServer<DefaultHandler> server;

            server = new TcpFileServer<DefaultHandler>(IPAddress.Any, 11000);
            {
                server.Start("C:\\Temp"); // start server, root directory is "C:\\Temp"
            }

            if (File.Exists("C:\\Temp\\sended1.txt")) { File.Delete("C:\\Temp\\sended1.txt"); }
            if (File.Exists("C:\\Temp\\sended2.txt")) { File.Delete("C:\\Temp\\sended2.txt"); }
            if (File.Exists("C:\\Temp\\sended3.txt")) { File.Delete("C:\\Temp\\sended3.txt"); }

            using (var stream1 = new FileStream("C:\\Temp\\send.txt", FileMode.Open))
            {
                using (var stream2 = new TcpFileStream("127.0.0.1", 11000, "sended1.txt", FileMode.Create))
                {
                    stream1.CopyTo(stream2); stream1.Position = 0; // copy from local file stream to tcp file stream (sended1.txt)
                }
                using (var stream3 = new TcpFileStream("127.0.0.1", 11000, "sended2.txt", FileMode.Create))
                {
                    stream1.CopyTo(stream3); stream1.Position = 0; // copy from local file stream to tcp file stream (sended2.txt)
                }
                using (var stream4 = new TcpFileStream("127.0.0.1", 11000, "sended3.txt", FileMode.Create))
                {
                    stream1.CopyTo(stream4); stream1.Position = 0; // copy from local file stream to tcp file stream (sended3.txt)
                }

                Thread.Sleep(5000);
            }

            if (!File.Exists("C:\\Temp\\sended1.txt")) Assert.Fail("1 not sent ...");
            if (!File.Exists("C:\\Temp\\sended2.txt")) Assert.Fail("2 not sent ...");
            if (!File.Exists("C:\\Temp\\sended3.txt")) Assert.Fail("3 not sent ...");
            {
                server.Stop(); // go away
            }
        }

        #endregion
    }
}