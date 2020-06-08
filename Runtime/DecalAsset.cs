using System;
using UnityEngine;

namespace SamDriver.Decal
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Decal", menuName = "Decal/Decal Asset")]
    /// <summary>
    /// Describes the location of a particular decal within the texture(s) of a material.
    /// </summary>
    public class DecalAsset : ScriptableObject
    {
        static int diffuseAlphaID = Shader.PropertyToID("_DiffuseAlpha");

        public Material Material;

        // bounds in uv space
        [SerializeField] float uMin = 0f, vMin = 0f, uMax = 1f, vMax = 1f;

        // this block are all set by UpdateDerivedValues()

        /// <summary>
        /// .x is uMin, .y is vMin, .z is uMax, .w is vMax
        /// </summary>
        public Vector4 BoundsAsVector4 { get; private set; }
        public float UVWidth { get; private set; }
        public float UVHeight { get; private set; }
        public float UVWidthDividedByHeight { get; private set; }
        public bool HasAnyZeroSizedDimensions { get; private set; }

        public bool HasDiffuseAlphaTexture
        {
            get => (Material != null && Material.HasProperty(diffuseAlphaID));
        }
        public Texture2D diffuseAlpha
        {
            get => HasDiffuseAlphaTexture ? (Texture2D)Material.GetTexture(diffuseAlphaID) : null;
        }
        public float TexelsWidth
        {
            get
            {
                if (!HasDiffuseAlphaTexture) return 0f;
                Texture2D diffuseAlpha = (Texture2D)Material.GetTexture(diffuseAlphaID);
                return UVWidth * diffuseAlpha.width;
            }
        }
        public float TexelsHeight
        {
            get
            {
                if (!HasDiffuseAlphaTexture) return 0f;
                Texture2D diffuseAlpha = (Texture2D)Material.GetTexture(diffuseAlphaID);
                return UVHeight * diffuseAlpha.height;
            }
        }

        void OnEnable()
        {
            UpdateDerivedValues();
        }

        void OnValidate()
        {
            uMax = Mathf.Max(uMin, uMax);
            vMax = Mathf.Max(vMin, vMax);
            UpdateDerivedValues();
        }

        void UpdateDerivedValues()
        {
            BoundsAsVector4 = new Vector4(uMin, vMin, uMax, vMax);
            UVWidth = uMax - uMin;
            UVHeight = vMax - vMin;
            UVWidthDividedByHeight = UVWidth / UVHeight;
            HasAnyZeroSizedDimensions = (Mathf.Approximately(UVWidth, 0f) || Mathf.Approximately(UVHeight, 0f));
        }
    }
}
