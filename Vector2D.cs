﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AutoAVL
{
    /// <summary>
    /// A 2D vector class that represents both points and vectors.
    /// </summary>
    public class Vector2D
    {
        public float x, y;

        /// <summary>
        /// Initializes a new instance of the Vector2D class with coordinates (0, 0).
        /// </summary>
        public Vector2D()
        {
            x = 0.0f;
            y = 0.0f;
        }

        /// <summary>
        /// Initializes a new instance of the Vector2D class with the specified coordinates.
        /// </summary>
        /// <param name="a">The x-coordinate.</param>
        /// <param name="b">The y-coordinate.</param>
        public Vector2D(float a, float b)
        {
            x = a;
            y = b;
        }

        /// <summary>
        /// Negates the specified vector.
        /// </summary>
        /// <param name="a">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vector2D operator -(Vector2D a) => new Vector2D(-a.x, -a.y);

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static Vector2D operator +(Vector2D a, Vector2D b)
            => new Vector2D(a.x + b.x, a.y + b.y);

        /// <summary>
        /// Subtracts one vector from another.
        /// </summary>
        /// <param name="a">The vector to subtract from.</param>
        /// <param name="b">The vector to subtract.</param>
        /// <returns>The difference between the two vectors.</returns>
        public static Vector2D operator -(Vector2D a, Vector2D b)
            => a + (-b);

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="a">The vector to multiply.</param>
        /// <param name="value">The scalar value.</param>
        /// <returns>The product of the vector and scalar value.</returns>
        public static Vector2D operator *(Vector2D a, float value)
            => new Vector2D(a.x * value, a.y * value);

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="value">The scalar value.</param>
        /// <param name="a">The vector to multiply.</param>
        /// <returns>The product of the vector and scalar value.</returns>
        public static Vector2D operator *(float value, Vector2D a)
            => a * value;

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="a">The vector to divide.</param>
        /// <param name="value">The scalar value.</param>
        /// <returns>The quotient of the vector and scalar value.</returns>
        public static Vector2D operator /(Vector2D a, float value)
            => new Vector2D(a.x / value, a.y / value);

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">The scalar value.</
        /// <returns>The resulting Vector2D object after the division.</returns>


        public Vector2D Divide(float value)
        {
            if (value == 0.0f)
            {
                throw new DivideByZeroException("Error: division by zero");
            }
            return new Vector2D(x / value, y / value);
        }

        /// <summary>
        /// Sets the X and Y components of the vector to 0.
        /// </summary>
        public void Reset()
        {
            // Set the X and Y components of the vector to 0
            x = 0.0f;
            y = 0.0f;
        }


        /// <summary>
        /// Calculates and returns the dot product of two vectors.
        /// </summary>
        /// <param name="vector">The other Vector2D object.</param>
        /// <returns>The dot product value as a float.</returns>
        public float Dot(Vector2D vector)
        {
            return (x * vector.x) + (y * vector.y);
        }

        /// <summary>
        /// Computes the cross product of two 2D vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The scalar result of the cross product.</returns>
        public float Cross(Vector2D b)
        {
            // The cross product of two 2D vectors (a, b) is defined as:
            // a.x * b.y - a.y * b.x
            // This yields a scalar value representing the magnitude of the 3D vector perpendicular
            // to both a and b in the z-direction (assuming a right-handed coordinate system).

            // Compute the cross product and return the result.
            return x * b.y - y * b.x;
        }


        /// <summary>
        /// Transforms the vector from Cartesian coordinates to SVG coordinates using a specified origin.
        /// </summary>
        /// <param name="origin">The origin point of the SVG coordinate system.</param>
        /// <returns>A new Vector2D object that represents the vector in SVG coordinates.</returns>
        public Vector2D ToSvgCoordinates(Vector2D origin)
        {
            return new Vector2D(x - origin.x, origin.y - y);
        }

        public Vector2D FromSvgCoordinates(Vector2D origin)
        {
            return new Vector2D(origin.x + x, origin.y - y);
        }

        public float DistancePointToLine(Vector2D lineA, Vector2D lineB)
        {
            Vector2D lineDirection = lineB - lineA;
            Vector2D lineAToPoint = this - lineA;
            float distance = Math.Abs(lineDirection.y * lineAToPoint.x - lineDirection.x * lineAToPoint.y) / lineDirection.Length();
            return distance;
        }

        /// <summary>
        /// Computes the middle point between two vectors.
        /// </summary>
        /// <param name="vector">The other vector.</param>
        /// <returns>A new Vector2D object that represents the middle point.</returns>
        public Vector2D Middle(Vector2D vector)
        {
            float midX = (x + vector.x) / 2f;
            float midY = (y + vector.y) / 2f;
            return new Vector2D(midX, midY);
        }
        
        public float Slope()
        {
            return y / x;
        }

        /// <summary>
        /// Returns the angle between the vector and the x-axis.
        /// </summary>
        /// <returns>The angle between the vector and the positive x-axis in radians. The angle is always taken in a counter-clockwise rotation,
        /// so it will always be a positive value in the range 0 ≤ θ < 2π.</returns>
        public float UnsignedAngle()
        {
            float signedAngle = this.SignedAngle();
            
            return (signedAngle >= 0) ? signedAngle : signedAngle + 2 * (float) Math.PI;
        }
        
        /// <summary>
        /// Returns the angle between the vector and the x-axis.
        /// </summary>
        /// <returns>The angle between the vector and the positive x-axis in radians. The angle is taken using the rotation wich results in the smallest absolute value,
        /// so both clockwise and counter-clockwise rotations are used. In the case of an counter-clockwise rotation, the angle is positive, while if the rotation is clockwise the 
        /// angle will be negatie. The angle's values are in the range -π ≤ θ ≤ π. For the boundary positions, the angle will be 0, π/2, π and -π/2 for when the vector
        /// aligns with +x, +y, -x and -y axis respectively.</returns>
        public float SignedAngle()
        {
            
            return (float) Math.Atan2(y,x);
        }
        
        /// <summary>
        /// Returns the smallest angle between two vectors, regardless of order.
        /// </summary>
        /// <returns>The smallest angle delimited by vectors A and B, without considering the order the vectors are taken in.
        /// The value is always positive, in the range 0 ≤ θ ≤ π</returns>
        public static float SmallestAngleBetweenVectors(Vector2D vectorA, Vector2D vectorB)
        {
            float dotProduct = vectorA.Dot(vectorB);
            float lengthsProduct = vectorA.Length() * vectorB.Length();

            if (lengthsProduct == 0.0f)
            {
                throw new DivideByZeroException("Error: division by zero");
            }

            float cosAngle = dotProduct / lengthsProduct;
            float angle = (float) Math.Acos(cosAngle);

            return angle;
        }
        
        /// <summary>
        /// Returns the largest angle between two vectors, regardless of order.
        /// </summary>
        /// <returns>The largest angle delimited by vectors A and B, without considering the order the vectors are taken in.
        /// The value is always positive, in the range π ≤ θ ≤ 2π</returns>
        public static float LargestAngleBetweenVectors(Vector2D vectorA, Vector2D vectorB)
        {
            float smallestAngle = SmallestAngleBetweenVectors(vectorA, vectorB);
            
            return 2 * (float) Math.PI - smallestAngle;
        }
        
        /// <summary>
        /// Returns the smallest angle in module needed to rotate the current vector to the given vector's position.
        /// </summary>
        /// <returns>The smallest angle necessary to rotate the current vector to the position of the given vector.
        /// The angle can be either positive or negative, meaning the needed rotation is counter-clockwise or clockwise, respectively.
        /// Tha angle's value range is -π ≤ θ ≤ π.</returns>
        public float SignedRotationAngle(Vector2D vectorB)
        {
            float magnitudeProduct = this.Length() * vectorB.Length();
            float sin = Cross(vectorB) / magnitudeProduct;
            float cos = Dot(vectorB) / magnitudeProduct;

            return (float) Math.Atan2(sin, cos);
        }
        
        /// <summary>
        /// Returns the angle needed to rotate the current vector to the given vector's position.
        /// </summary>
        /// <returns>The angle necessary to rotate the current vector to the position of the given vector, always in a counter-clockwise rotation.
        /// The angle can only be positive, with it's value in the range 0 ≤ θ < 2π.</returns>
        public float UnsignedRotationAngle(Vector2D vectorB)
        {
            float signedRotationAngle = this.SignedRotationAngle(vectorB);

            return (signedRotationAngle >= 0) ? signedRotationAngle : signedRotationAngle + 2 * (float) Math.PI;
        }

        public static float AngleBetween(float opposite, float adjacent1, float adjacent2)
        {
            // Calculate the cosine value of the angle using the Law of Cosines formula
            float cosValue = (-opposite * opposite + adjacent2 * adjacent2 + adjacent1 * adjacent1) / (2 * adjacent2 * adjacent1);

            // Calculate the angle in radians using the inverse cosine function
            float angleInRadians = (float) Math.Acos(cosValue);

            return angleInRadians;
        }

        public static float DistanceToLine(Vector2D point, Vector2D lineStart, Vector2D lineEnd)
        {
            Vector2D vStartEnd = lineEnd - lineStart;
            Vector2D vStartPoint = lineEnd - point;

            float crossProduct = vStartPoint.Cross(vStartEnd);
            float dotProduct = vStartPoint.Dot(vStartEnd);

            float distance = Math.Abs(crossProduct) / vStartEnd.Length();

            return distance;
        }

        /// <summary>
        /// Rotates the vector counterclockwise by a given angle in radians.
        /// Modifies the current vector.
        /// </summary>
        /// <param name="angle">The angle value as a float in radians.</param>
        public void Rotate(float angle)
        {
            float cos = (float) Math.Cos(angle);
            float sin = (float) Math.Sin(angle);

            float new_x = x * cos - y * sin;
            float new_y = x * sin + y * cos;

            x = new_x;
            y = new_y;
        }

        /// <summary>
        /// Returns a new vector that is the result of rotating the current vector counterclockwise by a given angle in radians.
        /// Does not modify the current vector.
        /// </summary>
        /// <param name="angle">The angle value as a float in radians.</param>
        /// <returns>A new vector rotated by the given angle.</returns>
        public Vector2D Rotated(float angle)
        {
            float cos = (float) Math.Cos(angle);
            float sin = (float) Math.Sin(angle);

            float new_x = x * cos - y * sin;
            float new_y = x * sin + y * cos;
            

            return new Vector2D(new_x, new_y);
        }


        /// <summary>
        /// Rotates the vector by a given angle in degrees.
        /// </summary>
        /// <param name="angle">The angle value as a float in degrees.</param>
        public void RotateInDegrees(float angle)
        {
            float radians = angle * ((float) Math.PI / 180.0f);
            Rotate(radians);
        }

        /// <summary>
        /// Calculates and returns a new vector that is perpendicular to the current vector.
        /// </summary>
        /// <returns>A new Vector2D object that represents a vector perpendicular to the current vector.</returns>
        public Vector2D Perpendicular()
        {
            return new Vector2D(y, -x).Normalized();
        }

        /// <summary>
        /// Calculates and returns the length of the vector.
        /// </summary>
        /// <returns>The length value as a float.</returns>
        public float Length()
        {
            return (float) Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Calculates and returns a new vector that has the same direction as the current vector, but with a length of 1.
        /// </summary>
        /// <returns>A new Vector2D object that represents a normalized vector.</returns>
        public Vector2D Normalized()
        {
            float length = Length();
            return new Vector2D(x / length, y / length);
        }

        /// <summary>
        /// Normalizes the current vector, making it a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            float vectorLength = Length();
            x /= vectorLength;
            y /= vectorLength;
        }

        /// <summary>
        /// Calculates the perpendicular distance from a point to an infinite line that passes through the start and end points.
        /// </summary>
        /// <param name="start">The start point of the line.</param>
        /// <param name="end">The end point of the line.</param>
        /// <param name="point">The point to calculate the distance from.</param>
        /// <returns>The perpendicular distance from the point to the line.</returns>
        public static float PerpendicularDistance(Vector2D start, Vector2D end, Vector2D point)
        {
            // Calculate the vector P1P2
            Vector2D lineVector = end - start;

            // Calculate the vector P1P
            Vector2D pointVector = point - start;

            // Calculate the cross product of P1P2 and P1P
            float crossProduct = lineVector.Cross(pointVector);

            // Calculate the magnitude of P1P2
            float lineLength = lineVector.Length();

            // Calculate the perpendicular distance
            float distance = crossProduct / lineLength;

            return distance;
        }

        /// <summary>
        /// Returns the vector as a string in the format "(x, y)".
        /// </summary>
        /// <returns>A string representation of the vector.</returns>
        public override string ToString()
        {
            return $"({x}, {y})";
        }

        /// <summary>
        /// Returns a new Vector2D object with the same x and y components as this vector.
        /// </summary>
        /// <returns>A new Vector2D object with the same x and y components as this vector.</returns>
        public Vector2D Clone()
        {
            return new Vector2D(x, y);
        }

        public static Vector2D FindCenter(Vector2D A, Vector2D B, Vector2D C)
        {
            float tolerance = 0.00001f;
            Vector2D slopeAC = (C - A);

            if (Math.Abs(slopeAC.y) < tolerance)
            {
                // line AC is horizontal
                Vector2D middleAC = A.Middle(C);
                Vector2D middleAB = A.Middle(B);

                Vector2D vAB = B - A;
                Vector2D pAB = vAB.Perpendicular();

                float pSlopeAB = pAB.Slope();

                float circleX = middleAC.x;
                float circleY = middleAB.y - pSlopeAB * middleAB.x + pSlopeAB * circleX;

                return new Vector2D(circleX, circleY);
            }
            else if (Math.Abs(slopeAC.x) < tolerance)
            {
                // line AC is vertical
                Vector2D middleAC = A.Middle(C);
                Vector2D middleAB = A.Middle(B);

                Vector2D vAB = B - A;
                Vector2D pAB = vAB.Perpendicular();

                float pSlopeAB = pAB.Slope();

                float circleY = middleAC.y;
                float circleX = (circleY - middleAB.y + pSlopeAB * middleAB.x) / pSlopeAB;

                return new Vector2D(circleX, circleY);
            }
            else
            {
                Vector2D m1 = A.Middle(C);
                Vector2D m2 = A.Middle(B);

                Vector2D vAB = B - A;

                if (Math.Abs(vAB.y) < tolerance)
                {
                    float circleX = m2.x;

                    Vector2D vAC = C - A;
                    Vector2D pAC = vAC.Perpendicular();

                    float pSlopeAC = pAC.Slope();
                    float circleY = m1.y - pSlopeAC * m1.x + pSlopeAC * circleX;

                    return new Vector2D(circleX, circleY);
                }
                else if (Math.Abs(vAB.x) < tolerance)
                {
                    float circleY = m2.y;

                    Vector2D vAC = C - A;
                    Vector2D pAC = vAC.Perpendicular();

                    float pSlopeAC = pAC.Slope();
                    float circleX = (circleY - m1.y + pSlopeAC * m1.x) / pSlopeAC;

                    return new Vector2D(circleX, circleY);
                }
                else
                {
                    // General case - Calculate the equations of perpendicular bisectors for AB and BC
                    Vector2D middleAB = A.Middle(B);
                    Vector2D middleBC = B.Middle(C);

                    Vector2D vBC = C - B;

                    Vector2D pAB = vAB.Perpendicular();
                    Vector2D pBC = vBC.Perpendicular();

                    // Slopes of perpendicular bisectors
                    float pSlopeAB = pAB.Slope();
                    float pSlopeBC = pBC.Slope();

                    // Intersection point of the two perpendicular bisectors
                    float circleX = (pSlopeAB * middleAB.x - pSlopeBC * middleBC.x + middleBC.y - middleAB.y) / (pSlopeAB - pSlopeBC);
                    float circleY = middleAB.y + pSlopeAB * (circleX - middleAB.x);

                    return new Vector2D(circleX, circleY);
                }
            }
        }
    }
}
