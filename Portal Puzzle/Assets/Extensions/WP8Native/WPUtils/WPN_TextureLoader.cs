////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using UnionAssets.FLE;
using System.Collections;

public class WPN_TextureLoader : EventDispatcher {

	private string _url;

	public static WPN_TextureLoader Create() {
		return new GameObject("WPN_TextureLoader").AddComponent<WPN_TextureLoader>();
	}

	public void LoadTexture(string url) {
		_url = url;
		StartCoroutine(LoadCoroutin());
	}


	private IEnumerator LoadCoroutin () {
		// Start a download of the given URL
		WWW www = new WWW (_url);

		// Wait for download to complete
		yield return www;

		dispatch(BaseEvent.LOADED, www.texture);
	}

}
