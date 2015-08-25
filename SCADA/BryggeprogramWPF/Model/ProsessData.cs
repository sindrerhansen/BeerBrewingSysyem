using System;

namespace BryggeprogramWPF.Model
{
    public class ProsessData
    {
        public HLTank HLT { get; set; }
        public MashTank MashTank { get; set; }
        public BoilTank BoilTank { get; set; }
        public TimeSpan Timer { get; set; }
        public int CleaningState { get; set; }
        public int BrewingState { get; set; }
        

        public ProsessData()
        {
            HLT = new HLTank();
            MashTank = new MashTank();
            BoilTank = new BoilTank();
            Timer = new TimeSpan();
            BrewingState = 0;
            CleaningState = 0;
            

        }
    }
}
