-----------------
Version 3.8.0
-----------------
CM_FuseSync is designed to be used in congunction with SALSA with RandomEyes, and Mixamo Fuse Character Creator models, as outlined in the workflow created by:

Crazy Minnow Studio, LLC
CrazyMinnowStudio.com

The workflow is documented at the following URL, along with a downloadable zip file that contains the supporting files.

http://crazyminnowstudio.com/posts/using-mixamo-fuse-character-creator-with-salsa-with-randomeyes/


Package Contents
----------------
Crazy Minnow Studio/SALSA with RandomEyes/Third Party Support/
	Fuse Character Creator
		Editor
			CM_FuseSyncEditor.cs
				Custom inspector for CM_FuseSync.cs
			CM_FuseSetupEditor.cs
				Custom inspector for CM_FuseSetup.cs
		CM_FuseSync.cs
			Helper script to apply Salsa and RandomEyes BlendShape data to Mixamo Fuse character BlendShapes.
		CM_FuseSetup.cs
			SALSA 1-click Fuse setup script for new Fuse characters.
		ReadMe.txt
			This readme file.			
	Shared
		CM_RandomMovement.CS
			Random movement script for simple precedural idle animations.


Installation Instructions
-------------------------
1. Install SALSA with RandomEyes into your project.
	Select [Window] -> [Asset Store]
	Once the Asset Store window opens, select the download icon, and download and import [SALSA with RandomEyes].

2. Import the SALSA with RandomEyes Mixamo Fuse Character Creator support package.
	Select [Assets] -> [Import Package] -> [Custom Package...]
	Browse to the [SALSA_3rdPartySupport_FuseCharacterCreator.unitypackage] file and [Open].


Usage Instructions
------------------
1. Add a Fuse character, that contains BlendShapes, to your scene.

2. Select the character root, then select:
	[Component] -> [Crazy Minnow Studio] -> [Fuse Character Creator] -> [SALSA 1-Click Fuse Setup]
	This will add and configure all necessary component for a complete SALSA with RandomEyes setup.


What [SALSA 1-Click Fuse Setup] does
------------------------------------
1. It adds the following components:
	[Component] -> [Crazy Minnow Studio] -> [Salsa3D] (for lip sync)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for eyes)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for custom shapes)
	[Component] -> [Crazy Minnow Studio] -> [Fuse Character Creator] -> [CM_FuseSync] (for syncing SALSA with RandomEyes to your Fuse character)

2. On the Salsa3D component, it leaves the SkinnedMeshRenderer empty, and sets the SALSA [Range of Motion] to 75. (Set this to your preference)

3. On the RandomEyes3D componet for eyes, it leaves the SkinnedMeshRenderer empty, and sets the [Range of Motion] to 60. (Set this to your preference)

4. On the RandomEyes3D component for custom shapes, it attempts to find and link the main SkinnedMeshRenderer with BlendShapes. If you've exported a multi resolution character, delete the resolutions you're not using and create a prefab with only the resolution you wish to use.

5. On the RandomEyes3D component for custom shapes, it checks [Use Custom Shapes Only], Auto-Link's all custom shapes, and removes them from random selection. 
	You should selectively include small shapes, like eyebrow and facial twitches, in random selection to add natural random facial movement.

6. On the CM_FuseSync.cs component it attempts to link the following:
	Salsa3D
	RandomEyes3D (for eyes)
	The main SkinnedMeshRenderer with BlendShapes.
	The eyelashes SkinnedMeshRenderer with BlendShapes.
	The Left and Right eye bones.