/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 框架内部测试类
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

using UnityEngine;

namespace MyABFramework {
	/// <summary>测试类：测试SingleABLoader</summary>
	public class SingleBundleLoader_Test : MonoBehaviour {
		//引用类
		private SingleBundleLoader _LoadObj;

		//AB包名称
		private readonly string _ABName = "Scene_1/prefabs.ab";
		//AB包中资源名称
		private readonly string _AssetName1 = "Plane.prefab";

		//依赖Ab包名名称
		private readonly string _ABDependName1 = "Scene_1/textures.ab";
		private string _ABDependName2 = "Scene_1/textures.ab";
		//有依赖的AB包中资源名称
		private string _AssetName2 = "TestCUbePrefab.prefab";


		#region ［测试：简单无依赖包的加载］

		//void Start() {
		//	_LoadObj = new SingleBundleLoader(_ABName,LoadComplete);
		//	//加载AB包
		//	StartCoroutine(_LoadObj.LoadAssetBundle());
		//}


		///// <summary>
		///// 回调函数（一定条件下自动执行）
		///// </summary>
		///// <param name="abName"></param>
		//private void LoadComplete(string abName){
		//	//加载AB包中资源
		//	UnityEngine.Object tempObj =  _LoadObj.LoadAsset(_AssetName1,false);
		//	//克隆对象
		//	Instantiate(tempObj);
		//}

		#endregion


		#region［有依赖包加载］

		private void Start() {
			SingleBundleLoader _LoadDependObj1 = new SingleBundleLoader(_ABDependName1, LoadDependComplete1);
			//加载AB依赖包
			StartCoroutine(_LoadDependObj1.LoadAssetBundle());
		}


		/// <summary>依赖回调函数1</summary>
		private void LoadDependComplete1(string abName) {
			_LoadObj = new SingleBundleLoader(_ABName, LoadComplete);
			//加载AB依赖包
			StartCoroutine(_LoadObj.LoadAssetBundle());
		}

		/// <summary>依赖回调函数2</summary>
		/// <param name="abName"></param>
		private void LoadComplete(string abName) {
			//开始正式加载预设包
			//加载AB包中资源
			Object tempObj = _LoadObj.LoadAsset(_AssetName1, false);
			//克隆对象
			Instantiate(tempObj);
		}

		private void Update() {
			if(Input.GetKeyDown(KeyCode.A)) {
				//Debug.Log("释放镜像内存资源");
				//_LoadObj.Dispose();		
				Debug.Log("释放镜像内存资源，以及内存资源");
				_LoadObj.DisposeAll();
			}
		}

		#endregion
	}
}