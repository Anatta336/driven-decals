using UnityEngine;
using Unity.Collections;

namespace SamDriver.Decal
{
    /// <summary>
    /// Immutable Float3, suitable for use in Unity's job system.
    /// </summary>
    internal struct Float3
    {
        [ReadOnly] public readonly float x, y, z;

        float? _magnitude;
        public float Magnitude
        {
            get
            {
                if (!_magnitude.HasValue)
                {
                    _magnitude = Mathf.Sqrt(MagnitudeSquared);
                }
                return _magnitude.Value;
            }
        }

        public float MagnitudeSquared { get => x * x + y * y + z * z; }

        public Float3(float x_, float y_, float z_)
        {
            this.x = x_;
            this.y = y_;
            this.z = z_;
            _magnitude = null;
        }

        public Float3(Vector3 source)
          : this(source.x, source.y, source.z)
        {}

        public Float3(Float3 source)
          : this(source.x, source.y, source.z)
        {}

        public bool IsNearlyEqual(Float3 other, float fuzziness = 0.0001f)
        {
            return
              Mathf.Abs(this.x - other.x) < fuzziness &&
              Mathf.Abs(this.y - other.y) < fuzziness &&
              Mathf.Abs(this.z - other.z) < fuzziness;
        }

        public float ComponentInDimension(Dimension dimension)
        {
            switch (dimension)
            {
                case Dimension.x:
                    return this.x;
                case Dimension.y:
                    return this.y;
                case Dimension.z:
                default:
                    return this.z;
            }
        }

        public string ToString(string format)
        {
            return $"{x.ToString(format)}, {y.ToString(format)}, {z.ToString(format)}";
        }
        public override string ToString()
        {
            return ToString("F2");
        }

        public Vector3 AsVector3 { get => new Vector3(x, y, z); }

        public static explicit operator Vector3(Float3 a) => a.AsVector3;

        public static Float3 operator +(Float3 a) => a;
        
        public static Float3 operator -(Float3 a) => new Float3(-a.x, -a.y, -a.z);

        public static Float3 operator +(Float3 a, Float3 b) => new Float3(
          a.x + b.x,
          a.y + b.y,
          a.z + b.z
        );
        public static Float3 operator -(Float3 a, Float3 b) => a + (-b);

        public static Float3 operator *(Float3 a, float b) => new Float3(
          a.x * b,
          a.y * b,
          a.z * b
        );
        public static Float3 operator *(float a, Float3 b) => b * a;

        public static Float3 operator /(Float3 a, float b) => a * (1f / b);

        public static Float3 operator *(Float3 a, Float3 b) => new Float3(
          a.x * b.x,
          a.y * b.y,
          a.z * b.z
        );

        public static Float3 CrossProduct(Float3 a, Float3 b) => new Float3(
          a.y * b.z - a.z * b.y,
          a.z * b.x - a.x * b.z,
          a.x * b.y - a.y * b.x
        );

        public static float DotProduct(Float3 a, Float3 b) =>
          a.x * b.x + a.y * b.y + a.z * b.z;

        public static Float3 Normalize(Float3 a) =>
          Mathf.Approximately(a.Magnitude, 1f) ?
            a :
            (a / a.Magnitude);

        public static Float3 Lerp(Float3 a, Float3 b, float t) => new Float3(
          Mathf.Lerp(a.x, b.x, t),
          Mathf.Lerp(a.y, b.y, t),
          Mathf.Lerp(a.z, b.z, t)
        );

        public static Float3 SetXZero(Float3 a) => new Float3(0f, a.y, a.z);
        public static Float3 SetYZero(Float3 a) => new Float3(a.x, 0f, a.z);
        public static Float3 SetZZero(Float3 a) => new Float3(a.x, a.y, 0f);
    }
}
