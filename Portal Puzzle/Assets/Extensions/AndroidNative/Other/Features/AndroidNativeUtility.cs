using System;
using UnityEngine;
using System.Collections;

public class AndroidNativeUtility : SA_Singleton<AndroidNativeUtility> {


	//Events
	public static string PACKAGE_CHECK_RESPONCE = "package_check_responce";


	//Actions
	public event Action<AN_PackageCheckResult> OnPackageCheckResult = delegate{};
	public event Action<string> OnAndroidIdLoaded = delegate{};

	public event Action<string> InternalStoragePathLoaded = delegate{};
	public event Action<string> ExternalStoragePathLoaded = delegate{};


	public event Action<AN_Locale> LocaleInfoLoaded = delegate{};

	
	//--------------------------------------
	// Init
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------
	
	
	public void CheckIsPackageInstalled(string packageName) {
		AndroidNative.isPackageInstalled(packageName);
	}

	public void RunPackage(string packageName) {
		AndroidNative.runPackage(packageName);
	}

	public void LoadAndroidId() {
		AndroidNative.LoadAndroidId();
	}


	public void GetInternalStoragePath() {
		AndroidNative.GetInternalStoragePath();
	}
	
	public void GetExternalStoragePath() {
		AndroidNative.GetExternalStoragePath();
	}

	public void LoadLocaleInfo() {
		AndroidNative.LoadLocaleInfo();
	}


	//--------------------------------------
	// Static Methods
	//--------------------------------------

	public static void ShowPreloader(string title, string message) {
		AN_PoupsProxy.ShowPreloader(title, message);
	}
	
	public static void HidePreloader() {
		AN_PoupsProxy.HidePreloader();
	}


	public static void OpenAppRatingPage(string url) {
		AN_PoupsProxy.OpenAppRatePage(url);
	}



	public static void HideCurrentPopup() {
		AN_PoupsProxy.HideCurrentPopup();
	}


	//--------------------------------------
	// Events
	//--------------------------------------

	private void OnAndroidIdLoadedEvent(string id) {
		OnAndroidIdLoaded(id);
	}

	private void OnPacakgeFound(string packageName) {
		AN_PackageCheckResult result = new AN_PackageCheckResult(packageName, true);
		OnPackageCheckResult(result);
		dispatch(PACKAGE_CHECK_RESPONCE, result);
	}

	private void OnPacakgeNotFound(string packageName) {
		AN_PackageCheckResult result = new AN_PackageCheckResult(packageName, false);
		OnPackageCheckResult(result);
		dispatch(PACKAGE_CHECK_RESPONCE, result);
	}


	private void OnExternalStoragePathLoaded(string path) {
		ExternalStoragePathLoaded(path);
	}

	private void OnInternalStoragePathLoaded(string path) {
		InternalStoragePathLoaded(path);
	}


	private void OnLocaleInfoLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AN_Locale locale =  new AN_Locale();
		locale.CountryCode = storeData[0];
		locale.DisplayCountry = storeData[1];

		locale.LanguageCode = storeData[2];
		locale.DisplayLanguage = storeData[3];

		LocaleInfoLoaded(locale);

	}


}

