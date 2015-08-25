
namespace BryggeprogramWPF.Model
{

    public class Pump
    {
        public virtual bool Running
        {
            get;
            set;
        }

        public Pump()
        {
            Running = false;
        }

    }
}
