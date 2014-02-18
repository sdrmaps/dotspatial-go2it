using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Go2It
{
    public class LayerSelectionSwitcher
    {
        readonly List<ComboBox> _boxlist = new List<ComboBox>();
        readonly List<CheckedListBox> _checkedlist = new List<CheckedListBox>();

        readonly Dictionary<ComboBox, object> _olditems = new Dictionary<ComboBox, object>();

        public void Add(params ComboBox[] boxes)
        {
            _boxlist.AddRange(boxes);
            boxes.ToList().ForEach(box => box.SelectedIndexChanged += ComboHandler);
        }

        public void Add(params CheckedListBox[] checkedboxes)
        {
            _checkedlist.AddRange(checkedboxes);
            checkedboxes.ToList().ForEach(checkedbox => checkedbox.ItemCheck+= CheckHandler);
        }

        public void Remove(ComboBox box)
        {
            _boxlist.Remove(box);
            box.Items.Clear();
            box.SelectedIndexChanged -= ComboHandler;
        }

        public void Remove(CheckedListBox checkedBox)
        {
            _checkedlist.Remove(checkedBox);
            checkedBox.Items.Clear();
            checkedBox.ItemCheck -= CheckHandler;
        }

        private void ComboHandler(object sender, EventArgs e)
        {
            var trigger = (ComboBox)sender;
            var item = trigger.SelectedItem;
            object olditem = null;

            if (_olditems.ContainsKey(trigger)) olditem = _olditems[trigger];

            _boxlist.ForEach(box =>
            {
                if (box == trigger) return;
                if (olditem != null) box.Items.Add(olditem);
                box.Items.Remove(item);
            });
            _checkedlist.ForEach(checkedbox =>
            {
                if (olditem != null && olditem.ToString().Length > 0) checkedbox.Items.Add(olditem);
                checkedbox.Items.Remove(item);               
            });

            _olditems[trigger] = item;
        }

        private void CheckHandler(object sender, ItemCheckEventArgs e)
        {
            var trigger = (CheckedListBox)sender;
            var item = trigger.Items[e.Index];

            _checkedlist.ForEach(checkbox =>
            {
                if (checkbox == trigger) return;
                // seems backwards but check event doesnt fire until later
                if (e.CurrentValue == CheckState.Unchecked)
                {
                    checkbox.Items.Remove(item);
                }
                else
                {
                    checkbox.Items.Add(item);
                }
            });
            _boxlist.ForEach(box =>
            {
                // seems backwards but check event doesnt fire until later
                if (e.CurrentValue == CheckState.Unchecked)
                {
                    box.Items.Remove(item);
                }
                else
                {
                    box.Items.Add(item);
                }
            });
        }
    }
}
