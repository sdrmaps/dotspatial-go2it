using System;
using DotSpatial.Controls.Header;

namespace DotSpatial.SDR.Controls
{
    public class PermissionedActionItem : SimpleActionItem
    {
        #region Constructore and Destructors

        public PermissionedActionItem(string caption, bool enabled, EventHandler clickEventHandler) : base(caption, clickEventHandler)
        {
            Enabled = enabled;
        }

        public PermissionedActionItem(string rootKey, string caption, bool enabled, EventHandler clickEventHandler)
            : base(rootKey, caption, clickEventHandler)
        {
            Enabled = enabled;
        }

        public PermissionedActionItem(string rootKey, string menuContainerKey, string caption, bool enabled, EventHandler clickEventHandler)
            : base(rootKey, menuContainerKey, caption, clickEventHandler)
        {
            Enabled = enabled;
        }

        #endregion

        public virtual void Login()
        {
            Enabled = true;
        }

        public virtual void Logout()
        {
            Enabled = false;
        }
    }
}
