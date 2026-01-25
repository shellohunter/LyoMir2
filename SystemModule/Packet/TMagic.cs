using System.IO;

namespace SystemModule
{
    public class TMagic : Packets
    {
        
        
        
        public ushort wMagicID;
        
        
        
        public string sMagicName;
        
        
        
        public byte btEffectType;
        
        
        
        public byte btEffect;
        
        
        
        public ushort wSpell;
        
        
        
        public ushort wPower;
        
        
        
        public byte[] TrainLevel;
        
        
        
        public int[] MaxTrain;
        
        
        
        public byte btTrainLv;
        
        
        
        public byte btJob;
        
        
        
        public int dwDelayTime;
        
        
        
        public byte btDefSpell;
        
        
        
        public byte btDefPower;
        
        
        
        public ushort wMaxPower;
        
        
        
        public byte btDefMaxPower;
        
        
        
        public string sDescr;

        public TMagic()
        {
            TrainLevel = new byte[4];
            MaxTrain = new int[4];
        }

        protected override void ReadPacket(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(wMagicID);
            writer.Write(sMagicName.ToByte(13));
            writer.Write(btEffectType);
            writer.Write(btEffect);
            writer.Write((byte)0);
            writer.Write(wSpell);
            writer.Write(wPower);
            writer.Write(TrainLevel);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(MaxTrain[0]);
            writer.Write(MaxTrain[1]);
            writer.Write(MaxTrain[2]);
            writer.Write(MaxTrain[3]);
            writer.Write(btTrainLv);
            writer.Write(btJob);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(dwDelayTime);
            writer.Write(btDefSpell);
            writer.Write(btDefPower);
            writer.Write(wMaxPower);
            writer.Write(btDefMaxPower);
            writer.Write(sDescr.ToByte(19));
        }
    }
}