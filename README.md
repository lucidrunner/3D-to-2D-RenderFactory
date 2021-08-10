# 3D-to-2D-RenderFactory
Unity Plugin for creating 2D sprites from 3D animated models.

# TODO 

# DEBUG
Debug that the full extracted values are the same after having substepped\
Do this for both frame scales and standard scales\
Test clamping for both types\
What happens with statics when having several scenes?\

# FEATURES
Fully topdown rendering, any issues there? - implement as pickable option but use side rigs?\
Add option to run both factories in the same pass?\
Write a vector shader and see if we can get positional data correctly applied from model to a rendered image\
 a) Vector Shader\
 b) Vector Reader based on the shader-  compare data\
\
- maybe we wanna change animation source avatar to the model on the fly so we can reuse them for different models?\
(we probably want to do this as an settable option, false by default)\
- Create Root motion setup pdf and put somewhere in the project so I remember how the above things are solved\
\
Isometric Rendering without baseplate but reference point instead\
Add option to include baseplate for whatever reason\

# FIXES
We need to create the folder for the settings\
Also the RepaintAll isn't bound to if it's changed or not atm.\
We also need to change the provider to use a static Instance and attempt to load only if that's null (caching) and only create if that load fails\
Don't export root on static record\

Comment / Refactoring pass for all classes :O)\
- Recursively apply preset - disable while it's working and report progress while working\
Inspect Prefab usages - what can be done programmatically and what needs to be a prefab object?\
big cleanup of namespaces & editor / non-editor overlaps between them\
- Add a lot of flogd debug calls to critical junctions so we can actually see the innards of the factory working\
- Make sure we have min values set for everything\
- Take a look at the Animation Settings / Clamped Animation settings and figure out if we can make them inherit and access them\
more effectively where they're accessed (possibly done, inherits now but can be made more effective maybe?)\
- Add tooltips to a lot of buttons ;_;  (check all constructors where we initialise them)\
- ModelRendererRigSetup tooltips need to be written too\
- "Update when offscreen" (what does this do to bounding box?) and shadow settings on meshes should be experimented with\
- Break out the clamping calculations in a separate class so we can reuse them in different places\
(think this is hard to do any might have already done what I wanted to do with it?)\
- Go through the animation utilities, animator helper and animation analyser classes and see what can be combined.\
Is ClampedFrameCount ever used outside get frames for a clamped? Possibly on XML export\
Maybe FrameCount as overridable?\

# BUGS
"Bugless" atm

# TODO Later Big Things
-Get models from other places and test\
- Add clamped settings preview to the animation previewer\
- Depth mapping :7\
- Write a rider plugin to colour the numbers according to regions\
