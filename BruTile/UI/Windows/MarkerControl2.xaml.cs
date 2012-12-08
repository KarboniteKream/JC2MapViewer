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
    /// Interaction logic for MarkerControl2.xaml
    /// </summary>
    public partial class MarkerControl2 : UserControl, IMarkerControl
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MarkerControl2));
        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(MarkerControl2));

        public static RoutedEvent OverEvent = EventManager.RegisterRoutedEvent("Over", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MarkerControl2));

        public event RoutedEventHandler Over
        {
            add { AddHandler(OverEvent, value); }
            remove { RemoveHandler(OverEvent, value); }
        }

        public static RoutedEvent LeaveEvent = EventManager.RegisterRoutedEvent("Leave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MarkerControl2));

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

        public static DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(UserControl), new PropertyMetadata(new PropertyChangedCallback(OnPointsChanged)));

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        static void OnPointsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MarkerControl2 c = (MarkerControl2)sender;
            PointCollection p = e.NewValue as PointCollection;
            if (p != null)
            {
                c.poly.BeginInit();
                c.poly.Points = p;
                c.poly.EndInit();
            }
        }

        public MarkerControl2()
        {
            InitializeComponent();
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            RoutedEventArgs oe = new RoutedEventArgs(MarkerControl2.OverEvent);
            RaiseEvent(oe);
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            RoutedEventArgs le = new RoutedEventArgs(MarkerControl2.LeaveEvent);
            RaiseEvent(le);
        }
    }
}
