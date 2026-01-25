using SelGate.Services;
using System.Collections.Generic;
using SystemModule.Common;

namespace SelGate
{
    public class GateShare
    {
        public static string GateAddr = "*";
        
        
        
        public static int GatePort = 7100;
        
        
        
        public static long dwCheckServerTimeOutTime = 3 * 60 * 1000;
        public static long dwCheckServerTick = 0;
        public static long dwCheckServerTimeMin = 0;
        public static long dwCheckServerTimeMax = 0;
        
        
        
        public static StringList BlockIPList = null;
        
        
        
        public static IList<string> TempBlockIPList = null;
        public static int nMaxConnOfIPaddr = 50;
        
        
        
        public static long dwSessionTimeOutTime = 15 * 24 * 60 * 60 * 1000;
        public static IList<ClientThread> ServerGateList;

        public static void LoadBlockIPFile()
        {
            
            
            
            
            
            
            
        }

        public static void Initialization()
        {
            BlockIPList = new StringList();
            TempBlockIPList = new List<string>();
            ServerGateList = new List<ClientThread>();
        }
    }
}