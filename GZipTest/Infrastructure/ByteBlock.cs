namespace GZipTest.Infrastructure
{
    internal class ByteBlock
    {
        /// <summary>
        /// Идентификатор блока данных (для отладки)
        /// </summary>
        internal int Id { get; }

        /// <summary>
        /// Буфер для чтения блока данных
        /// </summary>
        internal byte[] Data { get; init; }

        private static int _counter;

        static ByteBlock() => _counter = 0;

        internal ByteBlock(int bufferSize = Options.BufferSize)
        {
            Id = ++_counter;
            Data = new byte[bufferSize];
        }
    }
}
