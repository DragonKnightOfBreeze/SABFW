/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 框架整体测试类
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
using System.Collections.Generic;
using UnityEngine;

namespace MyABFramework {
	/// <summary>测试：ABManager</summary>
	public class BundleManager_Test : MonoBehaviour {
		//场景名称
		private readonly string _SceneName = "???";
		//AB包名称
		private readonly string _ABName1 = "???";
		//资源名称
		private readonly string _AssetName = "???";


		private void Start() {
			//调用AB包（连锁智能调用AB包集合）
			StartCoroutine(AssetBundleMgr.Instance.LoadAssetBundle(_SceneName, _ABName1,null,LoadAllABComplete));
		}

		/// <summary>回调函数：所有的AB包都已经加载完毕了</summary>
		/// <param name="abName"></param>
		private void LoadAllABComplete() {
			UnityEngine.Object tempObj = null;

			//提取资源
			tempObj = AssetBundleMgr.Instance.LoadAsset(_SceneName, _ABName1, _AssetName);
			//克隆资源
			if(tempObj != null)
				Instantiate(tempObj);
		}

		/// <summary>测试销毁资源</summary>
		private void Update() {
			if(Input.GetKeyDown(KeyCode.A))
				AssetBundleMgr.Instance.DisposeAllInScene(_SceneName);
		}
	}
}