using System;
using System.Collections;
using MyABFramework.Global;

namespace MyABFramework {
	public interface IAssetBundleMgr {
		/// <summary>加载指定场景的指定包</summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="bundleName">指定的包名</param>
		/// <param name="handler">加载完成时调用的委托</param>
		/// <param name="allHandler">全部加载完成时调用的委托</param>
		IEnumerator LoadAssetBundle(string sceneName, string bundleName,LoadCompleteDelegate handler = null, LoadAllCompleteDelegate allHandler = null);

		/// <summary>加载指定场景的指定包中的指定资源</summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="bundleName">指定的包名</param>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns>指定的资源</returns>
		UnityEngine.Object LoadAsset(string sceneName, string bundleName, string assetName, bool isCache = false);

		/// <summary>卸载指定场景的指定包中的指定资源</summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="bundleName">指定的包名</param>
		/// <param name="asset">指定的资源</param>
		void UnloadAsset(string sceneName, string bundleName, UnityEngine.Object asset);

		/// <summary>释放指定场景的指定包中的内存镜像资源</summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="bundleName">指定的包名</param>
		void Dispose(string sceneName, string bundleName);

		/// <summary>释放指定场景的指包中的内存镜像资源，且释放内存资源</summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="bundleName">指定的包名</param>
		void DisposeAll(string sceneName, string bundleName);

		/// <summary>释放指定场景中的所有资源</summary>
		/// <param name="sceneName">指定的场景名称</param>
		void DisposeAllInScene(string sceneName);

		/// <summary>查询指定场景的指定包中包含的所有资源名称</summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="bundleName">指定的包名</param>
		string[] RetrieveAll(string sceneName, string bundleName);
	}
}