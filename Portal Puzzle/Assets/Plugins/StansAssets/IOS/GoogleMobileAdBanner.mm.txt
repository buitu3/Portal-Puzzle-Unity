//
//  GoogleMobileAdBanner.m
//  Unity-iPhone
//
//  Created by lacost on 2/8/14.
//
//

#import "GoogleMobileAdBanner.h"

@implementation GoogleMobileAdBanner


-(void) InitBanner:(int)size bannerId:(int)bannerId {
    NSLog(@"GAD: InitBanner");
    
    
    NSNumber *n = [NSNumber numberWithInt:bannerId];
    [self setBid:n];
    
    
    bool IsLandscape;
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
    
    
    if(size == 1) {
        [self setBannerView:[[GADBannerView alloc] initWithAdSize:kGADAdSizeBanner]];
    }
    
    
    if(size == 2) {
        
        if(IsLandscape) {
            [self setBannerView:[[GADBannerView alloc] initWithAdSize:kGADAdSizeSmartBannerLandscape]];
        } else {
            [self setBannerView:[[GADBannerView alloc] initWithAdSize:kGADAdSizeSmartBannerPortrait]];
        }
        
    }
    
    
    if(size == 3) {
        [self setBannerView:[[GADBannerView alloc] initWithAdSize:kGADAdSizeFullBanner]];
    }
    
    if(size == 4) {
        [self setBannerView:[[GADBannerView alloc] initWithAdSize:kGADAdSizeLeaderboard]];
    }
    
    
    if(size == 5) {
        [self setBannerView:[[GADBannerView alloc] initWithAdSize:kGADAdSizeMediumRectangle]];
    }
}

-(void) StartBannerRequest {
    NSLog(@"GAD: StartBannerRequest");

    UIViewController *vc =  UnityGetGLViewController();
    
    [self bannerView].adUnitID =  [[GoogleMobileAdController sharedInstance] GetUnitId];
    
    [self bannerView].rootViewController = vc;
    [[vc view] addSubview:[self bannerView]];
    [self bannerView].delegate = self;
    [self bannerView].inAppPurchaseDelegate = [GoogleMobileAdController sharedInstance];
    
    [[self bannerView] loadRequest:[[GoogleMobileAdController sharedInstance] GetAdRequest]];
    
    [self HideAd];
    
}



- (void) CreateBannerAdPos:(int)x y:(int)y size:(int)size bannerId:(int)bannerId {
    NSLog(@"GAD: CreateBannerAdPos");
    
    [self InitBanner:size bannerId:bannerId];
    
    [self bannerView].frame = CGRectMake(x,
                                         y,
                                         [self bannerView].frame.size.width,
                                         [self bannerView].frame.size.height);
    
    
    [self StartBannerRequest];
    
}




-(void) CreateBanner:(int)gravity size:(int)size bannerId:(int)bannerId {
     NSLog(@"GAD: CreateBanner");
    
    [self InitBanner:size bannerId:bannerId];
    
    
    float x = 0.0;
    float y = 0.0;
    
    if(gravity == 83) {
        y = [self GetH: 2];
    }
    
    if(gravity == 81) {
        x = [self GetW:1];
        y = [self GetH: 2];
        
    }
    
    if(gravity == 85) {
        x = [self GetW:2];
        y = [self GetH: 2];
        
    }
    
    
    if(gravity == 51) {
        //ziros
    }
    
    if(gravity == 49) {
        x = [self GetW:1];
        
    }
    
    if(gravity == 53) {
        x = [self GetW:2];
    }
    
    if(gravity == 19) {
        y = [self GetH: 1];
    }
    
    if(gravity == 17) {
        x = [self GetW:1];
        y = [self GetH: 1];
        
    }
    
    if(gravity == 21) {
        x = [self GetW:2];
        y = [self GetH: 1];
    }
    
    
    
    [self bannerView].frame = CGRectMake(x,y,
                                         [self bannerView].frame.size.width,
                                         [self bannerView].frame.size.height);
    
    
    
    [self StartBannerRequest];
}

- (float) GetW: (int) p {
    UIViewController *vc =  UnityGetGLViewController();
    
    bool IsLandscape;
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
    
    CGFloat w;
    if(IsLandscape) {
        w = vc.view.frame.size.height;
    } else {
        w = vc.view.frame.size.width;
    }
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        NSLog(@"IOS 8 detected");
        w = vc.view.frame.size.width;
    }
    
    
    if(p == 1) {
        return  (w - [self bannerView].frame.size.width) / 2;
    }
    
    if(p == 2) {
        return  w - [self bannerView].frame.size.width;
    }
    
    return 0.0;
    
    
}

- (float) GetH: (int) p {
    UIViewController *vc =  UnityGetGLViewController();
    
    bool IsLandscape;
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
    
    CGFloat h;
    if(IsLandscape) {
        h = vc.view.frame.size.width;
    } else {
        h = vc.view.frame.size.height;
    }
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        NSLog(@"IOS 8 detected");
        h = vc.view.frame.size.height;
    }
    
    
    if(p == 1) {
        return  (h - [self bannerView].frame.size.height) / 2;
    }
    
    if(p == 2) {
        return  h - [self bannerView].frame.size.height;
    }
    
    return 0.0;
    
    
    
}


- (void) Refresh {
    [[self bannerView] loadRequest:[[GoogleMobileAdController sharedInstance] GetAdRequest]];}

-(void) HideAd {
    [[self bannerView] removeFromSuperview];
    
}

-(void) ShowAd {
    UIViewController *vc =  UnityGetGLViewController();
    [[vc view] addSubview:[self bannerView]];
    
}




-(void) SetPosition:(int)gravity {
    float x = 0.0;
    float y = 0.0;
    
    if(gravity == 83) {
        y = [self GetH: 2];
    }
    
    if(gravity == 81) {
        x = [self GetW:1];
        y = [self GetH: 2];
        
    }
    
    if(gravity == 85) {
        x = [self GetW:2];
        y = [self GetH: 2];
        
    }
    
    
    if(gravity == 51) {
        //ziros
    }
    
    if(gravity == 49) {
        x = [self GetW:1];
        
    }
    
    if(gravity == 53) {
        x = [self GetW:2];
    }
    
    if(gravity == 19) {
        y = [self GetH: 1];
    }
    
    if(gravity == 17) {
        x = [self GetW:1];
        y = [self GetH: 1];
        
    }
    
    if(gravity == 21) {
        x = [self GetW:2];
        y = [self GetH: 1];
    }
    
    
    
    [self bannerView].frame = CGRectMake(x,y,
                                         [self bannerView].frame.size.width,
                                         [self bannerView].frame.size.height);
    
}



-(void) SetPosition:(int)x y:(int)y {
    [self bannerView].frame = CGRectMake(x,
                                         y,
                                         [self bannerView].frame.size.width,
                                         [self bannerView].frame.size.height);
}




#pragma mark GADBannerViewDelegate implementation



// We've received an ad successfully.
- (void)adViewDidReceiveAd:(GADBannerView *)adView {
    NSLog(@"GAD: Received ad successfully");
    
    NSMutableString * data = [[NSMutableString alloc] init];
    NSNumber *w = [NSNumber numberWithInt: (int)[self bannerView].frame.size.width];
    NSNumber *h = [NSNumber numberWithInt: (int)[self bannerView].frame.size.height];
    
    
    
    [data appendString:[[self bid] stringValue]];
    [data appendString:@"|"];
    [data appendString:  [w stringValue]];
    [data appendString:@"|"];
    [data appendString:  [h stringValue]];
    
    
    NSString *str = [data copy];
#if UNITY_VERSION < 500
    [data autorelease];
#endif
    
    UnitySendMessage("IOSAdMobController", "OnBannerAdLoaded", [str UTF8String]);
    
    
    
}

- (void)adView:(GADBannerView *)view didFailToReceiveAdWithError:(GADRequestError *)error {
    NSLog(@"GAD: Failed to receive ad with error: %@", [error localizedFailureReason]);
    UnitySendMessage("IOSAdMobController", "OnBannerAdFailedToLoad", [[[self bid] stringValue] UTF8String]);
    
}

- (void)adViewWillPresentScreen:(GADBannerView *)adView {
    NSLog(@"GAD: OnBannerAdOpened");
    UnitySendMessage("IOSAdMobController", "OnBannerAdOpened", [[[self bid] stringValue] UTF8String]);
}


- (void)adViewDidDismissScreen:(GADBannerView *)adView {
    NSLog(@"GAD: OnBannerAdClosed");
    UnitySendMessage("IOSAdMobController", "OnBannerAdClosed", [[[self bid] stringValue] UTF8String]);
}


- (void)adViewWillLeaveApplication:(GADBannerView *)adView {
    NSLog(@"GAD: OnBannerAdLeftApplication");
    UnitySendMessage("IOSAdMobController", "OnBannerAdLeftApplication", [[[self bid] stringValue] UTF8String]);
}





@end
