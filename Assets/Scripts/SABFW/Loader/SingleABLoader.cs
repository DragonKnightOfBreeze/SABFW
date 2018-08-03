/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 框架主流程：第二层
 * 通过WWW加载AssetBundle
 *
 *［用法］
 * 在Start方法中实例化SingleABLoader类（参数：AB包名，一个方法），并开始这个类的LoadAssetBundle协程，加载WWW资源；
 * 编写作为参数的方法，直到加载完所有依赖项位置，递归重复Start方法中的步骤；
 * 编写最终的作为参数的方法，这时可以通过SingleABLoader对象的LoadAsset方法，实例化一个游戏对象，然后将之克隆出来。
 * 其他的公共方法可以设置为通过某一按键触发调用。
 * 
 * 代码实例：
		void Start(){
			SingleABLoader _LoadDependObj1 = new SingleABLoader(_ABDependName1,LoadDependComplete1);
			StartCoroutine(_LoadDependObj1.LoadAssetBundle());
		}
		private void LoadDependComplete1(string abName){
			_LoadObj = new SingleABLoader(_ABName, LoadComplete);
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
 * 
 */
using System;
using System.Collections;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 通过WWW加载AssetBundle
	/// </summary>
	public class SingleABLoader : IDisposable {


		#region ［字段和属性］

		///<summary>引用类：资源加载类</summary>
		private AssetLoader _AssetLoader;
		/// <summary>AssetBundle名称</summary>
		private readonly string _ABName;
		/// <summary>AssetBundle下载路径</summary>
		private readonly string _ABDowloadPath;
		/// <summary>委托：加载完成</summary>
		private readonly DelLoadComplete _LoadCompleteHandler;
		/// <summary>WWW资源是否加载完成，AssetLoader是否实例化完成</summary>
		private bool _IsLoadFinished;

		/// <summary>属性：WWW资源是否加载完成，AssetLoader是否实例化完成</summary>
		public bool IsLoadFinished {
			get {
				if(!_IsLoadFinished)
					Debug.LogWarning(FWDefine.PREFIX + "AssetLoader实例化未完成！");
				return _IsLoadFinished;
			}
		}

		#endregion


		#region ［构造器］

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="abName">AB包名</param>
		/// <param name="handler">加载完成时调用的委托</param>
		public SingleABLoader(string abName, DelLoadComplete handler) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(abName));

			_ABName = abName;
			//AB包的下载路径
			_ABDowloadPath = PathTools.GetWWWPath() + "/" + _ABName;
			//委托的初始化
			_LoadCompleteHandler = handler;
			_IsLoadFinished = false;
		}

		#endregion


		#region ［公共方法］

		/// <summary>
		/// 加载AB包
		/// </summary>
		public IEnumerator LoadAssetBundle() {
			//离开范围，自动释放资源
			using (WWW www = new WWW(_ABDowloadPath)) {
				yield return www;
				//如果www下载资源包已经完成
				if (www.progress >= 1) {
					//获取AssetBundle实例
					AssetBundle abObj = www.assetBundle;
					//空引用检查
					if (abObj == null)
						throw new NullReferenceException(FWDefine.PREFIX + "WWW资源下载出错！" + "\t" + "AssetBundle地址：" + _ABDowloadPath + "\t" + "错误信息：" + www.error);

					//实例化引用类
					_AssetLoader = new AssetLoader(abObj);
					//AB下载完毕，调用委托
					_LoadCompleteHandler?.Invoke(_ABName);

					_IsLoadFinished = true;
				}
			}
		}


		/// <summary>
		/// 加载当前AB包中的指定的资源
		/// </summary>
		/// <param name="assetName">指定的资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns></returns>
		public UnityEngine.Object LoadAsset(string assetName, bool isCache = false) {
			if (string.IsNullOrEmpty(assetName))
				throw new ArgumentException(FWDefine.PREFIX + nameof(assetName));

			if (!IsLoadFinished)
				return null;
			return _AssetLoader.LoadAsset(assetName, isCache);
		}


		/// <summary>
		/// 卸载当前AB包中的指定的资源
		/// </summary>
		/// <param name="asset">指定的资源</param>
		public void UnloadAsset(UnityEngine.Object asset) {
			if (asset == null)
				throw new ArgumentException(FWDefine.PREFIX + "要卸载的资源为空！" + "\t" + nameof(asset));

			if (!IsLoadFinished)
				return;
			_AssetLoader.UnloadAsset(asset);
		}


		/// <summary>
		/// 释放当前AB包中的内存镜像资源
		/// </summary>
		public void Dispose() {
			if (!IsLoadFinished)
				return;
			_AssetLoader.Dispose();
			_AssetLoader = null;
		}


		/// <summary>
		/// 释放当前AB包中的内存镜像资源，且释放内存资源
		/// </summary>
		public void DisposeAll() {
			if (!IsLoadFinished)
				return;
			_AssetLoader.DisposeAll();
			_AssetLoader = null;
		}


		/// <summary>
		/// 查询当前AB包中包含的所有资源名称
		/// </summary>
		public string[] RetrieveAll() {
			if (!IsLoadFinished)
				return null;
			return _AssetLoader.RetrieveAll();
		}

		#endregion
	}
}