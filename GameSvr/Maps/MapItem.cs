using SystemModule;

namespace GameSvr
{
    public class MapItem
    {
        
        
        
        public int Id;
        
        
        
        public string Name;
        
        
        
        public ushort Looks;
        public byte AniCount;
        public int Reserved;
        
        
        
        public int Count;
        public object DropBaseObject;
        public object OfBaseObject;
        
        
        
        public int CanPickUpTick;
        public TUserItem UserItem;

        public MapItem()
        {
            Id = HUtil32.Sequence();
        }
    }
}