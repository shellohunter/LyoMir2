using System.Collections.Concurrent;
using System.Collections.Generic;
using SystemModule.Common;

namespace LoginGate
{
    public class GateShare
    {
        
        
        
        public static long dwCheckServerTimeOutTime = 3 * 60 * 1000;
        
        
        
        public static StringList BlockIPList = null;
        
        
        
        public static IList<string> TempBlockIPList = null;
        public static int nMaxConnOfIPaddr = 50;
        public static ConcurrentDictionary<int, ClientThread> ServerGateList;

        public static void LoadBlockIPFile()
        {
            
            
            
            
            
            
            
        }

        public static void Initialization()
        {
            BlockIPList = new StringList();
            TempBlockIPList = new List<string>();
            ServerGateList = new ConcurrentDictionary<int, ClientThread>();
        }
    }
}