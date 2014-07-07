using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DotSpatial.SDR.Controls
{
    public delegate void HotKeyEventHandler(string action);

    public class HotKey
    {
        // properties
        private Keys _key = Keys.None;
        public Keys Key
        {
            get { return _key; }
            set { _key = value; }
        }

        // constructors
        public HotKey() {}
        public HotKey(Keys key)
        {
            _key = key;
        }

        // custom hash code to ensure all items have unique hash code value
        public override int GetHashCode()
        {
            return (Int32)_key;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == (Int32)_key;
        }
    }

    public static class HotKeyManager
    {
        // dict for storing all hotkeys and theirt lookup actions
        private static readonly Dictionary<HotKey, string> HotKeys = new Dictionary<HotKey, string>();
        public static event HotKeyEventHandler HotKeyEvent;

        // msg and keys data from the main_form ProcessCmdKey firing convert it into hotkey and perform lookup
        public static bool FireHotKeyEvent(ref Message msg, Keys keyData)
        {
            try
            {
                // convert this into a hotkey for our needs
                var hKey = new HotKey(keyData);
                if (!HotKeys.ContainsKey(hKey)) return false;

                string action;
                HotKeys.TryGetValue(hKey, out action);
                if (string.IsNullOrEmpty(action)) return false;

                // notify our extension tools that a hotkey event has been fired
                HotKeyEvent(action);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void AddHotKey(HotKey hKey, string keyAction)
        {
            if (!HotKeys.ContainsKey(hKey))
            {
                HotKeys.Add(hKey, keyAction);
            }
        }

        public static void RemoveHotKey(HotKey hKey)
        {
            if (HotKeys.ContainsKey(hKey))
            {
                HotKeys.Remove(hKey);
            }
        }

        public static bool ContainsHotKey(HotKey hKey)
        {
            return HotKeys.ContainsKey(hKey);
        }
    }
}
