
namespace BryggeprogramWPF.Model
{
    public class Tank
    {

        public double CurrentVolume
        {
            get;
            set;
        }
        public double AddedVolume
        { get; set; }
        public double TappedVolume
        { get; set; }
        public double Temperature
        { get; set; }
        public double TemperatureSetpoint
        { get; set; }

        public Tank()
        {
            AddedVolume = 0;
            TappedVolume = 0;
            CurrentVolume = 0;
            Temperature = 0;
            TemperatureSetpoint = 0;

        }

    }

    
}
