# Changelog

## [0.0.6] - 2020-05-19
### Changed
- Significantly improved performance of generating the `RawMesh` in `MeshProjection` by avoiding garbage generation/collection. My test case went from 277ms to 26ms for that part of the process.
- `MeshProjection` no longer automatically starts when instantiated, you should now call `Begin()`
- Example `DecalSpawner` now passes hint to its generated decals that the projection process may take several frames to complete. That's important as the Unity jobs system allocates memory depending on how long it'll need to exist for.
- Various bits of minor code tidying, removing unused `using` statements and so on.

## [0.0.5] - 2020-05-18
### Changed
- Disabled a safety check so the ugly `UpToFiveTriangles` struct is no longer needed. Shouldn't have any externally visible effect.

## [0.0.4] - 2020-05-16
### Added
- Option to "Auto-Repeat" projection of the `DecalMesh` which triggers the projection process whenever the selected decal is moved, rotated, or resized in the editor.

## [0.0.3] - 2020-05-14
### Added
- Option to fade out a decal depending on the surface's angle to the decal's forward direction. Useful for hiding parts of the decal that are highly distorted by projection.
- Option to adjust a decal's draw order, useful for selecting which overlapping decal should be drawn on top.

### Changed
- Renamed `Decal` component to `DecalMesh`, making it clearer it has responsibility for mesh generation, and to free up the more general "Decal" name.
- Various improvements to `README`

### Fixed
- Normals on the decal mesh were being incorrectly affected by local scale.

## [0.0.2] - 2020-05-13
### Added
- Rework most of the mesh projection system to use Unity's Job system. Projection is now faster, uses all available CPU cores, and can be extended over several frames. However there's still a block on the primary thread while the mesh itself is built. Generating decals during gameplay remains a bad idea.
- Added `Decal`'s `GenerateProjectedMeshDelayed` which allows the projection process to take place over several frames.

### Changed
- Renamed `Decal`'s `GenerateProjectedMesh` to `GenerateProjectedMeshImmediate` to indicate that it forces the projection to be completed during that method call.
- Large changes to contents of `DrivenDecals/Util` to make them compatible with the Job system. All changes were to `internal` things.
- Various improvements to `README`

## [0.0.1] - 2020-05-12
Initial pre-release version.