using UnityEngine;
using System.Collections.Generic;

// to avoid naming conflicts the decal system uses the namespace SamDriver.Decal
using SamDriver.Decal;

/// <summary>
/// An example of creating decals during runtime.
/// Responds to input button "Fire1" by (re)spawning a circle of decals and projecting
/// them against a specified set of meshes.
/// 
/// A reminder that projecting decals using this system is (currently) slow, so using
/// it to create decals during interactive segments is strongly discouraged.
/// </summary>
public class DecalSpawner : MonoBehaviour
{
  public int SpawnCount = 6;
  public float DecalScale = 0.4f;
  public float ProjectionDepth = 1f;
  public DecalAsset DecalToSpawn;
  public List<MeshFilter> MeshesToProjectAgainst = new List<MeshFilter>();

  List<Decal> spawnedDecals = new List<Decal>();

  void Update()
  {
    if (Input.GetButtonDown("Fire1"))
    {
      Clear();
      Spawn();
    }
  }

  void Spawn()
  {
    foreach (Vector3 localPosition in SpawningPattern(SpawnCount))
    {
      spawnedDecals.Add(CreateDecal(localPosition.x, localPosition.y));
    }
  }

  Decal CreateDecal(float localX, float localY)
  {
    // create game object as child of the spawner
    GameObject decalObject = new GameObject($"Spawned Decal ({localX.ToString("F3")},{localY.ToString("F3")})");
    decalObject.transform.SetParent(this.transform, false);

    // set the transform however you wish, keeping in mind that rotation affects how it'll be projected
    decalObject.transform.localPosition = new Vector3(localX, localY, 0f);
    decalObject.transform.localScale = new Vector3(DecalScale, DecalScale, ProjectionDepth);

    // needs a MeshFilter and MeshRenderer to render the decal
    decalObject.AddComponent<MeshFilter>();
    var meshRenderer = decalObject.AddComponent<MeshRenderer>();

    // decals generally shouldn't cast shadows
    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

    // create the decal component itself
    var decal = decalObject.AddComponent<Decal>();

    // provide it with a DecalAsset
    decal.DecalAsset = this.DecalToSpawn;

    // scale the decal's object to match selected decal's ratio
    // (same behaviour as clicking the "Scale to match decal shape" button)
    decal.ScaleToMatchDecalBoundsRatio();

    // we can set any of the options available on the Decal component,
    // here we'll horizontally flip some of them at random
    decal.IsFlipU = (Random.value > 0.5f);

    // SetupMaterialPropertyBlock should be called whenever the DecalAsset or
    // the per-decal options like flip and opacity are changed.
    decal.SetupMaterialPropertyBlock();

    // set up what the decal will project against
    // if you skip this, it'll default to projecting against any nearby static meshes
    decal.ShouldUseSceneStaticMeshes = false;
    decal.MeshesToProjectAgainst = this.MeshesToProjectAgainst;

    // perform the projection
    // this step is embarassingly slow, doing it during gameplay is a bad idea
    decal.GenerateProjectedMesh();

    return decal;
  }

  void Clear()
  {
    foreach (var decal in spawnedDecals)
    {
      Destroy(decal.gameObject);
    }
    spawnedDecals.Clear();
  }

  void OnDrawGizmosSelected()
  {
    // reuse the spawning pattern of the decals
    foreach (Vector3 localPosition in SpawningPattern(SpawnCount))
    {
      // draw a line representing where each decal will be projected
      Vector3 worldStart = transform.TransformPoint(localPosition + Vector3.back * ProjectionDepth * 0.5f);
      Vector3 worldEnd = transform.TransformPoint(localPosition + Vector3.forward * ProjectionDepth * 0.5f);
      Gizmos.DrawLine(worldStart, worldEnd);
    }
  }

  IEnumerable<Vector3> SpawningPattern(int totalCount)
  {
    for (int i = 0; i < totalCount; ++i)
    {
      float angleRadians = ((float)i / SpawnCount) * Mathf.PI * 2f;
      yield return new Vector3(
        Mathf.Cos(angleRadians),
        Mathf.Sin(angleRadians),
        0f
      );
    }
  }
}
