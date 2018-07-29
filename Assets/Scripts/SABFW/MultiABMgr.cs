/*******
 * ［标题］
 * 项目：AssetBundle框架设计
 * 作者：微风的龙骑士 风游迩
 * 
 * 主流程（第3层）：一个场景中的多个AssetBundle的管理
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
 * ［用法］
 *
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 多个AB包的管理
	/// </summary>
	public class MultiABMgr {

		///<summary>（下层）引用类：单个AB包加载实现类</summary>s
		private SingleABLoader _CurSingleABLoader;

		///<summary>AB包实现类的缓存集合</summary>
		private Dictionary<string, SingleABLoader> _DicSingleABLoaderCache;

		///<summary>当前场景名称（调试使用）</summary>
		private string _CurSceneName;

		///<summary>当前AB包名称</summary>
		private string _CurABName;

		///<summary>AB包名称与对应依赖关系的集合</summary>
		private Dictionary<string, ABRelation> _DicABRelation;

		///<summary>委托：所有的AB包是否加载完成</summary>
		private DelLoadComplete _LoadAllABsCompleteHandler;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="sceneName">场景名称</param>
		/// <param name="abName">AB包名称</param>
		/// <param name="handler">委托：是否调用完成</param>
		public MultiABMgr(string sceneName, string abName, DelLoadComplete handler){
			if (string.IsNullOrEmpty(sceneName))
				throw new ArgumentException(nameof(sceneName));
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			_CurSceneName = sceneName;
			_CurABName = abName;
			_DicSingleABLoaderCache = new Dictionary<string, SingleABLoader>();
			_DicABRelation = new Dictionary<string, ABRelation>();
			_LoadAllABsCompleteHandler = handler;
		}


		/// <summary>
		/// 加载AB包
		/// </summary>
		/// <param name="abName"></param>
		/// <returns></returns>
		public IEnumerator LoadAssetBundle(string abName){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			//AB包关系的建立
			if (!_DicABRelation.ContainsKey(abName)) {
				_DicABRelation.Add(abName, new ABRelation(abName));
			}
			ABRelation tempABRelationObj = _DicABRelation[abName];

			//得到指定AB包所有的依赖关系（查询相应的清单文件）
			string[] DependenceArray = ABManifestLoader.GetInstance().RetrieveDependencies(abName);
			foreach (var dependenceItem in DependenceArray) {
				//添加依赖项
				tempABRelationObj.AddABDependence(dependenceItem);
				//添加引用项
				//协程方法（递归调用）
				yield return LoadReference(dependenceItem, abName);
			}

			//真正加载AB包，并等待加载完成
			if (_DicSingleABLoaderCache.ContainsKey(abName)) {
				yield return _DicSingleABLoaderCache[abName].LoadAssetBundle();
			}
			else {
				_CurSingleABLoader = new SingleABLoader(abName, CompleteLoadAB);
				_DicSingleABLoaderCache.Add(abName, _CurSingleABLoader);
				yield return _CurSingleABLoader.LoadAssetBundle();
			}
		}


		/// <summary>
		/// 加载AB包中的资源
		/// </summary>
		/// <param name="abName">AB包名称</param>
		/// <param name="assetName">资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns></returns>
		public UnityEngine.Object LoadAsset(string abName, string assetName, bool isCache = false){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));
			if (string.IsNullOrEmpty(assetName))
				throw new ArgumentException(nameof(assetName));

			SingleABLoader tempSingleABLoader;
			_DicSingleABLoaderCache.TryGetValue(abName, out tempSingleABLoader);
			//空引用检查
			if (tempSingleABLoader == null)
				throw new NullReferenceException("无法加载指定的资源！" + "\t" + nameof(tempSingleABLoader));
			return tempSingleABLoader.LoadAsset(assetName, isCache);
		}


		/// <summary>
		/// 释放本场景中所有资源
		/// （在场景切换时使用）
		/// </summary>
		public void DisposeAllAssets(){
			try {
				//逐一释放所有加载过的AB包中的资源
				foreach (var item in _DicSingleABLoaderCache.Values) {
					item.DisposeAll();
				}
			}
			finally {
				_DicSingleABLoaderCache.Clear();
				_DicSingleABLoaderCache = null;

				//释放其他的对象占用资源
				_DicABRelation.Clear();
				_DicABRelation = null;
				_CurABName = null;
				_CurSceneName = null;
				_LoadAllABsCompleteHandler = null;

				//卸载没有使用到的资源
				Resources.UnloadUnusedAssets();
				//强制垃圾收集
				GC.Collect();
			}
		}


		/// <summary>
		/// 完成指定AB包调用
		/// </summary>
		/// <param name="abName"></param>
		private void CompleteLoadAB(string abName) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			if (abName.Equals(_CurABName)) {
				_LoadAllABsCompleteHandler?.Invoke(abName);
			}
		}


		/// <summary>
		/// 加载引用AB包
		/// </summary>
		/// <param name="abName">AB包的名称</param>
		/// <param name="refABName">被引用的AB包的名称</param>
		/// <returns></returns>
		private IEnumerator LoadReference(string abName, string refABName){
			//如果AB包已经加载
			if (_DicABRelation.ContainsKey(abName)) {
				ABRelation tempABRelationObj = _DicABRelation[abName];
				//添加AB包引用关系（被依赖）
				tempABRelationObj.AddABReference(refABName);
			}
			else {
				ABRelation tempABRelationObj = new ABRelation(abName);
				tempABRelationObj.AddABReference(refABName);
				_DicABRelation.Add(abName, tempABRelationObj);
			}
			//TODO	应该是写在循环外
			//递归调用：开始加载所依赖的AB包
			yield return LoadAssetBundle(abName);
		}
	}
}