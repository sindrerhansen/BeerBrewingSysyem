using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF
{
    public static class Functions
    {
        
        public static string GenerateSendValue(string inn, string hedder)
        {
            string systemDevider = "_";
            double number;
            var _string = inn.Replace(".", ",");
            if (double.TryParse(_string, out number))
            {
                var _return = Convert.ToString(number / 10) +hedder+ systemDevider;
                return _return;
            }
            else
            {
                MessageBox.Show("Input need to be a number!");
                return "";
            }
        }
    }
}
