using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SamDriver.Decal
{
  internal static class Square
  {
    public static int CornerCount = 4;

    static Vector2[] _centeredUnitSquareCorners;
    public static Vector2[] CenteredUnitSquareCorners
    {
      get
      {
        if (_centeredUnitSquareCorners == null || _centeredUnitSquareCorners.Length == 0)
        {
          _centeredUnitSquareCorners = SquareCorners(0f, 0f, 1f).ToArray();
          if (_centeredUnitSquareCorners.Length != CornerCount)
          {
            throw new System.Exception($"{nameof(SquareCorners)} expected to give {CornerCount} values, but gave {_centeredUnitSquareCorners.Length}.");
          }
        }
        return _centeredUnitSquareCorners;
      }
    }

    public static IEnumerable<Vector2> SquareCorners(float midX, float midY, float size)
    {
      yield return new Vector2(midX - size * 0.5f, midY - size * 0.5f);
      yield return new Vector2(midX + size * 0.5f, midY - size * 0.5f);
      yield return new Vector2(midX + size * 0.5f, midY + size * 0.5f);
      yield return new Vector2(midX - size * 0.5f, midY + size * 0.5f);
    }
  }
}