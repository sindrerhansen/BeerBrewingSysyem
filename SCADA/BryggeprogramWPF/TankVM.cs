using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF
{
    public class TankVM : INotifyPropertyChanged

    {

        
        string onOff;
        public TankVM()
        {
            
            OnOff = "LightGray";
        }

        
        public string OnOff {
            get { return onOff; }
            set {
                onOff = value;
                if(PropertyChanged!=null)
                PropertyChanged(this, new PropertyChangedEventArgs("OnOff"));
            }
        }

        public void SetOn()
        {
            OnOff = "Green";
        }

        public void SetOff()
        {
            OnOff = "LightGray";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
