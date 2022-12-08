using UnityEngine;
using System;

namespace Balaso
{
    /// <summary>
    /// Use this class to show the iOS 14 App Tracking Transparency native
    /// popup requesting user's authorization to obtain Identifier for Advertising  (IDFA)
    /// </summary>
    ///
    /// <example>
    /// <code>
    ///     #if UNITY_IOS
    ///         AppTrackingTransparency.OnAuthorizationRequestDone += OnAuthorizationRequestDone;
    ///         AppTrackingTransparency.RequestTrackingAuthorization();
    ///     #endif
    /// </code>
    /// </example>
    public static class AppTrackingTransparency
    {   
#if UNITY_IOS
        private delegate void AppTrackingTransparencyCallback(int result);
        [AOT.MonoPInvokeCallback(typeof(AppTrackingTransparencyCallback))]
        private static void appTrackingTransparencyCallbackReceived(int result)
        {
            Debug.Log(string.Format("UnityAppTrackingTransparencyCallback received: {0}", result));

            // Force to use Default Synchronization context to run callback on Main Thread
            System.Threading.Tasks.Task.Delay(1)
            .ContinueWith((unused) =>
            {
                if (OnAuthorizationRequestDone != null)
                {
                    switch (result)
                    {
                        case 0:
                            OnAuthorizationRequestDone(AuthorizationStatus.NOT_DETERMINED);
                            break;
                        case 1:
                            OnAuthorizationRequestDone(AuthorizationStatus.RESTRICTED);
                            break;
                        case 2:
                            OnAuthorizationRequestDone(AuthorizationStatus.DENIED);
                            break;
                        case 3:
                            OnAuthorizationRequestDone(AuthorizationStatus.AUTHORIZED);
                            break;
                        default:
                            OnAuthorizationRequestDone(AuthorizationStatus.NOT_DETERMINED);
                            break;
                    }
                }
            }, currentSynchronizationContext);   
        }

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void requestTrackingAuthorization(AppTrackingTransparencyCallback callback);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern string identifierForAdvertising();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern int trackingAuthorizationStatus();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void registerAppForAdNetworkAttribution();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void updateConversionValue(int value);

#endif

        private static System.Threading.Tasks.TaskScheduler currentSynchronizationContext;

        static AppTrackingTransparency()
        {
            currentSynchronizationContext = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
        }

        /// <summary>
        /// Possible App Tracking Transparency authorization status 
        /// </summary>
        public enum AuthorizationStatus
        {
            NOT_DETERMINED,
            /// <summary>
            /// User restrited app tracking. IDFA not available
            /// </summary>
            RESTRICTED,
            /// <summary>
            /// User did not grant access to IDFA
            /// </summary>
            DENIED,
            /// <summary>
            /// You can safely request IDFA
            /// </summary>
            AUTHORIZED
        };

        /// <summary>
        /// Callback invoked once user made a decision through iOS App Tracking Transparency native popup
        /// </summary>
        public static Action<AuthorizationStatus> OnAuthorizationRequestDone;

        /// <summary>
        /// Obtain current Tracking Authorization Status
        /// </summary>
        public static AuthorizationStatus TrackingAuthorizationStatus
        {
            get
            {
#if UNITY_EDITOR
                Debug.Log("Running on Editor platform. Callback invoked with debug result");
                return AuthorizationStatus.AUTHORIZED;
#elif UNITY_IOS
                return (AuthorizationStatus) trackingAuthorizationStatus();
#else
                return AuthorizationStatus.NOT_DETERMINED; 
#endif
            }
        }

        /// <summary>
        /// Updates the conversion value and verifies the first launch of an app installed as a result of an ad.
        /// See https://developer.apple.com/documentation/storekit/skadnetwork/3566697-updateconversionvalue
        /// </summary>
        /// <param name="value"></param>
        public static void UpdateConversionValue(int value)
        {
#if UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log(string.Format("Updating conversion value to {0}", value));
                updateConversionValue(value);
            }
            else
            {
                Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
            }
#endif
        }

        /// <summary>
        /// Verifies the first launch of an app installed as a result of an ad.
        /// See https://developer.apple.com/documentation/storekit/skadnetwork/2943654-registerappforadnetworkattributi
        /// </summary>
        public static void RegisterAppForAdNetworkAttribution()
        {
#if UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Registering App for Ad Network Attribution");
                registerAppForAdNetworkAttribution();
            }
            else
            {
                Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
            }
#endif
        }

        /// <summary>
        /// Requests iOS Tracking Authorization. <br/>
        /// 
        /// <see cref="OnAuthorizationRequestDone" /> will
        /// be invoked with the authorization result for every iOS version.
        ///
        /// <list type="bullet">
        ///     <item>
        ///         <b>iOS 13 and lower</b>: Obtains if Advertising Tracking is enabled or not
        ///     </item>
        ///     <item>
        ///         <b>iOS 14+</b>: The native iOS Tracking Authorization popup will be shown to the user
        ///     </item>
        /// </list>
        /// </summary>
        ///
        /// <example>
        /// <code>
        ///     #if UNITY_IOS
        ///         AppTrackingTransparency.OnAuthorizationRequestDone += OnAuthorizationRequestDone;
        ///         AppTrackingTransparency.RequestTrackingAuthorization();
        ///     #endif
        /// </code>
        /// </example>
        public static void RequestTrackingAuthorization()
        {
#if UNITY_EDITOR
            Debug.Log("Running on Editor platform. Callback invoked with debug result");
            OnAuthorizationRequestDone?.Invoke(AuthorizationStatus.AUTHORIZED);
#elif UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Requesting authorization to iOS...");
                requestTrackingAuthorization(appTrackingTransparencyCallbackReceived);
            }
            else
            {
                Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
            }
#else
            Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
#endif
        }

        /// <summary>
        /// Obtains iOS Identifier for Advertising (IDFA) if authorized.
        /// </summary>
        /// <returns>The IDFA value if authorized, null otherwise</returns>
        public static string IdentifierForAdvertising()
        {
#if UNITY_EDITOR
            Debug.Log("Running on Editor platform. Callback invoked with debug result");
            return "unity-editor-test-idfa";
#elif UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Obtaining IDFA from iOS...");
                string idfa = identifierForAdvertising();
                return string.IsNullOrEmpty(idfa) ? null : idfa;
            }
            else
            {
                Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
                return null;
            }
#else
            Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
            return null;
#endif
        }
    }
}