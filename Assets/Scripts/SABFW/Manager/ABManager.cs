/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 主流程，最上层：
 * 入口脚本：所有场景的AB包管理
 * 
 * ［功能］
 * 1.整个框架的入口；
 * 2.读取清单文件，缓存到本脚本中（但是有什么用？）；
 * 3.以场景为单位，管理整个项目中的所有AB包；
 * 4.可以方便地进行AB包中资源的加载、卸载，AB包的资源释放、资源查询。
 *
 * ［用法］
 * 在Awake方法中开始LoadAssetBundle协程，编写作为参数的委托对应的方法，可在其中实例化和克隆AB包中资源。
 * 可根据特定事件，触发调用资源加载、卸载，AB包的资源释放、资源查询等方法。
 *
 * ［注意］
 * 不检查场景名称的正确性，请使用场景类型枚举转化。
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Global;

namespace SABFW {
	/// <summary>
	/// 入口脚本：所有场景的AB包管理
	/// </summary>
	public class ABManager : MonoBehaviour, IABManager {

		#region ［单例模式］

		///<summary>本类实例</summary>
		private static ABManager _Instance;

		/// <summary>得到本类实例</summary>
		public static ABManager Instance => _Instance ?? (_Instance = new GameObject(NAME_GO).AddComponent<ABManager>());

		#endregion


		#region ［常量、字段和属性］

		/// <summary>要自动挂载的游戏对象名</summary>
		private const string NAME_GO = "_AssetBundleMgr";

		/// <summary>多个AB包加载类的缓存集合</summary>
		private readonly Dictionary<string, MultiABLoader> _MultiABLoaderCacheDict = new Dictionary<string, MultiABLoader>();
		///<summary>清单文件的系统类</summary>
		private AssetBundleManifest _ManifestObj = null;

		#endregion


		#region ［Unity消息］

		void Awake() {
			//加载Manifest清单文件
			StartCoroutine(ABManifestManager.Instance.LoadABMainfest());
		}

		#endregion



		#region ［公共方法］

		/// <summary>
		///加载指定场景的指定AB包
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		/// <param name="handler">加载完成时调用的委托</param>
		/// <returns></returns>
		public IEnumerator LoadAssetBundle(string sceneName, string abName, DelLoadComplete handler) {
			//参数检查
			if (string.IsNullOrEmpty(sceneName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(sceneName));
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));

			//等待清单文件加载完成
			while (!ABManifestManager.Instance.IsLoadFinished)
				yield return null;
			_ManifestObj = ABManifestManager.Instance.GetABManifest();

			//把当前场景加入集合中
			if (!_MultiABLoaderCacheDict.ContainsKey(sceneName)) {
				MultiABLoader multiABLoader = new MultiABLoader(sceneName, abName, handler);
				_MultiABLoaderCacheDict.Add(sceneName, multiABLoader);
			}

			//加载指定的AB包
			yield return _MultiABLoaderCacheDict[sceneName].LoadAssetBundle(abName);
		}


		/// <summary>
		/// 加载指定场景的指定AB包中的指定资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns></returns>
		public UnityEngine.Object LoadAsset(string sceneName, string abName, string assetName, bool isCache = false) {
			return GetMultiABLoader(sceneName).LoadAsset(abName, assetName, isCache);
		}


		/// <summary>
		/// 卸载指定场景的指定AB包中的指定资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		/// <param name="asset">指定的资源</param>
		public void UnloadAsset(string sceneName, string abName, UnityEngine.Object asset) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));
			if (asset == null)
				throw new ArgumentException(FWDefine.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			GetMultiABLoader(sceneName).UnloadAsset(abName,asset);
		}


		/// <summary>
		/// 释放指定场景的指定AB包中的内存镜像资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		public void Dispose(string sceneName, string abName) {
			GetMultiABLoader(sceneName).Dispose(abName);
		}


		/// <summary>
		/// 释放指定场景的指定B包中的内存镜像资源，且释放内存资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		public void DisposeAll(string sceneName, string abName) {
			GetMultiABLoader(sceneName).DisposeAll(abName);
		}


		/// <summary>
		/// 查询指定场景的指定AB包中包含的所有资源名称
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		public string[] RetrieveAll(string sceneName,string abName) {
			return GetMultiABLoader(sceneName).RetrieveAll(abName);
		}


		/// <summary>
		/// 释放指定场景中的所有资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		public void DisposeAllInScene(string sceneName) {
			GetMultiABLoader(sceneName).DisposeAllInScene();
		}

		#endregion


		#region 私有方法

		/// <summary>
		/// 得到对应的MultiABLoader
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <returns></returns>
		private MultiABLoader GetMultiABLoader(string sceneName) {
			MultiABLoader tempMultiABLoader;
			_MultiABLoaderCacheDict.TryGetValue(sceneName, out tempMultiABLoader);
			if (tempMultiABLoader == null)
				throw new NullReferenceException("找不到对应的同一场景下的AssetBundle包！" + "\t" + "指定的场景名称：" + sceneName);

			return tempMultiABLoader;
		}

		#endregion


	}
}