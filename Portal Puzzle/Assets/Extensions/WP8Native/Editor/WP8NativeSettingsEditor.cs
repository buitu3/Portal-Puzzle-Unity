using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(WP8NativeSettings))]
public class WP8NativeSettingsEditor : Editor {

	/*GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");

	private WP8NativeSettings settings;

	// Use this for initialization
	void Awake () {
	
	}
	
	public override void OnInspectorGUI () {
		settings = WP8NativeSettings.Instance;

		UpgradeGUI ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		AboutGUI ();
	}

	public void UpgradeGUI(){
#if UNITY_5 || UNITY_5_0 || UNITY_5_0_1
		if (!FileStaticAPI.IsFileExists("Plugins/Newtonsoft.Json.pdb") ||
		    !FileStaticAPI.IsFileExists("Plugins/Newtonsoft.Json.dll") ||
		    !FileStaticAPI.IsFileExists("Plugins/Newtonsoft.Json.dll.mdb")) {

			EditorGUILayout.HelpBox("You need upgrade to Unity "  + Application.unityVersion, MessageType.Warning);
			if (GUILayout.Button("Upgrade")) {
				FileStaticAPI.CopyFile("Plugins/StansAssets/Newtonsoft.Json.pdb.txt", "Plugins/Newtonsoft.Json.pdb");
				FileStaticAPI.CopyFile("Plugins/StansAssets/Newtonsoft.Json.dll.txt", "Plugins/Newtonsoft.Json.dll");
				FileStaticAPI.CopyFile("Plugins/StansAssets/Newtonsoft.Json.dll.mdb.txt", "Plugins/Newtonsoft.Json.dll.mdb");
			}
		} else {
			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("WP8Native upgraded for Unity " + Application.unityVersion);
			EditorGUILayout.Space ();

			EditorGUILayout.HelpBox("You can downgrade to previous Unity 4.x versions", MessageType.Info);
			if (GUILayout.Button("Downgrade")) {
				FileStaticAPI.DeleteFile("Plugins/Newtonsoft.Json.pdb");
				FileStaticAPI.DeleteFile("Plugins/Newtonsoft.Json.dll");
				FileStaticAPI.DeleteFile("Plugins/Newtonsoft.Json.dll.mdb");
			}
		}
#endif
	}

	private void AboutGUI() {
		EditorGUILayout.HelpBox("About WP8Native", MessageType.None);
		EditorGUILayout.Space();

		SelectableLabelField(SdkVersion, WP8NativeSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
	}

	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}*/
}
