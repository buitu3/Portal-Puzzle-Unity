//
//  GoogleMobileAdController.h
//  Unity-iPhone
//
//  Created by lacost on 1/16/14.
//
//

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

/*
#import "GADBannerView.h"
#import "GADInterstitial.h"
#import "GADInAppPurchase.h"
*/
 
 
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

@interface GoogleMobileAdController : NSObject<GADInterstitialDelegate, GADInAppPurchaseDelegate>

@property(nonatomic, strong) GADInAppPurchase *purchase;

 + (id) sharedInstance;
 - (void) initAd:(NSString*) unit_id;
 - (void) ChangeBannersUnitID:(NSString*) unit_id;
 - (void) ChangeInterstisialsUnitID:(NSString*) unit_id;
 - (void) addKeyword:(NSString*) keyword;
 - (void) AddTestDevice:(NSString*) uid;
 - (void) AddTestDevices:(NSString*) uids;
 - (void) SetGender:(int) gender;
 - (void) setBirthday:(int)day month: (int)month year: (int) year;
 - (void) tagForChildDirectedTreatment:(BOOL) val;
 - (void) Refresh: (int) bannerId;
 - (void) CreateBannerAd:(int) gravity size: (int) size bannerId: (int) bannerId;
 - (void) CreateBannerAd:(int) x y: (int) y size: (int) size bannerId: (int) bannerId;
 - (void) ShowAd: (int) bannerId;
 - (void) HideAd: (int) bannerId;
 - (void) SetPosition:(int)gravity bannerId: (int) bannerId;
 - (void) SetPosition:(int)x y:(int)y bannerId: (int) bannerId;
 - (void) DestroyBanner: (int) bannerId;

 - (void) StartInterstitialAd;
 - (void) LoadInterstitialAd;
 - (void) ShowInterstitialAd;


- (void) reportPurchaseStatus: (int) value;


- (NSString*) GetUnitId;
- (GADRequest*) GetAdRequest;



@end
