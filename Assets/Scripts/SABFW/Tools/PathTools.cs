/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 路径工具类
 * 
 * ［功能］
 * 包含本框架中所有的路径常量
 * 
 * ［思路］
 *  路径工具类
 * 
 * ［用法］
 * 
 */
using System;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 路径工具类
	/// </summary>
	public static class PathTools {

		#region ［路径常量］

		/// <summary>路径：要打包的资源文件的根目录</summary>
		public const string PATH_ABResources = "ABResources";

		#endregion


		#region ［路径方法］

		/// <summary>
		/// 得到AB资源的完整输入目录
		/// </summary>
		/// <returns></returns>
		public static string GetABInputPath(){
			string path;

			//得到路径
			switch (Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
					path = Application.dataPath + "/" + PATH_ABResources;
					break;
				default:
					throw new PlatformNotSupportedException(FWDefine.PREFIX + "不支持这个平台！");
			}
			return path;
		}

		/// <summary>
		/// 得到AB资源的完整输出路径
		/// 完整输出路径=平台的路径+平台的名称
		/// </summary>
		/// <returns></returns>
		public static string GetABOutputPath(){
			return GetPlatformPath()+ "/" +GetPlatformName();
		}


		/// <summary>
		/// 得到平台名称
		/// </summary>
		/// <returns></returns>
		public static string GetPlatformPath(){
			string platformPath;
			switch (Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					platformPath = Application.streamingAssetsPath;
					break;
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
					platformPath = Application.persistentDataPath;
					break;
				default:
					throw new PlatformNotSupportedException(FWDefine.PREFIX + "不支持这个平台！");
			}
			return platformPath;
		}

		/// <summary>
		/// 得到平台的名称
		/// </summary>
		/// <returns></returns>
		public static string GetPlatformName(){
			string platformName;
			switch (Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					platformName = "Windows";
					break;
				case RuntimePlatform.Android:
					platformName = "Android";
					break;
				case RuntimePlatform.IPhonePlayer:
					platformName = "Iphone";
					break;
				default:
					throw new PlatformNotSupportedException(FWDefine.PREFIX+"不支持这个平台！");
			}
			return platformName;
		}


		/// <summary>
		/// 获取WWW协议下载路径
		/// </summary>
		/// <returns></returns>
		public static string GetWWWPath(){
			string path;

			switch (Application.platform) {
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					path = "file://" + GetABOutputPath();
					break;
				case RuntimePlatform.Android:
					path = "jar:file://" + GetABOutputPath();
					break;
				case RuntimePlatform.IPhonePlayer:
					path = GetABOutputPath() + "/Raw/";
					break;
				default:
					throw new PlatformNotSupportedException(FWDefine.PREFIX + "不支持这个平台！");
			}
			return path;
		}



		#endregion
	}
}