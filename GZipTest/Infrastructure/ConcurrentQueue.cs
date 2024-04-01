using System.Diagnostics;

namespace GZipTest.Infrastructure
{
    /// <summary>
    /// Потокобезопасная очередь FIFO
    /// </summary>
    internal class ConcurrentQueue
    {
        /// <summary>
        /// Синхронизируемая очередь 
        /// </summary>
        private readonly Queue<ByteBlock> _queue;

        /// <summary>
        /// объект блокировки
        /// </summary>
        private readonly object _queueLock = new object();

        private bool _isEnqueueClose;
        /// <summary>
        /// Состояние описывающее, закрыта ли очередь для добавления элементов
        /// </summary>
        internal bool IsEnqueueClose { get { return _isEnqueueClose; } }

        internal ConcurrentQueue()
        {
            _queue = new Queue<ByteBlock>();
            _isEnqueueClose = false;
        }

        /// <summary>
        /// Добавить в очередь
        /// </summary>
        /// <param name="buffer">буфер</param>
        internal void Enqueue(ByteBlock buffer)
        {
            if (_isEnqueueClose)
                return;

            lock (_queueLock)
            {
                if (_queue.Count > Options.MaxNumInQueue)
                    Monitor.Wait(_queueLock);   // освобождает _queueLock и блокирует текущий поток до вызова Pulse из другого потока

                _queue.Enqueue(buffer);
                Monitor.Pulse(_queueLock);

                #region для отладки
                //Debug.WriteLine($"E {buffer.Id}");
                #endregion
            }
        }

        /// <summary>
        /// Удалить и вернуть из очереди буфер, находящийся в начале очереди.
        /// </summary>
        /// <param name="value">буфер из очереди</param>
        /// <returns>true, если элемент был успешно удален и возвращен из начала очереди</returns>
        internal bool TryDequeue(out ByteBlock value)
        {
            lock (_queueLock)
            {
                while (_queue.Count == 0)
                {
                    if (_isEnqueueClose)
                    {
                        value = new ByteBlock(0);
                        return false;
                    }

                    Monitor.Wait(_queueLock);   // освобождает _queueLock и блокирует текущий поток до вызова Pulse из другого потока
                }

                value = _queue.Dequeue();
                Monitor.Pulse(_queueLock);
                
                #region для отладки
                //Debug.WriteLine($"D {value.Id}");
                #endregion

                return true;
            }
        }

        /// <summary>
        /// Закрыть очередь для добавления
        /// </summary>
        internal void CloseEnqueuing()
        {
            lock (_queueLock)
            {
                _isEnqueueClose = true;
                Monitor.PulseAll(_queueLock);
            }
        }
    }
}
