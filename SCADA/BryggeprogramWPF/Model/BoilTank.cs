
namespace BryggeprogramWPF.Model
{
    public class BoilTank:Tank
    {
        public Pump Pump { get; set; }
        public bool Element1_On { get; set; }
        public bool Element2_On { get; set; }
        public double TemperatureAfterCooler { get; set; }
        public BoilTank()
        {
            Pump = new Pump();

        }
    }
}
