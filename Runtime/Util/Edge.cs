using UnityEngine;
using Unity.Collections;

namespace SamDriver.Decal
{
    /// <summary>
    /// Immutable representation of an edge linking two vertices.
    /// </summary>
    internal struct Edge
    {
        [ReadOnly] public readonly Vertex A, B;

        public Edge(Vertex a_, Vertex b_)
        {
            this.A = a_;
            this.B = b_;
        }

        public Vertex GetVertexBetween(float tFromAToB) =>
            Vertex.CreateFromExisting(A, B, tFromAToB);

        /// <summary>
        /// Find the scalar t such that using it to linearly interpolate
        /// from v0 to v1 would give goal value in specified dimension.
        /// </summary>
        public float InverseLerpDimension(float goalPositionInDimension, Dimension dimension) =>
            Mathf.InverseLerp(
                A.Position.ComponentInDimension(dimension),
                B.Position.ComponentInDimension(dimension),
                goalPositionInDimension
        );

        /// <summary>
        /// Linearly interpolates between positions of A and B, giving the value of just one dimension.
        /// </summary>
        public float LerpDimension(float t, Dimension dimension) =>
            Mathf.Lerp(
                A.Position.ComponentInDimension(dimension),
                B.Position.ComponentInDimension(dimension),
                t
        );
    }
}
