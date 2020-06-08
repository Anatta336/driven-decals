using UnityEngine;
using UnityEditor;

namespace SamDriver.Decal
{
    [CustomEditor(typeof(DecalMesh))]
    [CanEditMultipleObjects]
    public class DecalMeshInspector : Editor
    {
        SerializedProperty decalAsset, opacity, zFadeDistance, isFlipU, isFlipV,
          minAngleFadeDegrees, maxAngleFadeDegrees, drawOrder,
          shouldUseAllSceneStaticMeshes, meshesToProjectAgainst,
          shouldReprojectOnMove;

        void OnEnable()
        {
            decalAsset = serializedObject.FindProperty(nameof(DecalMesh.DecalAsset));
            opacity = serializedObject.FindProperty(nameof(DecalMesh.Opacity));
            zFadeDistance = serializedObject.FindProperty(nameof(DecalMesh.ZFadeDistance));
            isFlipU = serializedObject.FindProperty(nameof(DecalMesh.IsFlipU));
            isFlipV = serializedObject.FindProperty(nameof(DecalMesh.IsFlipV));
            minAngleFadeDegrees = serializedObject.FindProperty(nameof(DecalMesh.MinAngleFadeDegrees));
            maxAngleFadeDegrees = serializedObject.FindProperty(nameof(DecalMesh.MaxAngleFadeDegrees));
            drawOrder = serializedObject.FindProperty(nameof(DecalMesh.DrawOrder));
            shouldUseAllSceneStaticMeshes = serializedObject.FindProperty(nameof(DecalMesh.ShouldUseSceneStaticMeshes));
            meshesToProjectAgainst = serializedObject.FindProperty(nameof(DecalMesh.MeshesToProjectAgainst));
            shouldReprojectOnMove = serializedObject.FindProperty(nameof(DecalMesh.ShouldReprojectOnMove));
        }

        public override void OnInspectorGUI()
        {
            bool isEditingMultipleObjects = (targets != null && targets.Length > 1);

            var items = new DecalMesh[targets.Length];
            for (int i = 0; i < targets.Length; ++i)
            {
                items[i] = (DecalMesh)targets[i];
            }
            var primaryItem = (DecalMesh)target;
            serializedObject.Update();

            ProjectMeshButtonGUI(primaryItem, items, isEditingMultipleObjects);
            ResetMeshButtonGUI(primaryItem, items, isEditingMultipleObjects);
            ScaleToMatchButtonGUI(primaryItem, items, isEditingMultipleObjects);
            DecalPickerGUI(primaryItem, items, isEditingMultipleObjects);
            OpacityOptionsGUI();
            DrawOrderOptionsGUI();
            FlipOptionsGUI();
            TargetMeshesGUI();

            serializedObject.ApplyModifiedProperties();

            Repaint();

            // bit dubious doing this call from within OnInspectorGUI, but with Repaint() should get called often enough
            foreach (var decal in items)
            {
                if (decal.ShouldReprojectOnMove &&
                  decal.HasTransformChangedSinceProjection &&
                  !decal.IsAwaitingProjectionResult)
                {
                    decal.GenerateProjectedMeshImmediate();
                }
            }
        }

        void ProjectMeshButtonGUI(DecalMesh primaryItem, DecalMesh[] items, bool isEditingMultipleObjects)
        {
            bool canAnyTargetProject = false;
            foreach (var item in items)
            {
                if (item.HasMeshToProjectAgainst)
                {
                    canAnyTargetProject = true;
                    break;
                }
            }

            using (new EditorGUI.DisabledScope(!canAnyTargetProject))
            {
                using (EditorGUIUtility.wideMode ? new EditorGUILayout.HorizontalScope() : null)
                {
                    if (GUILayout.Button("Project mesh"))
                    {
                        foreach (var item in items)
                        {
                            if (item.HasMeshToProjectAgainst)
                            {
                                item.GenerateProjectedMeshImmediate();
                            }
                        }
                    }
                    var previousWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 80f;
                    EditorGUILayout.PropertyField(shouldReprojectOnMove,
                      new GUIContent("Auto-Repeat", "Automatically attempt to project mesh when the decal is moved in the editor."));
                    EditorGUIUtility.labelWidth = previousWidth;
                }
            }
            if (!canAnyTargetProject)
            {
                if (shouldUseAllSceneStaticMeshes.boolValue)
                {
                    EditorGUILayout.HelpBox("Cannot project mesh until the decal is placed close to a static mesh in the scene, or \"All static meshes\" is unticked and at least one target mesh is selected.",
                      MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot project mesh until at least one target mesh has been selected.",
                      MessageType.Warning);
                }
            }
            if (!isEditingMultipleObjects && primaryItem.IsGeneratedMeshEmpty)
            {
                if (shouldUseAllSceneStaticMeshes.boolValue)
                {
                    EditorGUILayout.HelpBox("Decal mesh is currently empty so the decal is invisible.\nPlace this object so that the bounding box intersects at least one static mesh in the scene then click \"Generate Mesh\"",
                      MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("Decal mesh is currently empty so the decal is invisible.\nPlace this object so that the bounding box intersects at least one targetted mesh then click \"Generate Mesh\"",
                      MessageType.Warning);
                }
            }
        }

        void ResetMeshButtonGUI(DecalMesh primaryItem, DecalMesh[] items, bool isEditingMultipleObjects)
        {
            if (GUILayout.Button("Reset mesh"))
            {
                foreach (var item in items)
                {
                    item.GenerateSimpleQuadMesh();
                }
            }
        }

        void ScaleToMatchButtonGUI(DecalMesh primaryItem, DecalMesh[] items, bool isEditingMultipleObjects)
        {
            bool canAnyPerformScale = false;
            foreach (var item in items)
            {
                if (item.CanScaleToMatchDecal)
                {
                    canAnyPerformScale = true;
                    break;
                }
            }
            using (new EditorGUI.DisabledScope(!canAnyPerformScale))
            {
                if (GUILayout.Button("Scale to match decal shape"))
                {
                    foreach (var item in items)
                    {
                        if (item.CanScaleToMatchDecal)
                        {
                            item.ScaleToMatchDecalBoundsRatio();
                        }
                    }
                }
            }
            if (!isEditingMultipleObjects && !canAnyPerformScale)
            {
                if (primaryItem.HasZeroDimensionsOnDecalAsset)
                {
                    EditorGUILayout.HelpBox($"Cannot scale to match decal because selected {nameof(DecalAsset)} has width or height of zero.",
                      MessageType.Info);
                }
            }
        }

        void DecalPickerGUI(DecalMesh primaryItem, DecalMesh[] items, bool isEditingMultipleObjects)
        {
            EditorGUILayout.PropertyField(decalAsset,
              new GUIContent("Decal Asset",
              $"The decal to display. You can create new {nameof(DecalAsset)}s in your project assets.")
            );

            if (!isEditingMultipleObjects && !primaryItem.HasDecalAsset)
            {
                EditorGUILayout.HelpBox($"{nameof(DecalMesh)} requires a {nameof(DecalAsset)}",
                  MessageType.Error);
            }
        }

        void OpacityOptionsGUI()
        {
            EditorGUILayout.Slider(opacity, 0f, 1f, "Opacity");
            EditorGUILayout.Slider(zFadeDistance, 0f, 1f, "zFade Distance");

            float min = minAngleFadeDegrees.floatValue;
            float max = maxAngleFadeDegrees.floatValue;
            EditorGUILayout.MinMaxSlider("Angle Fade", ref min, ref max, 0f, 90f);
            minAngleFadeDegrees.floatValue = min;
            maxAngleFadeDegrees.floatValue = max;
        }

        void FlipOptionsGUI()
        {
            using (EditorGUIUtility.wideMode ? new EditorGUILayout.HorizontalScope() : null)
            {
                EditorGUILayout.PropertyField(isFlipU, new GUIContent("Flip Horizontal"));
                EditorGUILayout.PropertyField(isFlipV, new GUIContent("Flip Vertical"));
            }
        }

        void DrawOrderOptionsGUI()
        {
            EditorGUILayout.IntSlider(drawOrder, 0, 100, "Draw Order");
        }

        void TargetMeshesGUI()
        {
            EditorGUILayout.PropertyField(shouldUseAllSceneStaticMeshes,
              new GUIContent("All static meshes",
              "Rather than specifying which objects this decal should target, automatically find nearby static meshes in the scene."
            ));
            using (new EditorGUI.DisabledScope(shouldUseAllSceneStaticMeshes.boolValue))
            {
                EditorGUILayout.PropertyField(meshesToProjectAgainst,
                  new GUIContent("Target meshes",
                  "The meshes that this decal will attempt to project against, can be any number of scene objects with MeshFilter components."
                ));
            }
            if (shouldUseAllSceneStaticMeshes.boolValue)
            {
                EditorGUILayout.HelpBox($"To select specific meshes to target you need to first disable the \"All static meshes\" option.",
                  MessageType.Info);
            }
        }
    }
}
