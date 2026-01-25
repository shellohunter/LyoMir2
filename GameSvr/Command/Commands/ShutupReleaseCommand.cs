using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    [GameCommand("ShutupRelease", "恢复禁言", M2Share.g_sGameCommandShutupReleaseHelpMsg, 10)]
    public class ShutupReleaseCommand : BaseCommond
    {
        [DefaultCommand]
        public void ShutupRelease(string[] @params, TPlayObject PlayObject)
        {
            if (@params == null)
            {
                return;
            }
            var sHumanName = @params.Length > 0 ? @params[0] : "";
            var boAll = @params.Length > 1 ? bool.Parse(@params[1]) : false;
            if (string.IsNullOrEmpty(sHumanName) || !string.IsNullOrEmpty(sHumanName))
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            HUtil32.EnterCriticalSection(M2Share.g_DenySayMsgList);
            try
            {
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.g_DenySayMsgList);
            }
        }
    }
}