using System.Collections;
using UnityEngine;

namespace MyABFramework {
	public interface IManifestMgr {
		/// <summary>属性：WWW资源是否加载完成，清单文件是否加载完成</summary>
		bool IsLoadFinished { get; }

		/// <summary>加载Manifest清单文件</summary>
		IEnumerator LoadManifest();

		/// <summary>获取AssetBundleManifest系统类实例</summary>
		/// <returns>AssetBundleManifest系统类实例</returns>
		AssetBundleManifest GetManifest();

		/// <summary>获取AssetBundleManifest系统类中所有的依赖项</summary>
		string[] RetrieveDependencies(string bundleName);

		/// <summary>释放本类的资源</summary>
		/// <remarks>备注：一定要在最后调用</remarks>
		void Dispose();
	}
}