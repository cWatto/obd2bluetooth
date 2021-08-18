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
        public Form1()
        {
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
            Console.WriteLine("Sending EchoOff Command");
            writer.WriteLine(Encoding.ASCII.GetBytes("AT E0\r"));
            writer.Flush();
            Thread.Sleep(500);


            
            int b = 0;
            //reads until > is found or end of stream
            List<char> resp = new List<char>();
            while((b = stream.ReadByte()) > -1)
            {
                char c = Convert.ToChar(b);
                if( c == '>')
                {
                    break;
                }

                resp.Add(c);
            }

            Console.WriteLine($"Response!: {new string(resp.ToArray())}");


            //writer.WriteLine(Encoding.ASCII.GetBytes("AT L0\r"));


            //stream.read

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
