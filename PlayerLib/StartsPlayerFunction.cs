using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace PlayerLib
{
    #region 回调函数
    /// <summary>
    /// 表示将处理 StartsPlayerLib.StartsPlayer 对象的 StartsPlayer.PlayCompleted 事件的方法。
    /// </summary>
    public delegate void PlayCompletedEventHandler();

    /// <summary>
    /// 表示将处理 StartsPlayerLib.StartsPlayer 对象的 StartsPlayer.PlayPosition 事件的方法。
    /// </summary>
    public delegate void PlayPositionEventHandler(int Position);

    #endregion

    #region 枚举

    /// <summary>
    /// 播放顺序
    /// </summary>
    public enum PlayOrder : int
    {
        /// <summary>
        /// 顺序播放
        /// </summary>
        SequencePlay = 0,
        /// <summary>
        /// 顺序循环播放
        /// </summary>
        AllRepeatPlay = 1,
        /// <summary>
        /// 单曲循环播放
        /// </summary>
        OneRepeatPlay = 2,
        /// <summary>
        /// 随机播放
        /// </summary>
        RandomPlay = 3
    }

    /// <summary>
    /// 播放状态。
    /// </summary>
    public enum PlayState : int
    {
        /// <summary>
        /// 播放未开始或者出错。
        /// </summary>
        None = 0,
        /// <summary>
        /// 播放未准备好。
        /// </summary>
        NotReady = 524,
        /// <summary>
        /// 播放停止。
        /// </summary>
        Stop = 525,
        /// <summary>
        /// 正在播放。
        /// </summary>
        Play = 526,
        /// <summary>
        /// 正在录音。
        /// </summary>
        Record = 527,
        /// <summary>
        /// 正在跳转到指定位置。
        /// </summary>
        Seek = 528,
        /// <summary>
        /// 播放暂停。
        /// </summary>
        Pause = 529,
        /// <summary>
        ///  文件已打开。
        /// </summary>
        Open = 530
    }

    /// <summary>
    /// 播放速度。
    /// </summary>
    public enum PlaySpeed : int
    {
        /// <summary>
        /// 2倍速。
        /// </summary>
        Two = 2000,
        /// <summary>
        /// 正常速度。
        /// </summary>
        Normal = 1000,
        /// <summary>
        /// 1/2速度。
        /// </summary>
        Half = 500
    }

    /// <summary>
    /// 声道。
    /// </summary>
    public enum AudioSource : int
    {
        /// <summary>
        /// 左声道。
        /// </summary>
        Left = 1,
        /// <summary>
        /// 立体声。
        /// </summary>
        Stereo = 0,
        /// <summary>
        /// 右声道。
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// 设备类型。
    /// </summary>
    public enum DeviceType : int
    {
        /// <summary>
        /// 由系统自动选择。
        /// </summary>
        AutoSelect = 0,
        /// <summary>
        /// 由StartsPlayerLib自动选择。
        /// </summary>
        OwnerSelect = 1,
        /// <summary>
        /// AVI 类型。
        /// </summary>
        AVIVideo = 2,
        /// <summary>
        /// MPEG类型，支持很多类型的文件。
        /// </summary>
        MPEGVideo = 3,
        /// <summary>
        /// CD音频。
        /// </summary>
        CDAudio = 4,
        /// <summary>
        /// 数字音频。
        /// </summary>
        DAT = 5,
        /// <summary>
        /// 数字视频
        /// </summary>
        DigitalVideo = 6,
        /// <summary>
        /// Movie。
        /// </summary>
        MMMovie = 7,
        /// <summary>
        /// 未定义设备。
        /// </summary>
        Other = 8,
        /// <summary>
        /// 重叠视频。
        /// </summary>
        Overlay = 9,
        /// <summary>
        /// 扫描仪。
        /// </summary>
        Scanner = 10,
        /// <summary>
        /// 序列器，MIDI类型。
        /// </summary>
        Sequencer = 11,
        /// <summary>
        /// 合式录像机。
        /// </summary>
        VCR = 12,
        /// <summary>
        /// 激光视盘。
        /// </summary>
        VideoDisc = 13,
        /// <summary>
        /// Wave 音频。
        /// </summary>
        WaveAudio = 14
    }

    /// <summary>
    /// 设备信息。
    /// </summary>
    public enum DevCaps : int
    {
        /// <summary>
        /// 无信息。
        /// </summary>
        None = 0,
        /// <summary>
        /// 可以快进、快退。
        /// </summary>
        CanStep = 1,
        /// <summary>
        /// 可以打开、关闭CD-ROM。
        /// </summary>
        CanEject = 2,
        /// <summary>
        /// 可以播放。
        /// </summary>
        CanPlay = 4,
        /// <summary>
        /// 可以录制。
        /// </summary>
        CanRecord = 8,
        /// <summary>
        /// 可以保存。
        /// </summary>
        CanSave = 16,
        /// <summary>
        /// 可以显示视频。
        /// </summary>
        CanShowVideo = 32

    }

    /// <summary>
    /// 时间格式。
    /// </summary>
    public enum TimeFormat : int
    {
        /// <summary>
        /// 系统默认格式。
        /// </summary>
        Default = -1,
        /// <summary>
        /// 毫秒。
        /// </summary>
        MilliSeconds = 0,
        /// <summary>
        /// 小时/分/秒。
        /// </summary>
        HMS = 1,
        /// <summary>
        ///  （分/秒/帧）Minute/second/frame。
        /// </summary>
        MSF = 2,
        /// <summary>
        /// 帧。
        /// </summary>
        Frames = 3,
        /// <summary>
        /// SMPTE（电影与电视工程师学会[美]）, 24 帧。 
        /// </summary>
        SMPTE24 = 4,
        /// <summary>
        /// 25 帧。
        /// </summary>
        SMPTE25 = 5,
        /// <summary>
        /// 30 帧。
        /// </summary>
        SMPTE30 = 6,
        /// <summary>
        /// 30 frame drop。
        /// </summary>
        SMPTE30Drop = 7,
        /// <summary>
        /// 比特 (使用　脉冲编码调制[PCM]格式的waveaudio 类型文件)。
        /// </summary>
        Bytes = 8,
        /// <summary>
        /// 采样。
        /// </summary>
        Samples = 9,
        /// <summary>
        /// （道、分、秒、帧）Track,Minute,Second,Frame。
        /// </summary>
        TMSF = 10
    }

    #endregion

    #region
    /// <summary>
    /// StartsPlayer 播放器组件。
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(StartsPlayer), "Player.ico")]
    [Description("StartsPlayer 播放器组件，可以实现大部分常见音乐、视频的播放，可以实现声道切换。")]
    [DefaultEvent("PlayCompleted")]
    [DefaultProperty("DeviceType")]
    public partial class StartsPlayer : Component
    {
        #region 变量

        private int deviceID = 0;
        private bool shareAble = false;
        private int volume = 500;
        private int volumeStep = 50;
        private int framesStep = 0;
        private PlaySpeed playSpeed = PlaySpeed.Normal;
        private string fileName = string.Empty;
        private DeviceType deviceType = DeviceType.OwnerSelect;
        private bool mute = false;
        private AudioSource audioSource = AudioSource.Stereo;
        private IntPtr hWndDisplay = IntPtr.Zero;
        private Rectangle rect = new Rectangle(0, 0, 640, 480);
        private bool showVideo = true;
        private Size size = Size.Empty;
        private DevCaps devCaps = DevCaps.None;
        private TimeFormat timeFormat = TimeFormat.Default;
        private int startPos = 0;
        private int endPos = 0;
        private bool useFrom = false;
        private bool useTo = false;
        private Timer timer = null;

        #endregion

        #region 定义事件
        /// <summary>
        /// 表示将处理 StartsPlayerLib.StartsPlayer 对象的播放完成事件的方法。
        /// </summary>
        [Category("播放")]
        [Description("播放完成。")]
        public event PlayCompletedEventHandler PlayCompleted;

        /// <summary>
        /// 表示将处理 StartsPlayerLib.StartsPlayer 对象的得到播放位置事件的方法。
        /// </summary>
        [Category("播放")]
        [Description("得到当前文件的播放位置。")]
        public event PlayPositionEventHandler PlayPosition;
        #endregion

        #region StartsPlayer 构造函数。

        /// <summary>
        /// 初始化一个　StartsPlayerLib.StartsPlayer　实例。
        /// </summary>
        public StartsPlayer()
        {
            timer = new Timer();
            timer.Tag = 0;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
        }

        /// <summary>
        /// 初始化一个　StartsPlayerLib.StartsPlayer　实例。
        /// </summary>
        /// <param name="container">容器的接口。</param>
        public StartsPlayer(IContainer container)
        {
            container.Add(this, null);
            timer = new Timer();
            timer.Tag = 0;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
        }

        /// <summary>
        /// 初始化一个　StartsPlayerLib.StartsPlayer　实例。
        /// </summary>
        /// <param name="handle">要在上面显视频的窗口的句柄。</param>
        public StartsPlayer(IntPtr handle)
        {
            this.hWndDisplay = handle;
            timer = new Timer();
            timer.Tag = 0;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
        }

        /// <summary>
        /// 初始化一个　StartsPlayerLib.StartsPlayer　实例。
        /// </summary>
        /// <param name="handle">要在上面显视频的窗口的句柄。</param>
        /// <param name="rect">要在窗口上显示的位置和大小。</param>
        public StartsPlayer(IntPtr handle, Rectangle rect)
        {
            this.hWndDisplay = handle;
            this.rect = rect;
            timer = new Timer();
            timer.Tag = 0;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
        }

        #endregion

        #region Player 属性设置
        /// <summary>
        ///  获取或设置要打开的文件名称（包括路径）。
        /// </summary>
        [Category("播放")]
        [Description("获取或设置要打开的文件名称（包括路径）。")]
        [DefaultValue("")]
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        /// <summary>
        ///  获取或设置要打开的文件的驱动类型。
        /// </summary>
        [Category("播放")]
        [Description("获取或设置要打开的文件的驱动类型。")]
        [DefaultValue(DeviceType.OwnerSelect)]
        public DeviceType DeviceType
        {
            set { deviceType = value; }
            get { return deviceType; }
        }

        /// <summary>
        ///  获取打开设备的信息。
        /// </summary>
        [Category("播放信息")]
        [Description("获取打开设备的信息。")]
        [DefaultValue(DevCaps.None)]
        [Browsable(false)]
        public DevCaps DevCaps
        {
            get { return devCaps; }
        }

        /// <summary>
        /// 获取或设置时间格式。
        /// </summary>
        [Category("播放")]
        [Description("获取或设置时间格式。")]
        [DefaultValue(TimeFormat.Default)]
        public TimeFormat TimeFormat
        {
            get { return timeFormat; }
            set
            {
                timeFormat = value;
                MciSet(0, (int)timeFormat, MCI_WAIT | MCI_SET_TIME_FORMAT);
            }
        }

        /// <summary>
        /// 获取打开驱动器的ID，0表示没有打开或打开失败。
        /// </summary>
        [Category("播放信息")]
        [Browsable(false)]
        [Description("获取打开驱动器的ID，0表示没有打开或打开失败。")]
        [DefaultValue(0)]
        public int DeviceID
        {
            get
            {
                return deviceID;
            }
        }

        /// <summary>
        /// 获取或设置文件是否以共享方式打开。
        /// </summary>
        [Category("播放")]
        [Description("获取或设置文件是否以共享方式打开。")]
        [DefaultValue(false)]
        public bool ShareAble
        {
            get
            {
                return shareAble;
            }
            set
            {
                shareAble = value;
            }
        }

        /// <summary>
        /// 获取音轨数（对CDAudio和Videodisc）。
        /// </summary>
        [Category("播放信息")]
        [Description(" 获取音轨数（对CDAudio和Videodisc）。")]
        [Browsable(false)]
        public int Tracks
        {
            get { return GetTracks(); }
        }

        /// <summary>
        /// 获取当前正在播放的音轨（对CDAudio和Videodisc）。
        /// </summary>
        [Category("播放信息")]
        [Description(" 获取当前正在播放的音轨（对CDAudio和Videodisc）。")]
        [Browsable(false)]
        public int CurrentTrack
        {
            get { return GetCurrentTracks(); }
        }

        /// <summary>
        /// 获取或设置播放的开始位置（注意：只对当前的播放文件使用，播放下一个文件时，如果没有重新设置，不再使用）。
        /// </summary>
        [Category("播放")]
        [Description("获取或设置播放的开始位置（注意：只对当前的播放文件使用，播放下一个文件时，如果没有重新设置，不再使用）。")]
        [DefaultValue(0)]
        public int StartPos
        {
            get { return startPos; }
            set
            {
                startPos = value;
                useFrom = true;
            }
        }

        /// <summary>
        /// 获取或设置播放的结束位置（注意：只对当前的播放文件使用，播放下一个文件时，如果没有重新设置，不再使用）。
        /// </summary>
        [Category("播放")]
        [Description("获取或设置播放的结束位置（注意：只对当前的播放文件使用，播放下一个文件时，如果没有重新设置，不再使用）。")]
        [DefaultValue(0)]
        public int EndPos
        {
            get { return endPos; }
            set
            {
                endPos = value;
                useTo = true;
            }
        }

        /// <summary>
        /// 获取当前播放状态。
        /// </summary>
        [Category("播放信息")]
        [Browsable(false)]
        [DefaultValue(PlayState.None)]
        [Description("获取当前播放状态。")]
        public PlayState PlayState
        {
            get
            {
                return GetPlayState();
            }
        }

        /// <summary>
        /// 获取或设置播放速度。
        /// </summary>
        [Category("播放")]
        [Browsable(false)]
        [DefaultValue(PlaySpeed.Normal)]
        [Description("获取或设置播放速度。")]
        public PlaySpeed PlaySpeed
        {
            get { return playSpeed; }
            set
            {
                playSpeed = value;
                switch (playSpeed)
                {
                    case PlaySpeed.Two:
                        SetSpeed(2000);
                        break;
                    case PlaySpeed.Normal:
                        SetSpeed(1000);
                        break;
                    case PlaySpeed.Half:
                        SetSpeed(500);
                        break;
                }
            }
        }

        /// <summary>
        /// 获取和设置是否静音。
        /// </summary>
        [Category("声音")]
        [DefaultValue(false)]
        [Description("获取和设置当前播放文件是否静音。true：静音；false：取消静音。")]
        public bool Mute
        {
            get
            {
                return mute;
            }
            set
            {
                mute = value;
                SetMute(mute);
            }
        }

        /// <summary>
        /// 获取和设置声道。
        /// </summary>
        [Category("声音")]
        [Description("获取和设置声道。")]
        [DefaultValue(AudioSource.Stereo)]
        public AudioSource AudioSource
        {
            get
            {
                return audioSource;
            }
            set
            {
                audioSource = value;
                SetAudio(audioSource);
            }
        }

        /// <summary>
        /// 获取或设置每次播放时声音的默认值（默认音量为500，最大音量为1000；注意：这个是播放文件的音量，不是系统的音量）。
        /// </summary>
        [Category("声音")]
        [Description("获取或设置每次播放时声音的默认值（默认音量为500，最大音量为1000；注意：这个是播放文件的音量，不是系统的音量）。")]
        [DefaultValue(500)]
        public int Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (value < MIN_VOLUME)
                    volume = MIN_VOLUME;
                else if (value > MAX_VOLUME)
                    volume = MAX_VOLUME;
                else
                    volume = value;
                SetVolume(volume);
            }
        }

        /// <summary>
        /// 获取或设置每次增加或减少的音量值（默认50）。注意：这个是直接调节播放文件的音量。
        /// </summary>
        [Category("声音")]
        [Description("获取或设置每次增加或减少的音量值（默认50）。注意：这个是直接调节播放文件的音量。")]
        [DefaultValue(50)]
        public int VolumeStep
        {
            get
            {
                return volumeStep;
            }
            set
            {
                volumeStep = value;
            }
        }

        /// <summary>
        /// 获取或设置当前播放位置。
        /// </summary>
        [Category("播放信息")]
        [Description("获取或设置当前播放位置。")]
        [DefaultValue(0)]
        [Browsable(false)]
        public int Position
        {
            get
            {
                return GetPosition();
            }
            set
            {
                SeekToAny(value);
            }
        }

        /// <summary>
        /// 获取当前播放文件的长度。
        /// </summary>
        [Category("播放信息")]
        [Description("获取当前播放文件的长度。")]
        [Browsable(false)]
        [DefaultValue(0)]
        public int Length
        {
            get
            {
                return GetLength();
            }
        }

        /// <summary>
        /// 获取或设置间隔多少时间检测一次播放状态和播放位置。
        /// </summary>
        [DefaultValue(1000)]
        [Category("计时器")]
        [Description("获取或设置计时器开始计时之间的时间。")]
        public int Interval
        {
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }

        /// <summary>
        /// 设置显示窗口的句柄。
        /// </summary>
        [Browsable(false)]
        [Category("播放")]
        [Description("设置显示窗口的句柄。")]
        public IntPtr HWndDisplay
        {
            set
            {
                hWndDisplay = value;
                ShowHnd();
            }
        }

        /// <summary>
        /// 获取或设置是否显示视频。
        /// </summary>
        [Category("显示")]
        [Description("获取或设置是否显示视频。")]
        [DefaultValue(true)]
        public bool ShowVideo
        {
            get { return showVideo; }
            set { showVideo = value; }
        }

        /// <summary>
        /// 获取或设置显示窗口的大小。
        /// </summary>
        [Category("显示")]
        [Description("获取或设置显示窗口的大小。")]
        public Rectangle Rect
        {
            get
            {
                return GetDisplayRect();
            }
            set
            {
                rect = value;
                ShowDisplay(rect);
            }
        }

        /// <summary>
        /// 获取初步窗口的初始大小。
        /// </summary>
        [Browsable(false)]
        [Category("播放信息")]
        [Description("获取初始窗口的初始大小。")]
        public Size VideoSize
        {
            get { return size; }
        }

        /// <summary>
        /// 获取或设置每次快进或快退值。
        /// </summary>
        [Category("播放")]
        [Description("获取或设置每次快进或快退值。")]
        [DefaultValue(0)]
        public int FramesStep
        {
            get
            {
                return framesStep;
            }
            set
            {
                framesStep = value;
            }
        }
        #endregion

        #region WINMM 函数

        #region Private Method

        private string GetDriverType(string fileName)
        {
            string type = string.Empty;
            if (File.Exists(fileName))
            {
                FileInfo f = new FileInfo(fileName);

                /*if(f.Extension.ToUpper() == ".MID" || f.Extension.ToUpper() == ".RMI" || f.Extension.ToUpper() == ".IDI")
                {
                    deviceType = "Sequencer";
                }
                else if(f.Extension.ToUpper() == ".ASF" ||f.Extension.ToUpper() == ".ASX" ||f.Extension.ToUpper() == ".IVF" ||
                    f.Extension.ToUpper() == ".LSF" ||f.Extension.ToUpper() == ".LSX" ||f.Extension.ToUpper() == ".P2V" ||
                    f.Extension.ToUpper() == ".WAX" ||f.Extension.ToUpper() == ".WVX" ||f.Extension.ToUpper() == ".WM" ||
                    f.Extension.ToUpper() == ".WMA" ||f.Extension.ToUpper() == ".WMX" ||f.Extension.ToUpper() == ".WMP"||
                    f.Extension.ToUpper() == ".WMV" ||f.Extension.ToUpper() == ".M3U" ||f.Extension.ToUpper() == ".MP3" )
                {
                    deviceType = "MPEGVideo2";
                }
                else if(f.Extension.ToUpper() == ".RM" ||f.Extension.ToUpper() == ".RAM" ||f.Extension.ToUpper() == ".RA")
                {
                    deviceType = "RealPlayer";
                }*/
                //if (f.Extension.ToUpper() == ".AVI")
                //{
                //    type = "AVIVideo";
                //}
                //else
                //{
                    type = "MPEGVideo";
                //}
            }
            else
            {
                type = "";
                //throw new ApplicationException("需要打开的文件不存在，或者文件正在使用！");
            }
            return type;
        }

        private int GetDevcap(int item)
        {
            int err = 0;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0)
            {
                MCI_GETDEVCAPS_PARMS mgp = new MCI_GETDEVCAPS_PARMS();
                mgp.dwItem = item;

                err = mciSendCommandA(deviceID, MCI_GETDEVCAPS, MCI_WAIT | MCI_GETDEVCAPS_ITEM, ref mgp);

                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("获取已打开的文件的功能出错，" + buf.ToString());
                }

                return mgp.dwReturn;
            }

            return -1;
        }

        private void GetDevcaps()
        {
            int devType = 0;

            if (GetDevcap(MCI_GETDEVCAPS_CAN_PLAY) > 0)
                devCaps = DevCaps.CanPlay;

            if (GetDevcap(MCI_GETDEVCAPS_CAN_RECORD) > 0)
                devCaps |= DevCaps.CanRecord;

            if (GetDevcap(MCI_GETDEVCAPS_CAN_SAVE) > 0)
                devCaps |= DevCaps.CanSave;

            if (GetDevcap(MCI_GETDEVCAPS_CAN_EJECT) > 0)
                devCaps |= DevCaps.CanEject;

            if (GetDevcap(MCI_GETDEVCAPS_HAS_VIDEO) > 0)
            {
                if (ShowHnd() == 0)
                    devCaps |= DevCaps.CanShowVideo;
            }

            devType = GetDevcap(MCI_GETDEVCAPS_DEVICE_TYPE);
            if (devType == MCI_DEVTYPE_ANIMATION || devType == MCI_DEVTYPE_DIGITAL_VIDEO
               || devType == MCI_DEVTYPE_VCR || devType == MCI_DEVTYPE_OVERLAY)
            {
                devCaps = devCaps | DevCaps.CanStep;
            }

            else if (devType == MCI_DEVTYPE_CD_AUDIO)
            {
                deviceType = DeviceType.CDAudio;
            }
            else if (devType == MCI_DEVTYPE_VIDEODISC)
            {
                deviceType = DeviceType.VideoDisc;
            }
        }

        private int GetStatus(int item)
        {
            int err = 0;
            StringBuilder buf = new StringBuilder(1000);

            if (deviceID > 0)
            {
                MCI_STATUS_PARMS mcistatusparms = new MCI_STATUS_PARMS();
                mcistatusparms.dwItem = item;
                mcistatusparms.dwReturn = 0;
                err = mciSendCommandA(deviceID, MCI_STATUS, MCI_WAIT | MCI_STATUS_ITEM, ref mcistatusparms);

                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("获取播放状态出错，" + buf.ToString());
                }

                return mcistatusparms.dwReturn;
            }

            return -1;
        }

        private TimeFormat GetTimeFormat()
        {
            TimeFormat tf = timeFormat;

            if (deviceID > 0)
            {
                tf = (TimeFormat)GetStatus(MCI_STATUS_TIME_FORMAT);
            }
            return tf;
        }

        private Size GetVideoSize()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            Size size = new Size(0, 0);

            if (deviceID > 0 && (devCaps & DevCaps.CanShowVideo) == DevCaps.CanShowVideo)
            {
                MCI_OVLY_RECT_PARMS rect = new MCI_OVLY_RECT_PARMS();
                err = mciSendCommandA(deviceID, MCI_WHERE, MCI_WAIT | MCI_OVLY_WHERE_SOURCE, ref rect);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("获取视频大小出错，" + buf.ToString());
                }
                else
                {
                    size = rect.rect.Size;
                }
            }
            return size;
        }

        private Rectangle GetDisplayRect()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            Rectangle rectTmp = new Rectangle();
            rectTmp = rect;
            if (deviceID > 0 && (devCaps & DevCaps.CanShowVideo) == DevCaps.CanShowVideo)
            {
                MCI_OVLY_RECT_PARMS rectp = new MCI_OVLY_RECT_PARMS();
                err = mciSendCommandA(deviceID, MCI_WHERE, MCI_WAIT | MCI_OVLY_WHERE_DESTINATION, ref rectp);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("获取视频位置和大小出错，" + buf.ToString());
                }
                else
                {
                    rectTmp = rectp.rect;
                }
            }
            return rectTmp;
        }

        private void SetAudio(AudioSource audio)
        {
            MCI_DGV_SETAUDIO_PARMS s = new MCI_DGV_SETAUDIO_PARMS();
            int err;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0)
            {
                err = mciSendCommandA(deviceID, MCI_SETAUDIO, MCI_WAIT | MCI_SET_OFF, ref s);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("设置声道出错，" + buf.ToString());
                }
                else
                {
                    switch (audio)
                    {
                        case AudioSource.Left:
                            s.dwValue = MCI_DGV_SETAUDIO_SOURCE_LEFT;
                            break;
                        case AudioSource.Stereo:
                            s.dwValue = MCI_DGV_SETAUDIO_SOURCE_STEREO;
                            break;
                        case AudioSource.Right:
                            s.dwValue = MCI_DGV_SETAUDIO_SOURCE_RIGHT;
                            break;
                    }
                    s.dwItem = MCI_DGV_SETAUDIO_SOURCE;
                    err = mciSendCommandA(deviceID, MCI_SETAUDIO, MCI_DGV_SETAUDIO_VALUE | MCI_DGV_SETAUDIO_ITEM, ref s);
                    if (err != 0)
                    {
                        mciGetErrorStringA(err, buf, 1000);
                        //throw new ApplicationException("设置声道出错，" + buf.ToString());
                    }
                    else
                    {
                        s.dwValue = MCI_DGV_SETAUDIO_SOURCE_AVERAGE;
                        s.dwItem = MCI_DGV_SETAUDIO_SOURCE;
                        err = mciSendCommandA(deviceID, MCI_SETAUDIO, MCI_DGV_SETAUDIO_VALUE | MCI_DGV_SETAUDIO_ITEM, ref s);

                        if (err != 0)
                        {
                            mciGetErrorStringA(err, buf, 1000);
                            //throw new ApplicationException("设置声道出错，" + buf.ToString());
                        }
                        else
                        {
                            err = mciSendCommandA(deviceID, MCI_SETAUDIO, MCI_WAIT | MCI_SET_ON, ref s);
                            if (err != 0)
                            {
                                mciGetErrorStringA(err, buf, 1000);
                                //throw new ApplicationException("设置声道出错，" + buf.ToString());
                            }
                        }
                    }
                }
            }
        }

        private void SetVolume(int volume)
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            MCI_DGV_SETAUDIO_PARMS mdsp = new MCI_DGV_SETAUDIO_PARMS();

            if (deviceID > 0)
            {
                err = mciSendCommandA(deviceID, MCI_SETAUDIO, MCI_WAIT | MCI_SET_OFF, ref mdsp);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("设置声音出错，" + buf.ToString());
                }
                else
                {
                    if (volume > MAX_VOLUME)
                        volume = MAX_VOLUME;
                    if (volume < MIN_VOLUME)
                        volume = MIN_VOLUME;
                    mdsp.dwValue = volume;
                    mdsp.dwItem = MCI_DGV_SETAUDIO_VOLUME;
                    err = mciSendCommandA(deviceID, MCI_SETAUDIO, MCI_DGV_SETAUDIO_VALUE | MCI_DGV_SETAUDIO_ITEM, ref mdsp);
                    if (err != 0)
                    {
                        mciGetErrorStringA(err, buf, 1000);
                        //throw new ApplicationException("设置声音出错，" + buf.ToString());
                    }
                    else
                    {
                        err = mciSendCommandA(deviceID, MCI_SETAUDIO, MCI_WAIT | MCI_SET_ON, ref mdsp);
                        if (err != 0)
                        {
                            mciGetErrorStringA(err, buf, 1000);
                            //throw new ApplicationException("设置声音出错，" + buf.ToString());
                        }
                    }
                }
            }
        }

        private void SetMute(bool bValue)
        {
            int err = 0;
            StringBuilder buf = new StringBuilder(1000);
            if (bValue)
                err = MciSet(1, MCI_SET_AUDIO_ALL, MCI_WAIT | MCI_SET_AUDIO | MCI_SET_OFF);
            else
                err = MciSet(1, MCI_SET_AUDIO_ALL, MCI_WAIT | MCI_SET_AUDIO | MCI_SET_ON);
            if (err != 0)
            {
                mciGetErrorStringA(err, buf, 1000);
                //throw new ApplicationException("设置静音出错，" + buf.ToString());
            }
        }

        private int MciSet(int itemType, int item, int dwflags)
        {
            int err = -1;
            StringBuilder buf = new StringBuilder(1000);
            MCI_SET_PARMS mciSP = new MCI_SET_PARMS();

            if (deviceID > 0)
            {
                if (itemType == 0)
                {
                    mciSP.dwTimeFormat = item;
                    err = mciSendCommandA(deviceID, MCI_SET, dwflags, ref mciSP);
                }
                else if (itemType == 1)
                {
                    mciSP.dwAudio = item;
                    err = mciSendCommandA(deviceID, MCI_SET, dwflags, ref mciSP);
                }
                else
                {
                    mciSP.dwSpeed = item;
                    err = mciSendCommandA(deviceID, MCI_SET, dwflags, ref mciSP);
                }
            }
            return err;
        }

        private void ShowDisplay(Rectangle rect)
        {
            StringBuilder buf = new StringBuilder(1000);
            int err = 0;
            MCI_OVLY_RECT_PARMS dwParam2 = new MCI_OVLY_RECT_PARMS();

            dwParam2.rect = rect;
            if (deviceID > 0 && (devCaps & DevCaps.CanShowVideo) == DevCaps.CanShowVideo)
            {
                err = mciSendCommandA(deviceID, MCI_PUT, MCI_WAIT | MCI_ANIM_RECT | MCI_ANIM_PUT_DESTINATION, ref dwParam2);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("播放显示出错，" + buf.ToString());
                }
            }
        }

        private int ShowHnd()
        {
            int err = -1;
            MCI_DGV_WINDOW_PARMS whnd = new MCI_DGV_WINDOW_PARMS();

            //if (hWndDisplay == IntPtr.Zero)
            //return -1;

            whnd.hWnd = (int)hWndDisplay;

            if (showVideo)
                whnd.nCmdShow = SW_SHOW;
            else
                whnd.nCmdShow = SW_HIDE;

            if (deviceID > 0)
            {
                err = mciSendCommandA(deviceID, MCI_WINDOW, MCI_WAIT | MCI_ANIM_WINDOW_HWND | MCI_ANIM_WINDOW_STATE, ref whnd);
            }

            return err;
        }

        /// <summary>
        /// 设置播放速度。
        /// </summary>
        /// <param name="speed">speed：500 1/2速度；1000 正常速度；2000 2倍速度</param>
        private void SetSpeed(int speed)
        {
            int err = 0;
            StringBuilder buf = new StringBuilder(1000);

            err = MciSet(3, speed, MCI_WAIT | MCI_DGV_SET_SPEED);

            if (err != 0)
            {
                mciGetErrorStringA(err, buf, 1000);
                //throw new ApplicationException("设置播放速度出错，" + buf.ToString());
            }
        }

        private int GetLength()
        {
            int len = 100;
            if (deviceID > 0)
            {
                len = GetStatus(MCI_STATUS_LENGTH);
            }
            return len;
        }

        private int GetPosition()
        {
            int position = 0;
            if (deviceID > 0)
            {
                position = GetStatus(MCI_STATUS_POSITION);
            }
            return position;
        }

        private PlayState GetPlayState()
        {
            int mode = 0;
            mode = GetStatus(MCI_STATUS_MODE);
            switch (mode)
            {
                case MCI_MODE_NOT_READY:
                    return PlayState.NotReady;

                case MCI_MODE_PAUSE:
                    return PlayState.Pause;

                case MCI_MODE_PLAY:
                    return PlayState.Play;

                case MCI_MODE_STOP:
                    return PlayState.Stop;

                case MCI_MODE_OPEN:
                    return PlayState.Open;

                case MCI_MODE_RECORD:
                    return PlayState.Record;

                case MCI_MODE_SEEK:
                    return PlayState.Seek;

                default:
                    return PlayState.None;
            }
        }

        private void SeekToAny(int Value)
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            MCI_SEEK_PARMS SeekParms = new MCI_SEEK_PARMS();

            if (deviceID > 0)
            {
                if (Value > 0 && Value < Length)
                {
                    SeekParms.dwTo = Value;
                    //跳转的目标时间，时间单位为毫秒
                    err = mciSendCommandA(deviceID, MCI_SEEK, MCI_TO | MCI_WAIT, ref SeekParms);
                    if (err != 0)
                    {
                        mciGetErrorStringA(err, buf, 1000);
                        //throw new ApplicationException(buf.ToString());
                    }

                    Play();
                }
                else if (Value >= Length)
                    Next();
                else
                    Previous();
            }
        }

        private byte LoByte(Int16 word)
        {
            byte[] b = BitConverter.GetBytes(word);
            return b[0];
        }

        private byte HiByte(Int16 word)
        {
            byte[] b = BitConverter.GetBytes(word);
            return b[1];
        }

        private Int16 LoWord(Int32 dWord)
        {
            byte[] b = BitConverter.GetBytes(dWord);
            return Int16.Parse(b[1].ToString("X2") + b[0].ToString("X2"), NumberStyles.HexNumber);
        }

        private Int16 HiWord(Int32 dWord)
        {
            byte[] b = BitConverter.GetBytes(dWord);
            return Int16.Parse(b[3].ToString("X2") + b[2].ToString("X2"), NumberStyles.HexNumber);
        }

        private int GetTracks()
        {
            if (timeFormat == TimeFormat.TMSF)
            {
                return GetStatus(MCI_STATUS_NUMBER_OF_TRACKS);
            }
            return -1;
        }

        private int GetCurrentTracks()
        {
            if (timeFormat == TimeFormat.TMSF)
            {
                return GetStatus(MCI_STATUS_CURRENT_TRACK);
            }
            return -1;
        }

        #endregion

        #region Public Method
        public Dictionary<string, int> GetListSongTime(IEnumerable<string> listSong, bool isAdd)
        {
            Dictionary<string, int> dicSongInfo = new Dictionary<string, int>();
            int i = 10000;
            foreach (string fileFullName in listSong)
            {
                int err;
                int paras = 0;
                StringBuilder buf = new StringBuilder(1000);
                MCI_OPEN_PARMS op = new MCI_OPEN_PARMS();

                if (deviceID > 0)
                {
                    //if (!isAdd)
                    //{
                        Close();
                    //}
                }

                if ((int)timer.Tag == 1)
                {
                    timer.Tag = 0;
                    timer.Stop();
                }

                if (fileFullName.ToString() == "" && deviceType != DeviceType.CDAudio && deviceType != DeviceType.VideoDisc
                    && deviceType != DeviceType.WaveAudio && deviceType != DeviceType.Overlay)
                {
                    //throw new ApplicationException("要打开的文件名为空！");
                }

                paras = MCI_WAIT;
                if (deviceType != DeviceType.CDAudio && deviceType != DeviceType.VideoDisc)
                    paras |= MCI_OPEN_ELEMENT;

                if (deviceType != DeviceType.AutoSelect)
                {
                    if (deviceType == DeviceType.OwnerSelect)
                        op.lpstrDeviceType = GetDriverType(fileFullName);
                    else
                        op.lpstrDeviceType = deviceType.ToString();

                    paras = paras | MCI_OPEN_TYPE;
                }

                if (shareAble)
                {
                    if (fileFullName != string.Empty && deviceType != DeviceType.WaveAudio && deviceType != DeviceType.Overlay)
                        paras = paras | MCI_OPEN_SHAREABLE;
                }

                op.dwCallback = 0;
                op.wDeviceID = 0;
                op.lpstrElementName = fileFullName;

                err = mciSendCommandA(0, MCI_OPEN, paras, ref op);
                deviceID = op.wDeviceID;

                
                dicSongInfo.Add(fileFullName+(i++), Length);
            }

            return dicSongInfo;
        }

        /// <summary>
        /// 打开多媒体文件。
        /// </summary>
        public void Open()
        {
            int err;
            int paras = 0;
            StringBuilder buf = new StringBuilder(1000);

            MCI_OPEN_PARMS op = new MCI_OPEN_PARMS();

            if (deviceID > 0)
                Close();

            if ((int)timer.Tag == 1)
            {
                timer.Tag = 0;
                timer.Stop();
            }

            if (fileName.ToString() == "" && deviceType != DeviceType.CDAudio && deviceType != DeviceType.VideoDisc
                && deviceType != DeviceType.WaveAudio && deviceType != DeviceType.Overlay)
            {
                //throw new ApplicationException("要打开的文件名为空！");
            }

            paras = MCI_WAIT;
            if (deviceType != DeviceType.CDAudio && deviceType != DeviceType.VideoDisc)
                paras |= MCI_OPEN_ELEMENT;

            if (deviceType != DeviceType.AutoSelect)
            {
                if (deviceType == DeviceType.OwnerSelect)
                    op.lpstrDeviceType = GetDriverType(fileName);
                else
                    op.lpstrDeviceType = deviceType.ToString();

                paras = paras | MCI_OPEN_TYPE;
            }

            if (shareAble)
            {
                if (fileName != string.Empty && deviceType != DeviceType.WaveAudio && deviceType != DeviceType.Overlay)
                    paras = paras | MCI_OPEN_SHAREABLE;
            }

            op.dwCallback = 0;
            op.wDeviceID = 0;
            op.lpstrElementName = fileName;

            err = mciSendCommandA(0, MCI_OPEN, paras, ref op);

            if (err != 0)
            {
                deviceID = 0;
                mciGetErrorStringA(err, buf, 1000);
                throw new ApplicationException("打开文件出错，" + buf.ToString());
            }
            else
            {
                deviceID = op.wDeviceID;
                try
                {
                    GetDevcaps();

                    //以帧快进有问题，去掉
                    /*
                    if ((devCaps & DevCaps.CanShowVideo) != DevCaps.CanShowVideo)
                    {
                        if (framesStep == 0)
                            framesStep = 3000;
                    }
                     */

                    if (framesStep == 0)
                        framesStep = Length / 200;

                    if (deviceType == DeviceType.CDAudio || deviceType == DeviceType.VideoDisc)
                    {
                        timeFormat = TimeFormat.TMSF; //set timeformat to use tracks
                        MciSet(0, (int)timeFormat, MCI_WAIT | MCI_SET_TIME_FORMAT);
                    }
                    else
                    {
                        if (timeFormat != TimeFormat.Default)
                            MciSet(0, (int)timeFormat, MCI_WAIT | MCI_SET_TIME_FORMAT);
                    }

                    try
                    {
                        size = GetVideoSize();
                        SetVolume(volume);
                        SetAudio(audioSource);
                        SetMute(mute);
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    //throw new ApplicationException("打开文件出错，" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 从当前位置开始或在一个指定区段播放多媒体文件。
        /// </summary>
        public void Play()
        {
            int err;
            int flags = 0;
            StringBuilder buf = new StringBuilder(1000);

            if (deviceID > 0 && PlayState != PlayState.Play)
            {
                if ((devCaps & DevCaps.CanPlay) == DevCaps.CanPlay)
                {
                    if (deviceType == DeviceType.CDAudio || deviceType == DeviceType.VideoDisc)
                    {
                        if (GetStatus(MCI_STATUS_MEDIA_PRESENT) <= 0)
                        {
                            //throw new ApplicationException("设备中没有光盘，请先放入光盘。");
                        }
                    }
                    if ((int)timer.Tag == 0)
                    {
                        timer.Tag = 1;
                        timer.Start();
                    }
                    MCI_PLAY_PARMS mciplay = new MCI_PLAY_PARMS();

                    if (useFrom && startPos != 0)
                    {
                        mciplay.dwFrom = startPos;
                        flags |= MCI_FROM;
                        useFrom = false;
                    }

                    if (useTo && endPos != 0)
                    {
                        mciplay.dwTo = endPos;
                        flags |= MCI_TO;
                        useTo = false;
                    }

                    err = mciSendCommandA(deviceID, MCI_PLAY, flags, ref mciplay);
                    if (err != 0)
                    {
                        mciGetErrorStringA(err, buf, 1000);
                        //throw new ApplicationException("播放出错，" + buf.ToString());
                    }
                    else
                    {
                        if (hWndDisplay == IntPtr.Zero)
                            rect = new Rectangle(0, 0, VideoSize.Width, VideoSize.Height);
                        ShowDisplay(rect);
                    }
                }
                else
                {
                    Close();
                    //throw new ApplicationException("播放出错，该文件不能播放。");
                }
            }
        }

        /// <summary>
        /// 从当前位置开始或在一个指定区段中录音。
        /// </summary>
        public void Record()
        {
            int err;
            int flags = 0;
            StringBuilder buf = new StringBuilder(1000);

            if (deviceID > 0 && (devCaps & DevCaps.CanRecord) == DevCaps.CanRecord)
            {
                if ((int)timer.Tag == 0)
                {
                    timer.Tag = 1;
                    timer.Start();
                }
                MCI_RECORD_PARMS mrp = new MCI_RECORD_PARMS();

                if (useFrom && startPos != 0)
                {
                    mrp.dwFrom = startPos;
                    flags |= MCI_FROM;
                    useFrom = false;
                }

                if (useTo && endPos != 0)
                {
                    mrp.dwTo = endPos;
                    flags |= MCI_TO;
                    useTo = false;
                }

                err = mciSendCommandA(deviceID, MCI_RECORD, flags, ref mrp);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("录音出错，" + buf.ToString());
                }
            }
            else
            {
                //throw new ApplicationException("录音出错，打开的文件不能录音。");
            }
        }

        /// <summary>
        /// 保存文件（文件路径和文件名为打开的文件路径和文件名 FileName ）。　
        /// </summary>
        public void Save()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);

            if (fileName != string.Empty)
            {
                if (deviceID > 0 && (devCaps & DevCaps.CanSave) == DevCaps.CanSave)
                {
                    MCI_SAVE_PARMS msp = new MCI_SAVE_PARMS();

                    msp.lpfilename = fileName;
                    err = mciSendCommandA(deviceID, MCI_SAVE, MCI_WAIT | MCI_SAVE_FILE, ref msp);
                    if (err != 0)
                    {
                        mciGetErrorStringA(err, buf, 1000);
                        //throw new ApplicationException("保存文件出错，" + buf.ToString());
                    }
                    else
                    {
                        Close();
                    }
                }
            }
            else
            {
                //throw new ApplicationException("保存文件出错，文件名为空或文件正在使用。");
            }
        }

        /// <summary>
        /// 减少音量（每次减少的值由VolumeStep决定）。
        /// </summary>
        public void SoundDecrease()
        {
            if (DeviceID != 0)
            {
                Volume -= volumeStep;
            }
        }
        /// <summary>
        /// 增加音量（每次增加的值由VolumeStep决定）。
        /// </summary>
        public void SoundIncrease()
        {
            if (DeviceID != 0)
            {
                Volume += volumeStep;
            }
        }

        /// <summary>
        /// 播放快进（每次快进 FramesStep）
        /// </summary>
        public void Forward()
        {
            int err = 0;
            StringBuilder buf = new StringBuilder(1000);
            TimeFormat tf = timeFormat;

            if (deviceID > 0)
            {
                if ((devCaps & DevCaps.CanShowVideo) == DevCaps.CanShowVideo)
                {
                    if (GetLength() - GetPosition() <= framesStep)
                    {
                        MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();

                        Pause();

                        err = mciSendCommandA(deviceID, MCI_SEEK, MCI_SEEK_TO_END | MCI_WAIT, ref gen);
                        if (err != 0)
                        {
                            mciGetErrorStringA(err, buf, 1000);
                            //throw new ApplicationException(buf.ToString());
                        }

                        Play();
                    }
                    else
                    {
                        MCI_DGV_STEP_PARMS step = new MCI_DGV_STEP_PARMS();

                        Pause();

                        if (timeFormat != TimeFormat.Frames)
                        {
                            timeFormat = TimeFormat.Frames;
                            MciSet(0, MCI_FORMAT_FRAMES, MCI_SET_TIME_FORMAT);
                        }

                        step.dwFrames = framesStep;
                        err = mciSendCommandA(deviceID, MCI_STEP, MCI_WAIT | MCI_DGV_STEP_FRAMES, ref step);
                        if (err != 0)
                        {
                            mciGetErrorStringA(err, buf, 1000);
                            //throw new ApplicationException(buf.ToString());
                        }

                        if (tf != TimeFormat.Frames)
                        {
                            timeFormat = tf;
                            MciSet(0, (int)tf, MCI_SET_TIME_FORMAT);
                        }
                        Play();
                    }
                }
                else
                {
                    if (timeFormat != TimeFormat.MilliSeconds)
                    {
                        timeFormat = TimeFormat.MilliSeconds;
                        MciSet(0, MCI_FORMAT_MILLISECONDS, MCI_SET_TIME_FORMAT);
                    }

                    SeekToAny(GetPosition() + framesStep);

                    if (tf != TimeFormat.MilliSeconds)
                    {
                        timeFormat = tf;
                        MciSet(0, (int)tf, MCI_SET_TIME_FORMAT);
                    }
                }
            }
        }

        /// <summary>
        /// 播放后退（每次后退 FramesStep）
        /// </summary>
        public void Backward()
        {
            int err = 0;
            StringBuilder buf = new StringBuilder(1000);
            TimeFormat tf = timeFormat;

            if (deviceID > 0)
            {
                if ((devCaps & DevCaps.CanShowVideo) == DevCaps.CanShowVideo)
                {
                    if (GetPosition() < framesStep)
                    {
                        MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();

                        Pause();

                        err = mciSendCommandA(deviceID, MCI_SEEK, MCI_SEEK_TO_START | MCI_WAIT, ref gen);
                        if (err != 0)
                        {
                            mciGetErrorStringA(err, buf, 1000);
                            //throw new ApplicationException(buf.ToString());
                        }
                        Play();
                    }
                    else
                    {
                        MCI_DGV_STEP_PARMS step = new MCI_DGV_STEP_PARMS();

                        Pause();

                        if (timeFormat != TimeFormat.Frames)
                        {
                            timeFormat = TimeFormat.Frames;
                            MciSet(0, MCI_FORMAT_FRAMES, MCI_SET_TIME_FORMAT);
                        }

                        step.dwFrames = framesStep;
                        err = mciSendCommandA(deviceID, MCI_STEP, MCI_WAIT | MCI_DGV_STEP_FRAMES | MCI_DGV_STEP_REVERSE, ref step);
                        if (err != 0)
                        {
                            mciGetErrorStringA(err, buf, 1000);
                            //throw new ApplicationException(buf.ToString());
                        }

                        if (tf != TimeFormat.Frames)
                        {
                            timeFormat = tf;
                            MciSet(0, (int)tf, MCI_SET_TIME_FORMAT);
                        }

                        Play();
                    }
                }
                else
                {
                    if (timeFormat != TimeFormat.MilliSeconds)
                    {
                        timeFormat = TimeFormat.MilliSeconds;
                        MciSet(0, MCI_FORMAT_MILLISECONDS, MCI_SET_TIME_FORMAT);
                    }
                    SeekToAny(GetPosition() - framesStep);

                    if (tf != TimeFormat.MilliSeconds)
                    {
                        timeFormat = tf;
                        MciSet(0, (int)tf, MCI_SET_TIME_FORMAT);
                    }
                }
            }
        }

        /// <summary>
        /// 使当前播放文件跳到上一首(TMSF)或开始位置。
        /// </summary>
        public void Previous()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0)
            {
                if (timeFormat == TimeFormat.TMSF)
                {
                    int pos = CurrentTrack;
                    Pause();

                    //如果不是第一首，跳到上一首，否则重新开始第一首。
                    if (pos != 1)
                    {
                        StartPos = pos - 1;
                    }
                    else
                        StartPos = pos;
                    Play();
                }
                else
                {
                    MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();

                    Pause();

                    err = mciSendCommandA(deviceID, MCI_SEEK, MCI_SEEK_TO_START | MCI_WAIT, ref gen);
                    if (err != 0)
                    {
                        mciGetErrorStringA(err, buf, 1000);
                        //throw new ApplicationException(buf.ToString());
                    }
                    Play();
                }
            }
        }

        /// <summary>
        /// 使当前播放文件跳到下一首(TMSF)或最后。
        /// </summary>
        public void Next()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0)
            {
                if (timeFormat == TimeFormat.TMSF)
                {
                    Pause();

                    int pos = CurrentTrack;
                    int tracks = Tracks;

                    //如果是最后一首，重新开始最后一首，否则下一首。
                    if (pos == tracks)
                    {
                        StartPos = tracks;
                    }
                    else
                    {
                        StartPos = pos + 1;
                    }
                    Play();
                }
                else
                {
                    MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();

                    Pause();

                    err = mciSendCommandA(deviceID, MCI_SEEK, MCI_SEEK_TO_END | MCI_WAIT, ref gen);
                    if (err != 0)
                    {
                        mciGetErrorStringA(err, buf, 1000);
                        //throw new ApplicationException(buf.ToString());
                    }

                    Play();
                }
            }
        }

        /// <summary>
        /// 播放暂停。
        /// </summary>
        public void Pause()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0 && PlayState == PlayState.Play || PlayState == PlayState.Record)
            {
                MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();
                err = mciSendCommandA(deviceID, MCI_PAUSE, MCI_WAIT, ref gen);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException(buf.ToString());
                }
                else
                {
                    if ((int)timer.Tag == 1)
                    {
                        timer.Tag = 0;
                        timer.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// 继续播放。
        /// </summary>
        public void Resume()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0 && PlayState == PlayState.Pause)
            {
                MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();
                err = mciSendCommandA(deviceID, MCI_RESUME, MCI_WAIT, ref gen);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException(buf.ToString());
                }
                else
                {
                    if ((int)timer.Tag == 0)
                    {
                        timer.Tag = 1;
                        timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 停止当前播放文件。
        /// </summary>
        public void Stop()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0 && PlayState == PlayState.Play || PlayState == PlayState.Record)
            {
                Previous();
                MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();
                err = mciSendCommandA(deviceID, MCI_STOP, MCI_WAIT, ref gen);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException(buf.ToString());
                }
                else
                {
                    if ((int)timer.Tag == 1)
                    {
                        timer.Tag = 0;
                        timer.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// 打开　CD-ROM。
        /// </summary>
        public void OpenCdRom()
        {
            if (deviceID > 0 && (devCaps & DevCaps.CanEject) == DevCaps.CanEject)
            {
                int err = -1;
                StringBuilder buf = new StringBuilder(1000);
                MCI_SET_PARMS mciSP = new MCI_SET_PARMS();

                err = mciSendCommandA(deviceID, MCI_SET, MCI_WAIT | MCI_SET_DOOR_OPEN, ref mciSP);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("打开 CD-ROM 出错，" + buf.ToString());
                }
            }
        }

        /// <summary>
        /// 关闭 CD-ROM。
        /// </summary>
        public void CloseCdRom()
        {
            if (deviceID > 0 && (devCaps & DevCaps.CanEject) == DevCaps.CanEject)
            {
                int err = -1;
                StringBuilder buf = new StringBuilder(1000);
                MCI_SET_PARMS mciSP = new MCI_SET_PARMS();

                err = mciSendCommandA(deviceID, MCI_SET, MCI_WAIT | MCI_SET_DOOR_CLOSED, ref mciSP);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("关闭 CD-ROM 出错，" + buf.ToString());
                }
            }
        }

        /// <summary>
        /// 关闭当前播放文件。
        /// </summary>
        public void Close()
        {
            int err;
            StringBuilder buf = new StringBuilder(1000);
            if (deviceID > 0)
            {
                Stop();

                MCI_GENERIC_PARMS gen = new MCI_GENERIC_PARMS();
                err = mciSendCommandA(deviceID, MCI_CLOSE, MCI_WAIT, ref gen);
                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException(buf.ToString());
                }
                deviceID = 0;
                devCaps = DevCaps.None;
                size = Size.Empty;
                if ((int)timer.Tag == 1)
                {
                    timer.Tag = 0;
                    timer.Stop();
                }
            }
        }

        /// <summary>
        /// 获取 trackNum 音轨的长度（驱动类型为 CDAudio 或 VideoDisc）。
        /// </summary>
        /// <param name="trackNum">音轨数。</param>
        /// <returns>trackNum 音轨的长度（MSF格式）。</returns>
        public int GetTrackLength(int trackNum)
        {
            int err = 0;
            StringBuilder buf = new StringBuilder(1000);

            if (deviceID > 0 && timeFormat == TimeFormat.TMSF)
            {
                MCI_STATUS_PARMS mcistatusparms = new MCI_STATUS_PARMS();
                mcistatusparms.dwItem = MCI_STATUS_LENGTH;
                mcistatusparms.dwTrack = trackNum;
                mcistatusparms.dwReturn = 0;
                err = mciSendCommandA(deviceID, MCI_STATUS, MCI_WAIT | MCI_STATUS_ITEM | MCI_TRACK, ref mcistatusparms);

                if (err != 0)
                {
                    mciGetErrorStringA(err, buf, 1000);
                    //throw new ApplicationException("获取播放状态出错，" + buf.ToString());
                }

                return mcistatusparms.dwReturn;
            }
            return 0;
        }

        /// <summary>
        /// TimeFormat 为　MSF　格式的分钟。
        /// </summary>
        /// <param name="msf">MSF　值。</param>
        /// <returns>MSF　格式的分钟。</returns>
        public byte Mci_MSF_Minute(Int32 msf)
        {
            return LoByte(LoWord(msf));
        }

        /// <summary>
        /// TimeFormat 为　MSF　格式的秒。
        /// </summary>
        /// <param name="msf">MSF　值。</param>
        /// <returns>MSF　格式的秒。</returns>
        public byte Mci_MSF_Second(Int32 msf)
        {
            return HiByte(LoWord(msf));
        }

        /// <summary>
        /// TimeFormat 为　MSF　格式的帧。
        /// </summary>
        /// <param name="msf">MSF　值。</param>
        /// <returns>MSF　格式的帧。</returns>
        public byte Mci_MSF_Frame(Int32 msf)
        {
            return LoByte(HiWord(msf));
        }

        /// <summary>
        /// 把分、秒、帧转换为 MSF　值。
        /// </summary>
        /// <param name="m">分。</param>
        /// <param name="s">秒。</param>
        /// <param name="f">帧。</param>
        /// <returns>MSF　值。</returns>
        public Int32 Mci_Make_MSF(Byte m, Byte s, Byte f)
        {
            return (Int32)(m | (s << 8) | (f << 16));
        }

        /// <summary>
        /// TimeFormat 为　TMSF　格式的道。
        /// </summary>
        /// <param name="tmsf">TMSF　值。</param>
        /// <returns>TMSF　格式的道。</returns>
        public byte Mci_TMSF_Track(Int32 tmsf)
        {
            return LoByte(LoWord(tmsf));
        }

        /// <summary>
        /// TimeFormat 为　TMSF　格式的分钟。
        /// </summary>
        /// <param name="tmsf">TMSF　值。</param>
        /// <returns>TMSF　格式的分钟。</returns>
        public byte Mci_TMSF_Minute(Int32 tmsf)
        {
            return HiByte(LoWord(tmsf));
        }

        /// <summary>
        /// TimeFormat 为　TMSF　格式的秒。
        /// </summary>
        /// <param name="tmsf">TMSF　值。</param>
        /// <returns>TMSF　格式的秒。</returns>
        public byte Mci_TMSF_Second(Int32 tmsf)
        {
            return LoByte(HiWord(tmsf));
        }

        /// <summary>
        /// TimeFormat 为　TMSF　格式的帧。
        /// </summary>
        /// <param name="tmsf">TMSF　值。</param>
        /// <returns>TMSF　格式的帧。</returns>
        public byte Mci_TMSF_Frame(Int32 tmsf)
        {
            return HiByte(HiWord(tmsf));
        }

        /// <summary>
        /// 把道、分、秒、帧转换为　TMSF　值。
        /// </summary>
        /// <param name="t">道。</param>
        /// <param name="m">分。</param>
        /// <param name="s">秒。</param>
        /// <param name="f">帧。</param>
        /// <returns>TMSF　值。</returns>
        public Int32 Mci_Make_TMSF(Byte t, Byte m, Byte s, Byte f)
        {
            return (Int32)(t | (m << 8) | (s << 16) | (f << 24));
        }

        /// <summary>
        /// TimeFormat 为　HMS　格式的小时。
        /// </summary>
        /// <param name="hms">HMS　值。</param>
        /// <returns>HMS　格式的小时。</returns>
        public byte Mci_HMS_Hour(Int32 hms)
        {
            return LoByte(LoWord(hms));
        }

        /// <summary>
        /// TimeFormat 为　HMS　格式的分。
        /// </summary>
        /// <param name="hms">HMS　值。</param>
        /// <returns>HMS　格式的分。</returns>
        public byte Mci_HMS_Minute(Int32 hms)
        {
            return HiByte(LoWord(hms));
        }

        /// <summary>
        /// TimeFormat 为　HMS　格式的秒。
        /// </summary>
        /// <param name="hms">HMS　值。</param>
        /// <returns>HMS　格式的秒。</returns>
        public byte Mci_HMS_Second(Int32 hms)
        {
            return LoByte(HiWord(hms));
        }

        /// <summary>
        /// 把小时、分、秒转换为　HMS　值。
        /// </summary>
        /// <param name="h">小时。</param>
        /// <param name="m">分。</param>
        /// <param name="s">秒。</param>
        /// <returns>HMS　值。</returns>
        public Int32 Mci_Make_HMS(Byte h, Byte m, byte s)
        {
            return (Int32)(h | (m << 8) | (s << 16));
        }

        #endregion

        #region 响应事件
        private void OnPlayCompleted()
        {
            if (PlayCompleted != null)
            {
                PlayCompleted();
            }
        }

        private void OnPlayPosition(int position)
        {
            if (PlayPosition != null)
            {
                PlayPosition(position);
            }
        }
        #endregion

        #region Timer Event

        private void timer_Tick(object sender, EventArgs e)
        {
            OnPlayPosition(Position);

            PlayState playMode = GetPlayState();
            if (playMode == PlayState.Stop)
            {
                Close();
                OnPlayCompleted();
            }
        }

        #endregion

        #region Dispose
        /// <summary>
        /// 释放由 System.ComponentModel.Component 占用的非托管资源，还可以另外再释放托管资源。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
        protected override void Dispose(bool disposing)
        {
            if (deviceID > 0)
                Close();
            hWndDisplay = IntPtr.Zero;

            base.Dispose(disposing);
        }
        #endregion

        #endregion
    }
    #endregion
}
