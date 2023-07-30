﻿using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_HIPB : HipSection
    {
        public int Version = 2;
        public int HasNoLayers;
        public Platform ScoobyPlatform;
        public Dictionary<int, string> LayerNames = new Dictionary<int, string>();

        public Section_HIPB() : base(Section.HIPA)
        {
            HasNoLayers = 0;
            ScoobyPlatform = Platform.Unknown;
            LayerNames = new Dictionary<int, string>();
        }

        public Section_HIPB(BinaryReader binaryReader) : base(binaryReader, Section.HIPB)
        {
            Version = binaryReader.ReadInt32();

            HasNoLayers = 0;
            if (Version >= 1)
                HasNoLayers = Switch(binaryReader.ReadInt32());

            ScoobyPlatform = Platform.Unknown;

            LayerNames = new Dictionary<int, string>();

            if (Version >= 2)
            {
                ScoobyPlatform = (Platform)Switch(binaryReader.ReadInt32());
                int customLayerNameCount = Switch(binaryReader.ReadInt32());
                for (int i = 0; i < customLayerNameCount; i++)
                {
                    int index = Switch(binaryReader.ReadInt32());
                    string layerName = ReadString(binaryReader);
                    LayerNames[index] = layerName;
                }
            }
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.HIPB;

            listBytes.AddBigEndian(Version);
            if (Version >= 1)
                listBytes.AddBigEndian(HasNoLayers);
            if (Version >= 2)
            {
                listBytes.AddBigEndian((int)ScoobyPlatform);
                listBytes.AddBigEndian(LayerNames.Count);
                foreach (int index in LayerNames.Keys)
                {
                    listBytes.AddBigEndian(index);
                    listBytes.AddString(LayerNames[index]);
                }
            }
        }

        public string GetLayerName(int index)
        {
            if (LayerNames.ContainsKey(index))
                return LayerNames[index];
            return null;
        }
    }
}
