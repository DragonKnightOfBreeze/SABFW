/*******
 * ［标题］
 * 项目：AssetBundle框架设计
 * 作者：微风的龙骑士 风游迩
 * 
 * 删除所有AB包
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

namespace SABFW {
	/// <summary>
	/// 删除所有AB包
	/// </summary>
	public class DeleteAssetBundles  {


		/// <summary>
		/// 批量删除AB包文件
		/// </summary>
		[MenuItem("AssetBundleTools" + "/" + "DeleteAssetBundles",true,3)]
		public static void DeleteAllAB(){

			//得到删除AB包输出目录
			string strNeedDeleteDir = PathTools.GetABOutputPath();
			//空字符串检查
			if (string.IsNullOrEmpty(strNeedDeleteDir))
				return;

			//注意：这里参数true表示可以删除非空目录
			 Directory.Delete(strNeedDeleteDir,true);
			//去除删除警告
			File.Delete(strNeedDeleteDir+".meta");
			//刷新
			AssetDatabase.Refresh();
			
		}

	}
}