using Unity.Collections;

namespace SamDriver.Decal
{
  internal struct Float2
  {
    [ReadOnly]
    public readonly float x, y;

    public Float2(float x_, float y_)
    {
      x = x_;
      y = y_;
    }
  }
}