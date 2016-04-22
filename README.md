TcpFileServer
=============

ProtoBuf, TcpFileServer and TcpFileStream for .NET.

TcpFileServer is **async, quick and lightweight** TCP server and file stream wrapper for .NET to send files via internet or intranet using good old approach. It uses ProtoBuf to optimize network bandwidth.

Just look at this code sample to see how it works:

```CSharp
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
		stream1.CopyTo(stream2); Thread.Sleep(2000); stream1.Position = 0;
	}
	using (var stream3 = new TcpFileStream("127.0.0.1", 11000, "sended2.txt", FileMode.Create))
	{
		stream1.CopyTo(stream3); Thread.Sleep(2000); stream1.Position = 0;
	}
	using (var stream4 = new TcpFileStream("127.0.0.1", 11000, "sended3.txt", FileMode.Create))
	{
		stream1.CopyTo(stream4); Thread.Sleep(2000); stream1.Position = 0;
	}
}

if (!File.Exists("C:\\Temp\\sended1.txt")) Assert.Fail("1 not sent ...");
if (!File.Exists("C:\\Temp\\sended2.txt")) Assert.Fail("2 not sent ...");
if (!File.Exists("C:\\Temp\\sended3.txt")) Assert.Fail("3 not sent ...");
{
	server.Stop(); // go away
}
```