using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    [GameCommand("ShowDenyIPaddrLogon", "", 10)]
    public class ShowDenyIPaddrLogonCommand : BaseCommond
    {
        [DefaultCommand]
        public void ShowDenyIPaddrLogon(TPlayObject PlayObject)
        {
            int nCount;
            try
            {
                nCount = M2Share.g_DenyIPAddrList.Count;
                if (M2Share.g_DenyIPAddrList.Count <= 0)
                {
                    PlayObject.SysMsg("禁止登录角色列表为空。", MsgColor.Green, MsgType.Hint);
                }
                if (nCount > 0)
                {
                    for (var i = 0; i < M2Share.g_DenyIPAddrList.Count; i++)
                    {
                        
                    }
                }
            }
            finally
            {
            }
        }
    }
}