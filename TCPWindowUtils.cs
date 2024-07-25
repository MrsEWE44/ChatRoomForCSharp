using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace WindowsFormsApp2
{
    class TCPWindowUtils
    {

        public FlowLayoutPanel addHLayout(Form f, List<Control> cs)
        {
            FlowLayoutPanel hFLP = new FlowLayoutPanel();
            hFLP.Width = f.ClientSize.Width;
            hFLP.AutoSize = true;
            hFLP.FlowDirection = FlowDirection.LeftToRight;
            hFLP.Dock = DockStyle.Fill;
            foreach (Control c in cs)
            {
                hFLP.Controls.Add(c);
            }
            return hFLP;

        }
    }
}
