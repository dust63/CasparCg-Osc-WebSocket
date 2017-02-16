using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System.Web.Script.Serialization;
using Newtonsoft.Json;


namespace CasparCG.Osc.net
{
    public class OscHubRelay : IDisposable
    {
        OscReceiver receiver;


        private string serverIp;
        private int serverPort;

        public string PrimaryChannel
        {

            get
            {

                return receiver != null ? receiver.PrimaryChannel : "not setted";

            }
        }
        public string PrimaryLayer
        {

            get
            {
                return receiver != null ? receiver.PrimaryLayer : "not setted";
            }
        }



        public OscHubRelay(string ip, int port, int primaryChannel = 1, int primaryLayer = 10)
        {

            serverIp = ip;
            serverPort = port;


            receiver = new OscReceiver();
            receiver.PrimaryChannel = primaryChannel.ToString();
            receiver.PrimaryLayer = primaryLayer.ToString();
            receiver.OSCStart(serverIp, serverPort);
            Logger.logInfo(string.Format("Osc Receiver on Host {0}:{1} running: {2}", serverIp, serverPort, receiver.OscIsRunning));

            receiver.PrimaryPlayingChanged += receiver_PrimaryPlayingChanged;

            receiver.ActiveTimeChanged += receiver_ActiveTimeChanged;
            receiver.AudioMixerChannelsChanged += receiver_AudioMixerChannelsChanged;
            receiver.AudioMixerDbfsChanged += receiver_AudioMixerDbfsChanged;
            receiver.BackgroundTypeChanged += receiver_BackgroundTypeChanged;
            receiver.ChannelFormatChanged += receiver_ChannelFormatChanged;
            receiver.ChannelFrameGenerated += receiver_ChannelFrameGenerated;
            receiver.ChannelOutputTypeChanged += receiver_ChannelOutputTypeChanged;
            receiver.ChannelProfilerTimeChanged += receiver_ChannelProfilerTimeChanged;
            receiver.FFmepgSampleRateChanged += receiver_FFmepgSampleRateChanged;
            receiver.FFmpegAudioChannelsChanged += receiver_FFmpegAudioChannelsChanged;
            receiver.FFmpegAudioFormatChanged += receiver_FFmpegAudioFormatChanged;
            receiver.FFmpegCodecChanged += receiver_FFmpegCodecChanged;
            receiver.FFmpegFieldChanged += receiver_FFmpegFieldChanged;
            receiver.FFmpegFpsChanged += receiver_FFmpegFpsChanged;
            receiver.FFmpegFrameChanged += receiver_FFmpegFrameChanged;
            receiver.FFmpegHeightChanged += receiver_FFmpegHeightChanged;
            receiver.FFmpegLoopChanged += receiver_FFmpegLoopChanged;
            receiver.FFmpegPathChanged += receiver_FFmpegPathChanged;
            receiver.FFmpegTimeChanged += receiver_FFmpegTimeChanged;
            receiver.FFmpegWidthChanged += receiver_FFmpegWidthChanged;
            receiver.FlashBufferChanged += receiver_FlashBufferChanged;
            receiver.FlashFpsChanged += receiver_FlashFpsChanged;
            receiver.FlashHeightChanged += receiver_FlashHeightChanged;
            receiver.FlashPathChanged += receiver_FlashPathChanged;
            receiver.FlashWidthChanged += receiver_FlashWidthChanged;
            receiver.MessageReceived += receiver_MessageReceived;
            receiver.NewFrame += receiver_NewFrame;
            receiver.PausedChanged += receiver_PausedChanged;
            receiver.PrimaryClipChanged += receiver_PrimaryClipChanged;
            receiver.ProfilerTimeChanged += receiver_ProfilerTimeChanged;
            receiver.TypeChanged += receiver_TypeChanged;


        }

        void receiver_TypeChanged(object sender, TypeEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.type(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_ProfilerTimeChanged(object sender, ProfilerTimeEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.profilerTime(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_PrimaryClipChanged(object sender, EventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.primaryClip(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_PausedChanged(object sender, PausedEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.paused(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_NewFrame(object sender, FrameEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.newFrame(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_MessageReceived(object sender, OscMessageReceivedEventArgs e)
        {
            //var rec = sender as OscReceiver;
            //var obj = new { Adress = e.Message.Address, Datas = e.Message.Data };
            //var json = JsonConvert.SerializeObject(obj);
            //var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

            //context.Clients.All.messageReceived(rec.OscServerIp.ToString(), json);
        }

        void receiver_FlashWidthChanged(object sender, WidthEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.flashWidth(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_FlashPathChanged(object sender, PathEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.flashPath(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FlashHeightChanged(object sender, HeightEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.flashHeight(rec.OscServerIp.ToString(), json);
            }
        }


        void receiver_FlashFpsChanged(object sender, FpsEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.flashFps(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FlashBufferChanged(object sender, BufferEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.flashBufferChange(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FFmpegWidthChanged(object sender, WidthEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmpegWidth(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FFmpegTimeChanged(object sender, FileTimeEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmpegTime(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FFmpegPathChanged(object sender, PathEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmpegPath(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FFmpegLoopChanged(object sender, LoopEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmpegLoop(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FFmpegHeightChanged(object sender, HeightEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmpegHeight(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FFmpegFrameChanged(object sender, FileFrameEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmpegFrame(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_FFmpegFpsChanged(object sender, FpsEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmpegFps(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_FFmpegFieldChanged(object sender, FieldEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmepgField(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_FFmpegCodecChanged(object sender, VideoCodecEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmepgCodec(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_FFmpegAudioFormatChanged(object sender, AudioFormatEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmepgAudioFormat(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_FFmpegAudioChannelsChanged(object sender, AudioChannelsEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmepgAudioChannels(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_FFmepgSampleRateChanged(object sender, SampleRateEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.ffmepgSampleRate(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_ChannelProfilerTimeChanged(object sender, ChannelProfilerTimeEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.channelProfilerTime(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_ChannelOutputTypeChanged(object sender, ChannelOutputTypeEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.channelOutputType(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_ChannelFrameGenerated(object sender, ChannelFrameGeneratedEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.channelFrameGenerated(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_ChannelFormatChanged(object sender, ChannelFormatEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.channelFormat(rec.OscServerIp.ToString(), json);
            }
        }
        void receiver_BackgroundTypeChanged(object sender, TypeEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.backgroundType(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_AudioMixerDbfsChanged(object sender, AudioMixerDbfsEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.audioMixerDbfs(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_AudioMixerChannelsChanged(object sender, AudioMixerChannelsEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.audioMixerChannels(rec.OscServerIp.ToString(), json);
            }
        }

        void receiver_ActiveTimeChanged(object sender, ActiveTimeEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                //Console.WriteLine("FFmpegFrameChanged");
                context.Clients.All.activeTimeChangedd(rec.OscServerIp.ToString(), json);
            }
        }






        void receiver_PrimaryPlayingChanged(object sender, PlayingEventArgs e)
        {
            if (UserHandler.ConnectedIds.Count() > 0)
            {
                var rec = sender as OscReceiver;
                var json = JsonConvert.SerializeObject(e);
                var context = GlobalHost.ConnectionManager.GetHubContext<OscHub>();

                context.Clients.All.primaryPlaying(rec.OscServerIp.ToString(), json);
            }
        }




        private void Send(string name, dynamic message)
        {

        }

        public void Dispose()
        {
            Logger.logInfo(string.Format("Disposing Osc Receiver on Host {0}:{1}", serverIp, serverPort));
            receiver.OSCStop();
            GC.SuppressFinalize(this);
        }
    }
}
