using UnityEngine;

namespace SamDriver.Decal
{
  internal struct Vertex
  {
    public Vector3 decalPosition;
    public Vector3 projectedDecalPosition;
    public Vector3 decalNormal;

    public override bool Equals(object obj) 
    {
      if (!(obj is Vertex)) return false;

      Vertex other = (Vertex)obj;
      return (this.decalPosition == other.decalPosition &&
        this.decalNormal == other.decalNormal);
    }

    public bool EqualPosition(Vertex other)
    {
      return this.decalPosition == other.decalPosition;
    }

    public override int GetHashCode()
    {
      return decalPosition.GetHashCode();
    }

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
}
