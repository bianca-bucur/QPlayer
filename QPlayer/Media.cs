using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QPlayer
{
    public class Media
    {
        private static int current_ID = 0;
        public static int Current_ID {
            get
            {
                return current_ID;
            }
            set
            {
                current_ID = value;
            }
        }

        public Uri source { get; set; }
        public int m_ID { get; set; }
        public string m_Title { get; set; }
        //public TimeSpam m_Length { get; set; }

        public Media()
        {
            current_ID++;
            m_ID = current_ID;
        }

        public static TimeSpan GetVideoDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }
    }
}
