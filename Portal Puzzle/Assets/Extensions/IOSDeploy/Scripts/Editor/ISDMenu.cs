using UnityEngine;
using System.Collections;
using UnityEditor;

public class ISDMenu : EditorWindow
{
#if UNITY_EDITOR
	[MenuItem("Window/IOS Deploy/Edit Settings")]
	public static void Edit() {
		Selection.activeObject = ISDSettings.Instance;
	}

	[MenuItem("Window/IOS Deploy/Documentation")]
	public static void Docs() {
		Application.OpenURL("https://goo.gl/06bEsX");
	}
#endif
}
