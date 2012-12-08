/*
 JC2MapViewer
 Copyright 2010 - DerPlaya78
 
 this program is free software: you can redistribute it and/or modify
 it under the terms of the GNU Lesser General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Lesser General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BruTile.UI.Windows
{
    /// <summary>
    /// Interaction logic for MarkerControl.xaml
    /// </summary>
    public partial class MarkerControl : UserControl, IMarkerControl
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MarkerControl));
        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(MarkerControl));
        public static DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(MarkerControl), new PropertyMetadata(new PropertyChangedCallback(OnImageChanged)));

        public static RoutedEvent OverEvent = EventManager.RegisterRoutedEvent("Over", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MarkerControl));

        public event RoutedEventHandler Over
        {
            add { AddHandler(OverEvent, value); }
            remove { RemoveHandler(OverEvent, value); }
        }

        public static RoutedEvent LeaveEvent = EventManager.RegisterRoutedEvent("Leave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MarkerControl));

        public event RoutedEventHandler Leave
        {
            add { AddHandler(LeaveEvent, value); }
            remove { RemoveHandler(LeaveEvent, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        static void OnImageChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            MarkerControl c = (MarkerControl)source;
            ImageSource s = e.NewValue as ImageSource;
            c.image.Source = s;   
            
            double left = -s.Width / 2.0;
            double top = -s.Height;

            c.canvas.Margin = new Thickness(left, top, left, top);
        }

        public MarkerControl()
        {
            InitializeComponent();
        }

        private int zIndex;
        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            zIndex = Canvas.GetZIndex(this);
            Canvas.SetZIndex(this, 1000);
            RoutedEventArgs oe = new RoutedEventArgs(MarkerControl.OverEvent);
            RaiseEvent(oe);
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Canvas.SetZIndex(this, zIndex);
            RoutedEventArgs le = new RoutedEventArgs(MarkerControl.LeaveEvent);
            RaiseEvent(le);
        }
    }
}
