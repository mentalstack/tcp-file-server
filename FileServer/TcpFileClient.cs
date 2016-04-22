namespace FileServer
{
    using System.Net;
    using System.Net.Sockets;

    using System.IO;
    using System.IO.Compression;

    using System;

    /// <summary>
    /// Tcp file client.
    /// </summary>
    public class TcpFileClient
    {
        #region Defines

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public Guid Id { get; set; }

        #endregion

        #region Private Properties : Network

        /// <summary>
        /// Gets or sets client.
        /// </summary>
        private TcpClient Client { get; set; }

        /// <summary>
        /// Gets or sets binary writer.
        /// </summary>
        private BinaryWriter Writer { get; set; }

        /// <summary>
        /// Gets or sets binary reader.
        /// </summary>
        private BinaryReader Reader { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends Seek message.
        /// </summary>
        public long Seek(long offset, SeekOrigin origin)
        {
            byte[] serialized;

            var message = new Seek { Offset = offset, Origin = origin };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }

            long result = Reader.ReadInt64();
            {
                return result;
            }
        }

        /// <summary>
        /// Sends Open message.
        /// </summary>
        public void Open(string name, FileMode mode)
        {
            byte[] serialized;

            var message = new Open { Name = name, FileMode = mode};
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }
        }

        /// <summary>
        /// Sends Read message.
        /// </summary>
        public int Read(byte[] buffer, int offset, int count)
        {
            byte[] serialized;

            var message = new Read { Offset = offset, Count = count };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }

            int result = Reader.ReadInt32();
            {
                buffer = Reader.ReadBytes(count);
            }

            return result;
        }

        /// <summary>
        /// Sends Write message.
        /// </summary>
        public void Write(byte[] buffer, int offset, int count)
        {
            byte[] serialized;

            var message = new Write { Buffer = buffer, Offset = offset, Count = count };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }
        }

        /// <summary>
        /// Sends Flush message.
        /// </summary>
        public void Flush()
        {
            byte[] serialized;

            var message = new Flush { };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }
        }

        /// <summary>
        /// Sends SetPosition message.
        /// </summary>
        public void SetPosition(long value)
        {
            byte[] serialized;

            var message = new SetPosition { Position = value };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }
        }

        /// <summary>
        /// Sends GetPosition message.
        /// </summary>
        public long GetPosition()
        {
            byte[] serialized;

            var message = new GetPosition { };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }

            long result = Reader.ReadInt64();
            {
                return result;
            }
        }

        /// <summary>
        /// Sends SetLength message.
        /// </summary>
        public void SetLength(long value)
        {
            byte[] serialized;

            var message = new SetLength { Length = value };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }
        }

        /// <summary>
        /// Sends GetLength message.
        /// </summary>
        public long GetLength()
        {
            byte[] serialized;

            var message = new GetLength { };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }

            long result = Reader.ReadInt64();
            {
                return result;
            }
        }

        /// <summary>
        /// Sends Close message.
        /// </summary>
        public void Close()
        {
            byte[] serialized;

            var message = new Close { };
            {
                serialized = message.Serialize(); Writer.Write(serialized.Length); Writer.Write(serialized);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Connects to media server.
        /// </summary>
        internal TcpFileClient Connect(string hostname, int port)
        {
            Client = new TcpClient(hostname, port);
            {
                Writer = new BinaryWriter(Client.GetStream()); // define writer
                Reader = new BinaryReader(Client.GetStream()); // define reader
            }

            return this;
        }

        #endregion
    }
}