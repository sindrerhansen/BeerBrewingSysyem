using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using BryggeprogramWPF.Messages;
using BryggeprogramWPF.Model;

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

      

    }
}
