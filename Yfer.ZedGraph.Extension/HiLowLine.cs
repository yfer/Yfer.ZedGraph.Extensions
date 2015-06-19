using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Security.Permissions;
using ZedGraph;

namespace Yfer.ZedGraph.Extension
{
    [Serializable]
    public class HiLowLine : Line, ICloneable
    {
        #region Constructors

        public HiLowLine() : this(Color.Empty)
        {
        }

        public HiLowLine(Color color) : base(color)
        {
            Fill.Brush = new SolidBrush(color);
        }

        public HiLowLine(HiLowLine rhs) : base(rhs)
        {
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public HiLowLine Clone()
        {
            return new HiLowLine(this);
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Current schema value that defines the version of the serialized file
        /// </summary>
        public const int schema = 14;

        /// <summary>
        /// Constructor for deserializing objects
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
        /// </param>
        /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
        /// </param>
        protected HiLowLine(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
        /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion

        #region Rendering Methods

        public void DrawFilled(Graphics g, GraphPane pane, CurveItem curve, float scaleFactor)
        {
            var source = this;

            var yAxis = curve.GetYAxis(pane);
            var xAxis = curve.GetXAxis(pane);

            var basePix = yAxis.Scale.Transform(0.0);
            using (var pen = source.GetPen(pane, scaleFactor))
            {
                var list1 = new List<PointF>();
                var list2 = new List<PointF>();
                for (var i = 0; i < curve.Points.Count; i++)
                {
                    var pt = curve.Points[i];

                    if (pt.X != PointPair.Missing &&
                            pt.Y != PointPair.Missing &&
                            !Double.IsNaN(pt.X) &&
                            !Double.IsNaN(pt.Y) &&
                            !Double.IsInfinity(pt.X) &&
                            !Double.IsInfinity(pt.Y) &&
                            (!xAxis.Scale.IsLog || pt.X > 0.0) &&
                            (!yAxis.Scale.IsLog || pt.Y > 0.0))
                    {
                        var pixY = yAxis.Scale.Transform(curve.IsOverrideOrdinal, i, pt.Y);
                        var pixX = xAxis.Scale.Transform(curve.IsOverrideOrdinal, i, pt.X);
                        var pixZ = yAxis.Scale.Transform(curve.IsOverrideOrdinal, i, pt.Z);
                        list1.Add(new PointF(pixX,pixY));
                        list2.Add(new PointF(pixX,pixZ));
                    }
                }
                list2.Reverse();
                list1.AddRange(list2);
                var points = list1.ToArray();

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPolygon(Fill.Brush, points);
                
                g.DrawPolygon(pen, points);
            }
        }

        #endregion

    }
}
