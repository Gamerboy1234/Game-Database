
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logger;

namespace Utilities
{
    public static class AsyncHelper
    {
        #region Public Methods

        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;

            var synch = new ExclusiveSynchronizationContext();

            SynchronizationContext.SetSynchronizationContext(synch);

            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }

                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }

                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);

            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;

            var synch = new ExclusiveSynchronizationContext();

            SynchronizationContext.SetSynchronizationContext(synch);

            var result = default(T);

            synch.Post(async _ =>
            {
                try
                {
                    result = await task();
                }

                catch (Exception ex)
                {
                    Log.Error(ex);

                    synch.InnerException = ex;
                    throw;
                }

                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);

            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);

            return result;
        }

        #endregion Public Methods


        #region Private Classes

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            #region Private Fields

            private bool _done;
            private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);
            private readonly Queue<Tuple<SendOrPostCallback, object>> _items = new Queue<Tuple<SendOrPostCallback, object>>();

            #endregion Private Fields


            #region Properties

            public Exception InnerException { private get; set; }

            #endregion Properties


            #region Public Methods

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (_items)
                {
                    _items.Enqueue(Tuple.Create(d, state));
                }

                _workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => _done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!_done)
                {
                    Tuple<SendOrPostCallback, object> task = null;

                    lock (_items)
                    {
                        if (_items.Count > 0)
                        {
                            task = _items.Dequeue();
                        }
                    }

                    if (task != null)
                    {
                        task.Item1(task.Item2);

                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }

                    else
                    {
                        _workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}
