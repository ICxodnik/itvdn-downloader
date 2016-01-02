using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itvdnDownloader
{
    class BatchDownloader
    {

        public void Downloader(IList<object> items)
        {
            //var scheduler = new LimitedConcurrencyLevelTaskScheduler(5);
            //TaskFactory factory = new TaskFactory(scheduler);

            //while (!actionQueue.IsEmpty)
            //{
            //    factory.StartNew(() =>
            //    {
            //        Action action;
            //        if (actionExecution.TryDequeue(out action))
            //            action.Invoke();

            //    }, TaskCreationOptions.LongRunning);
            //}
        }
    }
}
