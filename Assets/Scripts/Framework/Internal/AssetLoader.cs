/**
 * ［概述］
 * 第一层：AB资源加载类
 *
 * 1.管理与加载指定AssetBundle包的资源
 * 2.加载具有缓存功能的资源，带有默认参数
 * 3.卸载、释放AssetBundle资源
 * 4.查看当前AssetBundle包内的资源
 * 
 * ［用法］
 * 
 * 
 * ［备注］ 
 * 
 * 
 * 项目：自用AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 */ 

using System;
using System.Collections;
using MyABFramework.Global;
using UnityEngine;

namespace MyABFramework {
	/// <summary>第一层：AB资源加载类</summary>
	public class AssetLoader {
		
		#region ［字段］

		///<summary>当前的AssetBundle包</summary>
		private readonly AssetBundle curBundle;
		/// <summary>缓存容器集合</summary>
		private readonly Hashtable cacheHashtable;

		#endregion


		#region ［构造器］

		/// <summary>构造方法</summary>
		/// <param name="bundle">AssetBundle包</param>
		internal AssetLoader(AssetBundle bundle) {
//			if(bundle == null)
//				throw new ArgumentException(FrameConst.PREFIX + "AssetBundle包为空！",nameof(bundle));

			curBundle = bundle;
			cacheHashtable = new Hashtable();
		}

		#endregion


		#region ［内部方法］

		/// <summary>加载当前AB包中的指定资源</summary>
		/// <param name="name">指定的资源名称</param>
		/// <param name="isCache">是否缓存</param>
		internal UnityEngine.Object LoadAsset(string name, bool isCache = false) {
//			if(string.IsNullOrWhiteSpace(assetName))
//				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！",nameof(assetName));

			return LoadResource<UnityEngine.Object>(name, isCache);
		}


		/// <summary>卸载当前AB包中的指定资源</summary>
		/// <param name="asset">指定的资源</param>
		internal void UnloadAsset(UnityEngine.Object asset) {
//			if(asset == null)
//				throw new ArgumentException(FrameConst.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			Resources.UnloadAsset(asset);
		}

		
		/// <summary>释放当前包中的内存镜像资源</summary>
		internal void Dispose() {
			curBundle.Unload(false);
		}


		/// <summary>释放当前包中的内存镜像资源，且释放内存资源</summary>
		internal void DisposeAll() {
			curBundle.Unload(true);
		}

		
		/// <summary>查询当前包中的所有资源的名称</summary>
		internal string[] RetrieveAll() {
			return curBundle.GetAllAssetNames();
		}

		#endregion


		#region ［私有方法］

		/// <summary>加载当前包中的资源，带缓存</summary>
		/// <param name="name">指定的资源名称</param>
		/// <param name="isCache">是否缓存</param>
		private T LoadResource<T>(string name, bool isCache) where T : UnityEngine.Object {
			//如果缓存集合中存在，则返回
			if(cacheHashtable.Contains(name))
				return cacheHashtable[name] as T;

			//正式加载
			T resource = curBundle.LoadAsset<T>(name);
			if(resource == null)
				throw new ArgumentException(FrameConst.PREFIX + $"要读取的资源为空！指定的资源名称：{name}",nameof(resource));
			//判断是否缓存
			if(isCache)
				cacheHashtable.Add(name, resource);
			return resource;
		}

		#endregion
	}
}