/*******
 * ［标题］
 * 项目：AssetBundle框架设计
 * 作者：微风的龙骑士 风游迩
 * 
 * 辅助类：读取清单文件
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
using static SABFW.FWDefine;

namespace SABFW {
	/// <summary>
	/// 读取清单文件
	/// </summary>
	public class ABManifestLoader : IDisposable {

		/// <summary>本类实例</summary>
		private static ABManifestLoader _Instance;
          
		/// <summary>得到本类实例</summary>
		public static ABManifestLoader GetInstance(){
			if (_Instance == null) 
				_Instance = new ABManifestLoader();
			return _Instance;
		}


		/// <summary>清单文件的系统类</summary>
		private AssetBundleManifest _ManifestObj;
		/// <summary>清单文件的路径</summary>
		private string _StrManifestPath;
		/// <summary>读取AB清单文件的AssetBundle</summary>
		private AssetBundle _ABReadManifest;
		/// <summary>清单文件是否加载完成</summary>
		private bool _IsLoadFinish;

		public bool IsLoadFinish {
			get { return _IsLoadFinish; }
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		private ABManifestLoader(){
			//确定清单文件WWW下载路径
			_StrManifestPath = PathTools.GetWWWPath() + "/" + PathTools.GetPlatformName();
			_ManifestObj = null;
			_ABReadManifest = null;
			_IsLoadFinish = false;
		}


		/// <summary>
		/// 加载Manifest清单文件
		/// </summary>
		/// <returns></returns>
		public IEnumerator LoadMainfesetFile(){
			using (WWW www = new WWW(_StrManifestPath)) {
				yield return www;
				if (www.progress >= 1) {
					//加载完成，获取AB实例
					AssetBundle abObj = www.assetBundle;
					//空引用检查
					if (abObj == null) 
						throw new NullReferenceException("WWW资源下载出错！" + "\t" + "AssetBundle URL：" + _StrManifestPath + "\t" + "错误信息：" + www.error);

					_ABReadManifest = abObj;
					//读取清单文件（到系统类实例中）
					_ManifestObj = _ABReadManifest.LoadAsset(NAME_AssetBundleManifest) as AssetBundleManifest;
					//本次加载与读取清单文件完毕。
					_IsLoadFinish = true;
				}
			}
		}


		/// <summary>
		/// 获取AssetBundleManifest系统类实例
		/// </summary>
		/// <returns></returns>
		public AssetBundleManifest GetABManifest(){
			//如果未加载完成，或者引发空引用
			if (!_IsLoadFinish || _ManifestObj == null)
				throw new Exception("得到AB清单文件失败！");

			return _ManifestObj;
		}


		/// <summary>
		/// 获取AssetBundleManifest（系统类）中所有的依赖项
		/// </summary>
		public string[] RetrieveDependencies(string abName){
			//空引用检查，以及空字符串检查
			if (_ManifestObj == null || string.IsNullOrEmpty(abName)) 
				throw new Exception("空引用异常，或者空字符串异常！");

			return _ManifestObj.GetAllDependencies(abName);
		}


		/// <summary>
		/// 释放本类的资源
		/// 一定要在最后调用
		/// </summary>
		public void Dispose(){
			if (_ABReadManifest == null)
				return;

			_ABReadManifest.Unload(true);
		}
	}
}