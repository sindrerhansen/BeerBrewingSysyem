using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BryggeprogramWPF
{
    [Serializable]
    public class BrewingData
    {
        //double mashInTemperature;
        //int mashInVolume;
        //double mashInHltTemperature;
        //double mashStep1Temperature;
        //int mashStep1Time;
        //double mashStep2Temperature;
        //int mashStep2Time;
        //double mashStep3Temperature;
        //int mashStep3Time;
        //double mashStep4Temperature;
        //int mashStep4Time;
        //double spargeTemperature;
        //int spargeVolume;
        //int boilTime;
        
        public double MashInTemperature { get; set; }
        public int MashInVolume { get; set; }
        public double MashInHltTemperature { get; set; }
        public double MashStep1Temperature { get; set; }
        public int MashStep1Time { get; set; }
        public double MashStep2Temperature { get; set; }
        public int MashStep2Time { get; set; }
        public double MashStep3Temperature { get; set; }
        public int MashStep3Time { get; set; }
        public double MashStep4Temperature { get; set; }
        public int MashStep4Time { get; set; }
        public double SpargeTemperature { get; set; }
        public double SpargeVolume { get; set; }
        public int BoilTime { get; set; }

        public void SaveData(BrewingData bd)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "BrewSession "+DateTime.Now.ToString("d M yyyy"); // Default file name
            dlg.DefaultExt = ".brewdata"; // Default file extension
            dlg.Filter = "Text documents (.brewdata)|*.brewdata"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                String json = JsonConvert.SerializeObject(bd);
                System.IO.File.WriteAllText(filename, json);
            }


        }

        public BrewingData ReadData()
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".brewdata"; // Default file extension
            dlg.Filter = "Text documents (.brewdata)|*.brewdata"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                BrewingData returnBrewingData = new BrewingData();
                String file = System.IO.File.ReadAllText(@filename);
                dynamic fileData = JsonConvert.DeserializeObject(file);

                returnBrewingData.MashInTemperature = fileData.MashInTemperature;
                returnBrewingData.MashInHltTemperature = fileData.MashInHltTemperature;
                returnBrewingData.MashInVolume = fileData.MashInVolume;
                returnBrewingData.MashStep1Temperature = fileData.MashStep1Temperature;
                returnBrewingData.MashStep1Time = fileData.MashStep1Time;
                returnBrewingData.MashStep2Temperature = fileData.MashStep2Temperature;
                returnBrewingData.MashStep2Time = fileData.MashStep2Time;
                returnBrewingData.MashStep3Temperature = fileData.MashStep3Temperature;
                returnBrewingData.MashStep3Time = fileData.MashStep3Time;
                returnBrewingData.MashStep4Temperature = fileData.MashStep4Temperature;
                returnBrewingData.MashStep4Time = fileData.MashStep4Time;
                returnBrewingData.SpargeTemperature = fileData.SpargeTemperature;
                returnBrewingData.SpargeVolume = fileData.SpargeVolume;
                returnBrewingData.BoilTime = fileData.BoilTime;
                
                return returnBrewingData;
            }
            else
            {
                return null;
            }
        }
    }
}
