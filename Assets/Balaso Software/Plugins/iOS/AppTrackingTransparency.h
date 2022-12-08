#ifdef __cplusplus
extern "C" {
#endif

typedef void (*AppTrackingTransparencyCallback)(int result);
void requestTrackingAuthorization(AppTrackingTransparencyCallback callback);
char* identifierForAdvertising();
int trackingAuthorizationStatus();
void registerAppForAdNetworkAttribution();
void updateConversionValue(int value);
#ifdef __cplusplus
}
#endif
