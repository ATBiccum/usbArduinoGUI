using System;
using System.Collections.Generic;
using System.Text;

namespace usbArduinoGUI
{
    public class parseSubString
    {
        private int subStringLocation { get; set; }

        public parseSubString()
        {
            subStringLocation = 0; //Set the inital location within the string[]
        }

        public string parseString(string stringToParse, int numberOfChars)
        {   //Fill returnString with a subString of stringToParse, using a byte usually(numberofChars)
            string returnString = stringToParse.Substring(subStringLocation, numberOfChars);   
            subStringLocation += numberOfChars;
            return returnString;
        }
    }
}
