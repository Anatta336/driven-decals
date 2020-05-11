# Driven Decals
A mesh-based PBR decal system for Unity. Intended primarily for use with the Universal Render Pipeline's forward renderer.

*TODO: image of pretty decals*

## Key Features
* Creates meshes that behave like any other mesh in your scene. Making them easier to work with and use with other features.
* Easy to customise using Unity's Shader Graph.
* Low rendering cost and full compatibility with URP's forward renderer makes it ideal for use in XR.
* Custom inspectors that provide immediate in-editor feedback.
* Support for multi-object editing and undo.

## Key Limitations
* Not well suited for generating decals at runtime. For example it's not recommended as a way to dynamically place bullet holes.
* Inefficient compared to other methods available to deferred renderers such as the high definition render pipeline (HDRP).

## Getting Started
### Requirements
* Unity 2018 or later, using the lightweight render pipeline (LWRP) or universal render pipeline (URP)

### Installation
1. Download drivenDecals.unitypackage
2. Open your project in Unity (alternatively create a test project using the LWRP or URP template.)
3. Drag the drivenDecals.unitypackage file into the "Project" window in Unity.
4. Accept the default recommendation to include everything, and click Import.
5. Don't rename or move the Assets/DrivenDecals/Editor directory, unfortunately Unity requires some hardcoded paths.

### Your First Decal
You can create a decal object in your scene using either the right-click menu in the Hierarchy window or the GameObject menu. Look for *3D Object* → *Driven Decal*.

![Selecting Driven Decal from the object creation menu in Unity.](/documentation/createDrivenDecalMenu.png)

At first the decal will be a simple quad floating in space. When selected the decal will also display a grey wireframe cube which represents the region it will be projected into. If you can't see the grey wireframe box you may have gizmos disabled in your scene view. Try clicking the "Gizmos" button near the top-right of the scene window to toggle them.

![A freshly created decal object in the Unity template scene, represented as a floating quad filled by a grid pattern.](/documentation/defaultDecal.png)

You can use Unity's standard controls to move, rotate, and scale the decal object. Position it so the grey wireframe is passing through some static meshes in the scene and the textured quad is on the side where the decal will be visible. If you're using the Unity template scene then the floor, walls, or wooden planks are all suitable.

![A decal object positioned so that the grey wireframe box overlaps some wooden planks in Unity's template scene. The decal's texture is shown on a quad floating in the air.](/documentation/unprojectedDecal.png)

Click "Project mesh" in the inspector to project the decal forward through its volume, generating a new mesh that matches the shape of whatever meshes it encounters. When making adjustments you'll need to click "Project mesh" again to see their effect. If there's no mesh being generated double check that the decal's grey wireframe is passing *through* the mesh of another GameObject that's marked as static.

![A decal object after projection. The decal's texture now appears flattened against the wooden planks.](/documentation/projectedDecal.png)

Take some time to experiment. See what happens when the decal is projected against various forms and from various angles. You may want to choose a more interesting *DecalAsset* from the included samples. Notice that "fastenings" set provide normal, metallic, smoothness and occlusion information which is rendered correctly.

You're free to use both this decal system and the example assets in any commercial or non-commercial project you wish (for details see [Licence](#-licence)).

## Creating Your Own Decals
You'll probably want to use more than just the sample decals so let's step through how to create your own.

### 1. Texture
Textures for decals are handled in Unity the same as any other texture. Keep in mind that each decal will be defined by a simple axis aligned rectangle. If you have multiple decals packed into a single texture you'll usually want to avoid positioning them so that their rectangles would intersect. When using multiple textures (such as separate diffuse and normal textures) the same rectangle is used to define the position of the decal on each texture. The decal system copes fine with non-square textures.

Let's create a simple diffuse-only texture by drawing something in an image editor. Unless you want the decal to have hard rectangle edges you'll want to save the texture with an alpha channel too.

![Unity's texture importer showing an image with the letters A, B, C, D, each individually circled.](/documentation/pencilLettersImport.png)

### 2. Material
As is typical in Unity, your material will take textures and any other properties and use them to render the mesh. To work in this decal system the material needs to handle some specific properties (see [Custom Shaders](#-custom-shaders) for a list) so the material you create will need to use a compatible shader. There are several shaders included in the samples which should cover the typical use cases. See the [Material](#-material) section for more details.

The *Decal Diffuse* shader suits our purposes well as we're not providing any texture maps apart from diffuse with alpha. The shader still renders using the PBR system so we can modify uniform values for smoothness, metallic, and ambient occlusion.

![Unity's material inspector showing a material using the previously imported texture and the "Shader Graphs/Decal Diffuse" shader.](/documentation/pencilLettersMaterial.png)

### 3. DecalAsset
To define what part of the texture will be used for each asset we need to create a *DecalAsset* by right-clicking in the project window and selecting *Create* → *Decal* → *Decal Asset*. Just like textures and materials, *DecalAssets* are assets in the project and not objects in the scene. Once you've made a *DecalAsset* you can use it in as many decal objects as you like.

![Unity's create asset menu, showing the path to creating a Decal Asset.](/documentation/createDecalAsset.png)

The *DecalAsset* needs to know what material to use, so we'll select the material we just made. If you select a material which isn't fully compatible with the decal system the inspector window will list what it's missing.

With a compatible material selected a preview of the diffuse texture should appear. We can click and drag in that preview to draw a rectangle around the part we want to use for this decal. To make small adjustments we can use the clickable buttons with arrows to move the borders of the rectangle around. If you prefer you can also directly type in values in the fields below.

![DecalAsset's inspector window with the texture preview showing that the area around the letter A has been selected.](/documentation/decalAssetPencilA.png)

Each *DecalAsset* defines a single rectangle on the texture. If a texture has several decals on it we can create multiple *DecalAssets*, all using the same material.

### 4. Decal object
To get the decal into the scene we create a decal object. Right-click in the hierarchy window and select *3D Object* → *Driven Decal*, just like if you were creating a cube or other basic object. Use the inspector to select one of the *DecalAssets* we just created and you should see the decal appear floating in the scene.

![Selecting a Decal Asset to use for a newly created decal object](/documentation/selectDecalAsset.png)

Now we position the decal object so that the grey wireframe box is intersecting with suitable target meshes, and click "Project mesh" in the inspector. If you want the decal to be projected on a mesh that isn't static, or only want it to appear on certain meshes then you can manually set the [Projection Targets](#-projection-targets).

![Two decal objects projected against the wooden plank. The letters now appears to be written on the surface of the wood.](/documentation/projectedLetterA.png)

If you want the same decal to appear multiple times just create more decal objects (or duplicate this one) and have them use the same *DecalAsset*. It's often helpful to use the decal's flip horizontal, flip vertical, and the object's rotation to make it less obvious that a texture is being reused.

## Overview
### Decal
A *Decal* is a component on a GameObject. Every decal that appears in your scene will be represented by an individual object. Although you can add it as a component to any GameObject usually you'll want it to be on its own object so you can easily adjust the position, rotation and scale. The GameObject will need to also have a *MeshFilter* and *MeshRenderer* to display the decal, and these are automatically added.

The object defines the location of a decal in the scene and the *Decal* component is responsible for generating the mesh. Reference to a *DecalAsset* is used decide what the decal should look like. You can have many decals all using the same *DecalAsset*.

You create a decal object by right-clicking in the Hierarchy window and selecting *3D Object* → *Driven Decal*, or using the top menu bar's *GameObject* → *3D Object* → *Driven Decal*

#### Projection Targets
By default a decal has the "All static meshes" option enables and it'll try to project against any nearby mesh that belongs to a GameObject marked as static. You may want more control over what the decal is projected against, which you can do by disabling that option and manually selecting the meshes to target.

With "All static meshes" unticked, the "Target meshes" field will no longer be greyed out. The easiest way to add to that list of meshes is to drag a GameObject with a MeshFilter (anything that gets rendered as a mesh will have one) from the hierarchy window over to the "Target meshes" text in the inspector. You can also expand the "Target meshes" field, manually change the Size value and add or remove MeshFilters from the list.

![A decal projected against the safety hat in Unity's template scene. The inspector shows that the decal is targeting only the hat's MeshFilter.](/documentation/projectedOnHat.png)

Limiting the target meshes can also help improve performance of mesh projection. In a dense scene with large meshes there may be a significant number of triangles to process when projecting against all static meshes. When targetting "all" static meshes there is a culling process but it's (currently) quite simple so a potentially large number of triangles still need to be handled.

Decals can be projected against any mesh in the scene, not just static meshes. But remember once the decal mesh has been generated it's handled by Unity like any other mesh - it doesn't automatically know that it should follow the target. So if you project a decal against a car and then move the car, the decals will be left behind floating in the air. A fun image, but probably not intentional. For a situation like that you should make the decal objects be a child of the car object, that way when the car moves it'll take the decals with it.

### DecalAsset
A *DecalAsset* is an asset in your project. It decides how a decal will look by defining a rectangular region on the source texture, and by referring to a *Material* that does the rendering. You can have many *DecalAssets* all using the same *Material*.

To select what region of the source texture will be used in a *DecalAsset* you can use the mouse to drag a rectangle over the preview image, use the small buttons with arrows on them, or directly enter values for uMin, uMax, vMin, and vMax. Common practice is to start by using the mouse to select approximately the correct region and then use the clickable buttons to adjust it.

You create a *DecalAsset* by right-clicking in the Project window and selecting *Create* → *Decal* → *Decal Asset*.

### Material
As with other meshes in Unity, the decal's mesh is rendered using a *Material*. The material provides the texture(s) and the process for rendering. Decals need special materials so you can't just use a material made for any model. You can make your own decal materials by using one of the included decal shaders, or you can create your own shader (see [Custom Shaders](#-custom-shaders)).

Generally you'll want to create one Material for each texture set. A texture set may be a single diffuse texture or a matching set of diffuse, normal, metallic, occlusion, etc. textures. A common practice is to design a texture set so that it contains multiple related decals, very similar to the idea of a spritesheet.

You create a decal material as you would any other Material in Unity by right-clicking in the Project window and selecting *Create* → *Material*. The newly created material will default to using one of Unity's shaders which isn't suitable for decals, so you should click the Shader dropdown menu at the top of the inspector and select a suitable shader. When selecting a shader you can type "decal" to more easily find the example shaders.

Note that the "Legacy Shaders/Decal" shader is **not** compatible with this decal system. DecalOverlayBounds and DecalThumbnail are shaders used in the custom inspectors, and are also not suitable for decals.

![Shader selection on a new material, showing a listing of shaders filtered by the string "decal".](/documentation/decalShadersList.png)

## Generating Decals at Runtime
*TODO: example code, warning about performance*

## Static Batching
Unity's built-in static batching system doesn't cope well with how UVs are manipulated in these decals, so I highly recommend not enabling "Batching Static" on decal objects. It's fine to have static batching enabled in your project as a whole. Be careful not to toggle on the "Static" option at top of the inspector window for a decal object as that turns on static batching.

## Baked Lighting
Once generated, Unity treats the decal's mesh the same as any other mesh the scene. That means they'll show up in baked lighting, reflection probes, and anything else in just the same way a mesh made in any other way would.

By default decal objects are marked to "Contribute GI" like a static mesh. That means a decal projected against a wall will be - once the lighting has been regenerated - properly affected by baked light effects. Again remember that so far as Unity is concerned the decal is just a mesh with a material.

## Custom Shaders
To further customise how your decals appear you may wish to create your own shaders. The included decal shaders are all created with Unity's Shader Graph. Making a copy of the *Decal Diffuse Normal* shader graph is a good place to start. Notice in particular that you shouldn't directly use the UV0 channel as provided by the mesh, instead you should pass it through the *Decal UV* subgraph.

The decal system looks for certain named properties on decal materials, which your custom shader should expose. The following two are required for the decal system to work:
* **_DiffuseAlpha** Used for the preview image when working on a *DecalAsset* and to generate thumbnails.
* **_Bounds** Defines which region of the texture(s) will be used by this decal.

These properties are highly recommended as certain features will not work without them, but the system as a whole will cope if they're missing:
* **_Opacity** Float in range 0 to 1 to uniformly fade out the decal, with 0 being hidden.
* **_FlipU** Boolean intended to flip the texture region in the U axis. The *Decal UV* subgraph can do this for you.
* **_FlipV** Boolean intended to flip the texture region in the V axis. The *Decal UV* subgraph can do this for you.
* **_ZFadeStart** Float in range 0 to 1 to fade out the decal as it gets near the Z axis limits of its region. The *Fade near z bounds* subgraph can help achieve this effect.

The *DecalAsset* inspector will inform you if you use a material that's lacking any of these properties.

Although not required by the decal system, all the sample shaders use the *Offset in viewspace* subgraph to shift the decal's vertices a short distance towards the camera when they are rendered. This should prevent z-fighting artefacts under typical use. If you are using decals on very distant objects - especially if they're viewed at a grazing angle - you may need to increase the offset distance.

## High Definition Render Pipeline
Although this decal system works in the HDRP, I don't recommend its use. The HDRP is a deferred renderer which allows for PBR decals to be applied in a more efficient way than is possible in the forward renderer of the URP. If you're looking to use decals in an HDRP project I recommend starting with the [Decal Projector](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@5.7/manual/Decal-Projector.html).

There is also a known issue with the rendering of previews and thumbnails texture not working in the HDRP. As this decal system isn't intended for use in the HDRP, fixing that is a low priority.

## Future Work
The process for generating the projected mesh has significant potential for optimisation. Not least, it should be well suited to being split over several threads. Improvements here may make the system more suitable for generating decals at runtime.

## Authors
Sam Driver - [Website](https://samdriver.xyz/), [Twitter](https://twitter.com/SamDriver_)

## Licence
The source code of this project and associated documentation is licensed under the MIT licence.
The included example assets are licensed under the Attribution 4.0 International (CC BY 4.0) licence.
If you release something that makes use of this decal system a small acknowledgement in the credits would be appreciated, but is not required.

