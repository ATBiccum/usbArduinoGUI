using System;
using System.Collections.Generic;
using System.Text;

namespace usbArduinoGUI
{
    class SolarCalc
    {
        //Field 
        private static double ResistorValue;
        public double[] analogVoltage = new double[6];


        //Constructor that takes no argument
        public SolarCalc()
        {
            ResistorValue = 100.0;
        }

        //Methods
        public void ParseSolarData(string newPacket)
        {
            for(int i = 0; i < 6; i++)
            {
                analogVoltage[i] = Convert.ToDouble(newPacket.Substring(6 + (i * 4), 4));
                /*int thing = 6 + (i * 4);
                analogVoltage[i] = Convert.ToDouble(parseSubString.parseString(newPacket, 4));*/
            }
        }

        public string GetVoltage(double value)
        {
            double analogValue = value / 1000.0;
            return analogValue.ToString(" 0.0 V");
        }
        public string GetCurrent(double analogOne, double shuntResistor)
        {
            double shuntVoltage = (analogOne - shuntResistor) / ResistorValue;
            return shuntVoltage.ToString(" 0.0 mA; -0.0 mA; 0.0 mA"); //If posisitive format like first, negative like second, zero like 3rd
        }

        public string LEDCurrent(double analogOne, double shuntResistor)
        {
            double shuntVoltage = (analogOne - shuntResistor) / ResistorValue;
            if (shuntVoltage < 0)
            {
                shuntVoltage = 0;
            }
            return shuntVoltage.ToString(" 0.0 mA; -0.0 mA; 0.0 mA"); //If posisitive format like first, negative like second, zero like 3rd
        }
    }
}
