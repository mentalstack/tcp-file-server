namespace FileServer
{
    using ProtoBuf;

    using System.Net;
    using System.Net.Sockets;

    using System;
    using System.Linq;
    using System.IO;

    /// <summary>
    /// Media message.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(100, typeof(Open))]
    [ProtoInclude(101, typeof(Seek))]
    [ProtoInclude(102, typeof(Read))]
    [ProtoInclude(103, typeof(Write))]
    [ProtoInclude(104, typeof(Flush))]
    [ProtoInclude(105, typeof(SetPosition))]
    [ProtoInclude(106, typeof(GetPosition))]
    [ProtoInclude(107, typeof(SetLength))]
    [ProtoInclude(108, typeof(GetLength))]
    [ProtoInclude(109, typeof(Close))]
    public abstract class Message
    {
        #region Public Methods : Serialization

        /// <summary>
        /// Serializes message.
        /// </summary>
        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                /* serialize */ Serializer.Serialize(stream, this); return stream.ToArray();
            }
        }

        /// <summary>
        /// Generic deserializes message.
        /// </summary>
        public static T Deserialize<T>(byte[] buffer) where T : Message
        {
            using (var stream = new MemoryStream(buffer))
            {
                /* generic deserialize for special cases */ return Serializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// Deserializes message.
        /// </summary>
        public static Message Deserialize(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                /* deserialize */ return Serializer.Deserialize<Message>(stream);
            }
        }

        #endregion
    }

    /// <summary>
    /// Open message.
    /// </summary>
    [ProtoContract]
    public class Open : Message
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets file mode.
        /// </summary>
        [ProtoMember(2)]
        public FileMode FileMode { get; set; }

        #endregion
    }

    /// <summary>
    /// Seek message.
    /// </summary>
    [ProtoContract]
    public class Seek : Message
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets offset.
        /// </summary>
        [ProtoMember(2)]
        public long Offset { get; set; }

        /// <summary>
        /// Gets or sets seek origin.
        /// </summary>
        [ProtoMember(3)]
        public SeekOrigin Origin { get; set; }

        /// <summary>
        /// Gets or sets count.
        /// </summary>
        [ProtoMember(4)]
        public int Count { get; set; }

        #endregion
    }

    /// <summary>
    /// Read message.
    /// </summary>
    [ProtoContract]
    public class Read : Message
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets offset.
        /// </summary>
        [ProtoMember(2)]
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets byte buffer.
        /// </summary>
        [ProtoMember(3)]
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Gets or sets count.
        /// </summary>
        [ProtoMember(4)]
        public int Count { get; set; }

        #endregion
    }

    /// <summary>
    /// Write message.
    /// </summary>
    [ProtoContract]
    public class Write : Message
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets offset.
        /// </summary>
        [ProtoMember(2)]
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets byte buffer.
        /// </summary>
        [ProtoMember(3)]
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Gets or sets count.
        /// </summary>
        [ProtoMember(4)]
        public int Count { get; set; }

        #endregion
    }

    /// <summary>
    /// Flush message.
    /// </summary>
    [ProtoContract]
    public class Flush : Message
    {
        #region Defines

        #endregion
    }

    /// <summary>
    /// SetPosition message.
    /// </summary>
    [ProtoContract]
    public class SetPosition : Message
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets position.
        /// </summary>
        [ProtoMember(2)]
        public long Position { get; set; }

        #endregion
    }

    /// <summary>
    /// GetPosition message.
    /// </summary>
    [ProtoContract]
    public class GetPosition : Message
    {
        #region Defines

        #endregion
    }

    /// <summary>
    /// SetLength message.
    /// </summary>
    [ProtoContract]
    public class SetLength : Message
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets length.
        /// </summary>
        [ProtoMember(2)]
        public long Length { get; set; }

        #endregion
    }

    /// <summary>
    /// GetLength message.
    /// </summary>
    [ProtoContract]
    public class GetLength : Message
    {
        #region Defines

        #endregion
    }

    /// <summary>
    /// Close message.
    /// </summary>
    [ProtoContract]
    public class Close : Message
    {
        #region Defines

        #endregion
    }

    /// <summary>
    /// Command handler.
    /// </summary>
    public abstract class CommandHandler
    {
        #region Defines

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets client.
        /// </summary>
        protected TcpClient Client { get; set; }

        /// <summary>
        /// Gets or sets binary writer.
        /// </summary>
        protected BinaryWriter Writer { get; set; }

        /// <summary>
        /// Gets or sets binary reader.
        /// </summary>
        protected BinaryReader Reader { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Does consume.
        /// </summary>
        public abstract void Handle();

        /// <summary>
        /// Does consume async.
        /// </summary>
        public abstract void HandleAsync();

        #endregion

        #region Public Methods : Binding

        /// <summary>
        /// Binds to client.
        /// </summary>
        internal CommandHandler Bind(TcpClient client)
        {
            Client = client; // define client
            {
                Writer = new BinaryWriter(Client.GetStream()); // define writer
                Reader = new BinaryReader(Client.GetStream()); // define reader
            }

            return this;
        }

        #endregion

        #region Constructors

        #endregion
    }

    /// <summary>
    /// Tcp file server.
    /// </summary>
    public class TcpFileServer<T> where T : CommandHandler
    {
        #region Private Fields

        /// <summary>
        /// Default port.
        /// </summary>
        private int _port = 11000;

        /// <summary>
        /// Default ip address.
        /// </summary>
        private IPAddress _ipAddress = IPAddress.Any;

        /// <summary>
        /// Stopped flag.
        /// </summary>
        private bool _stopped = false;

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets listener.
        /// </summary>
        private TcpListener Listener { get; set; }

        #endregion

        #region Private Properties : Defaults

        /// <summary>
        /// Gets or sets port.
        /// </summary>
        private int Port
        {
            get { return _port; } set { _port = value; }
        }

        /// <summary>
        /// Gets or sets ip address.
        /// </summary>
        private IPAddress IpAddress
        {
            get { return _ipAddress; } set { _ipAddress = value; }
        }

        /// <summary>
        /// Gets or sets stopped.
        /// </summary>
        private bool Stopped
        {
            get { return _stopped; } set { _stopped = value; }
        }

        #endregion

        #region Public Methods : Server

        /// <summary>
        /// Starts listen.
        /// </summary>
        public async void Start(params object[] args)
        {
            while (!Stopped) // listen loop
            {
                TcpClient client = null; // define client to handle
                {
                    client = await Listener.AcceptTcpClientAsync().ConfigureAwait(false);
                }

                var handler = ((T)Activator.CreateInstance(typeof(T), args));
                {
                    handler.Bind(client).HandleAsync();
                }
            }
        }

        /// <summary>
        /// Stops listen.
        /// </summary>
        public void Stop()
        {
            /* set flag */ Stopped = true;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TcpFileServer() { }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        public TcpFileServer(IPAddress ipAddress, int port)
        {
            IpAddress = ipAddress; Port = port;
            {
                Listener = new TcpListener(IpAddress, Port); Listener.Start();
            }
        }

        #endregion
    }
}