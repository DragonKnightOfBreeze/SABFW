/**
 * ［概述］
 * 第二层：单包加载
 *
 * 通过WWW加载AssetBundle。
 * 
 * ［用法］
 * 在Start方法中实例化SingleABLoader类（参数：AB包名，一个方法），并开始这个类的LoadAssetBundle协程，加载WWW资源；
 * 编写作为参数的方法，直到加载完所有依赖项位置，递归重复Start方法中的步骤；
 * 编写最终的作为参数的方法，这时可以通过SingleABLoader对象的LoadAsset方法，实例化一个游戏对象，然后将之克隆出来。
 * 其他的公共方法可以设置为通过某一按键触发调用。
 * 		
 * ［备注］ 
 * 
 * 
 * 项目：自用AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 */ 

/*［调用代码示例］

void Start(){
		SingleBundleLoader _LoadDependObj1 = new SingleBundleLoader(_ABDependName1,LoadDependComplete1);
		StartCoroutine(_LoadDependObj1.LoadAssetBundle());
	}
	private void LoadDependComplete1(string abName){
		_LoadObj = new SingleBundleLoader(_ABName, LoadComplete);
		StartCoroutine(_LoadDependObj2.LoadAssetBundle());
	}
	private void LoadComplete(string abName){
		UnityEngine.Object tempObj1 =  _LoadObj.LoadAsset(_AssetName1,false);
		Instantiate(tempObj1);
	}
	void Update(){
		if (Input.GetKeyDown(KeyCode.A)) {
			_LoadObj.RetrieveAll();
			//_LoadObj.Dispose();		
			//_LoadObj.DisposeAll();
			//...
		}
	}
}
 */

using System;
using System.Collections;
using MyABFramework.Global;
using UnityEngine;

namespace MyABFramework {
	/// <summary>第二层：单包加载</summary>
	public class SingleBundleLoader {
		#region ［字段和属性］

		///<summary>引用的资源加载类</summary>
		private AssetLoader assetLoader;
		/// <summary>AssetBundle包名</summary>
		private readonly string bundleName;
		
		/// <summary>AssetBundle下载路径</summary>
		private readonly string bundleDownloadPath;
		/// <summary>委托，在加载完成时调用</summary>
		private readonly LoadCompleteDelegate loadCompleteHandler;
		/// <summary>WWW资源是否加载完成，AssetLoader是否实例化完成</summary>
		private bool isLoadFinished;

		/// <summary>属性：WWW资源是否加载完成，AssetLoader是否实例化完成</summary>
		private bool IsLoadFinished {
			get {
				if(!isLoadFinished)
					Debug.LogWarning(FrameConst.PREFIX + "AssetLoader实例化未完成！");
				return isLoadFinished;
			}
		}

		#endregion


		#region ［构造器］

		/// <summary>构造函数</summary>
		/// <param name="bundleName">AssetBundle包名</param>
		/// <param name="handler">加载完成时调用的委托</param>
		internal SingleBundleLoader(string bundleName, LoadCompleteDelegate handler) {
//			if(string.IsNullOrEmpty(abName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(abName));

			this.bundleName = bundleName;
			bundleDownloadPath = PathTools.GetWWWPath() + "/" + this.bundleName;
			isLoadFinished = false;
			loadCompleteHandler = handler;
		}

		#endregion


		#region ［内部方法］

		/// <summary>加载AssetBundle包</summary>
		internal IEnumerator LoadAssetBundle() {
			//离开范围，则自动释放资源
			using(WWW www = new WWW(bundleDownloadPath)) {
				yield return www;
				//如果www下载资源包已经完成
				if(www.progress >= 1) {
					//获取AssetBundle实例
					AssetBundle bundle = www.assetBundle;
					if(bundle == null)
						throw new ArgumentException(FrameConst.PREFIX + $"WWW资源下载出错！AssetBundle地址：{bundleDownloadPath}，错误信息：{www.error}",nameof(bundle));
					//实例化引用类
					assetLoader = new AssetLoader(bundle);
					//AssetBundle下载完毕，调用对应的委托
					isLoadFinished = true;
					loadCompleteHandler?.Invoke(bundleName);
				}
			}
		}


		/// <summary>加载当前包中的指定资源</summary>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns>当前包中的指定资源</returns>
		internal UnityEngine.Object LoadAsset(string assetName, bool isCache = false) {
//			if(string.IsNullOrEmpty(assetName))
//				throw new ArgumentException(FrameConst.PREFIX + nameof(assetName));
			if(!IsLoadFinished)
				return null;
			return assetLoader.LoadAsset(assetName, isCache);
		}


		/// <summary>卸载当前包中的指定资源</summary>
		/// <param name="asset">指定的资源</param>
		internal void UnloadAsset(UnityEngine.Object asset) {
//			if(asset == null)
//				throw new ArgumentException(FrameConst.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			if(!IsLoadFinished)
				return;
			assetLoader.UnloadAsset(asset);
		}


		/// <summary>释放当前包中的内存镜像资源</summary>
		internal void Dispose() {
			if(!IsLoadFinished)
				return;
			assetLoader.Dispose();
			assetLoader = null;
		}


		/// <summary>释放当前包中的内存镜像资源，且释放内存资源</summary>
		internal void DisposeAll() {
			if(!IsLoadFinished)
				return;
			assetLoader.DisposeAll();
			assetLoader = null;
		}


		/// <summary>查询当前包中的所有资源的名称</summary>
		internal string[] RetrieveAll() {
			if(!IsLoadFinished)
				return null;
			return assetLoader.RetrieveAll();
		}

		#endregion
	}
}