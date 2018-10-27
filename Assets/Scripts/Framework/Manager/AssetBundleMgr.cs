/**
 * ［概述］
 * 最上层：所有场景中的AssetBundle包的管理器
 *
 * 在Awake方法中开始LoadAssetBundle协程，编写作为参数的委托对应的方法，可在其中实例化和克隆AB包中资源。
 * 可根据特定事件，触发调用资源加载、卸载，AB包的资源释放、资源查询等方法。
 * 
 * ［用法］
 * 1.整个框架的入口；
 * 2.读取清单文件，缓存到本脚本中；
 * 3.以场景为单位，管理整个项目中的所有AB包；
 * 4.可以方便地进行AB包中资源的加载、卸载，AB包的资源释放、资源查询。
 * 
 * ［备注］
 * 动态挂载。
 * 不检查场景名称的正确性，请使用场景类型枚举转化。
 * 
 * 项目：自用AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 */ 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyABFramework.Global;

namespace MyABFramework {
	/// <summary>最上层：所有场景中的AssetBundle包的管理器</summary>
	public class AssetBundleMgr : MonoBehaviour, IAssetBundleMgr {
		
		#region ［单例模式］

		///<summary>本类实例</summary>
		private static AssetBundleMgr instance;
		
		/// <summary>得到本类实例</summary>
		public static AssetBundleMgr Instance {
			get {
				if(instance == null)
					instance = new GameObject("_AssetBundleMgr").AddComponent<AssetBundleMgr>();
				return instance;
			}
		}
		
		#endregion


		#region ［字段和属性］
		
		///<summary>清单文件的系统类</summary>
		[HideInInspector]
		public AssetBundleManifest Manifest;
		
		/// <summary>场景名称与多包加载类的缓存集合</summary>
		private Dictionary<string, MultiBundleLoader> multiBundleLoaderDict;
		
		#endregion


		#region ［Unity消息］

		private void Awake() {
			DontDestroyOnLoad(gameObject);
			
			multiBundleLoaderDict = new Dictionary<string, MultiBundleLoader>();
			
			//开始加载Manifest清单文件
			StartCoroutine(ManifestMgr.Instance.LoadManifest());
		}

		#endregion


		#region ［公共方法］

		public IEnumerator LoadAssetBundle(string sceneName, string bundleName,LoadCompleteDelegate handler = null, LoadAllCompleteDelegate allHandler = null) {
			if(string.IsNullOrWhiteSpace(sceneName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(sceneName));
			if(string.IsNullOrWhiteSpace(bundleName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(bundleName));

			//等待清单文件加载完成
			while(!ManifestMgr.Instance.IsLoadFinished)
				yield return null;
			Manifest = ManifestMgr.Instance.GetManifest();

			//把当前场景加入集合中
			if(!multiBundleLoaderDict.ContainsKey(sceneName)) {
				MultiBundleLoader multiBundleLoader = new MultiBundleLoader(handler,allHandler);
				multiBundleLoaderDict.Add(sceneName, multiBundleLoader);
			}
			//加载指定的AB包
			yield return multiBundleLoaderDict[sceneName].LoadAssetBundle(bundleName);
		}

		
		public UnityEngine.Object LoadAsset(string sceneName, string bundleName, string assetName, bool isCache = false) {
			if(string.IsNullOrWhiteSpace(sceneName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(sceneName));
			if(string.IsNullOrWhiteSpace(bundleName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(bundleName));
			if(string.IsNullOrWhiteSpace(assetName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(assetName));
			
			return GetMultiABLoader(sceneName).LoadAsset(bundleName, assetName, isCache);
		}


		public void UnloadAsset(string sceneName, string bundleName, UnityEngine.Object asset) {
			if(string.IsNullOrWhiteSpace(sceneName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(sceneName));
			if(string.IsNullOrWhiteSpace(bundleName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(bundleName));
			if(asset == null)
				throw new ArgumentException(FrameConst.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			GetMultiABLoader(sceneName).UnloadAsset(bundleName, asset);
		}


		public void Dispose(string sceneName, string bundleName) {
			if(string.IsNullOrWhiteSpace(sceneName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(sceneName));
			if(string.IsNullOrWhiteSpace(bundleName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(bundleName));
			
			GetMultiABLoader(sceneName).Dispose(bundleName);
		}


		public void DisposeAll(string sceneName, string bundleName) {
			if(string.IsNullOrWhiteSpace(sceneName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(sceneName));
			
			GetMultiABLoader(sceneName).DisposeAll(bundleName);
		}


		public void DisposeAllInScene(string sceneName) {
			if(string.IsNullOrWhiteSpace(sceneName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(sceneName));
			
			GetMultiABLoader(sceneName).DisposeAllInScene();
		}
		
		public string[] RetrieveAll(string sceneName, string bundleName) {
			if(string.IsNullOrWhiteSpace(sceneName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(sceneName));
			if(string.IsNullOrWhiteSpace(bundleName))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(bundleName));
			
			return GetMultiABLoader(sceneName).RetrieveAll(bundleName);
		}

		#endregion


		#region ［私有方法］

		/// <summary>得到对应的MultiABLoader</summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <returns>对应的MultiABLoader</returns>
		private MultiBundleLoader GetMultiABLoader(string sceneName) {
			MultiBundleLoader multiBundleLoader;
			multiBundleLoaderDict.TryGetValue(sceneName, out multiBundleLoader);
			if(multiBundleLoader == null)
				throw new ArgumentException(FrameConst.PREFIX + $"找不到对应的同一场景下的AssetBundle包！指定的场景名称：{sceneName}", nameof(multiBundleLoader));

			return multiBundleLoader;
		}

		#endregion
	}
}