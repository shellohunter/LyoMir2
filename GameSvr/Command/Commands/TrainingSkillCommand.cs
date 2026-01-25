using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    
    
    
    [GameCommand("TrainingSkill", "调整指定玩家技能等级", "人物名称  技能名称 修炼等级(0-3)", 10)]
    public class TrainingSkillCommand : BaseCommond
    {
        [DefaultCommand]
        public void TrainingSkill(string[] @Params, TPlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            var sHumanName = @Params.Length > 0 ? @Params[0] : "";
            var sSkillName = @Params.Length > 1 ? @Params[1] : "";
            var nLevel = @Params.Length > 2 ? int.Parse(@Params[2]) : 0;
            TUserMagic UserMagic;
            TPlayObject m_PlayObject;
            if (string.IsNullOrEmpty(sHumanName) || sSkillName == "" || nLevel <= 0)
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            nLevel = HUtil32._MIN(3, nLevel);
            m_PlayObject = M2Share.UserEngine.GetPlayObject(sHumanName);
            if (m_PlayObject == null)
            {
                PlayObject.SysMsg($"{sHumanName}不在线，或在其它服务器上!!", MsgColor.Red, MsgType.Hint);
                return;
            }
            for (var i = 0; i < m_PlayObject.m_MagicList.Count; i++)
            {
                UserMagic = m_PlayObject.m_MagicList[i];
                
                
                
                
                
                
                
                
            }
        }
    }
}