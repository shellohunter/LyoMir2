using ProtoBuf;
using System;
using System.IO;

namespace SystemModule
{
    [ProtoContract]
    public class TMagicRcd : Packets
    {
        
        
        
        [ProtoMember(1)]
        public ushort wMagIdx;
        
        
        
        [ProtoMember(2)]
        public byte btLevel;
        
        
        
        [ProtoMember(3)]
        public byte btKey;
        
        
        
        [ProtoMember(4)]
        public int nTranPoint;

        protected override void ReadPacket(BinaryReader reader)
        {
            this.wMagIdx = reader.ReadUInt16();
            this.btLevel = reader.ReadByte();
            this.btKey = reader.ReadByte();
            this.nTranPoint = reader.ReadInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(wMagIdx);
            writer.Write(btLevel);
            writer.Write(btKey);
            writer.Write(nTranPoint);
        }
    }
}