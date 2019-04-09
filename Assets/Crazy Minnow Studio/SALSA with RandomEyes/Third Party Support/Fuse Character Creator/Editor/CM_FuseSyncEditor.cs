using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.Fuse
{
	/// <summary>
	/// This is the custom inspector for CM_FuseSync, a script that acts as a proxy between 
	/// SALSA with RandomEyes and Mixamo Fuse characters, and allows users to link SALSA with 
	/// RandomEyes to Fuse characters without any model modifications.
	/// 
	/// Crazy Minnow Studio, LLC
	/// CrazyMinnowStudio.com
	/// 
	/// NOTE:While every attempt has been made to ensure the safe content and operation of 
	/// these files, they are provided as-is, without warranty or guarantee of any kind. 
	/// By downloading and using these files you are accepting any and all risks associated 
	/// and release Crazy Minnow Studio, LLC of any and all liability.
	[CustomEditor(typeof(CM_FuseSync))]
	public class CM_FuseSyncEditor : Editor 
	{
		private CM_FuseSync fuseSync; // CM_FuseSync reference
		private bool dirtySmall; // SaySmall dirty inspector status
		private bool dirtyMedium; // SayMedum dirty inspector status
		private bool dirtyLarge; // SayLarge dirty inspector status
		private bool dirtyHair; // Facial hair dirty inspector status

		private float width = 0f; // Inspector width
		private float addWidth = 10f; // Percentage
		private float deleteWidth = 10f; // Percentage
		private float shapeNameWidth = 60f; // Percentage
		private float percentageWidth = 30f; // Percentage

		public void OnEnable()
		{
			// Get reference
			fuseSync = target as CM_FuseSync;

			// Initialize
			if (fuseSync.initialize)
			{
				fuseSync.GetSalsa3D();
				fuseSync.GetRandomEyes3D();
				fuseSync.GetBody();
				fuseSync.GetEyeLashes();
				fuseSync.GetEyeBones();
                if (fuseSync.facialHair.Count == 0) fuseSync.GetFacialHair();
				if (fuseSync.saySmall == null) fuseSync.saySmall = new List<CM_ShapeGroup>();
				if (fuseSync.sayMedium == null) fuseSync.sayMedium = new List<CM_ShapeGroup>();
				if (fuseSync.sayLarge == null) fuseSync.sayLarge = new List<CM_ShapeGroup>();
				fuseSync.GetShapeNames();
				fuseSync.SetDefaultSmall();
				fuseSync.SetDefaultMedium();
				fuseSync.SetDefaultLarge();
				fuseSync.initialize = false;
			}
		}

		public override void OnInspectorGUI()
		{
			// Minus 45 width for the scroll bar
			width = Screen.width - 60f;

			// Set dirty flags
			dirtySmall = false; 
			dirtyMedium = false;
			dirtyLarge = false;
			dirtyHair = false;

			// Keep trying to get the shapeNames until I've got them
			if (fuseSync.GetShapeNames() == 0) fuseSync.GetShapeNames();

			// Make sure the CM_ShapeGroups are initialized
			if (fuseSync.saySmall == null) fuseSync.saySmall = new System.Collections.Generic.List<CM_ShapeGroup>();
			if (fuseSync.sayMedium == null) fuseSync.sayMedium = new System.Collections.Generic.List<CM_ShapeGroup>();
			if (fuseSync.sayLarge == null) fuseSync.sayLarge = new System.Collections.Generic.List<CM_ShapeGroup>();

			GUILayout.Space(10);
			EditorGUILayout.BeginVertical();
            {
                fuseSync.salsa3D = EditorGUILayout.ObjectField(
                    "Salsa3D", fuseSync.salsa3D, typeof(Salsa3D), true) as Salsa3D;
                fuseSync.randomEyes3D = EditorGUILayout.ObjectField(
                    "RandomEyes3D", fuseSync.randomEyes3D, typeof(RandomEyes3D), true) as RandomEyes3D;
                fuseSync.body = EditorGUILayout.ObjectField(
                    "Body", fuseSync.body, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
                fuseSync.eyelashes = EditorGUILayout.ObjectField(
                    "Eyelashes", fuseSync.eyelashes, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
                fuseSync.leftEyeBone = EditorGUILayout.ObjectField(
                    "Left Eye Bone", fuseSync.leftEyeBone, typeof(GameObject), true) as GameObject;
                fuseSync.rightEyeBone = EditorGUILayout.ObjectField(
                    "Right Eye Bone", fuseSync.rightEyeBone, typeof(GameObject), true) as GameObject;
                fuseSync.blinkEyelidPercent = EditorGUILayout.Slider(
                    "Eyelid blink percent", fuseSync.blinkEyelidPercent, 0f, 100f);
                fuseSync.blinkEyelashPercent = EditorGUILayout.Slider(
                    "Eyelash blink percent", fuseSync.blinkEyelashPercent, 0f, 100f);
            }
			EditorGUILayout.EndVertical();

			GUILayout.Space(20);

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Other SkinnedMeshRenderers (Beards, Moustaches, Teeth, etc.)");
					if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
					{
						fuseSync.facialHair.Add(new SkinnedMeshRenderer());
					}
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical();
					{
						if (fuseSync.facialHair.Count > 0)
						{
							for (int i = 0; i < fuseSync.facialHair.Count; i++)
							{
								GUILayout.BeginHorizontal();
								if (GUILayout.Button(
									new GUIContent("X", "Remove this SkinnedMeshRenderer from the list"),
									new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
								{
									fuseSync.facialHair.RemoveAt(i);
									dirtyHair = true;
								}
								if (!dirtyHair) fuseSync.facialHair[i] = EditorGUILayout.ObjectField(
									fuseSync.facialHair[i], typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
								GUILayout.EndHorizontal();
							}
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();

			GUILayout.Space(20);

			if (fuseSync.body)
			{
				EditorGUILayout.LabelField("SALSA shape groups");
				GUILayout.Space(10);
				
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("SaySmall Shapes");
						if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
						{
							fuseSync.saySmall.Add(new CM_ShapeGroup());
							fuseSync.initialize = false;
						}
					}
					EditorGUILayout.EndHorizontal();
	
					if (fuseSync.saySmall.Count > 0)
					{
						GUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField(
								new GUIContent("Delete", "Remove shape"),
								GUILayout.Width((deleteWidth / 100) * width));
							EditorGUILayout.LabelField(
								new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
								GUILayout.Width((shapeNameWidth / 100) * width));
							EditorGUILayout.LabelField(
								new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
								GUILayout.Width((percentageWidth / 100) * width));
						}
						GUILayout.EndHorizontal();
	
						for (int i=0; i<fuseSync.saySmall.Count; i++)
						{
							GUILayout.BeginHorizontal();
							{
								if (GUILayout.Button(
									new GUIContent("X", "Remove this shape from the list (index:" + fuseSync.saySmall[i].shapeIndex + ")"),
									GUILayout.Width((deleteWidth / 100) * width)))
								{
									fuseSync.saySmall.RemoveAt(i);
									dirtySmall = true;
									break;
								}
								if (!dirtySmall)
								{
									fuseSync.saySmall[i].shapeIndex = EditorGUILayout.Popup(
										fuseSync.saySmall[i].shapeIndex, fuseSync.shapeNames,
										GUILayout.Width((shapeNameWidth / 100) * width));
									fuseSync.saySmall[i].shapeName =
										fuseSync.body.sharedMesh.GetBlendShapeName(
											fuseSync.saySmall[i].shapeIndex);
									fuseSync.saySmall[i].percentage = EditorGUILayout.Slider(
										fuseSync.saySmall[i].percentage, 0f, 100f,
										GUILayout.Width((percentageWidth / 100) * width));
									fuseSync.initialize = false;
								}
							}
							GUILayout.EndHorizontal();
						}
					}
				}
				EditorGUILayout.EndVertical();


				GUILayout.Space(10);
				
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("SayMedium Shapes");
						if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
						{
							fuseSync.sayMedium.Add(new CM_ShapeGroup());
							fuseSync.initialize = false;
						}
					}
					EditorGUILayout.EndHorizontal();
	
					if (fuseSync.sayMedium.Count > 0)
					{
						GUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField(
								new GUIContent("Delete", "Remove shape"),
								GUILayout.Width((deleteWidth / 100) * width));
							EditorGUILayout.LabelField(
								new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
								GUILayout.Width((shapeNameWidth / 100) * width));
							EditorGUILayout.LabelField(
								new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
								GUILayout.Width((percentageWidth / 100) * width));
						}
						GUILayout.EndHorizontal();
						
						for (int i=0; i<fuseSync.sayMedium.Count; i++)
						{
							GUILayout.BeginHorizontal();
							{
								if (GUILayout.Button(
									new GUIContent("X", "Remove this shape from the list (index:" + fuseSync.sayMedium[i].shapeIndex + ")"),
									GUILayout.Width((deleteWidth / 100) * width)))
								{
									fuseSync.sayMedium.RemoveAt(i);
									dirtyMedium = true;
									break;
								}
								if (!dirtyMedium)
								{
									fuseSync.sayMedium[i].shapeIndex = EditorGUILayout.Popup(
										fuseSync.sayMedium[i].shapeIndex, fuseSync.shapeNames,
										GUILayout.Width((shapeNameWidth / 100) * width));
									fuseSync.sayMedium[i].shapeName =
										fuseSync.body.sharedMesh.GetBlendShapeName(
											fuseSync.sayMedium[i].shapeIndex);
									fuseSync.sayMedium[i].percentage = EditorGUILayout.Slider(
										fuseSync.sayMedium[i].percentage, 0f, 100f,
										GUILayout.Width((percentageWidth / 100) * width));
									fuseSync.initialize = false;
								}
							}
							GUILayout.EndHorizontal();
						}
					}
				}
				EditorGUILayout.EndVertical();
				
				GUILayout.Space(10);
				
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("SayLarge Shapes");
						if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
						{
							fuseSync.sayLarge.Add(new CM_ShapeGroup());
							fuseSync.initialize = false;
						}
					}
					EditorGUILayout.EndHorizontal();
	
					if (fuseSync.sayLarge.Count > 0)
					{
						GUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField(
								new GUIContent("Delete", "Remove shape"),
								GUILayout.Width((deleteWidth / 100) * width));
							EditorGUILayout.LabelField(
								new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
								GUILayout.Width((shapeNameWidth / 100) * width));
							EditorGUILayout.LabelField(
								new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
								GUILayout.Width((percentageWidth / 100) * width));
						}
						GUILayout.EndHorizontal();
						
						for (int i=0; i<fuseSync.sayLarge.Count; i++)
						{
							GUILayout.BeginHorizontal();
							{
								if (GUILayout.Button(
									new GUIContent("X", "Remove this shape from the list (index:" + fuseSync.sayLarge[i].shapeIndex + ")"),
									GUILayout.Width((deleteWidth / 100) * width)))
								{
									fuseSync.sayLarge.RemoveAt(i);
									dirtyLarge = true;
									break;
								}
								if (!dirtyLarge)
								{
									fuseSync.sayLarge[i].shapeIndex = EditorGUILayout.Popup(
										fuseSync.sayLarge[i].shapeIndex, fuseSync.shapeNames,
										GUILayout.Width((shapeNameWidth / 100) * width));
									fuseSync.sayLarge[i].shapeName = fuseSync.body.sharedMesh.GetBlendShapeName(
										fuseSync.sayLarge[i].shapeIndex);
									fuseSync.sayLarge[i].percentage = EditorGUILayout.Slider(
										fuseSync.sayLarge[i].percentage, 0f, 100f,
										GUILayout.Width((percentageWidth / 100) * width));
									fuseSync.initialize = false;
								}
							}
							GUILayout.EndHorizontal();
						}
					}
				}
				EditorGUILayout.EndVertical();
			}
		}
	}
}
