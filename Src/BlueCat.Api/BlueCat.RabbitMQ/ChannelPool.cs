
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Class ChannelPool.
    /// </summary>
    /// <seealso cref="BlueCat.RabbitMQ.IChannelPool" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public class ChannelPool : IChannelPool
    {
        /// <summary>
        /// Gets the connection pool.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected IConnectionPool ConnectionPool { get; }

        /// <summary>
        /// Gets the channels.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected ConcurrentDictionary<string, ChannelPoolItem> Channels { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets the total duration of the dispose wait.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected TimeSpan TotalDisposeWaitDuration { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public ILogger<ChannelPool> Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelPool"/> class.
        /// </summary>
        /// <param name="connectionPool">The connection pool.</param>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public ChannelPool(IConnectionPool connectionPool)
        {
            ConnectionPool = connectionPool;
            Channels = new ConcurrentDictionary<string, ChannelPoolItem>();
            Logger = NullLogger<ChannelPool>.Instance;
        }

        /// <summary>
        /// Acquires the specified channel name.
        /// </summary>
        /// <param name="channelName">Name of the channel.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>IChannelAccessor.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public virtual IChannelAccessor Acquire(string channelName = null, string connectionName = null)
        {
            CheckDisposed();

            channelName = channelName ?? "";

            var poolItem = Channels.GetOrAdd(
                channelName,
                _ => new ChannelPoolItem(CreateChannel(channelName, connectionName))
            );

            poolItem.Acquire();

            return new ChannelAccessor(
                poolItem.Channel,
                channelName,
                () => poolItem.Release()
            );
        }

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="channelName">Name of the channel.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>IModel.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected virtual IModel CreateChannel(string channelName, string connectionName)
        {
            return ConnectionPool
                .Get(connectionName)
                .CreateModel();
        }

        /// <summary>
        /// Checks the disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">ChannelPool</exception>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(ChannelPool));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!Channels.Any())
            {
                Logger.LogDebug($"Disposed channel pool with no channels in the pool.");
                return;
            }

            var poolDisposeStopwatch = Stopwatch.StartNew();

            Logger.LogInformation($"Disposing channel pool ({Channels.Count} channels).");

            var remainingWaitDuration = TotalDisposeWaitDuration;

            foreach (var poolItem in Channels.Values)
            {
                var poolItemDisposeStopwatch = Stopwatch.StartNew();

                try
                {
                    poolItem.WaitIfInUse(remainingWaitDuration);
                    poolItem.Dispose();
                }
                catch
                { }

                poolItemDisposeStopwatch.Stop();

                remainingWaitDuration = remainingWaitDuration > poolItemDisposeStopwatch.Elapsed
                    ? remainingWaitDuration.Subtract(poolItemDisposeStopwatch.Elapsed)
                    : TimeSpan.Zero;
            }

            poolDisposeStopwatch.Stop();

            Logger.LogInformation($"Disposed RabbitMQ Channel Pool ({Channels.Count} channels in {poolDisposeStopwatch.Elapsed.TotalMilliseconds:0.00} ms).");

            if(poolDisposeStopwatch.Elapsed.TotalSeconds > 5.0)
            {
                Logger.LogWarning($"Disposing RabbitMQ Channel Pool got time greather than expected: {poolDisposeStopwatch.Elapsed.TotalMilliseconds:0.00} ms.");
            }

            Channels.Clear();
        }

        /// <summary>
        /// Class ChannelPoolItem.
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected class ChannelPoolItem : IDisposable
        {
            /// <summary>
            /// Gets the channel.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public IModel Channel { get; }

            /// <summary>
            /// Gets a value indicating whether this instance is in use.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public bool IsInUse
            {
                get => _isInUse;
                private set => _isInUse = value;
            }
            /// <summary>
            /// The is in use
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            private volatile bool _isInUse;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChannelPoolItem"/> class.
            /// </summary>
            /// <param name="channel">The channel.</param>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public ChannelPoolItem(IModel channel)
            {
                Channel = channel;
            }

            /// <summary>
            /// Acquires this instance.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public void Acquire()
            {
                lock (this)
                {
                    while (IsInUse)
                    {
                        Monitor.Wait(this);
                    }

                    IsInUse = true;
                }
            }

            /// <summary>
            /// Waits if in use.
            /// </summary>
            /// <param name="timeout">The timeout.</param>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public void WaitIfInUse(TimeSpan timeout)
            {
                lock (this)
                {
                    if (!IsInUse)
                    {
                        return;
                    }

                    Monitor.Wait(this, timeout);
                }
            }

            /// <summary>
            /// Releases this instance.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public void Release()
            {
                lock (this)
                {
                    IsInUse = false;
                    Monitor.PulseAll(this);
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public void Dispose()
            {
                Channel.Dispose();
            }
        }

        /// <summary>
        /// Class ChannelAccessor.
        /// </summary>
        /// <seealso cref="BlueCat.RabbitMQ.IChannelAccessor" />
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected class ChannelAccessor : IChannelAccessor
        {
            /// <summary>
            /// Reference to the channel.
            /// Never dispose the <see cref="Channel" /> object.
            /// Instead, dispose the <see cref="IChannelAccessor" /> after usage.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public IModel Channel { get; }

            /// <summary>
            /// Name of the channel.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public string Name { get; }

            /// <summary>
            /// The dispose action
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            private readonly Action _disposeAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChannelAccessor"/> class.
            /// </summary>
            /// <param name="channel">The channel.</param>
            /// <param name="name">The name.</param>
            /// <param name="disposeAction">The dispose action.</param>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public ChannelAccessor(IModel channel, string name, Action disposeAction)
            {
                _disposeAction = disposeAction;
                Name = name;
                Channel = channel;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <remarks>
            /// <para>作者    :jason</para>	
            /// <para>创建时间:2018-12-19</para>
            /// <para>最后更新:jason</para>	
            /// <para>更新时间:2018-12-19</para>
            /// </remarks>
            public void Dispose()
            {
                _disposeAction.Invoke();
            }
        }
    }
}