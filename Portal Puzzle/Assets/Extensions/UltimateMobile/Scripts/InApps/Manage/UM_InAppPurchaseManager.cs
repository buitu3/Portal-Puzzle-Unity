//#define ATC_SUPPORT_ENABLED

using UnityEngine;
using UnionAssets.FLE;
using System;
using System.Collections;
using System.Collections.Generic;

#if ATC_SUPPORT_ENABLED
using CodeStage.AntiCheat.ObscuredTypes;
#endif

public class UM_InAppPurchaseManager : SA_Singleton<UM_InAppPurchaseManager> {

	//events
	public const string ON_PURCHASE_FLOW_FINISHED   = "on_purchase_flow_finished";
	public const string ON_BILLING_CONNECT_FINISHED   = "on_billing_connect_finished";



	//actions
	public static Action<UM_BillingConnectionResult> OnBillingConnectFinishedAction = delegate {};
	public static Action<UM_PurchaseResult> OnPurchaseFlowFinishedAction = delegate {};

	private bool _IsInited = false;
	private bool _IsPurchasingAvailable = false;


	public const string PREFS_KEY = "UM_InAppPurchaseManager";


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);


		switch(Application.platform) {
			
		case RuntimePlatform.Android:
			//listening for purchase and consume events
			AndroidInAppPurchaseManager.ActionProductPurchased += Android_ActionProductPurchased;
			AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_PRODUCT_CONSUMED,  Android_OnProductConsumed);
			
			//initilaizing store
			AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_BILLING_SETUP_FINISHED, Android_OnBillingConnected);
			AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, Android_OnRetrieveProductsFinised);

			break;
			
		case RuntimePlatform.IPhonePlayer:
			IOSInAppPurchaseManager.instance.OnStoreKitInitComplete += IOS_OnStoreKitInitComplete;
			IOSInAppPurchaseManager.instance.OnTransactionComplete  += IOS_OnTransactionComplete;
			break;

		case RuntimePlatform.WP8Player:
			WP8InAppPurchasesManager.instance.addEventListener(WP8InAppPurchasesManager.INITIALIZED, WP8_OnInitComplete);
			WP8InAppPurchasesManager.instance.addEventListener(WP8InAppPurchasesManager.PRODUCT_PURCHASE_FINISHED, WP8_OnProductPurchased);
			break;
		}

	}


	public void Init() {
		switch(Application.platform) {
			
		case RuntimePlatform.Android:
			foreach(UM_InAppProduct product in UltimateMobileSettings.Instance.InAppProducts) {
				AndroidInAppPurchaseManager.instance.addProduct(product.AndroidId);
			}
			AndroidInAppPurchaseManager.instance.loadStore();
			break;
			
		case RuntimePlatform.IPhonePlayer:
			foreach(UM_InAppProduct product in UltimateMobileSettings.Instance.InAppProducts) {
				IOSInAppPurchaseManager.instance.addProductId(product.IOSId);
			}
			IOSInAppPurchaseManager.instance.loadStore();
			break;
			
			
		case RuntimePlatform.WP8Player:
			WP8InAppPurchasesManager.instance.init();
			break;
			
			
		}
	}

	//--------------------------------------
	// Methods
	//--------------------------------------




	public void Purchase(string productId) {

		UM_InAppProduct p = UltimateMobileSettings.Instance.GetProductById(productId);


		if(p != null) {
			switch(Application.platform) {
				
			case RuntimePlatform.Android:
				AndroidInAppPurchaseManager.instance.purchase(p.AndroidId);
				break;
				
			case RuntimePlatform.IPhonePlayer:

				IOSInAppPurchaseManager.instance.buyProduct(p.IOSId);
				break;
				
				
			case RuntimePlatform.WP8Player:
				WP8InAppPurchasesManager.instance.purchase(p.WP8Id);
				break;
			}


		} else {
			Debug.LogWarning("UM_InAppPurchaseManager product not found: " + productId);
		}

	}

	public bool IsProductPurchased(string id) {
		return IsProductPurchased(UltimateMobileSettings.Instance.GetProductById(id));
	}


	public bool IsProductPurchased(UM_InAppProduct product) {
		if(product == null) {
			return false;
		}

		switch(Application.platform) {
			
		case RuntimePlatform.Android:

			if(AndroidInAppPurchaseManager.instance.IsInventoryLoaded) {
				return AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(product.AndroidId);
			} else {
				#if ATC_SUPPORT_ENABLED
				return ObscuredPrefs.HasKey(PREFS_KEY + product.id);
				#else
				return PlayerPrefs.HasKey(PREFS_KEY + product.id);
				#endif

			}
			 
		case RuntimePlatform.IPhonePlayer:
			#if ATC_SUPPORT_ENABLED
			return ObscuredPrefs.HasKey(PREFS_KEY + product.id);
			#else
			return PlayerPrefs.HasKey(PREFS_KEY + product.id);
			#endif


		case RuntimePlatform.WP8Player:
			if(WP8InAppPurchasesManager.instance.IsInitialized) {
				return product.WP8Template.isPurchased;
			} else {
				#if ATC_SUPPORT_ENABLED
				return ObscuredPrefs.HasKey(PREFS_KEY + product.id);
				#else
				return PlayerPrefs.HasKey(PREFS_KEY + product.id);
				#endif


			}
		}

		return false;
	}


	public void DeleteNonConsumablePurchaseRecord(UM_InAppProduct product) {
		#if ATC_SUPPORT_ENABLED
		ObscuredPrefs.DeleteKey(PREFS_KEY + product.id);
		#else
		PlayerPrefs.DeleteKey(PREFS_KEY + product.id);
		#endif
	}

	public void RestorePurchases() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			IOSInAppPurchaseManager.instance.restorePurchases();
		}
	}

	

	public List<UM_InAppProduct> InAppProducts {
		get {
			return UltimateMobileSettings.Instance.InAppProducts;
		}
	}

	public UM_InAppProduct GetProductById(string id) {
		return UltimateMobileSettings.Instance.GetProductById(id);
	}
	
	
	public UM_InAppProduct GetProductByIOSId(string id) {
		return UltimateMobileSettings.Instance.GetProductByIOSId(id);
	}
	
	
	public UM_InAppProduct GetProductByAndroidId(string id) {
		return UltimateMobileSettings.Instance.GetProductByAndroidId(id);
	}
	
	public UM_InAppProduct GetProductByWp8Id(string id) {
		return UltimateMobileSettings.Instance.GetProductByWp8Id(id);
	}


	//--------------------------------------
	// Get / SET
	//--------------------------------------

	public bool IsInited {
		get {
			return _IsInited;
		}
	}

	public bool IsPurchasingAvailable {
		get {
			return _IsPurchasingAvailable;
		}
	}

	//--------------------------------------
	// WP8 Listners
	//--------------------------------------

	private void WP8_OnInitComplete() {

		_IsInited = true;
		_IsPurchasingAvailable = true;

		UM_BillingConnectionResult r =  new UM_BillingConnectionResult();
		r.message = "Inited";
		r.isSuccess = true;


		foreach(UM_InAppProduct product in UltimateMobileSettings.Instance.InAppProducts) {


			WP8ProductTemplate tpl =  WP8InAppPurchasesManager.instance.GetProductById(product.WP8Id);
			if(tpl != null) {
				product.SetTemplate(tpl);
				if(product.WP8Template.isPurchased && !product.IsConsumable) {
					SaveNonConsumableItemPurchaseInfo(product);
				}
			}
			
		}

		OnBillingConnectFinishedAction(r);
		dispatch(ON_BILLING_CONNECT_FINISHED, r);
	}

	private void WP8_OnProductPurchased(CEvent e) {
		WP8PurchseResponce recponce = e.data as WP8PurchseResponce;

		UM_InAppProduct p = UltimateMobileSettings.Instance.GetProductByWp8Id(recponce.productId);
		if(p != null) {
			UM_PurchaseResult r =  new UM_PurchaseResult();
			r.product = p;
			r.WP8_PurchaseInfo = recponce;

			SendPurchaseEvent(r);
		} else {
			SendNoTemplateEvent();
		}
	}

	//--------------------------------------
	// IOS Listners
	//--------------------------------------
	

	private void IOS_OnTransactionComplete (IOSStoreKitResponse responce) {

		UM_InAppProduct p = UltimateMobileSettings.Instance.GetProductByIOSId(responce.productIdentifier);
		if(p != null) {
			UM_PurchaseResult r =  new UM_PurchaseResult();
			r.product = p;
			r.IOS_PurchaseInfo = responce;
			SendPurchaseEvent(r);
		} else {
			SendNoTemplateEvent();
		}


	}

	private void IOS_OnStoreKitInitComplete (ISN_Result res) {

		UM_BillingConnectionResult r =  new UM_BillingConnectionResult();
		_IsInited = true;
		_IsPurchasingAvailable = res.IsSucceeded;
		r.isSuccess = res.IsSucceeded;
		if(res.IsSucceeded) {
			r.message = "Inited";

			foreach(UM_InAppProduct product in UltimateMobileSettings.Instance.InAppProducts) {
				
				IOSProductTemplate tpl = IOSInAppPurchaseManager.instance.GetProductById(product.IOSId); 
				if(tpl != null) {
					product.SetTemplate(tpl);
				}
				
			}

			OnBillingConnectFinishedAction(r);
			dispatch(ON_BILLING_CONNECT_FINISHED, r);
		} else {

			if(res.error != null) {
				r.message = res.error.description;
			}

			OnBillingConnectFinishedAction(r);
			dispatch(ON_BILLING_CONNECT_FINISHED, r);
		}

	}
	

	//--------------------------------------
	// Android Listners
	//--------------------------------------




	void Android_ActionProductPurchased (BillingResult result) {
		UM_InAppProduct p = UltimateMobileSettings.Instance.GetProductByAndroidId(result.purchase.SKU);

		if(p != null) {
			if(p.IsConsumable && result.isSuccess) {
				AndroidInAppPurchaseManager.instance.consume(result.purchase.SKU);
			} else {
				
				UM_PurchaseResult r =  new UM_PurchaseResult();
				r.isSuccess = result.isSuccess;
				r.product = p;
				r.Google_PurchaseInfo = result.purchase;
				
				SendPurchaseEvent(r);
			}
		} else {
			SendNoTemplateEvent();
		}
	}	
	
	private void Android_OnProductConsumed(CEvent e) {
		BillingResult result = e.data as BillingResult;

		UM_InAppProduct p = UltimateMobileSettings.Instance.GetProductByAndroidId(result.purchase.SKU);
		if(p != null) {
			UM_PurchaseResult r =  new UM_PurchaseResult();
			r.isSuccess = result.isSuccess;
			r.product = p;
			r.Google_PurchaseInfo = result.purchase;
			SendPurchaseEvent(r);
		} else {
			SendNoTemplateEvent();
		}
	}
	
	
	private void Android_OnBillingConnected(CEvent e) {
		BillingResult result = e.data as BillingResult;
		UM_BillingConnectionResult r =  new UM_BillingConnectionResult();
		AndroidInAppPurchaseManager.instance.removeEventListener (AndroidInAppPurchaseManager.ON_BILLING_SETUP_FINISHED, Android_OnBillingConnected);
		
		
		if(result.isSuccess) {
			//Store connection is Successful. Next we loading product and customer purchasing details
			r.isSuccess = true;
			AndroidInAppPurchaseManager.instance.retrieveProducDetails();

		} else {
			r.isSuccess = false;
			dispatch(ON_BILLING_CONNECT_FINISHED, r);
		}


		r.message = result.message;

		
	}
	
	private void Android_OnRetrieveProductsFinised(CEvent e) {
		_IsInited = true;

		BillingResult result = e.data as BillingResult;
		AndroidInAppPurchaseManager.instance.removeEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, Android_OnRetrieveProductsFinised);

		UM_BillingConnectionResult r =  new UM_BillingConnectionResult();
		r.message = result.message;
		r.isSuccess = result.isSuccess;
		_IsPurchasingAvailable = r.isSuccess;
		if(r.isSuccess) {
			foreach(UM_InAppProduct product in UltimateMobileSettings.Instance.InAppProducts) {
				GoogleProductTemplate tpl = AndroidInAppPurchaseManager.instance.inventory.GetProductDetails(product.AndroidId);
				if(tpl != null) {
					product.SetTemplate(tpl);
					if(product.IsConsumable && AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(product.AndroidId)) {
						AndroidInAppPurchaseManager.instance.consume(product.AndroidId);
					}

					if(!product.IsConsumable && AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(product.AndroidId)) {
						SaveNonConsumableItemPurchaseInfo(product);
					}
				}

			}
		}


		OnBillingConnectFinishedAction(r);
		dispatch(ON_BILLING_CONNECT_FINISHED, r);
		
	}


	//--------------------------------------
	// Private Methods
	//--------------------------------------

	private void SendPurchaseEvent(UM_PurchaseResult result) {


		switch(Application.platform) {
			
		case RuntimePlatform.Android:
			break;
		case RuntimePlatform.IPhonePlayer:

			switch(result.IOS_PurchaseInfo.state) {
			case InAppPurchaseState.Purchased:
			case InAppPurchaseState.Restored:
				result.isSuccess = true;
				break;
			case InAppPurchaseState.Deferred:
			case InAppPurchaseState.Failed:
				result.isSuccess = false;
				break;
			}
			break;
		case RuntimePlatform.WP8Player:
			result.isSuccess = result.WP8_PurchaseInfo.IsSuccses;
			break;
		}


		if(!result.product.IsConsumable && result.isSuccess) {
			Debug.Log("UM: Purchase saved to PlayerPrefs");
			SaveNonConsumableItemPurchaseInfo(result.product);
		}

		OnPurchaseFlowFinishedAction(result);
		dispatch(ON_PURCHASE_FLOW_FINISHED, result);
	}

	private void SaveNonConsumableItemPurchaseInfo(UM_InAppProduct product) {
		#if ATC_SUPPORT_ENABLED
		ObscuredPrefs.SetInt(PREFS_KEY + product.id, 1);
		#else
		PlayerPrefs.SetInt(PREFS_KEY + product.id, 1);
		#endif
	}


	private void SendNoTemplateEvent() {
		Debug.LogWarning("UM: Product tamplate not found");
		UM_PurchaseResult r =  new UM_PurchaseResult();
		OnPurchaseFlowFinishedAction(r);
		dispatch(ON_PURCHASE_FLOW_FINISHED, r);
	}

}
