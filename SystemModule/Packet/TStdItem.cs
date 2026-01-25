using System;
using System.IO;

namespace SystemModule
{
    public class TStdItem : Packets
    {
        
        
        
        public string Name;

        
        
        
        public byte StdMode;

        
        
        
        public byte Shape;

        
        
        
        public byte Weight;

        public byte AniCount;

        
        
        
        public sbyte Source;

        public byte reserved;

        
        
        
        public byte NeedIdentify;

        
        
        
        public ushort Looks;

        
        
        
        public int DuraMax;

        
        
        
        public int AC;

        
        
        
        public int MAC;

        
        
        
        public int DC;

        
        
        
        public int MC;

        
        
        
        public int SC;

        
        
        
        public int Need;

        
        
        
        public int NeedLevel;

        
        
        
        public uint Price;

        protected override void ReadPacket(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            var nameBuff = HUtil32.StringToByteAry(Name, out int nameLen);
            nameBuff[0] = (byte)nameLen;
            Array.Resize(ref nameBuff, 21);
            writer.Write(nameBuff);
            writer.Write(StdMode);
            writer.Write(Shape);
            writer.Write(Weight);
            writer.Write(AniCount);
            writer.Write(Source);
            writer.Write(reserved);
            writer.Write(NeedIdentify);
            writer.Write(Looks);
            writer.Write(DuraMax);
            writer.Write(AC);
            writer.Write(MAC);
            writer.Write(DC);
            writer.Write(MC);
            writer.Write(SC);
            writer.Write(Need);
            writer.Write(NeedLevel);
            writer.Write(Price);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }
}