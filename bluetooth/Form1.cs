using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bluetooth
{
    public partial class Form1 : Form
    {

        
        /*
         * pid mode 1 is live data
         */
        const byte PID_MODE = 0x01;
        /*
         * All respones from OBD reader are marked with 0x41 or A character
         */
        const byte RESPONSE_MARKER = 0x41;


        private byte EOF_MARKER = Convert.ToByte('>');
        /*
            Encoding: ASCII

         */
        public Form1()
        {
            // Tells the OBD reader to provide current data

            //InitializeComponent();


            BluetoothClient btClient = new BluetoothClient();
            List<string> btItems = new List<String>();
            BluetoothDeviceInfo selectedDevice = null;
    
            while (selectedDevice == null)
            {
                Console.WriteLine("Searching for BT Devices");
                BluetoothDeviceInfo[] devices = btClient.DiscoverDevicesInRange();
                
                foreach(BluetoothDeviceInfo d in devices)
                {
                    btItems.Add(d.DeviceName);
                }
                Console.WriteLine($"Found {devices.Length} devices");
                Console.WriteLine("---Devices---");
                for (int i = 0; i < devices.Length; i++)
                {
                    Console.WriteLine($"{i}) {devices[i].DeviceName} ({devices[i].DeviceAddress})");
                }
                Console.WriteLine("Hit enter to search again");

            
                int deviceId = -1;
                if( int.TryParse(Console.ReadLine(), out deviceId))
                {
                    if( deviceId >= 0 && deviceId <= devices.Length)
                    {
                        selectedDevice = devices[deviceId];
                    }
                    else
                    {
                        Console.WriteLine("Invalid number...");
                    }
                    
                }
            }

            
            Console.WriteLine($"Attempting client connection to: {selectedDevice.DeviceName}");
            btClient.Connect(selectedDevice.DeviceAddress, BluetoothService.SerialPort);
            Console.WriteLine($"Client Status: {(btClient.Connected ? "Connected" : "Not Connected")}");

            NetworkStream stream = btClient.GetStream();
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
            //EchoOff Command
            Console.WriteLine("Sending INIT Command");
            SendCommand("ATZ", writer);
            Thread.Sleep(1000);
            ReadCommand(stream);

            Console.WriteLine("Sending ECHO OFF Command");
            SendCommand("ATE0", writer);
            Thread.Sleep(1000);
            ReadCommand(stream);

            Console.WriteLine("Sending HEADERS ON Command");
            SendCommand("ATH1", writer);
            Thread.Sleep(1000);
            ReadCommand(stream);

            Console.WriteLine("Sending LINE FEEDS OFF Command");
            SendCommand("ATL0", writer);
            Thread.Sleep(1000);
            ReadCommand(stream);
            
            //writer.WriteLine(Encoding.ASCII.GetBytes("AT L0\r"));


            //stream.read

        }
        private void SendCommand(string command, StreamWriter stream)
        {
            //Formats into uppercase hexadecimal, adds termination line
            string request = (PID_MODE.ToString() + command + "\r");
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            Console.WriteLine($"Sending PID Request: {request}");
            stream.Write(buffer);
        }

        private string ReadCommand(NetworkStream stream)
        {
            byte[] readBuffer = new byte[1024];

            int bytesRead = stream.Read(readBuffer, 0, readBuffer.Length);

            List<char> finalMessage = new List<char>();


            for (int i = 0; i < bytesRead; i++){
                char c = (char)readBuffer[i];
                
                if( c == '\r')
                {
                    break;
                }

                if( c != (char)0x00)
                {
                    finalMessage.Add(c);
                }
            }

            char[] responseCharArray  = finalMessage.ToArray();
            var test = new string(responseCharArray);

            Console.WriteLine($"Unconverted: {test}");
            return "";
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
