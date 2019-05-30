using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Framework.Extension;
using System.Linq;

namespace EZNEW.Framework.Code
{

    /// <summary>
    /// generate serial number
    /// </summary>
    public static class SerialNumber
    {
        static Dictionary<string, SnowflakeNet> snowflakeNetGroups = new Dictionary<string, SnowflakeNet>();

        /// <summary>
        /// Register SerialNumber Generator
        /// </summary>
        /// param name="idGroups">Id Groups</param>
        /// <param name="dataCenterId">data center id(1-31)</param>
        /// <param name="worderId">work id(1-31)</param>
        /// <param name="sequence">sequence</param>
        public static void RegisterGenerator(IEnumerable<string> idGroups, long dataCenterId, long worderId, long sequence = 0L)
        {
            if (idGroups.IsNullOrEmpty())
            {
                return;
            }
            idGroups = idGroups.Distinct();
            foreach (string group in idGroups)
            {
                if (!snowflakeNetGroups.ContainsKey(group))
                {
                    snowflakeNetGroups.Add(group, new SnowflakeNet(worderId, dataCenterId, sequence));
                }
            }
        }

        /// <summary>
        /// get a serial number
        /// </summary>
        /// <param name="idGroup">Id Group</param>
        /// <returns>serial number</returns>
        public static long GetSerialNumber(string idGroup = "")
        {
            if (idGroup.IsNullOrEmpty() || !snowflakeNetGroups.ContainsKey(idGroup))
            {
                throw new Exception("not register current idGroup");
            }
            var generator = snowflakeNetGroups[idGroup];
            return generator.GenerateID();
        }
    }

    /// <summary>
    /// Snowflake
    /// </summary>
    internal class SnowflakeNet
    {
        public const long twepoch = 106686661L;
        DateTime startDate = new DateTime(2017, 7, 10, 0, 0, 0, DateTimeKind.Utc);
        const int workerIdBits = 5;
        const int datacenterIdBits = 5;
        const int sequenceBits = 12;
        const long maxWorkerId = -1L ^ (-1L << workerIdBits);
        const long maxDatacenterId = -1L ^ (-1L << datacenterIdBits);
        private const int workerIdShift = sequenceBits;
        private const int datacenterIdShift = sequenceBits + workerIdBits;
        public const int timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits;
        private const long sequenceMask = -1L ^ (-1L << sequenceBits);
        private long lastTimestamp = -1L;
        readonly object lockObj = new object();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="workerId">work app id(1-31)</param>
        /// <param name="dataCenterId">data center id(1-31)</param>
        /// <param name="sequence">sequence</param>
        public SnowflakeNet(long workerId, long dataCenterId, long sequence = 0L)
        {
            WorkerId = workerId;
            this.DataCenterId = dataCenterId;
            this.Sequence = sequence;
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id can't be greater than {0} or less than 0", maxWorkerId));
            }
            if (dataCenterId > maxDatacenterId || dataCenterId < 0)
            {
                throw new ArgumentException(string.Format("datacenter Id can't be greater than {0} or less than 0", maxDatacenterId));
            }
        }

        /// <summary>
        /// Work Id
        /// </summary>
        public long WorkerId { get; } = 0L;

        /// <summary>
        /// Data Center Id
        /// </summary>
        public long DataCenterId { get; } = 0L;

        /// <summary>
        /// Sequence
        /// </summary>
        public long Sequence { get; private set; } = 0L;

        /// <summary>
        /// Generate Id
        /// </summary>
        /// <returns></returns>
        public virtual long GenerateID()
        {
            lock (lockObj)
            {
                var timestamp = TimeGen();
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("time is return back,generated fail");
                }
                if (lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & sequenceMask;
                    if (Sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    Sequence = 0;
                }
                lastTimestamp = timestamp;
                var id = ((timestamp - twepoch) << timestampLeftShift) |
                         (DataCenterId << datacenterIdShift) |
                         (WorkerId << workerIdShift) | Sequence;

                return id;
            }
        }

        public virtual long GenerateIDByTime(long timestamp)
        {
            lock (lockObj)
            {
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("time is return back,generated fail");
                }
                if (lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & sequenceMask;
                    if (Sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    Sequence = 0;
                }
                lastTimestamp = timestamp;
                var id = ((timestamp - twepoch) << timestampLeftShift) |
                         (DataCenterId << datacenterIdShift) |
                         (WorkerId << workerIdShift) | Sequence;

                return id;
            }
        }

        /// <summary>
        /// wait next millis
        /// </summary>
        /// <param name="lastTimestamp">lastTimestamp</param>
        /// <returns></returns>
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// return time
        /// </summary>
        /// <returns></returns>
        public virtual long TimeGen()
        {
            return (long)(DateTime.UtcNow - startDate).TotalMilliseconds;
        }
    }
}
