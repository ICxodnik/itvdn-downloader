using DVB.Crew.ismdownloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace itvdnDownloader
{
    class BatchDownloader
    {
        private const int m_delayMiliseconds = 5 * 1000; // 5 second
        private readonly uint m_concurentThreads;
        private volatile int m_currentThreads = 0;
        private CancellationTokenSource m_cts = new CancellationTokenSource();
        private readonly TaskFactory m_factory;
        private readonly string m_outDir;

        public BatchDownloader(string outDir, uint concurentThreads = 1)
        {
            m_outDir = outDir;
            m_concurentThreads = concurentThreads;
            m_factory = new TaskFactory(m_cts.Token);
        }


        public async Task Download(IList<LessonData> items)
        {
            var queue = new Queue<LessonData>(items);
            while (queue.Count > 0)
            {
                if (m_currentThreads < m_concurentThreads)
                {
                    var item = queue.Dequeue();
#pragma warning disable CS4014 // don't await
                    Prepare(item).ContinueWith(OnComplete);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Interlocked.Increment(ref m_currentThreads);
                }

                await Task.Delay(m_delayMiliseconds);
            }


        }

        public void Stop()
        {
            m_cts.Cancel();
        }

        private void OnComplete(Task task)
        {
            Interlocked.Decrement(ref m_currentThreads);
        }

        private Task Prepare(LessonData data)
        {
            return m_factory.StartNew(() =>
            {
                var service = new ItvdnWeb();
                data.ManifestUrl = service.GetLessonManifestUrl(data.Url).Result;
                if (data.ManifestUrl == null)
                {
                    return;
                }
                var outFile = Path.Combine(m_outDir, $"{data.Number}. {data.Title}.mkv");
                var args = new Arguments(new string[] { data.ManifestUrl, outFile });

#if DEBUG
                //args.StopAfter = TimeSpan.FromSeconds(40);
#endif

                var mediaDownloader = new ISMDownloader(args, m_cts.Token);
                mediaDownloader.OnStateChanged += MediaDownloader_OnStateChanged;
                mediaDownloader.Download(data);

            }, m_cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        private void MediaDownloader_OnStateChanged(LessonData context, ISMDownloader.State state, int progress)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (state)
                {
                    case ISMDownloader.State.AquireManifestUrl:
                        context.Status = "AAA";
                        break;
                    case ISMDownloader.State.Download:
                        context.Status = "BBB";
                        break;
                    case ISMDownloader.State.Mux:
                        context.Status = "CCC";
                        break;
                    case ISMDownloader.State.Completed:
                        context.Status = "Completed";
                        break;
                }
                context.Progress = progress;
            });
        }
    }
}
