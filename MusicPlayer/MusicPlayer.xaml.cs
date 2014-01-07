using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.ComponentModel;
using PlayerLib;
using System.Windows.Media;
using Microsoft.Win32;

namespace MiniMusicPlayer
{
    /// <summary>
    /// MusicPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPlayer : Window
    {
        #region  变量
        private ZPlay startsPlayer;
        static int nFFTPoint = 32;
        int HarmonicNumber = nFFTPoint / 2 + 1;
        int[] HarmonicFreq;
        int[] LeftAmplitude;
        int[] RightAmplitude;
        int[] LeftPhase;
        int[] RightPhase;
        bool manualStop = false;
        private TCallbackFunc CallbackFunc;

        //增加歌曲列表滚动条变量
        private int oldValue = 0;
        private float vScrollMultiplier;
        private float vAbsPos;
        private float ListHeight;

        private PlayState playState = PlayState.Pause;
        private string songListfullName = ""; //播放列表名称
        private int playIndex = -1, playIndexNext = -1; //当前播放索引
        private bool playFlag = false;//表示开始播放或暂停 TRUE播放(暂停之后播放)，FALSE表示歌曲第一次播放
        private bool list_Isnull = false; //判断删除歌曲时是否全部删除列表中歌曲
        private bool IsPlayAfterload = true;//启动后即点击播放按钮
        private bool IsSelectChange = false;
        private bool isIncreaseVolumn = false;//true 增加音量
        private System.Windows.Forms.Timer timer1;//更改播放进度条timer
        private System.Windows.Forms.Timer timer3;
        private int addIndex = 0;
        #endregion

        private ObservableCollection<Song> songList;
        public ObservableCollection<Song> SongList
        {
            get { return songList; }
            set { songList = value; }
        }

        public MusicPlayer()
        {
            this.InitializeComponent();

            // 在此点之下插入创建对象所需的代码。
            InitPlayer();
        }

        private void InitPlayer()
        {
            timer1 = new System.Windows.Forms.Timer();
            this.timer1.Interval = 1000;
            this.timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = true;//启动timer

            this.timer3 = new System.Windows.Forms.Timer();
            this.timer3.Interval = 50;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);


            SongList = new ObservableCollection<Song>();
            lbSongList.ItemsSource = SongList;
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            startsPlayer.GetFFTData(nFFTPoint, TFFTWindow.fwBartlettHann, ref HarmonicNumber, ref HarmonicFreq, ref LeftAmplitude, ref RightAmplitude, ref LeftPhase, ref RightPhase);
            if (playState != PlayState.Stop)
            {
                TStreamTime pos = new TStreamTime();
                startsPlayer.GetPosition(ref pos);
                if (Song_trackBar.Maximum > pos.sec)
                {
                    Song_trackBar.Value = System.Convert.ToInt32((int)(pos.sec));
                    if (pos.hms.minute < 10)
                    {
                        tbMinute.Text = "0" + pos.hms.minute.ToString();
                    }
                    else
                    {
                        tbMinute.Text = pos.hms.minute.ToString();
                    }
                    if (pos.hms.second < 10)
                    {
                        tbSecond.Text = "0" + pos.hms.second.ToString();
                    }
                    else
                    {
                        tbSecond.Text = pos.hms.second.ToString();
                    }
                }
            }
            //else pictureBoxFFT.Refresh();

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //pictureBoxFFT.Refresh();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (playState == PlayState.Play)
            {
                startsPlayer.PausePlayback();
                playState = PlayState.Pause;
                btnPlay.Visibility = System.Windows.Visibility.Visible;
                btnsuspend.Visibility = System.Windows.Visibility.Hidden;
                btnPlay.Focus();
            }
            else
            {
                if (playFlag == false)//如果歌曲第一次播放
                {
                    if (IsPlayAfterload == true) { SetStartindex(); IsPlayAfterload = false; }

                    if (playIndexNext >= 0)
                    {
                        try
                        {
                            startsPlayer.OpenFile(((Song)this.lbSongList.Items[playIndexNext]).Tag.ToString(), TStreamFormat.sfAutodetect);
                            startsPlayer.StartPlayback();
                            timer3.Enabled = true;
                            manualStop = false;
                            Song_trackBar.Value = 0;
                            Song_trackBar.Minimum = 0;
                            TStreamInfo StreamInfo = new TStreamInfo();
                            startsPlayer.GetStreamInfo(ref StreamInfo);
                            Song_trackBar.Maximum = System.Convert.ToInt32((int)(StreamInfo.Length.sec));//将歌曲长度作为进度条的范围；
                            //songList.Items[playIndexNext].BackColor = Color.Yellow;
                            playIndex = playIndexNext;
                        }
                        catch (Exception ex)
                        {
                            //MyMessageBox.Show("歌曲不存在或已删除或不支持此格式 ！" + ex.Message, "提示信息", MyMessageBox.CYButtons.OK, MyMessageBox.CYIcon.Error);
                            return;
                        }


                        playFlag = true;//表示歌曲已经开始播放
                        playState = PlayState.Play;

                        SetControlState(playState); //设置按钮状态
                        btnsuspend.Visibility = System.Windows.Visibility.Visible;
                        btnPlay.Visibility = System.Windows.Visibility.Hidden;


                    }
                    else
                    {
                        //MessageBox.Show("请选择歌曲！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    startsPlayer.ResumePlayback();
                    playState = PlayState.Play;
                    btnsuspend.Visibility = System.Windows.Visibility.Visible;
                    btnPlay.Visibility = System.Windows.Visibility.Hidden;
                    //btnPlay.Text = "暂停";
                }
            }

            tbSongName.Text = ((Song)this.lbSongList.Items[playIndexNext]).Name;
        }

        private void SetControlState(PlayState pState)
        {
            switch ((int)pState)
            {
                case (int)PlayState.Play:
                    // btnPause.Enabled = true;
                    // btnResume.Enabled = false;
                    btnStop.IsEnabled = true;
                    break;
                case (int)PlayState.Stop:
                    //btnPause.Enabled = false;
                    //btnResume.Enabled = false;
                    btnStop.IsEnabled = false;
                    break;
                case (int)PlayState.Pause:
                    // btnPause.Enabled = false;
                    // btnResume.Enabled = true;
                    btnStop.IsEnabled = true;
                    break;
                default:
                    btnStop.IsEnabled = false;
                    //btnPause.Enabled = false;
                    //btnResume.Enabled = false;
                    break;
            }
        }

        private void lbSongList_GotFocus(object sender, RoutedEventArgs e)
        {
            //ListBoxItem item = e.OriginalSource as ListBoxItem;
            //Song song = item.DataContext as Song;
            //song.IsSelected = (song.IsSelected == true ? false : true);
        }

        [DllImport("Kernel32", CharSet = CharSet.Auto)]
        static extern Int32 GetShortPathName(String path, StringBuilder shortPath, Int32 shortPathLength);
        [DllImport("winmm.dll")]
        public static extern int mciSendString(string m_strCmd, StringBuilder m_strReceive, int m_v1, int m_v2);

        private string GetDuration(string filePath)
        {
            StringBuilder shortpath = new StringBuilder(80);
            GetShortPathName(filePath, shortpath, shortpath.Capacity);
            string name = shortpath.ToString();
            StringBuilder buf = new StringBuilder(80);
            mciSendString("close all", buf, buf.Capacity, 0);
            mciSendString("open " + name + " alias media", buf, buf.Capacity, 0);
            mciSendString("status media length", buf, buf.Capacity, 0);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)Convert.ToDouble(buf.ToString().Trim()));
            return ts.ToString();
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbSongList.Items.Count == 0)
            {
                list_Isnull = true;
            }

            OpenFile();

            if (list_Isnull == true && lbSongList.Items.Count > 0)
            {
                playIndex = playIndexNext = 0;
                ((Song)lbSongList.Items[0]).IsSelected = true;
                playFlag = false;
            }
            SaveList();
            ChecksongList();
        }

        /// <summary>
        /// 添加歌曲或播放列表
        /// </summary>
        private void OpenFile()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Multiselect = true;
            openfile.Filter = @"音频文件(*.mp3格式)|*.mp3|音频文件(*.wma格式)|*.wma|音频文件(*.wav格式)|*.wav";

			bool flag = (bool)openfile.ShowDialog();
			if (!flag)
            {
                return;
            }

            string[] fileList = openfile.FileNames;

            //获取音乐长度
            StartsPlayer startsPlayer4Length = new StartsPlayer();
            Dictionary<string, int> listSongInfo = startsPlayer4Length.GetListSongTime(fileList, true);
            startsPlayer4Length.Close();
            startsPlayer4Length.Dispose();

            foreach (string key in listSongInfo.Keys)
            {
                string songTime = "";
                string fileName = "";
                int value = 0;
                listSongInfo.TryGetValue(key, out value);

                songTime = string.Format("{0:00}:{1:00}", value / 1000 / 60, value / 1000 % 60);
                fileName = key.Substring(0, key.Length - 5);

                FileInfo MyFileInfo = new FileInfo(fileName);

                if (MyFileInfo.Extension.ToLower() != ".sta")
                {
                    //添加到listbox
                    Song newSong = new Song(MyFileInfo.Name, songTime);
                    newSong.Index = addIndex++;
                    newSong.Tag = fileName;
                    SongList.Add(newSong);
                }
                else
                {
                    List<string> list = FileOperate.ReadFile(fileName);
                    foreach (string str in list)
                    {
                        MyFileInfo = new FileInfo(str);

                        //添加到listbox
                        Song newSong = new Song(MyFileInfo.Name, songTime);
                        newSong.Index = addIndex++;
                        newSong.Tag = fileName;
                        SongList.Add(newSong);
                    }
                }
            }
            //SetScrollBar();
        }


        /// <summary>
        /// 音乐播放完毕时
        /// </summary>
        private void startsPlayer_PlayCompleted()
        {
            this.Dispatcher.Invoke(new Action(
                 delegate
                 {
                     if (rdoSequencePaly.IsChecked == true)
                     {
                         if (playIndex == this.lbSongList.Items.Count - 1)
                         {
                             btnStop_Click(null, null);
                         }
                         SetPlayIndex(PlayOrder.SequencePlay);
                     }
                     else if (rdoRandomPlay.IsChecked == true)
                     {
                         SetPlayIndex(PlayOrder.RandomPlay);
                     }
                     else if (rdoOneRepeatPlay.IsChecked == true)
                     {
                         SetPlayIndex(PlayOrder.OneRepeatPlay);
                     }
                     else
                     {
                         SetPlayIndex(PlayOrder.AllRepeatPlay);
                     }



                     if (playIndexNext >= 0)
                     {
                         //**************
                         playState = PlayState.Pause;
                         playFlag = false;
                         btnPlay_Click(null, null);

                     }
                     else
                     {
                         playIndexNext = 0;
                     }
                 }));


        }

        /// <summary>
        /// 根据播放方式来设置playIndex
        /// </summary>
        /// <param name="playOrder"></param>
        private void SetPlayIndex(PlayOrder playOrder)
        {
            if (lbSongList.Items.Count > 0)
            {
                if (playIndex >= 0)
                {
                    //lbSongList.Items[playIndex].BackColor = Color.White;
                }

                switch ((int)playOrder)
                {
                    case (int)PlayOrder.SequencePlay:   //顺序播放
                        playIndexNext++;
                        while (playIndexNext <= (lbSongList.Items.Count - 1) && (((Song)lbSongList.Items[playIndexNext]).IsSelected != true))
                        {
                            playIndexNext++;
                        }
                        if (playIndexNext > lbSongList.Items.Count - 1)
                            playIndexNext = -1;
                        break;
                    case (int)PlayOrder.AllRepeatPlay:  //全部循环
                        playIndexNext++;
                        if (playIndexNext > lbSongList.Items.Count - 1) { playIndexNext = 0; }
                        while (playIndexNext <= (lbSongList.Items.Count - 1) && ((Song)lbSongList.Items[playIndexNext]).IsSelected != true)
                        {
                            if (playIndexNext == lbSongList.Items.Count - 1)
                            {
                                playIndexNext = 0;
                            }
                            else
                            {
                                playIndexNext++;
                            }
                        }
                        break;
                    case (int)PlayOrder.OneRepeatPlay:  //单曲循环
                        break;
                    case (int)PlayOrder.RandomPlay:     //随机播放
                        Random rdm = new Random(unchecked((int)DateTime.Now.Ticks));
                        playIndexNext = rdm.Next() % lbSongList.Items.Count;
                        while (playIndexNext <= (lbSongList.Items.Count - 1) && ((Song)lbSongList.Items[playIndexNext]).IsSelected != true)
                        {
                            playIndexNext = rdm.Next() % lbSongList.Items.Count;
                        }
                        if (playIndexNext > lbSongList.Items.Count - 1)
                        {
                            playIndexNext = 0;
                        }
                        break;
                    default:
                        playIndexNext = 0;
                        break;
                }
            }
        }

        delegate void songplayCompleted();
        public int MyCallbackFunc(uint objptr, int user_data, TCallbackMessage msg, uint param1, uint param2)
        {
            switch (msg)
            {
                case TCallbackMessage.MsgStop:
                    if (manualStop) break;
                    songplayCompleted playCompleted = new songplayCompleted(startsPlayer_PlayCompleted);
                    playCompleted.BeginInvoke(null, null);
                    break;
            }

            return 0;
        }

        /// <summary>
        /// 读取播放列表
        /// </summary>
        private void ReadList()
        {
            FileInfo fileInfo = new FileInfo(songListfullName);

            if (fileInfo.Exists)
            {
                List<string> list = FileOperate.ReadFile(songListfullName).OrderBy(a => a, new CaseInsensitiveComparer()).ToList();

                //获取音乐长度
                StartsPlayer startsPlayer4Length = new StartsPlayer();
                Dictionary<string, int> listSongInfo = startsPlayer4Length.GetListSongTime(list, true);
                startsPlayer4Length.Close();
                startsPlayer4Length.Dispose();

                foreach (string key in listSongInfo.Keys)
                {
                    string songTime = "";
                    string fileName = "";
                    int value = 0;
                    listSongInfo.TryGetValue(key, out value);

                    songTime = string.Format("{0:00}:{1:00}", value / 1000 / 60, value / 1000 % 60);
                    fileName = key.Substring(0, key.Length - 5);

                    FileInfo MyFileInfo = new FileInfo(fileName);

                    //添加到listbox
                    Song newSong = new Song(MyFileInfo.Name, songTime);
                    newSong.Index = addIndex++;
                    newSong.Tag = fileName;
                    SongList.Add(newSong);

                    //string[] SubItem = { MyFileInfo.Name, songTime };
                    //ListViewItem item = new ListViewItem(SubItem);
                    //item.Tag = fileName;
                    //songList.Items.Add(item);
                    //SetScrollBar();
                }
            }
        }

        /// <summary>
        /// 保存播放列表
        /// </summary>
        private void SaveList()
        {
            string fileList = "";
            string path = "";
            if (songListfullName != "")
            {
                path = songListfullName.Substring(0, songListfullName.LastIndexOf("\\")); //歌曲列表所在目录
            }
            if (path != "")
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            for (int i = 0; i < this.lbSongList.Items.Count; i++)
            {
                fileList = string.Format("{0}\n{1}", fileList, ((Song)lbSongList.Items[i]).Tag.ToString());
            }

            FileOperate.WriteFile(songListfullName, fileList);

        }

        /// <summary>
        /// 设置按钮状态
        /// </summary>
        /// 
        private void SetVlolumn()
        {
            int left = 0;
            int right = 0;
            startsPlayer.GetMasterVolume(ref left, ref right);
            //progressBarVolume.Value = (left + right) / 2 == 0 ? 10 : (left + right) / 2;
        }

        /// <summary>
        /// 检查歌曲列表，若无歌曲则‘上一首’和‘下一首’及‘播放’按钮为disabled
        /// </summary>
        private void ChecksongList()
        {
            if (this.lbSongList.Items.Count < 1)
            {
                btnPre.IsEnabled = false;
                btnNext.IsEnabled = false;
                btnPlay.IsEnabled = false;
                btnsuspend.IsEnabled = false;
                btnStop.IsEnabled = false;
                btnDelSong.IsEnabled = false;
                //btnPlay.Text = "播放";
                btnPlay.Visibility = System.Windows.Visibility.Visible;
                btnsuspend.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                btnPre.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnPlay.IsEnabled = true;
                btnsuspend.IsEnabled = true;
                btnDelSong.IsEnabled = true;

                for (int i = 0; i < lbSongList.Items.Count; i++)
                {
                    ((Song)lbSongList.Items[i]).IsSelected = true;
                }
            }
        }

        private void SetStartindex()//设置默认播放的第一首歌曲序号
        {
            for (int i = 0; i < lbSongList.Items.Count; i++)
            {
                if (((Song)lbSongList.Items[i]).IsSelected == true)
                {
                    playIndex = playIndexNext = i;
                    break;
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            manualStop = true;//人为停止,非自然播放结束
            startsPlayer.StopPlayback();
            timer3.Enabled = false;
            playState = PlayState.Stop;
            //songList.Items[playIndex].BackColor = Color.White;
            playFlag = false;
            SetControlState(playState); //设置按钮状态
            btnPlay.Focus();
            //btnPlay.Text="播放";
            btnPlay.Visibility = System.Windows.Visibility.Visible;
            btnsuspend.Visibility = System.Windows.Visibility.Hidden;
            Song_trackBar.Value = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 歌曲播放结束callback
            CallbackFunc = new TCallbackFunc(MyCallbackFunc);
            startsPlayer = new ZPlay();
            startsPlayer.SetCallbackFunc(CallbackFunc, (TCallbackMessage)((TCallbackMessage.MsgStop)), 0);

            HarmonicFreq = new int[HarmonicNumber];
            LeftAmplitude = new int[HarmonicNumber];
            RightAmplitude = new int[HarmonicNumber];
            LeftPhase = new int[HarmonicNumber];
            RightPhase = new int[HarmonicNumber];
            SetVlolumn();

            //音量控制，需要的话要添加相应控件
            //progressBarVolume.Maximum = 100;
            //progressBarVolume.Minimum = 0;

            string FilePath = System.Windows.Forms.Application.StartupPath + "\\文件列表\\";

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            string[] files = Directory.GetFiles(FilePath);
            string tempFile = "";
            foreach (string file in files)   //音乐功能启动时加载最近访问的歌曲列表
            {
                if (!file.Substring(file.LastIndexOf(".")).Equals(".sta")) continue;//文件类型过滤
                if (tempFile == "")
                {
                    tempFile = file;
                }
                else
                {

                    FileInfo file1 = new FileInfo(tempFile);
                    FileInfo file2 = new FileInfo(file);
                    if (file1.LastAccessTime < file2.LastAccessTime) { tempFile = file2.FullName; }
                    else { tempFile = file1.FullName; }

                }

            }
            if ("" == tempFile)
            {
                songListfullName = FilePath + "1.sta";
                SaveList();

            }
            else
            {
                songListfullName = tempFile;
            }
            //labelSongList.Text = "播放列表：" + new FileInfo(songListfullName).Name;
            ReadList(); //加载已存在的歌曲列表文件

            ChecksongList();
            SetStartindex();
            
            //默认一种播放模式，新员工添加
            rdoAllRepeatPlay.IsChecked = true;
            SetPlayIndex(PlayOrder.AllRepeatPlay);
            playIndex = playIndexNext = 0;
			//((Song)lbSongList.Items[0]).IsSelected = true;
        }

        /// <summary>
        /// 双击播放音乐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSongList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.lbSongList.SelectedItems.Count > 0 && IsSelectChange)
            {
                manualStop = true;//人为停止,非自然播放结束
                startsPlayer.StopPlayback();
                timer3.Enabled = false;
                if (playIndex >= 0 && playIndex < (lbSongList.Items.Count))
                {
                    //songList.Items[playIndex].BackColor = Color.White;
                }

                playIndexNext = lbSongList.SelectedIndex;

                //((Song)lbSongList.SelectedItems[0]).IsSelected = true;
                ((ListBoxItem)lbSongList.ItemContainerGenerator.ContainerFromItem((Song)lbSongList.Items[playIndexNext])).Background = new SolidColorBrush(Colors.Chocolate);

                Song preSong = GetPreSong((Song)lbSongList.Items[playIndexNext]);
                if (preSong != null)
                {
                    ((ListBoxItem)lbSongList.ItemContainerGenerator.ContainerFromItem(preSong)).Background = null;
                }


                playFlag = false;
                IsPlayAfterload = false;
                playState = PlayState.Pause;
                manualStop = false;
                btnPlay_Click(null, null);
            }
            IsSelectChange = false;
        }

        private void lbSongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSongList.SelectedItems.Count < 0)
            {
                return;
            }
            IsSelectChange = true;
			try
			{
				if (((Song)lbSongList.Items[lbSongList.SelectedIndex]).IsSelected == true)
				{
					if (((Song)lbSongList.Items[lbSongList.SelectedIndex]).IsSelected == false)
					{
						((Song)lbSongList.Items[lbSongList.SelectedIndex]).IsSelected = true;
					}
					else
					{
						((Song)lbSongList.Items[lbSongList.SelectedIndex]).IsSelected = false;
					}
				}
			}
			catch
			{
			}
        }

        private void btnsuspend_Click(object sender, RoutedEventArgs e)
        {
            btnsuspend.Visibility = System.Windows.Visibility.Hidden;
            startsPlayer.PausePlayback();
            playState = PlayState.Pause;

            SetControlState(playState); //设置按钮状态
            btnPlay.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {

            if (this.lbSongList.Items.Count > 0)
            {
                manualStop = true;
                startsPlayer.StopPlayback();
                timer3.Enabled = false;
                //songList.Items[playIndex].BackColor = Color.White;
                ((Song)lbSongList.Items[playIndex]).IsSelected = false;
                playIndexNext = playIndex;
                do
                {
                    playIndexNext--;
                    if (playIndexNext == -1)
                    {
                        playIndexNext = lbSongList.Items.Count - 1; //循环到最后一首

                    }
                }
                while (playIndexNext >= 0 && playIndexNext <= (lbSongList.Items.Count - 1) && ((Song)(lbSongList.Items[playIndexNext])).IsSelected != true);

                //lbSongList.Items[playIndexNext].BackColor = Color.Yellow;
                playFlag = false;

                if (playState == PlayState.Play)
                {
                    playState = PlayState.Pause;
                    btnPlay_Click(null, null);
                }
                else
                {
                    //songList.Items[playIndexNext].BackColor = Color.Yellow;
                    startsPlayer.OpenFile(((Song)lbSongList.Items[playIndexNext]).Tag.ToString(), TStreamFormat.sfAutodetect);
                }

            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (lbSongList.Items.Count > 0)
            {
                manualStop = true;
                startsPlayer.StopPlayback();
                timer3.Enabled = false;
                //songList.Items[playIndex].BackColor = Color.White;
                ((Song)lbSongList.Items[playIndex]).IsSelected = false;
                playIndexNext = playIndex;
                do
                {
                    playIndexNext++;
                    if (playIndexNext == (lbSongList.Items.Count))
                    {
                        playIndexNext = 0; //循环到第一首

                    }
                }
                while (playIndexNext >= 0 && playIndexNext <= (lbSongList.Items.Count - 1) && (((Song)lbSongList.Items[playIndexNext]).IsSelected != true));

                //songList.Items[playIndexNext].BackColor = Color.Yellow;
                //当前播放歌曲变色
                Song currentSong = ((Song)lbSongList.Items[playIndexNext]);
                ListBoxItem currentListBocItem = this.lbSongList.ItemContainerGenerator.ContainerFromItem(currentSong) as ListBoxItem;
                currentListBocItem.Background = new SolidColorBrush(Colors.Chocolate);
                //上一首歌曲变色
                Song preSong = GetPreSong(currentSong);
                if (preSong != null)
                {
                    ListBoxItem preListBocItem = this.lbSongList.ItemContainerGenerator.ContainerFromItem(preSong) as ListBoxItem;
                    preListBocItem.Background = null;
                    //preListBocItem.IsSelected = false;
                }

                playFlag = false;

                if (playState == PlayState.Play)
                {
                    playState = PlayState.Pause;
                    btnPlay_Click(null, null);
                }
                else
                {
                    //lbSongList.Items[playIndexNext].BackColor = Color.Yellow;
                    startsPlayer.OpenFile(((Song)lbSongList.Items[playIndexNext]).Tag.ToString(), TStreamFormat.sfAutodetect);
                }

            }
        }

        private void Song_trackBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TStreamTime newpos = new TStreamTime();
            TStreamInfo Info = new TStreamInfo();
            startsPlayer.GetStreamInfo(ref Info);
            newpos.sec = Convert.ToUInt32(Song_trackBar.Value);
            Console.WriteLine(newpos.sec);
            startsPlayer.Seek(TTimeFormat.tfSecond, ref newpos, TSeekMethod.smFromBeginning);
        }

        private void btnDelSong_Click(object sender, RoutedEventArgs e)
        {
            List<Song> listSo = new List<Song>();

            foreach (Song item in SongList)
            {
                if (item.IsDelete == true)
                {
                    listSo.Add(item);
                }
            }
            if (listSo.Count == 0)
            {
                return;
            }

            //if (WindowUserMsg.Show("是否删除所有标记的歌曲？", MsgBoxButton.YesNo) == MessageBoxResult.No)
            //{
            //    return;
            //}

            try
            {
                for (int i = 0; i < listSo.Count; i++)
                {
                    SongList.Remove(listSo[i]);
                }

                if (playIndexNext > (this.lbSongList.Items.Count - 1))//下首歌的索引超出播放列表的长度时默认指向最后一首歌
                {
                    playIndexNext = lbSongList.Items.Count - 1;
                }

                if (lbSongList.Items.Count > 0)
                {
                    //光标移动
                    ((Song)lbSongList.Items[playIndexNext]).IsSelected = true;
                }
                else
                {
                    //不存在歌曲，关闭播放器
                    playIndexNext = -1;
                    playIndex = -1;
                    tbSongName.Text = "享受音乐每一秒";
                    manualStop = true;
                    startsPlayer.StopPlayback();
                    timer3.Enabled = false;
                    startsPlayer.Close();
                    playState = PlayState.Stop;
                    SaveList();
                    ChecksongList();
                    return;
                }
            }
            catch (Exception ex)
            {
            }


            //加了一个，如果当前播放的被删除，那么自动播放下一首，并且标签自动更新
            //btnNext_Click(null, null);
            SaveList();
            ChecksongList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnStop_Click(null, null);
        }

        private ChildType FindVisualChild<ChildType>(DependencyObject obj) where ChildType : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ChildType)
                {
                    return child as ChildType;
                }
                else
                {
                    ChildType childOfChild = FindVisualChild<ChildType>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        private Song GetPreSong(Song CurrentSong)
        {
            return SongList.FirstOrDefault(p => p.Index == CurrentSong.Index - 1);
        }
    }
}