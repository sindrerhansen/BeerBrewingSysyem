using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF
{
    public class Data
    {
        public static List<Measurement> GetData()
        {
            var measurements = new List<Measurement>();

            for (int i = 0; i < 5; i++)
            {

                measurements.Add(new Measurement() { DetectorId = i, DateTime = DateTime.Now, Value = 0 });

            }
            measurements.Sort((m1, m2) => m1.DateTime.CompareTo(m2.DateTime));
            return measurements;
        }

        public static List<Measurement> UpdateData(List<double> value)
        {

            var measurements = new List<Measurement>();

            for (int i = 0; i < 5; i++)
            {
                measurements.Add(new Measurement() { DetectorId = i, DateTime = DateTime.Now, Value = value[i] });
            }
            return measurements;
        }
    }

    public class Measurement
    {
        public int DetectorId { get; set; }
        public double Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
