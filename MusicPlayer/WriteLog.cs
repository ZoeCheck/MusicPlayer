using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MiniMusicPlayer
{
    public class FileOperate
    {
        public static void WriteFile(string path, string logStr)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("GB2312"));
            sw.Write(string.Format( logStr));
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public static List<string> ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.GetEncoding("GB2312"));
            List<string> list = new List<string>(50);
            string file = "";
            while (!sr.EndOfStream)
            {
                file = sr.ReadLine();
                if(file != "")
                list.Add(file);
            }
            sr.Close();
            sr.Dispose();

            return list;
        }
    }
}
