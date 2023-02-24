# 3D-to-2D-RenderFactory

## So, what is this?  
A half-finished Unity Plugin for creating 2D sprites from 3D animated models, currently available as a downloadable scene instead. Originally developed as a way to both learn Unity and more properly pick up C# back in 2018.X. Tested and working up to Unity 2020.3.15f2, there were some odd issues with at least 2019.4.18f1 where it would very occasionally softlock Unity on startup.

The original goal of this whole project was to create Might and Magic 8-style spritesheets from animated 3D models. Over time it grew to also allow capturing of static images, as well as support for isometrical rendering (largely inspired by Grandia 1s 2D-in-a-3D-world style.)   

Currently busy with IRL-stuff but aiming to have an "official" release of this by mid-march.

## So, what does it produce?
2D sprites! Or rather, pngs.  

On a more serious note: The factory works by letting you set up Camera rigs around a 3D model and then stop motion stepping through their animations, capturing and export each frame as an image. The rigs can generally be run in either an isometric mode - think Age of Empires 2 - or a side view mode - think Doom. Beyond fast and storage efficient image capturing the factory also produces metadata about the model, as well as the option to also export any baked in movement and rotation on an individual X/Y/Z-axis basis.  
  
On a more technical level everything works with the help of native unity parts. The image capturing is done by letting each camera output to a render texture which is then saved to the disk. Movement export is done by recording the movement and rotation change during rendering, with optional extra non-rendered frames for higher data fidelity. Since the goal of the factory was to create a common output from different input sources a settable unit-baseline is used to determine what the actual output size and movement magnitude should be, which is also recorded in the metadata.  

Almost all steps of the above beyond importing is automated, requiring a user to only modify some top level settings. Fuller documenation is on the TODO-list.  
  
## Some caveats
It should be noted that the factory was originally slanted towards Adobe Mixamo and different free characters. A fuller set of animated characters have not been tested. Mainly more modern animated characters than free ones from the original ~2018-2020 development period might cause issues, or more complicated animations in general since this is designed for simpler humanoids using animation clips. The custom Editor UI is also written in IMGUI, which causes some issues with color tinting between the light and dark unity skins.
  
## Current progress
The current goal is to create proper documentation, debug different model sources, as well as implementing the current batch of worked on features by mid-March 2023. After that the idea is to release this in a more easily installabe form rather than just having it as a unity scene.
  
I've also written a private importer that takes the output from this, creates sprites and then rebakes them with the movement into unity animations. I'm currently in the process of moving that away from some paid plugins so I can make it public too.
