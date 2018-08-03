/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
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

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SABFW {
	/// <summary>
	/// 删除所有AB包
	/// </summary>
	public class Edi_DeleteAssetBundles  {
		/// <summary>
		/// 批量删除AB包文件
		/// </summary>
		[MenuItem("AssetBundleTools" + "/" + "DeleteAssetBundles",true,3)]
		public static void DeleteAssetBundles(){
			//得到删除AB包输出目录
			string abOutputPath = PathTools.GetABOutputPath();

			//注意：这里参数true表示可以删除非空目录
			Directory.Delete(abOutputPath,true);
			//去除删除警告
			File.Delete(abOutputPath+".meta");

			//刷新
			AssetDatabase.Refresh();
			//打印提示消息
			Debug.Log(FWDefine.PREFIX+FWDefine.INFO_DeleteAssetBundles);
		}
	}
}