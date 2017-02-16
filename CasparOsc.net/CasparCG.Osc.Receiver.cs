using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using Bespoke.Common.Osc;
using System.Diagnostics;


namespace CasparCG.Osc.net
{
    public class OscReceiver : IDisposable
    {

        public OscReceiver()
        {
            PrimaryLayer = "1";
            PrimaryChannel = "10";
            OscPort = 6250;
            OscTransportType = TransportType.Udp;
            OscServerIp = IPAddress.Parse("127.0.0.1");
        }

        private static Mutex mutMessage = new Mutex();

        //FOR PRIMARY EVENT CHECKING
        private bool firstFrameReceived = false;
        long lastFrame = -1;


        private int _playCheckIntervall = 80;
        public int PlayCheckIntervall
        {
            get{
                return _playCheckIntervall;
            }
            set
            {
                _playCheckIntervall = value;
                if (timerCheckPrimaryPlaying != null)
                    timerCheckPrimaryPlaying.Change(0, _playCheckIntervall);
            }
        }

        public IPAddress OscServerIp { get; set; }
        public int OscPort { get; set; }


        private bool _primaryPlaying;
        public bool PrimaryPlaying { get { return _primaryPlaying; } }

        private bool _primaryLoop;
        public bool PrimaryLoop { get { return _primaryLoop; } }

        private Int64 _primaryFrame;
        public Int64 PrimaryFrame { get { return _primaryFrame; } }

        private string _primaryChannel;
        public string PrimaryChannel { get { return _primaryChannel; } set { _primaryChannel = value; firstFrameReceived = false; lastFrame = -1; } }

        private string _primaryLayer;
        public string PrimaryLayer { get { return _primaryLayer; } set { _primaryLayer = value; firstFrameReceived = false; lastFrame = -1; } }


        public OscServer OscAgent { get { return receiver; } }
        public TransportType OscTransportType { get; set; }
        public bool OscIsRunning
        {
            get
            {
                if (receiver == null)
                {
                    return false;
                }
                else
                {
                    return receiver.IsRunning;
                }
            }
        }




        //private OscReceiver receiver;
        private OscServer receiver;
    
   

        public void OSCStart()
        {
            // This is the port we are going to listen on 
            if (receiver != null)
            {
                OSCStop();
                OSCStart();
            }
            else
            {
                lastFrame = -1;
                firstFrameReceived = false;
                _primaryPlaying = false;
                _primaryLoop = false;
                _primaryFrame = 0;

                // Create the receiver
                receiver = new OscServer(TransportType.Udp, OscServerIp, OscPort);
                receiver.MessageReceived += receiver_MessageReceived;
                receiver.BundleReceived += receiver_BundleReceived;
                //receiver.RegisterMethod("/channel/1/stage/layer/10/file/frame");
                //receiver.RegisterMethod("/channel/1/stage/layer/10/file/fps");
                receiver.Start();

                StartCheckingPrimaryPlaying();
                //threadCheckPrimaryPlaying = new Thread(new ThreadStart(CheckForPlayStatus));
                //threadCheckPrimaryPlaying.Priority = ThreadPriority.Highest;
                //threadCheckPrimaryPlaying.Start();
            }

        }
        public void OSCStart(string ip, int port)
        {
            OSCStop();
            OscServerIp = IPAddress.Parse(ip);
            OscPort = port;
            OSCStart();

        }
        public void OSCStart(IPAddress ip, int port)
        {
            OSCStop();
            OscServerIp = ip;
            OscPort = port;
            OSCStart();
        }

        public void OSCStop()
        {
            if (receiver != null)
            {

                stopOccurence = 0;
                playOccurence = 0;

                StopCheckingPrimaryPlaying();


                // close the Reciver 
                receiver.Stop();
                receiver = null;




                _primaryPlaying = false;
                _primaryLoop = false;
                _primaryFrame = 0;

            }
        }



        protected void receiver_BundleReceived(object sender, OscBundleReceivedEventArgs e)
        {
            OscBundle b = e.Bundle;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ParseBundleMessage), b);
            //ParseBundleMessage(e.Bundle);
        }

        protected void receiver_MessageReceived(object sender, Bespoke.Common.Osc.OscMessageReceivedEventArgs e)
        {

            //if (e.Message.Data.Count > 0)
            //{
            //    rslt += DateTime.Now.ToString("HH:mm:ss:fff") + " | " + e.Message.Address + "-" + e.Message.Data[0].ToString() + Environment.NewLine;
            //}

        }

        protected void ParseBundleMessage(Object b)
        {
            mutMessage.WaitOne();
            OscBundle bTmp = (OscBundle)b;
            foreach (OscMessage m in bTmp.Messages)
            {
                //Console.WriteLine(" Value: " + m);
                OscMessage _m = m;
                if (m.Data.Count > 0)
                {
                    //ParseOCSMessage(_m);
                    //Thread th = new Thread(new ParameterizedThreadStart(ParseOCSMessage));
                    //th.Start(_m);

                    ThreadPool.QueueUserWorkItem(new WaitCallback(ParseOCSMessage), m);

                }
            }
            mutMessage.ReleaseMutex();
        }

        protected void ParseOCSMessage(Object _m)
        {

            OscMessage m = (OscMessage)_m;

            //Console.WriteLine(m.Address);
            string chId = "0";
            string layerId = "0";
            try
            {
                string[] arr = Regex.Split(m.Address, "/");
                if (m.Address.StartsWith("/channel/") && arr.Length > 3)
                {
                    //Channel ID is always in 2nd position
                    chId = arr[2];

                    if (m.Address.Contains("/stage/layer/") && arr.Length > 7)
                    {
                        layerId = arr[5];

                        if (arr[6] == "file" || arr[arr.Length - 1] == "loop")
                        {
                            //FFMPEG Producer
                            ParseFFMPEGProducerMessages(m, chId, layerId);
                            return;
                        }

                        if (arr[6] == "host" || arr[6] == "buffer")
                        {
                            //FLASH PRODUCER
                            ParseFlashProducerMessages(m, chId, layerId);
                            return;
                        }
                        ParseGeneralStateMessages(m, chId, layerId);

                    }
                    //If the message address is not begin with /stage/layer message, then it's a mixer message    
                    else if (arr[3] == "mixer" && arr[4] == "audio")
                    {
                        //MIXER MESSAGES
                        ParseMixerMessages(m, chId);
                        return;
                    }

                    else
                    {
                        ParseChannelMessages(m, chId);

                    }

                }

            }
            catch
            {

            }
            OnMessageReceived(new OscMessageReceivedEventArgs(m));
        }


        protected void ParseGeneralStateMessages(OscMessage m, string chId, string layerId)
        {
            try
            {
                Int64 valInt1;
                float val1f;
                float val2f;
                string val1stg;
                bool val1bool;
                //Console.WriteLine(m.Address);
                //Console.WriteLine("Parse General State Message." + chId + "-" + layerId + " | " + m.ToString());
                if (m.Address.EndsWith("frame"))
                {
                    valInt1 = (Int64)m.Data[0];
                    OnFrameChanged(new FrameEventArgs(chId, layerId, valInt1, m));
                    return;
                }

                if (m.Address.EndsWith("profiler/time"))
                {
                    val1f = (float)m.Data[0];
                    val2f = (float)m.Data[1];
                    OnProfilerTimeChanged(new ProfilerTimeEventArgs(chId, layerId, val1f, val2f, m));
                    return;
                }

                if (m.Address.EndsWith("time") && !m.Address.Contains("background") && !m.Address.Contains("profiler"))
                {
                    val1f = (float)m.Data[0];
                    OnActiveTimeChanged(new ActiveTimeEventArgs(chId, layerId, val1f, m));
                    return;
                }

                if (m.Address.EndsWith("type"))
                {
                    val1stg = (string)m.Data[0];
                    OnTypeChanged(new TypeEventArgs(chId, layerId, val1stg, m));
                    return;
                }

                if (m.Address.EndsWith("background/type"))
                {
                    val1stg = (string)m.Data[0];
                    OnBackgroundTypeChanged(new TypeEventArgs(chId, layerId, val1stg, m));
                    return;
                }

                if (m.Address.EndsWith("paused"))
                {
                    val1bool = (bool)m.Data[0];
                    OnPausedChanged(new PausedEventArgs(chId, layerId, val1bool, m));
                    return;
                }




            }
            catch { }

        }

        protected void ParseFFMPEGProducerMessages(OscMessage m, string chId, string layerId)
        {
            //FFMPEG PRODUCER
            Int64 valInt1;
            Int64 valInt2;
            float val1f;
            float val2f;
            string val1stg;
            bool val1bool;

            //Console.WriteLine(m.Address);

            if (m.Address.EndsWith("frame"))
            {
                valInt1 = (Int64)m.Data[0];
                valInt2 = (Int64)m.Data[1];
                if (chId == PrimaryChannel && PrimaryLayer == layerId)
                {
                    if (valInt1 != PrimaryFrame)
                    {
                        _primaryFrame = valInt1;
                        if (!firstFrameReceived)
                        {
                            lastFrame = _primaryFrame;
                            firstFrameReceived = true;
                        }
                    }
                }
                OnFFmpegFrameChanged(new FileFrameEventArgs(chId, layerId, valInt1, valInt2, m));
                return;
            }

            if (m.Address.EndsWith("time"))
            {
                val1f = (float)m.Data[0];
                val2f = (float)m.Data[1];
                OnFFmpegTimeChanged(new FileTimeEventArgs(chId, layerId, val1f, val2f, m));
                return;
            }


            if (m.Address.EndsWith("fps"))
            {
                val1f = (float)m.Data[0];
                OnFFmpegFpsChanged(new FpsEventArgs(chId, layerId, val1f, m));
                return;
            }

            if (m.Address.EndsWith("path"))
            {
                val1stg = (string)m.Data[0];
                OnFFmpegPathChanged(new PathEventArgs(chId, layerId, val1stg, m));
                return;
            }

            if (m.Address.EndsWith("video/width"))
            {
                valInt1 = (Int64)m.Data[0];
                OnFFmpegWidthChanged(new WidthEventArgs(chId, layerId, valInt1, m));
                return;
            }

            if (m.Address.EndsWith("video/height"))
            {
                valInt1 = (Int64)m.Data[0];
                OnFFmpegHeightChanged(new HeightEventArgs(chId, layerId, valInt1, m));
                return;
            }

            if (m.Address.EndsWith("video/field"))
            {
                val1stg = (string)m.Data[0];
                OnFFmpegFieldChanged(new FieldEventArgs(chId, layerId, val1stg, m));
                return;
            }

            if (m.Address.EndsWith("video/codec"))
            {
                val1stg = (string)m.Data[0];
                OnFFmpegCodecChanged(new VideoCodecEventArgs(chId, layerId, val1stg, m));
                return;
            }

            if (m.Address.EndsWith("audio/sample-rate"))
            {
                valInt1 = (Int64)m.Data[0];
                OnFFmpegSampleRateChanged(new SampleRateEventArgs(chId, layerId, valInt1, m));
                return;
            }

            if (m.Address.EndsWith("audio/channels"))
            {
                valInt1 = (Int64)m.Data[0];
                OnFFmpegAudioChannelsChanged(new AudioChannelsEventArgs(chId, layerId, valInt1, m));
                return;
            }

            if (m.Address.EndsWith("audio/format"))
            {
                val1stg = (string)m.Data[0];
                OnFFmpegAudioFormatChanged(new AudioFormatEventArgs(chId, layerId, val1stg, m));
                return;
            }

            if (m.Address.EndsWith("audio/codec"))
            {
                val1stg = (string)m.Data[0];
                OnFFmpegAudioFormatChanged(new AudioFormatEventArgs(chId, layerId, val1stg, m));
                return;
            }

            if (m.Address.EndsWith("loop"))
            {
                val1bool = (bool)m.Data[0];
                if (chId == PrimaryChannel && PrimaryLayer == layerId)
                {
                    _primaryLoop = val1bool;
                }
                OnFFmpegLoopChanged(new LoopEventArgs(chId, layerId, val1bool, m));
                return;
            }

        }

        protected void ParseFlashProducerMessages(OscMessage m, string chId, string layerId)
        {
            //FLASH PRODUCER
            Int64 valInt1;
            float val1f;
            string val1stg;


            //Console.WriteLine(m.Address);

            if (m.Address.EndsWith("fps"))
            {
                val1f = (Int64)m.Data[0];
                OnFlashFpsChanged(new FpsEventArgs(chId, layerId, val1f, m));
                return;
            }

            if (m.Address.EndsWith("path"))
            {
                val1stg = (string)m.Data[0];
                OnFlashPathChanged(new PathEventArgs(chId, layerId, val1stg, m));
                return;
            }

            if (m.Address.EndsWith("width"))
            {
                valInt1 = (Int64)m.Data[0];
                OnFlashWidthChanged(new WidthEventArgs(chId, layerId, valInt1, m));
                return;
            }

            if (m.Address.EndsWith("height"))
            {
                valInt1 = (Int64)m.Data[0];
                OnFlashHeightChanged(new HeightEventArgs(chId, layerId, valInt1, m));
                return;
            }

            if (m.Address.EndsWith("buffer"))
            {
                valInt1 = (Int64)m.Data[0];
                OnFlashBufferChanged(new BufferEventArgs(chId, layerId, valInt1, m));
                return;
            }
        }

        protected void ParseMixerMessages(OscMessage m, string chId)
        {
            //MIXER MESSAGES
            Int64 valInt1;
            float val1f;

            //Console.WriteLine(m.Address);

            if (m.Address.EndsWith("nb_channels"))
            {
                valInt1 = (Int64)m.Data[0];
                OnAudioMixerChannelsChanged(new AudioMixerChannelsEventArgs(chId, valInt1, m));
                return;
            }

            if (m.Address.EndsWith("dBFS"))
            {
                val1f = (float)m.Data[0];
                OnAudioMixerDbfsChanged(new AudioMixerDbfsEventArgs(chId, val1f, m));
                return;
            }

        }

        protected void ParseChannelMessages(OscMessage m, string chId)
        {
            Int64 valInt1;
            Int64 valInt2;
            float val1f;
            float val2f;
            string val1stg;
            Int64 outNum = 0;
            string[] arr;
            if (m.Address.EndsWith("format"))
            {
                val1stg = (string)m.Data[0];
                OnChannelFormatChanged(new ChannelFormatEventArgs(chId, val1stg, m));
                return;
            }

            if (m.Address.EndsWith("profiler/time"))
            {
                val1f = (float)m.Data[0];
                val2f = (float)m.Data[1];
                OnChannelProfilerTimeChanged(new ChannelProfilerTimeEventArgs(chId, val1f, val2f, m));
                return;
            }

            if (m.Address.Contains("output/port/"))
            {
                arr = Regex.Split(m.Address, "/");
                if (arr != null && arr.Length >= 6 && Int64.TryParse(arr[5], out outNum))
                {

                    if (m.Address.EndsWith("type"))
                    {
                        val1stg = (string)m.Data[0];
                        OnChannelOutputTypeChanged(new ChannelOutputTypeEventArgs(chId, val1stg, outNum, m));
                        return;
                    }

                    if (m.Address.EndsWith("frame"))
                    {
                        valInt1 = (Int64)m.Data[0];
                        valInt2 = (Int64)m.Data[1];

                        OnChannelFrameGenerated(new ChannelFrameGeneratedEventArgs(chId, valInt1, valInt2, outNum, m));
                    }

                }
            }


        }

        #region FUNCTIONALITY TO CHECK PLAYING STATUS ON PRIMARY EVENT LAYER (FOR PLAYOUT SOFTWARE)
        private int stopOccurence = 0;
        private int playOccurence = 0;

        //private Stopwatch sw = new Stopwatch();

        private System.Threading.Timer timerCheckPrimaryPlaying;
        private AutoResetEvent autoEvent = new AutoResetEvent(false);

        public void StartCheckingPrimaryPlaying()
        {
            if (timerCheckPrimaryPlaying != null)
            {
                StopCheckingPrimaryPlaying();
                StartCheckingPrimaryPlaying();
                //sw.Start();
            }
            else
            {
                timerCheckPrimaryPlaying = new Timer(CheckForPlayStatus, autoEvent, 200, PlayCheckIntervall);
            }
        }

        public void StopCheckingPrimaryPlaying()
        {
            if (timerCheckPrimaryPlaying != null)
            {
                //sw.Stop();
                timerCheckPrimaryPlaying.Change(0, 0);
                autoEvent.WaitOne();
                timerCheckPrimaryPlaying.Dispose();
                timerCheckPrimaryPlaying = null;
            }
        }
        private void CheckForPlayStatus(Object stateInfo)
        {
            //Console.WriteLine("Elapsed timer: " + sw.ElapsedMilliseconds.ToString() + " Intervall setted: " + PlayCheckIntervall.ToString());
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            if (lastFrame == PrimaryFrame || lastFrame == -1)
            {
                stopOccurence++;
                if (stopOccurence > 2 && PrimaryPlaying)
                {
                    Console.WriteLine("Stop");
                    _primaryPlaying = false;
                    OnPrimaryPlayingChanged();
                }
                playOccurence = 0;
            }
            else
            {
                playOccurence++;
                if (playOccurence > 2 && !PrimaryPlaying)
                {
                    Console.WriteLine("Play");
                    _primaryPlaying = true;
                    OnPrimaryPlayingChanged();
                    OnPrimaryClipChanged();
                }
                stopOccurence = 0;

                if (lastFrame > PrimaryFrame && !_primaryLoop)
                {
                    OnPrimaryClipChanged();

                }
            }


            lastFrame = PrimaryFrame;
            autoEvent.Set();
            //sw.Restart();

        }
        #endregion

        #region OCS MESSAGE HANDLER

        public delegate void MessageReceivedEventHandler(object sender, OscMessageReceivedEventArgs e);
        private event MessageReceivedEventHandler _MessageReceived;
        public event MessageReceivedEventHandler MessageReceived
        {
            add
            {
                _MessageReceived -= value;
                _MessageReceived += value;
            }
            remove
            {
                _MessageReceived -= value;
            }
        }
        protected void OnMessageReceived(OscMessageReceivedEventArgs e)
        {

            if (_MessageReceived != null)
            {
                _MessageReceived(this, e);
            }
        }
        #endregion

        #region GENERAL STATE MESSAGES HANDLER
        //GENERAL STATE EVENT HANDLER
        public delegate void FrameChangedEventHandler(object sender, FrameEventArgs e);
        public delegate void TypeChangedEventHandler(object sender, TypeEventArgs e);
        public delegate void BackgroundTypeChangedEventHandler(object sender, TypeEventArgs e);
        public delegate void ProfilerTimeChangedEventHandler(object sender, ProfilerTimeEventArgs e);
        public delegate void ActiveTimeChangedEventChangedHandler(object sender, ActiveTimeEventArgs e);
        public delegate void PausedEventChangedHandler(object sender, PausedEventArgs e);
        public delegate void PlayingEventChangedHandler(object sender, PlayingEventArgs e);

        private event FrameChangedEventHandler _NewFrame;
        public event FrameChangedEventHandler NewFrame
        {
            add
            {
                _NewFrame -= value;
                _NewFrame += value;
            }
            remove
            {
                _NewFrame -= value;
            }
        }
        protected void OnFrameChanged(FrameEventArgs e)
        {
            if (_NewFrame != null)
            {
                _NewFrame(this, e);
            }
        }

        public event TypeChangedEventHandler _TypeChanged;
        public event TypeChangedEventHandler TypeChanged
        {
            add
            {
                _TypeChanged -= value;
                _TypeChanged += value;
            }
            remove
            {
                _TypeChanged -= value;
            }
        }
        protected void OnTypeChanged(TypeEventArgs e)
        {
            if (_TypeChanged != null)
            {
                _TypeChanged(this, e);
            }
        }

        private event BackgroundTypeChangedEventHandler _BackgroundTypeChanged;
        public event BackgroundTypeChangedEventHandler BackgroundTypeChanged
        {
            add
            {
                _BackgroundTypeChanged -= value;
                _BackgroundTypeChanged += value;
            }
            remove
            {
                _BackgroundTypeChanged -= value;
            }
        }
        protected void OnBackgroundTypeChanged(TypeEventArgs e)
        {
            if (_BackgroundTypeChanged != null)
            {
                _BackgroundTypeChanged(this, e);
            }
        }

        private event ProfilerTimeChangedEventHandler _ProfilerTimeChanged;
        public event ProfilerTimeChangedEventHandler ProfilerTimeChanged
        {
            add
            {
                _ProfilerTimeChanged -= value;
                _ProfilerTimeChanged += value;
            }
            remove
            {
                _ProfilerTimeChanged -= value;
            }
        }
        protected void OnProfilerTimeChanged(ProfilerTimeEventArgs e)
        {
            if (_ProfilerTimeChanged != null)
            {
                _ProfilerTimeChanged(this, e);
            }
        }

        private event ActiveTimeChangedEventChangedHandler _ActiveTimeChanged;
        public event ActiveTimeChangedEventChangedHandler ActiveTimeChanged
        {
            add
            {
                _ActiveTimeChanged -= value;
                _ActiveTimeChanged += value;
            }
            remove
            {
                _ActiveTimeChanged -= value;
            }
        }
        protected void OnActiveTimeChanged(ActiveTimeEventArgs e)
        {
            if (_ActiveTimeChanged != null)
            {
                _ActiveTimeChanged(this, e);
            }
        }

        private event PausedEventChangedHandler _PausedChanged;
        public event PausedEventChangedHandler PausedChanged
        {
            add
            {
                _PausedChanged -= value;
                _PausedChanged += value;
            }
            remove
            {
                _PausedChanged -= value;
            }
        }
        protected void OnPausedChanged(PausedEventArgs e)
        {
            if (_PausedChanged != null)
            {
                _PausedChanged(this, e);
            }
        }


        private event EventHandler _PrimaryClipChanged;
        public event EventHandler PrimaryClipChanged
        {
            add
            {
                _PrimaryClipChanged -= value;
                _PrimaryClipChanged += value;
            }
            remove
            {
                _PrimaryClipChanged -= value;
            }
        }
        protected void OnPrimaryClipChanged()
        {
            if (_PrimaryClipChanged != null)
            {
                _PrimaryClipChanged(this, EventArgs.Empty);
            }
        }

        private event PlayingEventChangedHandler _PrimaryPlayingChanged;
        public event PlayingEventChangedHandler PrimaryPlayingChanged
        {
            add
            {
                _PrimaryPlayingChanged -= value;
                _PrimaryPlayingChanged += value;
            }
            remove
            {
                _PrimaryPlayingChanged -= value;
            }
        }
        protected void OnPrimaryPlayingChanged()
        {
            if (_PrimaryPlayingChanged != null)
            {
                _PrimaryPlayingChanged(this, new PlayingEventArgs(PrimaryChannel, PrimaryLayer, _primaryPlaying));
            }
        }

        #endregion

        #region FFMPEG PRODUCER MESSAGES HANDLER
        //FFMPEG PRODUCER HANDLER
        public delegate void FFmpegTimeEventChangedHandler(object sender, FileTimeEventArgs e);
        public delegate void FFmpegFrameChangedEventHandler(object sender, FileFrameEventArgs e);
        public delegate void FFmpegFpsChangedEventHandler(object sender, FpsEventArgs e);
        public delegate void FFmpegPathChangedEventHandler(object sender, PathEventArgs e);
        public delegate void FFmpegWidthChangedEventHandler(object sender, WidthEventArgs e);
        public delegate void FFmpegHeightChangedEventHandler(object sender, HeightEventArgs e);
        public delegate void FFmpegFieldChangedEventHandler(object sender, FieldEventArgs e);
        public delegate void FFmpegCodecChangedEventHandler(object sender, VideoCodecEventArgs e);
        public delegate void FFmpegSampleRateChangedEventHandler(object sender, SampleRateEventArgs e);
        public delegate void FFmpegAudioChannelsChangedEventHandler(object sender, AudioChannelsEventArgs e);
        public delegate void FFmpegAudioFormatChangedEventHandler(object sender, AudioFormatEventArgs e);
        public delegate void FFmpegAudioCodecChangedEventHandler(object sender, AudioCodecEventArgs e);
        public delegate void FFmpegLoopChangedEventHandler(object sender, LoopEventArgs e);



        private FFmpegTimeEventChangedHandler _FFmpegTimeChanged;
        public event FFmpegTimeEventChangedHandler FFmpegTimeChanged
        {
            add
            {
                _FFmpegTimeChanged -= value;
                _FFmpegTimeChanged += value;
            }
            remove
            {
                _FFmpegTimeChanged -= value;
            }
        }
        protected void OnFFmpegTimeChanged(FileTimeEventArgs e)
        {
            if (_FFmpegTimeChanged != null)
            {
                _FFmpegTimeChanged(this, e);
            }
        }

        private event FFmpegFrameChangedEventHandler _FFmpegFrameChanged;
        public event FFmpegFrameChangedEventHandler FFmpegFrameChanged
        {
            add
            {
                _FFmpegFrameChanged -= value;
                _FFmpegFrameChanged += value;
            }
            remove
            {
                _FFmpegFrameChanged -= value;
            }
        }
        protected void OnFFmpegFrameChanged(FileFrameEventArgs e)
        {
            if (_FFmpegFrameChanged != null)
            {
                _FFmpegFrameChanged(this, e);
            }
        }

        private event FFmpegFpsChangedEventHandler _FFmpegFpsChanged;
        public event FFmpegFpsChangedEventHandler FFmpegFpsChanged
        {
            add
            {
                _FFmpegFpsChanged -= value;
                _FFmpegFpsChanged += value;
            }
            remove
            {
                _FFmpegFpsChanged -= value;
            }
        }
        protected void OnFFmpegFpsChanged(FpsEventArgs e)
        {
            if (_FFmpegFpsChanged != null)
            {
                _FFmpegFpsChanged(this, e);
            }
        }

        private event FFmpegPathChangedEventHandler _FFmpegPathChanged;
        public event FFmpegPathChangedEventHandler FFmpegPathChanged
        {
            add
            {
                _FFmpegPathChanged -= value;
                _FFmpegPathChanged += value;
            }
            remove
            {
                _FFmpegPathChanged -= value;
            }
        }
        protected void OnFFmpegPathChanged(PathEventArgs e)
        {
            if (_FFmpegPathChanged != null)
            {
                _FFmpegPathChanged(this, e);
            }

        }


        private event FFmpegWidthChangedEventHandler _FFmpegWidthChanged;
        public event FFmpegWidthChangedEventHandler FFmpegWidthChanged
        {
            add
            {
                _FFmpegWidthChanged -= value;
                _FFmpegWidthChanged += value;
            }
            remove
            {
                _FFmpegWidthChanged -= value;
            }
        }
        protected void OnFFmpegWidthChanged(WidthEventArgs e)
        {
            if (_FFmpegWidthChanged != null)
            {
                _FFmpegWidthChanged(this, e);
            }

        }

        private event FFmpegHeightChangedEventHandler _FFmpegHeightChanged;
        public event FFmpegHeightChangedEventHandler FFmpegHeightChanged
        {
            add
            {
                _FFmpegHeightChanged -= value;
                _FFmpegHeightChanged += value;
            }
            remove
            {
                _FFmpegHeightChanged -= value;
            }
        }
        protected void OnFFmpegHeightChanged(HeightEventArgs e)
        {
            if (_FFmpegHeightChanged != null)
            {
                _FFmpegHeightChanged(this, e);
            }
        }


        private event FFmpegFieldChangedEventHandler _FFmpegFieldChanged;
        public event FFmpegFieldChangedEventHandler FFmpegFieldChanged
        {
            add
            {
                _FFmpegFieldChanged -= value;
                _FFmpegFieldChanged += value;
            }
            remove
            {
                _FFmpegFieldChanged -= value;
            }
        }
        protected void OnFFmpegFieldChanged(FieldEventArgs e)
        {
            if (_FFmpegFieldChanged != null)
            {
                _FFmpegFieldChanged(this, e);
            }
        }

        private event FFmpegCodecChangedEventHandler _FFmpegCodecChanged;
        public event FFmpegCodecChangedEventHandler FFmpegCodecChanged
        {
            add
            {
                _FFmpegCodecChanged -= value;
                _FFmpegCodecChanged += value;
            }
            remove
            {
                _FFmpegCodecChanged -= value;
            }
        }
        protected void OnFFmpegCodecChanged(VideoCodecEventArgs e)
        {
            if (_FFmpegCodecChanged != null)
            {
                _FFmpegCodecChanged(this, e);
            }
        }

        private event FFmpegSampleRateChangedEventHandler _FFmepgSampleRateChanged;
        public event FFmpegSampleRateChangedEventHandler FFmepgSampleRateChanged
        {
            add
            {
                _FFmepgSampleRateChanged -= value;
                _FFmepgSampleRateChanged += value;
            }
            remove
            {
                _FFmepgSampleRateChanged -= value;
            }
        }
        protected void OnFFmpegSampleRateChanged(SampleRateEventArgs e)
        {
            if (_FFmepgSampleRateChanged != null)
            {
                _FFmepgSampleRateChanged(this, e);
            }
        }

        private event FFmpegAudioChannelsChangedEventHandler _FFmpegAudioChannelsChanged;
        public event FFmpegAudioChannelsChangedEventHandler FFmpegAudioChannelsChanged
        {
            add
            {
                _FFmpegAudioChannelsChanged -= value;
                _FFmpegAudioChannelsChanged += value;
            }
            remove
            {
                _FFmpegAudioChannelsChanged -= value;
            }
        }
        protected void OnFFmpegAudioChannelsChanged(AudioChannelsEventArgs e)
        {
            if (_FFmpegAudioChannelsChanged != null)
            {
                _FFmpegAudioChannelsChanged(this, e);
            }
        }

        private event FFmpegAudioFormatChangedEventHandler _FFmpegAudioFormatChanged;
        public event FFmpegAudioFormatChangedEventHandler FFmpegAudioFormatChanged
        {
            add
            {
                _FFmpegAudioFormatChanged -= value;
                _FFmpegAudioFormatChanged += value;
            }
            remove
            {
                _FFmpegAudioFormatChanged -= value;
            }
        }
        protected void OnFFmpegAudioFormatChanged(AudioFormatEventArgs e)
        {
            if (_FFmpegAudioFormatChanged != null)
            {
                _FFmpegAudioFormatChanged(this, e);
            }
        }

        private event FFmpegLoopChangedEventHandler _FFmpegLoopChanged;
        public event FFmpegLoopChangedEventHandler FFmpegLoopChanged
        {
            add
            {
                _FFmpegLoopChanged -= value;
                _FFmpegLoopChanged += value;
            }
            remove
            {
                _FFmpegLoopChanged -= value;
            }
        }
        protected void OnFFmpegLoopChanged(LoopEventArgs e)
        {
            if (_FFmpegLoopChanged != null)
            {
                _FFmpegLoopChanged(this, e);
            }
        }

        #endregion

        #region FLASH PRODUCER MESSAGES HANDLER
        //FLASH PRODUCER
        public delegate void FlashPathChangedEventHandler(object sender, PathEventArgs e);
        public delegate void FlashWidthChangedEventHandler(object sender, WidthEventArgs e);
        public delegate void FlashHeightChangedEventHandler(object sender, HeightEventArgs e);
        public delegate void FlashFpsChangedEventHandler(object sender, FpsEventArgs e);
        public delegate void FlashBufferChangedEventHandler(object sender, BufferEventArgs e);

        private event FlashPathChangedEventHandler _FlashPathChanged;
        public event FlashPathChangedEventHandler FlashPathChanged
        {
            add
            {
                _FlashPathChanged -= value;
                _FlashPathChanged += value;
            }
            remove
            {
                _FlashPathChanged -= value;
            }
        }
        protected void OnFlashPathChanged(PathEventArgs e)
        {
            if (_FlashPathChanged != null)
            {
                _FlashPathChanged(this, e);
            }
        }

        private event FlashWidthChangedEventHandler _FlashWidthChanged;
        public event FlashWidthChangedEventHandler FlashWidthChanged
        {
            add
            {
                _FlashWidthChanged -= value;
                _FlashWidthChanged += value;
            }
            remove
            {
                _FlashWidthChanged -= value;
            }
        }
        protected void OnFlashWidthChanged(WidthEventArgs e)
        {
            if (_FlashWidthChanged != null)
            {
                _FlashWidthChanged(this, e);
            }
        }

        private event FlashHeightChangedEventHandler _FlashHeightChanged;
        public event FlashHeightChangedEventHandler FlashHeightChanged
        {
            add
            {
                _FlashHeightChanged -= value;
                _FlashHeightChanged += value;
            }
            remove
            {
                _FlashHeightChanged -= value;
            }
        }
        protected void OnFlashHeightChanged(HeightEventArgs e)
        {
            if (_FlashHeightChanged != null)
            {
                _FlashHeightChanged(this, e);
            }
        }

        private event FlashFpsChangedEventHandler _FlashFpsChanged;
        public event FlashFpsChangedEventHandler FlashFpsChanged
        {
            add
            {
                _FlashFpsChanged -= value;
                _FlashFpsChanged += value;
            }
            remove
            {
                _FlashFpsChanged -= value;
            }
        }
        protected void OnFlashFpsChanged(FpsEventArgs e)
        {
            if (_FlashFpsChanged != null)
            {
                _FlashFpsChanged(this, e);
            }
        }


        private event FlashBufferChangedEventHandler _FlashBufferChanged;
        public event FlashBufferChangedEventHandler FlashBufferChanged
        {
            add
            {
                _FlashBufferChanged -= value;
                _FlashBufferChanged += value;
            }
            remove
            {
                _FlashBufferChanged -= value;
            }
        }
        protected void OnFlashBufferChanged(BufferEventArgs e)
        {
            if (_FlashBufferChanged != null)
            {
                _FlashBufferChanged(this, e);
            }
        }
        #endregion

        #region MIXER MESSAGES HANDLER
        //MIXER MESSAGES
        public delegate void AudioMixerChannelsChangedEventHandler(object sender, AudioMixerChannelsEventArgs e);
        public delegate void AudioMixerDbfsChangedEventHandler(object sender, AudioMixerDbfsEventArgs e);

        private event AudioMixerChannelsChangedEventHandler _AudioMixerChannelsChanged;
        public event AudioMixerChannelsChangedEventHandler AudioMixerChannelsChanged
        {
            add
            {
                _AudioMixerChannelsChanged -= value;
                _AudioMixerChannelsChanged += value;
            }
            remove
            {
                _AudioMixerChannelsChanged -= value;
            }
        }
        protected void OnAudioMixerChannelsChanged(AudioMixerChannelsEventArgs e)
        {
            if (_AudioMixerChannelsChanged != null)
            {
                _AudioMixerChannelsChanged(this, e);
            }
        }


        private event AudioMixerDbfsChangedEventHandler _AudioMixerDbfsChanged;
        public event AudioMixerDbfsChangedEventHandler AudioMixerDbfsChanged
        {
            add
            {
                _AudioMixerDbfsChanged -= value;
                _AudioMixerDbfsChanged += value;
            }
            remove
            {
                _AudioMixerDbfsChanged -= value;
            }
        }
        protected void OnAudioMixerDbfsChanged(AudioMixerDbfsEventArgs e)
        {
            if (_AudioMixerDbfsChanged != null)
            {
                _AudioMixerDbfsChanged(this, e);
            }
        }

        #endregion

        #region CHANNEL MESSAGES HANDLER
        public delegate void ChannelFormatChangedEventHandler(object sender, ChannelFormatEventArgs e);
        public delegate void ChannelProfilerTimeChangedEventHandler(object sender, ChannelProfilerTimeEventArgs e);
        public delegate void ChannelOutputTypeChangedEventHandler(object sender, ChannelOutputTypeEventArgs e);
        public delegate void ChannelFrameGeneratedChangedEventHandler(object sender, ChannelFrameGeneratedEventArgs e);

        private event ChannelFormatChangedEventHandler _ChannelFormatChanged;
        public event ChannelFormatChangedEventHandler ChannelFormatChanged
        {
            add
            {
                _ChannelFormatChanged -= value;
                _ChannelFormatChanged += value;
            }
            remove
            {
                _ChannelFormatChanged -= value;
            }
        }
        protected void OnChannelFormatChanged(ChannelFormatEventArgs e)
        {
            if (_ChannelFormatChanged != null)
            {
                _ChannelFormatChanged(this, e);
            }

        }

        private event ChannelProfilerTimeChangedEventHandler _ChannelProfilerTimeChanged;
        public event ChannelProfilerTimeChangedEventHandler ChannelProfilerTimeChanged
        {
            add
            {
                _ChannelProfilerTimeChanged -= value;
                _ChannelProfilerTimeChanged += value;
            }
            remove
            {
                _ChannelProfilerTimeChanged -= value;
            }
        }
        protected void OnChannelProfilerTimeChanged(ChannelProfilerTimeEventArgs e)
        {
            if (_ChannelProfilerTimeChanged != null)
            {
                _ChannelProfilerTimeChanged(this, e);
            }

        }

        private event ChannelOutputTypeChangedEventHandler _ChannelOutputTypeChanged;
        public event ChannelOutputTypeChangedEventHandler ChannelOutputTypeChanged
        {
            add
            {
                _ChannelOutputTypeChanged -= value;
                _ChannelOutputTypeChanged += value;
            }
            remove
            {
                _ChannelOutputTypeChanged -= value;
            }
        }
        protected void OnChannelOutputTypeChanged(ChannelOutputTypeEventArgs e)
        {
            if (_ChannelOutputTypeChanged != null)
            {
                _ChannelOutputTypeChanged(this, e);
            }

        }

        private event ChannelFrameGeneratedChangedEventHandler _ChannelFrameGenerated;
        public event ChannelFrameGeneratedChangedEventHandler ChannelFrameGenerated
        {
            add
            {
                _ChannelFrameGenerated -= value;
                _ChannelFrameGenerated += value;
            }
            remove
            {
                _ChannelFrameGenerated -= value;
            }
        }
        protected void OnChannelFrameGenerated(ChannelFrameGeneratedEventArgs e)
        {
            if (_ChannelFrameGenerated != null)
            {
                _ChannelFrameGenerated(this, e);
            }

        }
        #endregion

        public void Dispose()
        {
            this.OSCStop();
            GC.SuppressFinalize(this);
        }
    }







    public enum CasparOscVideoFieldType
    {
        Unknown, Progressive, Interlaced
    }






}
