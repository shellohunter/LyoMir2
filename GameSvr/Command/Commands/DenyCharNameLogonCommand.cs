using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    
    
    
    [GameCommand("DenyCharNameLogon", "将指定玩家添加到禁止人物列表", "人物名称 是否永久封(0,1)", 10)]
    public class DenyCharNameLogonCommand : BaseCommond
    {
        [DefaultCommand]
        public void DenyCharNameLogon(string[] @Params, TPlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            var sCharName = @Params.Length > 0 ? @Params[0] : "";
            var sFixDeny = @Params.Length > 1 ? @Params[1] : "";
            if (sCharName == "")
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            try
            {
                if (sFixDeny != "" && sFixDeny[0] == '1')
                {
                    
                    M2Share.SaveDenyChrNameList();
                    PlayObject.SysMsg(sCharName + "已加入禁止人物列表", MsgColor.Green, MsgType.Hint);
                }
                else
                {
                    
                    PlayObject.SysMsg(sCharName + "已加入临时禁止人物列表", MsgColor.Green, MsgType.Hint);
                }
            }
            finally
            {
            }
        }
    }
}