using System;
using Bespoke.Common.Osc;
using Newtonsoft.Json;

namespace CasparCG.Osc.net

{


    public class OscMessageReceivedEventArgs : Bespoke.Common.Osc.OscMessageReceivedEventArgs
    {
        public OscMessageReceivedEventArgs(Bespoke.Common.Osc.OscMessage m):base(m)
        {
           
        }
    }

    public class ChannelEventArgs : EventArgs
    {

        public string Channel { get; set; }

        public string Host
        {
            get
            {
                return OscMessage == null ? "" : OscMessage.SourceEndPoint.Address.ToString();

            }
        }
        public int Port
        {
            get
            {
                return OscMessage == null ? -1 : OscMessage.SourceEndPoint.Port;

            }
        }
        public string Address
        {
            get
            {
                return OscMessage == null ? "" :  OscMessage.Address;

            }
        }

        [JsonIgnore]
        public OscMessage OscMessage { get; set; }

    }

    public class StageEventArgs : ChannelEventArgs
    {

        public string Layer { get; set; }
        

    }

    public class FrameEventArgs : StageEventArgs
    {

        public Int64 Frame { get; set; }
        public FrameEventArgs(string ch, string layer, Int64 value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Frame = value;
            this.OscMessage = m;
        }
    }

    public class TypeEventArgs : StageEventArgs
    {

        public string Type { get; set; }
        public TypeEventArgs(string ch, string layer, string value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Type = value;
            this.OscMessage = m;
        }
    }

    public class ActiveTimeEventArgs : StageEventArgs
    {

        public float Time { get; set; }
        public ActiveTimeEventArgs(string ch, string layer, float value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Time = value;
            this.OscMessage = m;
        }
    }

    public class FileFrameEventArgs : StageEventArgs
    {

        public long TotalTime { get; set; }
        public long ElapsedTime { get; set; }
        public FileFrameEventArgs(string ch, string layer, long elapsedTime, long totTime, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.TotalTime = totTime;
            this.ElapsedTime = elapsedTime;
            this.OscMessage = m;
        }
    }

    public class ProfilerTimeEventArgs : StageEventArgs
    {

        public float ActualTime { get; set; }
        public float ExpectedTime { get; set; }
        public ProfilerTimeEventArgs(string ch, string layer, float actValue, float expValue, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.ActualTime = actValue;
            this.ExpectedTime = expValue;
            this.OscMessage = m;

        }
    }

    public class PlayingEventArgs : StageEventArgs
    {

        public bool Playing { get; set; }
        public PlayingEventArgs(string ch, string layer, bool value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Playing = value;
            this.OscMessage = m;
        }
    }

    public class PausedEventArgs : StageEventArgs
    {

        public bool Paused { get; set; }
        public PausedEventArgs(string ch, string layer, bool value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Paused = value;
            this.OscMessage = m;
        }
    }

    public class FpsEventArgs : StageEventArgs
    {

        public float Fps { get; set; }
        public FpsEventArgs(string ch, string layer, float value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Fps = value;
            this.OscMessage = m;
        }
    }

    public class PathEventArgs : StageEventArgs
    {

        public string Path { get; set; }
        public PathEventArgs(string ch, string layer, string value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Path = value;
            this.OscMessage = m;
        }
    }

    public class WidthEventArgs : StageEventArgs
    {

        public Int64 Width { get; set; }
        public WidthEventArgs(string ch, string layer, Int64 value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Width = value;
            this.OscMessage = m;
        }
    }

    public class HeightEventArgs : StageEventArgs
    {

        public Int64 Height { get; set; }
        public HeightEventArgs(string ch, string layer, Int64 value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Height = value;
            this.OscMessage = m;
        }
    }

    public class FieldEventArgs : StageEventArgs
    {

        public CasparOscVideoFieldType Field { get; set; }
        public FieldEventArgs(string ch, string layer, string value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            switch (value)
            {
                case "progressive":
                    Field = CasparOscVideoFieldType.Progressive;
                    break;
                case "interlaced":
                    Field = CasparOscVideoFieldType.Interlaced;
                    break;
            }

        }
    }

    public class FileTimeEventArgs : StageEventArgs
    {

        public float Elapsed { get; set; }
        public float Total { get; set; }

        public FileTimeEventArgs(string ch, string layer, float elapsedValue, float totValue, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Elapsed = elapsedValue;
            this.Total = totValue;
            this.OscMessage = m;
        }
    }

    public class VideoCodecEventArgs : StageEventArgs
    {

        public string Codec { get; set; }
        public VideoCodecEventArgs(string ch, string layer, string value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Codec = value;
            this.OscMessage = m;
        }
    }

    public class SampleRateEventArgs : StageEventArgs
    {

        public Int64 SampleRate { get; set; }
        public SampleRateEventArgs(string ch, string layer, Int64 value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.SampleRate = value;
            this.OscMessage = m;
        }
    }

    public class AudioChannelsEventArgs : StageEventArgs
    {

        public Int64 AudioChannels { get; set; }
        public AudioChannelsEventArgs(string ch, string layer, Int64 value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.AudioChannels = value;
            this.OscMessage = m;
        }
    }

    public class AudioFormatEventArgs : StageEventArgs
    {

        public string AudioFormat { get; set; }
        public AudioFormatEventArgs(string ch, string layer, string value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.AudioFormat = value;
            this.OscMessage = m;
        }
    }

    public class AudioCodecEventArgs : StageEventArgs
    {

        public string Codec { get; set; }
        public AudioCodecEventArgs(string ch, string layer, string value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Codec = value;
            this.OscMessage = m;
        }
    }

    public class LoopEventArgs : StageEventArgs
    {

        public bool Loop { get; set; }
        public LoopEventArgs(string ch, string layer, bool value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.Loop = value;
            this.OscMessage = m;
        }
    }

    public class BufferEventArgs : StageEventArgs
    {

        public Int64 BufferValue { get; set; }
        public BufferEventArgs(string ch, string layer, Int64 value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Layer = layer;
            this.BufferValue = value;
            this.OscMessage = m;
        }
    }

    public class AudioMixerChannelsEventArgs : ChannelEventArgs
    {
        public Int64 AudioChannels { get; set; }
        public AudioMixerChannelsEventArgs(string ch, Int64 value, OscMessage m = null)
        {
            this.Channel = ch;
            this.AudioChannels = value;
            this.OscMessage = m;
        }
    }

    public class AudioMixerDbfsEventArgs : ChannelEventArgs
    {

        public double Dbfs { get; set; }
        public AudioMixerDbfsEventArgs(string ch, double value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Dbfs = value;
            this.OscMessage = m;
        }
    }

    public class ChannelFormatEventArgs : ChannelEventArgs
    {

        public string Format { get; set; }
        public ChannelFormatEventArgs(string ch, string value, OscMessage m = null)
        {
            this.Channel = ch;
            this.Format = value;
            this.OscMessage = m;
        }
    }

    public class ChannelProfilerTimeEventArgs : ChannelEventArgs
    {

        public float RenderTime { get; set; }
        public float TotalTime { get; set; }
        public ChannelProfilerTimeEventArgs(string ch, float value1, float value2, OscMessage m = null)
        {
            this.Channel = ch;
            this.RenderTime = value1;
            this.TotalTime = value2;
            this.OscMessage = m;
        }
    }

    public class ChannelOutputTypeEventArgs : ChannelEventArgs
    {

        public Int64 OuputNumber { get; set; }
        public string OutputType { get; set; }
        public ChannelOutputTypeEventArgs(string ch, string value, Int64 outNum, OscMessage m = null)
        {
            this.Channel = ch;
            this.OutputType = value;
            this.OuputNumber = outNum;
            this.OscMessage = m;
        }
    }

    public class ChannelFrameGeneratedEventArgs : ChannelEventArgs
    {

        public Int64 FramesGenerated { get; set; }
        public Int64 MaxFrames { get; set; }
        public Int64 OuputNumber { get; set; }
        public ChannelFrameGeneratedEventArgs(string ch, Int64 value1, Int64 value2, Int64 outNum, OscMessage m = null)
        {
            this.Channel = ch;
            this.FramesGenerated = value1;
            this.MaxFrames = value2;
            this.OuputNumber = outNum;
            this.OscMessage = m;
            //gge
        }
    }


    class test:CasparCG.Osc.net.OscReceiver
    {

       
    }
}
