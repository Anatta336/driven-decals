using UnityEngine;

namespace SamDriver.Decal
{
  internal struct DirectionToVertex
  {
    public Vertex start;
    public Vertex target;
    public Float3 direction;
    public float distancetoTarget;

    public DirectionToVertex(Vertex start_, Vertex target_)
    {
      this.start = start_;
      this.target = target_;

      var displacement = target_.Position - start_.Position;
      this.distancetoTarget = displacement.Magnitude;
      this.direction = displacement / distancetoTarget;
    }

    public bool RoughlyParallel(DirectionToVertex other, float fuzziness = 0.001f)
    {
      return this.direction.IsNearlyEqual(other.direction, fuzziness) ||
        this.direction.IsNearlyEqual(other.direction * -1f, fuzziness);
    }

    public bool RoughlyEqualDirection(DirectionToVertex other, float fuzziness = 0.001f)
    {
      return this.direction.IsNearlyEqual(other.direction, fuzziness);
    }
  }
}
