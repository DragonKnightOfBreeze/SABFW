/*******
 * ［标题］
 * 项目：AssetBundle框架设计
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
using UnityEditor;      //引入Unity编辑器命名空间

namespace SABFW {
	/// <summary>
	/// 打包资源且输出路径
	/// </summary>
	public class BuildAssetBundles {

		/// <summary>
		/// 打包生成所有的AssetBundle包
		/// （特性：在菜单中增加一个标签卡）
		/// </summary>
		[MenuItem("AssetBundleTools"+"/"+"BuildAssetBundles",true,2)]
		public static void BuildAllAB() {
			//获取打包AB输出路径
			var abOutPathDir = PathTools.GetABOutputPath();
			//如果不存在StreamingAssets目录，则自动生成
			if (!Directory.Exists(abOutPathDir))
				Directory.CreateDirectory(abOutPathDir);
			//打包生成
			BuildPipeline.BuildAssetBundles(abOutPathDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
		}

	}
}