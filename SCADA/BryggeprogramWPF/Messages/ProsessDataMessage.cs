using BryggeprogramWPF.Model;

namespace BryggeprogramWPF.Messages
{
    public class ProsessDataMessage
    {
        public ProsessData ProsessData { get; private set; }
        public ProsessDataMessage(ProsessData data)
        {
            ProsessData = data;
        }

        
    }
}
