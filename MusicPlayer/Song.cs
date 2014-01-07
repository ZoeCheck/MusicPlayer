namespace MiniMusicPlayer
{
    public class Song : NotifyProperty
    {
        #region 变量
        private string _Name;
        private string _Duration;
        private bool _IsSelected;//是否选中
        private string _Path;
        private bool _IsDelete;//是否做删除标记
        #endregion

        #region 属性
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnChangedProperties("Name");
            }
        }

        public string Duration
        {
            get { return _Duration; }
            set
            {
                _Duration = value;
                OnChangedProperties("Duration");
            }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnChangedProperties("IsSelected");
            }
        }

        public string Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                OnChangedProperties("Path");
            }
        }

        public bool IsDelete
        {
            get { return _IsDelete; }
            set
            {
                _IsDelete = value;
                OnChangedProperties("IsDelete");
            }
        }

        public object Tag { get; set; }
        public int Index { get; set; }
        #endregion

        public Song(string name, string duration)
        {
            Name = name;
            Duration = duration;
            IsSelected = false;
            IsDelete = false;
        }
    }
}
