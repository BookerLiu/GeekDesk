using System.Threading;
using System.Windows.Threading;

namespace GeekDesk.MyThread
{
    public class DispatcherBuild
    {

        //创建一个Dispatcher来单独使用ui线程
        public static Dispatcher Build()
        {
            Dispatcher dispatcher = null;
            var manualResetEvent = new ManualResetEvent(false);
            var thread = new System.Threading.Thread(() =>
            {
                dispatcher = Dispatcher.CurrentDispatcher;
                var synchronizationContext = new DispatcherSynchronizationContext(dispatcher);
                SynchronizationContext.SetSynchronizationContext(synchronizationContext);

                manualResetEvent.Set();
                Dispatcher.Run();
            });
            thread.IsBackground = true;
            thread.Start();
            manualResetEvent.WaitOne();
            manualResetEvent.Dispose();
            return dispatcher;
        }
    }
}
