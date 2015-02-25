using System;
using System.Drawing;
using ZedGraph;

namespace Yfer.ZedGraph.Extension
{
    /// <summary>
    /// Encapsulates a curve type that is displayed as a line and/or a set of
    /// symbols at each point.
    /// </summary>
    /// 
    /// <author> John Champion </author>
    /// <version> $Revision: 3.22 $ $Date: 2007/08/10 16:22:54 $ </version>
    [Serializable]
    public class HiLowLineItem : LineItem
    {
        #region Fields
        /// <summary>
        /// Private field that stores the <see cref="ZedGraph.Fill"/> data for this
        /// <see cref="HiLowLineItem"/>.  Use the public property <see cref="Fill"/> to
        /// access this value.
        /// </summary>
        private Fill _fill;

        private HiLowLine _HiLowLine;
        #endregion
  
        #region Properties
        public Fill Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }
        public HiLowLine HiLowLine
        {
            get { return _HiLowLine; }
            set { _HiLowLine = value; }
        }
        #endregion

        #region Constructors


		public HiLowLineItem( string label, IPointList points, Color color)
			: base(label, points, color,SymbolType.None,1)
		{
            _HiLowLine = new HiLowLine(color);
        }
        public HiLowLineItem(string label, IPointList points, Color color,SymbolType type)
            : base(label, points, color, type)
        {
            _HiLowLine = new HiLowLine(color);
        }

        #endregion

        #region Methods

        public override void Draw(Graphics g, GraphPane pane, int pos, float scaleFactor)
        {
            if (_isVisible)
            {
                HiLowLine.DrawFilled(g, pane, this, scaleFactor);
                Symbol.Draw(g, pane, this, scaleFactor, IsSelected);
            }
        }
        #endregion
    }
}
