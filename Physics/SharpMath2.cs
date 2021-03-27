/* MIT License

Copyright (c) 2017 Timothy Moore

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Libvaxy.Physics
{
    /// <summary>
    /// Describes a line that's projected onto a specified axis. This is a useful
    /// mathematical concept. Axis aligned lines *do* have position because they 
    /// are only used as an interim calculation, where position won't change.
    /// </summary>
    public class AxisAlignedLine2
    {
        /// <summary>
        /// The axis that this projected line is on. Optional.
        /// </summary>
        public readonly Vector2 Axis;

        /// <summary>
        /// The minimum of this line
        /// </summary>
        public readonly float Min;

        /// <summary>
        /// The maximum of this line
        /// </summary>
        public readonly float Max;

        /// <summary>
        /// Initializes an an axis aligned line. Will autocorrect if min &gt; max
        /// </summary>
        /// <param name="axis">The axis</param>
        /// <param name="min">The min</param>
        /// <param name="max">The max</param>
        public AxisAlignedLine2(Vector2 axis, float min, float max)
        {
            Axis = axis;

            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        /// <summary>
        /// Determines if line1 intersects line2.
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line2">Line 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If line1 and line2 intersect</returns>
        /// <exception cref="ArgumentException">if line1.Axis != line2.Axis</exception>
        public static bool Intersects(AxisAlignedLine2 line1, AxisAlignedLine2 line2, bool strict)
        {
            if (line1.Axis != line2.Axis)
                throw new ArgumentException($"Lines {line1} and {line2} are not aligned - you will need to convert to Line2 to check intersection.");

            return Intersects(line1.Min, line1.Max, line2.Min, line2.Max, strict, false);
        }

        /// <summary>
        /// Determines the best way for line1 to move to prevent intersection with line2
        /// </summary>
        /// <param name="line1">Line1</param>
        /// <param name="line2">Line2</param>
        /// <returns>MTV for line1</returns>
        public static float? IntersectMTV(AxisAlignedLine2 line1, AxisAlignedLine2 line2)
        {
            if (line1.Axis != line2.Axis)
                throw new ArgumentException($"Lines {line1} and {line2} are not aligned - you will need to convert to Line2 to check intersection.");

            return IntersectMTV(line1.Min, line1.Max, line2.Min, line2.Max, false);
        }

        /// <summary>
        /// Determines if the specified line contains the specified point.
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="point">The point</param>
        /// <param name="strict">If the edges of the line are excluded</param>
        /// <returns>if line contains point</returns>
        public static bool Contains(AxisAlignedLine2 line, float point, bool strict)
        {
            return Contains(line.Min, line.Max, point, strict, false);
        }

        /// <summary>
        /// Determines if axis aligned line (min1, max1) intersects (min2, max2)
        /// </summary>
        /// <param name="min1">Min 1</param>
        /// <param name="max1">Max 1</param>
        /// <param name="min2">Min 2</param>
        /// <param name="max2">Max 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <param name="correctMinMax">If true (default true) mins and maxes will be swapped if in the wrong order</param>
        /// <returns>If (min1, max1) intersects (min2, max2)</returns>
        public static bool Intersects(float min1, float max1, float min2, float max2, bool strict, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min1, tmp2 = max1;
                min1 = Math.Min(tmp1, tmp2);
                max1 = Math.Max(tmp1, tmp2);

                tmp1 = min2;
                tmp2 = max2;
                min2 = Math.Min(tmp1, tmp2);
                max2 = Math.Max(tmp1, tmp2);

            }

            if (strict)
                return (min1 <= min2 && max1 > min2) || (min2 <= min1 && max2 > min1);
            else
                return (min1 <= min2 && max1 >= min2) || (min2 <= min1 && max2 >= min1);
        }

        /// <summary>
        /// Determines the translation to move line 1 to have line 1 not intersect line 2. Returns
        /// null if line1 does not intersect line1.
        /// </summary>
        /// <param name="min1">Line 1 min</param>
        /// <param name="max1">Line 1 max</param>
        /// <param name="min2">Line 2 min</param>
        /// <param name="max2">Line 2 max</param>
        /// <param name="correctMinMax">If mins and maxs might be reversed</param>
        /// <returns>a number to move along the projected axis (positive or negative) or null if no intersection</returns>
        public static float? IntersectMTV(float min1, float max1, float min2, float max2, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min1, tmp2 = max1;
                min1 = Math.Min(tmp1, tmp2);
                max1 = Math.Max(tmp1, tmp2);

                tmp1 = min2;
                tmp2 = max2;
                min2 = Math.Min(tmp1, tmp2);
                max2 = Math.Max(tmp1, tmp2);
            }

            if (min1 <= min2 && max1 > min2)
                return min2 - max1;
            else if (min2 <= min1 && max2 > min1)
                return max2 - min1;
            return null;
        }

        /// <summary>
        /// Determines if the line from (min, max) contains point
        /// </summary>
        /// <param name="min">Min of line</param>
        /// <param name="max">Max of line</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">If edges are excluded</param>
        /// <param name="correctMinMax">if true (default true) min and max will be swapped if in the wrong order</param>
        /// <returns>if line (min, max) contains point</returns>
        public static bool Contains(float min, float max, float point, bool strict, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min, tmp2 = max;
                min = Math.Min(tmp1, tmp2);
                max = Math.Max(tmp1, tmp2);
            }

            if (strict)
                return min < point && max > point;
            else
                return min <= point && max >= point;
        }

        /// <summary>
        /// Detrmines the shortest distance from the line to get to point. Returns
        /// null if the point is on the line (not strict). Always returns a positive value.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="line">Line.</param>
        /// <param name="point">Point.</param>
        public static float? MinDistance(AxisAlignedLine2 line, float point)
        {
            return MinDistance(line.Min, line.Max, point, false);
        }

        /// <summary>
        /// Determines the shortest distance for line1 to go to touch line2. Returns
        /// null if line1 and line 2 intersect (not strictly)
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="line1">Line1.</param>
        /// <param name="line2">Line2.</param>
        public static float? MinDistance(AxisAlignedLine2 line1, AxisAlignedLine2 line2)
        {
            return MinDistance(line1.Min, line1.Max, line2.Min, line2.Max, false);
        }

        /// <summary>
        /// Determines the shortest distance from the line (min, max) to the point. Returns
        /// null if the point is on the line (not strict). Always returns a positive value.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="min">Minimum of line.</param>
        /// <param name="max">Maximum of line.</param>
        /// <param name="point">Point to check.</param>
        /// <param name="correctMinMax">If set to <c>true</c> will correct minimum max being reversed if they are</param>
        public static float? MinDistance(float min, float max, float point, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min, tmp2 = max;
                min = Math.Min(tmp1, tmp2);
                max = Math.Max(tmp1, tmp2);
            }

            if (point < min)
                return min - point;
            else if (point > max)
                return point - max;
            else
                return null;
        }

        /// <summary>
        /// Calculates the shortest distance for line1 (min1, max1) to get to line2 (min2, max2).
        /// Returns null if line1 and line2 intersect (not strictly)
        /// </summary>
        /// <returns>The distance along the mutual axis or null.</returns>
        /// <param name="min1">Min1.</param>
        /// <param name="max1">Max1.</param>
        /// <param name="min2">Min2.</param>
        /// <param name="max2">Max2.</param>
        /// <param name="correctMinMax">If set to <c>true</c> correct minimum max being potentially reversed.</param>
        public static float? MinDistance(float min1, float max1, float min2, float max2, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min1, tmp2 = max1;
                min1 = Math.Min(tmp1, tmp2);
                max1 = Math.Max(tmp1, tmp2);

                tmp1 = min2;
                tmp2 = max2;
                min2 = Math.Min(tmp1, tmp2);
                max2 = Math.Max(tmp1, tmp2);
            }

            if (min1 < min2)
            {
                if (max1 < min2)
                    return min2 - max1;
                else
                    return null;
            }
            else if (min2 < min1)
            {
                if (max2 < min1)
                    return min1 - max2;
                else
                    return null;
            }

            return null;
        }

        /// <summary>
        /// Creates a human-readable representation of this line
        /// </summary>
        /// <returns>string representation of this vector</returns>
        public override string ToString()
        {
            return $"[{Min} -> {Max} along {Axis}]";
        }
    }

    /// <summary>
    /// Describes a circle in the x-y plane.
    /// </summary>
    public struct Circle2
    {
        /// <summary>
        /// The radius of the circle
        /// </summary>
        public readonly float Radius;

        /// <summary>
        /// Constructs a circle with the specified radius
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        public Circle2(float radius)
        {
            Radius = radius;
        }

        /// <summary>
        /// Determines if the first circle is equal to the second circle
        /// </summary>
        /// <param name="c1">The first circle</param>
        /// <param name="c2">The second circle</param>
        /// <returns>If c1 is equal to c2</returns>
        public static bool operator ==(Circle2 c1, Circle2 c2)
        {
            if (ReferenceEquals(c1, null) || ReferenceEquals(c2, null))
                return ReferenceEquals(c1, c2);

            return c1.Radius == c2.Radius;
        }

        /// <summary>
        /// Determines if the first circle is not equal to the second circle
        /// </summary>
        /// <param name="c1">The first circle</param>
        /// <param name="c2">The second circle</param>
        /// <returns>If c1 is not equal to c2</returns>
        public static bool operator !=(Circle2 c1, Circle2 c2)
        {
            if (ReferenceEquals(c1, null) || ReferenceEquals(c2, null))
                return !ReferenceEquals(c1, c2);

            return c1.Radius != c2.Radius;
        }

        /// <summary>
        /// Determines if this circle is logically the same as the 
        /// specified object.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>if it is a circle with the same radius</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Circle2))
                return false;

            var other = (Circle2)obj;
            return this == other;
        }

        /// <summary>
        /// Calculate a hashcode based solely on the radius of this circle.
        /// </summary>
        /// <returns>hashcode</returns>
        public override int GetHashCode()
        {
            return Radius.GetHashCode();
        }

        /// <summary>
        /// Determines if the circle at the specified position contains the point
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="pos">The top-left of the circles bounding box</param>
        /// <param name="point">The point to check if is in the circle at pos</param>
        /// <param name="strict">If the edges do not count</param>
        /// <returns>If the circle at pos contains point</returns>
        public static bool Contains(Circle2 circle, Vector2 pos, Vector2 point, bool strict)
        {
            var distSq = (point - new Vector2(pos.X + circle.Radius, pos.Y + circle.Radius)).LengthSquared();

            if (strict)
                return distSq < circle.Radius * circle.Radius;
            else
                return distSq <= circle.Radius * circle.Radius;
        }

        /// <summary>
        /// Determines if the first circle at the specified position intersects the second circle
        /// at the specified position.
        /// </summary>
        /// <param name="circle1">First circle</param>
        /// <param name="circle2">Second circle</param>
        /// <param name="pos1">Top-left of the bounding box of the first circle</param>
        /// <param name="pos2">Top-left of the bounding box of the second circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle1 at pos1 intersects circle2 at pos2</returns>
        public static bool Intersects(Circle2 circle1, Circle2 circle2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(circle1.Radius, circle2.Radius, pos1, pos2, strict);
        }

        /// <summary>
        /// Determines if the first circle of specified radius and (bounding box top left) intersects
        /// the second circle of specified radius and (bounding box top left)
        /// </summary>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="radius2">Radius of the second circle</param>
        /// <param name="pos1">Top-left of the bounding box of the first circle</param>
        /// <param name="pos2">Top-left of the bounding box of the second circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle1 of radius=radius1, topleft=pos1 intersects circle2 of radius=radius2, topleft=pos2</returns>
        public static bool Intersects(float radius1, float radius2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            var vecCenterToCenter = pos1 - pos2;
            vecCenterToCenter.X += radius1 - radius2;
            vecCenterToCenter.Y += radius1 - radius2;
            var distSq = vecCenterToCenter.LengthSquared();
            return distSq < (radius1 + radius2) * (radius1 + radius2);
        }

        /// <summary>
        /// Determines the shortest axis and overlap for which the first circle at the specified position
        /// overlaps the second circle at the specified position. If the circles do not overlap, returns null.
        /// </summary>
        /// <param name="circle1">First circle</param>
        /// <param name="circle2">Second circle</param>
        /// <param name="pos1">Top-left of the first circles bounding box</param>
        /// <param name="pos2">Top-left of the second circles bounding box</param>
        /// <returns></returns>
        public static Tuple<Vector2, float> IntersectMTV(Circle2 circle1, Circle2 circle2, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMTV(circle1.Radius, circle2.Radius, pos1, pos2);
        }

        /// <summary>
        /// Determines the shortest axis and overlap for which the first circle, specified by its radius and its bounding
        /// box's top-left, intersects the second circle specified by its radius and bounding box top-left. Returns null if
        /// the circles do not overlap.
        /// </summary>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="radius2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns>The direction and magnitude to move pos1 to prevent intersection</returns>
        public static Tuple<Vector2, float> IntersectMTV(float radius1, float radius2, Vector2 pos1, Vector2 pos2)
        {
            var betweenVec = pos1 - pos2;
            betweenVec.X += (radius1 - radius2);
            betweenVec.Y += (radius1 - radius2);

            var lengthSq = betweenVec.LengthSquared();
            if (lengthSq < (radius1 + radius2) * (radius1 + radius2))
            {
                var len = Math.Sqrt(lengthSq);
                betweenVec *= (float)(1 / len);

                return Tuple.Create(betweenVec, radius1 + radius2 - (float)len);
            }
            return null;
        }

        /// <summary>
        /// Projects the specified circle with the upper-left at the specified position onto
        /// the specified axis. 
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="pos">The position of the circle</param>
        /// <param name="axis">the axis to project along</param>
        /// <returns>Projects circle at pos along axis</returns>
        public static AxisAlignedLine2 ProjectAlongAxis(Circle2 circle, Vector2 pos, Vector2 axis)
        {
            return ProjectAlongAxis(circle.Radius, pos, axis);
        }

        /// <summary>
        /// Projects a circle defined by its radius and the top-left of its bounding box along
        /// the specified axis.
        /// </summary>
        /// <param name="radius">Radius of the circle to project</param>
        /// <param name="pos">Position of the circle</param>
        /// <param name="axis">Axis to project on</param>
        /// <returns></returns>
        public static AxisAlignedLine2 ProjectAlongAxis(float radius, Vector2 pos, Vector2 axis)
        {
            var centerProj = Vector2.Dot(new Vector2(pos.X + radius, pos.Y + radius), axis);

            return new AxisAlignedLine2(axis, centerProj - radius, centerProj + radius);
        }
    }

    public enum LineInterType
    {
        /// <summary>
        /// Two segments with different slopes which do not intersect
        /// </summary>
        NonParallelNone,
        /// <summary>
        /// Two segments with different slopes which intersect at a 
        /// single point.
        /// </summary>
        NonParallelPoint,
        /// <summary>
        /// Two parallel but not coincident segments. These never intersect
        /// </summary>
        ParallelNone,
        /// <summary>
        /// Two coincident segments which do not intersect
        /// </summary>
        CoincidentNone,
        /// <summary>
        /// Two coincident segments which intersect at a point
        /// </summary>
        CoincidentPoint,
        /// <summary>
        /// Two coincident segments which intersect on infinitely many points
        /// </summary>
        CoincidentLine
    }

    /// <summary>
    /// Describes a line. Does not have position and is meant to be reused.
    /// </summary>
    public class Line2
    {
        /// <summary>
        /// Where the line begins
        /// </summary>
        public readonly Vector2 Start;

        /// <summary>
        /// Where the line ends
        /// </summary>
        public readonly Vector2 End;

        /// <summary>
        /// End - Start
        /// </summary>
        public readonly Vector2 Delta;

        /// <summary>
        /// Normalized Delta
        /// </summary>
        public readonly Vector2 Axis;

        /// <summary>
        /// The normalized normal of axis.
        /// </summary>
        public readonly Vector2 Normal;

        /// <summary>
        /// Square of the magnitude of this line
        /// </summary>
        public readonly float MagnitudeSquared;

        /// <summary>
        /// Magnitude of this line
        /// </summary>
        public readonly float Magnitude;

        /// <summary>
        /// Min x
        /// </summary>
        public readonly float MinX;
        /// <summary>
        /// Min y
        /// </summary>
        public readonly float MinY;

        /// <summary>
        /// Max x
        /// </summary>
        public readonly float MaxX;

        /// <summary>
        /// Max y
        /// </summary>
        public readonly float MaxY;

        /// <summary>
        /// Slope of this line
        /// </summary>
        public readonly float Slope;

        /// <summary>
        /// Where this line would hit the y intercept. NaN if vertical line.
        /// </summary>
        public readonly float YIntercept;

        /// <summary>
        /// If this line is horizontal
        /// </summary>
        public readonly bool Horizontal;

        /// <summary>
        /// If this line is vertical
        /// </summary>
        public readonly bool Vertical;

        /// <summary>
        /// Creates a line from start to end
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        public Line2(Vector2 start, Vector2 end)
        {
            if (Math2.Approximately(start, end))
                throw new ArgumentException($"start is approximately end - that's a point, not a line. start={start}, end={end}");

            Start = start;
            End = end;


            Delta = End - Start;
            Axis = Vector2.Normalize(Delta);
            Normal = Vector2.Normalize(Math2.Perpendicular(Delta));
            MagnitudeSquared = Delta.LengthSquared();
            Magnitude = (float)Math.Sqrt(MagnitudeSquared);

            MinX = Math.Min(Start.X, End.X);
            MinY = Math.Min(Start.Y, End.Y);
            MaxX = Math.Max(Start.X, End.X);
            MaxY = Math.Max(Start.Y, End.Y);
            Horizontal = Math.Abs(End.Y - Start.Y) <= Math2.DEFAULT_EPSILON;
            Vertical = Math.Abs(End.X - Start.X) <= Math2.DEFAULT_EPSILON;

            if (Vertical)
                Slope = float.PositiveInfinity;
            else
                Slope = (End.Y - Start.Y) / (End.X - Start.X);

            if (Vertical)
                YIntercept = float.NaN;
            else
            {
                // y = mx + b
                // Start.Y = Slope * Start.X + b
                // b = Start.Y - Slope * Start.X
                YIntercept = Start.Y - Slope * Start.X;
            }
        }

        /// <summary>
        /// Determines if the two lines are parallel. Shifting lines will not
        /// effect the result.
        /// </summary>
        /// <param name="line1">The first line</param>
        /// <param name="line2">The second line</param>
        /// <returns>True if the lines are parallel, false otherwise</returns>
        public static bool Parallel(Line2 line1, Line2 line2)
        {
            return (
                Math2.Approximately(line1.Axis, line2.Axis)
                || Math2.Approximately(line1.Axis, -line2.Axis)
                );
        }

        /// <summary>
        /// Determines if the given point is along the infinite line described
        /// by the given line shifted the given amount.
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="pos">The shift for the line</param>
        /// <param name="pt">The point</param>
        /// <returns>True if pt is on the infinite line extension of the segment</returns>
        public static bool AlongInfiniteLine(Line2 line, Vector2 pos, Vector2 pt)
        {
            float normalPart = Vector2.Dot(pt - pos - line.Start, line.Normal);
            return Math2.Approximately(normalPart, 0);
        }

        /// <summary>
        /// Determines if the given line contains the given point.
        /// </summary>
        /// <param name="line">The line to check</param>
        /// <param name="pos">The offset for the line</param>
        /// <param name="pt">The point to check</param>
        /// <returns>True if pt is on the line, false otherwise</returns>
        public static bool Contains(Line2 line, Vector2 pos, Vector2 pt)
        {
            // The horizontal/vertical checks are not required but are
            // very fast to calculate and short-circuit the common case
            // (false) very quickly
            if (line.Horizontal)
            {
                return Math2.Approximately(line.Start.Y + pos.Y, pt.Y)
                    && AxisAlignedLine2.Contains(line.MinX, line.MaxX, pt.X - pos.X, false, false);
            }
            if (line.Vertical)
            {
                return Math2.Approximately(line.Start.X + pos.X, pt.X)
                    && AxisAlignedLine2.Contains(line.MinY, line.MaxY, pt.Y - pos.Y, false, false);
            }

            // Our line is not necessarily a linear space, but if we shift
            // our line to the origin and adjust the point correspondingly
            // then we have a linear space and the problem remains the same.

            // Our line at the origin is just the infinite line with slope
            // Axis. We can form an orthonormal basis of R2 as (Axis, Normal).
            // Hence we can write pt = line_part * Axis + normal_part * Normal. 
            // where line_part and normal_part are floats. If the normal_part
            // is 0, then pt = line_part * Axis, hence the point is on the
            // infinite line.

            // Since we are working with an orthonormal basis, we can find
            // components with dot products.

            // To check the finite line, we consider the start of the line
            // the origin. Then the end of the line is line.Magnitude * line.Axis.

            Vector2 lineStart = pos + line.Start;

            float normalPart = Math2.Dot(pt - lineStart, line.Normal);
            if (!Math2.Approximately(normalPart, 0))
                return false;

            float axisPart = Math2.Dot(pt - lineStart, line.Axis);
            return axisPart > -Math2.DEFAULT_EPSILON
                && axisPart < line.Magnitude + Math2.DEFAULT_EPSILON;
        }

        private static void FindSortedOverlap(float[] projs, bool[] isFromLine1)
        {
            // ascending insertion sort while simultaneously updating 
            // isFromLine1
            for (int i = 0; i < 3; i++)
            {
                int best = i;
                for (int j = i + 1; j < 4; j++)
                {
                    if (projs[j] < projs[best])
                    {
                        best = j;
                    }
                }
                if (best != i)
                {
                    float projTmp = projs[i];
                    projs[i] = projs[best];
                    projs[best] = projTmp;
                    bool isFromLine1Tmp = isFromLine1[i];
                    isFromLine1[i] = isFromLine1[best];
                    isFromLine1[best] = isFromLine1Tmp;
                }
            }
        }

        /// <summary>
        /// Checks the type of intersection between the two coincident lines.
        /// </summary>
        /// <param name="a">The first line</param>
        /// <param name="b">The second line</param>
        /// <param name="pos1">The offset for the first line</param>
        /// <param name="pos2">The offset for the second line</param>
        /// <returns>The type of intersection</returns>
        public static LineInterType CheckCoincidentIntersectionType(Line2 a, Line2 b, Vector2 pos1, Vector2 pos2)
        {
            Vector2 relOrigin = a.Start + pos1;

            float[] projs = new float[4] {
                0,
                a.Magnitude,
                Math2.Dot((b.Start + pos2) - relOrigin, a.Axis),
                Math2.Dot((b.End + pos2) - relOrigin, a.Axis)
            };

            bool[] isFromLine1 = new bool[4] {
                true,
                true,
                false,
                false
            };

            FindSortedOverlap(projs, isFromLine1);

            if (Math2.Approximately(projs[1], projs[2]))
                return LineInterType.CoincidentPoint;
            if (isFromLine1[0] == isFromLine1[1])
                return LineInterType.CoincidentNone;
            return LineInterType.CoincidentLine;
        }

        /// <summary>
        /// Determines if line1 intersects line2, when line1 is offset by pos1 and line2 
        /// is offset by pos2.
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line2">Line 2</param>
        /// <param name="pos1">Origin of line 1</param>
        /// <param name="pos2">Origin of line 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If line1 intersects line2</returns>
        public static bool Intersects(Line2 line1, Line2 line2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            if (Parallel(line1, line2))
            {
                if (!AlongInfiniteLine(line1, pos1, line2.Start + pos2))
                    return false;
                LineInterType iType = CheckCoincidentIntersectionType(line1, line2, pos1, pos2);
                if (iType == LineInterType.CoincidentNone)
                    return false;
                if (iType == LineInterType.CoincidentPoint)
                    return !strict;
                return true;
            }

            return GetIntersection(line1, line2, pos1, pos2, strict, out Vector2 pt);
        }

        /// <summary>
        /// Finds the intersection of two non-parallel lines a and b. Returns
        /// true if the point of intersection is on both line segments, returns
        /// false if the point of intersection is not on at least one line
        /// segment. In either case, pt is set to where the intersection is
        /// on the infinite lines.
        /// </summary>
        /// <param name="line1">First line</param>
        /// <param name="line2">Second line</param>
        /// <param name="pos1">The shift of the first line</param>
        /// <param name="pos2">The shift of the second line</param>
        /// <param name="strict">True if we should return true if pt is on an edge of a line as well
        /// as in the middle of the line. False to return true only if pt is really within the lines</param>
        /// <returns>True if both segments contain the pt, false otherwise</returns>
        public static bool GetIntersection(Line2 line1, Line2 line2, Vector2 pos1, Vector2 pos2, bool strict, out Vector2 pt)
        {
            // The infinite lines intersect at exactly one point. The segments intersect
            // if they both contain that point. We will treat the lines as first-degree
            // Bezier lines to skip the vertical case
            // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection

            float x1 = line1.Start.X + pos1.X;
            float x2 = line1.End.X + pos1.X;
            float x3 = line2.Start.X + pos2.X;
            float x4 = line2.End.X + pos2.X;
            float y1 = line1.Start.Y + pos1.Y;
            float y2 = line1.End.Y + pos1.Y;
            float y3 = line2.Start.Y + pos2.Y;
            float y4 = line2.End.Y + pos2.Y;

            float det = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            // we assume det != 0 (lines not parallel)

            var t = (
                ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / det
            );

            pt = new Vector2(x1 + (x2 - x1) * t, y1 + (y2 - y1) * t);

            float min = strict ? Math2.DEFAULT_EPSILON : -Math2.DEFAULT_EPSILON;
            float max = 1 - min;

            if (t < min || t > max)
                return false;

            float u = -(
                ((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / det
            );
            return u >= min && u <= max;
        }

        /// <summary>
        /// Finds the line of overlap between the the two lines if there is
        /// one. If the two lines are not coincident (i.e., if the infinite
        /// lines are not the same) then they don't share a line of points.
        /// If they are coincident, they may still share no points (two
        /// seperate but coincident line segments), one point (they share
        /// an edge), or infinitely many points (the share a coincident
        /// line segment). In all but the last case, this returns false
        /// and overlap is set to null. In the last case this returns true
        /// and overlap is set to the line of overlap.
        /// </summary>
        /// <param name="a">The first line</param>
        /// <param name="b">The second line</param>
        /// <param name="pos1">The position of the first line</param>
        /// <param name="pos2">the position of the second line</param>
        /// <param name="overlap">Set to null or the line of overlap</param>
        /// <returns>True if a and b overlap at infinitely many points,
        /// false otherwise</returns>
        public static bool LineOverlap(Line2 a, Line2 b, Vector2 pos1, Vector2 pos2, out Line2 overlap)
        {
            if (!Parallel(a, b))
            {
                overlap = null;
                return false;
            }
            if (!AlongInfiniteLine(a, pos1, b.Start + pos2))
            {
                overlap = null;
                return false;
            }

            Vector2 relOrigin = a.Start + pos1;

            float[] projs = new float[4] {
                0,
                a.Magnitude,
                Math2.Dot((b.Start + pos2) - relOrigin, a.Axis),
                Math2.Dot((b.End + pos2) - relOrigin, a.Axis)
            };

            bool[] isFromLine1 = new bool[4] {
                true,
                true,
                false,
                false
            };

            FindSortedOverlap(projs, isFromLine1);

            if (isFromLine1[0] == isFromLine1[1])
            {
                // at best we overlap at one point, most likely no overlap
                overlap = null;
                return false;
            }

            if (Math2.Approximately(projs[1], projs[2]))
            {
                // Overlap at one point
                overlap = null;
                return false;
            }

            overlap = new Line2(
                relOrigin + projs[1] * a.Axis,
                relOrigin + projs[2] * a.Axis
            );
            return true;
        }

        /// <summary>
        /// Create a human-readable representation of this line
        /// </summary>
        /// <returns>human-readable string</returns>
        public override string ToString()
        {
            return $"[{Start} to {End}]";
        }
    }

    /// <summary>
    /// Contains utility functions for doing math in two-dimensions that
    /// don't fit elsewhere. Also contains any necessary constants.
    /// </summary>
    public class Math2
    {
        /// <summary>
        /// Default epsilon
        /// </summary>
        public const float DEFAULT_EPSILON = 0.001f;

        /// <summary>
        /// Determines if v1, v2, and v3 are collinear
        /// </summary>
        /// <remarks>
        /// Calculates if the area of the triangle of v1, v2, v3 is less than or equal to epsilon.
        /// </remarks>
        /// <param name="v1">Vector 1</param>
        /// <param name="v2">Vector 2</param>
        /// <param name="v3">Vector 3</param>
        /// <param name="epsilon">How close is close enough</param>
        /// <returns>If v1, v2, v3 is collinear</returns>
        public static bool IsOnLine(Vector2 v1, Vector2 v2, Vector2 v3, float epsilon = DEFAULT_EPSILON)
        {
            return AreaOfTriangle(v1, v2, v3) <= epsilon;
        }

        /// <summary>
        /// Calculates the square of the area of the triangle made up of the specified points.
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <param name="v3">Third point</param>
        /// <returns>Area of the triangle made up of the given 3 points</returns>
        public static float AreaOfTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return 0.5f * Math.Abs((v2.X - v1.X) * (v3.Y - v1.Y) - (v3.X - v1.X) * (v2.Y - v1.Y));
        }

        /// <summary>
        /// Finds a vector that is perpendicular to the specified vector.
        /// </summary>
        /// <returns>A vector perpendicular to v</returns>
        /// <param name="v">Vector</param>
        public static Vector2 Perpendicular(Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        /// <summary>
        /// Finds the dot product of (x1, y1) and (x2, y2)
        /// </summary>
        /// <returns>The dot.</returns>
        /// <param name="x1">The first x value.</param>
        /// <param name="y1">The first y value.</param>
        /// <param name="x2">The second x value.</param>
        /// <param name="y2">The second y value.</param>
        public static float Dot(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return Dot(v1.X, v1.Y, v2.X, v2.Y);
        }

        public static float Dot(Vector2 v, float x2, float y2)
        {
            return Dot(v.X, v.Y, x2, y2);
        }

        /// <summary>
        /// Determines if f1 and f2 are approximately the same.
        /// </summary>
        /// <returns>The approximately.</returns>
        /// <param name="f1">F1.</param>
        /// <param name="f2">F2.</param>
        /// <param name="epsilon">Epsilon.</param>
        public static bool Approximately(float f1, float f2, float epsilon = DEFAULT_EPSILON)
        {
            return Math.Abs(f1 - f2) <= epsilon;
        }

        /// <summary>
        /// Determines if vectors v1 and v2 are approximately equal, such that
        /// both coordinates are within epsilon.
        /// </summary>
        /// <returns>If v1 and v2 are approximately equal.</returns>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <param name="epsilon">Epsilon.</param>
        public static bool Approximately(Vector2 v1, Vector2 v2, float epsilon = DEFAULT_EPSILON)
        {
            return Approximately(v1.X, v2.X, epsilon) && Approximately(v1.Y, v2.Y, epsilon);
        }

        /// <summary>
        /// Rotates the specified vector about the specified vector a rotation of the
        /// specified amount.
        /// </summary>
        /// <param name="vec">The vector to rotate</param>
        /// <param name="about">The point to rotate vec around</param>
        /// <param name="rotation">The rotation</param>
        /// <returns>The vector vec rotated about about rotation.Theta radians.</returns>
        public static Vector2 Rotate(Vector2 vec, Vector2 about, Rotation2 rotation)
        {
            if (rotation.Theta == 0)
                return vec;
            var tmp = vec - about;
            return new Vector2(tmp.X * rotation.CosTheta - tmp.Y * rotation.SinTheta + about.X,
                               tmp.X * rotation.SinTheta + tmp.Y * rotation.CosTheta + about.Y);
        }

        /// <summary>
        /// Returns either the vector or -vector such that MakeStandardNormal(vec) == MakeStandardNormal(-vec)
        /// </summary>
        /// <param name="vec">The vector</param>
        /// <returns>Normal such that vec.X is positive (unless vec.X is 0, in which such that vec.Y is positive)</returns>
        public static Vector2 MakeStandardNormal(Vector2 vec)
        {
            if (vec.X < -DEFAULT_EPSILON)
                return -vec;

            if (Approximately(vec.X, 0) && vec.Y < 0)
                return -vec;

            return vec;
        }
    }

    /// <summary>
    /// Describes a simple polygon based on it's vertices. Does not
    /// have position - most functions require specifying the origin of the
    /// polygon. Polygons are meant to be reused.
    /// </summary>
    public class Polygon2 : Shape2
    {
        /// <summary>
        /// The vertices of this polygon, such that any two adjacent vertices
        /// create a line of the polygon
        /// </summary>
        public readonly Vector2[] Vertices;

        /// <summary>
        /// The lines of this polygon, such that any two adjacent (wrapping)
        /// lines share a vertex
        /// </summary>
        public readonly Line2[] Lines;

        /// <summary>
        /// The center of this polyogn
        /// </summary>
        public readonly Vector2 Center;

        /// <summary>
        /// This convex polygon partitioned into triangles, sorted by the area
        /// of the triangles in descending order
        /// </summary>
        public readonly Triangle2[] TrianglePartition;

        /// <summary>
        /// The three normal vectors of this polygon, normalized
        /// </summary>
        public readonly List<Vector2> Normals;

        /// <summary>
        /// The bounding box.
        /// </summary>
        public readonly Rect2 AABB;

        private float _LongestAxisLength;

        /// <summary>
        /// The longest line that can be created inside this polygon. 
        /// <example>
        /// var poly = ShapeUtils.CreateRectangle(2, 3);
        /// 
        /// Console.WriteLine($"corner-to-corner = longest axis = Math.Sqrt(2 * 2 + 3 * 3) = {Math.Sqrt(2 * 2 + 3 * 3)} = {poly.LongestAxisLength}");
        /// </example>
        /// </summary>
        public float LongestAxisLength
        {
            get
            {
                if (_LongestAxisLength < 0)
                {
                    Vector2[] verts = Vertices;
                    float longestAxisLenSq = -1;
                    for (int i = 1, len = verts.Length; i < len; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            var vec = verts[i] - verts[j];
                            var vecLenSq = vec.LengthSquared();
                            if (vecLenSq > longestAxisLenSq)
                                longestAxisLenSq = vecLenSq;
                        }
                    }
                    _LongestAxisLength = (float)Math.Sqrt(longestAxisLenSq);
                }

                return _LongestAxisLength;
            }
        }

        /// <summary>
        /// The area of this polygon
        /// </summary>
        public readonly float Area;

        /// <summary>
        /// If this polygon is defined clockwise
        /// </summary>
        public readonly bool Clockwise;

        /// <summary>
        /// Initializes a polygon with the specified vertices
        /// </summary>
        /// <param name="vertices">Vertices</param>
        /// <exception cref="ArgumentNullException">If vertices is null</exception>
        public Polygon2(Vector2[] vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            Vertices = vertices;

            Normals = new List<Vector2>();
            Vector2 tmp;
            for (int i = 1; i < vertices.Length; i++)
            {
                tmp = Math2.MakeStandardNormal(Vector2.Normalize(Math2.Perpendicular(vertices[i] - vertices[i - 1])));
                if (!Normals.Contains(tmp))
                    Normals.Add(tmp);
            }

            tmp = Math2.MakeStandardNormal(Vector2.Normalize(Math2.Perpendicular(vertices[0] - vertices[vertices.Length - 1])));
            if (!Normals.Contains(tmp))
                Normals.Add(tmp);

            var min = new Vector2(vertices[0].X, vertices[0].Y);
            var max = new Vector2(min.X, min.Y);
            for (int i = 1; i < vertices.Length; i++)
            {
                min.X = Math.Min(min.X, vertices[i].X);
                min.Y = Math.Min(min.Y, vertices[i].Y);
                max.X = Math.Max(max.X, vertices[i].X);
                max.Y = Math.Max(max.Y, vertices[i].Y);
            }
            AABB = new Rect2(min, max);

            _LongestAxisLength = -1;

            // Center, area, and lines
            TrianglePartition = new Triangle2[Vertices.Length - 2];
            float[] triangleSortKeys = new float[TrianglePartition.Length];
            float area = 0;
            Lines = new Line2[Vertices.Length];
            Lines[0] = new Line2(Vertices[Vertices.Length - 1], Vertices[0]);
            var last = Vertices[0];
            Center = new Vector2(0, 0);
            for (int i = 1; i < Vertices.Length - 1; i++)
            {
                var next = Vertices[i];
                var next2 = Vertices[i + 1];
                Lines[i] = new Line2(last, next);
                var tri = new Triangle2(new Vector2[] { Vertices[0], next, next2 });
                TrianglePartition[i - 1] = tri;
                triangleSortKeys[i - 1] = -tri.Area;
                area += tri.Area;
                Center += tri.Center * tri.Area;
                last = next;
            }
            Lines[Vertices.Length - 1] = new Line2(Vertices[Vertices.Length - 2], Vertices[Vertices.Length - 1]);

            Array.Sort(triangleSortKeys, TrianglePartition);

            Area = area;
            Center /= area;

            last = Vertices[Vertices.Length - 1];
            var centToLast = (last - Center);
            var angLast = Math.Atan2(centToLast.Y, centToLast.X);
            var cwCounter = 0;
            var ccwCounter = 0;
            var foundDefinitiveResult = false;
            for (int i = 0; i < Vertices.Length; i++)
            {
                var curr = Vertices[i];
                var centToCurr = (curr - Center);
                var angCurr = Math.Atan2(centToCurr.Y, centToCurr.X);

                var clockwise = angCurr < angLast;
                if (clockwise)
                    cwCounter++;
                else
                    ccwCounter++;

                Clockwise = clockwise;
                if (Math.Abs(angLast - angCurr) > Math2.DEFAULT_EPSILON)
                {
                    foundDefinitiveResult = true;
                    break;
                }

                last = curr;
                centToLast = centToCurr;
                angLast = angCurr;
            }
            if (!foundDefinitiveResult)
                Clockwise = cwCounter > ccwCounter;
        }

        /// <summary>
        /// Determines if the specified polygon at the specified position and rotation contains the specified point
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="pos">Origin of the polygon</param>
        /// <param name="rot">Rotation of the polygon</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">True if the edges do not count as inside</param>
        /// <returns>If the polygon at pos with rotation rot about its center contains point</returns>
        public static bool Contains(Polygon2 poly, Vector2 pos, Rotation2 rot, Vector2 point, bool strict)
        {
            // The point is contained in the polygon iff it is contained in one of the triangles
            // which partition this polygon. Due to how we constructed the triangles, it will
            // be on the edge of the polygon if its on the first 2 edges of the triangle.

            for (int i = 0, len = poly.TrianglePartition.Length; i < len; i++)
            {
                var tri = poly.TrianglePartition[i];

                if (Triangle2.Contains(tri, pos, point))
                {
                    if (strict && (Line2.Contains(tri.Edges[0], pos, point) || Line2.Contains(tri.Edges[1], pos, point)))
                        return false;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if the first polygon intersects the second polygon when they are at
        /// the respective positions and rotations.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Position of the first polygon</param>
        /// <param name="pos2">Position of the second polygon</param>
        /// <param name="rot1">Rotation of the first polygon</param>
        /// <param name="rot2">Rotation fo the second polyogn</param>
        /// <param name="strict">If overlapping is required for intersection</param>
        /// <returns>If poly1 at pos1 with rotation rot1 intersects poly2 at pos2with rotation rot2</returns>
        public static bool Intersects(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2, Rotation2 rot1, Rotation2 rot2, bool strict)
        {
            if (rot1 == Rotation2.Zero && rot2 == Rotation2.Zero)
            {
                // This was a serious performance bottleneck so we speed up the fast case
                HashSet<Vector2> seen = new HashSet<Vector2>();
                Vector2[] poly1Verts = poly1.Vertices;
                Vector2[] poly2Verts = poly2.Vertices;
                for (int i = 0, len = poly1.Normals.Count; i < len; i++)
                {
                    var axis = poly1.Normals[i];
                    var proj1 = ProjectAlongAxis(axis, pos1, poly1Verts);
                    var proj2 = ProjectAlongAxis(axis, pos2, poly2Verts);
                    if (!AxisAlignedLine2.Intersects(proj1, proj2, strict))
                        return false;
                    seen.Add(axis);
                }
                for (int i = 0, len = poly2.Normals.Count; i < len; i++)
                {
                    var axis = poly2.Normals[i];
                    if (seen.Contains(axis))
                        continue;

                    var proj1 = ProjectAlongAxis(axis, pos1, poly1Verts);
                    var proj2 = ProjectAlongAxis(axis, pos2, poly2Verts);
                    if (!AxisAlignedLine2.Intersects(proj1, proj2, strict))
                        return false;
                }
                return true;
            }

            foreach (var norm in poly1.Normals.Select((v) => Tuple.Create(v, rot1)).Union(poly2.Normals.Select((v) => Tuple.Create(v, rot2))))
            {
                var axis = Math2.Rotate(norm.Item1, Vector2.Zero, norm.Item2);
                if (!IntersectsAlongAxis(poly1, poly2, pos1, pos2, rot1, rot2, strict, axis))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines the mtv to move pos1 by to prevent poly1 at pos1 from intersecting poly2 at pos2.
        /// Returns null if poly1 and poly2 do not intersect.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Position of the first polygon</param>
        /// <param name="pos2">Position of the second polygon</param>
        /// <param name="rot1">Rotation of the first polyogn</param>
        /// <param name="rot2">Rotation of the second polygon</param>
        /// <returns>MTV to move poly1 to prevent intersection with poly2</returns>
        public static Tuple<Vector2, float> IntersectMTV(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2, Rotation2 rot1, Rotation2 rot2)
        {
            Vector2 bestAxis = Vector2.Zero;
            float bestMagn = float.MaxValue;

            foreach (var norm in poly1.Normals.Select((v) => Tuple.Create(v, rot1)).Union(poly2.Normals.Select((v) => Tuple.Create(v, rot2))))
            {
                var axis = Math2.Rotate(norm.Item1, Vector2.Zero, norm.Item2);
                var mtv = IntersectMTVAlongAxis(poly1, poly2, pos1, pos2, rot1, rot2, axis);
                if (!mtv.HasValue)
                    return null;
                else if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = axis;
                    bestMagn = mtv.Value;
                }
            }

            return Tuple.Create(bestAxis, bestMagn);
        }

        /// <summary>
        /// Determines if polygon 1 and polygon 2 at position 1 and position 2, respectively, intersect along axis.
        /// </summary>
        /// <param name="poly1">polygon 1</param>
        /// <param name="poly2">polygon 2</param>
        /// <param name="pos1">Origin of polygon 1</param>
        /// <param name="pos2">Origin of polygon 2</param>
        /// <param name="rot1">Rotation of the first polygon</param>
        /// <param name="rot2">Rotation of the second polygon</param>
        /// <param name="strict">If overlapping is required for intersection</param>
        /// <param name="axis">The axis to check</param>
        /// <returns>If poly1 at pos1 intersects poly2 at pos2 along axis</returns>
        public static bool IntersectsAlongAxis(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2, Rotation2 rot1, Rotation2 rot2, bool strict, Vector2 axis)
        {
            var proj1 = ProjectAlongAxis(poly1, pos1, rot1, axis);
            var proj2 = ProjectAlongAxis(poly2, pos2, rot2, axis);

            return AxisAlignedLine2.Intersects(proj1, proj2, strict);
        }

        /// <summary>
        /// Determines the distance along axis, if any, that polygon 1 should be shifted by
        /// to prevent intersection with polygon 2. Null if no intersection along axis.
        /// </summary>
        /// <param name="poly1">polygon 1</param>
        /// <param name="poly2">polygon 2</param>
        /// <param name="pos1">polygon 1 origin</param>
        /// <param name="pos2">polygon 2 origin</param>
        /// <param name="rot1">polygon 1 rotation</param>
        /// <param name="rot2">polygon 2 rotation</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>a number to shift pos1 along axis by to prevent poly1 at pos1 from intersecting poly2 at pos2, or null if no int. along axis</returns>
        public static float? IntersectMTVAlongAxis(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2, Rotation2 rot1, Rotation2 rot2, Vector2 axis)
        {
            var proj1 = ProjectAlongAxis(poly1, pos1, rot1, axis);
            var proj2 = ProjectAlongAxis(poly2, pos2, rot2, axis);

            return AxisAlignedLine2.IntersectMTV(proj1, proj2);
        }
        /// <summary>
        /// Projects the polygon at position onto the specified axis.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="pos">The polygons origin</param>
        /// <param name="rot">the rotation of the polygon</param>
        /// <param name="axis">The axis to project onto</param>
        /// <returns>poly at pos projected along axis</returns>
        public static AxisAlignedLine2 ProjectAlongAxis(Polygon2 poly, Vector2 pos, Rotation2 rot, Vector2 axis)
        {
            return ProjectAlongAxis(axis, pos, rot, poly.Center, poly.Vertices);
        }

        /// <summary>
        /// Calculates the shortest distance from the specified polygon to the specified point,
        /// and the axis from polygon to pos.
        /// 
        /// Returns null if pt is contained in the polygon (not strictly).
        /// </summary>
        /// <returns>The distance form poly to pt.</returns>
        /// <param name="poly">The polygon</param>
        /// <param name="pos">Origin of the polygon</param>
        /// <param name="rot">Rotation of the polygon</param>
        /// <param name="pt">Point to check.</param>
        public static Tuple<Vector2, float> MinDistance(Polygon2 poly, Vector2 pos, Rotation2 rot, Vector2 pt)
        {
            /*
             * Definitions
             * 
             * For each line in the polygon, find the normal of the line in the direction of outside the polygon.
             * Call the side of the original line that contains none of the polygon "above the line". The other side is "below the line".
             * 
             * If the point falls above the line:
             *   Imagine two additional lines that are normal to the line and fall on the start and end, respectively.
             *   For each of those two lines, call the side of the line that contains the original line "below the line". The other side is "above the line"
             *   
             *   If the point is above the line containing the start:
             *     The shortest vector is from the start to the point
             *   
             *   If the point is above the line containing the end:
             *     The shortest vector is from the end to the point
             *     
             *   Otherwise
             *     The shortest vector is from the line to the point
             * 
             * If this is not true for ANY of the lines, the polygon does not contain the point.
             */

            var last = Math2.Rotate(poly.Vertices[poly.Vertices.Length - 1], poly.Center, rot) + pos;
            for (var i = 0; i < poly.Vertices.Length; i++)
            {
                var curr = Math2.Rotate(poly.Vertices[i], poly.Center, rot) + pos;
                var axis = curr - last;
                Vector2 norm;
                if (poly.Clockwise)
                    norm = new Vector2(-axis.Y, axis.X);
                else
                    norm = new Vector2(axis.Y, -axis.X);
                norm = Vector2.Normalize(norm);
                axis = Vector2.Normalize(axis);

                var lineProjOnNorm = Vector2.Dot(norm, last);
                var ptProjOnNorm = Vector2.Dot(norm, pt);

                if (ptProjOnNorm > lineProjOnNorm)
                {
                    var ptProjOnAxis = Vector2.Dot(axis, pt);
                    var stProjOnAxis = Vector2.Dot(axis, last);

                    if (ptProjOnAxis < stProjOnAxis)
                    {
                        var res = pt - last;
                        return Tuple.Create(Vector2.Normalize(res), res.Length());
                    }

                    var enProjOnAxis = Vector2.Dot(axis, curr);

                    if (ptProjOnAxis > enProjOnAxis)
                    {
                        var res = pt - curr;
                        return Tuple.Create(Vector2.Normalize(res), res.Length());
                    }


                    var distOnNorm = ptProjOnNorm - lineProjOnNorm;
                    return Tuple.Create(norm, distOnNorm);
                }

                last = curr;
            }

            return null;
        }

        private static IEnumerable<Vector2> GetExtraMinDistanceVecsPolyPoly(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2)
        {
            foreach (var vert in poly1.Vertices)
            {
                foreach (var vert2 in poly2.Vertices)
                {
                    var roughAxis = ((vert2 + pos2) - (vert + pos1));
                    roughAxis.Normalize();
                    yield return Math2.MakeStandardNormal(roughAxis);
                }
            }
        }

        /// <summary>
        /// Calculates the shortest distance and direction to go from poly1 at pos1 to poly2 at pos2. Returns null
        /// if the polygons intersect.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Origin of first polygon</param>
        /// <param name="pos2">Origin of second polygon</param>
        /// <param name="rot1">Rotation of first polygon</param>
        /// <param name="rot2">Rotation of second polygon</param>
        public static Tuple<Vector2, float> MinDistance(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2, Rotation2 rot1, Rotation2 rot2)
        {
            if (rot1.Theta != 0 || rot2.Theta != 0)
            {
                throw new NotSupportedException("Finding the minimum distance between polygons requires calculating the rotated polygons. This operation is expensive and should be cached. " +
                                                "Create the rotated polygons with Polygon2#GetRotated and call this function with Rotation2.Zero for both rotations.");
            }

            var axises = poly1.Normals.Union(poly2.Normals).Union(GetExtraMinDistanceVecsPolyPoly(poly1, poly2, pos1, pos2));
            Vector2? bestAxis = null; // note this is the one with the longest distance
            float bestDist = 0;
            foreach (var norm in axises)
            {
                var proj1 = ProjectAlongAxis(poly1, pos1, rot1, norm);
                var proj2 = ProjectAlongAxis(poly2, pos2, rot2, norm);

                var dist = AxisAlignedLine2.MinDistance(proj1, proj2);
                if (dist.HasValue && (bestAxis == null || dist.Value > bestDist))
                {
                    bestDist = dist.Value;
                    if (proj2.Min < proj1.Min && dist > 0)
                        bestAxis = -norm;
                    else
                        bestAxis = norm;
                }
            }

            if (!bestAxis.HasValue)
                return null; // they intersect

            return Tuple.Create(bestAxis.Value, bestDist);
        }

        /// <summary>
        /// Returns a polygon that is created by rotated the original polygon
        /// about its center by the specified amount. Returns the original polygon if
        /// rot.Theta == 0.
        /// </summary>
        /// <returns>The rotated polygon.</returns>
        /// <param name="original">Original.</param>
        /// <param name="rot">Rot.</param>
        public static Polygon2 GetRotated(Polygon2 original, Rotation2 rot)
        {
            if (rot.Theta == 0)
                return original;

            var rotatedVerts = new Vector2[original.Vertices.Length];
            for (var i = 0; i < original.Vertices.Length; i++)
            {
                rotatedVerts[i] = Math2.Rotate(original.Vertices[i], original.Center, rot);
            }

            return new Polygon2(rotatedVerts);
        }


        /// <summary>
        /// Creates the ray trace polygons from the given polygon moving from start to end. The returned set of polygons
        /// may not be the smallest possible set of polygons which perform this job. 
        /// 
        /// In order to determine if polygon A intersects polygon B during a move from position S to E, you can check if
        /// B intersects any of the polygons in CreateRaytraceAblesFromPolygon(A, E - S) when they are placed at S.
        /// </summary>
        /// <example>
        /// <code>
        /// Polygon2 a = ShapeUtils.CreateCircle(10, 0, 0, 5);
        /// Polygon2 b = ShapeUtils.CreateCircle(15, 0, 0, 7);
        /// 
        /// Vector2 from = new Vector2(3, 3);
        /// Vector2 to = new Vector2(15, 3);
        /// Vector2 bloc = new Vector2(6, 3);
        /// 
        /// List<Polygon2> traces = Polygon2.CreateRaytraceAbles(a, to - from);
        /// foreach (var trace in traces)
        /// {
        ///     if (Polygon2.Intersects(trace, b, from, bloc, true))
        ///     {
        ///         Console.WriteLine("Intersects!");
        ///         break;
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="poly">The polygon that you want to move</param>
        /// <param name="offset">The direction and magnitude that the polygon moves</param>
        /// <returns>A set of polygons which completely contain the area that the polygon will intersect during a move
        /// from the origin to offset.</returns>
        public static List<Polygon2> CreateRaytraceAbles(Polygon2 poly, Vector2 offset)
        {
            var ourLinesAsRects = new List<Polygon2>();
            if (Math2.Approximately(offset, Vector2.Zero))
            {
                ourLinesAsRects.Add(poly);
                return ourLinesAsRects;
            }

            for (int lineIndex = 0, nLines = poly.Lines.Length; lineIndex < nLines; lineIndex++)
            {
                var line = poly.Lines[lineIndex];
                if (!Math2.IsOnLine(line.Start, line.End, line.Start + offset))
                {
                    ourLinesAsRects.Add(new Polygon2(new Vector2[]
                    {
                    line.Start,
                    line.End,
                    line.End + offset,
                    line.Start + offset
                    }));
                }
            }

            return ourLinesAsRects;
        }

        #region NoRotation
        /// <summary>
        /// Determines if the specified polygons intersect when at the specified positions and not rotated.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Origin of first polygon</param>
        /// <param name="pos2">Origin of second polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly1 at pos1 not rotated and poly2 at pos2 not rotated intersect</returns>
        public static bool Intersects(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(poly1, poly2, pos1, pos2, Rotation2.Zero, Rotation2.Zero, strict);
        }

        /// <summary>
        /// Determines if the first polygon at position 1 intersects the second polygon at position 2, where
        /// neither polygon is rotated.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Origin of first polygon</param>
        /// <param name="pos2">Origin of second polygon</param>
        /// <returns>If poly1 at pos1 not rotated intersects poly2 at pos2 not rotated</returns>
        public static Tuple<Vector2, float> IntersectMTV(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMTV(poly1, poly2, pos1, pos2, Rotation2.Zero, Rotation2.Zero);
        }

        /// <summary>
        /// Determines the shortest way for the specified polygon at the specified position with
        /// no rotation to get to the specified point, if point is not (non-strictly) intersected
        /// the polygon when it's at the specified position with no rotation.
        /// </summary>
        /// <param name="poly">Polygon</param>
        /// <param name="pos">Position of the polygon</param>
        /// <param name="pt">Point to check</param>
        /// <returns>axis to go in, distance to go if pos is not in poly, otherwise null</returns>
        public static Tuple<Vector2, float> MinDistance(Polygon2 poly, Vector2 pos, Vector2 pt)
        {
            return MinDistance(poly, pos, Rotation2.Zero, pt);
        }

        /// <summary>
        /// Determines the shortest way for the first polygon at position 1 to touch the second polygon at
        /// position 2, assuming the polygons do not intersect (not strictly) and are not rotated.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Position of first polygon</param>
        /// <param name="pos2">Position of second polygon</param>
        /// <returns>axis to go in, distance to go if poly1 does not intersect poly2, otherwise null</returns>
        public static Tuple<Vector2, float> MinDistance(Polygon2 poly1, Polygon2 poly2, Vector2 pos1, Vector2 pos2)
        {
            return MinDistance(poly1, poly2, pos1, pos2, Rotation2.Zero, Rotation2.Zero);
        }
        #endregion
    }

    /// <summary>
    /// Describes a rectangle. Meant to be reused.
    /// </summary>
    public class Rect2 : Shape2
    {
        /// <summary>
        /// The vertices of this rectangle as a clockwise array.
        /// </summary>
        public readonly Vector2[] Vertices;

        /// <summary>
        /// The corner with the smallest x and y coordinates on this
        /// rectangle.
        /// </summary>
        public Vector2 Min => Vertices[0];

        /// <summary>
        /// The corner with the largest x and y coordinates on this
        /// rectangle
        /// </summary>
        public Vector2 Max => Vertices[2];

        /// <summary>
        /// The corner with the largest x and smallest y coordinates on
        /// this rectangle
        /// </summary>
        public Vector2 UpperRight => Vertices[1];

        /// <summary>
        /// The corner with the smallest x and largest y coordinates on this
        /// rectangle
        /// </summary>
        public Vector2 LowerLeft => Vertices[3];

        /// <summary>
        /// The center of this rectangle
        /// </summary>
        public readonly Vector2 Center;

        /// <summary>
        /// The width of this rectangle
        /// </summary>
        public readonly float Width;

        /// <summary>
        /// The height of this rectangle
        /// </summary>
        public readonly float Height;

        /// <summary>
        /// Creates a bounding box with the specified upper-left and bottom-right.
        /// Will autocorrect if min.X > max.X or min.Y > max.Y
        /// </summary>
        /// <param name="min">Min x, min y</param>
        /// <param name="max">Max x, max y</param>
        /// <exception cref="ArgumentException">If min and max do not make a box</exception>
        public Rect2(Vector2 min, Vector2 max)
        {
            float area = (max.X - min.X) * (max.Y - min.Y);
            if (area > -Math2.DEFAULT_EPSILON && area < Math2.DEFAULT_EPSILON)
                throw new ArgumentException($"min={min}, max={max} - that's a line or a point, not a box (area below epsilon {Math2.DEFAULT_EPSILON} (got {area}))");

            float tmpX1 = min.X, tmpX2 = max.X;
            float tmpY1 = min.Y, tmpY2 = max.Y;

            min.X = Math.Min(tmpX1, tmpX2);
            min.Y = Math.Min(tmpY1, tmpY2);
            max.X = Math.Max(tmpX1, tmpX2);
            max.Y = Math.Max(tmpY1, tmpY2);

            Vertices = new Vector2[]
            {
                min, new Vector2(max.X, min.Y), max, new Vector2(min.X, max.Y)
            };

            Center = new Vector2((Min.X + Max.X) / 2, (Min.Y + Max.Y) / 2);

            Width = Max.X - Min.X;
            Height = Max.Y - Min.Y;
        }

        /// <summary>
        /// Creates a bounding box from the specified points. Will correct if minX > maxX or minY > maxY.
        /// </summary>
        /// <param name="minX">Min or max x (different from maxX)</param>
        /// <param name="minY">Min or max y (different from maxY)</param>
        /// <param name="maxX">Min or max x (different from minX)</param>
        /// <param name="maxY">Min or max y (different from minY)</param>
        public Rect2(float minX, float minY, float maxX, float maxY) : this(new Vector2(minX, minY), new Vector2(maxX, maxY))
        {
        }

        /// <summary>
        /// Determines if box1 with origin pos1 intersects box2 with origin pos2.
        /// </summary>
        /// <param name="box1">Box 1</param>
        /// <param name="box2">Box 2</param>
        /// <param name="pos1">Origin of box 1</param>
        /// <param name="pos2">Origin of box 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If box1 intersects box2 when box1 is at pos1 and box2 is at pos2</returns>
        public static bool Intersects(Rect2 box1, Rect2 box2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return AxisAlignedLine2.Intersects(box1.Min.X + pos1.X, box1.Max.X + pos1.X, box2.Min.X + pos2.X, box2.Max.X + pos2.X, strict, false)
                && AxisAlignedLine2.Intersects(box1.Min.Y + pos1.Y, box1.Max.Y + pos1.Y, box2.Min.Y + pos2.Y, box2.Max.Y + pos2.Y, strict, false);
        }

        /// <summary>
        /// Determines if the box when at pos contains point.
        /// </summary>
        /// <param name="box">The box</param>
        /// <param name="pos">Origin of box</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">true if the edges do not count</param>
        /// <returns>If the box at pos contains point</returns>
        public static bool Contains(Rect2 box, Vector2 pos, Vector2 point, bool strict)
        {
            return AxisAlignedLine2.Contains(box.Min.X + pos.X, box.Max.X + pos.X, point.X, strict, false)
                && AxisAlignedLine2.Contains(box.Min.Y + pos.Y, box.Max.Y + pos.Y, point.Y, strict, false);
        }

        /// <summary>
        /// Determines if innerBox is contained entirely in outerBox
        /// </summary>
        /// <param name="outerBox">the (bigger) box that you want to check contains the inner box</param>
        /// <param name="innerBox">the (smaller) box that you want to check is contained in the outer box</param>
        /// <param name="posOuter">where the outer box is located</param>
        /// <param name="posInner">where the inner box is located</param>
        /// <param name="strict">true to return false if innerBox touches an edge of outerBox, false otherwise</param>
        /// <returns>true if innerBox is contained in outerBox, false otherwise</returns>
        public static bool Contains(Rect2 outerBox, Rect2 innerBox, Vector2 posOuter, Vector2 posInner, bool strict)
        {
            return Contains(outerBox, posOuter, innerBox.Min + posInner, strict) && Contains(outerBox, posOuter, innerBox.Max + posInner, strict);
        }

        /// <summary>
        /// Deterimines in the box contains the specified polygon
        /// </summary>
        /// <param name="box">The box</param>
        /// <param name="poly">The polygon</param>
        /// <param name="boxPos">Where the box is located</param>
        /// <param name="polyPos">Where the polygon is located</param>
        /// <param name="strict">true if we return false if the any part of the polygon is on the edge, false otherwise</param>
        /// <returns>true if the poly is contained in box, false otherwise</returns>
        public static bool Contains(Rect2 box, Polygon2 poly, Vector2 boxPos, Vector2 polyPos, bool strict)
        {
            return Contains(box, poly.AABB, boxPos, polyPos, strict);
        }

        /// <summary>
        /// Projects the rectangle at pos along axis.
        /// </summary>
        /// <param name="rect">The rectangle to project</param>
        /// <param name="pos">The origin of the rectangle</param>
        /// <param name="axis">The axis to project on</param>
        /// <returns>The projection of rect at pos along axis</returns>
        public static AxisAlignedLine2 ProjectAlongAxis(Rect2 rect, Vector2 pos, Vector2 axis)
        {
            //return ProjectAlongAxis(axis, pos, Rotation2.Zero, rect.Center, rect.Min, rect.UpperRight, rect.LowerLeft, rect.Max);
            return ProjectAlongAxis(axis, pos, rect.Vertices);
        }
    }

    /// <summary>
    /// Describes a rectangle that is describing the percentages to go
    /// of the true rectangle. Useful in some UI circumstances.
    /// </summary>
	public class RelativeRectangle2 : Rect2
    {
        /// <summary>
        /// Create a new relative rectangle
        /// </summary>
        /// <param name="min">vector of smallest x and y coordinates</param>
        /// <param name="max">vector of largest x and y coordinates</param>
        public RelativeRectangle2(Vector2 min, Vector2 max) : base(min, max)
        {
        }

        /// <summary>
        /// Create a new relative rectangle
        /// </summary>
        /// <param name="x">smallest x</param>
        /// <param name="y">smallest y</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        public RelativeRectangle2(float x, float y, float w, float h) : base(new Vector2(x, y), new Vector2(x + w, y + h))
        {
        }

        /// <summary>
        /// Multiply our min with original min and our max with original max and return
        /// as a rect
        /// </summary>
        /// <param name="original">the original</param>
        /// <returns>scaled rect</returns>
        public Rect2 ToRect(Rect2 original)
        {
            return new Rect2(original.Min * Min, original.Max * Max);
        }

#if !NOT_MONOGAME
        /// <summary>
        /// Multiply our min with original min and our max with original max and return
        /// as a rect
        /// </summary>
        /// <param name="original">the monogame original</param>
        /// <returns>the rect</returns>
        public Rect2 ToRect(Rectangle original)
        {
            return new Rect2(
                new Vector2(original.Left + original.Width * Min.X, original.Top + original.Height * Min.Y),
                    new Vector2(original.Left + original.Width * Max.X, original.Top + original.Height * Max.Y)
            );
        }
#endif
    }

    /// <summary>
    /// Describes a rotation about the z axis, with sin and cos of theta
    /// cached.
    /// </summary>
    public struct Rotation2
    {
        /// <summary>
        /// Rotation Theta=0
        /// </summary>
        public static readonly Rotation2 Zero = new Rotation2(0, 1, 0);

        /// <summary>
        /// Theta in radians.
        /// </summary>
        public readonly float Theta;

        /// <summary>
        /// Math.Cos(Theta)
        /// </summary>
        public readonly float CosTheta;

        /// <summary>
        /// Math.Sin(Theta)
        /// </summary>
        public readonly float SinTheta;

        /// <summary>
        /// Create a new rotation by specifying the theta, its cosin, and its sin.
        /// 
        /// Theta will be normalized to 0 &lt;= theta &lt;= 2pi
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="cosTheta"></param>
        /// <param name="sinTheta"></param>
        public Rotation2(float theta, float cosTheta, float sinTheta)
        {
            if (float.IsInfinity(theta) || float.IsNaN(theta))
                throw new ArgumentException($"Invalid theta: {theta}", nameof(theta));
            if (theta < 0)
            {
                int numToAdd = (int)Math.Ceiling((-theta) / (Math.PI * 2));
                theta += (float)Math.PI * 2 * numToAdd;
            }
            else if (theta >= Math.PI * 2)
            {
                int numToReduce = (int)Math.Floor(theta / (Math.PI * 2));
                theta -= (float)Math.PI * 2 * numToReduce;
            }

            Theta = theta;
            CosTheta = cosTheta;
            SinTheta = sinTheta;
        }

        /// <summary>
        /// Create a new rotation at the specified theta, calculating the cos and sin.
        /// 
        /// Theta will be normalized to 0 &lt;= theta &lt;= 2pi
        /// </summary>
        /// <param name="theta"></param>
        public Rotation2(float theta) : this(theta, (float)Math.Cos(theta), (float)Math.Sin(theta))
        {
        }

        /// <summary>
        /// Determine if the two rotations have the same theta
        /// </summary>
        /// <param name="r1">First rotation</param>
        /// <param name="r2">Second rotation</param>
        /// <returns>if r1 and r2 are the same logical rotation</returns>
        public static bool operator ==(Rotation2 r1, Rotation2 r2)
        {
            if (ReferenceEquals(r1, null) || ReferenceEquals(r2, null))
                return ReferenceEquals(r1, r2);

            return r1.Theta == r2.Theta;
        }

        /// <summary>
        /// Determine if the two rotations are not the same
        /// </summary>
        /// <param name="r1">first rotation</param>
        /// <param name="r2">second rotation</param>
        /// <returns>if r1 and r2 are not the same logical rotation</returns>
        public static bool operator !=(Rotation2 r1, Rotation2 r2)
        {
            if (ReferenceEquals(r1, null) || ReferenceEquals(r2, null))
                return ReferenceEquals(r1, r2);

            return r1.Theta != r2.Theta;
        }

        /// <summary>
        /// Determine if obj is a rotation that is logically equal to this one
        /// </summary>
        /// <param name="obj">the object</param>
        /// <returns>if it is logically equal</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Rotation2))
                return false;

            return this == ((Rotation2)obj);
        }

        /// <summary>
        /// The hashcode of this rotation based on just Theta
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Theta.GetHashCode();
        }

        /// <summary>
        /// Create a human-readable representation of this rotation
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return $"{Theta} rads";
        }
    }

    /// <summary>
    /// Parent class for shapes - contains functions for comparing different shapes.
    /// </summary>
    public class Shape2
    {
        /// <summary>
        /// Determines if polygon at position 1 intersects the rectangle at position 2. Polygon may
        /// be rotated, but the rectangle cannot (use a polygon if you want to rotate it).
        /// </summary>
        /// <param name="poly">Polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of the polygon.</param>
        /// <param name="strict">If overlapping is required for intersection</param>
        /// <returns>if poly at pos1 intersects rect at pos2</returns>
        public static bool Intersects(Polygon2 poly, Rect2 rect, Vector2 pos1, Vector2 pos2, Rotation2 rot1, bool strict)
        {
            bool checkedX = false, checkedY = false;
            for (int i = 0; i < poly.Normals.Count; i++)
            {
                var norm = Math2.Rotate(poly.Normals[i], Vector2.Zero, rot1);
                if (!IntersectsAlongAxis(poly, rect, pos1, pos2, rot1, strict, norm))
                    return false;

                if (norm.X == 0)
                    checkedY = true;
                if (norm.Y == 0)
                    checkedX = true;
            }

            if (!checkedX && !IntersectsAlongAxis(poly, rect, pos1, pos2, rot1, strict, Vector2.UnitX))
                return false;
            if (!checkedY && !IntersectsAlongAxis(poly, rect, pos1, pos2, rot1, strict, Vector2.UnitY))
                return false;

            return true;
        }

        /// <summary>
        /// Determines the vector, if any, to move poly at pos1 rotated rot1 to prevent intersection of rect
        /// at pos2.
        /// </summary>
        /// <param name="poly">Polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of the polygon.</param>
        /// <returns>The vector to move pos1 by or null</returns>
        public static Tuple<Vector2, float> IntersectMTV(Polygon2 poly, Rect2 rect, Vector2 pos1, Vector2 pos2, Rotation2 rot1)
        {
            bool checkedX = false, checkedY = false;

            Vector2 bestAxis = Vector2.Zero;
            float bestMagn = float.MaxValue;

            for (int i = 0; i < poly.Normals.Count; i++)
            {
                var norm = Math2.Rotate(poly.Normals[i], Vector2.Zero, rot1);
                var mtv = IntersectMTVAlongAxis(poly, rect, pos1, pos2, rot1, norm);
                if (!mtv.HasValue)
                    return null;

                if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = norm;
                    bestMagn = mtv.Value;
                }

                if (norm.X == 0)
                    checkedY = true;
                if (norm.Y == 0)
                    checkedX = true;
            }

            if (!checkedX)
            {
                var mtv = IntersectMTVAlongAxis(poly, rect, pos1, pos2, rot1, Vector2.UnitX);
                if (!mtv.HasValue)
                    return null;

                if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = Vector2.UnitX;
                    bestMagn = mtv.Value;
                }
            }

            if (!checkedY)
            {
                var mtv = IntersectMTVAlongAxis(poly, rect, pos1, pos2, rot1, Vector2.UnitY);
                if (!mtv.HasValue)
                    return null;

                if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = Vector2.UnitY;
                    bestMagn = mtv.Value;
                }
            }

            return Tuple.Create(bestAxis, bestMagn);
        }

        /// <summary>
        /// Determines the vector to move pos1 to get rect not to intersect poly at pos2 rotated
        /// by rot2 radians.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of </param>
        /// <param name="rot2">Rotation of the polygon</param>
        /// <returns>Offset of pos1 to get rect not to intersect poly</returns>
        public static Tuple<Vector2, float> IntersectMTV(Rect2 rect, Polygon2 poly, Vector2 pos1, Vector2 pos2, Rotation2 rot2)
        {
            var res = IntersectMTV(poly, rect, pos2, pos1, rot2);
            return res != null ? Tuple.Create(-res.Item1, res.Item2) : res;
        }

        /// <summary>
        /// Determines if the rectangle at pos1 intersects the polygon at pos2.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">Origin of retangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="rot2">Rotation of the polygon.</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If rect at pos1 intersects poly at pos2</returns>
        public static bool Intersects(Rect2 rect, Polygon2 poly, Vector2 pos1, Vector2 pos2, Rotation2 rot2, bool strict)
        {
            return Intersects(poly, rect, pos2, pos1, rot2, strict);
        }


        /// <summary>
        /// Determines if the specified polygon and rectangle where poly is at pos1 and rect is at pos2 intersect
        /// along the specified axis.
        /// </summary>
        /// <param name="poly">polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of the polygon.</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>If poly at pos1 intersects rect at pos2 along axis</returns>
        public static bool IntersectsAlongAxis(Polygon2 poly, Rect2 rect, Vector2 pos1, Vector2 pos2, Rotation2 rot1, bool strict, Vector2 axis)
        {
            var proj1 = Polygon2.ProjectAlongAxis(poly, pos1, rot1, axis);
            var proj2 = Rect2.ProjectAlongAxis(rect, pos2, axis);

            return AxisAlignedLine2.Intersects(proj1, proj2, strict);
        }

        /// <summary>
        /// Determines if the specified rectangle and polygon where rect is at pos1 and poly is at pos2 intersect 
        /// along the specified axis.
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="poly">Polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="rot2">Rotation of polygon</param>
        /// <param name="strict"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool IntersectsAlongAxis(Rect2 rect, Polygon2 poly, Vector2 pos1, Vector2 pos2, Rotation2 rot2, bool strict, Vector2 axis)
        {
            return IntersectsAlongAxis(poly, rect, pos2, pos1, rot2, strict, axis);
        }

        /// <summary>
        /// Determines the mtv along axis to move poly at pos1 to prevent intersection with rect at pos2.
        /// </summary>
        /// <param name="poly">polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of polygon in radians</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>Number if poly intersects rect along axis, null otherwise</returns>
        public static float? IntersectMTVAlongAxis(Polygon2 poly, Rect2 rect, Vector2 pos1, Vector2 pos2, Rotation2 rot1, Vector2 axis)
        {
            var proj1 = Polygon2.ProjectAlongAxis(poly, pos1, rot1, axis);
            var proj2 = Rect2.ProjectAlongAxis(rect, pos2, axis);

            return AxisAlignedLine2.IntersectMTV(proj1, proj2);
        }

        /// <summary>
        /// Determines the mtv along axis to move rect at pos1 to prevent intersection with poly at pos2
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="poly">polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="rot2">Rotation of the polygon in radians</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>Number if rect intersects poly along axis, null otherwise</returns>
        public static float? IntersectMTVAlongAxis(Rect2 rect, Polygon2 poly, Vector2 pos1, Vector2 pos2, Rotation2 rot2, Vector2 axis)
        {
            var proj1 = Rect2.ProjectAlongAxis(rect, pos1, axis);
            var proj2 = Polygon2.ProjectAlongAxis(poly, pos2, rot2, axis);

            return AxisAlignedLine2.IntersectMTV(proj1, proj2);
        }

        /// <summary>
        /// Determines if the specified polygon at the specified position and rotation
        /// intersects the specified circle at it's respective position.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="circle">The circle</param>
        /// <param name="pos1">The origin for the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="rot1">The rotation of the polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly at pos1 with rotation rot1 intersects the circle at pos2</returns>
        public static bool Intersects(Polygon2 poly, Circle2 circle, Vector2 pos1, Vector2 pos2, Rotation2 rot1, bool strict)
        {
            // look at pictures of https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection if you don't
            // believe this is true
            return poly.Lines.Any((l) => CircleIntersectsLine(circle, l, pos2, pos1, rot1, poly.Center, strict)) || Polygon2.Contains(poly, pos1, rot1, new Vector2(pos2.X + circle.Radius, pos2.Y + circle.Radius), strict);
        }

        /// <summary>
        /// Determines the minimum translation that must be applied the specified polygon (at the given position
        /// and rotation) to prevent intersection with the circle (at its given rotation). If the two are not overlapping,
        /// returns null.
        /// 
        /// Returns a tuple of the axis to move the polygon in (unit vector) and the distance to move the polygon.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="circle">The circle</param>
        /// <param name="pos1">The origin of the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="rot1">The rotation of the polygon</param>
        /// <returns></returns>
        public static Tuple<Vector2, float> IntersectMTV(Polygon2 poly, Circle2 circle, Vector2 pos1, Vector2 pos2, Rotation2 rot1)
        {
            // We have two situations, either the circle is not strictly intersecting the polygon, or
            // there exists at least one shortest line that you could push the polygon to prevent 
            // intersection with the circle.

            // That line will either go from a vertix of the polygon to a point on the edge of the circle,
            // or it will go from a point on a line of the polygon to the edge of the circle.

            // If the line comes from a vertix of the polygon, the MTV will be along the line produced
            // by going from the center of the circle to the vertix, and the distance can be found by
            // projecting the cirle on that axis and the polygon on that axis and doing 1D overlap.

            // If the line comes from a point on the edge of the polygon, the MTV will be along the
            // normal of that line, and the distance can be found by projecting the circle on that axis
            // and the polygon on that axis and doing 1D overlap.

            // As with all SAT, if we find any axis that the circle and polygon do not overlap, we've
            // proven they do not intersect.

            // The worst case performance is related to 2x the number of vertices of the polygon, the same speed
            // as for 2 polygons of equal number of vertices.

            HashSet<Vector2> checkedAxis = new HashSet<Vector2>();

            Vector2 bestAxis = Vector2.Zero;
            float shortestOverlap = float.MaxValue;

            Func<Vector2, bool> checkAxis = (axis) =>
            {
                var standard = Math2.MakeStandardNormal(axis);
                if (!checkedAxis.Contains(standard))
                {
                    checkedAxis.Add(standard);
                    var polyProj = Polygon2.ProjectAlongAxis(poly, pos1, rot1, axis);
                    var circleProj = Circle2.ProjectAlongAxis(circle, pos2, axis);

                    var mtv = AxisAlignedLine2.IntersectMTV(polyProj, circleProj);
                    if (!mtv.HasValue)
                        return false;

                    if (Math.Abs(mtv.Value) < Math.Abs(shortestOverlap))
                    {
                        bestAxis = axis;
                        shortestOverlap = mtv.Value;
                    }
                }
                return true;
            };

            var circleCenter = new Vector2(pos2.X + circle.Radius, pos2.Y + circle.Radius);
            int last = poly.Vertices.Length - 1;
            var lastVec = Math2.Rotate(poly.Vertices[last], poly.Center, rot1) + pos1;
            for (int curr = 0; curr < poly.Vertices.Length; curr++)
            {
                var currVec = Math2.Rotate(poly.Vertices[curr], poly.Center, rot1) + pos1;

                // Test along circle center -> vector
                if (!checkAxis(Vector2.Normalize(currVec - circleCenter)))
                    return null;

                // Test along line normal
                if (!checkAxis(Vector2.Normalize(Math2.Perpendicular(currVec - lastVec))))
                    return null;

                last = curr;
                lastVec = currVec;
            }

            return Tuple.Create(bestAxis, shortestOverlap);
        }

        /// <summary>
        /// Determines if the specified circle, at the given position, intersects the specified polygon,
        /// at the given position and rotation.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <param name="rot2">The rotation of the polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle at pos1 intersects poly at pos2 with rotation rot2</returns>
        public static bool Intersects(Circle2 circle, Polygon2 poly, Vector2 pos1, Vector2 pos2, Rotation2 rot2, bool strict)
        {
            return Intersects(poly, circle, pos2, pos1, rot2, strict);
        }

        /// <summary>
        /// Determines the minimum translation vector that must be applied to the circle at the given position to
        /// prevent overlap with the polygon at the given position and rotation. If the circle and the polygon do
        /// not overlap, returns null. Otherwise, returns a tuple of the unit axis to move the circle in, and the
        /// distance to move the circle.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <param name="rot2">The rotation of the polygon</param>
        /// <returns>The mtv to move the circle at pos1 to prevent overlap with the poly at pos2 with rotation rot2</returns>
        public static Tuple<Vector2, float> IntersectMTV(Circle2 circle, Polygon2 poly, Vector2 pos1, Vector2 pos2, Rotation2 rot2)
        {
            var res = IntersectMTV(poly, circle, pos2, pos1, rot2);
            if (res != null)
                return Tuple.Create(-res.Item1, res.Item2);
            return null;
        }

        /// <summary>
        /// Determines if the specified circle an rectangle intersect at their given positions.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the rectangle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle at pos1 intersects rect at pos2</returns>
        public static bool Intersects(Circle2 circle, Rect2 rect, Vector2 pos1, Vector2 pos2, bool strict)
        {
            var circleCenter = new Vector2(pos1.X + circle.Radius, pos1.Y + circle.Radius);
            return CircleIntersectsHorizontalLine(circle, new Line2(rect.Min + pos2, rect.UpperRight + pos2), circleCenter, strict)
                || CircleIntersectsHorizontalLine(circle, new Line2(rect.LowerLeft + pos2, rect.Max + pos2), circleCenter, strict)
                || CircleIntersectsVerticalLine(circle, new Line2(rect.Min + pos2, rect.LowerLeft + pos2), circleCenter, strict)
                || CircleIntersectsVerticalLine(circle, new Line2(rect.UpperRight + pos2, rect.Max + pos2), circleCenter, strict)
                || Rect2.Contains(rect, pos2, new Vector2(pos1.X + circle.Radius, pos1.Y + circle.Radius), strict);
        }

        /// <summary>
        /// Determines if the specified rectangle and circle intersect at their given positions.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="circle">The circle</param>
        /// <param name="pos1">The origin of the rectangle</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns></returns>
        public static bool Intersects(Rect2 rect, Circle2 circle, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(circle, rect, pos2, pos1, strict);
        }

        /// <summary>
        /// Determines the minimum translation vector to be applied to the circle to 
        /// prevent overlap with the rectangle, when they are at their given positions.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The rectangles origin</param>
        /// <returns>MTV for circle at pos1 to prevent overlap with rect at pos2</returns>
        public static Tuple<Vector2, float> IntersectMTV(Circle2 circle, Rect2 rect, Vector2 pos1, Vector2 pos2)
        {
            // Same as polygon rect, just converted to rects points
            HashSet<Vector2> checkedAxis = new HashSet<Vector2>();

            Vector2 bestAxis = Vector2.Zero;
            float shortestOverlap = float.MaxValue;

            Func<Vector2, bool> checkAxis = (axis) =>
            {
                var standard = Math2.MakeStandardNormal(axis);
                if (!checkedAxis.Contains(standard))
                {
                    checkedAxis.Add(standard);
                    var circleProj = Circle2.ProjectAlongAxis(circle, pos1, axis);
                    var rectProj = Rect2.ProjectAlongAxis(rect, pos2, axis);

                    var mtv = AxisAlignedLine2.IntersectMTV(circleProj, rectProj);
                    if (!mtv.HasValue)
                        return false;

                    if (Math.Abs(mtv.Value) < Math.Abs(shortestOverlap))
                    {
                        bestAxis = axis;
                        shortestOverlap = mtv.Value;
                    }
                }
                return true;
            };

            var circleCenter = new Vector2(pos1.X + circle.Radius, pos1.Y + circle.Radius);
            int last = 4;
            var lastVec = rect.UpperRight + pos2;
            for (int curr = 0; curr < 4; curr++)
            {
                Vector2 currVec = Vector2.Zero;
                switch (curr)
                {
                    case 0:
                        currVec = rect.Min + pos2;
                        break;
                    case 1:
                        currVec = rect.LowerLeft + pos2;
                        break;
                    case 2:
                        currVec = rect.Max + pos2;
                        break;
                    case 3:
                        currVec = rect.UpperRight + pos2;
                        break;
                }

                // Test along circle center -> vector
                if (!checkAxis(Vector2.Normalize(currVec - circleCenter)))
                    return null;

                // Test along line normal
                if (!checkAxis(Vector2.Normalize(Math2.Perpendicular(currVec - lastVec))))
                    return null;

                last = curr;
                lastVec = currVec;
            }

            return Tuple.Create(bestAxis, shortestOverlap);
        }

        /// <summary>
        /// Determines the minimum translation vector to be applied to the rectangle to
        /// prevent overlap with the circle, when they are at their given positions.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="circle">The circle</param>
        /// <param name="pos1">The origin of the rectangle</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <returns>MTV for rect at pos1 to prevent overlap with circle at pos2</returns>
        public static Tuple<Vector2, float> IntersectMTV(Rect2 rect, Circle2 circle, Vector2 pos1, Vector2 pos2)
        {
            var res = IntersectMTV(circle, rect, pos2, pos1);
            if (res != null)
                return Tuple.Create(-res.Item1, res.Item2);
            return null;
        }

        /// <summary>
        /// Projects the polygon from the given points with origin pos along the specified axis.
        /// </summary>
        /// <param name="axis">Axis to project onto</param>
        /// <param name="pos">Origin of polygon</param>
        /// <param name="rot">Rotation of the polygon in radians</param>
        /// <param name="center">Center of the polygon</param>
        /// <param name="points">Points of polygon</param>
        /// <returns>Projection of polygon of points at pos along axis</returns>
        protected static AxisAlignedLine2 ProjectAlongAxis(Vector2 axis, Vector2 pos, Rotation2 rot, Vector2 center, params Vector2[] points)
        {
            if (rot == Rotation2.Zero)
                return ProjectAlongAxis(axis, pos, points);

            float min = 0;
            float max = 0;

            for (int i = 0; i < points.Length; i++)
            {
                var polyPt = Math2.Rotate(points[i], center, rot);
                var tmp = Math2.Dot(polyPt.X + pos.X, polyPt.Y + pos.Y, axis.X, axis.Y);

                if (i == 0)
                {
                    min = max = tmp;
                }
                else
                {
                    min = Math.Min(min, tmp);
                    max = Math.Max(max, tmp);
                }
            }

            return new AxisAlignedLine2(axis, min, max);
        }

        /// <summary>
        /// A faster variant of ProjectAlongAxis that assumes no rotation.
        /// </summary>
        /// <param name="axis">The axis that the points are being projected along</param>
        /// <param name="pos">The offset for the points</param>
        /// <param name="points">The points in the convex polygon</param>
        /// <returns>The projectino of the polygon comprised of points at pos along axis</returns>
        protected static AxisAlignedLine2 ProjectAlongAxis(Vector2 axis, Vector2 pos, Vector2[] points)
        {
			int len = points.Length;
			if (len == 0)
				return new AxisAlignedLine2(axis, 0, 0);

			float min;
			float max;

			min = axis.X * (points[0].X + pos.X) + axis.Y * (points[0].Y + pos.Y);
			max = min;
			for (int i = 1; i < len; i++)
			{
				float tmp = axis.X * (points[i].X + pos.X) + axis.Y * (points[i].Y + pos.Y);

				if (tmp < min)
					min = tmp;
				if (tmp > max)
					max = tmp;
			}

			return new AxisAlignedLine2(axis, min, max);
		}

        /// <summary>
        /// Determines if the circle whose bounding boxs top left is at the first postion intersects the line
        /// at the second position who is rotated the specified amount about the specified point.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="line">The line</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the line</param>
        /// <param name="rot2">What rotation the line is under</param>
        /// <param name="about2">What the line is rotated about</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If the circle at pos1 intersects the line at pos2 rotated rot2 about about2</returns>
        protected static bool CircleIntersectsLine(Circle2 circle, Line2 line, Vector2 pos1, Vector2 pos2, Rotation2 rot2, Vector2 about2, bool strict)
        {
            // Make more math friendly
            var actualLine = new Line2(Math2.Rotate(line.Start, about2, rot2) + pos2, Math2.Rotate(line.End, about2, rot2) + pos2);
            var circleCenter = new Vector2(pos1.X + circle.Radius, pos1.Y + circle.Radius);

            // Check weird situations
            if (actualLine.Horizontal)
                return CircleIntersectsHorizontalLine(circle, actualLine, circleCenter, strict);
            if (actualLine.Vertical)
                return CircleIntersectsVerticalLine(circle, actualLine, circleCenter, strict);

            // Goal:
            // 1. Find closest distance, closestDistance, on the line to the circle (assuming the line was infinite)
            //   1a Determine if closestPoint is intersects the circle according to strict
            //    - If it does not, we've shown there is no intersection.
            // 2. Find closest point, closestPoint, on the line to the circle (assuming the line was infinite)
            // 3. Determine if closestPoint is on the line (including edges)
            //   - If it is, we've shown there is intersection.
            // 4. Determine which edge, edgeClosest, is closest to closestPoint
            // 5. Determine if edgeClosest intersects the circle according to strict
            //   - If it does, we've shown there is intersection
            //   - If it does not, we've shown there is no intersection

            // Step 1
            // We're trying to find closestDistance

            // Recall that the shortest line from a line to a point will be normal to the line
            // Thus, the shortest distance from a line to a point can be found by projecting 
            // the line onto it's own normal vector and projecting the point onto the lines 
            // normal vector; the distance between those points is the shortest distance from
            // the two points. 

            // The projection of a line onto its normal will be a single point, and will be same
            // for any point on that line. So we pick a point that's convienent (the start or end).
            var lineProjectedOntoItsNormal = Vector2.Dot(actualLine.Start, actualLine.Normal);
            var centerOfCircleProjectedOntoNormalOfLine = Vector2.Dot(circleCenter, actualLine.Normal);
            var closestDistance = Math.Abs(centerOfCircleProjectedOntoNormalOfLine - lineProjectedOntoItsNormal);

            // Step 1a
            if (strict)
            {
                if (closestDistance >= circle.Radius)
                    return false;
            }
            else
            {
                if (closestDistance > circle.Radius)
                    return false;
            }

            // Step 2
            // We're trying to find closestPoint

            // We can just walk the vector from the center to the closest point, which we know is on 
            // the normal axis and the distance closestDistance. However it's helpful to get the signed
            // version End - Start to walk.
            var signedDistanceCircleCenterToLine = lineProjectedOntoItsNormal - centerOfCircleProjectedOntoNormalOfLine;
            var closestPoint = circleCenter - actualLine.Normal * signedDistanceCircleCenterToLine;

            // Step 3
            // Determine if closestPoint is on the line (including edges)

            // We're going to accomplish this by projecting the line onto it's own axis and the closestPoint onto the lines
            // axis. Then we have a 1D comparison.
            var lineStartProjectedOntoLineAxis = Vector2.Dot(actualLine.Start, actualLine.Axis);
            var lineEndProjectedOntoLineAxis = Vector2.Dot(actualLine.End, actualLine.Axis);

            var closestPointProjectedOntoLineAxis = Vector2.Dot(closestPoint, actualLine.Axis);

            if (AxisAlignedLine2.Contains(lineStartProjectedOntoLineAxis, lineEndProjectedOntoLineAxis, closestPointProjectedOntoLineAxis, false, true))
            {
                return true;
            }

            // Step 4
            // We're trying to find edgeClosest.
            //
            // We're going to reuse those projections from step 3.
            //
            // (for each "point" in the next paragraph I mean "point projected on the lines axis" but that's wordy)
            //
            // We know that the start is closest iff EITHER the start is less than the end and the 
            // closest point is less than the start, OR the start is greater than the end and 
            // closest point is greater than the end.

            var closestEdge = Vector2.Zero;
            if (lineStartProjectedOntoLineAxis < lineEndProjectedOntoLineAxis)
                closestEdge = (closestPointProjectedOntoLineAxis <= lineStartProjectedOntoLineAxis) ? actualLine.Start : actualLine.End;
            else
                closestEdge = (closestPointProjectedOntoLineAxis >= lineEndProjectedOntoLineAxis) ? actualLine.Start : actualLine.End;

            // Step 5 
            // Circle->Point intersection for closestEdge

            var distToCircleFromClosestEdgeSq = (circleCenter - closestEdge).LengthSquared();
            if (strict)
                return distToCircleFromClosestEdgeSq < (circle.Radius * circle.Radius);
            else
                return distToCircleFromClosestEdgeSq <= (circle.Radius * circle.Radius);

            // If you had trouble following, see the horizontal and vertical cases which are the same process but the projections
            // are simpler
        }

        /// <summary>
        /// Determines if the circle at the specified position intersects the line, 
        /// which is at its true position and rotation, when the line is assumed to be horizontal.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="line">The line</param>
        /// <param name="circleCenter">The center of the circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If the circle with center circleCenter intersects the horizontal line</returns>
        protected static bool CircleIntersectsHorizontalLine(Circle2 circle, Line2 line, Vector2 circleCenter, bool strict)
        {
            // This is exactly the same process as CircleIntersectsLine, except the projetions are easier
            var lineY = line.Start.Y;

            // Step 1 - Find closest distance
            var vecCircleCenterToLine1D = lineY - circleCenter.Y;
            var closestDistance = Math.Abs(vecCircleCenterToLine1D);

            // Step 1a
            if (strict)
            {
                if (closestDistance >= circle.Radius)
                    return false;
            }
            else
            {
                if (closestDistance > circle.Radius)
                    return false;
            }

            // Step 2 - Find closest point
            var closestPointX = circleCenter.X;

            // Step 3 - Is closest point on line
            if (AxisAlignedLine2.Contains(line.Start.X, line.End.X, closestPointX, false, true))
                return true;

            // Step 4 - Find edgeClosest
            float edgeClosestX;
            if (line.Start.X < line.End.X)
                edgeClosestX = (closestPointX <= line.Start.X) ? line.Start.X : line.End.X;
            else
                edgeClosestX = (closestPointX >= line.Start.X) ? line.Start.X : line.End.X;

            // Step 5 - Circle-point intersection on closest point
            var distClosestEdgeToCircleSq = new Vector2(circleCenter.X - edgeClosestX, circleCenter.Y - lineY).LengthSquared();

            if (strict)
                return distClosestEdgeToCircleSq < circle.Radius * circle.Radius;
            else
                return distClosestEdgeToCircleSq <= circle.Radius * circle.Radius;
        }

        /// <summary>
        /// Determines if the circle at the specified position intersects the line, which
        /// is at its true position and rotation, when the line is assumed to be vertical
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="line">The line</param>
        /// <param name="circleCenter">The center of the circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If the circle with center circleCenter intersects the line</returns>
        protected static bool CircleIntersectsVerticalLine(Circle2 circle, Line2 line, Vector2 circleCenter, bool strict)
        {
            // Same process as horizontal, but axis flipped
            var lineX = line.Start.X;
            // Step 1 - Find closest distance
            var vecCircleCenterToLine1D = lineX - circleCenter.X;
            var closestDistance = Math.Abs(vecCircleCenterToLine1D);

            // Step 1a
            if (strict)
            {
                if (closestDistance >= circle.Radius)
                    return false;
            }
            else
            {
                if (closestDistance > circle.Radius)
                    return false;
            }

            // Step 2 - Find closest point
            var closestPointY = circleCenter.Y;

            // Step 3 - Is closest point on line
            if (AxisAlignedLine2.Contains(line.Start.Y, line.End.Y, closestPointY, false, true))
                return true;

            // Step 4 - Find edgeClosest
            float edgeClosestY;
            if (line.Start.Y < line.End.Y)
                edgeClosestY = (closestPointY <= line.Start.Y) ? line.Start.Y : line.End.Y;
            else
                edgeClosestY = (closestPointY >= line.Start.Y) ? line.Start.Y : line.End.Y;

            // Step 5 - Circle-point intersection on closest point
            var distClosestEdgeToCircleSq = new Vector2(circleCenter.X - lineX, circleCenter.Y - edgeClosestY).LengthSquared();

            if (strict)
                return distClosestEdgeToCircleSq < circle.Radius * circle.Radius;
            else
                return distClosestEdgeToCircleSq <= circle.Radius * circle.Radius;
        }
        #region NoRotation
        /// <summary>
        /// Determines if the specified polygon at pos1 with no rotation and rectangle at pos2 intersect
        /// </summary>
        /// <param name="poly">Polygon to check</param>
        /// <param name="rect">Rectangle to check</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rect</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly at pos1 intersects rect at pos2</returns>
        public static bool Intersects(Polygon2 poly, Rect2 rect, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(poly, rect, pos1, pos2, Rotation2.Zero, strict);
        }

        /// <summary>
        /// Determines if the specified rectangle at pos1 intersects the specified polygon at pos2 with
        /// no rotation.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If rect at pos1 no rotation intersects poly at pos2</returns>
        public static bool Intersects(Rect2 rect, Polygon2 poly, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(rect, poly, pos1, pos2, Rotation2.Zero, strict);
        }

        /// <summary>
        /// Determines if the specified polygon at pos1 with no rotation intersects the specified
        /// 
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="rect"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static Tuple<Vector2, float> IntersectMTV(Polygon2 poly, Rect2 rect, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMTV(poly, rect, pos1, pos2, Rotation2.Zero);
        }

        /// <summary>
        /// Determines the minimum translation vector to be applied to the rect to prevent
        /// intersection with the specified polygon, when they are at the given positions.
        /// </summary>
        /// <param name="rect">The rect</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The origin of the rect</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <returns>MTV to move rect at pos1 to prevent overlap with poly at pos2</returns>
        public static Tuple<Vector2, float> IntersectMTV(Rect2 rect, Polygon2 poly, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMTV(rect, poly, pos1, pos2, Rotation2.Zero);
        }

        /// <summary>
        /// Determines if the polygon and circle intersect when at the given positions.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="circle">The circle</param>
        /// <param name="pos1">The origin of the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly at pos1 intersects circle at pos2</returns>
        public static bool Intersects(Polygon2 poly, Circle2 circle, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(poly, circle, pos1, pos2, Rotation2.Zero, strict);
        }

        /// <summary>
        /// Determines if the circle and polygon intersect when at the given positions.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle at pos1 intersects poly at pos2</returns>
        public static bool Intersects(Circle2 circle, Polygon2 poly, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(circle, poly, pos1, pos2, Rotation2.Zero, strict);
        }

        /// <summary>
        /// Determines the minimum translation vector the be applied to the polygon to prevent
        /// intersection with the specified circle, when they are at the given positions.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="circle">The circle</param>
        /// <param name="pos1">The position of the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <returns>MTV to move poly at pos1 to prevent overlap with circle at pos2</returns>
        public static Tuple<Vector2, float> IntersectMTV(Polygon2 poly, Circle2 circle, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMTV(poly, circle, pos1, pos2, Rotation2.Zero);
        }

        /// <summary>
        /// Determines the minimum translation vector to be applied to the circle to prevent
        /// intersection with the specified polyogn, when they are at the given positions.
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <returns></returns>
        public static Tuple<Vector2, float> IntersectMTV(Circle2 circle, Polygon2 poly, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMTV(circle, poly, pos1, pos2, Rotation2.Zero);
        }
        #endregion
    }

    /// <summary>
    /// A class containing utilities that help creating shapes.
    /// </summary>
    public class ShapeUtils
    {
        /// <summary>
        /// A dictionary containing the circle shapes.
        /// </summary>
        private static Dictionary<Tuple<float, float, float, float>, Polygon2> CircleCache = new Dictionary<Tuple<float, float, float, float>, Polygon2>();

        /// <summary>
        /// A dictionary containing the rectangle shapes.
        /// </summary>
        private static Dictionary<Tuple<float, float, float, float>, Polygon2> RectangleCache = new Dictionary<Tuple<float, float, float, float>, Polygon2>();

        /// <summary>
        /// A dictionary containing the convex polygon shapes.
        /// </summary>
        private static Dictionary<int, Polygon2> ConvexPolygonCache = new Dictionary<int, Polygon2>();

#if !NOT_MONOGAME
        /// <summary>
        /// Fetches the convex polygon (the smallest possible polygon containing all the non-transparent pixels) of the given texture.
        /// </summary>
        /// <param name="Texture">The texture.</param>
        public static Polygon2 CreateConvexPolygon(Texture2D Texture)
        {
            var Key = Texture.GetHashCode();

            if (ConvexPolygonCache.ContainsKey(Key))
                return ConvexPolygonCache[Key];

            var uints = new uint[Texture.Width * Texture.Height];
            Texture.GetData<uint>(uints);

            var Points = new List<Vector2>();

            for (var i = 0; i < Texture.Width; i++)
                for (var j = 0; j < Texture.Height; j++)
                    if (uints[j * Texture.Width + i] != 0)
                        Points.Add(new Vector2(i, j));

            if (Points.Count <= 2)
                throw new Exception("Can not create a convex hull from a line.");

            int n = Points.Count, k = 0;
            var h = new List<Vector2>(
                new Vector2[2 * n]
            );

            Points.Sort(
                (a, b) =>
                a.X == b.X ?
                     a.Y.CompareTo(b.Y)
                : (a.X > b.X ? 1 : -1)
             );

            for (var i = 0; i < n; ++i)
            {
                while (k >= 2 && cross(h[k - 2], h[k - 1], Points[i]) <= 0)
                    k--;
                h[k++] = Points[i];
            }

            for (int i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && cross(h[k - 2], h[k - 1], Points[i]) <= 0)
                    k--;
                h[k++] = Points[i];
            }

            Points = h.Take(k - 1).ToList();
            return ConvexPolygonCache[Key] = new Polygon2(Points.ToArray());
        }
#endif

        /// <summary>
        /// Returns the cross product of the given three vectors.
        /// </summary>
        /// <param name="v1">Vector 1.</param>
        /// <param name="v2">Vector 2.</param>
        /// <param name="v3">Vector 3.</param>
        /// <returns></returns>
        private static double cross(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return (v2.X - v1.X) * (v3.Y - v1.Y) - (v2.Y - v1.Y) * (v3.X - v1.X);
        }

        /// <summary>
        /// Fetches a rectangle shape with the given width, height, x and y center.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="x">The X center of the rectangle.</param>
        /// <param name="y">The Y center of the rectangle.</param>
        /// <returns>A rectangle shape with the given width, height, x and y center.</returns>
        public static Polygon2 CreateRectangle(float width, float height, float x = 0, float y = 0)
        {
            var Key = new Tuple<float, float, float, float>(width, height, x, y);

            if (RectangleCache.ContainsKey(Key))
                return RectangleCache[Key];

            return RectangleCache[Key] = new Polygon2(new[] {
                 new Vector2(x, y),
                 new Vector2(x + width, y),
                 new Vector2(x + width, y + height),
                 new Vector2(x, y + height)
            });
        }

        /// <summary>
        /// Fetches a circle shape with the given radius, center, and segments. Because of the discretization
        /// of the circle, it is not possible to perfectly get the AABB to match both the radius and the position.
        /// This will match the position.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="x">The X center of the circle.</param>
        /// <param name="y">The Y center of the circle.</param>
        /// <param name="segments">The amount of segments (more segments equals higher detailed circle)</param>
        /// <returns>A circle with the given radius, center, and segments, as a polygon2 shape.</returns>
        public static Polygon2 CreateCircle(float radius, float x = 0, float y = 0, int segments = 32)
        {
            var Key = new Tuple<float, float, float, float>(radius, x, y, segments);

            if (CircleCache.ContainsKey(Key))
                return CircleCache[Key];

            var Center = new Vector2(radius + x, radius + y);
            var increment = (Math.PI * 2.0) / segments;
            var theta = 0.0;
            var verts = new List<Vector2>(segments);

            Vector2 correction = new Vector2(radius, radius);
            for (var i = 0; i < segments; i++)
            {
                Vector2 vert = radius * new Vector2(
                        (float)Math.Cos(theta),
                        (float)Math.Sin(theta)
                    );

                if (vert.X < correction.X)
                    correction.X = vert.X;
                if (vert.Y < correction.Y)
                    correction.Y = vert.Y;

                verts.Add(
                    Center + vert
                );
                theta += increment;
            }

            correction.X += radius;
            correction.Y += radius;

            for (var i = 0; i < segments; i++)
            {
                verts[i] -= correction;
            }

            return CircleCache[Key] = new Polygon2(verts.ToArray());
        }
    }

    /// <summary>
    /// Describes a triangle, which is a collection of three points. This is
    /// used for the implementation of the Polygon2.
    /// </summary>
    public class Triangle2
    {
        /// <summary>
        /// The 3 vertices of this triangle.
        /// </summary>
        public Vector2[] Vertices;

        /// <summary>
        /// This is used to determine if points are inside the triangle.
        /// This has 4 values where the first 2 correspond to row 1 and
        /// the second 2 to row 2 of a 2x2 matrix. When that matrix is
        /// matrix-multiplied by a point, if the result has a sum less
        /// than 1 and each component is positive, the point is in the
        /// triangle.
        /// </summary>
        private float[] InvContainsBasis;

        /// <summary>
        /// The centroid of the triangle
        /// </summary>
        public readonly Vector2 Center;

        /// <summary>
        /// The edges of the triangle, where the first edge is from 
        /// Vertices[0] to Vertices[1], etc.
        /// </summary>
        public readonly Line2[] Edges;

        /// <summary>
        /// The area of the triangle.
        /// </summary>
        public readonly float Area;

        /// <summary>
        /// Constructs a triangle with the given vertices, assuming that
        /// the vertices define a triangle (i.e., are not collinear)
        /// </summary>
        /// <param name="vertices">The vertices of the triangle</param>
        public Triangle2(Vector2[] vertices)
        {
            Vertices = vertices;

            Vector2 vertSum = Vector2.Zero;
            for (int i = 0; i < 3; i++)
            {
                vertSum += vertices[i];
            }

            Center = vertSum / 3.0f;
            float a = vertices[1].X - vertices[0].X;
            float b = vertices[2].X - vertices[0].X;
            float c = vertices[1].Y - vertices[0].Y;
            float d = vertices[2].Y - vertices[0].Y;

            float det = a * d - b * c;
            Area = 0.5f * det;

            float invDet = 1 / det;
            InvContainsBasis = new float[4]
            {
                invDet * d, -invDet * b,
                -invDet * c, invDet * a
            };

            Edges = new Line2[]
            {
                new Line2(Vertices[0], Vertices[1]),
                new Line2(Vertices[1], Vertices[2]),
                new Line2(Vertices[2], Vertices[0])
            };
        }

        /// <summary>
        /// Checks if this triangle contains the given point. This is
        /// never strict.
        /// </summary>
        /// <param name="tri">The triangle</param>
        /// <param name="pos">The position of the triangle</param>
        /// <param name="pt">The point to check</param>
        /// <returns>true if this triangle contains the point or the point
        /// is along an edge of this polygon</returns>
        public static bool Contains(Triangle2 tri, Vector2 pos, Vector2 pt)
        {
            Vector2 relPt = pt - pos - tri.Vertices[0];
            float r = tri.InvContainsBasis[0] * relPt.X + tri.InvContainsBasis[1] * relPt.Y;
            if (r < -Math2.DEFAULT_EPSILON)
                return false;

            float t = tri.InvContainsBasis[2] * relPt.X + tri.InvContainsBasis[3] * relPt.Y;
            if (t < -Math2.DEFAULT_EPSILON)
                return false;

            return (r + t) < 1 + Math2.DEFAULT_EPSILON;
        }
    }
}