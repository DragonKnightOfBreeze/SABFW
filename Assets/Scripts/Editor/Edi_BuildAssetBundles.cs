/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 打包资源且输出路径
 * 
 * ［标题］
 * 
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

using System.IO;
using UnityEditor;
using UnityEngine;

//引入Unity编辑器命名空间

namespace SABFW {
	/// <summary>
	/// 打包资源且输出路径
	/// </summary>
	public class Edi_BuildAssetBundles {

		/// <summary>
		/// 打包生成所有的AssetBundle包
		/// （特性：在菜单中增加一个标签卡）
		/// </summary>
		[MenuItem("AssetBundleTools"+"/"+"BuildAssetBundles",true,2)]
		public static void BuildAssetBundles() {
			//获取打包AB输出路径
			string abOutputPath = PathTools.GetABOutputPath();
			//如果不存在StreamingAssets目录，则自动生成
			if (!Directory.Exists(abOutputPath))
				Directory.CreateDirectory(abOutputPath);

			//打包生成
			BuildPipeline.BuildAssetBundles(abOutputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

			//打印提示消息
			Debug.Log(FWDefine.PREFIX+FWDefine.INFO_BuildAssetBundles);
		}
	}
}