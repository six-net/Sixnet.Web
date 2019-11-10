using EZNEW.Framework.Extension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Internal.MQ
{
    /// <summary>
    /// internal message queue
    /// </summary>
    public class InternalMessageQueue
    {
        static readonly BlockingCollection<IInternalMessageQueueCommand> InternalQueue = null;
        static readonly object consumerLock = new object();
        static Action consumeAction = null;
        static int currentConsumer = 0;

        /// <summary>
        /// max consumer count
        /// </summary>
        public static int MaxConsumerCount { get; set; } = 1;

        static InternalMessageQueue()
        {
            InternalQueue = new BlockingCollection<IInternalMessageQueueCommand>();
            consumeAction = () =>
            {
                foreach (var cmd in InternalQueue.GetConsumingEnumerable())
                {
                    cmd?.Run();
                }
            };
            StartConsume();
        }

        #region Enqueue

        /// <summary>
        /// enqueue message
        /// </summary>
        /// <param name="commands">commands</param>
        public static void Enqueue(params IInternalMessageQueueCommand[] commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return;
            }
            foreach (var cmd in commands)
            {
                InternalQueue.TryAdd(cmd);
            }
        }

        #endregion

        #region Start Consume

        /// <summary>
        /// start consume
        /// </summary>
        /// <param name="count">start new consumer count</param>
        public static void StartConsume(int count = 1)
        {
            lock (consumerLock)
            {
                for (var i = 0; i < count; i++)
                {
                    if (currentConsumer < MaxConsumerCount)
                    {
                        currentConsumer++;
                        Task.Run(consumeAction);
                    }
                }
            }
        }

        #endregion
    }
}
