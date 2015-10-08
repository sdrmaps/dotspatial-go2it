//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SDR.Configuration.Properties;

//namespace SDR.Configuration.Plugins
//{
//    public delegate void UpdateEventHandler(object sender, EventArgs e);

//    public class AliDatabase
//    {
//        public event UpdateEventHandler AliInterfaceChanged;

//        public enum Aliinterface
//        {
//            Deactivated,
//            Ali,
//            GlobalCad,
//            Interpol,
//        }

//        public Interface AliInterface
//        {
//            get
//            {
//                var aliMode = UserSettings.Default.AliMode;
//                if (aliMode.Length <= 0)
//                {
//                    AliInterface = AliSettings.Interface.Deactivated;
//                }
//                AliSettings.Interface am;
//                Enum.TryParse(aliMode, true, out am);
//                return am;
//            }
//            set
//            {
//                UserSettings.Default.AliMode = value.ToString();
//                OnInterfaceChanged();
//            }
//        }



//        protected virtual void OnInterfaceChanged()
//        {
//            if (AliInterfaceChanged != null)
//                AliInterfaceChanged(this, EventArgs.Empty);
//        }

//        public int AliPort
//        {
//            get
//            {
//                return UserSettings.Default.AliInterfacePort;
//            }
//            set
//            {
//                UserSettings.Default.AliInterfacePort = value;
//            }
//        }

//        public string GlobalLogLocation
//        {
//            get { return UserSettings.Default.GlobalCadLogLocation; }
//            set { UserSettings.Default.GlobalCadLogLocation = value.ToString(); }
//        }

//        public string InterpolLogin
//        {
//            get { return UserSettings.Default.InterpolDbLogin; }
//            set { UserSettings.Default.InterpolDbLogin = value.ToString(); }
//        }

//        // TODO: salt and hash this
//        public string InterpolPassword
//        {
//            get { return UserSettings.Default.InterpolDbLogin; }
//            set { UserSettings.Default.InterpolDbLogin = value.ToString(); }
//        }
//    }
//}
