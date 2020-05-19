using UnityEngine;

namespace SamDriver.Decal
{
  public class DecalException : UnityException
  {
    public DecalException() : base() {}
    public DecalException(string message) : base(message) {}
  }
}
