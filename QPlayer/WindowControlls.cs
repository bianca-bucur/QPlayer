using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QPlayer
{
    public class WindowControlls
    {
        public static void Close_Window(object sender)
        {
            Window w = (Window)sender;
            w.Close();
        }

        public static void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            Window w = (Window)sender;
            if (e.ChangedButton == MouseButton.Left)
            {
                w.DragMove();
            }
        }

        public static void Minimize_Window(object sender)
        {
            Window w = (Window)sender;
            w.Visibility = Visibility.Hidden;
        }
    }
}
