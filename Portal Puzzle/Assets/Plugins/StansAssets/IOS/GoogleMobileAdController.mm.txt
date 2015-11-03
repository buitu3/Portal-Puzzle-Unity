//
//  GoogleMobileAdController.m
//  Unity-iPhone
//
//  Created by lacost on 1/16/14.
//
//

#import "GoogleMobileAdController.h"
#import "GoogleMobileAdBanner.h"



@implementation GoogleMobileAdController

static NSString * _ad_unit_id;
static NSString * _interstitial_ad_unit_id;
static GoogleMobileAdController *_sharedInstance;

static GADInterstitial *interstitial_ = NULL;
static bool showInterstitialOnLoad = false;

static GADRequest *adRequest;

static NSMutableDictionary* _banners;

+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _banners = [[NSMutableDictionary alloc] init];
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

- (void) initAd:(NSString *)unit_id {
    
    NSLog(@"google ad Inited");
    
    _ad_unit_id = unit_id;
#if UNITY_VERSION < 500
    [_ad_unit_id retain];
#endif
    
    
    _interstitial_ad_unit_id = unit_id;
    
#if UNITY_VERSION < 500
    [_interstitial_ad_unit_id retain];
#endif
    
    
    adRequest =[GADRequest request];
    
    
#if UNITY_VERSION < 500
    [adRequest retain];
#endif
    
    
    
    NSLog(@"ad unity id: %@",_ad_unit_id);
}

-(void) ChangeBannersUnitID:(NSString *)unit_id {
    NSLog(@"ChangeBannersUnitID");
    _ad_unit_id = unit_id;
    
#if UNITY_VERSION < 500
    [_ad_unit_id retain];
#endif
    
    
    NSLog(@"ad unity id: %@",_ad_unit_id);
}

-(void) ChangeInterstisialsUnitID:(NSString *)unit_id {
    NSLog(@"ChangeInterstisialsUnitID");
    _interstitial_ad_unit_id = unit_id;
    
#if UNITY_VERSION < 500
    [_interstitial_ad_unit_id retain];
#endif
    
    
    NSLog(@"interstitial ad unity id: %@",_interstitial_ad_unit_id);
}



- (void) CreateBannerAd:(int)gravity size:(int)size bannerId:(int)bannerId {
    
    GoogleMobileAdBanner* banner;
    banner = [[GoogleMobileAdBanner alloc] init];
    
    [banner CreateBanner:gravity size:size bannerId:bannerId];
    [_banners setObject:banner forKey:[NSNumber numberWithInt:bannerId]];
    
}

-(void) CreateBannerAd:(int)x y:(int)y size:(int)size bannerId:(int)bannerId {
    GoogleMobileAdBanner* banner;
    banner = [[GoogleMobileAdBanner alloc] init];
    
    
    [banner CreateBannerAdPos:x y:y size:size bannerId:bannerId];
    [_banners setObject:banner forKey:[NSNumber numberWithInt:bannerId]];
    
}


- (void) addKeyword:(NSString *)keyword {
    
    //[adRequest addKeyword:keyword];
}

- (void) AddTestDevice:(NSString *)uid {
    
    NSLog(@"Adding TestDevice ID: %@", uid);
    
    NSMutableArray *ids = [[NSMutableArray alloc] init];
    [ids addObject:uid];
    for (NSString *string in [adRequest testDevices]) {
        [ids addObject:string];
    }
    
    adRequest.testDevices = [ids copy];
    
}

- (void) AddTestDevices:(NSString *)uids {
    
    NSArray *items = [uids componentsSeparatedByString:@","];
    
    for(NSString* s in items) {
        [self AddTestDevice:s];
    }
    
}



- (void) SetGender:(int)gender {
    if(gender == 0) {
        adRequest.gender = kGADGenderFemale;
    }
    
    if(gender == 1) {
        adRequest.gender = kGADGenderMale;
    }
    
    if(gender == 2) {
        adRequest.gender = kGADGenderUnknown;
    }
    
}

- (void) setBirthday:(int)day month:(int)month year:(int)year {
    // [adRequest setBirthdayWithMonth:month day:day year:year];
}

- (void) tagForChildDirectedTreatment:(BOOL)val {
    [adRequest tagForChildDirectedTreatment:val];
}

- (void) Refresh:(int)bannerId {
    GoogleMobileAdBanner *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner Refresh];
        
    }
    
    
}

-(void) HideAd:(int)bannerId {
    GoogleMobileAdBanner *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner HideAd];
        
    }
    
}

-(void) ShowAd:(int)bannerId {
    GoogleMobileAdBanner *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner ShowAd];
        
    }
    
}

- (void) SetPosition:(int)gravity bannerId:(int)bannerId {
    GoogleMobileAdBanner *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner SetPosition:gravity];
    }
}

-(void) SetPosition:(int)x y:(int)y bannerId:(int)bannerId {
    GoogleMobileAdBanner *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner SetPosition:x y:y];
    }
}

- (void) DestroyBanner:(int)bannerId {
    GoogleMobileAdBanner *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner HideAd];
        // [[banner bannerView] dealloc];
        
#if UNITY_VERSION < 500
        [banner dealloc];
#endif
        
        
        
    }
    
}


- (void) reportPurchaseStatus:(int)value {
    if(self.purchase != nil) {
        
        switch (value) {
            case 0:
                [self.purchase reportPurchaseStatus:kGADInAppPurchaseStatusSuccessful];
                break;
            case 1:
                [self.purchase reportPurchaseStatus:kGADInAppPurchaseStatusError];
                break;
            case 2:
                [self.purchase reportPurchaseStatus:kGADInAppPurchaseStatusInvalidProduct];
                break;
            case 3:
                [self.purchase reportPurchaseStatus:kGADInAppPurchaseStatusCancel];
                break;
                
            default:
                break;
        }
        self.purchase = nil;
    }
}

#pragma mark getters

- (NSString*) GetUnitId {
    return  _ad_unit_id;
}
- (GADRequest*) GetAdRequest {
    return adRequest;
}


#pragma mark Interstitial

-(void) StartInterstitialAd {
    NSLog(@"StartInterstitialAd");
    [self LoadInterstitialAd];
    showInterstitialOnLoad = true;
    
}

-(void) LoadInterstitialAd {
    interstitial_ = [[GADInterstitial alloc] init];
    interstitial_.adUnitID = _interstitial_ad_unit_id;
    interstitial_.delegate = self;
    interstitial_.inAppPurchaseDelegate = self;
    [interstitial_ loadRequest:adRequest];
    
    showInterstitialOnLoad = false;
}

-(void) ShowInterstitialAd {
    if(interstitial_ != NULL) {
        UIViewController *vc =  UnityGetGLViewController();
        [interstitial_ presentFromRootViewController:vc];
    }
    
}

#pragma mark GADInterstitialDelegate implementation

- (void)interstitialDidReceiveAd:(GADInterstitial *)interstitial {
    NSLog(@"interstitial interstitialDidReceiveAd ");
    
    if(showInterstitialOnLoad) {
        [self ShowInterstitialAd];
    }
    
    UnitySendMessage("IOSAdMobController", "OnInterstitialAdLoaded", "");
    
    
}
- (void)interstitial:(GADInterstitial *)interstitial didFailToReceiveAdWithError:(GADRequestError *)error {
    NSLog(@"interstitial didFailToReceiveAdWithError: %@", error.description);
    
    UnitySendMessage("IOSAdMobController", "OnInterstitialAdFailedToLoad", "");
    
}

- (void)interstitialWillPresentScreen:(GADInterstitial *)interstitial {
    NSLog(@"interstitial interstitialWillPresentScreen ");
    UnitySendMessage("IOSAdMobController", "OnInterstitialAdOpened", "");
    
}


- (void)interstitialDidDismissScreen:(GADInterstitial *)interstitial {
    NSLog(@"interstitial interstitialDidDismissScreen ");
    UnitySendMessage("IOSAdMobController", "OnInterstitialAdClosed", "");
    
}

- (void)interstitialWillLeaveApplication:(GADInterstitial *)interstitial {
    NSLog(@"interstitial interstitialWillLeaveApplication ");
    UnitySendMessage("IOSAdMobController", "OnInterstitialAdLeftApplication", "");
    
}

#pragma mark GADInAppPurchaseDelegate implementation

- (void)didReceiveInAppPurchase:(GADInAppPurchase *)purchase {
    self.purchase = purchase;
    UnitySendMessage("IOSAdMobController", "OnInAppPurchaseRequested",  [purchase.productID UTF8String]);
}





#pragma mark Unity data parce implementation

+(NSString *) charToNSString:(char *)value {
    if (value != NULL) {
        return [NSString stringWithUTF8String: value];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}

+(const char *)NSIntToChar:(NSInteger)value {
    NSString *tmp = [NSString stringWithFormat:@"%d", value];
    return [tmp UTF8String];
}

+ (const char *)NSStringToChar:(NSString *)value {
    return [value UTF8String];
}





extern "C" {
    
    void _initGoogleAd (char* unit_id)  {
        [[GoogleMobileAdController sharedInstance] initAd:[GoogleMobileAdController charToNSString:unit_id ]];
    }
    
    void _GADChangeBannersUnitID (char* unit_id)  {
        [[GoogleMobileAdController sharedInstance] ChangeBannersUnitID:[GoogleMobileAdController charToNSString:unit_id ]];
    }
    
    void _GADChangeInterstisialsUnitID (char* unit_id)  {
        [[GoogleMobileAdController sharedInstance] ChangeInterstisialsUnitID:[GoogleMobileAdController charToNSString:unit_id ]];
    }
    
    void _GADAddKeyWord (char* keyword)  {
        [[GoogleMobileAdController sharedInstance] addKeyword:[GoogleMobileAdController charToNSString:keyword ]];
    }
    
    
    void _GADAddTestDevice (char* uid)  {
        [[GoogleMobileAdController sharedInstance] AddTestDevice:[GoogleMobileAdController charToNSString:uid ]];
    }
    
    void _GADAddTestDevices (char* uids)  {
        [[GoogleMobileAdController sharedInstance] AddTestDevices:[GoogleMobileAdController charToNSString:uids ]];
    }
    
    
    void _GADSetGender (int gender)  {
        [[GoogleMobileAdController sharedInstance] SetGender:gender];
    }
    
    void _GADSetBirthday (int day, int month, int year)  {
        [[GoogleMobileAdController sharedInstance] setBirthday:day month:month year:year];
    }
    
    void _GADTagForChildDirectedTreatment (BOOL val)  {
        [[GoogleMobileAdController sharedInstance] tagForChildDirectedTreatment:val];
        
    }
    
    
    void _GADCreateBannerAd (int gravity, int size, int bannerId)  {
        [[GoogleMobileAdController sharedInstance] CreateBannerAd:gravity size:size bannerId:bannerId];
    }
    
    void _GADCreateBannerAdPos(int x, int y, int size, int bannerId) {
        [[GoogleMobileAdController sharedInstance] CreateBannerAd:x y:y size:size bannerId:bannerId];
    }
    
    
    void _GADRefresh (int bannerId)  {
        [[GoogleMobileAdController sharedInstance] Refresh:bannerId];
    }
    
    
    void _GADShowAd(int bannerId) {
        [[GoogleMobileAdController sharedInstance] ShowAd:bannerId];
    }
    
    void _GADHideAd(int bannerId) {
        [[GoogleMobileAdController sharedInstance] HideAd:bannerId];
    }
    
    void _GADSetPosition(int gravity, int bannerId) {
        [[GoogleMobileAdController sharedInstance] SetPosition:gravity bannerId:bannerId];
    }
    
    void _GADSetPositionXY(int x, int y, int bannerId) {
        [[GoogleMobileAdController sharedInstance] SetPosition:x y:y bannerId:bannerId];
    }
    
    
    void _GADDestroyBanner(int bannerId) {
        [[GoogleMobileAdController sharedInstance] DestroyBanner:bannerId];
    }
    
    
    void _GADStartInterstitialAd() {
        [[GoogleMobileAdController sharedInstance] StartInterstitialAd];
    }
    
    void _GADLoadInterstitialAd() {
        [[GoogleMobileAdController sharedInstance] LoadInterstitialAd];
    }
    
    void _GADShowInterstitialAd() {
        [[GoogleMobileAdController sharedInstance] ShowInterstitialAd];
    }
    
    void _GADReportPurchaseStatus(int status) {
        // [[GoogleMobileAdController sharedInstance] reportPurchaseStatus:status];
    }
    
    
    
}

@end
