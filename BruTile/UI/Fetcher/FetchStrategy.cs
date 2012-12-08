// Copyright 2009 - Paul den Dulk (Geodan)
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
using BruTile;

namespace BruTile.UI.Fetcher
{
    class FetchStrategy //: IFetchStrategy
    {
        private Sorter sorter = new Sorter();

        public IList<TileInfo> GetTilesWanted(TileSchema schema, Extent extent, int level)
        {
            IList<TileInfo> infos = new List<TileInfo>();
            int step = 1;
            // Iterating through all levels from current to zero. If lower levels are
            // not availeble the renderer can fall back on higher level tiles. 
            while (level >= 0)
            {
                IList<TileInfo> infosOfLevel = schema.GetTilesInView(extent, level);
                infosOfLevel = PrioritizeTiles(infosOfLevel, extent.CenterX, extent.CenterY, sorter);

                foreach (TileInfo info in infosOfLevel)
                {
                    if ((info.Index.Row >= 0) && (info.Index.Col >= 0)) infos.Add(info);
                }
                level = level - step;
                step++;
            }

            return infos;
        }

        /// <summary>
        /// Puts the tiles in the order in which they should be retrieved. Tiles close to the center
        /// come first.
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private static List<TileInfo> PrioritizeTiles(IList<TileInfo> inTiles, double centerX, double centerY, Sorter sorter)
        {
            List<TileInfo> infos = new List<TileInfo>(inTiles);

            for (int i = 0; i < infos.Count; i++)
            {
                double priority = -Utilities.Distance(centerX, centerY, infos[i].Extent.CenterX, infos[i].Extent.CenterY);
                infos[i].Priority = priority;
            }

            infos.Sort(sorter);
            return infos;
        }       

        private class Sorter : IComparer<TileInfo>
        {
            public int Compare(TileInfo x, TileInfo y)
            {
                if (x.Priority > y.Priority) return -1;
                if (x.Priority < y.Priority) return 1;
                return 0;
            }
        }
    }
}
