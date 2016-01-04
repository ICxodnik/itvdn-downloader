using DVB.Crew.ismdownloader;
using DVB.Crew.Mux;
using DVB.Crew.SmoothStreaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace itvdnDownloader
{
    class ISMDownloader
    {
        private const float PercentMultiplier = 100 * 10000000;
        private ISM2MKV m_muxer;
        private readonly Arguments m_args;
        private readonly CancellationToken m_cancellationToken;
        private LessonData m_context;
        private TimeSpan m_duration;
        public event Action<LessonData, State, int> OnStateChanged;

        public ISMDownloader(Arguments args, CancellationToken cancellationToken)
        {
            m_args = args;
            m_cancellationToken = cancellationToken;
        }

        public void Download(LessonData context)
        {
            m_context = context;
            StateChanged(State.AquireManifestUrl);
            m_muxer = new ISM2MKV(new Uri(m_args.ManifestUrl), m_args.Output, m_args.Cookie, m_args.Proxy);

            // initialize working dir
            m_muxer.WorkingDirectory = InitWorkingDirectory(m_args.Output, context.Number);
            m_muxer.StopAfter = m_args.StopAfter;

            if (m_args.StopAfter == default(TimeSpan))
            {
                m_duration = TimeSpan.FromSeconds(m_muxer.Manifest.Duration / 10000000);
            }
            else
            {
                m_duration = m_args.StopAfter;
            }

            switch (m_args.TrackSelectionMode)
            {
                case TrackSelectionMode.All:
                    using (IEnumerator<StreamInfo> enumerator = m_muxer.Manifest.SelectedStreams.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            StreamInfo current = enumerator.Current;
                            current.SelectedTracks.Clear();
                            foreach (TrackInfo trackInfo in (IEnumerable<TrackInfo>)current.AvailableTracks)
                                current.SelectedTracks.Add(trackInfo);
                        }
                        break;
                    }
                case TrackSelectionMode.LowestQuality:
                    using (IEnumerator<StreamInfo> enumerator = m_muxer.Manifest.SelectedStreams.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            StreamInfo current = enumerator.Current;
                            current.SelectedTracks.Clear();
                            current.SelectedTracks.Add(current.AvailableTracks[current.AvailableTracks.Count - 1]);
                        }
                        break;
                    }
            }
            if (!m_muxer.Download)
                Console.WriteLine("Muxing from HDD\n");
            Console.Write(m_muxer.Manifest.GetDescription());
            Console.WriteLine("Recording duration:");
            m_muxer.DurationChanged += new EventHandler(this.DisplayDuration);
            StateChanged(State.Download);
            m_muxer.Record();
            Console.WriteLine();
            Console.WriteLine("Downloading finished!");
            DateTime now = DateTime.Now;
            Console.WriteLine();
            Console.WriteLine("Muxing selected tracks to MKV");
            StateChanged(State.Mux);
            m_muxer.Mux();
            StateChanged(State.Completed);
            Console.WriteLine("Mux completed in " + DateTime.Now.Subtract(now).ToString());

            m_muxer.WorkingDirectory.Delete(true);
        }

        private DirectoryInfo InitWorkingDirectory(string output, string key)
        {
            return new DirectoryInfo(Path.Combine(Path.GetDirectoryName(m_args.Output), "_cache", key));
        }

        private void DisplayDuration(object sender, EventArgs e)
        {
            if (m_cancellationToken.IsCancellationRequested)
            {
                m_muxer.StopRecording();
                return;
            }
            StateChanged(State.Download);
        }

        private void StateChanged(State state)
        {
            int progress = 0;
            if (state == State.Download)
            {
                progress = (int)(100 * m_muxer.Duration.TotalSeconds / m_duration.TotalSeconds);
            }
            OnStateChanged?.Invoke(m_context, state, progress);
        }

        public enum State
        {
            AquireManifestUrl,
            Download,
            Mux,
            Completed
        }
    }
}
