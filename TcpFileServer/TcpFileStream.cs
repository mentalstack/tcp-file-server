namespace FileServer
{
    using System;
    using System.IO;

    /// <summary>
    /// Tcp file stream.
    /// </summary>
    public class TcpFileStream : Stream
    {
        #region Defines

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets client.
        /// </summary>
        private TcpFileClient Client { get; set; }

        #endregion

        #region Public Properties : Stream

        /// <summary>
        /// Gets can seek.
        /// </summary>
        public override bool CanSeek { get { return true; } }

        /// <summary>
        /// Gets can write.
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// Gets can read.
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// Gets or sets position.
        /// </summary>
        public override long Position
        {
            get { return Client.GetPosition(); } set { Client.SetPosition(value); }
        }

        /// <summary>
        /// Gets length.
        /// </summary>
        public override long Length
        {
            get { return Client.GetLength(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Flushes stream.
        /// </summary>
        public override void Flush()
        {
            Client.Flush();
        }

        /// <summary>
        /// Seeks from origin.
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return Client.Seek(offset, origin);
        }

        /// <summary>
        /// Reads bytes.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Client.Read(buffer, offset, count);
        }

        /// <summary>
        /// Writes bytes.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Client.Write(buffer, offset, count);
        }

        /// <summary>
        /// Sets specified length.
        /// </summary>
        public override void SetLength(long value)
        {
            Client.SetLength(value);
        }

        /// <summary>
        /// Closes stream.
        /// </summary>
        public override void Close()
        {
            Client.Close();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TcpFileStream()
        {
            Client = new TcpFileClient();
        }

        /// <summary>
        /// Constructor with specified parameters.
        /// </summary>
        public TcpFileStream(string hostname, int port, string name, FileMode mode) : this()
        {
            Client.Connect(hostname, port).Open(name, mode);
        }

        /// <summary>
        /// Constructor with specified parameters.
        /// </summary>
        public TcpFileStream(string hostname, int port) : this()
        {
            Client.Connect(hostname, port);
        }

        #endregion
    }
}