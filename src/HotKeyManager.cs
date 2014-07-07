using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Go2It
{
    public delegate void HotKeyEventHandler(Message msg, Keys keyData);

    public class HotKeyManager
    {
        // private Message _message;
        // private Keys _keys;

        public event HotKeyEventHandler HotKeyEvent;

        //public void ProcessHotKey(Message msg, Keys keyData)
        //{

            
        //}

        public virtual void HotKeyPressed(Message msg, Keys KeyData)
        {
            
        }
    }
}
