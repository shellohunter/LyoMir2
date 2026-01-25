
namespace SelGate.Conf
{
    
    
    
    public class GateConfig
    {
        public int m_nShowLogLevel = 0;
        public bool ShowDebugLog = false;
        public int m_nGateCount = 0;
        public bool m_fCheckNewIDOfIP = false;
        public bool m_fCheckNullSession = false;
        public bool m_fOverSpeedSendBack = false;
        public bool m_fDefenceCCPacket = false;
        public bool m_fKickOverSpeed = false;
        public bool m_fKickOverPacketSize = false;
        
        public bool m_fAllowGetBackChr = false;
        
        public bool m_fAllowDeleteChr = false;
        
        public bool m_fNewChrNameFilter = false;
        
        public bool m_fDenyNullChar = false;
        
        public bool m_fDenyAnsiChar = false;
        
        public bool m_fDenySpecChar = false;
        
        public bool m_fDenyHellenicChars = false;
        
        public bool m_fDenyRussiaChar = false;
        
        public bool m_fDenySpecNO1 = false;
        
        public bool m_fDenySpecNO2 = false;
        
        public bool m_fDenySpecNO3 = false;
        
        public bool m_fDenySpecNO4 = false;
        
        public bool m_fDenySBCChar = false;
        
        public bool m_fDenykanjiChar = false;
        
        public bool m_fDenyTabsChar = false;
        
        public int m_nCheckNewIDOfIP = 0;
        public int m_nMaxConnectOfIP = 0;
        public int m_nClientTimeOutTime = 0;
        public int m_nNomClientPacketSize = 0;
        public int m_nMaxClientPacketCount = 0;

        public GateConfig()
        {
            m_nShowLogLevel = 3;
            m_nGateCount = 1;
            m_fCheckNewIDOfIP = true;
            m_fCheckNullSession = true;
            m_fOverSpeedSendBack = false;
            m_fDefenceCCPacket = false;
            m_fKickOverSpeed = false;
            m_fKickOverPacketSize = true;
            
            m_fAllowGetBackChr = true;
            m_fAllowDeleteChr = true;
            m_fNewChrNameFilter = true;
            m_fDenyNullChar = true;
            m_fDenyAnsiChar = false;
            m_fDenySpecChar = false;
            m_fDenyHellenicChars = true;
            m_fDenyRussiaChar = true;
            m_fDenySpecNO1 = true;
            m_fDenySpecNO2 = true;
            m_fDenySpecNO3 = false;
            m_fDenySpecNO4 = true;
            m_fDenySBCChar = true;
            m_fDenykanjiChar = true;
            m_fDenyTabsChar = false;
            m_nNomClientPacketSize = 400;
            m_nMaxConnectOfIP = 20;
            m_nCheckNewIDOfIP = 5;
            m_nClientTimeOutTime = 60 * 1000;
            m_nMaxClientPacketCount = 2;
            ShowDebugLog = false;
        }
    }
}