# Changelog

## [0.7.0] - 2020-06-09
### Added
- Thanks to [Kenny5](https://github.com/Kenny5) the decal system can now be added to a project through the Unity Package Manager system rather than manually downloading the .package file. See the [README](README.md) for updated installation instructions.
- Tests for Triangle and DecalAsset. Code coverage is lower than it should be, but it's a start.

### Changed
- Thanks to [Kenny5](https://github.com/Kenny5) many directories have been moved around to support Unity's package manager.
- This changelog now uses [semantic version numbers](https://semver.org/) instead of just dates. Note that the major version number is still 0, indicating the project is in the initial development phase so even minor version increases may make breaking changes. I expect to move to 1.0.0 fairly soon.
- Rearranged the documentation. The README is now far shorter with the bulk of documentation [moved to its own file](Documentation~/DrivenDecals.md). 
- Updated the 60 second introduction video to use the package manager method of installation.

### Fixed
- `Triangle`'s `GeometryNormal` is now correctly normalized.

## [0.6.0] - 2020-05-19
### Changed
- Significantly improved performance of generating the `RawMesh` in `MeshProjection` by avoiding garbage generation/collection. My test case went from 277ms to 26ms for that part of the process.
- `MeshProjection` no longer automatically starts when instantiated, you should now call `Begin()`
- Example `DecalSpawner` now passes hint to its generated decals that the projection process may take several frames to complete. That's important as the Unity jobs system allocates memory depending on how long it'll need to exist for.
- Various bits of minor code tidying, removing unused `using` statements and so on.

## [0.5.0] - 2020-05-18
### Changed
- Disabled a safety check so the ugly `UpToFiveTriangles` struct is no longer needed. Shouldn't have any externally visible effect.

## [0.4.0] - 2020-05-16
### Added
- Option to "Auto-Repeat" projection of the `DecalMesh` which triggers the projection process whenever the selected decal is moved, rotated, or resized in the editor.

## [0.3.0] - 2020-05-14
### Added
- Option to fade out a decal depending on the surface's angle to the decal's forward direction. Useful for hiding parts of the decal that are highly distorted by projection.
- Option to adjust a decal's draw order, useful for selecting which overlapping decal should be drawn on top.

### Changed
- Renamed `Decal` component to `DecalMesh`, making it clearer it has responsibility for mesh generation, and to free up the more general "Decal" name.
- Various improvements to `README`

### Fixed
- Normals on the decal mesh were being incorrectly affected by local scale.

## [0.2.0] - 2020-05-13
### Added
- Rework most of the mesh projection system to use Unity's Job system. Projection is now faster, uses all available CPU cores, and can be extended over several frames. However there's still a block on the primary thread while the mesh itself is built. Generating decals during gameplay remains a bad idea.
- Added `Decal`'s `GenerateProjectedMeshDelayed` which allows the projection process to take place over several frames.

### Changed
- Renamed `Decal`'s `GenerateProjectedMesh` to `GenerateProjectedMeshImmediate` to indicate that it forces the projection to be completed during that method call.
- Large changes to contents of `DrivenDecals/Util` to make them compatible with the Job system. All changes were to `internal` things.
- Various improvements to `README`

## [0.1.0] - 2020-05-12
Initial pre-release version.
