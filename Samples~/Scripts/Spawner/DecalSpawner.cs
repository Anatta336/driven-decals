using UnityEngine;
using System.Collections.Generic;

// to avoid naming conflicts with your project the decal system uses its own namespace.
// (as this example is within the same namespace this "using" directive isn't actually needed)
using SamDriver.Decal;

namespace SamDriver.Decal.Sample
{
    /// <summary>
    /// An example of creating decals during runtime.
    /// Responds to input button "Fire1" by (re)spawning a circle of decals and projecting
    /// them against a specified set of meshes.
    /// 
    /// Projecting decals using this system is relatively slow, so using it to create decals
    /// during gameplay like this is generally discouraged.
    /// If you need decals that are cheap to spawn, search for view-space projection decals.
    /// </summary>
    public class DecalSpawner : MonoBehaviour
    {
        public int SpawnCount = 6;
        public float DecalScale = 0.4f;
        public float ProjectionDepth = 1f;
        public DecalAsset DecalToSpawn;
        public List<MeshFilter> MeshesToProjectAgainst = new List<MeshFilter>();

        List<DecalMesh> spawnedDecals = new List<DecalMesh>();

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

            foreach (var decal in spawnedDecals)
            {
                if (decal.HasMeshToProjectAgainst)
                {
                    // because we're spawning several decals at once it's possible to take more than
                    // 4 frames for all the jobs to complete.
                    // (this changes how the temporary memory used by the jobs is allocated)
                    bool mayTakeMoreThanFourFrames = true;

                    // begin to perform the projection
                    // if you want the decal mesh to be generated immediately use GenerateProjectedMeshImmediate,
                    // but beware it can easily lock up the main thread and cause skipped frames.
                    decal.GenerateProjectedMeshDelayed(mayTakeMoreThanFourFrames);
                }
            }
        }

        DecalMesh CreateDecal(float localX, float localY)
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

            // create the DecalMesh component itself
            var decal = decalObject.AddComponent<DecalMesh>();

            // provide it with a DecalAsset
            decal.DecalAsset = this.DecalToSpawn;

            // scale the decal's object to match selected decal's ratio
            // (same behaviour as clicking the "Scale to match decal shape" button)
            decal.ScaleToMatchDecalBoundsRatio();

            // we can set any of the options available on the DecalMesh component,
            // here we'll horizontally flip some of them at random
            decal.IsFlipU = (Random.value > 0.5f);

            // SetupMaterialPropertyBlock should be called whenever the DecalAsset or
            // the per-decal options like flip and opacity are changed.
            decal.SetupMaterialPropertyBlock();

            // set up what the decal will project against
            // if you skip this it'll default to projecting against any nearby static meshes
            decal.ShouldUseSceneStaticMeshes = false;
            decal.MeshesToProjectAgainst = this.MeshesToProjectAgainst;

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

        /// <summary>
        /// Generate local positions distributed in a unit circle on the xy plane.
        /// </summary>
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
}
