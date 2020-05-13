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
  internal struct Vertex
  {
    [ReadOnly]
    public Float3 Position;
    [ReadOnly]
    public Float3 Normal;

    #region constructors
    public Vertex(Float3 position_, Float3 normal_)
    {
      this.Position = position_;
      this.Normal = normal_;
    }
    #endregion

    #region static methods
    public static Vertex CreateFromExisting(Vertex a, Vertex b, float tFromAToB)
    {
      tFromAToB = Mathf.Clamp01(tFromAToB);
      
      Float3 createdPosition = Float3.Lerp(a.Position, b.Position, tFromAToB);

      // spherical linear interpolation is probably more correct,
      // but I think linear interpolation gives more consistency with rendering
      Float3 createdNormal = Float3.Normalize(Float3.Lerp(a.Normal, b.Normal, tFromAToB));

      return new Vertex(createdPosition, createdNormal);
    }
    #endregion

  }

  /*
  internal struct Vertex
  {
    public static Vertex CreateFromMesh(
      Vector3 meshVertexPosition, Vector3 meshVertexNormal,
      Transform meshTransform, Transform decalTransform)
    {
      Vertex created = new Vertex();
      Vector3 worldPosition = meshTransform.TransformPoint(meshVertexPosition);
      created.decalPosition = decalTransform.InverseTransformPoint(worldPosition);
      created.SetProjectedPositionFromPosition();

      Vector3 worldNormal = meshTransform.TransformDirection(meshVertexNormal);
      created.decalNormal = decalTransform.InverseTransformDirection(worldNormal);
      
      return created;
    }

    public static Vertex CreateFromExisting(Vertex a, Vertex b, float tFromAtoB)
    {
      if (tFromAtoB < 0f || tFromAtoB > 1f) Debug.LogWarning($"{nameof(tFromAtoB)} = {tFromAtoB} which is out of expected range.");

      Vertex created = new Vertex();
      created.decalPosition = a.decalPosition + (b.decalPosition - a.decalPosition) * tFromAtoB;
      created.SetProjectedPositionFromPosition();

      // spherical linear interpolation is probably more correct,
      // but I think linear interpolation gives more consistency for how it's done when rendering
      // created.decalNormal = Vector3.Slerp(a.decalNormal, b.decalNormal, tFromAtoB);
      created.decalNormal = Vector3.Lerp(a.decalNormal, b.decalNormal, tFromAtoB);

      return created;
    }

    public Vertex(Vector3 decalPosition_, Vector3 decalNormal_)
    {
      this.decalPosition = decalPosition_;
      this.projectedDecalPosition = new Vector3(
        decalPosition.x,
        decalPosition.y,
        0f
      );
      this.decalNormal = decalNormal_;
    }
    
    public float GetComponent(Dimension dimension)
    {
      switch (dimension)
      {
        case Dimension.x:
          return decalPosition.x;
        case Dimension.y:
          return decalPosition.y;
        case Dimension.z:
          return decalPosition.z;
        default:
          Debug.LogWarning($"Unhandled dimension {dimension}");
          return 0;
      }
    }

    public void SetProjectedPositionFromPosition()
    {
      // projecting in decalspace to its xy plane is just zeroing out z component
      projectedDecalPosition = new Vector3(
        decalPosition.x,
        decalPosition.y,
        0f
      );
    }
  }
  */
}
