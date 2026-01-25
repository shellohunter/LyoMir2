using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using SystemModule;
using SystemModule.Common;

namespace GameGate
{
    public class GateShare
    {
        public static bool ShowLog = true;
        
        
        
        public const int MaxSession = 5000;
        
        
        
        public const long dwCheckServerTimeOutTime = 3 * 60 * 1000;
        
        
        
        public const long dwSessionTimeOutTime = 15 * 24 * 60 * 60 * 1000;
        
        
        
        public static StringList BlockIPList = null;
        
        
        
        public static IList<string> TempBlockIPList = null;
        
        
        
        public static ConcurrentDictionary<string, byte> ChatCommandFilter;
        public static Dictionary<string, ClientSession> PunishList;
        public static HardwareFilter HWFilter;

        public static void Initialization()
        {
            BlockIPList = new StringList();
            TempBlockIPList = new List<string>();
            PunishList = new Dictionary<string, ClientSession>();
            ChatCommandFilter = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public class HardwareHeader : Packets
    {
        public uint dwMagicCode;
        public byte[] xMd5Digest;

        public HardwareHeader(byte[] buffer) : base(buffer)
        {

        }

        protected override void ReadPacket(BinaryReader reader)
        {
            dwMagicCode = reader.ReadUInt32();
            xMd5Digest = reader.ReadBytes(16);
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}