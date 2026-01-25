using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    
    
    
    [GameCommand("EnableSendMsg", "从禁言列表中删除指定玩家", "人物名称", 10)]
    public class EnableSendMsgCommand : BaseCommond
    {
        [DefaultCommand]
        public void EnableSendMsg(string[] @params, TPlayObject PlayObject)
        {
            if (@params == null)
            {
                return;
            }
            var sHumanName = @params.Length > 0 ? @params[0] : "";
            if (string.IsNullOrEmpty(sHumanName))
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            for (var i = M2Share.g_DisableSendMsgList.Count - 1; i >= 0; i--)
            {
                if (M2Share.g_DisableSendMsgList.Count <= 0)
                {
                    break;
                }
                
                
                
                
                
                
                
                
                
                
                
                
            }
            PlayObject.SysMsg(sHumanName + " 没有被禁言!!!", MsgColor.Red, MsgType.Hint);
        }
    }
}