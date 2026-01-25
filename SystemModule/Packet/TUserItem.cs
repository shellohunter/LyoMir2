using ProtoBuf;
using System.IO;

namespace SystemModule
{
    [ProtoContract]
    public class TUserItem : Packets
    {
        
        
        
        [ProtoMember(1)]
        public int MakeIndex;
        
        
        
        [ProtoMember(2)]
        public ushort wIndex;
        
        
        
        [ProtoMember(3)]
        public ushort Dura;
        
        
        
        [ProtoMember(4)]
        public ushort DuraMax;
        [ProtoMember(5)]
        public byte[] btValue;

        public TUserItem()
        {
            btValue = new byte[14];
        }

        public TUserItem(TUserItem userItem)
        {
            this.MakeIndex = userItem.MakeIndex;
            this.wIndex = userItem.wIndex;
            this.Dura = userItem.Dura;
            this.DuraMax = userItem.DuraMax;
            this.btValue = userItem.btValue;
        }

        protected override void ReadPacket(BinaryReader reader)
        {
            this.MakeIndex = reader.ReadInt32();
            this.wIndex = reader.ReadUInt16();
            this.Dura = reader.ReadUInt16();
            this.DuraMax = reader.ReadUInt16();
            this.btValue = reader.ReadBytes(14);
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(MakeIndex);
            writer.Write(wIndex);
            writer.Write(Dura);
            writer.Write(DuraMax);
            writer.Write(btValue);
        }
    }
}