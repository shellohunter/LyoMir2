using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    
    
    
    [GameCommand("ShutupList", "查看禁言列表中的内容", 10)]
    public class ShutupListCommand : BaseCommond
    {
        [DefaultCommand]
        public void ShutupList(TPlayObject PlayObject)
        {
            HUtil32.EnterCriticalSection(M2Share.g_DenySayMsgList);
            try
            {
                var nCount = M2Share.g_DenySayMsgList.Count;
                if (M2Share.g_DenySayMsgList.Count <= 0)
                {
                    PlayObject.SysMsg(M2Share.g_sGameCommandShutupListIsNullMsg, MsgColor.Green, MsgType.Hint);
                }
                if (nCount > 0)
                {
                    
                    
                    
                    
                    

                    
                    
                    
                    
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.g_DenySayMsgList);
            }
        }
    }
}