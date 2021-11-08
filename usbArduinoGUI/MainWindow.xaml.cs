using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace usbArduinoGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool bPortOpen = false;
        private string text;

        SerialPort serialport = new SerialPort();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            serialport.BaudRate = 115200;               //Setup baud rate for spi
            serialport.ReceivedBytesThreshold = 1;      //How many bytes we receive before we trigger event
            serialport.DataReceived += SerialPort_DataReceived;
            
            foreach (string port in ports)              //Each port in ports array gets added to combobox
            {
                comboBox1.Items.Add(port);
            }
            comboBox1.SelectedIndex = 2;                //Select the correct port from the list
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //Note text from serial port is on a different thread and we need to test this
                text = serialport.ReadLine();                   //Read the line because carriage line return and new line, it does these for us

                if (text_Received.Dispatcher.CheckAccess())     //If we have access to the thread then update the ui
                {
                    UpdateUI(text);
                }
                else
                {                                               //If we do not have access to the thread
                    text_Received.Dispatcher.Invoke(() => { UpdateUI(text); });
                }
            }
            catch (TimeoutException) { }                        //If no data is received then timeout error
        }

        private void UpdateUI(string text)
        {
            text_Received.Text = text + text_Received.Text;
        }

        private void butt_OpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (!bPortOpen)
            {
                serialport.PortName = comboBox1.Text;       //Receive the name of the port from the combobox
                serialport.Open();                          //Open the port using serialport
                butt_OpenClose.Content = "Close";           //Rename the content in the button to close because it is open now
                bPortOpen = true;                           //Restate our bool to true now
            }
            else
            {
                serialport.Close();                         //Close the port using seriaport
                butt_OpenClose.Content = "Open";            //Change button content to open now that its closed
                bPortOpen = false;                          //Restate our bool 
            }
        }

        private void butt_Clear_Click(object sender, RoutedEventArgs e)
        {
            text_Received.Text = "";
        }
    }
}
