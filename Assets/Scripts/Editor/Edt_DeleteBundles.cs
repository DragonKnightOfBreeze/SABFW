/**
 * ［概述］
 * 删除所有的AssetBundle包
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

using System;
using System.IO;
using MyABFramework.Global;
using UnityEditor;
using UnityEngine;

namespace MyABFramework.Editor {
	/// <summary>删除所有的AssetBundle包</summary>
	public static class Edt_DeleteBundles {
		/// <summary>删除所有的AssetBundle包</summary>
		[MenuItem(FrameConst.MENU_DeleteBundles, true, 3)]
		public static void DeleteBundles() {
			//得到AssetBundle包输出目录
			string inputPath = PathTools.GetOutputPath();

			//注意：这里参数true表示可以删除非空目录
			Directory.Delete(inputPath, true);
			//去除删除警告
			File.Delete(inputPath + ".meta");

			//刷新
			AssetDatabase.Refresh();
			//打印提示消息
			Debug.Log(FrameConst.PREFIX + FrameConst.INFO_DeleteBundles);
		}
	}
}