using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Messages
{
    public class MulticastMessage
    {
        public string DataString { get; private set; }
        public MulticastMessage(string data)
        {
            DataString = data;
        }
    }
}
