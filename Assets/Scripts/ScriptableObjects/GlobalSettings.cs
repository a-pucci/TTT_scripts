using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlobalSettings", menuName = "ScriptableObjects/Settings/Global Settings")]
public class GlobalSettings : ScriptableObject {

	#region Fields

	// Static
	private static GlobalSettings instance;
	
	// Public
	public float ballSwingTimer;
	public bool ballsConsidersVerticalWalls;
	public LayerMask characterCollisionMask;
	
	#endregion

	#region Unity Callbacks

	#endregion

	#region Methods
	
	public static GlobalSettings GetInstance() {
		if (!instance) {
			GlobalSettings[] all = Resources.FindObjectsOfTypeAll<GlobalSettings>();
			instance = (all.Length > 0) ? all[0] : null;
		}
		
#if UNITY_EDITOR
		
		if (!instance) {
			string[] configsGUIDs = AssetDatabase.FindAssets("t:" + typeof(GlobalSettings).Name);
			if (configsGUIDs.Length > 0) {
				instance = Resources.Load<GlobalSettings>(AssetDatabase.GUIDToAssetPath(configsGUIDs[0]));
			}
		}
		
#endif

		if (!instance) {
			instance = CreateInstance<GlobalSettings>();
			instance.name = "DefaultGlobalSettings";
		}
		return instance;
	}

	#endregion
}