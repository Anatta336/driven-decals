using NUnit.Framework;
using UnityEngine;

namespace SamDriver.Decal.Test
{
    public class TriangleTests
    {
        // reminder you can mark a function with [SetUp] or [TearDown] to automatically
        // call it before or after each function marked with [Test]

        static Triangle BuildTriangle(Float3 posA, Float3 posB, Float3 posC, Float3 vertexNormal)
        {
            Vertex vertexA = new Vertex(posA, vertexNormal);
            Vertex vertexB = new Vertex(posB, vertexNormal);
            Vertex vertexC = new Vertex(posC, vertexNormal);
            return new Triangle(vertexA, vertexB, vertexC);
        }

        static Triangle PlanarXYTriangle()
        {
            return BuildTriangle(
                new Float3(0f, 0f, 0f),
                new Float3(1f, 0f, 0f),
                new Float3(0f, 1f, 0f),
                new Float3(1f, 0f, 0f)
            );
        }

        static Triangle LiftedFromXYTriangle()
        {
            return BuildTriangle(
                new Float3(0f, 0f, 0f),
                new Float3(1f, 0f, 1f),
                new Float3(0f, 1f, 0f),
                new Float3(1f, 0f, 0f)
            );
        }

        [Test]
        public void GeometryNormal_XYPlane_UnitVectorOnZAxis()
        {
            Triangle triangle = PlanarXYTriangle();

            Assert.That(triangle.GeometryNormal.x, Is.EqualTo(0f));
            Assert.That(triangle.GeometryNormal.y, Is.EqualTo(0f));
            Assert.That(triangle.GeometryNormal.z, Is.EqualTo(1f));
        }

        [Test]
        public void GeometryNormal_Nonplanar_OrthogonalUnitVector()
        {
            Triangle triangle = LiftedFromXYTriangle();

            float halfSqrtTwo = 0.5f * Mathf.Sqrt(2f);
            Assert.That(triangle.GeometryNormal.x, Is.EqualTo(-halfSqrtTwo));
            Assert.That(triangle.GeometryNormal.y, Is.EqualTo(0f));
            Assert.That(triangle.GeometryNormal.z, Is.EqualTo(halfSqrtTwo));
        }

        [Test]
        public void GetZAtXY_AtOriginCorner_Zero()
        {
            Triangle triangle = LiftedFromXYTriangle();

            Assert.That(triangle.GetZAtXY(0f, 0f), Is.EqualTo(0f));
        }

        [Test]
        public void GetZAtXY_AtLiftedCorner_One()
        {
            Triangle triangle = LiftedFromXYTriangle();

            Assert.That(triangle.GetZAtXY(1f, 0f), Is.EqualTo(1f));
        }
        
        [Test]
        public void GetZAtXY_HalfWayAlongEdgeTowardsLiftedCorner_Half()
        {
            Triangle triangle = LiftedFromXYTriangle();

            Assert.That(triangle.GetZAtXY(0.5f, 0f), Is.EqualTo(0.5f));
        }

        [Test]
        public void GetZAtXY_HalfWayAlongBodyTowardsLiftedCorner_Half()
        {
            Triangle triangle = LiftedFromXYTriangle();

            Assert.That(triangle.GetZAtXY(0.5f, 0.1f), Is.EqualTo(0.5f));
        }

        //TODO: should repeat above set with XAtYZ and YAtXZ

        [Test]
        public void IsAxialZLineWithin_InsidePlanarTriangle_True()
        {
            Triangle triangle = PlanarXYTriangle();
            
            Assert.That(triangle.IsAxialZLineWithin(0.2f, 0.2f), Is.True);
        }

        [Test]
        public void IsAxialZLineWithin_OutsidePlanarTriangle_False()
        {
            Triangle triangle = PlanarXYTriangle();
            
            Assert.That(triangle.IsAxialZLineWithin(0.8f, 0.8f), Is.False);
        }

        [Test]
        public void IsAxialZLineWithin_InsideLiftedTriangle_True()
        {
            Triangle triangle = LiftedFromXYTriangle();
            
            Assert.That(triangle.IsAxialZLineWithin(0.2f, 0.2f), Is.True);
        }

        [Test]
        public void IsAxialZLineWithin_OutsideLiftedTriangle_False()
        {
            Triangle triangle = LiftedFromXYTriangle();
            
            Assert.That(triangle.IsAxialZLineWithin(0.8f, 0.8f), Is.False);
        }

    }
}
