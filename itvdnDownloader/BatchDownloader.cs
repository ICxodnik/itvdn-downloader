using DVB.Crew.ismdownloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Threading;

namespace itvdnDownloader
{
    class BatchDownloader
    {
        private CancellationTokenSource m_cts = new CancellationTokenSource();
        private readonly string m_outDir;

        public BatchDownloader(string outDir)
        {
            m_outDir = outDir;
        }

        public async Task Download(IList<LessonData> items)
        {
            var manifestGetter = new TransformBlock<LessonData, LessonData>((Func<LessonData, LessonData>)LoadManifest,
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 8, CancellationToken = m_cts.Token });

            var mediaDownloader = new TransformBlock<LessonData, ISMDownloader>((Func<LessonData, ISMDownloader>)DownloadMedia,
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2, CancellationToken = m_cts.Token });

            var mediaMuxer = new ActionBlock<ISMDownloader>((Action<ISMDownloader>)GenerateOutputFile, 
                new ExecutionDataflowBlockOptions() { CancellationToken = m_cts.Token });

            manifestGetter.LinkTo(mediaDownloader, new DataflowLinkOptions() { PropagateCompletion = true }, 
                data => data.ManifestUrl != null);
            manifestGetter.LinkTo(DataflowBlock.NullTarget<LessonData>());
            mediaDownloader.LinkTo(mediaMuxer, new DataflowLinkOptions() { PropagateCompletion = true });

            foreach (var item in items)
            {
                item.Progress = 50;
                manifestGetter.Post(item);
            }
            manifestGetter.Complete();

            await mediaMuxer.Completion;
        }

        public void Stop()
        {
            m_cts.Cancel();
        }

        private LessonData LoadManifest(LessonData data)
        {
            var service = new ItvdnWeb();
            data.OutputFilePath = Path.Combine(m_outDir, $"{data.Number}. {data.Title}.mkv");
            data.ManifestUrl = service.GetLessonManifestUrl(data.Url).Result;
            return data;
        }

        private ISMDownloader DownloadMedia(LessonData data)
        {
            var args = new Arguments(new string[] { data.ManifestUrl, data.OutputFilePath });

#if DEBUG
            args.StopAfter = TimeSpan.FromSeconds(15);
#endif

            var mediaDownloader = new ISMDownloader(args, m_cts.Token);
            mediaDownloader.OnStateChanged += MediaDownloader_OnStateChanged;
            mediaDownloader.Download(data);
            return mediaDownloader;
        }

        private void GenerateOutputFile(ISMDownloader mediaDownloader)
        {
            mediaDownloader.Mux();
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
