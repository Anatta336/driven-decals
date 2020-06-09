using UnityEngine;
using Unity.Collections;

namespace SamDriver.Decal
{
    /// <summary>
    /// Immutable vertex with position and normal
    /// </summary>
    internal struct Vertex
    {
        [ReadOnly] public Float3 Position;
        [ReadOnly] public Float3 Normal;

        public Vertex(Float3 position_, Float3 normal_)
        {
            this.Position = position_;
            this.Normal = normal_;
        }

        public static Vertex CreateFromExisting(Vertex a, Vertex b, float tFromAToB)
        {
            tFromAToB = Mathf.Clamp01(tFromAToB);

            Float3 createdPosition = Float3.Lerp(a.Position, b.Position, tFromAToB);

            // spherical linear interpolation is probably more correct,
            // but I think linear interpolation gives consistency with rendering
            Float3 createdNormal = Float3.Normalize(Float3.Lerp(a.Normal, b.Normal, tFromAToB));

            return new Vertex(createdPosition, createdNormal);
        }
    }
}
