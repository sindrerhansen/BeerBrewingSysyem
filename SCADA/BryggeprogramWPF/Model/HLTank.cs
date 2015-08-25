
namespace BryggeprogramWPF.Model
{
    public class HLTank : Tank
    {
        public virtual Pump CirculationPump { get; set; }
        public virtual Pump TransferPump {get; set;}
        public bool HeatingElementOn { get; set; }

        public HLTank()
        {
            CirculationPump = new Pump();
            TransferPump = new Pump();
         
        }
    }

    
}
