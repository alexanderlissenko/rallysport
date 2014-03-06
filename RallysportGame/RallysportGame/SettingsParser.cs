using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using IniParser;
//using IniParser.Model;

namespace RallysportGame
{
    //Declaring enums directly in namespace makes global access easier
    enum Settings : byte
    {
        WINDOW_HEIGHT,
        WINDOW_WIDTH,
        GRAVITY
    }

    

    static class SettingsParser
    {

        static Dictionary<Settings, Int32> intSettings;
        static Dictionary<Settings, float> floatSettings;
        static Dictionary<Settings, bool> boolSettings;


        //Loads settings from the ini file specified by path.
        static public void Init(String path)
        {
            intSettings = new Dictionary<Settings, Int32>();
            floatSettings = new Dictionary<Settings, float>();
            boolSettings = new Dictionary<Settings, bool>();
            //IniData data = new FileIniDataParser().ReadFile(path);
/*
            foreach (SectionData sd in data.Sections)
            {
                foreach (KeyData k in sd.Keys)
                {
                    String kName = k.KeyName.ToUpper();
                    foreach (Settings eValue in Enum.GetValues(typeof(Settings)))
                    {
                        //Compare kName and eName
                        if (kName.Equals(Enum.GetName(typeof(Settings), eValue)))
                        {
                            if (k.Value.Equals("true") || k.Value.Equals("false"))
                                boolSettings.Add(eValue, bool.Parse(k.Value));
                            else if (k.Value.Contains("."))  //This is a float
                                floatSettings.Add(eValue, float.Parse(k.Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                            else
                                intSettings.Add(eValue, Int32.Parse(k.Value));
                        }
                    }
                }
            } 
*/
        }
        
        /*
         * Returns the value of the setting s or, if invalid, int.MinValue
         */

        static public int GetInt(Settings s){
            int result = Int32.MinValue;
            intSettings.TryGetValue(s, out result);
            return result;
        }

        static public float GetFloat(Settings s)
        {
            float result = float.MinValue;
            floatSettings.TryGetValue(s, out result);
            return result;
        }
        static public bool GetBool(Settings s)
        {
            bool result = false;
            boolSettings.TryGetValue(s, out result);
            return result;
        }

    }
}
