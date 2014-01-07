using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VideoConferenceClient
{
	/// <summary>
	/// WindowSystemConfig.xaml 的交互逻辑
	/// </summary>
	public partial class WindowSystemConfig : Window
	{

        private string selectedTabText = "编码器配置";

		public WindowSystemConfig()
		{
			this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
		}

        private object FindTabObject(string tabText)
        {
            object tabObject = null;
            switch (selectedTabText)
            {
                case "编码器配置":
                    tabObject = tabControl_DeviceList;
                    break;
                case "视频源控制":
                    tabObject = this.tabiVideosControl.Controls[0];
                    break;
                case "对话":
                    tabObject = this.tabiChatConfig.Controls[0];
                    break;
                case "预览窗口":
                    tabObject = this.tabiPreviewWindowConfig.Controls[0];
                    break;
                case "其它":
                    tabObject = this.tabiInstanceConfig.Controls[0];
                    break;
                default:
                    break;
            }

            return tabObject;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            object tabObject = FindTabObject(selectedTabText);
            if (null == tabObject)
            {
                return;
            }

            switch (selectedTabText)
            {
                case "编码器配置":
                    {
                        //TabControl tabControl_DeviceList = (TabControl)tabObject;
                        //AddDevice(tabControl_DeviceList);
                        MessageBox.Show("保存成功！");
                    }
                    break;
                case "视频源控制":
                    {
                        ConfigUserControl routerSetting = (ConfigUserControl)tabObject;
                        routerSetting.SaveConfigAll();
                    }
                    break;
                case "对话":
                    {
                        PIPsetting pip = (PIPsetting)tabObject;
                        pip.Save();
                    }
                    break;
                case "预览窗口":
                    {
                        ConfigConference cf = (ConfigConference)tabObject;
                        cf.Save();
                    }
                    break;
                case "其它":
                    {
                        Config_HIS_PACS chp = (Config_HIS_PACS)tabObject;
                        chp.Save();
                    }
                    break;
                default:
                    break;
            }
        }
	}
}