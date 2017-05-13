using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation
{
    internal class BlockBlob : IBlob
    {
        private readonly CloudBlockBlob _blockBlob;
        private readonly string _name;
        private readonly ILogger<BlockBlob> _logger;

        public BlockBlob(CloudBlockBlob blockBlob, string name, ILogger<BlockBlob> logger)
        {
            if (blockBlob == null) throw new ArgumentNullException(nameof(blockBlob));

            _blockBlob = blockBlob;
            _name = name;
            _logger = logger;
        }

        internal CloudBlockBlob CloudBlockBlob => _blockBlob;

        public async Task<byte[]> DownloadBytesAsync()
        {
            _logger?.LogTrace("BlockBlob: DownloadBytesAsync - attempting download of {NAME}", _name);
            Stopwatch sw = Stopwatch.StartNew();
            byte[] result;
            using (MemoryStream stream = new MemoryStream())
            {
                await _blockBlob.DownloadToStreamAsync(stream);
                result = stream.ToArray();
            }
            sw.Stop();
            _logger?.LogTrace("BlockBlob: DownloadBytesAsync - download of {NAME} succeeded in {TIMEMES}ms", _name, sw.ElapsedMilliseconds);
            return result;
        }

        public Task<string> DownloadStringAsync()
        {
            return DownloadStringAsync(Encoding.UTF8);
        }

        public async Task<string> DownloadStringAsync(Encoding encoding)
        {
            _logger?.LogTrace("BlockBlob: DownloadString - attempting download of {NAME}", _name);
            Stopwatch sw = Stopwatch.StartNew();
            string result = await _blockBlob.DownloadTextAsync(encoding, null, null, null);
            sw.Stop();
            _logger?.LogTrace("BlockBlob: DownloadString - download of {NAME} succeeded in {TIMEMS}ms", _name, sw.ElapsedMilliseconds);
            return result;
        }

        public async Task<Stream> DownloadAsync()
        {
            _logger?.LogTrace("BlockBlob: DownloadAsync - attempting download of {NAME}", _name);
            Stopwatch sw = Stopwatch.StartNew();
            MemoryStream stream = new MemoryStream();
            await _blockBlob.DownloadToStreamAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            sw.Stop();
            _logger?.LogTrace("BlockBlob: DownloadAsync - download of {NAME} to stream succeeded in {TIMEMS}ms", _name, sw.ElapsedMilliseconds);
            return stream;
        }

        public async Task<Stream> GetUploadStreamAsync()
        {
            return await _blockBlob.OpenWriteAsync();
        }

        public async Task<bool> Exists()
        {
            bool doesExist = await _blockBlob.ExistsAsync();
            return doesExist;
        }

        public async Task UploadBytesAsync(byte[] bytes)
        {
            _logger?.LogTrace("BlockBlob: UploadBytesAsync - attempting upload of {NAME} bytes {BYTES}", bytes.Length, _name);
            Stopwatch sw = Stopwatch.StartNew();
            await _blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            sw.Stop();
            _logger?.LogTrace("BlockBlob: UploadBytesAsync - upload of {NAME} succeeded in {TIMEMS}ms", _name, sw.ElapsedMilliseconds);
        }

        public async Task UploadStreamAsync(Stream stream)
        {
            _logger?.LogTrace("BlockBlob: UploadStreamAsync - attempting upload of {NAME}", _name);
            Stopwatch sw = Stopwatch.StartNew();
            await _blockBlob.UploadFromStreamAsync(stream);
            sw.Stop();
            _logger?.LogTrace("BlockBlob: UploadStreamAsync - upload of {NAME} succeeded in {TIMEMS}ms", _name, sw.ElapsedMilliseconds);
        }        

        public async Task DeleteAsync()
        {
            _logger?.LogTrace("BlockBlob: DeleteAsync - attempting delete of {NAME}", _name);
            Stopwatch sw = Stopwatch.StartNew();
            await _blockBlob.DeleteIfExistsAsync();
            sw.Stop();
            _logger?.LogTrace("BlockBlob: DeleteAsync - delete of {NAME} succeeded in {TIMEMS}ms", _name, sw.ElapsedMilliseconds);
        }
        
        public Uri Url => _blockBlob.Uri;
    }
}
