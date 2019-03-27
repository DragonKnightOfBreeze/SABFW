/**
 * ［概述］
 * 路径工具类
 * 
 * ［用法］
 * 
 * 
 * ［备注］ 
 * 
 * 
 * 项目：自用AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 */
using System;
using MyABFramework.Global;
using UnityEngine;

namespace MyABFramework {
	/// <summary>路径工具类</summary>
	public static class PathTools {
		#region ［常量］

		/// <summary>路径：要打包的资源文件的根路径</summary>
		private const string PATH_Resources = "ABResources";

		#endregion


		#region ［方法］

		/// <summary>得到资源文件的完整输入路径</summary>
		/// <returns>完整输入路径</returns>
		public static string GetInputPath() {
			string path;
			switch(Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
					path = Application.dataPath + "/" + PATH_Resources;
					break;
				default:
					throw new PlatformNotSupportedException(FrameConst.PREFIX + "不支持这个平台！");
			}
			return path;
		}

		/// <summary>得到AssetBundle包的完整输出路径</summary>
		/// <remarks>备注：完整输出路径 = 平台目录 + 平台名称。</remarks>
		/// <returns>完整输出路径</returns>
		public static string GetOutputPath() {
			return GetPlatformPath() + "/" + GetPlatformName();
		}


		/// <summary>得到平台路径</summary>
		/// <returns>平台路径</returns>
		public static string GetPlatformPath() {
			string path;
			switch(Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					path = Application.streamingAssetsPath;
					break;
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
					path = Application.persistentDataPath;
					break;
				default:
					throw new PlatformNotSupportedException(FrameConst.PREFIX + "不支持这个平台！");
			}
			return path;
		}

		
		/// <summary>得到平台名称</summary>
		/// <returns>平台名称</returns>
		public static string GetPlatformName() {
			string name;
			switch(Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					name = "Windows";
					break;
				case RuntimePlatform.Android:
					name = "Android";
					break;
				case RuntimePlatform.IPhonePlayer:
					name = "Iphone";
					break;
				default:
					throw new PlatformNotSupportedException(FrameConst.PREFIX + "不支持这个平台！");
			}
			return name;
		}


		/// <summary>得到WWW协议下载路径</summary>
		/// <returns>WWW协议下载路径</returns>
		public static string GetWWWPath() {
			string path;
			switch(Application.platform) {
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					path = "file://" + GetOutputPath();
					break;
				case RuntimePlatform.Android:
					path = "jar:file://" + GetOutputPath();
					break;
				case RuntimePlatform.IPhonePlayer:
					path = GetOutputPath() + "/Raw/";
					break;
				default:
					throw new PlatformNotSupportedException(FrameConst.PREFIX + "不支持这个平台！");
			}
			return path;
		}

		#endregion
	}
}