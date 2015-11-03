////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

public class GoogleAnalyticsMenu : EditorWindow {
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	#if UNITY_EDITOR

	[MenuItem("Window/Google Analytics/Edit Settings", false, 1)]
	public static void Edit() {
		Selection.activeObject = GoogleAnalyticsSettings.Instance;
	}

	[MenuItem("Window/Google Analytics/Create Analytics GameObject")]
	public static void Create() {
		GameObject an = new GameObject("Google Analytics");
		an.AddComponent<GoogleAnalytics>();
		Selection.activeObject = an;
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Browse")]
	public static void OpenDocumentation() {
		string url = "http://goo.gl/Wc8Q3G";
		Application.OpenURL(url);
	}
	
	
	[MenuItem("Window/Google Analytics/Plugin Documentation/Getting Started/Setup")]
	public static void GettingStarted() {
		string url = "https://unionassets.com/google-analytics-sdk/get-started-with-analytics-78";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Getting Started/Tracking Options")]
	public static void p0() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-80";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Basic features")]
	public static void p1() {
		string url = "https://unionassets.com/google-analytics-sdk/using-basic-features-without-scripting-265";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Initialization")]
	public static void p2() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#initialization";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Data reporting")]
	public static void p3() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#data-reporting";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Page Tracking")]
	public static void p4() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#page-tracking";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Event Tracking")]
	public static void p5() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#event-tracking";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Measuring Purchases")]
	public static void p6() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#measuring-purchases";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Measuring Refunds")]
	public static void p7() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#measuring-refunds";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Measuring the Checkout Process")]
	public static void p8() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#measuring-the-checkout-process";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Measuring Internal Promotions")]
	public static void p9() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#measuring-internal-promotions";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Ecommerce Tracking")]
	public static void p10() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#ecommerce-tracking";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Social Interactions")]
	public static void p11() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#social-interactions";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Exception Tracking")]
	public static void p12() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#exception-tracking";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/User Timing Tracking")]
	public static void p13() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#user-timing-tracking";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Screen Tracking")]
	public static void p14() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#screen-tracking";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Scripting API/Custom Hit builder")]
	public static void p15() {
		string url = "https://unionassets.com/google-analytics-sdk/plugin-set-up-82#custom-hit-builder";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/Implementation/Web Player")]
	public static void p16() {
		string url = "https://unionassets.com/google-analytics-sdk/web-player-83";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Plugin Documentation/More")]
	public static void p17() {
		string url = "https://unionassets.com/google-analytics-sdk/manual#more";
		Application.OpenURL(url);
	}
	













	[MenuItem("Window/Google Analytics/Google Documentation/Measurement Protocol Developer Guide")]
	public static void ProtocolDocumentation() {
		string url = "https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Google Analytics/Google Documentation/Measurement Protocol Parameter Reference")]
	public static void ParamDocumentation() {
		string url = "https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters";
		Application.OpenURL(url);
	}





	[MenuItem("Window/Google Analytics/Discussions/Unity Forum")]
	public static void UnityForum() {
		string url = "http://goo.gl/B7YHzf";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Google Analytics/Discussions/PlayMaker Forum")]
	public static void PlayMakerForum() {
		string url = "http://goo.gl/0bLwcT";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Google Analytics/Support")]
	public static void Support() {
		string url = "http://goo.gl/QqSmBM";
		Application.OpenURL(url);
	}
	

	#endif

}
