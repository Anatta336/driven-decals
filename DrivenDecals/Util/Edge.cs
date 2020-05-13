using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace SamDriver.Decal
{
  internal struct Edge
  {
    [ReadOnly]
    public readonly Vertex A, B;

    #region constructors
    public Edge(Vertex a_, Vertex b_)
    {
      this.A = a_;
      this.B = b_;
    }
    #endregion

    #region methods
    public Vertex GetVertexBetween(float tFromAToB) =>
      Vertex.CreateFromExisting(A, B, tFromAToB);


    /// <summary>
    /// Find the scalar t such that using it to linearly interpolating
    /// from v0 to v1 would give goal value in specified dimension.
    /// </summary>
    public float InverseLerpDimension(float goalPositionInDimension, Dimension dimension) =>
      Mathf.InverseLerp(
        A.Position.ComponentInDimension(dimension),
        B.Position.ComponentInDimension(dimension),
        goalPositionInDimension
    );

    // <summary>
    /// Linearly interpolates between positions of A and B, giving the value of just one dimension.
    /// </summary>
    public float LerpDimension(float t, Dimension dimension) =>
      Mathf.Lerp(
        A.Position.ComponentInDimension(dimension),
        B.Position.ComponentInDimension(dimension),
        t
    );


    #endregion

  }
}
