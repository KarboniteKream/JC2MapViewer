// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Threading;
using BruTile.Cache;

namespace BruTile.UI.Fetcher
{
    class TileFetcher<T> 
    {
        #region Fields

        private MemoryCache<T> memoryCache;
        private TileSource tileSource;
        private ITileFactory<T> tileFactory;
        private Extent extent;
        private double resolution;
        private IList<TileIndex> tilesInProgress = new List<TileIndex>();
        private IList<TileInfo> infos = new List<TileInfo>();
        private IDictionary<TileIndex, int> retries = new Dictionary<TileIndex, int>();
        private int threadMax = 4;
        private int threadCount = 0;
        private AutoResetEvent waitHandle = new AutoResetEvent(true);
        private FetchStrategy strategy = new FetchStrategy();
        private int maxRetries = 2;
        private Thread loopThread;
        private volatile bool isDone = true;
        private volatile bool isViewChanged = false;
        
        #endregion

        #region EventHandlers

        public event DataChangedEventHandler DataChanged;

        #endregion

        #region Constructors Destructors

        public TileFetcher(TileSource source, MemoryCache<T> memoryCache, ITileFactory<T> tileFactory)
        {
            if (source == null) throw new ArgumentException("TileProvider can not be null");
            this.tileSource = source;

            if (memoryCache == null) throw new ArgumentException("MemoryCache can not be null");
            this.memoryCache = memoryCache;

            if (tileFactory == null) throw new ArgumentException("ITileFactory can not be null");
            this.tileFactory = tileFactory;
        }

        ~TileFetcher()
        {
        }

        #endregion

        #region Public Methods

        public void ViewChanged(Extent extent, double resolution)
        {
            this.extent = extent;
            this.resolution = resolution;
            isViewChanged = true;

            if (isDone)
            {
                isDone = false;
                loopThread = new Thread(TileFetchLoop);
                loopThread.IsBackground = true;
                loopThread.Name = "LoopThread";
                Console.WriteLine("Start Thread");
                loopThread.Start();
            }
        }

        public void AbortFetch()
        {
            isDone = true;
            waitHandle.Set();
        }

        #endregion

        #region Private Methods

        private void TileFetchLoop()
        {
            try
            {
                System.Threading.Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                waitHandle.Set();

                while (!isDone)
                {
                    waitHandle.WaitOne();

                    if (isViewChanged)
                    {
                        int level = BruTile.Utilities.GetNearestLevel(tileSource.Schema.Resolutions, resolution);
                        infos = strategy.GetTilesWanted(tileSource.Schema, extent, level);
                        retries.Clear();
                        isViewChanged = false;
                    }

                    infos = GetTilesMissing(infos, memoryCache);

                    FetchTiles();

                    Console.WriteLine(threadCount);

                    if (this.infos.Count == 0)
                    {
                        isDone = true;
                        waitHandle.Set();
                    }

                    if (threadCount >= threadMax)
                        waitHandle.Reset();
                }
            }
            finally
            {
                isDone = true;
            }
        }

        private IList<TileInfo> GetTilesMissing(IList<TileInfo> infosIn, MemoryCache<T> memoryCache)
        {
            IList<TileInfo> tilesOut = new List<TileInfo>();
            foreach (TileInfo info in infosIn)
            {
                if ((memoryCache.Find(info.Index) == null) &&
                    (!retries.Keys.Contains(info.Index) || retries[info.Index] < maxRetries))

                    tilesOut.Add(info);
            }
            return tilesOut;
        }

        private void FetchTiles()
        {
            foreach (TileInfo info in infos)
            {
                if (threadCount >= threadMax) return;
                FetchTile(info);
            }
        }

        private void FetchTile(TileInfo info)
        {
            //first a number of checks
            if (tilesInProgress.Contains(info.Index)) return;
            if (retries.Keys.Contains(info.Index) && retries[info.Index] >= maxRetries) return;
            if (memoryCache.Find(info.Index) != null) return;

            //now we can go for the request.
            lock (tilesInProgress) { tilesInProgress.Add(info.Index); }
            if (!retries.Keys.Contains(info.Index)) retries.Add(info.Index, 0);
            else retries[info.Index]++;
            threadCount++;
            StartFetchOnThread(info);
        }

        private void StartFetchOnThread(TileInfo info)
        {
            FetchOnThread fetchOnThread = new FetchOnThread(tileSource.Provider, info, new FetchTileCompletedEventHandler(LocalFetchCompleted));
            Thread thread = new Thread(fetchOnThread.FetchTile);
            thread.Name = "Tile Fetcher";
            Console.WriteLine("Start tile fetcher");
            thread.Start();
        }

        private void LocalFetchCompleted(object sender, FetchTileCompletedEventArgs e)
        {
            //todo remove object sender
            try
            {
                if (e.Error is System.IO.FileNotFoundException)
                {
                    return;
                }
                if (e.Error == null && e.Cancelled == false)
                    memoryCache.Add(e.TileInfo.Index, tileFactory.GetTile(e.Image));
            }
            catch (Exception ex)
            {
                e.Error = ex;
            }
            finally
            {
                threadCount--;
                lock (tilesInProgress)
                {
                    if (tilesInProgress.Contains(e.TileInfo.Index))
                        tilesInProgress.Remove(e.TileInfo.Index);
                }
                waitHandle.Set();
            }

            if (this.DataChanged != null)
                this.DataChanged(this, new DataChangedEventArgs(e.Error, e.Cancelled, e.TileInfo.Extent));
        }

        #endregion
    }

    public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs e);

    public class DataChangedEventArgs
    {
        public DataChangedEventArgs(Exception error, bool cancelled, Extent extent)
        {
            this.Error = error;
            this.Cancelled = cancelled;
            this.extent = extent;
        }

        public Exception Error;
        public bool Cancelled;
        public Extent extent;
    }


}
