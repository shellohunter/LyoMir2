using System;
using System.IO;

namespace GameGate
{
    
    
    
    public class GateConfig
    {
        public bool m_fAddLog;
        
        
        
        public int ShowLogLevel;
        
        
        
        public bool ShowDebugLog;
        
        
        
        public int GateCount;
        
        
        
        public bool CheckNullSession;
        
        
        
        public bool IsOverSpeedSendBack;
        
        
        
        public bool IsDefenceCCPacket;
        public bool IsKickOverSpeed;
        public bool IsDenyPresend;
        public bool IsItemSpeedCompensate;
        public bool IsDoMotaeboSpeedCheck;
        public bool IsKickOverPacketSize;
        public int MaxConnectOfIP;
        public int MaxClientCount;
        public int ClientTimeOutTime;
        public int NomClientPacketSize;
        public int MaxClientPacketSize;
        public int MaxClientPacketCount;
        public string m_szCMDSpaceMove;
        public string m_szOverClientCntMsg;
        public string m_szHWIDBlockedMsg;
        public string m_szChatFilterReplace;
        public string m_szOverSpeedSendBack;
        public string m_szPacketDecryptFailed;
        public string m_szBlockHWIDFileName;
        
        
        
        public bool IsChatFilter;
        
        
        
        public bool IsChatInterval;
        
        
        
        public bool IsChatCmdFilter;
        
        
        
        public bool IsTurnInterval;
        
        
        
        public bool IsMoveInterval;
        
        
        
        public bool IsSpellInterval;
        
        
        
        public bool IsAttackInterval;
        
        
        
        public bool IsButchInterval;
        
        
        
        public bool IsSitDownInterval;
        
        
        
        public bool IsSpaceMoveNextPickupInterval;
        
        
        
        public bool IsPickupInterval;
        
        
        
        public bool IsEatInterval;
        
        
        
        public bool IsProcClientHardwareID;
        
        
        
        
        public string ProClientHardwareKey;
        
        
        
        public int ChatInterval;
        
        
        
        public int TurnInterval;
        
        
        
        public int MoveInterval;
        
        
        
        public int SpellNextInterval;
        
        
        
        public int AttackInterval;
        
        
        
        public int ButchInterval;
        
        
        
        public int SitDownInterval;
        
        
        
        public int PickupInterval;
        
        
        
        public int EatInterval;
        
        
        
        public int MoveNextSpellCompensate;
        
        
        
        public int MoveNextAttackCompensate;
        
        
        
        public int AttackNextMoveCompensate;
        
        
        
        public int AttackNextSpellCompensate;
        
        
        
        public int SpellNextMoveCompensate;
        
        
        
        public int SpellNextAttackCompensate;
        
        
        
        public int SpaceMoveNextPickupInterval;
        
        
        
        public int PunishBaseInterval;
        
        
        
        
        public double PunishIntervalRate;
        public int PunishMoveInterval;
        public int PunishSpellInterval;
        public int PunishAttackInterval;
        public int MaxItemSpeed;
        public int MaxItemSpeedRate;
        
        
        
        public bool ClientShowHintNewType;
        public bool OpenClientSpeedRate;
        public bool SyncClientSpeed;
        public int ClientMoveSpeedRate;
        public int ClientSpellSpeedRate;
        public int ClientAttackSpeedRate;
        public TPunishMethod OverSpeedPunishMethod;
        public TBlockIPMethod BlockIPMethod;
        public TChatFilterMethod ChatFilterMethod;
        public TOverSpeedMsgMethod SpeedHackWarnMethod;

        public GateConfig()
        {
            CheckNullSession = true;
            IsOverSpeedSendBack = false;
            IsDefenceCCPacket = false;
            IsKickOverSpeed = false;
            IsDenyPresend = false;
            IsItemSpeedCompensate = false;
            IsDoMotaeboSpeedCheck = true;
            IsKickOverPacketSize = true;
            BlockIPMethod = TBlockIPMethod.mDisconnect;
            NomClientPacketSize = 400;
            MaxClientPacketSize = 10240;
            MaxConnectOfIP = 50;
            MaxClientCount = 50;
            ClientTimeOutTime = 15 * 1000;
            MaxClientPacketCount = 15;
            m_szOverSpeedSendBack = "[提示]：请爱护游戏环境，关闭加速外挂重新登陆！";
            m_szCMDSpaceMove = "Move";
            m_szPacketDecryptFailed = "[警告]：游戏连接被断开，请重新登陆！原因：使用非法外挂，客户端不配套，开启的客户端数量过多。";
            m_szOverClientCntMsg = "开启游戏过多，链接被断开！";
            m_szHWIDBlockedMsg = "机器码已被封，链接被断开！";
            m_szChatFilterReplace = "说话内容被屏蔽";
            m_szBlockHWIDFileName = Path.Combine(AppContext.BaseDirectory, "BlockHWID.txt");
            IsChatCmdFilter = false;
            IsChatFilter = true;
            IsChatInterval = true;
            IsTurnInterval = true;
            IsMoveInterval = true;
            IsSpellInterval = true;
            IsAttackInterval = true;
            IsButchInterval = true;
            IsSitDownInterval = true;
            IsSpaceMoveNextPickupInterval = true;
            IsPickupInterval = true;
            IsEatInterval = true;
            IsProcClientHardwareID = false;
            ProClientHardwareKey = "openmir2";
            ChatInterval = 800;
            TurnInterval = 350;
            MoveInterval = 570;
            AttackInterval = 900;
            ButchInterval = 450;
            SitDownInterval = 450;
            PickupInterval = 330;
            EatInterval = 330;
            MoveNextSpellCompensate = 100;
            MoveNextAttackCompensate = 250;
            AttackNextMoveCompensate = 200;
            AttackNextSpellCompensate = 200;
            SpellNextMoveCompensate = 200;
            SpellNextAttackCompensate = 200;
            SpaceMoveNextPickupInterval = 600;
            PunishBaseInterval = 20;
            PunishIntervalRate = 1.00;
            OverSpeedPunishMethod = TPunishMethod.DelaySend;
            PunishMoveInterval = 150;
            PunishSpellInterval = 150;
            PunishAttackInterval = 150;
            ChatFilterMethod = TChatFilterMethod.ctReplaceAll;
            SpeedHackWarnMethod = TOverSpeedMsgMethod.ptSysmsg;
            MaxItemSpeed = 6;
            MaxItemSpeedRate = 60;
            ClientShowHintNewType = true;
            OpenClientSpeedRate = false;
            SyncClientSpeed = false;
            ClientMoveSpeedRate = 0;
            ClientSpellSpeedRate = 0;
            ClientAttackSpeedRate = 0;
            ShowDebugLog = false;
        }
    }
}