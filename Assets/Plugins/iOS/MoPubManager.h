//
//  MopubManager.h
//  MoPub
//
//  Copyright (c) 2017 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <MoPubSDKFramework/MoPub.h>


typedef enum
{
    MoPubBannerType_320x50,
    MoPubBannerType_300x250,
    MoPubBannerType_728x90,
    MoPubBannerType_160x600
} MoPubBannerType;

typedef enum
{
    MoPubAdPositionTopLeft,
    MoPubAdPositionTopCenter,
    MoPubAdPositionTopRight,
    MoPubAdPositionCentered,
    MoPubAdPositionBottomLeft,
    MoPubAdPositionBottomCenter,
    MoPubAdPositionBottomRight
} MoPubAdPosition;


@interface MoPubManager : NSObject <MPAdViewDelegate, MPInterstitialAdControllerDelegate, CLLocationManagerDelegate, MPRewardedVideoDelegate>
{
@private
    BOOL _locationEnabled;
    NSString* _adUnitId;
    BOOL _autorefresh;
}
@property (nonatomic, retain) MPAdView *adView;
@property (nonatomic, retain) CLLocationManager *locationManager;
@property (nonatomic, retain) CLLocation *lastKnownLocation;
@property (nonatomic) MoPubAdPosition bannerPosition;


+ (MoPubManager*)sharedManager;

+ (MoPubManager*)managerForAdunit:(NSString*)adUnitId;

+ (UIViewController*)unityViewController;

+ (void)sendUnityEvent:(NSString*)eventName withArgs:(NSArray*)args;

- (void)sendUnityEvent:(NSString*)eventName;

- (id)initWithAdUnit:(NSString*)adUnitId;

- (void)enableLocationSupport:(BOOL)shouldEnable;

- (void)createBanner:(MoPubBannerType)bannerType atPosition:(MoPubAdPosition)position;

- (void)destroyBanner;

- (void)showBanner;

- (void)hideBanner:(BOOL)shouldDestroy;

- (void)setAutorefreshEnabled:(BOOL)enabled;

- (void)forceRefresh;

- (void)refreshAd:(NSString*)keywords userDataKeywords:(NSString*)userDataKeywords;

- (void)requestInterstitialAd:(NSString*)keywords userDataKeywords:(NSString*)userDataKeywords;

- (BOOL)interstitialIsReady;

- (void)showInterstitialAd;

- (void)destroyInterstitialAd;

@end
