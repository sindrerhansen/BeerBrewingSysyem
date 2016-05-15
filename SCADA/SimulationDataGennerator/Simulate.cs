using System;

namespace SimulationDataGennerator
{
    public class Simulate
    {
        int conter;
        public string GennerateSimulatedArduinoValues()
        {
            string ret = "";
            ret += "STATE" + "10" + "_";

            ret += "HltTe" + (0 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            ret += "HltSp" + (0 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            ret += "MatTe" + (5 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            ret += "RimRO" + (10 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            ret += "RimLO" + (10 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            ret += "BotTe" + (15 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            ret += "MatVo" + (20 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            ret += "BoCoT" + (25 + 10 * Math.Sin(conter)).ToString().Replace(',', '.') + "_";
            conter++;
            return ret;
        }
    }
}
