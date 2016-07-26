using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DotSpatial.SDR.Data.Database;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Controls
{
    public delegate void HotKeyEventHandler(string action);

    public class HotKey
    {
        // properties
        private Keys _keys = Keys.None;
        private string _description = string.Empty;

        public Keys Key
        {
            get { return _keys; }
            set { _keys = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        // constructors
        public HotKey() {}

        public HotKey(Keys key)
        {
            _keys = key;
        }
        
        public HotKey(Keys key, string description)
        {
            _keys = key;
            _description = description;
        }

        // custom hash code to ensure all items have unique hash code value
        public override int GetHashCode()
        {
            return (Int32)_keys;
        }

        // custom comparison to ignore the description and only compare hashcodes
        public override bool Equals(object obj)
        {
            if (_keys != Keys.None)
            {
                return obj.GetHashCode() == (Int32)_keys;
            }
            return false;
        }
    }

    public static class HotKeyManager
    {
        public static event HotKeyEventHandler HotKeyEvent;
        // dict for storing all hotkeys and their lookup actions
        private static readonly Dictionary<HotKey, string> HotKeys = new Dictionary<HotKey, string>();
        // msg and keys data received from MainForm ProcessCmdKey method
        public static bool FireHotKeyEvent(ref Message msg, Keys keyData)
        {
            try
            {
                // convert this into a hotkey for our needs (key value)
                var hKey = new HotKey(keyData);
                // check if this keycode exists in the dictionary
                if (HotKeys.ContainsKey(hKey))
                {
                    string cmd; // get the command to fire
                    HotKeys.TryGetValue(hKey, out cmd);
                    if (string.IsNullOrEmpty(cmd)) return false;

                    HotKeyEvent(cmd);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void AddHotKey(HotKey hKey, string keyAction)
        {
            // first check if this keyAction is already set
            bool exists = false;
            foreach(KeyValuePair<HotKey, string> h in HotKeys)
            {
                if (h.Value == keyAction)
                {
                    exists = true;
                    break;
                }
            }
            // we need to add this keyAction, next check that the keyValue isnt already taken
            if (!exists)
            {
                if (!HotKeys.ContainsKey(hKey))
                {
                    HotKeys.Add(hKey, keyAction);
                }
                else
                {
                    var nullKey = new HotKey();
                    HotKeys.Add(nullKey, keyAction);
                }
            }
        }

        public static void RemoveHotKey(HotKey hKey)
        {
            if (HotKeys.ContainsKey(hKey))
            {
                HotKeys.Remove(hKey);
            }
        }

        public static void ClearHotKeys()
        {
            HotKeys.Clear();    
        }

        public static Dictionary<HotKey, string> HotKeysDictionary()
        {
            return HotKeys;
        }

        public static bool SaveHotKeys()
        {
            try
            {
                string conn = SdrConfig.Settings.Instance.ApplicationRepoConnectionString;
                SQLiteHelper.ClearTable(conn, "hotkeys");
                foreach (KeyValuePair<HotKey, string> kvPair in HotKeys)
                {
                    HotKey hKey = kvPair.Key;
                    var d = new Dictionary<string, string>
                {
                    {"keys", hKey.Key.ToString()},
                    {"description", hKey.Description},
                    {"command", kvPair.Value}
                };
                    SQLiteHelper.Insert(conn, "hotkeys", d);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool LoadHotKeys()
        {
            // basically this checks if there has been any hotkeys set by a previous user
            // any missing hotkeys will load default settings from their own assemblies
            try
            {
                string conn = SdrConfig.Settings.Instance.ApplicationRepoConnectionString;
                const string query = "SELECT command, keys, description FROM hotkeys";
                DataTable table = SQLiteHelper.GetDataTable(conn, query);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow r = table.Rows[i];
                    var txtkey = r["keys"].ToString();
                    var txtcmd = r["command"].ToString();
                    var txtdesc = r["description"].ToString();
                    // parse the text key value into a key enum
                    var keys = (Keys)Enum.Parse(typeof(Keys), txtkey);
                    // add the hotkey to the dict for lookup
                    AddHotKey(new HotKey(keys, txtdesc), txtcmd);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
