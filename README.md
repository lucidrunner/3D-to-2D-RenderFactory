# 3D-to-2D-RenderFactory

## So, what is this?  
A half-finished Unity Plugin for creating 2D sprites from 3D animated models, currently available as a downloadable scene instead. Originally developed as a way to both learn Unity and more properly pick up C# back in 2018.X. Tested and working up to Unity 2020.3.15f2, there were some odd issues with at least 2019.4.18f1 where it would very occasionally softlock Unity on startup.

The original goal of this whole project was to create Might and Magic 8-style spritesheets from animated 3D models. Over time it grew to also allow capturing of static images, as well as support for isometrical rendering (largely inspired by Grandia 1s 2D-in-a-3D-world style.)   

Currently busy with IRL-stuff but aiming to have a "official" release of this by mid-march.

## So, what does it produce?
2D sprites! Or rather, pngs.  

On a more serious note: The factory works by letting you set up Camera rigs around a 3D model and then stop motion stepping through their animations, capturing and export each frame as an image. The rigs can generally be run in either an isometric mode - think Age of Empires 2 - or a side view mode - think Doom or the previously mentioned Might and Magic 8. Beyond fast and storage efficient image capturing the factory also produces XML metadata about the model, as well as the option to also export any baked in movement and rotation on an individual X/Y/Z-axis basis.  
  
On a more technical level everything works with the help of native unity parts. The image capturing is done by letting each camera output to a render texture which is then saved to the disk. Movement export is done by recording the movement and rotation change during rendering, as well as through extra stop motion steps that aren't rendered to provide greater accurace. Since the goal of the factory was to create a common output from different input sources size baselines are used to determine what the actual output size and movement magnitude should be, which is also recorded in the metadata.  

Almost all steps of the above beyond importing is mostly automated, requiring a user to only modify some top level and import settings. Fuller documenation is on the TODO-list.  
  
## Some caveats
It should be noted that the factory was originally slanted towards Adobe Mixamo and different free characters. A fuller set of animated characters have not been tested. Mainly more modern animated characters than free ones from the original ~2018-2020 development period might cause issues, or more complicated animations in general since this is designed for simpler humanoids using animation clips. The custom Editor UI is also written in IMGUI, which causes some issues with color tinting between the light and dark unity skins.
  
## Current progress
The current goal is to create proper documentation, debug different model sources, as well as implementing the current batch of proposed features (and json export) by mid-March 2023. I'm also looking at refactoring the current inheritance structure to a composition based framework instead, here I'm mainly looking at moving away from the current Static / Dynamic factory setup to something a bit more sane (although this might be too convoluted and abandonded in the end.) 
  
I've also written a private importer that takes the output from this, creates sprites and then rebakes them with the movement into unity animations. I'm currently in the process of moving that away from some paid plugins so I can make it public too.

  


   
   
# Old Description Below, will be removed when I figure out what's actually relevant in it
# TODO 

## DEBUG
Debug that the full extracted values are the same after having substepped  
Do this for both frame scales and standard scales  
Test clamping for both types  
What happens with statics when having several scenes?

## FEATURES
Fully topdown rendering, any issues there? - implement as pickable option but use side rigs?  
Add option to run both factories in the same pass?  
Write a vector shader and see if we can get positional data correctly applied from model to a rendered image  
 a) Vector Shader  
 b) Vector Reader based on the shader-  compare data  
   
- maybe we wanna change animation source avatar to the model on the fly so we can reuse them for different models?  
(we probably want to do this as an settable option, false by default)  
- Create Root motion setup pdf and put somewhere in the project so I remember how the above things are solved  
  
Isometric Rendering without baseplate but reference point instead  
Add option to include baseplate for whatever reason  

## FIXES
We need to create the folder for the settings  
Also the RepaintAll isn't bound to if it's changed or not atm.  
We also need to change the provider to use a static Instance and attempt to load only if that's null (caching) and only create if that load fails  
Don't export root on static record  
  
Comment / Refactoring pass for all classes :O)  
- Recursively apply preset - disable while it's working and report progress while working  
Inspect Prefab usages - what can be done programmatically and what needs to be a prefab object?  
big cleanup of namespaces & editor / non-editor overlaps between them  
- Add a lot of flogd debug calls to critical junctions so we can actually see the innards of the factory working  
- Make sure we have min values set for everything  
- Take a look at the Animation Settings / Clamped Animation settings and figure out if we can make them inherit and access them  
more effectively where they're accessed (possibly done, inherits now but can be made more effective maybe?)  
- Add tooltips to a lot of buttons ;_;  (check all constructors where we initialise them)  
- ModelRendererRigSetup tooltips need to be written too  
- "Update when offscreen" (what does this do to bounding box?) and shadow settings on meshes should be experimented with  
- Break out the clamping calculations in a separate class so we can reuse them in different places  
(think this is hard to do any might have already done what I wanted to do with it?)  
- Go through the animation utilities, animator helper and animation analyser classes and see what can be combined.  
Is ClampedFrameCount ever used outside get frames for a clamped? Possibly on XML export  
Maybe FrameCount as overridable?  

## BUGS
"Bugless" atm

# TODO Later Big Things
-Get models from other places and test  
- Add clamped settings preview to the animation previewer  
- Depth mapping :7  
