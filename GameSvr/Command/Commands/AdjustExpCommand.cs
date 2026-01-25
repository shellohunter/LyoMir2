using GameSvr.CommandSystem;

namespace GameSvr
{
    
    
    
    [GameCommand("AdjustExp", "", 10)]
    public class AdjustExpCommand : BaseCommond
    {
        [DefaultCommand]
        public void AdjustExp(string[] @Params, TPlayObject PlayObject)
        {
            if (PlayObject.m_btPermission < 6)
            {
                return;
            }
        }
    }
}