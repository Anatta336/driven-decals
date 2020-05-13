using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SamDriver.Decal
{
  internal static class Square
  {
    public static int CornerCount = 4;

    static Float2[] _centeredUnitSquareCorners;
    public static Float2[] CenteredUnitSquareCorners
    {
      get
      {
        if (_centeredUnitSquareCorners == null || _centeredUnitSquareCorners.Length == 0)
        {
          _centeredUnitSquareCorners = SquareCorners(0f, 0f, 1f).ToArray();
          if (_centeredUnitSquareCorners.Length != CornerCount)
          {
            throw new DecalException($"{nameof(SquareCorners)} expected to give {CornerCount} values, but gave {_centeredUnitSquareCorners.Length}.");
          }
        }
        return _centeredUnitSquareCorners;
      }
    }

    public static IEnumerable<Float2> SquareCorners(float midX = 0f, float midY = 0f, float size = 1f)
    {
      yield return new Float2(midX - size * 0.5f, midY - size * 0.5f);
      yield return new Float2(midX + size * 0.5f, midY - size * 0.5f);
      yield return new Float2(midX + size * 0.5f, midY + size * 0.5f);
      yield return new Float2(midX - size * 0.5f, midY + size * 0.5f);
    }
  }
}