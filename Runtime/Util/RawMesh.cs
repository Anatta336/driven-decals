using Unity.Collections;
using System;

namespace SamDriver.Decal
{
    /// <summary>
    /// Immutable representation of a mesh, ready to be used in the
    /// Unity job system.
    /// </summary>
    internal struct RawMesh : IDisposable
    {
        [ReadOnly] public readonly NativeArray<int> Indices;
        [ReadOnly] public readonly NativeArray<Float3> Positions;
        [ReadOnly] public readonly NativeArray<Float3> Normals;

        public int TriangleCount { get => Indices.Length / 3; }

        public RawMesh(
          NativeArray<int> indices_,
          NativeArray<Float3> positions_,
          NativeArray<Float3> normals_)
        {
            this.Indices = indices_;
            this.Positions = positions_;
            this.Normals = normals_;
        }

        public void Dispose()
        {
            Indices.Dispose();
            Positions.Dispose();
            Normals.Dispose();
        }
    }
}
