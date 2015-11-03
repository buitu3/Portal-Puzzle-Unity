//
//  GoogleMobileAdBanner.h
//  Unity-iPhone
//
//  Created by lacost on 2/8/14.
//
//

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
//#import "GADBannerView.h"
//#import "GADInterstitial.h"
#import "GoogleMobileAdController.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

@interface GoogleMobileAdBanner : NSObject<GADBannerViewDelegate>

    @property (strong)  GADBannerView *bannerView;
    @property (strong)  NSNumber *bid;

    - (void) InitBanner:(int) size bannerId: (int) bannerId;
    - (void) CreateBanner:(int) gravity size: (int) size bannerId: (int) bannerId;
    - (void) CreateBannerAdPos:(int) x y: (int) y size: (int) size bannerId: (int) bannerId;

    - (void) Refresh;
    - (void) ShowAd;
    - (void) HideAd;
    - (void) SetPosition:(int)gravity;
    - (void) SetPosition:(int)x y:(int)y;
@end
