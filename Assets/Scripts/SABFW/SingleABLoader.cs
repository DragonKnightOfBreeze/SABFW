/*******
 * ［标题］
 * 项目：AssetBundle框架设计
 * 作者：微风的龙骑士 风游迩
 * 
 * 框架主流程：第2层
 * 通过WWW加载AssetBundle
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 *  
 * 
 * ［用法］
 * 
 */
using System;
using System.Collections;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 
	/// </summary>
	public class SingleABLoader : IDisposable {

		///<summary>引用类：资源加载类</summary>
		private AssetLoader _AssetLoader;
		/// <summary>AssetBundle名称</summary>
		private string _ABName;
		/// <summary>AssetBundle下载路径</summary>
		private string _ABDowloadPath;
		/// <summary>委托：加载完成</summary>
		private DelLoadComplete _LoadCompleteHandler;


		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="abName">AB包名</param>
		/// <param name="loadComplete">加载完成时要调用的方法</param>
		public SingleABLoader(string abName,DelLoadComplete loadComplete){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			_ABName = abName;
			//AB包的下载路径
			_ABDowloadPath = PathTools.GetWWWPath()+"/"+_ABName;
			//委托的初始化
			_LoadCompleteHandler = loadComplete;
		}

		/// <summary>
		/// 加载AssetBundle资源包
		/// </summary>
		public IEnumerator LoadAssetBundle(){
			//离开范围，自动释放资源
			using (WWW www = new WWW(_ABDowloadPath)) {
				yield return www;
				//如果www下载资源包已经完成
				if (www.progress >= 1) {
					//获取AssetBundle实例
					AssetBundle abObj = www.assetBundle;
					//空引用检查
					if (abObj == null) 
						throw new NullReferenceException("WWW资源下载出错！" + "\t" + "AssetBundle URL：" + _ABDowloadPath + "\t" + "错误信息：" + www.error);

					//实例化引用类
					_AssetLoader = new AssetLoader(abObj);
					//AB下载完毕，调用委托
					_LoadCompleteHandler?.Invoke(_ABName);
				}
			}
		}	
			
		/// <summary>
		/// 加载AB包内的资源
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="isCache"></param>
		/// <returns></returns>
		public UnityEngine.Object LoadAsset(string assetName,bool isCache){
			if(string.IsNullOrEmpty(assetName))
				throw new ArgumentException(nameof(assetName));
			if (_AssetLoader == null)  
				throw new NullReferenceException("参数_AssetLoader为空！" + "\t" + nameof(_AssetLoader));

			return _AssetLoader.LoadAsset(assetName, isCache);
		}


		/// <summary>
		/// 卸载AB包中的资源
		/// </summary>
		/// <param name="asset"></param>
		public void UnloadAsset(UnityEngine.Object asset){
			if (_AssetLoader == null)
				throw new NullReferenceException("参数_AssetLoader为空！" + "\t" + nameof(_AssetLoader));

			_AssetLoader.UnloadAsset(asset);
		}


		/// <summary>
		/// 释放AB包中的资源
		/// </summary>
		public void Dispose(){
			if (_AssetLoader == null)
				throw new NullReferenceException("参数_AssetLoader为空！" + "\t" + nameof(_AssetLoader));

			_AssetLoader.Dispose();
			_AssetLoader = null;
		}


		/// <summary>
		/// 释放当前AB、资源包，且释放所有资源
		/// </summary>
		public void DisposeAll(){
			if (_AssetLoader == null)
				throw new NullReferenceException("参数_AssetLoader为空！" + "\t" + nameof(_AssetLoader));

			_AssetLoader.DisposeAll();
			_AssetLoader = null;
		}


		/// <summary>
		/// 查询当前AssetBundle中包含的所有资源名称
		/// </summary>
		public string[] RetriveAllAssetNames(){
			if (_AssetLoader == null)
				throw new NullReferenceException("参数_AssetLoader为空！" + "\t" + nameof(_AssetLoader));

			return _AssetLoader.RetriveAllAssetName();
		}
	}
}