using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile
{
    public class TileSource
    {
        FileTileProvider tileProvider;
        TileSchema tileSchema;

        public TileSource(FileTileProvider tileProvider, TileSchema tileSchema)
        {
            this.tileProvider = tileProvider;
            this.tileSchema = tileSchema;
        }

        public FileTileProvider Provider
        {
            get { return tileProvider; }
        }

        public TileSchema Schema
        {
            get { return tileSchema; }
        }
    }
}
