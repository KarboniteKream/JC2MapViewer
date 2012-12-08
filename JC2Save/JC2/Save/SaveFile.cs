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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace JC2.Save
{
    public class SaveFile
    {
        public SaveFileHeader Header { get; private set; }
        
        const int CHUNK_HEADERS_OFFSET = 512;
        const int OFFSET_XBOX360 = 53248;
        const int CHUNK_DATA_OFFSET = 236032;

        private readonly List<ChunkData> _chunkData = new List<ChunkData>();
        private readonly List<IParsedChunk> _parsedChunks = new List<IParsedChunk>();
        public ReadOnlyCollection<IParsedChunk> ParsedChunks
        {
            get { return _parsedChunks.AsReadOnly(); }
        }

        public SaveFile(string fileName)
        {
            Read(File.OpenRead(fileName));
        }

        public void Read(Stream input)
        {
            // lazy check... pc savegames are exactly 891904 bytes...
            bool isXBox = input.Length != 891904;
            int _headerOffset;
            int _chunkHeaderOffset;
            int _chunkDataOffset;

            if (isXBox) EndianessSwitchableBitConverter.Mode = EndianessSwitchableBitConverter.Endianess.BIG;

            BinaryReader r;
            if (isXBox)
            {
                r = new BigEndianBinaryReader(input);
                _headerOffset = OFFSET_XBOX360;
                _chunkHeaderOffset = CHUNK_HEADERS_OFFSET + OFFSET_XBOX360;
                _chunkDataOffset = CHUNK_DATA_OFFSET + OFFSET_XBOX360;
            }
            else
            {
                r = new BinaryReader(input);
                _headerOffset = 0;
                _chunkHeaderOffset = CHUNK_HEADERS_OFFSET;
                _chunkDataOffset = CHUNK_DATA_OFFSET;
            }

            r.BaseStream.Seek(_headerOffset, SeekOrigin.Begin);

            Header = new SaveFileHeader();
            Header.Read(r);

            // file can contain up to 9 headers - probably history, BUT can also contain junk, so read only first header....

            r.BaseStream.Seek(_chunkHeaderOffset, SeekOrigin.Begin);
            int count = r.ReadInt32();
            List<ChunkHeader> _chunkHeaders = new List<ChunkHeader>(count);
            for (int i = 0; i < count; i++)
            {
                ChunkHeader header = new ChunkHeader(r);
                _chunkHeaders.Add(header);
            }

            _chunkData.Clear();
            _parsedChunks.Clear();
            for (int i = 0; i < count; i++)
            {
                ChunkData data = new ChunkData();
                data.Read(r, _chunkHeaders[i], _chunkDataOffset);
                _chunkData.Add(data);
                
                IParsedChunk p = ParsedChunkTypeHelper.TryParse(data);
                _parsedChunks.Add(p);

                /*
                if (p == null)
                {
                    foreach (ChunkDataEntry e in data.DataEntries)
                    {
                        if (e.Data.Length == 4)
                        {
                            uint hash = BitConverter.ToUInt32(e.Data, 0);
                            string name = NameLookup.GetName(hash);
                            if (name != null)
                            {
                                //System.Diagnostics.Debugger.Break();
                            }
                        }
                    }
                }*/
            }
            r.Close();
            input.Close();
        }

        public Dictionary<string, SavedSettlementInfo> GetSettlementsInfo()
        {
            Dictionary<string, SavedSettlementInfo> settlements = new Dictionary<string, SavedSettlementInfo>();
            foreach (var chunk in ParsedChunks)
            {
                if (chunk is IContainsSettlementInfo)
                {
                    IContainsSettlementInfo info = chunk as IContainsSettlementInfo;
                    if (info != null)
                    {
                        foreach (var o in info.GetSettlementsInfo())
                        {
                            settlements.Add(o.Key, o.Value);
                        }
                    }
                }
            }
            return settlements;
        }

        public List<SavedObjectInfo> GetSavedObjectInfo(out Dictionary<string, int> counts)
        {
            counts = new Dictionary<string, int>(SavedObjectInfoLookup.TotalCountByCategory);
            List<SavedObjectInfo> returnValue = new List<SavedObjectInfo>();

            foreach (var chunk in ParsedChunks)
            {
                if (chunk is IContainsSavedObjectInfo)
                {
                    IContainsSavedObjectInfo info = chunk as IContainsSavedObjectInfo;
                    if (info != null)
                    {
                        returnValue.AddRange(info.GetSavedObjectInfo(counts));

                    }
                }
            }
            return returnValue;
        }
    }
}
