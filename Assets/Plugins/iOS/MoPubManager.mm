//
//  MopubManager.m
//  MoPub
//
//  Copyright (c) 2017 __MyCompanyName__. All rights reserved.
//

#import "MoPubManager.h"


#ifdef __cplusplus
extern "C" {
#endif
    // life cycle management
    void UnityPause(int pause);
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
#ifdef __cplusplus
}
#endif


@implementation MoPubManager

@synthesize adView = _adView, locationManager = _locationManager, lastKnownLocation = _lastKnownLocation, bannerPosition;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

// Manager to be used for methods that do not require a specific adunit to operate on.
+ (MoPubManager*)sharedManager
{
    static MoPubManager* sharedManager = nil;

    if (!sharedManager)
        sharedManager = [[MoPubManager alloc] init];

    return sharedManager;
}

// Manager to be used for adunit specific methods
+ (MoPubManager*)managerForAdunit:(NSString*)adUnitId
{
    static NSMutableDictionary* managerDict = nil;

    if (!managerDict)
        managerDict = [[NSMutableDictionary alloc] init];

    MoPubManager* manager = [managerDict valueForKey:adUnitId];
    if (!manager) {
        manager = [[MoPubManager alloc] initWithAdUnit:adUnitId];
        managerDict[adUnitId] = manager;
    }

    return manager;
}


+ (UIViewController*)unityViewController
{
    return [[[UIApplication sharedApplication] keyWindow] rootViewController];
}


+ (void)sendUnityEvent:(NSString*)eventName withArgs:(NSArray*)args
{
    NSData* data = [NSJSONSerialization dataWithJSONObject:args options:0 error:nil];
    UnitySendMessage("MoPubManager", eventName.UTF8String, [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding].UTF8String);
}


- (void)sendUnityEvent:(NSString*)eventName
{
    [[self class] sendUnityEvent:eventName withArgs:@[_adUnitId]];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (void)adjustAdViewFrameToShowAdView
{
    if (@available(iOS 11.0, *)) {
        UIView* superview = _adView.superview;
        if (superview) {
            _adView.translatesAutoresizingMaskIntoConstraints = NO;
            NSMutableArray<NSLayoutConstraint*>* constraints = [NSMutableArray arrayWithArray:@[
                [_adView.widthAnchor constraintEqualToConstant:CGRectGetWidth(_adView.frame)],
                [_adView.heightAnchor constraintEqualToConstant:CGRectGetHeight(_adView.frame)],
            ]];
            switch(bannerPosition) {
                case MoPubAdPositionTopLeft:
                    [constraints addObjectsFromArray:@[[_adView.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor],
                                                       [_adView.leftAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.leftAnchor]]];
                    break;
                case MoPubAdPositionTopCenter:
                    [constraints addObjectsFromArray:@[[_adView.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor],
                                                       [_adView.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor]]];
                    break;
                case MoPubAdPositionTopRight:
                    [constraints addObjectsFromArray:@[[_adView.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor],
                                                       [_adView.rightAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.rightAnchor]]];
                    break;
                case MoPubAdPositionCentered:
                    [constraints addObjectsFromArray:@[[_adView.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor],
                                                       [_adView.centerYAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerYAnchor]]];
                    break;
                case MoPubAdPositionBottomLeft:
                    [constraints addObjectsFromArray:@[[_adView.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor],
                                                       [_adView.leftAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.leftAnchor]]];
                    break;
                case MoPubAdPositionBottomCenter:
                    [constraints addObjectsFromArray:@[[_adView.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor],
                                                       [_adView.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor]]];
                    break;
                case MoPubAdPositionBottomRight:
                    [constraints addObjectsFromArray:@[[_adView.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor],
                                                       [_adView.rightAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.rightAnchor]]];
                    break;
            }
            [NSLayoutConstraint activateConstraints:constraints];
            NSLog(@"setting adView frame: %@", NSStringFromCGRect(_adView.frame));
        } else
            NSLog(@"_adview.superview was nil! Was the ad view not added to another view?@");
    } else {
        // fetch screen dimensions and useful values
        CGRect origFrame = _adView.frame;

        CGFloat screenHeight = [UIScreen mainScreen].bounds.size.height;
        CGFloat screenWidth = [UIScreen mainScreen].bounds.size.width;

        switch(bannerPosition) {
            case MoPubAdPositionTopLeft:
                origFrame.origin.x = 0;
                origFrame.origin.y = 0;
                _adView.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case MoPubAdPositionTopCenter:
                origFrame.origin.x = (screenWidth / 2) - (origFrame.size.width / 2);
                _adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case MoPubAdPositionTopRight:
                origFrame.origin.x = screenWidth - origFrame.size.width;
                origFrame.origin.y = 0;
                _adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case MoPubAdPositionCentered:
                origFrame.origin.x = (screenWidth / 2) - (origFrame.size.width / 2);
                origFrame.origin.y = (screenHeight / 2) - (origFrame.size.height / 2);
                _adView.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case MoPubAdPositionBottomLeft:
                origFrame.origin.x = 0;
                origFrame.origin.y = screenHeight - origFrame.size.height;
                _adView.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
            case MoPubAdPositionBottomCenter:
                origFrame.origin.x = (screenWidth / 2) - (origFrame.size.width / 2);
                origFrame.origin.y = screenHeight - origFrame.size.height;
                _adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
            case MoPubAdPositionBottomRight:
                origFrame.origin.x = screenWidth - _adView.frame.size.width;
                origFrame.origin.y = screenHeight - origFrame.size.height;
                _adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
        }

        _adView.frame = origFrame;
        NSLog(@"setting adView frame: %@", NSStringFromCGRect(origFrame));
    }
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (id)initWithAdUnit:(NSString*)adUnitId
{
    self = [super init];
    if (self) self->_adUnitId = adUnitId;
    return self;
}

- (void)enableLocationSupport:(BOOL)shouldEnable
{
    if (_locationEnabled == shouldEnable)
        return;

    _locationEnabled = shouldEnable;

    // are we stopping or starting location use?
    if (_locationEnabled) {
        // autorelease and retain just in case we have an old one to avoid leaking
        self.locationManager = [[CLLocationManager alloc] init];
        self.locationManager.delegate = self;
        self.locationManager.distanceFilter = 100;
        self.locationManager.desiredAccuracy = kCLLocationAccuracyBest;

        // Make sure the user has location on in settings
        if ([CLLocationManager locationServicesEnabled]) {
            // Only start updating if we can get location information
            [self.locationManager startUpdatingLocation];
        } else {
            _locationEnabled = NO;
            self.locationManager = nil;
        }
    } else { // turning off
        [self.locationManager stopUpdatingLocation];
        self.locationManager.delegate = nil;
        self.locationManager = nil;
    }
}


- (void)createBanner:(MoPubBannerType)bannerType atPosition:(MoPubAdPosition)position
{
    // kill the current adView if we have one
    if (_adView)
        [self hideBanner:YES];

    bannerPosition = position;

    switch (bannerType) {
        case MoPubBannerType_320x50: {
            _adView = [[MPAdView alloc] initWithAdUnitId:_adUnitId size:MOPUB_BANNER_SIZE];
            [_adView lockNativeAdsToOrientation:MPNativeAdOrientationPortrait];
            break;
        }
        case MoPubBannerType_728x90: {
            _adView = [[MPAdView alloc] initWithAdUnitId:_adUnitId size:MOPUB_LEADERBOARD_SIZE];
            [_adView lockNativeAdsToOrientation:MPNativeAdOrientationPortrait];
            break;
        }
        case MoPubBannerType_160x600: {
            _adView = [[MPAdView alloc] initWithAdUnitId:_adUnitId size:MOPUB_WIDE_SKYSCRAPER_SIZE];
            break;
        }
        case MoPubBannerType_300x250: {
            _adView = [[MPAdView alloc] initWithAdUnitId:_adUnitId size:MOPUB_MEDIUM_RECT_SIZE];
            break;
        }
    }

    // do we have location enabled?
    if (_locationEnabled && _lastKnownLocation)
        _adView.location = _lastKnownLocation;

    _adView.delegate = self;
    _autorefresh = YES;
    [[MoPubManager unityViewController].view addSubview:_adView];
    [_adView loadAd];
}


- (void)destroyBanner
{
    [_adView removeFromSuperview];
    _adView.delegate = nil;
    self.adView = nil;
}


- (void)showBanner
{
    if (!_adView)
        return;

    _adView.hidden = NO;
    if (_autorefresh)
        [_adView startAutomaticallyRefreshingContents];
}


- (void)hideBanner:(BOOL)shouldDestroy
{
    if (!_adView)
        return;

    _adView.hidden = YES;
    [_adView stopAutomaticallyRefreshingContents];

    if (shouldDestroy)
        [self destroyBanner];
}


- (void)refreshAd:(NSString*)keywords userDataKeywords:(NSString*)userDataKeywords
{
    if (!_adView)
        return;
    _adView.keywords = keywords;
    _adView.userDataKeywords = userDataKeywords;
    [_adView loadAd];
}


- (void)setAutorefreshEnabled:(BOOL)enabled
{
    _autorefresh = enabled;
    if (!_adView || _adView.hidden)
        return;
    else if (enabled)
        [_adView startAutomaticallyRefreshingContents];
    else
        [_adView stopAutomaticallyRefreshingContents];
}


- (void)forceRefresh
{
    if (!_adView)
        return;
    [_adView forceRefreshAd];
}


- (void)requestInterstitialAd:(NSString*)keywords userDataKeywords:(NSString*)userDataKeywords
{
    MPInterstitialAdController* interstitial = [MPInterstitialAdController interstitialAdControllerForAdUnitId:_adUnitId];

    if (_locationEnabled && _lastKnownLocation)
        interstitial.location = _lastKnownLocation;

    interstitial.keywords = keywords;
    interstitial.userDataKeywords = userDataKeywords;
    interstitial.delegate = self;
    [interstitial loadAd];
}


- (BOOL)interstitialIsReady
{
    return [MPInterstitialAdController interstitialAdControllerForAdUnitId:_adUnitId].ready;
}


- (void)showInterstitialAd
{
    MPInterstitialAdController* interstitial = [MPInterstitialAdController interstitialAdControllerForAdUnitId:_adUnitId];
    interstitial.delegate = self;
    if (!interstitial.ready) {
        NSLog(@"interstitial ad is not yet loaded");
        return;
    }

    [interstitial showFromViewController:[MoPubManager unityViewController]];
}


- (void)destroyInterstitialAd
{
    MPInterstitialAdController* interstitial = [MPInterstitialAdController interstitialAdControllerForAdUnitId:_adUnitId];

    if ([interstitial isViewLoaded])
        [interstitial removeFromParentViewController];
    interstitial.delegate = nil;

    [MPInterstitialAdController removeSharedInterstitialAdController:interstitial];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - MPAdViewDelegate

- (UIViewController*)viewControllerForPresentingModalView 
{
    return [MoPubManager unityViewController];
}


/*
*  These callbacks notify you regarding whether the ad view (un)successfully
*  loaded an ad.
*/
- (void)adViewDidFailToLoadAd:(MPAdView*)view
{
    _adView.hidden = YES;
    [self sendUnityEvent:@"EmitAdFailedEvent"];
}


- (void)adViewDidLoadAd:(MPAdView*)view
{
    // resize the banner
    CGRect newFrame = _adView.frame;
    newFrame.size = _adView.adContentViewSize;
    _adView.frame = newFrame;

    [self adjustAdViewFrameToShowAdView];
    _adView.hidden = NO;

    [[self class] sendUnityEvent:@"EmitAdLoadedEvent" withArgs:@[_adUnitId, @(_adView.frame.size.height)]];
}


/*
*  These callbacks are triggered when the ad view is about to present/dismiss a
*  modal view. If your application may be disrupted by these actions, you can
*  use these notifications to handle them (for example, a game might need to
*  pause/unpause).
*/
- (void)willPresentModalViewForAd:(MPAdView*)view
{
    NSLog(@"willPresentModalViewForAd");
    [self sendUnityEvent:@"EmitAdExpandedEvent"];
    UnityPause(true);
}


- (void)didDismissModalViewForAd:(MPAdView*)view
{
    NSLog(@"didDismissModalViewForAd");
    [self sendUnityEvent:@"EmitAdCollapsedEvent"];
    UnityPause(false);
}


/*
*  This callback is triggered when the ad view has retrieved ad parameters
*  (headers) from the MoPub server. See MPInterstitialAdController for an
*  example of how this should be used.
 - (void)adView:(MPAdView*)view didReceiveResponseParams:(NSDictionary*)params
 {

 }
*/


- (void)adViewShouldClose:(MPAdView*)view
{
    NSLog(@"adViewShouldClose");
    UnityPause(false);
    [self hideBanner:YES];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - MPInterstitialAdControllerDelegate

- (void)interstitialDidLoadAd:(MPInterstitialAdController*)interstitial
{
    [self sendUnityEvent:@"EmitInterstitialLoadedEvent"];
}


- (void)interstitialDidFailToLoadAd:(MPInterstitialAdController*)interstitial withError:(NSError*)error
{
    NSLog(@"interstitialDidFailToLoadAd adUnit: %@ error: %@", _adUnitId, error);
    [[self class] sendUnityEvent:@"EmitInterstitialFailedEvent" withArgs:@[_adUnitId, error.localizedDescription]];
}


- (void)interstitialDidExpire:(MPInterstitialAdController*)interstitial
{
    [self sendUnityEvent:@"EmitInterstitialShownEvent"];
}


- (void)interstitialWillAppear:(MPInterstitialAdController*)interstitial
{
    UnityPause(true);
    [self sendUnityEvent:@"EmitInterstitialShownEvent"];
}


- (void)interstitialWillDisappear:(MPInterstitialAdController*)interstitial
{
    NSLog(@"interstitialWillDisappear");
    UnityPause(false);
}


- (void)interstitialDidDisappear:(MPInterstitialAdController*)interstitial
{
    NSLog(@"interstitialDidDisappear");
    UnityPause(false);
    [self sendUnityEvent:@"EmitInterstitialDismissedEvent"];
}


- (void)interstitialDidReceiveTapEvent:(MPInterstitialAdController*)interstitial
{
    NSLog(@"interstitialDidReceiveTapEvent");
    [self sendUnityEvent:@"EmitInterstitialClickedEvent"];
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - CLLocationManagerDelegate

- (void)locationManager:(CLLocationManager*)manager didUpdateToLocation:(CLLocation*)newLocation fromLocation:(CLLocation*)oldLocation
{
    // update our locations
    if (_adView)
        _adView.location = newLocation;
    self.lastKnownLocation = newLocation;
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - MPRewardedVideoDelegate

- (void)rewardedVideoAdDidLoadForAdUnitID:(NSString*)adUnitID
{
    NSLog(@"rewardedVideoAdDidLoadForAdUnitID");
    [self sendUnityEvent:@"EmitRewardedVideoLoadedEvent"];
}


- (void)rewardedVideoAdDidFailToLoadForAdUnitID:(NSString*)adUnitID error:(NSError*)error
{
    NSLog(@"rewardedVideoAdDidFailToLoadForAdUnitID adUnit: %@ error: %@", adUnitID, error);
    [[self class] sendUnityEvent:@"EmitRewardedVideoFailedEvent" withArgs:@[adUnitID, error.localizedDescription]];
}


- (void)rewardedVideoAdDidExpireForAdUnitID:(NSString*)adUnitID
{
    NSLog(@"rewardedVideoAdDidExpireForAdUnitID");
    [self sendUnityEvent:@"EmitRewardedVideoExpiredEvent"];
}


- (void)rewardedVideoAdDidFailToPlayForAdUnitID:(NSString*)adUnitID error:(NSError*)error
{
    NSLog(@"rewardedVideoAdDidFailToPlayForAdUnitID adUnit: %@ error: %@", _adUnitId, error);
    [[self class] sendUnityEvent:@"EmitRewardedVideoFailedToPlayEvent" withArgs:@[adUnitID, error.localizedDescription]];
}


- (void)rewardedVideoAdWillAppearForAdUnitID:(NSString*)adUnitID
{
    NSLog(@"rewardedVideoAdWillAppearForAdUnitID");
    UnityPause(true);
    [self sendUnityEvent:@"EmitRewardedVideoShownEvent"];
}


//- (void)rewardedVideoAdDidAppearForAdUnitID:(NSString*)adUnitID;

- (void)rewardedVideoAdDidDisappearForAdUnitID:(NSString*)adUnitID
{
    NSLog(@"rewardedVideoAdDidDisappearForAdUnitID");
    UnityPause(false);
    [self sendUnityEvent:@"EmitRewardedVideoClosedEvent"];
}


//- (void)rewardedVideoAdDidDisappearForAdUnitID:(NSString*)adUnitID;

- (void)rewardedVideoAdDidReceiveTapEventForAdUnitID:(NSString*)adUnitID
{
    NSLog(@"rewardedVideoAdDidReceiveTapEventForAdUnitID");
    [self sendUnityEvent:@"EmitRewardedVideoClickedEvent"];
}

- (void)rewardedVideoAdWillLeaveApplicationForAdUnitID:(NSString*)adUnitID
{
    NSLog(@"rewardedVideoAdWillLeaveApplicationForAdUnitID");
    [self sendUnityEvent:@"EmitRewardedVideoLeavingApplicationEvent"];
}


- (void)rewardedVideoAdShouldRewardForAdUnitID:(NSString*)adUnitID reward:(MPRewardedVideoReward*)reward
{
    NSLog(@"rewardedVideoAdShouldRewardForAdUnitID");
    [[self class] sendUnityEvent:@"EmitRewardedVideoReceivedRewardEvent"
                        withArgs:@[adUnitID, reward.currencyType, reward.amount]];
}

@end
