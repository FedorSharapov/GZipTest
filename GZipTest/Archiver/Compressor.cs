using GZipTest.Infrastructure;
using System.IO.Compression;

namespace GZipTest.Archiver.Archiver
{
    internal sealed class Compressor : ArchiverGZip
    {
        internal Compressor(string sourcePath, string targetPath, CancellationToken token)
            : base(sourcePath, targetPath, token) { }

        protected override void Read()
        {
            using FileStream sourceStream = File.Open(_sourcePath, FileMode.Open, FileAccess.Read);
            while (sourceStream.Position < sourceStream.Length)
            {
                if (_token.IsCancellationRequested)
                    return;

                var remainderBytes = sourceStream.Length - sourceStream.Position;
                var bufferSize = remainderBytes < Options.BufferSize ? (int)remainderBytes : Options.BufferSize;

                var buffer = new ByteBlock(bufferSize);
                sourceStream.Read(buffer.Data, 0, buffer.Data.Length);
                _queue.Enqueue(buffer);
            }

            _queue.CloseEnqueuing();
        }

        protected override void Write()
        {
            using FileStream targetStream = new FileStream(_targetPath, FileMode.Create);

            // На место первых 8 байт записываем первоначальный размер файла в байтах 
            var sourceFileLength = new FileInfo(_sourcePath).Length;
            var buf = BitConverter.GetBytes(sourceFileLength);
            targetStream.Write(buf, 0, buf.Length);

            // извлекаем из очереди, сжимаем и записываем в новый файл
            using GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
            while (true)
            {
                if (_token.IsCancellationRequested)
                    return;

                if (_queue.TryDequeue(out ByteBlock byteBlock))
                    compressionStream.Write(byteBlock.Data);
                else if (_queue.IsEnqueueClose)     // если очередь пуста и добавление в очередь завершено
                    break;
            }
        }
    }
}
