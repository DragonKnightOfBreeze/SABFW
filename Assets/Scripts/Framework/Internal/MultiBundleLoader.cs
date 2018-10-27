/**
 * ［概述］
 * 第三层：同场景中的多包加载
 *
 * 1.获取AB包之间的依赖于引用关系
 * 2.管理AssetBundle之间的自动连锁（递归）加载机制。
 * 
 * ［用法］
 *
 * 
 * ［备注］ 
 * 依赖关系和引用关系是一种相反的关系
 * 在加载某项之前，首先加载该项的依赖项，建立依赖关系
 * 该项的依赖项又将该项当做它的引用项，建立引用关系
 * 
 * 项目：自用AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 */ 

using System;
using System.Collections;
using System.Collections.Generic;
using MyABFramework.Global;
using UnityEngine;

namespace MyABFramework {
	/// <summary>第三层：同场景中的多包加载</summary>
	public class MultiBundleLoader {
		#region ［字段和属性］

		/// <summary>当前引用的单包加载类</summary>
		private SingleBundleLoader curSingleBundleLoader;
		///<summary>包名与单包加载类的缓存集合</summary>
		private Dictionary<string, SingleBundleLoader> singleBundleLoaderDict;
		
		///<summary>包名与对应依赖关系的集合</summary>
		private Dictionary<string, BundleRelation> bundleRelationDict;

		/// <summary>委托：在每个包加载完成时调用</summary>
		private LoadCompleteDelegate loadCompleteHandler;
		///<summary>委托，在所有包加载完成时调用</summary>
		private readonly LoadAllCompleteDelegate loadAllCompleteHandler;

		
		
		#endregion


		#region ［构造器］

		/// <summary>构造函数</summary>
		/// <param name="handler">加载完成时调用的委托</param>
		/// <param name="allHandler">全部加载完成时调用的委托</param>
		internal MultiBundleLoader(LoadCompleteDelegate handler,LoadAllCompleteDelegate allHandler) {
//			if(string.IsNullOrEmpty(sceneName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(sceneName));
//			if(string.IsNullOrEmpty(bundleName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(bundleName));

			singleBundleLoaderDict = new Dictionary<string, SingleBundleLoader>();
			bundleRelationDict = new Dictionary<string, BundleRelation>();
			loadCompleteHandler = handler;
			loadAllCompleteHandler = allHandler;
		}

		#endregion


		#region ［公共方法］

		/// <summary>加载指定的包</summary>
		/// <param name="bundleName">指定的包名</param>
		internal IEnumerator LoadAssetBundle(string bundleName) {
			//递归加载
			yield return LoadDependence(bundleName);
			//调用对应的委托
			loadAllCompleteHandler?.Invoke();
		}


		/// <summary>加载指定包中的指定资源</summary>
		/// <param name="bundleName">指定的包名</param>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns>指定的资源</returns>
		internal UnityEngine.Object LoadAsset(string bundleName, string assetName, bool isCache = false) {
//			if(string.IsNullOrEmpty(bundleName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(bundleName));
//			if(string.IsNullOrEmpty(assetName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(assetName));

			return GetSingleBundleLoader(bundleName).LoadAsset(assetName, isCache);
		}


		/// <summary>卸载指定包中的指定资源</summary>
		/// <param name="bundleName">指定的包名</param>
		/// <param name="asset">指定的资源</param>
		internal void UnloadAsset(string bundleName, UnityEngine.Object asset) {
//			if(string.IsNullOrEmpty(abName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(abName));
//			if(asset == null)
//				throw new ArgumentException(FrameConst.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			GetSingleBundleLoader(bundleName).UnloadAsset(asset);
		}


		/// <summary>释放指定包中的内存镜像资源</summary>
		internal void Dispose(string bundleName) {
//			if(string.IsNullOrEmpty(bundleName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(bundleName));

			GetSingleBundleLoader(bundleName).Dispose();
		}


		/// <summary>释放指定包中的内存镜像资源，且释放内存资源</summary>
		internal void DisposeAll(string bundleName) {
//			if(string.IsNullOrEmpty(bundleName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(bundleName));

			GetSingleBundleLoader(bundleName).DisposeAll();
		}


		/// <summary>查询指定包中的所有资源的名称</summary>
		internal string[] RetrieveAll(string bundleName) {
//			if(string.IsNullOrEmpty(abName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(abName));

			return GetSingleBundleLoader(bundleName).RetrieveAll();
		}


		/// <summary>释放本场景中的所有资源</summary>
		/// <remarks>备注：一般在场景切换时使用。</remarks>
		internal void DisposeAllInScene() {
			try {
				//逐一释放所有加载过的AB包中的资源
				foreach(var item in singleBundleLoaderDict.Values)
					item.DisposeAll();
			}
			finally { 
				//使用反射技术，释放本类所有资源
				foreach(var field in GetType().GetFields())
					field.SetValue(this,null);
				
				//释放本对象中你的所有资源
//				curSingleBundleLoader = null;
//				singleBundleLoaderDict.Clear();
//				singleBundleLoaderDict = null;
//				bundleRelationDict.Clear();
//				bundleRelationDict = null;
//				curBundleName = null;
//				curSceneName = null;
//				loadAllCompleteHandler = null;
				
				//卸载没有使用到的资源
				Resources.UnloadUnusedAssets();
				//强制垃圾收集
				GC.Collect();
			}
		}

		#endregion


		#region ［私有方法］

//		/// <summary>完成指定包的调用</summary>
//		/// <param name="bundleName">指定的包名</param>
//		private void LoadComplete(string bundleName) {
////			if(string.IsNullOrEmpty(bundleName))
////				throw new ArgumentException(FrameConst.PREFIX + nameof(bundleName));
//
//			if(bundleName.Equals(curBundleName))
//				loadAllCompleteHandler?.Invoke();
//		}


		/// <summary>加载依赖的AB包</summary>
		/// <param name="bundleName">指定的包名</param>
		private IEnumerator LoadDependence(string bundleName) {
			//AssetBundle包关系的建立
			if(!bundleRelationDict.ContainsKey(bundleName))
				bundleRelationDict.Add(bundleName, new BundleRelation(bundleName));
			BundleRelation tempBundleRelation = bundleRelationDict[bundleName];

			//得到指定AssetBundle包所有的依赖关系（查询相应的清单文件）
			string[] dependenceArray = ManifestMgr.Instance.RetrieveDependencies(bundleName);
			foreach(var dependenceItem in dependenceArray) {
				//添加依赖项
				tempBundleRelation.AddABDependence(dependenceItem);
				//添加引用项（通过协程方法，递归调用）
				yield return LoadReference(dependenceItem, bundleName);
			}
			//真正加载AssetBundle包，并等待加载完成
			if(singleBundleLoaderDict.ContainsKey(bundleName)) {
				yield return singleBundleLoaderDict[bundleName].LoadAssetBundle();
			}
			else {
				//当前的SingleBundleLoader变成了下一个
				curSingleBundleLoader = new SingleBundleLoader(bundleName,loadCompleteHandler);
				singleBundleLoaderDict.Add(bundleName, curSingleBundleLoader);
				yield return curSingleBundleLoader.LoadAssetBundle();
			}
		}
		
		
		/// <summary>加载引用的AB包</summary>
		/// <param name="bundleName">指定的包名</param>
		/// <param name="refBundleName">被引用的包名</param>
		private IEnumerator LoadReference(string bundleName, string refBundleName) {
			//如果包已经加载
			if(bundleRelationDict.ContainsKey(bundleName)) {
				BundleRelation tempBundleRelation = bundleRelationDict[bundleName];
				//添加AB包引用关系（被依赖）
				tempBundleRelation.AddReference(refBundleName);
			}
			//如果包未被加载
			else {
				BundleRelation tempBundleRelation = new BundleRelation(bundleName);
				tempBundleRelation.AddReference(refBundleName);
				bundleRelationDict.Add(bundleName, tempBundleRelation);
			}
			//递归调用：开始加载所依赖的包
			yield return LoadDependence(bundleName);
		}


		/// <summary>得到对应的SingleABLoader</summary>
		/// <param name="bundleName">指定的包名</param>
		/// <returns>对应的SingleABLoader</returns>
		private SingleBundleLoader GetSingleBundleLoader(string bundleName) {
			SingleBundleLoader singleBundleLoader;
			singleBundleLoaderDict.TryGetValue(bundleName, out singleBundleLoader);
			if(singleBundleLoader == null)
				throw new ArgumentException(FrameConst.PREFIX + $"找不到对应的AssetBundle包！指定的包名：{bundleName}",nameof(singleBundleLoader));
	
			return singleBundleLoader;
		}

		#endregion
	}
}