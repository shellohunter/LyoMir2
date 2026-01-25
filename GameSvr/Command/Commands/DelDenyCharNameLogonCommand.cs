using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    [GameCommand("DelDenyCharNameLogon", "", "人物名称", 10)]
    public class DelDenyCharNameLogonCommand : BaseCommond
    {
        [DefaultCommand]
        public void DelDenyCharNameLogon(string[] @Params, TPlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            var sCharName = @Params.Length > 0 ? @Params[0] : "";
            if (sCharName == "")
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            var boDelete = false;
            try
            {
                for (var i = 0; i < M2Share.g_DenyChrNameList.Count; i++)
                {
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                }
            }
            finally
            {
            }
            if (!boDelete)
            {
                PlayObject.SysMsg(sCharName + "没有被禁止登录。", MsgColor.Green, MsgType.Hint);
            }
        }
    }
}