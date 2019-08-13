//
//  webviewHandler.h
//  Unity-iPhone
//
//  Created by R&D on 12. 7. 26..
//  Copyright (c) 2012년 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface webviewHandler : NSObject<UIWebViewDelegate>{
    bool m_bLoadCom;
    int  scrollPos;
    bool m_bShowLoading;
    bool m_bCheckBoxCheck;
    UIWebView* m_pWebView;
    UINavigationBar *m_pWebviewBar;
    
    CGPoint ncPoint;
    CGPoint ratioPoint;
    
    CGFloat nCheckBoxHei;
	
	bool m_bKr;
}


-(void) showWebView:(NSString *)urlStr size:(CGSize )size  checkBoxOn:(bool)checkBoxOn;
-(void) showWebView:(NSString *)urlStr size:(CGSize )size scrollPos:(float)pos checkBoxOn:(bool)checkBoxOn;
-(void) startIndicatorView;
-(void) stopIndicatorView;

-(void)onCheckBox:(id)sender;
-(void) initData;
+ (webviewHandler*)webviewInstance;

@end
