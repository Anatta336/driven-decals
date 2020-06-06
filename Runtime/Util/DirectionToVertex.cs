namespace SamDriver.Decal
{
    /// <summary>
    /// Immutable representation of difference between two points.
    /// </summary>
    internal struct DirectionToVertex
    {
        public readonly Vertex start;
        public readonly Vertex target;
        public readonly Float3 direction;
        public readonly float distancetoTarget;

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
