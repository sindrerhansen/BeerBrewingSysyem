using GalaSoft.MvvmLight;
using BryggeprogramWPF.Messages;
using BryggeprogramWPF.Model;
using System;

namespace BryggeprogramWPF.ViewModel
{
    public class ProsessViewModel : ViewModelBase
    {
        
        private ProsessData _prosessData = new ProsessData();
        public ProsessViewModel()
        {
            MessengerInstance.Register<ProsessDataMessage>(this, ProsessDataMessageRecived);
        }

        private void ProsessDataMessageRecived(ProsessDataMessage obj)
        {
            ProsessData = obj.ProsessData;

            if (obj.ProsessData.BrewingState > 0 )
            {
                State = obj.ProsessData.BrewingState;
                Timer = obj.ProsessData.Timer;
            }
            else if (obj.ProsessData.CleaningState > 0)
            {
                State = obj.ProsessData.CleaningState;
                Timer = obj.ProsessData.Timer;
                
            }
            else
            {
                State = 0;
                Timer = DateTime.Now.TimeOfDay;
            }

            
        }

        private TimeSpan timer;
        public TimeSpan Timer
        {
            get { return timer; }
            private set { 
                timer = value;
                RaisePropertyChanged("Timer");
            }
        }

        public ProsessData ProsessData
        {
            get
            {
                return _prosessData;
                
            }
            set
            {
                _prosessData = value;

                RaisePropertyChanged("ProsessData");
            }
        }
       
        private int state;
        public int State {
            get { return state; }
            private set 
            { 
                state = value;
                RaisePropertyChanged("State");
                
            }
        }

      

    }
}
