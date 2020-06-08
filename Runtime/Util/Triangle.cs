using Unity.Collections;
using System.Collections.Generic;
using System.Collections;

namespace SamDriver.Decal
{
    /// <summary>
    /// Immutable representation of a triangle, with several useful methods.
    /// </summary>
    internal struct Triangle : IEnumerable<Vertex>
    {
        [ReadOnly] public readonly bool IsPresent; // will default to false when struct is uninitialised
        [ReadOnly] public readonly Vertex A, B, C;

        /// <summary>
        /// The normal of the plane defined by this triangle. Note this may not be equal to the
        /// normal(s) stored on the vertices that make up this triangle.
        /// </summary>
        public Float3 GeometryNormal
        {
            get
            {
                if (!_geometryNormal.HasValue)
                {
                    _geometryNormal = Float3.Normalize(
                        Float3.CrossProduct(
                            B.Position - A.Position,
                            C.Position - A.Position
                    ));
                }
                return _geometryNormal.Value;
            }
        }
        Float3? _geometryNormal;

        public Triangle(Vertex a_, Vertex b_, Vertex c_)
        {
            this.A = a_;
            this.B = b_;
            this.C = c_;
            _geometryNormal = null;
            IsPresent = true;
        }

        public IEnumerator<Vertex> GetEnumerator()
        {
            yield return A;
            yield return B;
            yield return C;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<Edge> EnumerateEdges()
        {
            yield return new Edge(A, B);
            yield return new Edge(B, C);
            yield return new Edge(C, A);
        }

        /// <summary>
        /// Tests if a line defined by a y and z coordinate pair intersects this triangle.
        /// Equivalent to projecting the triangle onto the yz plane and testing for a point being inside.
        /// </summary>
        public bool IsAxialXLineWithin(float y, float z)
        {
            // projecting the triangle onto local yz plane just means setting x components to zero
            return IsProjectedVertexWithinTriangle(
                Float3.SetXZero(A.Position),
                Float3.SetXZero(B.Position),
                Float3.SetXZero(C.Position),
                new Float3(0f, y, z)
            );
        }

        /// <summary>
        /// Tests if a line defined by a x and z coordinate pair intersects this triangle.
        /// Equivalent to projecting the triangle onto the xz plane and testing for a point being inside.
        /// </summary>
        public bool IsAxialYLineWithin(float x, float z)
        {
            // projecting the triangle onto local xz plane just means setting y components to zero
            return IsProjectedVertexWithinTriangle(
                Float3.SetYZero(A.Position),
                Float3.SetYZero(B.Position),
                Float3.SetYZero(C.Position),
                new Float3(x, 0f, z)
            );
        }

        /// <summary>
        /// Tests if a line defined by a x and y coordinate pair intersects this triangle.
        /// Equivalent to projecting the triangle onto the xy plane and testing for a point being inside.
        /// </summary>
        public bool IsAxialZLineWithin(float x, float y)
        {
            // projecting the triangle onto local xy plane just means setting z components to zero
            return IsProjectedVertexWithinTriangle(
                Float3.SetZZero(A.Position),
                Float3.SetZZero(B.Position),
                Float3.SetZZero(C.Position),
                new Float3(x, y, 0f)
            );
        }

        /// <summary>
        /// Given an yz position find z on the plane defined by this triangle.
        /// </summary>
        public float getXAtYZ(float y, float z)
        {
            // we have a plane defined by this triangle, and just need to find y given xz
            return (Float3.DotProduct(GeometryNormal, A.Position) -
                    GeometryNormal.y * y - GeometryNormal.z * z
                ) / GeometryNormal.x;
        }

        /// <summary>
        /// Given an xz position find y on the plane defined by this triangle.
        /// </summary>
        public float getYAtXZ(float x, float z)
        {
            // we have a plane defined by this triangle, and just need to find y given xz
            return (Float3.DotProduct(GeometryNormal, A.Position) -
                    GeometryNormal.x * x - GeometryNormal.z * z
                ) / GeometryNormal.y;
        }

        /// <summary>
        /// Given an xy position find z on the plane defined by this triangle.
        /// </summary>
        public float GetZAtXY(float x, float y)
        {
            // we have a plane defined by this triangle, and just need to find z given xy
            return (Float3.DotProduct(GeometryNormal, A.Position) -
                    GeometryNormal.x * x - GeometryNormal.y * y
                ) / GeometryNormal.z;
        }

        /// <summary>
        /// Note! Assumes the given point is within the triangle.
        /// </summary>
        public Float3 InterpolateNormal(Float3 targetPoint)
        {
            Float3 u = B.Position - A.Position;
            Float3 v = C.Position - A.Position;
            Float3 n = Float3.CrossProduct(u, v);
            Float3 w = targetPoint - A.Position;

            // barycentric coordinates of the projection P' of P to plane of triangle
            float gamma = Float3.DotProduct(Float3.CrossProduct(u, w), n) / n.MagnitudeSquared;
            float beta = Float3.DotProduct(Float3.CrossProduct(w, v), n) / n.MagnitudeSquared;
            float alpha = 1f - gamma - beta;

            // normalP = α*normalA + β*normalB + γ*normalC
            Float3 interpolated =
              alpha * A.Normal +
              beta * B.Normal +
              gamma * C.Normal;
            return Float3.Normalize(interpolated);
        }
        
        public static bool IsProjectedVertexWithinTriangle(
          Float3 A, Float3 B, Float3 C, Float3 P)
        {
            // given triangle ABC and point P
            Float3 u = B - A;
            Float3 v = C - A;
            Float3 n = Float3.CrossProduct(u, v);
            Float3 w = P - A;

            // barycentric coordinates of the projection P' of P to plane of triangle
            float gamma = Float3.DotProduct(Float3.CrossProduct(u, w), n) / n.MagnitudeSquared;
            float beta = Float3.DotProduct(Float3.CrossProduct(w, v), n) / n.MagnitudeSquared;

            //NOTE: alpha is only a correct barycentric coordinate if point is within triangle
            float alpha = 1f - gamma - beta;

            // for reference: alpha is the weight of A, beta weight of B, gamma weight of C

            // P' is within triangle if barycentric coordinates are all in range 0 to 1
            return (alpha >= 0f) && (alpha <= 1f) &&
                (beta >= 0f) && (beta <= 1f) &&
                (gamma >= 0f) && (gamma <= 1f);
        }
    }
}
