using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PlayerLib
{
    public partial class StartsPlayer
    {
        #region WINMM CONST

        private const int MCI_OPEN = 0x0803;
        private const int MCI_CLOSE = 0x0804;
        private const int MCI_ESCAPE = 0x0805;
        private const int MCI_PLAY = 0x0806;
        private const int MCI_SEEK = 0x807;
        private const int MCI_STOP = 0x808;
        private const int MCI_PAUSE = 0x809;
        private const int MCI_INFO = 0x080A;
        private const int MCI_GETDEVCAPS = 0x080B;
        private const int MCI_SPIN = 0x080C;
        private const int MCI_SET = 0x080D;
        private const int MCI_STEP = 0x080E;
        private const int MCI_RECORD = 0x080F;
        private const int MCI_SYSINFO = 0x0810;
        private const int MCI_BREAK = 0x0811;
        private const int MCI_SOUND = 0x0812;
        private const int MCI_SAVE = 0x0813;
        private const int MCI_STATUS = 0x814;
        private const int MCI_CUE = 0x0830;
        private const int MCI_REALIZE = 0x0840;
        private const int MCI_WINDOW = 0x0841;
        private const int MCI_PUT = 0x0842;
        private const int MCI_WHERE = 0x843;
        private const int MCI_FREEZE = 0x0844;
        private const int MCI_UNFREEZE = 0x0845;
        private const int MCI_LOAD = 0x0850;
        private const int MCI_CUT = 0x0851;
        private const int MCI_COPY = 0x0852;
        private const int MCI_PASTE = 0x0853;
        private const int MCI_UPDATE = 0x0854;
        private const int MCI_DELETE = 0x0856;
        private const int MCI_RESUME = 0x855;
        private const int MCI_CAPTURE = 0x0870;
        private const int MCI_MONITOR = 0x0871;
        private const int MCI_RESERVE = 0x0872;
        private const int MCI_SETAUDIO = 0x0873;
        private const int MCI_SIGNAL = 0x0875;
        private const int MCI_SETVIDEO = 0x0876;
        private const int MCI_QUALITY = 0x0877;
        private const int MCI_LIST =  0x0878;
        private const int MCI_UNDO = 0x0879;
        private const int MCI_CONFIGURE = 0x087a;
        private const int MCI_RESTORE = 0x087b;

        private const int MCI_NOTIFY = 0x00000001;
        private const int MCI_WAIT = 0x00000002;
        private const int MCI_FROM = 0x00000004;
        private const int MCI_TO = 0x00000008;
        private const int MCI_TRACK = 0x00000010;

        private const int MCI_OPEN_TYPE_ID = 0x1000;
        private const int MCI_OPEN_TYPE = 0x2000;
        private const int MCI_OPEN_SHAREABLE = 0x100;
        private const int MCI_OPEN_ELEMENT = 0x200;
        private const int MCI_OPEN_ALIAS = 0x400;

        private const int MCI_SET_DOOR_OPEN = 0x00000100;
        private const int MCI_SET_DOOR_CLOSED = 0x00000200;
        private const int MCI_SET_TIME_FORMAT = 0x00000400;
        private const int MCI_SET_AUDIO = 0x00000800;
        private const int MCI_SET_VIDEO = 0x00001000;
        private const int MCI_SET_ON = 0x00002000;
        private const int MCI_SET_OFF = 0x00004000;

        private const int MCI_SET_AUDIO_ALL = 0x00000000;
        private const int MCI_SET_AUDIO_LEFT = 0x00000001;
        private const int MCI_SET_AUDIO_RIGHT = 0x00000002;

        private const int MCI_DGV_SET_SPEED = 0x00020000;

        //constants used in 'set time format' and 'status time format' commands 
        private const int MCI_FORMAT_MILLISECONDS = 0x0;
        private const int MCI_FORMAT_HMS = 0x1;
        private const int MCI_FORMAT_MSF = 0x2;
        private const int MCI_FORMAT_FRAMES = 0x3;
        private const int MCI_FORMAT_SMPTE_24 = 0x4;
        private const int MCI_FORMAT_SMPTE_25 = 0x5;
        private const int MCI_FORMAT_SMPTE_30 = 0x6;
        private const int MCI_FORMAT_SMPTE_30DROP = 0x7;
        private const int MCI_FORMAT_BYTES = 0x8;
        private const int MCI_FORMAT_SAMPLES = 0x9;
        private const int MCI_FORMAT_TMSF = 0x10;

        /************************ value ************************/
        private const int MCI_DGV_SETAUDIO_SOURCE_LEFT = 0x1;
        private const int MCI_DGV_SETAUDIO_SOURCE_STEREO = 0x0;
        private const int MCI_DGV_SETAUDIO_SOURCE_RIGHT = 0x2;
        private const int MCI_DGV_SETAUDIO_SOURCE_AVERAGE = 0x00004000;

        private const int MCI_DGV_SETAUDIO_VOLUME = 0x00004002;  //音量大小控制
        private const int MCI_DGV_SETAUDIO_SOURCE = 0x00004004;

        private const int MCI_DGV_SETAUDIO_ITEM = 0x00800000;
        private const int MCI_DGV_SETAUDIO_VALUE = 0x01000000;

        private const int MAX_VOLUME = 1000;
        private const int MIN_VOLUME = 0;

        /************************ value ************************/

        /*******************************************************/
        private const int MCI_OVLY_WHERE_SOURCE = 0x20000;
        private const int MCI_OVLY_WHERE_DESTINATION = 0x40000;
        /*******************************************************/


        /*******************************************************/
        private const int MCI_ANIM_WINDOW_HWND = 0x00010000;
        private const int MCI_ANIM_WINDOW_STATE = 0x00040000;
        private const int MCI_ANIM_WINDOW_TEXT = 0x00080000;
        private const int MCI_ANIM_WINDOW_ENABLE_STRETCH = 0x00100000;
        private const int MCI_ANIM_WINDOW_DISABLE_STRETCH = 0x00200000;
        private const int MCI_ANIM_WINDOW_DEFAULT = 0x00000000;


        private const int MCI_ANIM_PUT_SOURCE = 0x00020000;
        private const int MCI_ANIM_RECT = 0x00010000;
        private const int MCI_ANIM_PUT_DESTINATION = 0x00040000;
        /*******************************************************/

        //MCI_SEEK
        private const int MCI_SEEK_TO_START = 0x00000100;
        private const int MCI_SEEK_TO_END = 0x00000200;

        //ShowWindow Command
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_NORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_FORCEMINIMIZE = 11;
        private const int SW_MAX = 10;

        //flags for dwFlags parameter of MCI_STATUS command message 
        private const int MCI_STATUS_ITEM = 0x00000100;
        private const int MCI_STATUS_START = 0x00000200;

        //flags for dwItem field of the MCI_STATUS_PARMS parameter block 
        private const int MCI_STATUS_LENGTH = 0x00000001;
        private const int MCI_STATUS_POSITION = 0x00000002;
        private const int MCI_STATUS_NUMBER_OF_TRACKS = 0x00000003;
        private const int MCI_STATUS_MODE = 0x00000004;
        private const int MCI_STATUS_MEDIA_PRESENT = 0x00000005;
        private const int MCI_STATUS_TIME_FORMAT = 0x00000006;
        private const int MCI_STATUS_READY = 0x00000007;
        private const int MCI_STATUS_CURRENT_TRACK = 0x00000008;

        private const int MCI_STRING_OFFSET = 512;

        //constants for predefined MCI device types 
        private const int MCI_DEVTYPE_VCR = MCI_STRING_OFFSET + 1;
        private const int MCI_DEVTYPE_VIDEODISC = MCI_STRING_OFFSET + 2;
        private const int MCI_DEVTYPE_OVERLAY = MCI_STRING_OFFSET + 3;
        private const int MCI_DEVTYPE_CD_AUDIO = MCI_STRING_OFFSET + 4;
        private const int MCI_DEVTYPE_DAT = MCI_STRING_OFFSET + 5;
        private const int MCI_DEVTYPE_SCANNER = MCI_STRING_OFFSET + 6;
        private const int MCI_DEVTYPE_ANIMATION = MCI_STRING_OFFSET + 7;
        private const int MCI_DEVTYPE_DIGITAL_VIDEO = MCI_STRING_OFFSET + 8;
        private const int MCI_DEVTYPE_OTHER = MCI_STRING_OFFSET + 9;
        private const int MCI_DEVTYPE_WAVEFORM_AUDIO = MCI_STRING_OFFSET + 10;
        private const int MCI_DEVTYPE_SEQUENCER = MCI_STRING_OFFSET + 11;

        //return values for 'status mode' command 
        private const int MCI_MODE_NOT_READY = MCI_STRING_OFFSET + 12;
        private const int MCI_MODE_STOP = MCI_STRING_OFFSET + 13;
        private const int MCI_MODE_PLAY = MCI_STRING_OFFSET + 14;
        private const int MCI_MODE_RECORD = MCI_STRING_OFFSET + 15;
        private const int MCI_MODE_SEEK = MCI_STRING_OFFSET + 16;
        private const int MCI_MODE_PAUSE = MCI_STRING_OFFSET + 17;
        private const int MCI_MODE_OPEN = MCI_STRING_OFFSET + 18;


        /* flags for dwFlags parameter of MCI_STEP command message */

        private const int MCI_DGV_STEP_REVERSE = 0x00010000;
        private const int MCI_DGV_STEP_FRAMES = 0x00020000;

        //flags for dwFlags parameter of MCI_GETDEVCAPS command message 
        private const int MCI_GETDEVCAPS_ITEM = 0x00000100;

        //flags for dwItem field of the MCI_GETDEVCAPS_PARMS parameter block 
        private const int MCI_GETDEVCAPS_CAN_RECORD = 0x00000001;
        private const int MCI_GETDEVCAPS_HAS_AUDIO = 0x00000002;
        private const int MCI_GETDEVCAPS_HAS_VIDEO = 0x00000003;
        private const int MCI_GETDEVCAPS_DEVICE_TYPE = 0x00000004;
        private const int MCI_GETDEVCAPS_USES_FILES = 0x00000005;
        private const int MCI_GETDEVCAPS_COMPOUND_DEVICE = 0x00000006;
        private const int MCI_GETDEVCAPS_CAN_EJECT = 0x00000007;
        private const int MCI_GETDEVCAPS_CAN_PLAY = 0x00000008;
        private const int MCI_GETDEVCAPS_CAN_SAVE = 0x00000009;

        //flags for dwFlags parameter of MCI_SAVE command message 
        private const int MCI_SAVE_FILE = 0x00000100;

        /* flags for dwFlags parameter of public const int MCI_INFO command message */

        private const int MCI_INFO_PRODUCT = 0x00000100;
        private const int MCI_INFO_FILE = 0x00000200;
        private const int MCI_INFO_MEDIA_UPC = 0x00000400;
        private const int MCI_INFO_MEDIA_IDENTITY = 0x00000800;
        private const int MCI_INFO_NAME = 0x00001000;
        private const int MCI_INFO_COPYRIGHT = 0x00002000;

        #endregion

        #region WINMM API
        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_OPEN_PARMS
        {
            public Int32 dwCallback; // Equivalente al hwdnCallback de mciSendString.
            public Int32 wDeviceID;  //ID del dispositivo que en nuestro caso al inicio sera 0.
            public string lpstrDeviceType; // Nombre del dispositivo.
            public string lpstrElementName; // Nombre del elemento, usualmente es la dirección del archivo que se quiere abrir
            public string lpstrAlias; // Alias del dispositivo abierto.
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_PLAY_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwFrom;
            public Int32 dwTo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_RECORD_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwFrom;
            public Int32 dwTo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_SAVE_PARMS
        {
            public Int32 dwCallback;
            public string lpfilename;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_SET_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwTimeFormat;
            public Int32 dwAudio;
            public Int32 dwFileFormat;
            public Int32 dwSpeed;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_DGV_SETAUDIO_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwItem;
            public Int32 dwValue;
            public Int32 dwOver;
            public string lpstrAlgorithm;
            public string lpstrQuality;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_DGV_SETVEDEO_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwItem;
            public Int32 dwValue;
            public Int32 dwOver;
            public Int32 lpstrAlgorithm;
            public string lpstrQuality;
            public Int32 dwSourceNumber;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_OVLY_RECT_PARMS
        {
            public Int32 dwCallback;
            public Rectangle rect;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_DGV_WINDOW_PARMS
        {
            public Int32 dwCallback;
            public Int32 hWnd;
            public Int32 nCmdShow;
            public string lpstrText;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_GENERIC_PARMS
        {
            public Int32 dwCallback;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_SEEK_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwTo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_STATUS_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwReturn;
            public Int32 dwItem;
            public Int32 dwTrack;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_DGV_STEP_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwFrames;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_GETDEVCAPS_PARMS
        {
            public Int32 dwCallback;
            public Int32 dwReturn;
            public Int32 dwItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MCI_INFO_PARMS
        {
            public Int32 dwCallback;
            public string lpstrReturn;
            public Int32 dwRetSize;
        }

        /*[DllImport("winmm.dll")]
        public static extern int mciExecute(string str);*/

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,  //接受命令的设备ID,由 MCI_OPEN 命令的wDeviceID变量返回
            UInt32 uMessage,  //MCI 命令
            Int32 dwParam,	  //flags 列表区，一般都与 DWORD dwParam 中的相关成员配合使用
            ref  MCI_OPEN_PARMS Any);	 //相应MCI命令的参数区类型 

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_PLAY_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_RECORD_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_SAVE_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_SET_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_DGV_SETAUDIO_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_DGV_SETVEDEO_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_OVLY_RECT_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_DGV_WINDOW_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_GENERIC_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_SEEK_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_STATUS_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_DGV_STEP_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_GETDEVCAPS_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciSendCommandA(
            Int32 wDeviceID,
            UInt32 uMessage,
            Int32 dwParam,
            ref  MCI_INFO_PARMS Any);

        [DllImport("winmm.dll")]
        private static extern int mciGetErrorStringA(
            Int32 dwError,
            StringBuilder lpstrBuffer,
            Int32 uLength);
        #endregion
    }
}
