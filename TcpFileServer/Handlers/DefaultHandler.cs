namespace FileServer.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    using System.Collections;
    using System.Collections.Generic;

    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Default command handler.
    /// </summary>
    public class DefaultHandler : CommandHandler
    {
        #region Defines

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets root.
        /// </summary>
        private string Root { get; set; }

        /// <summary>
        /// Gets or sets file stream.
        /// </summary>
        private FileStream FileStream { get; set; }

        /// <summary>
        /// Gets or sets message handlers.
        /// </summary>
        private Dictionary<Type, Action<Message>> Handlers { get; set; }

        /// <summary>
        /// Gets or sets closed.
        /// </summary>
        private bool Closed { get; set; }

        #endregion

        #region Private Properties : State

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        private string Name { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends Seek message.
        /// </summary>
        private void Seek(Seek message)
        {
            Writer.Write(FileStream.Seek(message.Offset, message.Origin));
        }

        /// <summary>
        /// Sends Open message.
        /// </summary>
        private void Open(Open message)
        {
            Name = message.Name; // store name

            if (!Directory.Exists(Root)) { Directory.CreateDirectory(Root); }
            {
                FileStream = new FileStream(String.Format(@"{0}\{1}", Root, message.Name), message.FileMode);
            }
        }

        /// <summary>
        /// Sends Read message.
        /// </summary>
        private void Read(Read message)
        {
            var buffer = new byte[message.Count];

            Writer.Write(FileStream.Read(buffer, message.Offset, message.Count));
            {
                Writer.Write(buffer);
            }
        }

        /// <summary>
        /// Sends Write message.
        /// </summary>
        private void Write(Write message)
        {
            FileStream.Write(message.Buffer, message.Offset, message.Count);
        }

        /// <summary>
        /// Sends Flush message.
        /// </summary>
        private void Flush(Flush message)
        {
            FileStream.Flush();
        }

        /// <summary>
        /// Sends SetLength message.
        /// </summary>
        private void SetLength(SetLength message)
        {
            /* set length */ FileStream.SetLength(message.Length);
        }

        /// <summary>
        /// Sends GetLength message.
        /// </summary>
        private void GetLength(GetLength message)
        {
            Writer.Write(FileStream.Length);
        }

        /// <summary>
        /// Sends SetPosition message.
        /// </summary>
        private void SetPosition(SetPosition message)
        {
            FileStream.Seek(message.Position, SeekOrigin.Begin);
        }

        /// <summary>
        /// Sends GetPosition message.
        /// </summary>
        private void GetPosition(GetPosition message)
        {
            Writer.Write(FileStream.Position);
        }

        /// <summary>
        /// Sends Close message.
        /// </summary>
        private void Close(Close message)
        {
            Closed = true;
        }

        #endregion

        #region Public Methods : Handle

        /// <summary>
        /// Handles client.
        /// </summary>
        public override void Handle()
        {
            try // try to consume
            {
                while (!Closed) // infinity loop
                {
                    int count = Reader.ReadInt32();

                    Message message = Message.Deserialize(Reader.ReadBytes(count));
                    {
                        Handlers[message.GetType()](message);
                    }
                }
            }
            finally // close stream anyway if exceptions
            {
                if (FileStream != null) // check file stream is opened
                {
                    /* close */ FileStream.Close();
                }
            }
        }

        /// <summary>
        /// Handles client async.
        /// </summary>
        public override async void HandleAsync()
        {
            await Task.Factory.StartNew(() => Handle());
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DefaultHandler() { }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        public DefaultHandler(string root)
        {
            Handlers = null;

            Handlers = new Dictionary<Type, Action<Message>>()
            {
                { typeof(Open),        (message) => {        Open(message as Open);        } },
                { typeof(Read),        (message) => {        Read(message as Read);        } },
                { typeof(Write),       (message) => {       Write(message as Write);       } },
                { typeof(Seek),        (message) => {        Seek(message as Seek);        } },
                { typeof(Flush),       (message) => {       Flush(message as Flush);       } },
                { typeof(SetLength),   (message) => {   SetLength(message as SetLength);   } },
                { typeof(GetLength),   (message) => {   GetLength(message as GetLength);   } },
                { typeof(SetPosition), (message) => { SetPosition(message as SetPosition); } },
                { typeof(GetPosition), (message) => { GetPosition(message as GetPosition); } },
                { typeof(Close),       (message) => {       Close(message as Close);       } }
            };

            Root = root;
        }

        #endregion
    }
}