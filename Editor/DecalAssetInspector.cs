using System.IO;
using UnityEditor;
using UnityEngine;

namespace SamDriver.Decal
{
    [CustomEditor(typeof(DecalAsset))]
    [CanEditMultipleObjects]
    public class DecalAssetInspector : Editor
    {
        const string editorResourcesPath = "Packages/com.samdriver.driven-decals/Editor/Resources/";
        static Material FetchEditorMaterial(string materialName)
        {
            var path = Path.Combine(editorResourcesPath, $"Materials/{materialName}.mat");
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null) throw new FileNotFoundException($"Couldn't find material file at {path}");
            return material;
        }

        Material _thumbnailMaterial;
        Material thumbnailMaterial
        {
            get
            {
                if (_thumbnailMaterial == null)
                {
                    _thumbnailMaterial = FetchEditorMaterial("EditorDecalThumbnail");
                }
                return _thumbnailMaterial;
            }
        }

        Material _overlayBoundsMaterial;
        Material overlayBoundsMaterial
        {
            get
            {
                if (_overlayBoundsMaterial == null)
                {
                    _overlayBoundsMaterial = FetchEditorMaterial("EditorDecalOverlayBounds");
                }
                return _overlayBoundsMaterial;
            }
        }

        static int colourForAlphaID = Shader.PropertyToID("_ColourForAlpha");
        static int boundsID = Shader.PropertyToID("_Bounds");

        static string[] essentialMaterialProperties = new string[] {
            "_DiffuseAlpha",
            "_Bounds",
        };
        static string[] supportedMaterialProperties = new string[] {
            "_Opacity",
            "_FlipU",
            "_FlipV",
            "_ZFadeStart",
            "_MinAngleFadeRadians",
            "_MaxAngleFadeRadians",
        };

        static Color previewBackColour = Color.white;
        static float maxPreviewHeight = 520f;

        SerializedProperty material;
        SerializedProperty uMin, vMin, uMax, vMax;

        void OnEnable()
        {
            material = serializedObject.FindProperty(nameof(DecalAsset.Material));

            // can't use nameof here as these properties aren't public
            uMin = serializedObject.FindProperty("uMin");
            vMin = serializedObject.FindProperty("vMin");
            uMax = serializedObject.FindProperty("uMax");
            vMax = serializedObject.FindProperty("vMax");
        }

        public override void OnInspectorGUI()
        {
            bool isEditingMultipleObjects = (targets != null && targets.Length > 1);
            serializedObject.Update();

            MaterialPickerGUI();

            if (!isEditingMultipleObjects)
            {
                PreviewSettingsGUI();
                PreviewAndRegionSelectionGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        void MaterialPickerGUI()
        {
            EditorGUILayout.PropertyField(material, new GUIContent("Material"));

            var selectedMaterial = (Material)material.objectReferenceValue;
            if (selectedMaterial == null) return;

            // warn about missing properties
            foreach (string propertyName in essentialMaterialProperties)
            {
                if (!selectedMaterial.HasProperty(propertyName))
                {
                    EditorGUILayout.HelpBox($"{selectedMaterial.name} doesn't have a {propertyName} property, which is required for use as a decal material.",
                      MessageType.Error);
                }
            }
            foreach (string propertyName in supportedMaterialProperties)
            {
                if (!selectedMaterial.HasProperty(propertyName))
                {
                    EditorGUILayout.HelpBox($"{selectedMaterial.name} doesn't have a {propertyName} property, so some decal features may not function.",
                      MessageType.Warning);
                }
            }
        }

        void PreviewSettingsGUI()
        {
            previewBackColour = EditorGUILayout.ColorField("Preview background:", previewBackColour);
        }

        void PreviewAndRegionSelectionGUI()
        {
            Texture2D previewTexture = ((DecalAsset)target).diffuseAlpha;
            if (previewTexture == null) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                float nudgeButtonSize = 20f;
                var nudgeButtonParams = new GUILayoutOption[] { GUILayout.Width(nudgeButtonSize), GUILayout.Height(nudgeButtonSize) };
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(nudgeButtonSize)))
                {
                    if (GUILayout.RepeatButton("↑", nudgeButtonParams))
                    {
                        vMax.floatValue += 1f / previewTexture.height;
                    }
                    if (GUILayout.RepeatButton("↓", nudgeButtonParams))
                    {
                        vMax.floatValue -= 1f / previewTexture.height;
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.RepeatButton("↑", nudgeButtonParams))
                    {
                        vMin.floatValue += 1f / previewTexture.height;
                    }
                    if (GUILayout.RepeatButton("↓", nudgeButtonParams))
                    {
                        vMin.floatValue -= 1f / previewTexture.height;
                    }
                    GUILayout.Space(nudgeButtonSize + 4f);
                }

                using (new EditorGUILayout.VerticalScope(GUILayout.Width(nudgeButtonSize)))
                {
                    // not great to be using hardcoded, but can't find an exposed property.
                    float inspectorPaddingLeft = 18f;
                    float inspectorPaddingRight = 6f;
                    float availableWidth = EditorGUIUtility.currentViewWidth - nudgeButtonSize - inspectorPaddingLeft - inspectorPaddingRight;

                    // make a region that matches ratio of texture and scales to fit availableWidth and maxPreviewHeight
                    float widthOverHeight = (float)previewTexture.width / (float)previewTexture.height;
                    float width = availableWidth;
                    float height = Mathf.Min(maxPreviewHeight, availableWidth / widthOverHeight);
                    width = Mathf.Min(availableWidth, height * widthOverHeight);

                    // DrawPreviewTexture not available directly in *GUILayout, so separately reserve space then use it
                    var previewRegion = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
                      new GUILayoutOption[] { GUILayout.Width(width), GUILayout.Height(height) }
                    );

                    overlayBoundsMaterial.SetVector(boundsID, ((DecalAsset)target).BoundsAsVector4);
                    overlayBoundsMaterial.SetColor(colourForAlphaID, previewBackColour);
                    EditorGUI.DrawPreviewTexture(previewRegion, previewTexture, overlayBoundsMaterial);

                    HandleMouse(previewRegion);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.RepeatButton("←", nudgeButtonParams))
                        {
                            uMin.floatValue -= 1f / previewTexture.width;
                        }
                        if (GUILayout.RepeatButton("→", nudgeButtonParams))
                        {
                            uMin.floatValue += 1f / previewTexture.width;
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.RepeatButton("←", nudgeButtonParams))
                        {
                            uMax.floatValue -= 1f / previewTexture.width;
                        }
                        if (GUILayout.RepeatButton("→", nudgeButtonParams))
                        {
                            uMax.floatValue += 1f / previewTexture.width;
                        }
                    }
                }
            }

            // use a small label size for these fields
            EditorGUIUtility.labelWidth = 40f;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.DelayedFloatField(uMin, new GUIContent("uMin"));
                EditorGUILayout.DelayedFloatField(uMax, new GUIContent("uMax"));
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.DelayedFloatField(vMin, new GUIContent("vMin"));
                EditorGUILayout.DelayedFloatField(vMax, new GUIContent("vMax"));
            }
        }

        Vector2 mouseStartOnTexture;
        bool isDraggingRegion = false;
        void HandleMouse(Rect region)
        {
            Event e = Event.current;
            if (!e.isMouse) return;

            Vector2 positionInRegion = e.mousePosition - region.min;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                // left mouse button down
                if (positionInRegion.x > 0f && positionInRegion.y > 0f &&
                  positionInRegion.x < region.width && positionInRegion.y < region.height)
                {
                    mouseStartOnTexture = PositionOnTextureInUV(region, positionInRegion);
                    isDraggingRegion = true;
                }
            }
            else if (isDraggingRegion &&
          (e.type == EventType.MouseDrag || (e.type == EventType.MouseUp && e.button == 0)))
            {
                // drag or left mouse button up
                Vector2 positionOnTexture = PositionOnTextureInUV(region, positionInRegion);
                SetBoundsByCorners(mouseStartOnTexture, positionOnTexture);
                Repaint();

                if (e.type == EventType.MouseUp)
                {
                    isDraggingRegion = false;
                }
            }
        }

        void SetBoundsByCorners(Vector2 cornerA, Vector2 cornerB)
        {
            uMin.floatValue = Mathf.Min(cornerA.x, cornerB.x);
            vMin.floatValue = Mathf.Min(cornerA.y, cornerB.y);
            uMax.floatValue = Mathf.Max(cornerA.x, cornerB.x);
            vMax.floatValue = Mathf.Max(cornerA.y, cornerB.y);
        }

        Vector2 PositionOnTextureInUV(Rect displayRegion, Vector2 positionInRegion)
        {
            return new Vector2(
          positionInRegion.x / displayRegion.width,
          1f - (positionInRegion.y / displayRegion.height)
            );
        }

        /// <summary>
        /// RenderStaticPreview is poorly documented. Nonetheless this produces a nice
        /// square thumbnail preview of the decal.
        /// </summary>
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets,
        int width, int height)
        {
            DecalAsset decalAsset = (DecalAsset)target;
            if (decalAsset == null || !decalAsset.HasDiffuseAlphaTexture)
            {
                return null;
            }

            PreviewRenderUtility utility = new PreviewRenderUtility();
            utility.BeginStaticPreview(new Rect(0, 0, width, height));

            // Normally would set up a little scene within PreviewRenderUtility, then render that manually
            // useful links:
            // https://github.com/raphael-ernaelsten/Texture3DPreview-for-Unity/blob/master/Assets/Texture3DPreview/Editor/Extensions/Texture3DExtensions.cs
            // http://answers.unity.com/answers/1650714/view.html
            // for example:
            // utility.camera.transform.position = Vector2.zero;
            // utility.camera.transform.position = utility.camera.transform.forward * -distance;
            // utility.camera.backgroundColor = Color.red;
            // utility.DrawMesh(someMesh, Matrix4x4.identity, someMaterial, 0);
            // utility.camera.Render();

            // But in this case we just blit to camera's target
            thumbnailMaterial.SetVector(boundsID, decalAsset.BoundsAsVector4);
            thumbnailMaterial.SetColor(colourForAlphaID, previewBackColour);
            Graphics.Blit(decalAsset.diffuseAlpha, utility.camera.targetTexture, thumbnailMaterial, 0);

            // EndStaticPreview() gives us the texture generated by utility.camera
            // (for some reason returning a texture made this way works, returning a texture I make doesn't)
            var result = utility.EndStaticPreview();
            utility.Cleanup();

            return result;
        }
    }
}
