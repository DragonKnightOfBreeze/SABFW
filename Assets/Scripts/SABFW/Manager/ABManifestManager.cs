/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * AssetBundle清单文件读取类
 * 
 */
using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 读取清单文件
	/// </summary>
	public class ABManifestManager : IABManifestManager {

		#region ［单例模式］

		/// <summary>本类实例</summary>
		private static ABManifestManager _Instance;
          
		/// <summary>得到本类实例</summary>
		public static ABManifestManager Instance => _Instance ?? (_Instance = new ABManifestManager());

		#endregion


		#region ［字段和属性］

		/// <summary>清单文件的路径</summary>
		private readonly string _ABManifestPath;
		/// <summary>清单文件的系统类</summary>
		private AssetBundleManifest _ABManifest;
		/// <summary>读取AB清单文件的AssetBundle</summary>
		private AssetBundle _ABManifestReader;
		/// <summary>WWW资源是否加载完成，清单文件是否加载完成</summary>
		private bool _IsLoadFinished;

		/// <summary>属性：WWW资源是否加载完成，清单文件是否加载完成</summary>
		public bool IsLoadFinished {
			get {
				if(!_IsLoadFinished)
					Debug.LogWarning(FWDefine.PREFIX + "AssetBundleManifest加载未完成！");
				return _IsLoadFinished;
			}
		}

		#endregion


		#region ［构造器］

		/// <summary>
		/// 构造函数
		/// </summary>
		private ABManifestManager() {
			//确定清单文件WWW下载路径
			_ABManifestPath = PathTools.GetWWWPath() + "/" + PathTools.GetPlatformName();
			_ABManifest = null;
			_ABManifestReader = null;
			_IsLoadFinished = false;
		}

		#endregion


		#region ［公共方法］

		/// <summary>
		/// 加载Manifest清单文件
		/// </summary>
		/// <returns></returns>
		public IEnumerator LoadABMainfest() {
			using (WWW www = new WWW(_ABManifestPath)) {
				yield return www;
				if (www.progress >= 1) {
					//加载完成，获取AB实例
					AssetBundle abObj = www.assetBundle;
					if (abObj == null)
						throw new NullReferenceException(FWDefine.PREFIX + "WWW资源下载出错！" + "\t" + "AssetBundle地址：" + _ABManifestPath + "\t" + "错误信息：" + www.error);

					_ABManifestReader = abObj;
					//读取清单文件（到系统类实例中）
					_ABManifest = _ABManifestReader.LoadAsset(FWDefine.NAME_ABManifest) as AssetBundleManifest;

					//本次加载与读取清单文件完毕。
					_IsLoadFinished = true;
				}
			}
		}


		/// <summary>
		/// 获取AssetBundleManifest系统类实例
		/// </summary>
		/// <returns></returns>
		public AssetBundleManifest GetABManifest() {
			if (!IsLoadFinished)
				return null;
			return _ABManifest;
		}


		/// <summary>
		/// 获取AssetBundleManifest（系统类）中所有的依赖项
		/// </summary>
		public string[] RetrieveDependencies(string abName) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX+nameof(abName));

			if (!IsLoadFinished)
				return null;
			return _ABManifest.GetAllDependencies(abName);
		}


		/// <summary>
		/// 释放本类的资源
		/// 一定要在最后调用
		/// </summary>
		public void Dispose() {
			if (!IsLoadFinished)
				return;
			_ABManifestReader.Unload(true);
		}

		#endregion
	}
}