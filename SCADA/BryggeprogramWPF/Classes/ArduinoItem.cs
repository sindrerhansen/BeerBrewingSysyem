using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Classes
{
    public class ArduinoItem
    {
        public Guid Id { get; private set; }
        public string Area { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public Type Type { get; set; }
    }
}
