using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    [GameCommand("DelDenyAccountLogon", "", "登录帐号", 10)]
    public class DelDenyAccountLogonCommand : BaseCommond
    {
        [DefaultCommand]
        public void DelDenyAccountLogon(string[] @Params, TPlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            var sAccount = @Params.Length > 0 ? @Params[0] : "";
            var sFixDeny = @Params.Length > 1 ? @Params[1] : "";
            if (sAccount == "")
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            var boDelete = false;
            for (var i = 0; i < M2Share.g_DenyAccountList.Count; i++)
            {
                
                
                
                
                
                
                
                
                
                
                
            }
            if (!boDelete)
            {
                PlayObject.SysMsg(sAccount + "没有被禁止登录。", MsgColor.Green, MsgType.Hint);
            }
        }
    }
}