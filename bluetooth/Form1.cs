using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
                    Console.WriteLine($"{i}) {devices[i].DeviceName}");
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

            Console.WriteLine($"Device Status: {(selectedDevice.Authenticated ? "Authenticated" : "Unauthenticated")}");
            //selected device is selected
            if (!selectedDevice.Authenticated)
            {
                Console.WriteLine("Requested access, accept the request on your device.");
                if (BluetoothSecurity.PairRequest(selectedDevice.DeviceAddress, "1234"))
                {
                    Console.WriteLine("Pair requested succeeded");
                }
                else
                {
                    Console.WriteLine("Pair requested failed");
                }
                
            }

            selectedDevice.Refresh();
            Console.WriteLine($"Device Status: {(selectedDevice.Authenticated ? "Authenticated" : "Unauthenticated")}");

            Console.WriteLine($"Attempting client connection to: {selectedDevice.DeviceName}");
            btClient.Connect(selectedDevice.DeviceAddress, BluetoothService.SerialPort);

            Console.WriteLine($"Client Status: {(btClient.Connected ? "Connected" : "Not Connected")}");


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
