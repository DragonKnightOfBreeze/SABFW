/*******
 * ［标题］
 * 项目：AssetBundle框架设计
 * 作者：微风的龙骑士 风游迩
 * 
 * 主流程（最上层）：所有场景的AB包管理
 * 
 * ［功能］
 * 1.读取清单文件，缓存本脚本
 * 2.以场景为单位，管理整个项目中的所有AB包
 * 
 * ［思路］
 *  
 * 
 * ［用法］
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 所有场景的AB包管理（入口）
	/// </summary>
	public class AssetBundleMgr : MonoBehaviour {

		///<summary>本类实例</summary>
		private static AssetBundleMgr _Instance;

		/// <summary>得到本类实例</summary>
		public static AssetBundleMgr GetInstance(){
			if(_Instance == null)
				_Instance = new GameObject(NAME_GO).AddComponent<AssetBundleMgr>();
			return _Instance;
		}


		private const string NAME_GO = "_AssetBundleMgr";

		/// <summary>场景集合</summary>
		private Dictionary<string, MultiABMgr> _DicAllScenes = new Dictionary<string, MultiABMgr>();
		///<summary>清单文件的系统类</summary>
		private AssetBundleManifest _ManifestObj = null;




		void Awake(){
			//加载Manifest清单文件
			StartCoroutine(ABManifestLoader.GetInstance().LoadMainfesetFile());
		}






		/// <summary>
		/// 下载指定的AB包
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="abName"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public IEnumerator LoadAssetBundlePack(string sceneName, string abName, DelLoadComplete handler){
			//参数检查
			if(string.IsNullOrEmpty(sceneName))
				throw new ArgumentException(nameof(sceneName));
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			//等待清单文件加载完成
			while (!ABManifestLoader.GetInstance().IsLoadFinish) 
				yield return null;
			_ManifestObj = ABManifestLoader.GetInstance().GetABManifest();

			//把当前场景加入集合中
			if (!_DicAllScenes.ContainsKey(sceneName)) {
				MultiABMgr multiABObj = new MultiABMgr(sceneName,abName,handler);
				_DicAllScenes.Add(sceneName, multiABObj);
			}

			//调用下一层（多包管理类）
			MultiABMgr tempMultiABObj = _DicAllScenes[sceneName];
			//加载指定的AB包
			yield return tempMultiABObj.LoadAssetBundle(abName);
		}

		/// <summary>
		/// 加载AB包中的资源
		/// </summary>
		/// <param name="sceneName">场景名称</param>
		/// <param name="abName">AB包名称</param>
		/// <param name="assetName">资源名称</param>
		/// <param name="isCache">是否使用缓存</param>
		/// <returns></returns>
		public UnityEngine.Object LoadAsset(string sceneName, string abName,string assetName,bool isCache = false){
			//TODO
			if (!_DicAllScenes.ContainsKey(sceneName)) {
				 Debug.LogError("找不到场景名称！"+"\t"+sceneName);
				return null;
			}

			MultiABMgr multiABObj = _DicAllScenes[sceneName];
			return multiABObj.LoadAsset(abName, assetName, isCache);
		}

		/// <summary>
		/// 释放场景中的所有资源
		/// </summary>
		public void DisposeAllAssets(string sceneName){
			if (!_DicAllScenes.ContainsKey(sceneName)) {
				Debug.LogError("找不到场景名称！" + "\t" + sceneName);
				return;
			}

			MultiABMgr multiABObj = _DicAllScenes[sceneName];
			multiABObj.DisposeAllAssets();
		}


	}
}