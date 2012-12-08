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
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace BruTile
{
    public class TileSchema
    {
        #region Fields
        private string srs;
        private Extent extent;
        private double originX = Double.NaN;
        private double originY = Double.NaN;
        private List<double> resolutions = new List<double>();
        private int width;
        private int height;
        private string format;

        #endregion

        #region Properties

        public string Srs
        {
            get { return srs; }
            set { srs = value; }
        }

        public Extent Extent
        {
            get { return extent; }
            set { extent = value; }
        }

        public double OriginX
        {
            get { return originX; }
            set { originX = value; }
        }

        public double OriginY
        {
            get { return originY; }
            set { originY = value; }
        }

        public IList<double> Resolutions
        {
            get { return resolutions; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        /*
        public AxisDirection Axis
        {
            get { return axisDirection; }
            set 
            { 
                axisDirection = value;
                axis = CreateAxis(value);
            }
        }*/
        #endregion

        public TileSchema()
        {
            double[] resolutions = new double[] { 128, 64, 32, 16, 8, 4 };
            foreach (double resolution in resolutions) this.Resolutions.Add(resolution);
            this.Height = 256;
            this.Width = 256;
            this.Extent = new Extent(-32768, -32768, 32768, 32768);
            this.OriginX = 0;
            this.OriginY = 0;
            this.Format = "jpg";
        }

        #region Public Methods

        /// <summary>
        /// Returns a List of TileInfos that cover the provided extent. 
        /// </summary>
        public IList<TileInfo> GetTilesInView(Extent extent, double resolution)
        {
            int level = Utilities.GetNearestLevel(Resolutions, resolution);
            return GetTilesInView(extent, level);
        }

        private TileRange WorldToTile(Extent extent, int level)
        {
            double resolution = Resolutions[level];
            double tileWorldUnits = resolution * Width;
            int firstCol = (int)Math.Floor((extent.MinX - OriginX) / tileWorldUnits);
            int firstRow = (int)Math.Floor((-extent.MaxY + OriginY) / tileWorldUnits);
            int lastCol = (int)Math.Ceiling((extent.MaxX - OriginX) / tileWorldUnits);
            int lastRow = (int)Math.Ceiling((-extent.MinY + OriginY) / tileWorldUnits);
            return new TileRange(firstCol, firstRow, lastCol, lastRow);
        }

        private Extent TileToWorld(TileRange range, int level)
        {
            double resolution = Resolutions[level];
            double tileWorldUnits = resolution * Width;
            double minX = range.FirstCol * tileWorldUnits + OriginX;
            double minY = -(range.LastRow + 1) * tileWorldUnits + OriginY;
            double maxX = (range.LastCol + 1) * tileWorldUnits + OriginX;
            double maxY = -(range.FirstRow) * tileWorldUnits + OriginY;
            return new Extent(minX, minY, maxX, maxY);
        }
        
        public IList<TileInfo> GetTilesInView(Extent extent, int level)
        {
            IList<TileInfo> infos = new List<TileInfo>();
            TileRange range = WorldToTile(extent, level);
            infos.Clear();

            for (int x = range.FirstCol; x < range.LastCol; x++)
            {
                for (int y = range.FirstRow; y < range.LastRow; y++)
                {
                    TileInfo info = new TileInfo();
                    info.Extent = TileToWorld(new TileRange(x, y), level);
                    info.Index = new TileIndex(x, y, level);

                    if (WithinSchemaExtent(Extent, info.Extent))
                    {
                        infos.Add(info);
                    }
                }
            }
            return infos;
        }

        public Extent GetExtentOfTilesInView(Extent extent, int level)
        {
            TileRange range = WorldToTile(extent, level);
            return TileToWorld(range, level);
        }

        #endregion

        #region Private Methods

        private static bool WithinSchemaExtent(Extent schemaExtent, Extent tileExtent)
        {
            //Always return false when tile is outsize of schema
            if (!tileExtent.Intersects(schemaExtent)) return false;

            //Do not always accept when the tile is partially inside the schema. 
            //Reject tiles that have less than 0.1% percent overlap.
            //In practice they turn out to be mostly false positives due to rounding errors.
            //They are not present on the server and the failed requests make slow the application down.
            return ((tileExtent.Intersect(schemaExtent).Area / tileExtent.Area) > 0.001);
        }
        #endregion
    }

}
