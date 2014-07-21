using System;
using System.Collections;

namespace DotSpatial.SDR.Controls
{
    // delegate type for hooking up change notifications for arraylist
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    public class EventedArrayList : ArrayList
    {
        // event that notifies clients whenever the elements of the list change.
        public event ChangedEventHandler ListChanged;

        // invoke the changed event; called whenever list changes
        protected virtual void OnChanged(EventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }

        // override base methods that make changes to the list (fire event after each)
        public override int Add(object value)
        {
            int i = base.Add(value);
            OnChanged(EventArgs.Empty);
            return i;
        }

        public override void AddRange(ICollection c)
        {
            base.AddRange(c);
            OnChanged(EventArgs.Empty);
        }

        public override void Clear()
        {
            base.Clear();
            OnChanged(EventArgs.Empty);
        }

        public override void Insert(int index, object value)
        {
            base.Insert(index, value);
            OnChanged(EventArgs.Empty);
        }

        public override void Remove(object obj)
        {
            base.Remove(obj);
            OnChanged(EventArgs.Empty);
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnChanged(EventArgs.Empty);
        }

        public override void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            OnChanged(EventArgs.Empty);
        }

        public override void InsertRange(int index, ICollection c)
        {
            base.InsertRange(index, c);
            OnChanged(EventArgs.Empty);
        }

        public override void SetRange(int index, ICollection c)
        {
            base.SetRange(index, c);
            OnChanged(EventArgs.Empty);
        }

        public override object this[int index]
        {
            set
            {
                base[index] = value;
                OnChanged(EventArgs.Empty);
            }
        }
    }
}
