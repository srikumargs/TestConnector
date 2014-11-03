using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;
using Sage.Connector.Logging;

namespace Sage.Connector.Binding
{
    /// <summary>
    /// Class for handling creation of a multi-frame file to upload to blob storage.
    /// </summary>
    public sealed class BlobContentWriter : IDisposable, IEnumerable<string>
    {
        #region Private members

        private Stream _stream;
        private readonly List<string> _files;
        private readonly byte[] _header = new byte[sizeof(int) * 2];
        private readonly string _baseFileName;
        private string _currentFileName = string.Empty;
        private readonly long _fileMaxSize;
        private long _compressedLength;
        private long _uncompressedLength;
        private int _frameCount;
        private int _frameMaxSize;
        private bool _disposed;
        private bool _closed;

        #endregion

        #region Private methods

        /// <summary>
        /// Decompresses an array of byte and returns the uncompressed byte array.
        /// </summary>
        /// <param name="compressedData">The compressed data.</param>
        /// <param name="size">The number of bytes to decompress from the buffer.</param>
        /// <returns>The decompressed array of bytes.</returns>
        public static byte[] Decompress(byte[] compressedData, int size = (-1))
        {
            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = size == (-1) ? new MemoryStream(compressedData) : new MemoryStream(compressedData, 0, size))
                {
                    using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zip.CopyTo(outputStream);
                    }
                }

                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Compresses an array of byte and returns the compressed byte array.
        /// </summary>
        /// <param name="data">The data to compress.</param>
        /// <returns>The compressed array of bytes.</returns>
        public static byte[] Compress(byte[] data)
        {
            if (!CompressBlob())
            {
                return data;
            }

            using (var outputStream = new MemoryStream())
            {
                using (var zip = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zip.Write(data, 0, data.Length);
                }

                return outputStream.ToArray();
            }
        }

        private static bool CompressBlob()
        {
            String dontCompress = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_UNCOMPRESSED_BLOB", EnvironmentVariableTarget.Machine);
            return String.IsNullOrEmpty(dontCompress) || dontCompress != "1";
        }

        /// <summary>
        /// Creates a new file using the base name and current index and opens a stream on the file.
        /// </summary>
        private void InitializeStream()
        {
            _frameCount = 0;
            _frameMaxSize = 0;

            _files.Add(String.Format("{0}.{1:D3}", _baseFileName, _files.Count));
            _currentFileName = _files[_files.Count - 1];
            _stream = File.Open(_currentFileName, FileMode.Create);
            using (LogManager lm = new LogManager())
            {
                lm.WriteInfo(this, "BlobContentWriter opened stream for file: {0}", _currentFileName);
            }
            if (CompressBlob())
                _stream.Write(_header, 0, _header.Length);
        }

        /// <summary>
        /// Finalizes the file stream by updating the header before closing the stream down.
        /// </summary>
        /// <param name="continueWriting">True if a new stream should be opened for writing, otherwise false.</param>
        private void FinalizeStream(bool continueWriting = false)
        {
            try
            {
                if (_stream != null)
                {
                    _compressedLength += _stream.Length;

                    _stream.Seek(0, SeekOrigin.Begin);

                    var counter = BitConverter.GetBytes(_frameCount);
                    var largestFrame = BitConverter.GetBytes(_frameMaxSize);

                    if (CompressBlob())
                    {
                        _stream.Write(counter, 0, counter.Length);
                        _stream.Write(largestFrame, 0, largestFrame.Length);
                    }

                    _stream.Dispose();
                    using (LogManager lm = new LogManager())
                    {
                        lm.WriteInfo(this,"BlobContentWriter closed stream for file: {0}", _currentFileName);
                    }
                    _stream = null;
                }
            }
            finally
            {
                if (continueWriting)
                {
                    InitializeStream();
                }
                else
                {
                    _closed = true;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if managed resources should be cleaned up.</param>
        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            try
            {
                FinalizeStream();
            }
            finally
            {
                _disposed = true;
            }
        }

        #endregion

        #region Constructor and destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseFileName">The base name of the file(s) to generate content to.</param>
        /// <param name="maxSize">The max size of each file that will be generated.</param>
        public BlobContentWriter(string baseFileName, long maxSize)
        {
            if (String.IsNullOrEmpty(baseFileName)) throw new ArgumentNullException("baseFileName");

            _files = new List<string>();
            _baseFileName = baseFileName;
            _fileMaxSize = maxSize;
            _uncompressedLength = 0;
            _compressedLength = 0;

            InitializeStream();
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~BlobContentWriter()
        {
            Dispose(false);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Writes a new frame of content to the stream and returns the index of the frame. If max size is greater than
        /// zero, then file splitting will be used.
        /// </summary>
        /// <param name="content">The frame content to store.</param>
        /// <returns>The index of the frame, or (-1) if no content was passed.</returns>
        public int Write(string content)
        {
            if (String.IsNullOrEmpty(content)) return (-1);
            if (_closed) throw new ApplicationException("Content writer has been closed.");

            if ((_fileMaxSize > 0) && (_stream.Position > (_fileMaxSize + _header.Length)))
            {
                FinalizeStream(true);
            }

            var compressed = Compress(Encoding.ASCII.GetBytes(content));

            _frameMaxSize = Math.Max(compressed.Length, _frameMaxSize);

            var size = BitConverter.GetBytes(compressed.Length);

            if (CompressBlob())
                _stream.Write(size, 0, size.Length);
            _stream.Write(compressed, 0, compressed.Length);

            _uncompressedLength += (content.Length + size.Length);

            return _frameCount++;
        }

        /// <summary>
        /// Returns enumerator for the files list.
        /// </summary>
        /// <returns>The files list enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns enumerator for the files list.
        /// </summary>
        /// <returns>The files list enumerator</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        /// <summary>
        /// Closes the file stream.
        /// </summary>
        public void Close()
        {
            FinalizeStream();
        }

        /// <summary>
        /// Returns the name of the file from the file list using the specified index.
        /// </summary>
        public string Files(int index)
        {
            return _files[index];
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the name of the file from the file list using the specified index.
        /// </summary>
        public string this[int index]
        {
            get
            {
                return _files[index];
            }
        }

        /// <summary>
        /// Returns the number of generated files.
        /// </summary>
        public int FileCount
        {
            get
            {
                return _files.Count;
            }
        }

        /// <summary>
        /// Returns the total compressed length of the content.
        /// </summary>
        public long CompressedLength
        {
            get
            {
                return (_closed ? _compressedLength : _stream.Length + _compressedLength);
            }
        }

        /// <summary>
        /// Returns the uncompressed length of the total content.
        /// </summary>
        public long UncompressedLength
        {
            get
            {
                return _uncompressedLength;
            }
        }

        #endregion
    }
}
