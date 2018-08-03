/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 主流程，第三层：
 * 一个场景中的多个AB包的管理
 * 
 * ［功能］
 * 1.获取AB包之间的依赖于引用关系
 * 2.管理AssetBundle之间的自动连锁（递归）加载机制。
 *
 * ［思路］
 * 依赖关系和引用关系是一种相反的关系
 * 在加载某项之前，首先加载该项的依赖项，建立依赖关系
 * 该项的依赖项又将该项当做它的引用项，建立引用关系
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 一个场景中的多个AB包的管理
	/// </summary>
	public class MultiABLoader {

		#region ［字段和属性］

		///<summary>（下层）引用类：单个AB包加载实现类</summary>s
		private SingleABLoader _CurSingleABLoader;
		///<summary>A单个B包加载类的缓存集合</summary>
		private Dictionary<string, SingleABLoader> _SingleABLoaderCacheDict;
		///<summary>AB包名称与对应依赖关系的集合</summary>
		private Dictionary<string, ABRelation> _ABRelationDict;
		///<summary>当前场景名称（调试使用）</summary>
		private string _CurSceneName;
		///<summary>当前AB包名称</summary>
		private string _CurABName;
		///<summary>委托：所有的AB包是否加载完成</summary>
		private DelLoadComplete _LoadAllCompleteHandler;

		#endregion


		#region ［构造器］

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="sceneName">场景名称</param>
		/// <param name="abName">AB包名称</param>
		/// <param name="handler">加载完成时调用的委托</param>
		public MultiABLoader(string sceneName, string abName, DelLoadComplete handler) {
			if (string.IsNullOrEmpty(sceneName))
				throw new ArgumentException(FWDefine.PREFIX+ nameof(sceneName));
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));

			_SingleABLoaderCacheDict = new Dictionary<string, SingleABLoader>();
			_ABRelationDict = new Dictionary<string, ABRelation>();
			_CurSceneName = sceneName;
			_CurABName = abName;
			_LoadAllCompleteHandler = handler;
		}

		#endregion


		#region ［公共方法］

		/// <summary>
		/// 加载指定的AB包
		/// </summary>
		/// <param name="abName">AB包名称</param>
		/// <returns></returns>
		public IEnumerator LoadAssetBundle(string abName) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));

			//AB包关系的建立
			if (!_ABRelationDict.ContainsKey(abName)) {
				_ABRelationDict.Add(abName, new ABRelation(abName));
			}
			ABRelation tempABRelation = _ABRelationDict[abName];

			//得到指定AB包所有的依赖关系（查询相应的清单文件）
			string[] dependenceArray = ABManifestManager.Instance.RetrieveDependencies(abName);
			foreach (var dependenceItem in dependenceArray) {
				//添加依赖项
				tempABRelation.AddABDependence(dependenceItem);
				//添加引用项（通过协程方法，递归调用）
				yield return LoadReference(dependenceItem, abName);
			}

			//真正加载AB包，并等待加载完成
			if (_SingleABLoaderCacheDict.ContainsKey(abName)) {
				yield return _SingleABLoaderCacheDict[abName].LoadAssetBundle();
			}
			else {
				//当前的SingleABLoader变成了下一个
				_CurSingleABLoader = new SingleABLoader(abName, CompleteLoadAB);
				_SingleABLoaderCacheDict.Add(abName, _CurSingleABLoader);
				yield return _CurSingleABLoader.LoadAssetBundle();
			}
		}


		/// <summary>
		/// 加载指定的AB包中的指定资源
		/// </summary>
		/// <param name="abName">AB包名称</param>
		/// <param name="assetName">资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns></returns>
		public UnityEngine.Object LoadAsset(string abName, string assetName, bool isCache = false) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));
			if (string.IsNullOrEmpty(assetName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(assetName));

			return GetSingleABLoader(abName).LoadAsset(assetName, isCache);
		}


		/// <summary>
		/// 卸载指定的AB包中的指定资源
		/// </summary>
		/// <param name="abName">指定的AB包名称</param>
		/// <param name="asset">指定的资源</param>
		public void UnloadAsset(string abName,UnityEngine.Object asset) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));
			if (asset == null)
				throw new ArgumentException(FWDefine.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			GetSingleABLoader(abName).UnloadAsset(asset);
		}


		/// <summary>
		/// 释放指定的AB包中的内存镜像资源
		/// </summary>
		public void Dispose(string abName){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));
			
			GetSingleABLoader(abName).Dispose();
		}


		/// <summary>
		/// 释放指定的AB包中的内存镜像资源，且释放内存资源
		/// </summary>
		public void DisposeAll(string abName){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));

			GetSingleABLoader(abName).DisposeAll();
		}


		/// <summary>
		/// 查询指定的AB包中包含的所有资源名称
		/// </summary>
		public string[] RetrieveAll(string abName){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));

			return GetSingleABLoader(abName).RetrieveAll();
		}


		/// <summary>
		/// 释放本场景中的所有资源
		/// （一般在场景切换时使用）
		/// </summary>
		public void DisposeAllInScene() {
			try {
				//逐一释放所有加载过的AB包中的资源
				foreach (var item in _SingleABLoaderCacheDict.Values)
					item.DisposeAll();
			}
			finally {
				//TODO：使用反射技术，释放本类所有资源
				_SingleABLoaderCacheDict.Clear();
				_SingleABLoaderCacheDict = null;
				//释放其他的对象占用资源
				_ABRelationDict.Clear();
				_ABRelationDict = null;
				_CurABName = null;
				_CurSceneName = null;
				_LoadAllCompleteHandler = null;
				//卸载没有使用到的资源
				Resources.UnloadUnusedAssets();
				//强制垃圾收集
				GC.Collect();
			}
		}

		#endregion


		#region ［私有方法］

		/// <summary>
		/// 完成指定AB包调用
		/// </summary>
		/// <param name="abName"></param>
		private void CompleteLoadAB(string abName) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));

			if (abName.Equals(_CurABName)) {
				_LoadAllCompleteHandler?.Invoke(abName);
			}
		}


		/// <summary>
		/// 加载引用AB包
		/// </summary>
		/// <param name="abName">AB包的名称</param>
		/// <param name="refABName">被引用的AB包的名称</param>
		/// <returns></returns>
		private IEnumerator LoadReference(string abName, string refABName) {
			//如果AB包已经加载
			if (_ABRelationDict.ContainsKey(abName)) {
				ABRelation tempABRelation = _ABRelationDict[abName];
				//添加AB包引用关系（被依赖）
				tempABRelation.AddABReference(refABName);
			}
			//如果AB包未被加载
			else {
				ABRelation tempABRelation = new ABRelation(abName);
				tempABRelation.AddABReference(refABName);
				_ABRelationDict.Add(abName, tempABRelation);
			}
			//递归调用：开始加载所依赖的AB包
			yield return LoadAssetBundle(abName);
		}


		/// <summary>
		/// 得到对应的SingleABLoader
		/// </summary>
		/// <param name="abName">指定的AB包名称</param>
		/// <returns></returns>
		private SingleABLoader GetSingleABLoader(string abName){
			SingleABLoader tempSingleABLoader;
			_SingleABLoaderCacheDict.TryGetValue(abName, out tempSingleABLoader);
			if (tempSingleABLoader == null)
				throw new NullReferenceException("找不到对应的AssetBundle包！" + "\t" + "指定的AB包名称：" + abName);

			return tempSingleABLoader;
		}

		#endregion







	}
}