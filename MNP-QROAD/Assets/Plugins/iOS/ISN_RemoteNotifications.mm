#if !TARGET_OS_TV

//
//  ISNCamera.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 6/10/14.
//
//

#import <Foundation/Foundation.h>
#import "ISN_NativeCore.h"
#import "AppDelegateListener.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

NSString * const UNITY_SPLITTER = @"|";



@interface ISN_RemoteNotifications : NSObject

-(void) RegisterForRemoteNotifications;

@end


@implementation ISN_RemoteNotifications

static ISN_RemoteNotifications * rem_sharedInstance;


+ (id)sharedInstance {
    
    if (rem_sharedInstance == nil)  {
        rem_sharedInstance = [[self alloc] init];
    }
    
    return rem_sharedInstance;
}


- (id)init {
    if ((self = [super init])) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"[ISN_RemoteNotifications] Init"];
        
        NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
        
        [notificationCenter addObserver: self
                               selector: @selector (handle_DidRegisterForRemoteNotificationsWithDeviceToken:)
                                   name: kUnityDidRegisterForRemoteNotificationsWithDeviceToken
                                 object: nil];
        
        
        [notificationCenter addObserver: self
                               selector: @selector (handle_DidFailToRegisterForRemoteNotificationsWithError:)
                                   name: kUnityDidFailToRegisterForRemoteNotificationsWithError
                                 object: nil];
        
        
        [notificationCenter addObserver: self
                               selector: @selector (handle_DidReceiveRemoteNotification:)
                                   name: kUnityDidReceiveRemoteNotification
                                 object: nil];
        
    }
    
    return self;
}





//--------------------------------------
//  Public Methods
//--------------------------------------


-(void) RegisterForRemoteNotifications {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"[ISN_RemoteNotifications] RegisterForRemoteNotifications"];
    
    UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert |  UIUserNotificationTypeBadge | UIUserNotificationTypeSound categories:nil];
    [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
    [[UIApplication sharedApplication] registerForRemoteNotifications];
}


//--------------------------------------
//  Observers
//--------------------------------------


- (void) handle_DidReceiveRemoteNotification: (NSNotification*)receivedNotification {
    
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"[ISN_RemoteNotifications] handle_DidReceiveRemoteNotification"];
    
    
    NSDictionary* dic = (NSDictionary*) receivedNotification.userInfo;
    NSString* JSONString = [dic AsJSONString];
    UnitySendMessage("ISN_RemoteNotificationsController", "DidReceiveRemoteNotification",  [ISN_DataConvertor NSStringToChar:JSONString] );
}



- (void) handle_DidFailToRegisterForRemoteNotificationsWithError: (NSNotification*)receivedNotification {
    
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"[ISN_RemoteNotifications] handle_DidFailToRegisterForRemoteNotificationsWithError"];
    NSError* error = (NSError*) receivedNotification.userInfo;
    
    
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: error.description];
    UnitySendMessage("ISN_RemoteNotificationsController", "DidFailToRegisterForRemoteNotifications",  [ISN_DataConvertor serializeError:error] );
    
}


- (void) handle_DidRegisterForRemoteNotificationsWithDeviceToken: (NSNotification *) receivedNotification {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"[ISN_RemoteNotifications] handle_DidRegisterForRemoteNotificationsWithDeviceToken"];
    NSData* deviceToken = (NSData*) receivedNotification.userInfo;
    NSString * token = [NSString stringWithFormat:@"%@", deviceToken];
    
    token = [token stringByReplacingOccurrencesOfString:@" " withString:@""];
    token = [token stringByReplacingOccurrencesOfString:@">" withString:@""];
    token = [token stringByReplacingOccurrencesOfString:@"<" withString:@""];
    
    
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    [data appendString:token];
    [data appendString:UNITY_SPLITTER];
    [data appendString:[deviceToken AsBase64String]];
    
    
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog:token];
    UnitySendMessage("ISN_RemoteNotificationsController", "DidRegisterForRemoteNotifications", [ISN_DataConvertor NSStringToChar:data]);
    
}





extern "C" {
    
    
    //--------------------------------------
    //  IOS Native Plugin Section
    //--------------------------------------
    
    void _ISN_RegisterForRemoteNotifications() {
        [[ISN_RemoteNotifications sharedInstance] RegisterForRemoteNotifications];
    }
    
    
}


@end

#endif
