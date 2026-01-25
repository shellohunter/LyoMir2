using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    [GameCommand("DelDenyIPaddrLogon", "", "IP地址", 10)]
    public class DelDenyIPaddrLogonCommand : BaseCommond
    {
        [DefaultCommand]
        public void DelDenyIPaddrLogon(string[] @Params, TPlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            var sIPaddr = @Params.Length > 0 ? @Params[0] : "";
            if (sIPaddr == "")
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            var boDelete = false;
            try
            {
                for (var i = M2Share.g_DenyIPAddrList.Count - 1; i >= 0; i--)
                {
                    if (M2Share.g_DenyIPAddrList.Count <= 0)
                    {
                        break;
                    }
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                }
            }
            finally
            {
            }
            if (!boDelete)
            {
                PlayObject.SysMsg(sIPaddr + "没有被禁止登录。", MsgColor.Green, MsgType.Hint);
            }
        }
    }
}