using SystemModule;

namespace GameSvr
{
    
    
    
    public class TClientDealOffInfo : Packets
    {
        
        
        
        public string sDealCharName;
        
        
        
        public string sBuyCharName;
        
        
        
        public double dSellDateTime;
        
        
        
        public int nSellGold;
        
        
        
        public TClientItem[] UseItems;
        
        
        
        public byte N;

        protected override void ReadPacket(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(sDealCharName.ToByte(15));
            writer.Write(sBuyCharName.ToByte(15));
            writer.Write(dSellDateTime);
            writer.Write(nSellGold);
            var nullItem = new TClientItem();
            var nullBuff = nullItem.GetBuffer();
            for (int i = 0; i < UseItems.Length; i++)
            {
                if (UseItems[i] == null)
                {
                    writer.Write(nullBuff);
                }
                else
                {
                    writer.Write(UseItems[i].GetBuffer());
                }
            }
            writer.Write(N);
        }
    }
}