/**
 * ［概述］
 * 清单文件管理器
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
using System.Collections;
using MyABFramework.Global;
using UnityEngine;

namespace MyABFramework {
	/// <summary>清单文件管理器</summary>
	public class ManifestMgr : IManifestMgr {
		
		#region ［单例模式］

		/// <summary>本类实例</summary>
		private static ManifestMgr instance;
		
		/// <summary>得到本类实例</summary>
		public static ManifestMgr Instance => instance ?? (instance = new ManifestMgr());

		#endregion


		#region ［字段和属性］

		/// <summary>清单文件的路径</summary>
		private readonly string manifestPath;
		/// <summary>清单文件的系统类</summary>
		private AssetBundleManifest manifest;
		/// <summary>读取AB清单文件的AssetBundle</summary>
		private AssetBundle manifestReader;
		/// <summary>WWW资源是否加载完成，清单文件是否加载完成</summary>
		private bool isLoadFinished;

		/// <summary>属性：WWW资源是否加载完成，清单文件是否加载完成</summary>
		public bool IsLoadFinished {
			get {
				if(!isLoadFinished)
					Debug.LogWarning(FrameConst.PREFIX + "AssetBundleManifest加载未完成！");
				return isLoadFinished;
			}
		}

		#endregion


		#region ［构造器］

		/// <summary>构造函数</summary>
		private ManifestMgr() {
			//确定清单文件WWW下载路径
			manifestPath = PathTools.GetWWWPath() + "/" + PathTools.GetPlatformName();
			manifest = null;
			manifestReader = null;
			isLoadFinished = false;
		}

		#endregion


		#region ［公共方法］

		public IEnumerator LoadManifest() {
			using(WWW www = new WWW(manifestPath)) {
				yield return www;
				if(www.progress >= 1) {
					//加载完成，获取AB实例
					AssetBundle bundle = www.assetBundle;
					if(bundle == null)
						throw new ArgumentException(FrameConst.PREFIX + $"WWW资源下载出错！AssetBundle地址：{manifestPath}，错误信息：{www.error}",nameof(bundle));
					
					manifestReader = bundle;
					//读取清单文件（到系统类实例中）
					manifest = manifestReader.LoadAsset(FrameConst.NAME_Manifest) as AssetBundleManifest;
					//本次加载与读取清单文件完毕。
					isLoadFinished = true;
				}
			}
		}


		public AssetBundleManifest GetManifest() {
			if(!IsLoadFinished)
				return null;
			return manifest;
		}


		public string[] RetrieveDependencies(string bundleName) {
			if(string.IsNullOrWhiteSpace(bundleName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！",nameof(bundleName));

			if(!IsLoadFinished)
				return null;
			return manifest.GetAllDependencies(bundleName);
		}


		public void Dispose() {
			if(!IsLoadFinished)
				return;
			manifestReader.Unload(true);
		}

		#endregion
	}
}