using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SamDriver.Decal
{
  internal class Triangle : IEnumerable<Vertex>
  {
    protected Vertex[] vertices;

    // indexer for array access
    public Vertex this[int vertexIndex]
    {
      get => vertices[vertexIndex];
    }

    public IEnumerable<Edge> GetEdges()
    {
      Edge[] edges = new Edge[vertices.Length];
      for (int i = 0; i < edges.Length; ++i)
      {
        edges[i] = new Edge(vertices[i], vertices[(i + 1) % vertices.Length]);
      }
      
      return edges.AsEnumerable();
    }
    
    public IEnumerator<Vertex> GetEnumerator()
    {
      for (int i = 0; i < vertices.Length; ++i)
      {
        yield return vertices[i];
      }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// The normal of the plane defined by this triangle. Note this may not be equal to the
    /// normal stored on the vertices that make up this triangle. In addition the vertices may
    /// each have different normals.
    /// </summary>
    public Vector3 GeometryNormal
    {
      get
      {
        if (!_normal.HasValue)
        {
          _normal = Vector3.Cross(
            vertices[1].decalPosition - vertices[0].decalPosition,
            vertices[2].decalPosition - vertices[0].decalPosition
          ).normalized;
        }
        return _normal.Value;
      }
    }
    Vector3? _normal;

    public Triangle(Vertex v0, Vertex v1, Vertex v2)
    {
      vertices = new Vertex[] { v0, v1, v2 };
    }

    /// <summary>
    /// Given an yz position find z on the plane defined by this triangle.
    /// </summary>
    public float getXAtYZ(float y, float z)
    {
      // we have a plane defined by this triangle, and just need to find y given xz
      return (Vector3.Dot(GeometryNormal, vertices[0].decalPosition) - GeometryNormal.y * y - GeometryNormal.z * z) / GeometryNormal.x;
    }

    /// <summary>
    /// Given an xz position find y on the plane defined by this triangle.
    /// </summary>
    public float getYAtXZ(float x, float z)
    {
      // we have a plane defined by this triangle, and just need to find y given xz
      return (Vector3.Dot(GeometryNormal, vertices[0].decalPosition) - GeometryNormal.x * x - GeometryNormal.z * z) / GeometryNormal.y;
    }

    /// <summary>
    /// Given an xy position find z on the plane defined by this triangle.
    /// </summary>
    public float GetZAtXY(float x, float y)
    {
      // we have a plane defined by this triangle, and just need to find z given xy
      return (Vector3.Dot(GeometryNormal, vertices[0].decalPosition) - GeometryNormal.x * x - GeometryNormal.y * y) / GeometryNormal.z;
    }

    /// <summary>
    /// Tests if a line defined by a y and z coordinate pair intersects this triangle.
    /// Equivalent to projecting the triangle onto the yz plane and testing for a point being inside.
    /// </summary>
    public bool IsAxialXLineWithin(float y, float z)
    {
      // projecting the triangle onto yz plane just means setting x components to zero
      Vector3 projectedA = new Vector3(0f, vertices[0].decalPosition.y, vertices[0].decalPosition.z);
      Vector3 projectedB = new Vector3(0f, vertices[1].decalPosition.y, vertices[1].decalPosition.z);
      Vector3 projectedC = new Vector3(0f, vertices[2].decalPosition.y, vertices[2].decalPosition.z);
      Vector3 P = new Vector3(0f, y, z);

      return IsProjectedVertexWithinTriangle(projectedA, projectedB, projectedC, P);
    }

    /// <summary>
    /// Tests if a line defined by a x and z coordinate pair intersects this triangle.
    /// Equivalent to projecting the triangle onto the xz plane and testing for a point being inside.
    /// </summary>
    public bool IsAxialYLineWithin(float x, float z)
    {
      // projecting the triangle onto xz plane just means setting y components to zero
      Vector3 projectedA = new Vector3(vertices[0].decalPosition.x, 0f, vertices[0].decalPosition.z);
      Vector3 projectedB = new Vector3(vertices[1].decalPosition.x, 0f, vertices[1].decalPosition.z);
      Vector3 projectedC = new Vector3(vertices[2].decalPosition.x, 0f, vertices[2].decalPosition.z);
      Vector3 P = new Vector3(x, 0f, z);

      return IsProjectedVertexWithinTriangle(projectedA, projectedB, projectedC, P);
    }

    /// <summary>
    /// Tests if a line defined by a x and y coordinate pair intersects this triangle.
    /// Equivalent to projecting the triangle onto the xy plane and testing for a point being inside.
    /// </summary>
    public bool IsAxialZLineWithin(float x, float y)
    {
      // projecting the triangle onto xy plane just means setting z components to zero
      Vector3 projectedA = new Vector3(vertices[0].decalPosition.x, vertices[0].decalPosition.y, 0f);
      Vector3 projectedB = new Vector3(vertices[1].decalPosition.x, vertices[1].decalPosition.y, 0f);
      Vector3 projectedC = new Vector3(vertices[2].decalPosition.x, vertices[2].decalPosition.y, 0f);
      Vector3 P = new Vector3(x, y, 0f);

      return IsProjectedVertexWithinTriangle(projectedA, projectedB, projectedC, P);
    }

    /// <summary>
    /// Note! Assumes the given point is within the triangle.
    /// </summary>
    public Vector3 InterpolateNormal(Vector3 targetPoint)
    {
      // given triangle ABC and point P
      Vector3 A = vertices[0].decalPosition;
      Vector3 B = vertices[1].decalPosition;
      Vector3 C = vertices[2].decalPosition;

      Vector3 u = B - A;
      Vector3 v = C - A;
      Vector3 n = Vector3.Cross(u, v);
      Vector3 w = targetPoint - A;

      // barycentric coordinates of the projection P' of P to plane of triangle
      float nMagnitudeSquared = n.sqrMagnitude;
      float gamma = Vector3.Dot(Vector3.Cross(u, w), n) / nMagnitudeSquared;
      float beta = Vector3.Dot(Vector3.Cross(w, v), n) / nMagnitudeSquared;
      float alpha = 1f - gamma - beta;

      //TODO: check that the maths of this holds up
      // normal = α*normalA + β*normalB + (1 - α - β)*normalC
      // so long as point is within triangle, γ = (1 - α - β)
      Vector3 interpolated = alpha * vertices[0].decalNormal +
        beta * vertices[1].decalNormal +
        gamma * vertices[2].decalNormal;
      interpolated.Normalize();
      return interpolated;
    }

    public static bool IsProjectedVertexWithinTriangle(
      Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
      // given triangle ABC and point P
      Vector3 u = B - A;
      Vector3 v = C - A;
      Vector3 n = Vector3.Cross(u, v);
      Vector3 w = P - A;

      // barycentric coordinates of the projection P' of P to plane of triangle
      float nMagnitudeSquared = n.sqrMagnitude;
      float gamma = Vector3.Dot(Vector3.Cross(u, w), n) / nMagnitudeSquared;
      float beta = Vector3.Dot(Vector3.Cross(w, v), n) / nMagnitudeSquared;

      //NOTE: alpha is only a correct barycentric coordinate if point is within triangle
      float alpha = 1f - gamma - beta;

      // P' is within triangle if barycentric coordinates are all in range 0 to 1
      return (alpha >= 0f) && (alpha <= 1f) &&
        (beta >= 0f) && (beta <= 1f) &&
        (gamma >= 0f) && (gamma <= 1f);
    }
  }
}
