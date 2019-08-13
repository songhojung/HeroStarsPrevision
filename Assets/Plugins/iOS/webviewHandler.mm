//
//  webviewHandler.m
//  Unity-iPhone
//
//  Created by R&D on 12. 7. 26..
//  Copyright (c) 2012??__MyCompanyName__. All rights reserved.
//

#import "webviewHandler.h"
#import <CoreGraphics/CoreGraphics.h>
#import <QuartzCore/QuartzCore.h>

//void UnityPause(bool pause);
@implementation webviewHandler

+ (webviewHandler*)webviewInstance
{
    static webviewHandler *webviewInstance = nil;
    if (!webviewInstance) {
        webviewInstance = [[webviewHandler alloc] init];
        [webviewInstance initData];
    }
    return webviewInstance;
}

-(void) initData
{
    CGSize ScreenSize = [[UIScreen mainScreen] bounds].size;
    ncPoint = CGPointMake(ScreenSize.width / 2,ScreenSize.height / 2);
    ratioPoint = CGPointMake((ncPoint.x / 160), (ncPoint.y / 240));
    //if(ratioPoint.x > 2) ratioPoint.x = 2;
    //if(ratioPoint.y > 2) ratioPoint.y = 2;
    
    nCheckBoxHei = 40 * ratioPoint.y;

    NSString* m_Lang = [[NSLocale preferredLanguages] objectAtIndex: 0];
    
     m_bKr = [m_Lang containsString:@"ko"];
     NSLog(@"current lang:%@",m_Lang);
}

-(void) showWebView:(NSString *)urlStr size:(CGSize )size scrollPos:(float)pos  checkBoxOn:(bool)checkBoxOn
{
    scrollPos = pos;
    [self showWebView:urlStr size:size checkBoxOn:checkBoxOn];
}

- (void) showWebView:(NSString *)urlStr size:(CGSize )size  checkBoxOn:(bool)checkBoxOn
{
    [self stopIndicatorView];
    //UnityPause(true);
    m_bLoadCom = false;
    /*
    size.width = size.width>320?320:size.width;
    size.height = size.height>480?480:size.height;

    size.width *= ratioPoint.x;
    size.height *= ratioPoint.y;
    */
    UIView *EGView = [[[[UIApplication sharedApplication] keyWindow] subviews] objectAtIndex:0];
    
    if (![EGView viewWithTag:10])
    {
        m_pWebView = [[UIWebView alloc]initWithFrame:CGRectMake(0, 0, size.width, size.height - nCheckBoxHei) ];

        // Create colored border using CALayer property
        //[[m_pWebView layer] setBorderColor:[[UIColor grayColor] CGColor]];
        //[[m_pWebView layer] setBorderWidth:3];
        //m_pWebView.contentMode = UIViewContentModeScaleAspectFit;
        //[m_pWebView setCenter:ncPoint];
        [m_pWebView setDelegate:self];
        [m_pWebView setTag:10];
       
        //[webView.scrollView setBounces:YES];
        
        // Round corners using CALayer property
        //[[m_pWebView layer] setCornerRadius:10];
        //[m_pWebView.layer setMasksToBounds:YES];
        
                                     
        //UIButton *closeBtn = [UIButton buttonWithType:UIButtonTypeCustom];
        //UIImage *closeBtnImg = [UIImage imageNamed:@"btn_URL_close.png"];
        //[closeBtn setFrame:CGRectMake(size.width - (closeBtnImg.size.width / 2), closeBtnImg.size.height / 2, closeBtnImg.size.width, closeBtnImg.size.height)];
        //[closeBtn setImage:closeBtnImg forState:UIControlStateNormal];
        
        /*
        UIButton *closeBtn = [UIButton buttonWithType:UIButtonTypeRoundedRect];
        [closeBtn setTitle:@"Close" forState:UIControlStateNormal];
        [closeBtn setFrame:CGRectMake(size.width - 46, 3, 46, 46)];
        //[closeBtn setTitleColor:[UIColor colorWithRed:0 green:0 blue:0 alpha:1] forState:UIControlStateNormal];
        
        
        [m_pWebView addSubview:closeBtn];
        [closeBtn addTarget:self action:@selector(removeFromWebView:) forControlEvents:UIControlEventTouchUpInside];
        */
        //UINavigationBar *myBar = [[UINavigationBar alloc]initWithFrame:CGRectMake(0, 0, size.width, 50)];
        m_pWebviewBar = [[UINavigationBar alloc]initWithFrame:CGRectMake(0, size.height - nCheckBoxHei, size.width, nCheckBoxHei)];
        [EGView addSubview:m_pWebviewBar];
        UIBarButtonItem *backButton = [[UIBarButtonItem alloc]
                                       initWithTitle:m_bKr ? @"닫기" : @"Close"
                                       style:UIBarButtonItemStyleDone
                                       target:self
                                       action:@selector(removeFromWebView:)];
        
        UINavigationItem *item = [[UINavigationItem alloc]init];
        [item setRightBarButtonItem:backButton];
        
        if(checkBoxOn)
        {
            UIBarButtonItem *checkButton = [[UIBarButtonItem alloc]
                                           initWithTitle:m_bKr ? @"오늘 하루 그만보기" : @"Close For Today"
                                           style:UIBarButtonItemStyleDone
                                           target:self
                                           action:@selector(removeFromWebViewToday:)];
            [item setLeftBarButtonItem:checkButton];
        }

        
        
        [m_pWebviewBar setItems:[NSArray arrayWithObject:item]];

        
        
        [m_pWebView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:urlStr]]];

        //Test 오늘하루 그만 보기
        /*
        if(checkBoxOn)
        {
            //버튼을 커스텀으로 작성
            UIButton *checkbox = [UIButton buttonWithType:UIButtonTypeCustom];
            UIImage *checkOnImg =  [UIImage imageNamed: m_bKr ? @"checkboxOn.png" : @"checkboxOn_en.png"];
            UIImage *checkOffImg = [UIImage imageNamed: m_bKr ? @"checkboxOff.png": @"checkboxOff_en.png"];
        
            //위치 및 크기 조절
            [checkbox setFrame:CGRectMake(0, size.height - nCheckBoxHei, size.width, nCheckBoxHei)];
            //backgroundColor
            //checkbox.backgroundColor = [UIColor whiteColor];
            //노말상태에서 타이틀지정
            [checkbox setImage:checkOffImg forState:UIControlStateNormal];
            //선택된상태에서 타이틀지정
            [checkbox setImage:checkOnImg forState:UIControlStateSelected];
        
            [checkbox setSelected:m_bCheckBoxCheck];
        
            //클릭시 이벤트 지정
            [checkbox addTarget:self action:@selector(onCheckBox:) forControlEvents:UIControlEventTouchUpInside];
        
            [m_pWebView addSubview:checkbox];
        }
        */
        [EGView addSubview:m_pWebView];

        [self startIndicatorView];
        
    }
    
}


-(UIView *)indicatorView
{
       
    UIView *boxView = [[UIView alloc]initWithFrame:CGRectMake(0, 0, 60, 60)];
    [boxView setCenter:ncPoint];
    [boxView setBackgroundColor:[UIColor colorWithRed:0 green:0 blue:0 alpha:0.7]];
    [boxView setTag:21];
    
    UIActivityIndicatorView *indicator = [[UIActivityIndicatorView alloc]initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge];
    [indicator setCenter:CGPointMake(boxView.frame.size.width/2, boxView.frame.size.height/2)];
    [indicator setTag:20];
    [boxView addSubview:indicator];
    
    return boxView;
    
}

-(void)startIndicatorView
{
    if(!m_bShowLoading)
    {
        UIView *EGView = [[[[UIApplication sharedApplication] keyWindow] subviews] objectAtIndex:0];
        [EGView addSubview:[self indicatorView]];
        [self startIndicator];
        
        m_bShowLoading = YES;
    }
    
}

-(void) startIndicator
{
    
    UIView *EGView = [[[[UIApplication sharedApplication] keyWindow] subviews] objectAtIndex:0];
    UIView *boxView = [EGView viewWithTag:21];
    UIActivityIndicatorView *indicator = (UIActivityIndicatorView *)[boxView viewWithTag:20];
    
    [indicator startAnimating];
    
}

-(void) stopIndicatorView
{
    [self stopIndicator];
}

-(void) stopIndicator
{
    
    if(m_bShowLoading)
    {
        UIView *EGView = [[[[UIApplication sharedApplication] keyWindow] subviews] objectAtIndex:0];
        UIView *boxView = [EGView viewWithTag:21];
        if([boxView viewWithTag:20] != nil)
        {
            UIActivityIndicatorView *indicator = (UIActivityIndicatorView *)[boxView viewWithTag:20];
            
            [indicator stopAnimating];   
        }
        [boxView removeFromSuperview];
        m_bShowLoading = NO;
    }
}

- (void)removeFromWebView:(id)sender
{
    [self stopIndicator];
    m_pWebView.delegate = nil;
    [m_pWebView removeFromSuperview];
   // [m_pWebView release];
    [m_pWebviewBar removeFromSuperview];
  //  [m_pWebviewBar release];
    //UnityPause(false);
    //UnitySendMessage("NativeListener", "setWebViewClose", "False");
}

- (void)removeFromWebViewToday:(id)sender
{
    [self stopIndicator];
    m_pWebView.delegate = nil;
    [m_pWebView removeFromSuperview];
    //[m_pWebView release];
    [m_pWebviewBar removeFromSuperview];
//    [m_pWebviewBar release];
   // UnityPause(false);
   // UnitySendMessage("NativeListener", "setWebViewClose", "True");
}
/*
-(void) removeFromWebView:(UIButton *)tmpBtn
{
    //if(m_bLoadCom)
    {
        [self stopIndicator];
        [[tmpBtn superview] removeFromSuperview];        
        m_pWebView.delegate = nil;
        [m_pWebView release];
        UnityPause(false);
        UnitySendMessage("NativeListener", "setWebViewClose", m_bCheckBoxCheck ? "True" : "False");
    }
    
}
*/
- (void)webViewDidFinishLoad:(UIWebView *)pWebView
{
    [self stopIndicator];
    m_bLoadCom = true;
    
    if ([[UIDevice currentDevice] userInterfaceIdiom] != UIUserInterfaceIdiomPhone) scrollPos /= 2;
    
    if(scrollPos > 0)
    {       
        int height = [[pWebView stringByEvaluatingJavaScriptFromString:@"document.body.offsetHeight;"] intValue]; 
        if(scrollPos < height)
            pWebView.scrollView.contentOffset = CGPointMake(0, scrollPos); //webView.scrollView support is IOS5.0 later....
    }
}

-(void)onCheckBox:(id)sender
{
    //이벤트 발생버튼 지정
    UIButton *button = sender;
    //선택값을 반전 시켜줌
    button.selected=!button.selected;
    
    m_bCheckBoxCheck = button.selected;
}




@end
