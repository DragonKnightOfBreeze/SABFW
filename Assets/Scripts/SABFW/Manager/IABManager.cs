/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 接口：
 * 入口脚本：所有场景的AB包管理
 * 
 */
using System.Collections;

namespace SABFW {
	public interface IABManager {
		/// <summary>
		///加载指定场景的指定AB包
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		/// <param name="handler">加载完成时调用的委托</param>
		/// <returns></returns>
		IEnumerator LoadAssetBundle(string sceneName, string abName, DelLoadComplete handler);

		/// <summary>
		/// 加载指定场景的指定AB包中的指定资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns></returns>
		UnityEngine.Object LoadAsset(string sceneName, string abName, string assetName, bool isCache = false);

		/// <summary>
		/// 卸载指定场景的指定AB包中的指定资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		/// <param name="asset">指定的资源</param>
		void UnloadAsset(string sceneName, string abName, UnityEngine.Object asset);

		/// <summary>
		/// 释放指定场景的指定AB包中的内存镜像资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		void Dispose(string sceneName, string abName);

		/// <summary>
		/// 释放指定场景的指定B包中的内存镜像资源，且释放内存资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		void DisposeAll(string sceneName, string abName);

		/// <summary>
		/// 查询指定场景的指定AB包中包含的所有资源名称
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		/// <param name="abName">指定的AB包名称</param>
		string[] RetrieveAll(string sceneName,string abName);

		/// <summary>
		/// 释放指定场景中的所有资源
		/// </summary>
		/// <param name="sceneName">指定的场景名称</param>
		void DisposeAllInScene(string sceneName);
	}
}