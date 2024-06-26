# Driven Decals
A mesh-based PBR decal system for Unity. For use with the Universal Render Pipeline.

These types of decals are good for static things like cracks and graffiti added into the game during level design (or during an initial procedural generation process).

This way of handling decals is bad for things like adding bullet holes when the player shoots. For dynamic decals, you can use the Universal Render Pipeline's [decal rendering feature](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@12.0/manual/renderer-feature-decal.html).

I've [written a little interactive article about these two types of decal](https://samdriver.xyz/articles/decalsIntro.htm) if you would like to know more.

[60 second introduction video on using this package.](https://www.youtube.com/watch?v=zFEtdRrD2D4)

## Status - Last updated 2024-06-09
Note that this package has a "preview" version number. You are of course welcome to use it as you like, but please be aware there are known missing features (for example it doesn't work with animated meshes) and almost has certainly bugs.

These days I work full-time on projects away from Unity, so please don't expect rapid progress.

## Example Screenshot
![Decals applied to a cylinder, giving the impression that it is made of cracked concrete. One of the cracks reveals a bright abstract pattern beneath the surface.](/Documentation~/images/demonstration.png)

## Key Features
* Creates meshes that behave like any other mesh in your scene. Making them easier to work with and use with other features.
* Easy to customise using Unity's Shader Graph.
* Low rendering cost and full compatibility with URP's forward renderer makes it ideal for use in XR.
* Custom inspectors that provide immediate in-editor feedback.
* Support for multi-object editing and undo.

## Key Limitations
* Decal mesh generation is relatively slow. This system is not recommended as a way to dynamically place bullet holes or other effects during gameplay.
* Once the decal mesh is generated it remains as it is and will not adapt to the other meshes changing. This system will not work nicely with skinned mesh renderers or other meshes that get distorted at runtime.

## Changelog
[Human-friendly changelog](CHANGELOG.md)

## Getting Started
### Requirements
* Unity 2019.3.0f6 or later, using the universal render pipeline (URP) version 7.2.1 or later.

In theory it should work in the LWRP and Unity versions as far back as 2018. But it looks like shader graph really doesn't care about cross-version compatibility. If you need this to work in those earlier versions I may be able to put together something compatible with some extra work.

### Installation
1. Within your project open the Package Manager from *Window* → *Package Manager*
2. Click the **+** icon at the top-left of the window and select "Add package from git URL..."
3. Paste in `https://github.com/Anatta336/driven-decals.git` and click "Add"
4. Wait a minute. It looks a lot like nothing is happening, but Unity is busy thinking about packages.
5. When installed you'll see "Driven Decals" listed in the window.

**Or** you can manually modify your project's `manifest.json` file in the Packages folder to include:
```js
{
  "dependencies": {
    "com.samdriver.driven-decals": "https://github.com/Anatta336/driven-decals.git",
    ...
  },
}
```

### Documentation
[Extensive documentation including step-by-step instructions to get you started.](Documentation~/DrivenDecals.md)

## Authors
Sam Driver - [Website](https://samdriver.xyz/)

### Special Thanks
[Kenny5](https://github.com/Kenny5) - Made everything work nicely with the Unity Package Manager.

## Licence
The source code of this project and associated documentation is licensed under the [MIT licence](../master/LICENSE.txt).

The included example assets are licensed under the Attribution 4.0 International [(CC BY 4.0)](https://creativecommons.org/licenses/by/4.0/) licence.

These licences mean you already have permission to use this in whatever project you like, including commercial releases. They place no obligation on you to release your source code. If you release something that makes use of this decal system a small acknowledgement in the credits would be appreciated, but is not required.
