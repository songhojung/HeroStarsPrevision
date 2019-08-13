
#import "NativeBinding.h"
#import "KeychainItemWrapper.h"
#include <Security/SecItem.h>

#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import "webviewHandler.h"
extern "C"
{    
    //NSString to char*
    char* _Unity_MakeStringCopy(NSString* s)
    {
        char* ret = NULL;
        
        if(s != nil)
        {
            const char* src = [s UTF8String];
            if(src){
                ret = (char*)malloc(strlen(src) + 1);
                strcpy(ret, src);
            }
        }
        return ret;
    }
    
    char* getUUID()
    {
            //NSLog(@"getUUID");
            // initialize keychaing item for saving UUID.
            KeychainItemWrapper *wrapper =
            [[KeychainItemWrapper alloc] initWithIdentifier:@"UUID"
                     accessGroup:nil];
            
            NSString *uuid = [wrapper objectForKey:(id)kSecAttrAccount];
        
            //NSLog(@"uuid:%@", uuid);
            if( uuid == nil || uuid.length == 0)
                {
                    
                    // if there is not UUID in keychain, make UUID and save it.
                    CFUUIDRef uuidRef = CFUUIDCreate(NULL);
                    CFStringRef uuidStringRef = CFUUIDCreateString(NULL, uuidRef);
                    CFRelease(uuidRef);
                    uuid = [NSString stringWithString:(NSString *) uuidStringRef];
                    CFRelease(uuidStringRef);
                    
                    // save UUID in keychain
                    [wrapper setObject:uuid forKey:(id)kSecAttrAccount];
                    
                    //NSLog(@"if uuid:%@", uuid);
                    }
        
        return _Unity_MakeStringCopy(uuid);
    }
    
    void showWebView(char* _url)
    {
        //UIWebView *webView = [[UIWebView alloc]init];
//        NSString *urlString = @"http://www.sourcefreeze.com";
//        NSURL *url = [NSURL URLWithString:urlString];
//        NSURLRequest *urlRequest = [NSURLRequest requestWithURL:url];
//        [webView loadRequest:urlRequest];
        //[view addSubview:webView];
        
        
//        if (webView == nil)
//            return;
        //NSString *urlStr = @"http://www.naver.com";
//        NSLog(@"urlsetting");
//        NSURL *nsurl = [NSURL URLWithString:urlStr];
//        NSURLRequest *request = [NSURLRequest requestWithURL:nsurl];
//        [webView loadRequest:request];
         NSLog(@"IOS webview");
        
        NSString* nsStr =[[NSString alloc] initWithCString:_url encoding:NSUTF8StringEncoding];
        [[webviewHandler webviewInstance] showWebView:nsStr size:[[UIScreen mainScreen] bounds].size scrollPos: 0 checkBoxOn:NO];
        [nsStr release];
    }
    
    char* getCountryCode()
    {
        NSLocale *currentLocale = [NSLocale currentLocale];
        NSString *country = [currentLocale objectForKey:NSLocaleCountryCode];
        
        
        return _Unity_MakeStringCopy(country);
    
    }
}

