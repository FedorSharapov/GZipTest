namespace GZipTest.Infrastructure
{
    /// <summary>
    /// Настройки для отладки и экспериментов
    /// </summary>
    internal static class Options
    {
        /// <summary>
        /// Максимальное количество элементов в очереди
        /// </summary>
        internal const int MaxNumInQueue = 10;

        /// <summary>
        /// Размер буфера для чтения из файла
        /// </summary>
        internal const int BufferSize = 81920;
        // This value was originally picked to be the largest multiple of 4096 that is still smaller than the large object heap threshold (85K).
        // The CopyTo{Async} buffer is short-lived and is likely to be collected at Gen0, and it offers a significant improvement in Copy
        // performance.  Since then, the base implementations of CopyTo{Async} have been updated to use ArrayPool, which will end up rounding
        // this size up to the next power of two (131,072), which will by default be on the large object heap.  However, most of the time
        // the buffer should be pooled, the LOH threshold is now configurable and thus may be different than 85K, and there are measurable
        // benefits to using the larger buffer size.  So, for now, this value remains.
    }
}
