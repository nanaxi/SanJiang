using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace anysdk {

	public enum ToolBarPlace
	{
		kToolBarTopLeft = 1,/**< enum the toolbar is at topleft. */
		kToolBarTopRight,/**< enum the toolbar is at topright. */
		kToolBarMidLeft,/**< enum the toolbar is at midleft. */
		kToolBarMidRight,/**< enum the toolbar is at midright. */
		kToolBarBottomLeft,/**< enum the toolbar is at bottomleft. */
		kToolBarBottomRight,/**< enum the toolbar is at bottomright. */
	} ;

	public enum UserActionResultCode
	{
		kInitSuccess = 0,/**< enum value is callback of succeeding in initing sdk. */
		kInitFail,/**< enum  value is callback of failing to init sdk. */
		kLoginSuccess,/**< enum value is callback of succeeding in login.*/
		kLoginNetworkError,/**< enum value is callback of network error*/
		kLoginNoNeed,/**< enum value is callback of no need login.*/
		kLoginFail,/**< enum value is callback of failing to login. */
		kLoginCancel,/**< enum value is callback of canceling to login. */
		kLogoutSuccess,/**< enum value is callback of succeeding in logout. */
		kLogoutFail,/**< enum value is callback of failing to logout. */
		kPlatformEnter,/**< enum value is callback after enter platform. */
		kPlatformBack,/**< enum value is callback after exit antiAddiction. */
		kPausePage,/**< enum value is callback after exit pause page. */
		kExitPage,/**< enum value is callback after exit exit page. */
		kAntiAddictionQuery,/**< enum value is callback after querying antiAddiction. */
		kRealNameRegister,/**< enum value is callback after registering realname. */
		kAccountSwitchSuccess,/**< enum alue is callback of succeeding in switching account. */
		kAccountSwitchFail,/**< enum value is callback of failing to switch account. */
		kOpenShop,/**< enum value is callback of open the shop. */
		kAccountSwitchCancel,/**< enum value is callback of canceling to switch account. */
		kGameExitPage,/**< enum value is callback of no channel exit page. */
		kUserExtension = 50000 /**< enum value is  extension code . */
		
		
	} ;

	public class AnySDKUser
	{
		private static AnySDKUser _instance;
		
		public static AnySDKUser getInstance() {
			if( null == _instance ) {
				_instance = new AnySDKUser();
			}
			return _instance;
		}
		/**
    	 @brief User login
    	 */

		public  void login()
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			AnySDKUser_nativeLogin ();
#else
			Debug.Log("This platform does not support!");
#endif
		}
		/**
     	@brief User login
     	 	if the process of logining need to know  the param of server_id ,
     	 	you can use the function
     	 	and if you must change oauthloginserver, you can add the param of oauthLoginServer
    	 @param server_id
    	 @param oauthLoginServer
    	*/
		[Obsolete("Please use Resources.login(Dictionary<string,string> info) instead")]
		public  void login(string serverID,string authLoginServer = "")
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			AnySDKUser_nativeLoginWithParam (serverID,authLoginServer);
#else
			Debug.Log("This platform does not support!");
#endif
		}
		/**
      	@brief User login
         	 	if the process of logining need to know  the parameters ,
         	 	you can use the function
      	@param the parameters
        */
		public  void login(Dictionary<string,string> info)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS) 
			string sInfo = AnySDKUtil.dictionaryToString (info);
			Debug.Log("login   " + sInfo);
			AnySDKUser_nativeLoginWithMap (sInfo);
#else
			Debug.Log("This platform does not support!");
#endif
		}
		/**
   	 	 @brief Get user ID
    	 @return If user logined, return value is userID;
             else return value is empty string.
    	 */

		public  string getUserID()
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			StringBuilder userID = new StringBuilder();
			userID.Capacity = AnySDKUtil.MAX_CAPACITY_NUM;
			AnySDKUser_nativeGetUserID (userID);
			return userID.ToString();
#else
			Debug.Log("This platform does not support!");
			return "";
#endif
		}

		/**
   	 	 @brief Check whether the user logined or not
    	 @return If user logined, return value is true;
             else return value is false.
    	 */

		public  bool isLogined()
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			return AnySDKUser_nativeIsLogined ();
#else
			Debug.Log("This platform does not support!");
			return false;
#endif
		}
		/**
     	@brief Check function the plugin support or not
    	 @param the name of plugin
    	 @return if the function support ,return true
    	 	 	 else retur false
    	 */

		public  bool isFunctionSupported (string functionName)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			return AnySDKUser_nativeIsFunctionSupported (functionName);
#else
			Debug.Log("This platform does not support!");
			return false;
#endif
		}
		/**
		 * set debugmode for plugin
		 * 
		 */
		[Obsolete("This interface is obsolete!",false)]
		public  void setDebugMode(bool bDebug)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			AnySDKUser_nativeSetDebugMode (bDebug);
#else
			Debug.Log("This platform does not support!");
#endif
		}

		/**
    	 @brief set pListener The callback object for user result
    	 @param the MonoBehaviour object
    	 @param the callback of function
    	 */

		public  void setListener(MonoBehaviour gameObject,string functionName)
		{
#if !UNITY_EDITOR && UNITY_ANDROID 
			AnySDKUtil.registerActionCallback (AnySDKType.User, gameObject, functionName);
#elif UNITY_IOS
			string gameObjectName = gameObject.gameObject.name;
			AnySDKUser_nativeSetListener(gameObjectName,functionName);
#else
			Debug.Log("This platform does not support!");
#endif
		}
		/**
     	@brief get plugin id
   		 @return the plugin id
    	 */

		public string getPluginId()
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			StringBuilder pluginlId = new StringBuilder();
			pluginlId.Capacity = AnySDKUtil.MAX_CAPACITY_NUM;
			AnySDKUser_nativeGetPluginId (pluginlId);
			return pluginlId.ToString();
#else
			Debug.Log("This platform does not support!");
			return "";
#endif
		}
		/**
		 * Get Plugin version
		 * 
		 * @return string
	 	*/

		public  string getPluginVersion()
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			StringBuilder version = new StringBuilder();
			version.Capacity = AnySDKUtil.MAX_CAPACITY_NUM;
			AnySDKUser_nativeGetPluginVersion (version);
			return version.ToString();
#else
			Debug.Log("This platform does not support!");
			return "";
#endif
		}

		/**
		 * Get SDK version
		 * 
		 * @return string
	 	*/

		public  string getSDKVersion()
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			StringBuilder version = new StringBuilder();
			version.Capacity = AnySDKUtil.MAX_CAPACITY_NUM;
			AnySDKUser_nativeGetSDKVersion (version);
			return version.ToString();
#else
			Debug.Log("This platform does not support!");
			return "";
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param AnySDKParam param 
    	 *@return void
    	 */
		public  void callFuncWithParam(string functionName, AnySDKParam param)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			List<AnySDKParam> list = new List<AnySDKParam> ();
			list.Add (param);
			AnySDKUser_nativeCallFuncWithParam(functionName, list.ToArray(),list.Count);
#else
			Debug.Log("This platform does not support!");
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param List<AnySDKParam> param 
    	 *@return void
    	 */
		public  void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			if (param == null) 
			{
				AnySDKUser_nativeCallFuncWithParam (functionName, null, 0);

			} else {
				AnySDKUser_nativeCallFuncWithParam (functionName, param.ToArray (), param.Count);
			}
#else
			Debug.Log("This platform does not support!");
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param AnySDKParam param 
    	 *@return int
    	 */
		public  int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			List<AnySDKParam> list = new List<AnySDKParam> ();
			list.Add (param);
			return AnySDKUser_nativeCallIntFuncWithParam(functionName, list.ToArray(),list.Count);
#else
			Debug.Log("This platform does not support!");
			return -1;
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param List<AnySDKParam> param 
    	 *@return int
    	 */
		public  int  callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			if (param == null)
			{
				return AnySDKUser_nativeCallIntFuncWithParam (functionName, null, 0);

			} else {
				return AnySDKUser_nativeCallIntFuncWithParam (functionName, param.ToArray (), param.Count);
			}
#else
			Debug.Log("This platform does not support!");
			return -1;
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param AnySDKParam param 
    	 *@return float
    	 */
		public  float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			List<AnySDKParam> list = new List<AnySDKParam> ();
			list.Add (param);
			return AnySDKUser_nativeCallFloatFuncWithParam(functionName, list.ToArray(),list.Count);
#else
			Debug.Log("This platform does not support!");
			return 0;
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param List<AnySDKParam> param 
    	 *@return float
    	 */
		public  float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			if (param == null)
			{
				return AnySDKUser_nativeCallFloatFuncWithParam (functionName, null, 0);
				
			} else {
				return AnySDKUser_nativeCallFloatFuncWithParam (functionName, param.ToArray (), param.Count);
			}
#else
			Debug.Log("This platform does not support!");
			return 0;
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param AnySDKParam param 
    	 *@return bool
    	 */
		public  bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			List<AnySDKParam> list = new List<AnySDKParam> ();
			list.Add (param);
			return AnySDKUser_nativeCallBoolFuncWithParam(functionName, list.ToArray(),list.Count);
#else
			Debug.Log("This platform does not support!");
			return false;
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param List<AnySDKParam> param 
    	 *@return bool
    	 */
		public  bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			if (param == null)
			{
				return AnySDKUser_nativeCallBoolFuncWithParam (functionName, null, 0);
				
			} else {
				return AnySDKUser_nativeCallBoolFuncWithParam (functionName, param.ToArray (), param.Count);
			}
#else
			Debug.Log("This platform does not support!");
			return false;
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param AnySDKParam param 
    	 *@return string
    	 */
		public  string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			List<AnySDKParam> list = new List<AnySDKParam> ();
			list.Add (param);
			StringBuilder value = new StringBuilder();
			value.Capacity = AnySDKUtil.MAX_CAPACITY_NUM;
			AnySDKUser_nativeCallStringFuncWithParam(functionName, list.ToArray(),list.Count,value);
			return value.ToString ();
#else
			Debug.Log("This platform does not support!");
			return "";
#endif
		}

		/**
    	 *@brief methods for reflections
   	 	 *@param function name
   		 *@param List<AnySDKParam> param 
    	 *@return string
    	 */
		public  string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
#if !UNITY_EDITOR &&( UNITY_ANDROID ||  UNITY_IOS)  
			StringBuilder value = new StringBuilder();
			value.Capacity = AnySDKUtil.MAX_CAPACITY_NUM;
			if (param == null)
			{
				AnySDKUser_nativeCallStringFuncWithParam (functionName, null, 0,value);
				
			} else {
				AnySDKUser_nativeCallStringFuncWithParam (functionName, param.ToArray (), param.Count,value);
			}
			return value.ToString ();
#else
			Debug.Log("This platform does not support!");
			return "";
#endif
		}

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM,CallingConvention=CallingConvention.Cdecl)]
		private static extern void AnySDKUser_RegisterExternalCallDelegate(IntPtr functionPointer);
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeLogin();

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeSetListener(string gameName, string functionName);
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeLoginWithParam(string serverID, string authLoginServer);

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeLoginWithMap(string info);
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeGetUserID(StringBuilder userID);
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern bool AnySDKUser_nativeIsLogined();
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern bool AnySDKUser_nativeIsFunctionSupported(string functionName);
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeSetDebugMode(bool bDebug);
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeGetPluginId(StringBuilder pluginID);

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeGetPluginVersion(StringBuilder version);
		
		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeGetSDKVersion(StringBuilder version);

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeCallFuncWithParam(string functionName, AnySDKParam[] param,int count);

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern int AnySDKUser_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param,int count);

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern float AnySDKUser_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param,int count);

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern bool AnySDKUser_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param,int count);

		[DllImport(AnySDKUtil.ANYSDK_PLATFORM)]
		private static extern void AnySDKUser_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param,int count,StringBuilder value);
	}

}


