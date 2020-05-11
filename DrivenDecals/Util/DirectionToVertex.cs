using UnityEngine;

namespace SamDriver.Decal
{
  internal struct DirectionToVertex
  {
    public Vertex start;
    public Vertex target;
    public Vector3 direction;
    public float distancetoTarget;

    public DirectionToVertex(Vertex start_, Vertex target_)
    {
      this.start = start_;
      this.target = target_;

      Vector3 displacement = target_.decalPosition - start_.decalPosition;
      this.distancetoTarget = displacement.magnitude;
      this.direction = displacement / distancetoTarget;
    }

    public bool RoughlyParallel(DirectionToVertex other, float fuzziness = 0.001f)
    {
      return VectorRoughlyEqual(this.direction, other.direction, fuzziness) ||
        VectorRoughlyEqual(this.direction, other.direction * -1f, fuzziness);
    }

    public bool RoughlyEqualDirection(DirectionToVertex other, float fuzziness = 0.001f)
    {
      return VectorRoughlyEqual(this.direction, other.direction, fuzziness);
    }

    static bool VectorRoughlyEqual(Vector3 a, Vector3 b, float fuzziness = 0.001f)
    {
      return Mathf.Abs(a.x - b.x) < fuzziness &&
        Mathf.Abs(a.y - b.y) < fuzziness &&
        Mathf.Abs(a.z - b.z) < fuzziness;
    }
  }
}
