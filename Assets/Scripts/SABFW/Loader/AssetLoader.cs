/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 框架主流程：第一层
 * AB资源加载类
 * 
 * ［功能］
 * 1.管理与加载指定AB的资源
 * 2.加载具有缓存功能的资源，带有默认参数
 * 3.卸载、释放AB资源
 * 4.查看当前AB包内资源
 * 
 */
using System;
using System.Collections;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// AB资源加载类
	/// </summary>
	public class AssetLoader : IDisposable {

		#region ［字段］

		///<summary>当前AssetBundle包</summary>
		private readonly AssetBundle _CurAssetBundle;
		/// <summary>缓存容器集合</summary>
		private readonly Hashtable _CacheHashtable;

		#endregion


		#region ［构造器］

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="abObj">AssetBundle包</param>
		public AssetLoader(AssetBundle abObj) {
			if (abObj == null)
				throw new ArgumentException(FWDefine.PREFIX + "AssetBundle包为空！" + "\t" + nameof(abObj));

			_CurAssetBundle = abObj;
			_CacheHashtable = new Hashtable();
		}

		#endregion


		#region ［公共方法］

		/// <summary>
		/// 加载当前AB包中的指定的资源
		/// </summary>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否缓存</param>
		public UnityEngine.Object LoadAsset(string assetName, bool isCache = false) {
			if (string.IsNullOrEmpty(assetName))
				throw new ArgumentException(FWDefine.PREFIX+ nameof(assetName));

			return LoadResource<UnityEngine.Object>(assetName, isCache);
		}


		/// <summary>
		/// 卸载当前AB包中的指定的资源
		/// </summary>
		/// <param name="asset">指定的资源</param>
		public void UnloadAsset(UnityEngine.Object asset) {
			if (asset == null)
				throw new ArgumentException(FWDefine.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			Resources.UnloadAsset(asset);
		}

		/// <summary>
		/// 释放当前AB包中的内存镜像资源
		/// </summary>
		public void Dispose() {
			_CurAssetBundle.Unload(false);
		}

		/// <summary>
		/// 释放当前AB包中的内存镜像资源，且释放内存资源
		/// </summary>
		public void DisposeAll() {
			_CurAssetBundle.Unload(true);
		}

		/// <summary>
		/// 查询当前AB包中包含的所有资源名称
		/// </summary>
		public string[] RetrieveAll() {
			return _CurAssetBundle.GetAllAssetNames();
		}

		#endregion


		#region ［私有方法］

		/// <summary>
		/// 加载当前AB包中的资源，带缓存
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否缓存</param>
		private T LoadResource<T>(string assetName, bool isCache)
		where T : UnityEngine.Object {
			//如果缓存集合已经存在
			if (_CacheHashtable.Contains(assetName)) 
				return _CacheHashtable[assetName] as T;

			//正式加载
			T tempTResource = _CurAssetBundle.LoadAsset<T>(assetName);
			if (tempTResource == null)
				throw new NullReferenceException(FWDefine.PREFIX+ "要读取的资源为空！" + "\t" +"指定的资源名称："+assetName );

			//判断是否缓存
			if (isCache)
				_CacheHashtable.Add(assetName, tempTResource);

			return tempTResource;
		}

		#endregion








		
	}
}