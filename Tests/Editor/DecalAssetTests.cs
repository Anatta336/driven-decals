using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.TestTools;
using UnityEngine;
using UnityEngine.TestTools;

namespace SamDriver.Decal.Test
{
    public class DecalAssetTests
    {
        DecalAsset decalAsset;
        SerializedObject serializedDecalAsset;
        SerializedProperty uMin, vMin, uMax, vMax;

        [SetUp]
        public void PrepareDecalAsset()
        {
            decalAsset = ScriptableObject.CreateInstance<DecalAsset>();

            serializedDecalAsset = new SerializedObject(decalAsset);

            uMin = serializedDecalAsset.FindProperty("uMin");
            vMin = serializedDecalAsset.FindProperty("vMin");
            uMax = serializedDecalAsset.FindProperty("uMax");
            vMax = serializedDecalAsset.FindProperty("vMax");
        }

        [TearDown]
        public void DestroyDecalAsset()
        {
            Object.DestroyImmediate(decalAsset);
        }

        void UVtoZeroOne()
        {
            uMin.floatValue = 0f;
            vMin.floatValue = 0f;
            uMax.floatValue = 1f;
            vMax.floatValue = 1f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();
        }

        [Test]
        public void UMin_SetZero_Zero()
        {
            UVtoZeroOne();
            Assert.That(uMin.floatValue, Is.EqualTo(0f));
        }

        [Test]
        public void UMax_SetOne_One()
        {
            UVtoZeroOne();
            Assert.That(uMax.floatValue, Is.EqualTo(1f));
        }

        [Test]
        public void UMax_SetUMinHigherThanUMax_UMaxAlsoIncreased()
        {
            UVtoZeroOne();
            uMin.floatValue = 2f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(uMax.floatValue, Is.EqualTo(2f));
        }

        [Test]
        public void BoundsAsVector4_UVZeroOne_RecreatedInVector4()
        {
            UVtoZeroOne();

            Assert.That(decalAsset.BoundsAsVector4.x, Is.EqualTo(uMin.floatValue).Within(1).Ulps);
            Assert.That(decalAsset.BoundsAsVector4.y, Is.EqualTo(vMin.floatValue).Within(1).Ulps);
            Assert.That(decalAsset.BoundsAsVector4.z, Is.EqualTo(uMax.floatValue).Within(1).Ulps);
            Assert.That(decalAsset.BoundsAsVector4.w, Is.EqualTo(vMax.floatValue).Within(1).Ulps);
        }
        
        [Test]
        public void BoundsAsVector4_UVSmall_RecreatedInVector4()
        {
            uMin.floatValue = 0.5f;
            uMax.floatValue = 0.8f;
            vMin.floatValue = 0.2f;
            vMax.floatValue = 0.6f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(decalAsset.BoundsAsVector4.x, Is.EqualTo(uMin.floatValue).Within(1).Ulps);
            Assert.That(decalAsset.BoundsAsVector4.y, Is.EqualTo(vMin.floatValue).Within(1).Ulps);
            Assert.That(decalAsset.BoundsAsVector4.z, Is.EqualTo(uMax.floatValue).Within(1).Ulps);
            Assert.That(decalAsset.BoundsAsVector4.w, Is.EqualTo(vMax.floatValue).Within(1).Ulps);
        }
        
        [Test]
        public void UVWidth_UVZeroOne_WidthOne()
        {
            UVtoZeroOne();

            Assert.That(decalAsset.UVWidth, Is.EqualTo(1f));
        }

        [Test]
        public void UVHeight_UVZeroOne_HeightOne()
        {
            UVtoZeroOne();

            Assert.That(decalAsset.UVHeight, Is.EqualTo(1f));
        }

        [Test]
        public void UVWidth_UVOffsetZeroOne_WidthOne()
        {
            UVtoZeroOne();
            uMin.floatValue = 0.5f;
            uMax.floatValue = 1.5f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(decalAsset.UVWidth, Is.EqualTo(1f));
        }

        [Test]
        public void UVHeight_UVOffsetZeroOne_HeightOne()
        {
            UVtoZeroOne();
            vMin.floatValue = 0.5f;
            vMax.floatValue = 1.5f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(decalAsset.UVHeight, Is.EqualTo(1f).Within(1).Ulps);
        }
        
        [Test]
        public void UVWidth_UVSmall_WidthSmall()
        {
            uMin.floatValue = 0.5f;
            uMax.floatValue = 0.8f;
            vMin.floatValue = 0.2f;
            vMax.floatValue = 0.6f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(decalAsset.UVWidth, Is.EqualTo(0.3f).Within(1).Ulps);
        }
        
        [Test]
        public void UVHeight_UVSmall_HeightSmall()
        {
            uMin.floatValue = 0.5f;
            uMax.floatValue = 0.8f;
            vMin.floatValue = 0.2f;
            vMax.floatValue = 0.6f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(decalAsset.UVHeight, Is.EqualTo(0.4f).Within(1).Ulps);
        }

        [Test]
        public void HasAnyZeroSizedDimensions_NoZero_False()
        {
            UVtoZeroOne();

            Assert.That(decalAsset.HasAnyZeroSizedDimensions, Is.False);
        }

        [Test]
        public void HasAnyZeroSizedDimensions_ZeroWidth_True()
        {
            UVtoZeroOne();
            uMin.floatValue = 1f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(decalAsset.HasAnyZeroSizedDimensions, Is.True);
        }

        [Test]
        public void HasAnyZeroSizedDimensions_ZeroHeight_True()
        {
            UVtoZeroOne();
            vMin.floatValue = 1f;
            serializedDecalAsset.ApplyModifiedPropertiesWithoutUndo();
            serializedDecalAsset.Update();

            Assert.That(decalAsset.HasAnyZeroSizedDimensions, Is.True);
        }

        //TODO: would like to test availability of properties on Material, but
        // not sure how to make a fake material without needing a valid shader.
        // Could bundle a shader and material in the test directory..?

    }
}
