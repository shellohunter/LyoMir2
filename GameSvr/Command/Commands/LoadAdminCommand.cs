using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    
    
    
    [GameCommand("LoadAdmin", "重新加载管理员列表", 10)]
    public class LoadAdminCommand : BaseCommond
    {
        [DefaultCommand]
        public void LoadAdmin(TPlayObject PlayObject)
        {
            if (PlayObject.m_btPermission < 6)
            {
                return;
            }
            
            
            PlayObject.SysMsg("管理员列表重新加载成功...", MsgColor.Green, MsgType.Hint);
        }
    }
}