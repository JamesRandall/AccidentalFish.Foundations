﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Resources.Abstractions.Blobs
{
    /// <summary>
    /// A reference to a blob
    /// </summary>
    public interface IBlob
    {
        /// <summary>
        /// Download the blob as a stream
        /// </summary>
        /// <returns>A stream the blob can be read from</returns>
        Task<Stream> DownloadAsync();
        /// <summary>
        /// Download the blob as a UTF8 string
        /// </summary>
        /// <returns>A string containing the blob</returns>
        Task<string> DownloadStringAsync();
        /// <summary>
        /// Download the blob as a string using the specified encoding
        /// </summary>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>A string containing the blob</returns>
        Task<string> DownloadStringAsync(Encoding encoding);
        /// <summary>
        /// Download the blob as a byte array
        /// </summary>
        /// <returns>A byte array containing the blob</returns>
        Task<byte[]> DownloadBytesAsync();
        /// <summary>
        /// Gets a stream that can be used to write the blob to
        /// </summary>
        /// <returns>A writable stream</returns>
        Task<Stream> GetUploadStreamAsync();
        /// <summary>
        /// Uploads the blob from the byte array
        /// </summary>
        /// <param name="bytes">The byte array containing the blob</param>
        /// <returns>Awaitable task</returns>
        Task UploadBytesAsync(byte[] bytes);
        /// <summary>
        /// Upload the blob from the specified stream
        /// </summary>
        /// <param name="stream">A stream from which the blob can be read</param>
        /// <returns>An awaitable task</returns>
        Task UploadStreamAsync(Stream stream);        
        /// <summary>
        /// Deletes the blob
        /// </summary>
        /// <returns>An awaitable task</returns>
        Task DeleteAsync();
        /// <summary>
        /// The URL the blob is available on
        /// </summary>
        Uri Url { get; }
    }
}
