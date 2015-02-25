using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Yfer.ZedGraph.Extension.Test
{
    public partial class OscViewForm : Form
    {
        private const int PointCount = 1000000;

        public OscViewForm()
        {
            InitializeComponent();
            Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random();
        }
        bool a=false;
        FilteredPointPairList line;
        private void Random()
        {
            var pane = zedGraphControl1.GraphPane;
            pane.CurveList.Clear();
            double[]
                x = new double[PointCount], 
                y = new double[PointCount], 
                z = new double[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
            	x[i]=i*2;
                y[i] = i;
                if(a)
                    z[i]= Math.Sin((double)i / 5000) * ((double)i);
                else
                    z[i] = Math.Cos((double)i / 500) * ((double)i);
            }
            line = new FilteredPointPairList(x, y, z);
            var hlli = new HiLowLineItem("Million Points", line, Color.Red);

            line.SetBounds(1, PointCount, 1000);
            
            pane.CurveList.Add(hlli);
            zedGraphControl1.ZoomEvent += zedGraphControl1_ZoomEvent;
            a = !a;
            Refresh();
        }

        void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            Refresh();
        }
        public void Refresh()
        {
            zedGraphControl1.Refresh();

            GraphPane gp = zedGraphControl1.GraphPane;
            
            line.SetBounds(gp.XAxis.Scale.Min, gp.XAxis.Scale.Max, zedGraphControl1.Width);            
        }
        private void OscViewForm_Load(object sender, EventArgs e)
        {
            zedGraphControl1.GraphPane.XAxis.Scale.Max = 2000000;
            zedGraphControl1.GraphPane.XAxis.Scale.Min = 0;
            zedGraphControl1.GraphPane.YAxis.Scale.Max = 1000000;
            zedGraphControl1.GraphPane.YAxis.Scale.Min = -1000000;
            Random();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
                zedGraphControl1.GraphPane.YAxis.Type = AxisType.Log;
            else
                zedGraphControl1.GraphPane.YAxis.Type = AxisType.Linear;
            Refresh();
        }
    }
}
