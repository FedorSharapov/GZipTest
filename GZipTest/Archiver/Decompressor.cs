using GZipTest.Infrastructure;
using System.IO.Compression;

namespace GZipTest.Archiver
{
    internal sealed class Decompressor : ArchiverGZip
    {
        internal Decompressor(string sourcePath, string targetPath, CancellationToken token) 
            : base(sourcePath, targetPath, token) { }

        protected override void Read()
        {
            using FileStream sourceStream = File.Open(_sourcePath, FileMode.Open, FileAccess.Read);

            // Считываем первые 8 байт - это первоначальный размер файла в байтах 
            byte[] fileLengthBuffer = new byte[8];
            sourceStream.Read(fileLengthBuffer, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBuffer);

            // распаковываем и добавляем в очередь
            using GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress);
            while (fileLength > 0)
            {
                if (_token.IsCancellationRequested)
                    return;

                var bufferSize = fileLength < Options.BufferSize ? (int)fileLength : Options.BufferSize;

                var buffer = new ByteBlock(bufferSize);
                decompressionStream.Read(buffer.Data, 0, buffer.Data.Length);
                fileLength -= bufferSize;

                _queue.Enqueue(buffer);
            }

            _queue.CloseEnqueuing();
        }

        protected override void Write()
        {
            using FileStream targetStream = new FileStream(_targetPath, FileMode.Create);
            while (true)
            {
                if (_token.IsCancellationRequested)
                    return;

                if (_queue.TryDequeue(out ByteBlock byteBlock)) // извлекаем из очереди
                    targetStream.Write(byteBlock.Data);         // и записываем в новый файл
                else if (_queue.IsEnqueueClose)                 // если очередь пуста и добавление в очередь завершено
                    break;
            }
        }
    }
}
