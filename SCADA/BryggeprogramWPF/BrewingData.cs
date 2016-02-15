namespace BryggeprogramWPF
{
    public class BrewingData
    {
        
        public double MashInTemperature { get; set; }
        public double MashInVolume { get; set; }
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

        //public void SaveData(BrewingData bd)
        //{
        //    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
        //    dlg.FileName = "BrewSession "+DateTime.Now.ToString("d M yyyy"); // Default file name
        //    dlg.DefaultExt = ".brewdata"; // Default file extension
        //    dlg.Filter = "Text documents (.brewdata)|*.brewdata"; // Filter files by extension

        //    // Show save file dialog box
        //    Nullable<bool> result = dlg.ShowDialog();

        //    // Process save file dialog box results
        //    if (result == true)
        //    {
        //        // Save document
        //        string filename = dlg.FileName;
        //        String json = JsonConvert.SerializeObject(bd);
        //        System.IO.File.WriteAllText(filename, json);
        //    }


        //}

        
    }
}
