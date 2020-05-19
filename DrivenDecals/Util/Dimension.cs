using System.Collections.Generic;

namespace SamDriver.Decal
{
  internal enum Dimension
  {
    x,
    y,
    z,
  }

  internal static class DimensionHelper
  {
    public static IEnumerable<Dimension> Enumerate()
    {
      yield return Dimension.x;
      yield return Dimension.y;
      yield return Dimension.z;
    }
  }
}