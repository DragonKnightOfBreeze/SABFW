/**
 * ［概述］
 * 打包生成所有的AssetBundle包
 * 
 * ［用法］
 *
 * 
 * ［备注］ 
 *
 * 
 * 项目：自用AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 */ 

using System.IO;
using MyABFramework.Global;
using UnityEditor;
using UnityEngine;

namespace MyABFramework.Editor {
	/// <summary>打包生成所有的AssetBundle包</summary>
	public static class Edt_BuildBundles {
		
		/// <summary>打包生成所有的AssetBundle包</summary>
		/// <remarks>备注：Unity菜单中会增加一个标签卡。</remarks>
		[MenuItem(FrameConst.MENU_BuildBundles, true, 2)]
		public static void BuildBundles() {
			//获取AssetBundle包的输出路径
			string outputPath = PathTools.GetOutputPath();
			//如果不存在StreamingAssets目录，则自动创建
			if(!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);

			//打包生成AssetBundle包
			BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
			//打印提示消息
			Debug.Log(FrameConst.PREFIX + FrameConst.INFO_BuildBundles);
		}
	}
}