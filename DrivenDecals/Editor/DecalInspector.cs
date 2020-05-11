using UnityEngine;
using UnityEditor;

namespace SamDriver.Decal
{
  [CustomEditor(typeof(Decal))]
  [CanEditMultipleObjects]
  public class DecalInspector : Editor
  {
    SerializedProperty decalAsset;
    SerializedProperty opacity;
    SerializedProperty zFadeDistance;
    SerializedProperty isFlipU;
    SerializedProperty isFlipV;
    SerializedProperty shouldUseAllSceneStaticMeshes;
    SerializedProperty meshesToProjectAgainst;

    void OnEnable()
    {
      decalAsset = serializedObject.FindProperty(nameof(Decal.DecalAsset));
      opacity = serializedObject.FindProperty(nameof(Decal.Opacity));
      zFadeDistance = serializedObject.FindProperty(nameof(Decal.ZFadeDistance));
      isFlipU = serializedObject.FindProperty(nameof(Decal.IsFlipU));
      isFlipV = serializedObject.FindProperty(nameof(Decal.IsFlipV));
      shouldUseAllSceneStaticMeshes = serializedObject.FindProperty(nameof(Decal.ShouldUseSceneStaticMeshes));
      meshesToProjectAgainst = serializedObject.FindProperty(nameof(Decal.MeshesToProjectAgainst));
    }

    public override void OnInspectorGUI()
    {
      bool isEditingMultipleObjects = (targets != null && targets.Length > 1);

      var items = new Decal[targets.Length];
      for (int i = 0; i < targets.Length; ++i)
      {
        items[i] = (Decal)targets[i];
      }
      var primaryItem = (Decal)target;
      serializedObject.Update();

      ProjectMeshButtonGUI(primaryItem, items, isEditingMultipleObjects);
      ResetMeshButtonGUI(primaryItem, items, isEditingMultipleObjects);
      ScaleToMatchButtonGUI(primaryItem, items, isEditingMultipleObjects);
      DecalPickerGUI(primaryItem, items, isEditingMultipleObjects);
      OpacityOptionsGUI();
      FlipOptionsGUI();
      TargetMeshesGUI();

      serializedObject.ApplyModifiedProperties();
    }

    void ProjectMeshButtonGUI(Decal primaryItem, Decal[] items, bool isEditingMultipleObjects)
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
        if (GUILayout.Button("Project mesh"))
        {
          foreach (var item in items)
          {
            if (item.HasMeshToProjectAgainst)
            {
              item.GenerateProjectedMesh();
            }
          }
        }
      }
      if (!canAnyTargetProject)
      {
        if (shouldUseAllSceneStaticMeshes.boolValue)
        {
          EditorGUILayout.HelpBox("Cannot project mesh until the decal is placed close to a static mesh in the scene.",
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

    void ResetMeshButtonGUI(Decal primaryItem, Decal[] items, bool isEditingMultipleObjects)
    {
      if (GUILayout.Button("Reset mesh"))
      {
        foreach (var item in items)
        {
          item.GenerateSimpleQuadMesh();
        }
      }
    }

    void ScaleToMatchButtonGUI(Decal primaryItem, Decal[] items, bool isEditingMultipleObjects)
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

    void DecalPickerGUI(Decal primaryItem, Decal[] items, bool isEditingMultipleObjects)
    {
      EditorGUILayout.PropertyField(decalAsset,
        new GUIContent("Decal Asset",
        $"The decal to display. You can create new {nameof(DecalAsset)}s in your project assets.")
      );

      if (!isEditingMultipleObjects && !primaryItem.HasDecalAsset)
      {
        EditorGUILayout.HelpBox($"{nameof(Decal)} requires a {nameof(DecalAsset)}",
          MessageType.Error);
      }
    }

    void OpacityOptionsGUI()
    {
      EditorGUILayout.Slider(opacity, 0f, 1f, "Opacity");
      EditorGUILayout.Slider(zFadeDistance, 0f, 1f, "Z Fade Distance");
    }

    void FlipOptionsGUI()
    {
      using (EditorGUIUtility.wideMode ? new EditorGUILayout.HorizontalScope() : null)
      {
        EditorGUILayout.PropertyField(isFlipU, new GUIContent("Flip Horizontal"));
        EditorGUILayout.PropertyField(isFlipV, new GUIContent("Flip Vertical"));
      }
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
