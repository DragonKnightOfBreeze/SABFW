/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 *
 * 接口：
 * AssetBundle清单文件读取类
 * 
 */

using System;
using System.Collections;
using UnityEngine;

namespace SABFW {
	public interface IABManifestManager : IDisposable {
		/// <summary>属性：WWW资源是否加载完成，清单文件是否加载完成</summary>
		bool IsLoadFinished { get; }

		/// <summary>
		/// 加载Manifest清单文件
		/// </summary>
		/// <returns></returns>
		IEnumerator LoadABMainfest();

		/// <summary>
		/// 获取AssetBundleManifest系统类实例
		/// </summary>
		/// <returns></returns>
		AssetBundleManifest GetABManifest();

		/// <summary>
		/// 获取AssetBundleManifest（系统类）中所有的依赖项
		/// </summary>
		string[] RetrieveDependencies(string abName);
	}
}