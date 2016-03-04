using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BryggeprogramWPF.DataProvider
{
    public class SerialDataProvider
    {
        SerialPort mySerialPort = new SerialPort();

        public void ConnectToSerial(string port, int baudRate)
        {
            try
            {
                mySerialPort.PortName = port;
                mySerialPort.BaudRate = baudRate;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.WriteTimeout = 500;
                mySerialPort.Open();
                mySerialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            }
            catch
            {
                MessageBox.Show("No connection to arduino", "Connection info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
