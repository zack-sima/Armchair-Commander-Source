#import "AppTrackingTransparency.h"
#import <AdSupport/AdSupport.h>

#ifdef __IPHONE_14_0
#import <AppTrackingTransparency/ATTrackingManager.h>
#import <StoreKit/SKAdNetwork.h>
#endif

char* convertNSStringToCString(const NSString* nsString)
{
    if (nsString == NULL)
        return NULL;

    const char* nsStringUtf8 = [nsString UTF8String];
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);

    return cString;
}

void requestTrackingAuthorization(AppTrackingTransparencyCallback callback)
{
    if (@available(iOS 14, *)) {
#ifdef __IPHONE_14_0
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            // 0 - Not determined
            // 1 - Restricted
            // 2 - Denied
            // 3 - Authorized
            callback((int) status);
        }];
#endif
    }
    else
    {
        bool trackingEnabled = [[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled];
        callback(trackingEnabled ? 3 : 2);
    }
}

char* identifierForAdvertising() {
    // Check whether advertising tracking is enabled
    bool isTrackingAuthorized = false;
    if (@available(iOS 14, *)) {
#ifdef __IPHONE_14_0
        isTrackingAuthorized = [ATTrackingManager trackingAuthorizationStatus] == ATTrackingManagerAuthorizationStatusAuthorized;
#endif
    } else {
        isTrackingAuthorized = [[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled];
    }
    
    // Get and return IDFA
    if(isTrackingAuthorized) {
        NSUUID *identifier = [[ASIdentifierManager sharedManager] advertisingIdentifier];
        return convertNSStringToCString([identifier UUIDString]);
    }
    
    return nil;
}

int trackingAuthorizationStatus()
{
    if (@available(iOS 14, *)) {
#ifdef __IPHONE_14_0
        return (int) [ATTrackingManager trackingAuthorizationStatus];
#endif
    }
    else
    {
        bool trackingEnabled = [[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled];
        return trackingEnabled ? 3 : 2;
    }
}

void registerAppForAdNetworkAttribution()
{
    if (@available(iOS 14, *)) {
#ifdef __IPHONE_14_0
        [SKAdNetwork registerAppForAdNetworkAttribution];
#endif
    }
}

void updateConversionValue(int value)
{
    if (@available(iOS 14, *)) {
#ifdef __IPHONE_14_0
        [SKAdNetwork updateConversionValue:value];
#endif
    }
}
