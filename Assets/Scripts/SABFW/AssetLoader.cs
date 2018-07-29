/*******
 * ［标题］
 * 项目：AssetBundle框架设计
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
 * ［思路］
 *  
 * 
 * ［用法］
 *
 *
 * TODO：到底是使用自定义异常，还是使用 Debug.LogWarning 和 Debug.LogError？
 */
using System;
using System.Collections;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// AB资源加载类
	/// </summary>
	public class AssetLoader : IDisposable {

		///<summary>当前AssetBundle</summary>
		private readonly AssetBundle _CurAssetBundle;
		/// <summary>缓存容器集合</summary>
		private readonly Hashtable _HtCache;


		public AssetLoader(AssetBundle abObj){
			if (abObj == null)
				throw new ArgumentException( "AssetBundle资源为空！" + "\t" + nameof(abObj));

			_CurAssetBundle = abObj;
			_HtCache = new Hashtable();
		}


		/// <summary>
		/// 加载当前包中指定的资源
		/// </summary>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否缓存</param>
		public UnityEngine.Object LoadAsset(string assetName, bool isCache = false){
			if(string.IsNullOrEmpty(assetName))
				throw new ArgumentException(nameof(assetName));

			return LoadResource<UnityEngine.Object>(assetName, isCache);
		}

		/// <summary>
		/// 加载当前AB包的资源，带缓存
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否缓存</param>
		private T LoadResource<T>(string assetName, bool isCache)
			where T : UnityEngine.Object{
			//如果缓存集合已经存在
			if (_HtCache.Contains(assetName)) {
				return _HtCache[assetName] as T;
			}

			//正式加载
			T tempTResource = _CurAssetBundle.LoadAsset<T>(assetName);
			//空引用检查
			if (tempTResource == null)
				throw new NullReferenceException("要读取的资源为空！" + "\t" + nameof(tempTResource));

			//判断是否缓存
			if (isCache) {
				_HtCache.Add(assetName, tempTResource);
			}
			return tempTResource;
		}

		/// <summary>
		/// 卸载指定的资源
		/// </summary>
		/// <param name="asset"></param>
		public void UnloadAsset(UnityEngine.Object asset){
			if (asset == null)
				throw new ArgumentException("要卸载的资源为空！" + "\t" + nameof(asset));

			Resources.UnloadAsset(asset);
		}

		/// <summary>
		/// 释放当前AssetBundle内存镜像资源
		/// </summary>
		public void Dispose(){
			_CurAssetBundle.Unload(false);
		}

		/// <summary>
		/// 释放当前AssetBundle内存镜像资源，且释放内存资源
		/// </summary>
		public void DisposeAll(){
			_CurAssetBundle.Unload(true);
		}

		/// <summary>
		/// 查询当前AssetBundle中包含的所有资源名称
		/// </summary>
		public string[] RetriveAllAssetName(){
			return _CurAssetBundle.GetAllAssetNames();
		}
	}
}