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

                if (text_packetReceived.Dispatcher.CheckAccess())     //If we have access to the thread then update the ui
                {
                    UpdateUI(text);
                }
                else
                {                                               //If we do not have access to the thread
                    text_packetReceived.Dispatcher.Invoke(() => { UpdateUI(text); });
                }
            }
            catch (TimeoutException) { }                        //If no data is received then timeout error
        }

        private void UpdateUI(string text)
        {
            text_packetReceived.Text = text + text_packetReceived.Text; //Show the received text

            int checkSumCalc = 0;
            text_packetLength.Text = text.Length.ToString();
            if (text.Substring(0,3)=="###" && text.Length > 41) //Are we receiving a real packet?
            {
                text_packetNumber.Text = text.Substring(3, 3);
                text_A0.Text = text.Substring(6, 4);
                text_A1.Text = text.Substring(10, 4);
                text_A2.Text = text.Substring(14, 4);
                text_A3.Text = text.Substring(18, 4);
                text_A4.Text = text.Substring(22, 4);
                text_A5.Text = text.Substring(26, 4);
                text_Binary.Text = text.Substring(30,4);
                text_checkSumReceived.Text = text.Substring(38,3);

                for (int i=3; i<38; i++)
                {
                    checkSumCalc += (byte)text[i];
                }

                text_checkSumCalculated.Text = checkSumCalc.ToString();
            }
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
            text_packetReceived.Text = "";
        }
    }
}
