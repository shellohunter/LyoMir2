using System.Collections;
using System.Collections.Generic;
using System.IO;
using SystemModule;
using SystemModule.Common;

namespace GameGate
{
    public class Filter
    {
        public readonly HardwareFilter g_HWIDFilter = null;
        public static object g_ConnectOfIPLock = null;
        public static ArrayList g_ConnectOfIPList = null;
        private static readonly ArrayList g_BlockIPList = null;
        private static readonly ArrayList g_TempBlockIPList = null;
        public static ArrayList g_BlockIPAreaList = null;

        public Filter(HardwareFilter hwidFilter)
        {
            g_HWIDFilter = hwidFilter;
        }

        public static void LoadBlockIPList()
        {
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        public static void SaveBlockIPList()
        {
            
            
            
            
            
            
            
            
            
            
            
        }

        public static void AddToBlockIPList(string szIP)
        {
            var nIP = 0l;
            if (g_BlockIPList.IndexOf(szIP) < 0)
            {
                nIP = HUtil32.IpToInt(szIP);
                if (nIP != 0)
                {
                    
                }
            }
        }

        public static void AddToBlockIPList(int nIP)
        {
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        public static void AddToTempBlockIPList(string szIP)
        {
            var nIP = 0l;
            if (g_TempBlockIPList.IndexOf(szIP) < 0)
            {
                nIP = HUtil32.IpToInt(szIP);
                if (nIP != 0)
                {
                    
                }
            }
        }

        public static void AddToTempBlockIPList(int nIP)
        {
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        public static bool IsBlockIP(int nRemoteIP)
        {
            bool result = false;
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            return result;
        }

        public static bool OverConnectOfIP(long Addr)
        {
            bool result = false;
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            return result;
        }

        public static void DeleteConnectOfIP(long Addr)
        {
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        public static void ClearConnectOfIP()
        {
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        public static void LoadBlockIPAreaList()
        {
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        public static void SaveBlockIPAreaList()
        {
            
            
            
            
            
            
            
            
            
            
        }

        public static bool IsBlockIPArea(int nRemoteIP)
        {
            bool result = false;
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            return result;
        }

        public void initialization()
        {
            
            
            
            
            
        }
    }

    public class HardwareCnt
    {
        public byte[] HWID;
        public int Count;
    }

    public class HardwareFilter
    {
        private readonly IList<HardwareCnt> m_xCurList = null;
        private readonly IList<HardwareCnt> m_xDenyList = null;
        private ConfigManager _configManager => ConfigManager.Instance;

        public HardwareFilter()
        {
            m_xCurList = new List<HardwareCnt>();
            m_xDenyList = new List<HardwareCnt>();
        }

        public int AddDeny(byte[] HWID)
        {
            HardwareCnt pHWIDCnt;
            int result = -1;
            for (var i = 0; i < m_xDenyList.Count; i++)
            {
                pHWIDCnt = m_xDenyList[i];
                if (MD5.MD5Match(pHWIDCnt.HWID, HWID))
                {
                    result = i;
                    return result;
                }
            }
            pHWIDCnt = new HardwareCnt();
            pHWIDCnt.HWID = HWID;
            pHWIDCnt.Count = 0;
            m_xDenyList.Add(pHWIDCnt);
            return 1;
        }

        public int DelDeny(byte[] HWID)
        {
            HardwareCnt pHWIDCnt;
            int result = -1;
            for (var i = 0; i < m_xDenyList.Count; i++)
            {
                pHWIDCnt = m_xDenyList[i];
                if (MD5.MD5Match(pHWIDCnt.HWID, HWID))
                {
                    pHWIDCnt = null;
                    m_xDenyList.RemoveAt(i);
                    result = i;
                    break;
                }
            }
            return result;
        }

        public void ClearDeny()
        {
            m_xDenyList.Clear();
        }

        public void LoadDenyList()
        {
            var ls = new StringList();
            if (!File.Exists(_configManager.GateConfig.m_szBlockHWIDFileName))
            {
                ls.SaveToFile(_configManager.GateConfig.m_szBlockHWIDFileName);
            }
            ls.LoadFromFile(_configManager.GateConfig.m_szBlockHWIDFileName);
            for (var i = 0; i < ls.Count; i++)
            {
                if ((ls[i] == "") || (ls[i][0] == ';') || (ls[i].Length != 32))
                {
                    continue;
                }
                AddDeny(MD5.MD5UnPrInt(ls[i]));
            }
        }

        public void SaveDenyList()
        {
            
            
            
            
            
            
            
            
            
            
        }

        public bool IsFilter(byte[] HWID)
        {
            HardwareCnt pHWIDCnt;
            bool result = false;
            for (var i = 0; i < m_xDenyList.Count; i++)
            {
                pHWIDCnt = m_xDenyList[i];
                if (MD5.MD5Match(pHWIDCnt.HWID, HWID))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool IsFilter(byte[] HWID, ref bool fOverClientCount)
        {
            HardwareCnt pHWIDCnt;
            bool result = false;
            var fMatch = false;
            for (var i = 0; i < m_xCurList.Count; i++)
            {
                pHWIDCnt = m_xCurList[i];
                if (MD5.MD5Match(pHWIDCnt.HWID, HWID))
                {
                    if (pHWIDCnt.Count + 1 > _configManager.GateConfig.MaxClientCount)
                    {
                        result = true;
                        fOverClientCount = true;
                    }
                    else
                    {
                        pHWIDCnt.Count++;
                    }
                    fMatch = true;
                    break;
                }
            }
            if (!fMatch)
            {
                pHWIDCnt = new HardwareCnt();
                pHWIDCnt.HWID = HWID;
                pHWIDCnt.Count = 1;
                m_xCurList.Add(pHWIDCnt);
            }
            if (!result)
            {
                for (var i = 0; i < m_xDenyList.Count; i++)
                {
                    pHWIDCnt = m_xDenyList[i];
                    if (MD5.MD5Match(pHWIDCnt.HWID, HWID))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public int GetItemCount(byte[] HWID)
        {
            HardwareCnt pHWIDCnt;
            int result = 0;
            for (var i = 0; i < m_xCurList.Count; i++)
            {
                pHWIDCnt = m_xCurList[i];
                if (MD5.MD5Match(pHWIDCnt.HWID, HWID))
                {
                    result = pHWIDCnt.Count;
                    break;
                }
            }
            return result;
        }

        public void DecHWIDCount(byte[] HWID)
        {
            HardwareCnt pHWIDCnt;
            for (var i = 0; i < m_xCurList.Count; i++)
            {
                pHWIDCnt = m_xCurList[i];
                if (MD5.MD5Match(pHWIDCnt.HWID, HWID))
                {
                    if (pHWIDCnt.Count > 0)
                    {
                        pHWIDCnt.Count -= 1;
                    }
                    if (pHWIDCnt.Count == 0)
                    {
                        pHWIDCnt = null;
                        m_xCurList.RemoveAt(i);
                    }
                    break;
                }
            }
        }

        public void ClearHWIDCount()
        {
            m_xCurList.Clear();
        }
    }
}
