// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using BruTile;
using BruTile.Cache;
using System.Windows.Media;

namespace BruTile.UI.Windows
{
    public class Renderer
    {
        ITileCache<Image> images = new MemoryCache<Image>(100, 200);

        Dictionary<int, ImageSource> markerImages = new Dictionary<int, ImageSource>();
        public Dictionary<int, ImageSource> MarkerImages { get { return markerImages; } }

        public IMarkerControl ActiveMarker { get; private set; }

        public void Render(Canvas canvas, TileSchema schema, ITransform transform, MemoryCache<MemoryStream> cache, List<Marker> markerCache)
        {
            CollapseAll(canvas);
            int level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, transform.Resolution);
            DrawRecursive(canvas, schema, transform, cache, transform.Extent, level);
            DrawMarkers(canvas, schema, transform, markerCache, transform.Extent, level);
            RemoveCollapsed(canvas);
        }

        private void DrawMarkers(Canvas canvas, TileSchema schema, ITransform transform, List<Marker> cache, Extent extent, int level)
        {

            foreach (Marker m in cache)
            {
                if (m.UIElement == null)
                {
                    if (!m.Visible) continue;

                    if (m.Points != null)
                    {
                        MarkerControl2 c = new MarkerControl2();
                        c.Over += new RoutedEventHandler(c_Over);
                        c.Leave += new RoutedEventHandler(c_Leave);
                        c.Points = new PointCollection(m.Points);

                        c.Text = m.Text;
                        c.Description = m.Description;

                        m.UIElement = c;
                    }
                    else 
                    {
                        MarkerControl c = new MarkerControl();
                        c.Over += new RoutedEventHandler(c_Over);
                        c.Leave += new RoutedEventHandler(c_Leave);
                        if (markerImages != null)
                        {
                            if (markerImages.ContainsKey(m.ImageIndex))
                            {
                                c.Image = markerImages[m.ImageIndex];
                            }
                            else if (markerImages.ContainsKey(-1))
                            {
                                c.Image = markerImages[-1];
                            }
                        }

                        c.Text = m.Text;
                        c.Description = m.Description;

                        m.UIElement = c;                    
                    }
                }

                //MarkerControl marker = m.UIElement as MarkerControl;
                UIElement marker = m.UIElement as UIElement;
                marker.Visibility = Visibility.Collapsed;

                if (!m.Visible)
                {
                    continue;
                }
                    
                if (!canvas.Children.Contains(marker))
                {
                    canvas.Children.Add(marker);
                }

                Rect dest = MapTransformHelper.WorldToMap(extent, transform);
                Point p = transform.WorldToMap(m.X, m.Y);
                if (dest.Contains(p))
                {
                    Canvas.SetZIndex(marker, m.ZIndex);
                    Canvas.SetLeft(marker, p.X);
                    Canvas.SetTop(marker, p.Y);

                    if (m.Points != null)
                    {
                        if (!(marker.RenderTransform is ScaleTransform))
                        {
                            marker.RenderTransform = new ScaleTransform(1.0 / transform.Resolution, 1.0 / transform.Resolution);
                        }
                        else
                        {
                            ((ScaleTransform)marker.RenderTransform).ScaleX = 1.0 / transform.Resolution;
                            ((ScaleTransform)marker.RenderTransform).ScaleY = 1.0 / transform.Resolution;
                        }
                    }
                    /*
                    double factor = 4;
                    if (!(marker.RenderTransform is ScaleTransform))
                    {
                        marker.RenderTransform = new ScaleTransform(1.0 / transform.Resolution * factor, 1.0 / transform.Resolution * factor);
                    }
                    else {
                        ((ScaleTransform)marker.RenderTransform).ScaleX = 1.0 / transform.Resolution * factor;
                        ((ScaleTransform)marker.RenderTransform).ScaleY = 1.0 / transform.Resolution * factor;
                    }
                    */

                    marker.Visibility = Visibility.Visible;
                }
            }
        }

        void c_Leave(object sender, RoutedEventArgs e)
        {
            ActiveMarker = null;
        }

        void c_Over(object sender, RoutedEventArgs e)
        {
            IMarkerControl c = e.Source as IMarkerControl;
            if (c != null)
            {
                ActiveMarker = c;
            }
        }

        private static void CollapseAll(Canvas canvas)
        {
            foreach (UIElement element in canvas.Children)
            {
                element.Visibility = Visibility.Collapsed;
            }
        }

        private void DrawRecursive(Canvas canvas, TileSchema schema, ITransform transform, MemoryCache<MemoryStream> memoryCache, Extent extent, int level)
        {
            IList<TileInfo> tiles = schema.GetTilesInView(extent, level);

            foreach (TileInfo tile in tiles)
            {
                MemoryStream image = memoryCache.Find(tile.Index);
                if (image == null)
                {
                    if (level > 0) DrawRecursive(canvas, schema, transform, memoryCache, tile.Extent.Intersect(extent), level - 1);
                }
                else
                {
                    Rect dest = MapTransformHelper.WorldToMap(tile.Extent, transform);
                    double opacity = DrawImage(canvas, image, dest, tile);
                    if ((opacity < 1) && (level > 0)) DrawRecursive(canvas, schema, transform, memoryCache, tile.Extent.Intersect(extent), level - 1);
                }
            }
        }

        private double DrawImage(Canvas canvas, MemoryStream memoryStream, Rect dest, TileInfo tile)
        {
            Image image = images.Find(tile.Index);

            if (image == null)
            {
                image = new Image();
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapSource source = BitmapFrame.Create(memoryStream);
                image.BeginInit();
                image.Source = source;
                image.EndInit();
                images.Add(tile.Index, image);
                Canvas.SetZIndex(image, tile.Index.Level);
                image.Opacity = 0;
                AnimateOpacity(image, 0, 1, 600);
            }

            if (!canvas.Children.Contains(image))
                canvas.Children.Add(image);

            Rect destRounded = new Rect(
                new Point(
                    Math.Round(dest.X) - 0.5,
                    Math.Round(dest.Y) - 0.5),
                new Point(
                    Math.Round(dest.X + dest.Width) + 0.5,
                    Math.Round(dest.Y + dest.Height) + 0.5));

            Canvas.SetLeft(image, destRounded.X);
            Canvas.SetTop(image, destRounded.Y);
            image.Width = destRounded.Width;
            image.Height = destRounded.Height;

            image.Visibility = Visibility.Visible;
            return image.Opacity;
        }

        public static void AnimateOpacity(UIElement target, double from, double to, int duration)
        {
            target.Opacity = 0;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = new TimeSpan(0, 0, 0, 0, duration);

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            Storyboard storyBoard = new Storyboard();
            storyBoard.Children.Add(animation);
            storyBoard.Completed += new EventHandler(storyBoard_Completed);
            storyBoard.Begin();
        }

        static void storyBoard_Completed(object sender, EventArgs e)
        {
            //todo: remove that are now invisible.
           
        }

        private static void RemoveCollapsed(Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                UIElement element = canvas.Children[i];
                if ((element is Image) && (element.Visibility == Visibility.Collapsed))
                {
                    Image image = element as Image;
                    canvas.Children.Remove(image);
                }
            }
        }

        public static void RemoveMarkers(Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                UIElement element = canvas.Children[i];
                if (element is MarkerControl)
                {
                    MarkerControl marker = element as MarkerControl;
                    canvas.Children.Remove(marker);
                }
            }
        }
    }
}
