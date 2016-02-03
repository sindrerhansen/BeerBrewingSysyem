
namespace BryggeprogramWPF.Model
{
    public class MashTank: Tank
    {
        public Pump CirculationPump { get; set; }
        public Pump TransferPump{get;set;}
        public RIMSelement RimsRight { get; set; }
        public RIMSelement RimsLeft { get; set; }

        public MashTank()
        {
            CirculationPump = new Pump();
            TransferPump = new Pump();
            RimsRight = new RIMSelement();
            RimsLeft = new RIMSelement();
            
        }
    }
}
