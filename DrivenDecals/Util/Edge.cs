namespace SamDriver.Decal
{
  internal struct Edge
  {
    public Vertex v0;
    public Vertex v1;

    public Edge(Vertex v0_, Vertex v1_)
    {
      this.v0 = v0_;
      this.v1 = v1_;
    }

    public Vertex GetVertexBetween(float tFromV0ToV1)
    {
      return Vertex.CreateFromExisting(v0, v1, tFromV0ToV1);
    }

    /// <summary>
    /// Find the scalar t such that using it to linearly interpolating
    /// from v0 to v1 would give goal value in specified dimension.
    /// </summary>
    public float InverseLerpDimension(float goal, Dimension dimension)
    {
      return (goal - v0.GetComponent(dimension)) / 
        (v1.GetComponent(dimension) - v0.GetComponent(dimension));
    }

    /// <summary>
    /// Linearly interpolates between v0 and v1, giving the value of just one dimension.
    /// v0 is at t=0, v1 is at t=1
    /// </summary>
    public float LerpDimension(float t, Dimension dimension)
    {
      return v0.GetComponent(dimension) + t * (v1.GetComponent(dimension) - v0.GetComponent(dimension));
    }

    /// <summary>
    /// T is a scalar representing proportion of displacement from v0 to v1
    /// </summary>
    public float GetTForProjectedX(float targetX)
    {
      return (targetX - v0.projectedDecalPosition.x) /
        (v1.projectedDecalPosition.x - v0.projectedDecalPosition.x);
    }

    /// <summary>
    /// T is a scalar representing proportion of displacement from v0 to v1
    /// </summary>
    public float GetTForProjectedY(float targetY)
    {
      return (targetY - v0.projectedDecalPosition.y) /
        (v1.projectedDecalPosition.y - v0.projectedDecalPosition.y);
    }

    /// <summary>
    /// T is a scalar representing proportion of displacement from v0 to v1
    /// </summary>
    public float GetTForProjectedZ(float targetZ)
    {
      return (targetZ - v0.projectedDecalPosition.z) /
        (v1.projectedDecalPosition.z - v0.projectedDecalPosition.z);
    }


    /// <summary>
    /// Linearly interpolates between the projected x components of v0 and v1.
    /// v0 is at t=0, v1 is at t=1
    /// </summary>
    public float GetProjectedXForT(float t)
    {
      return v0.projectedDecalPosition.x + t * (v1.projectedDecalPosition.x - v0.projectedDecalPosition.x);
    }

    /// <summary>
    /// Linearly interpolates between the projected y components of v0 and v1.
    /// v0 is at t=0, v1 is at t=1
    /// </summary>
    public float GetProjectedYForT(float t)
    {
      return v0.projectedDecalPosition.y + t * (v1.projectedDecalPosition.y - v0.projectedDecalPosition.y);
    }

    /// <summary>
    /// Linearly interpolates between the projected z components of v0 and v1.
    /// v0 is at t=0, v1 is at t=1
    /// </summary>
    public float GetProjectedZForT(float t)
    {
      return v0.projectedDecalPosition.z + t * (v1.projectedDecalPosition.z - v0.projectedDecalPosition.z);
    }
  }
}