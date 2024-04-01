using GZipTest.Infrastructure;

namespace GZipTest.Archiver
{
    /// <summary>
    /// Архиватор позволяет сжимать и восстанавливать данные
    /// </summary>
    internal abstract class ArchiverGZip
    {
        /// <summary>
        /// Путь к исходному файлу
        /// </summary>
        protected readonly string _sourcePath;

        /// <summary>
        /// Путь к целевому файлу
        /// </summary>
        protected readonly string _targetPath;

        /// <summary>
        /// Синхронизированная очередь
        /// </summary>
        protected readonly ConcurrentQueue _queue;

        /// <summary>
        /// Токен отмены
        /// </summary>
        protected readonly CancellationToken _token;

        internal ArchiverGZip(string sourcePath, string targetPath, CancellationToken token)
        {
            _sourcePath = sourcePath;
            _targetPath = targetPath;
            _queue = new ConcurrentQueue();
            _token = token;
        }

        /// <summary>
        /// Запустить
        /// </summary>
        internal void Run()
        {
            Thread threadReadFile = new(Read);
            threadReadFile.Name = $"Read_Thread";
            threadReadFile.Start();

            Thread threadWriteFile = new(Write);
            threadWriteFile.Name = $"Write_Thread";
            threadWriteFile.Start();

            threadReadFile.Join();
            threadWriteFile.Join();
        }

        /// <summary>
        /// Чтение файла по блокам и добавление в очередь 
        /// </summary>
        protected abstract void Read();

        /// <summary>
        /// Извлечение из очереди и запись в новый файл
        /// </summary>
        protected abstract void Write();
    }
}
