using SystemModule;

namespace GameSvr
{
    
    
    
    public class SafeEvent : Event
    {
        public SafeEvent(Envirnoment Envir, int nX, int nY, int nType) : base(Envir, nX, nY, nType, HUtil32.GetTickCount(), true)
        {

        }

        public override void Run()
        {
            m_dwOpenStartTick = HUtil32.GetTickCount();
            base.Run();
        }
    }
}

