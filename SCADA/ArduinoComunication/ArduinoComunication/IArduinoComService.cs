using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ArduinoComunication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IArduinoComService
    {
        [OperationContract]
        string GetMessage(string name);
        [OperationContract]
        bool ArduinoWrite(string message);


    }
}
