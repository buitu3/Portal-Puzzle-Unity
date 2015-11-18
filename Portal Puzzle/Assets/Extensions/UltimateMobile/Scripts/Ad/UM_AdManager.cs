using UnityEngine;
using System.Collections;

public class UM_AdManager : SA_Singleton<UM_AdManager> {

	private  bool _IsInited = false ;

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public  void Init() {


	
		switch(Application.platform) {
			case RuntimePlatform.IPhonePlayer:
				if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
					GoogleMobileAd.Init();
				}
				break;
			case RuntimePlatform.Android:
				GoogleMobileAd.Init();
				break;
			case RuntimePlatform.WP8Player:
				GoogleMobileAd.Init();
				break;
		}

		
	
		
		_IsInited = true;
		
	}
	
	

	
	public int CreateAdBanner(TextAnchor anchor)  {
		if(!_IsInited) {
			Debug.LogWarning ("CreateBannerAd shoudl be called only after Init function. Call ignored");
			return 0;
		}

		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				return GoogleMobileAd.CreateAdBanner(anchor, GADBannerSize.BANNER).id;
			} else {
				return iAdBannerController.instance.CreateAdBanner(anchor).id;
			}
		case RuntimePlatform.Android:
			return GoogleMobileAd.CreateAdBanner(anchor, GADBannerSize.BANNER).id;
		case RuntimePlatform.WP8Player:
			return GoogleMobileAd.CreateAdBanner(anchor, GADBannerSize.BANNER).id;
		}

		return 0;

	}
	


	public bool IsBannerLoaded(int id) {

		if(!_IsInited) {
			Debug.LogWarning ("IsBannerLoaded shoudl be called only after Init function. Call ignored");
			return false;
		}


		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			
			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				return GoogleMobileAd.GetBanner(id).IsLoaded;
			} else {
				return iAdBannerController.instance.GetBanner(id).IsLoaded;
			}
		case RuntimePlatform.Android:
			return GoogleMobileAd.GetBanner(id).IsLoaded;
		case RuntimePlatform.WP8Player:
			return GoogleMobileAd.GetBanner(id).IsLoaded;
		}

		return false;
		
	}

	public bool IsBannerOnScreen(int id) {

		if(!_IsInited) {
			Debug.LogWarning ("IsBannerOnScreen shoudl be called only after Init function. Call ignored");
			return false;
		}

		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			
			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				return GoogleMobileAd.GetBanner(id).IsOnScreen;
			} else {
				return iAdBannerController.instance.GetBanner(id).IsOnScreen;
			}
		case RuntimePlatform.Android:
			return GoogleMobileAd.GetBanner(id).IsOnScreen;
		case RuntimePlatform.WP8Player:
			return GoogleMobileAd.GetBanner(id).IsOnScreen;
		}
		
		return false;
		
	}


	public void DestroyBanner(int id) {
		if(!_IsInited) {
			Debug.LogWarning ("DestroyCurrentBanner shoudl be called only after Init function. Call ignored");
			return;
		}
		
		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:

			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				GoogleMobileAd.DestroyBanner(id);
			} else {
				iAdBannerController.instance.DestroyBanner(id);
			}
			break;
		case RuntimePlatform.Android:
			GoogleMobileAd.DestroyBanner(id);
			break;
		case RuntimePlatform.WP8Player:
			GoogleMobileAd.DestroyBanner(id);
			break;
		}
	}


	public void HideBanner(int id) {
		if(!_IsInited) {
			Debug.LogWarning ("DestroyCurrentBanner shoudl be called only after Init function. Call ignored");
			return;
		}
		
		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			
			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				GoogleMobileAd.GetBanner(id).Hide();
			} else {
				iAdBannerController.instance.GetBanner(id).Hide();
			}
			break;
		case RuntimePlatform.Android:
			GoogleMobileAd.GetBanner(id).Hide();
			break;
		case RuntimePlatform.WP8Player:
			GoogleMobileAd.GetBanner(id).Hide();
			break;
		}
	}




	public void ShowBanner(int id) {
		if(!_IsInited) {
			Debug.LogWarning ("DestroyCurrentBanner shoudl be called only after Init function. Call ignored");
			return;
		}
		
		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			
			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				GoogleMobileAd.GetBanner(id).Show();
			} else {
				iAdBannerController.instance.GetBanner(id).Show();
			}
			break;
		case RuntimePlatform.Android:
			GoogleMobileAd.GetBanner(id).Show();
			break;
		case RuntimePlatform.WP8Player:
			GoogleMobileAd.GetBanner(id).Show();
			break;
		}
	}


	public void RefreshBanner(int id) {
		if(!_IsInited) {
			Debug.LogWarning ("DestroyCurrentBanner shoudl be called only after Init function. Call ignored");
			return;
		}
		
		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			
			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				GoogleMobileAd.GetBanner(id).Refresh();
			}
			break;
		case RuntimePlatform.Android:
			GoogleMobileAd.GetBanner(id).Refresh();
			break;
		case RuntimePlatform.WP8Player:
			GoogleMobileAd.GetBanner(id).Refresh();
			break;
		}
	}
	


	public void StartInterstitialAd() {
		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			
			if(UltimateMobileSettings.Instance.IOSAdEdngine == UM_IOSAdEngineOprions.GoogleMobileAd)  {
				GoogleMobileAd.StartInterstitialAd();
			} else {
				iAdBannerController.instance.StartInterstitialAd();
			}
			break;
		case RuntimePlatform.Android:
			GoogleMobileAd.StartInterstitialAd();
			break;
		case RuntimePlatform.WP8Player:
			GoogleMobileAd.StartInterstitialAd();
			break;
		}
	}
	


	//--------------------------------------
	//  GET / SET
	//--------------------------------------
	
	
	public bool IsInited {
		get {
			return _IsInited;
		}
	}
	

	
	//--------------------------------------
	// EVENTS Impl
	//--------------------------------------

}
