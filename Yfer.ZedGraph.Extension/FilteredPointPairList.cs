﻿using System;
using ZedGraph;

namespace Yfer.ZedGraph.Extension
{
    [Serializable]
    public class FilteredPointPairList : IPointList
    {

        #region Fields

        /// <summary>
        /// Instance of an array of x values
        /// </summary>
        private double[] _x;
        /// <summary>
        /// Instance of an array of x values
        /// </summary>
        private double[] _y;
        /// <summary>
        /// Instance of an array of z values
        /// </summary>
        private double[] _pointBase;
        /*
        /// <summary>
        /// This is the minimum value of the range of interest (typically the minimum of
        /// the range that you have zoomed into)
        /// </summary>
        //private double _xMinBound = double.MinValue;
        /// <summary>
        /// This is the maximum value of the range of interest (typically the maximum of
        /// the range that you have zoomed into)
        /// </summary>
        //private double _xMaxBound = double.MaxValue;
        */
        /// <summary>
        /// This is the maximum number of points that you want to see in the filtered dataset
        /// </summary>
        private int _maxPts = -1;

        /// <summary>
        /// The index of the xMinBound above
        /// </summary>
        private int _minBoundIndex = -1;
        /// <summary>
        /// The index of the xMaxBound above
        /// </summary>
        private int _maxBoundIndex = -1;

        //		/// <summary>
        //		/// Switch used to indicate if the next filtered point should be the high point or the
        //		/// low point within the current range.
        //		/// </summary>
        //		private bool _upDown = false;

        //		/// <summary>
        //		/// Determines if the high/low logic will be used.
        //		/// </summary>
        //		private bool _isApplyHighLowLogic = true;

        #endregion

        #region Properties

        /// <summary>
        /// Indexer to access the specified <see cref="PointPair"/> object by
        /// its ordinal position in the list.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="PointPairBase.Missing" /> for any value of <see paramref="index" />
        /// that is outside of its corresponding array bounds.
        /// </remarks>
        /// <param name="index">The ordinal position (zero-based) of the
        /// <see cref="PointPair"/> object to be accessed.</param>
        /// <value>A <see cref="PointPair"/> object reference.</value>
        public PointPair this[int index]
        {
            get
            {
 
                    double coef = 0;
                    // See if the array should be bounded
                    if (_minBoundIndex >= 0 && _maxBoundIndex >= 0 && _maxPts >= 0)
                    {
                        // get number of points in bounded range
                        var nPts = _maxBoundIndex - _minBoundIndex + 1;
                        coef = (double)nPts / (double)_maxPts;


                        if (nPts > _maxPts)
                        {
                            // if we're skipping points, then calculate the new index
                            var u = 0;
                            if (index == 0) u--;
                            if (index == _maxPts-1) 
                                u++;
                            index = _minBoundIndex + (int)((double)coef * (double)index)+u;

                        }
                        else
                        {
                            // otherwise, index is just offset by the start of the bounded range
                            index += _minBoundIndex;
                        }
                    }

                    double xVal, yVal, pointBase;
                    
                    if (index >= 0 && index < _x.Length)
                        xVal = _x[index];
                    else
                        xVal = PointPair.Missing;

                    if (index >= 0 && index < _y.Length)
                    {
                        var max = _y[index];
                        for (var i = 1; i < coef - 2; i++)
                        {
                            if (max < _y[index + i])
                                max = _y[index + i];
                        }
                        yVal = max;
                    }
                    else
                        yVal = PointPair.Missing;

                    if (index >= 0 && index < _pointBase.Length)
                    {
                        var min = _pointBase[index];
                        for (var i = 1; i < coef - 2; i++)
                        {
                            if (min > _pointBase[index + i])
                                min = _pointBase[index + i];
                        }
                        pointBase = min;
                    }
                    else
                        pointBase = PointPair.Missing;
                return new PointPair(xVal, yVal, pointBase);
            }

            set
            {
                // See if the array should be bounded
                if (_minBoundIndex >= 0 && _maxBoundIndex >= 0 && _maxPts >= 0)
                {
                    // get number of points in bounded range
                    var nPts = _maxBoundIndex - _minBoundIndex + 1;

                    if (nPts > _maxPts)
                    {
                        // if we're skipping points, then calculate the new index
                        index = _minBoundIndex + (int)((double)index * (double)nPts / (double)_maxPts);
                    }
                    else
                    {
                        // otherwise, index is just offset by the start of the bounded range
                        index += _minBoundIndex;
                    }
                }

                if (index >= 0 && index < _x.Length)
                    _x[index] = value.X;

                if (index >= 0 && index < _y.Length)
                    _y[index] = value.Y;

                if (index >= 0 && index < _pointBase.Length)
                    _pointBase[index] = value.Z;
            }
        }

        /// <summary>
        /// Returns the number of points according to the current state of the filter.
        /// </summary>
        public int Count
        {
            get
            {
                var arraySize = _x.Length;

                // Is the filter active?
                if (_minBoundIndex >= 0 && _maxBoundIndex >= 0 && _maxPts > 0)
                {
                    // get the number of points within the filter bounds
                    var boundSize = _maxBoundIndex - _minBoundIndex + 1;

                    // limit the point count to the filter bounds
                    if (boundSize < arraySize)
                        arraySize = boundSize;

                    // limit the point count to the declared max points
                    if (arraySize > _maxPts)
                        arraySize = _maxPts;
                }

                return arraySize;
            }
        }

        /// <summary>
        /// Gets the desired number of filtered points to output.  You can set this value by
        /// calling <see cref="SetBounds" />.
        /// </summary>
        public int MaxPts
        {
            get { return _maxPts; }
        }


        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to initialize the PointPairList from two arrays of
        /// type double.
        /// </summary>
        public FilteredPointPairList(double[] x, double[] y, double[] pointBase)
        {
            _x = x;
            _y = y;
            _pointBase = pointBase;
        }

        /// <summary>
        /// Constructor to initialize the PointPairList from two arrays of
        /// type double.
        /// </summary>
        public FilteredPointPairList(double[] x, double[] y)
        {
            _x = x;
            _y = y;
            _pointBase = _y;
        }
        /// <summary>
        /// The Copy Constructor
        /// </summary>
        /// <param name="rhs">The FilteredPointList from which to copy</param>
        public FilteredPointPairList(FilteredPointPairList rhs)
        {
            _x = (double[])rhs._x.Clone();
            _y = (double[])rhs._y.Clone();
            _pointBase = (double[])rhs._pointBase.Clone();
            _minBoundIndex = rhs._minBoundIndex;
            _maxBoundIndex = rhs._maxBoundIndex;
            _maxPts = rhs._maxPts;

        }

        /// <summary>
        /// Deep-copy clone routine
        /// </summary>
        /// <returns>A new, independent copy of the FilteredPointList</returns>
        virtual public object Clone()
        {
            return new FilteredPointPairList(this);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Set the data bounds to the specified minimum, maximum, and point count.  Use values of
        /// min=double.MinValue and max=double.MaxValue to get the full range of data.  Use maxPts=-1
        /// to not limit the number of points.  Call this method anytime the zoom range is changed.
        /// </summary>
        /// <param name="min">The lower bound for the X data of interest</param>
        /// <param name="max">The upper bound for the X data of interest</param>
        /// <param name="maxPts">The maximum number of points allowed to be
        /// output by the filter</param>
        // New code mods by ingineer
        public void SetBounds(double min, double max, int maxPts)
        {
            _maxPts = maxPts;

            // find the index of the start and end of the bounded range
            var first = Array.BinarySearch(_x, min);
            var last = Array.BinarySearch(_x, max);

            // Make sure the bounded indices are legitimate
            // if BinarySearch() doesn't find the value, it returns the bitwise
            // complement of the index of the 1st element larger than the sought value

            if (first < 0)
            {
                if (first == -1)
                    first = 0;
                else
                    first = ~(first + 1);
            }

            if (last < 0)
                last = ~last;

            _minBoundIndex = first;
            _maxBoundIndex = last;
        }

        // The old version, as of 21-Oct-2007
        //public void SetBounds2(double min, double max, int maxPts)
        //{
        //    _maxPts = maxPts;

        //    // assume data points are equally spaced, and calculate the X step size between
        //    // each data point
        //    double step = (_x[_x.Length - 1] - _x[0]) / (double)_x.Length;

        //    if (min < _x[0])
        //        min = _x[0];
        //    if (max > _x[_x.Length - 1])
        //        max = _x[_x.Length - 1];

        //    // calculate the index of the start of the bounded range
        //    int first = (int)((min - _x[0]) / step);

        //    // calculate the index of the last point of the bounded range
        //    int last = (int)((max - min) / step + first);

        //    // Make sure the bounded indices are legitimate
        //    first = Math.Max(Math.Min(first, _x.Length), 0);
        //    last = Math.Max(Math.Min(last, _x.Length), 0);

        //    _minBoundIndex = first;
        //    _maxBoundIndex = last;
        //}

        #endregion

    }
}
