/**
 * ［概述］
 * 给资源文件添加标签
 * 
 * 1.定义需要打包资源的文件夹根目录
 * 2.遍历每个“场景”文件夹
 *		A.遍历本场景目录下所有的目录或者文件。
 *			如果是目录，则继续“递归”访问里面的内容，直到定位到文件。
 *			如果找到文件，则使用AssetImporter类，标记“包名”。

 * ［用法］
 * 
 * 
 * ［备注］ 
 * AssetImporter.GetAtPath(path)，其中的path：相对Unity项目的路径，一般以"Asset"开始
 * Windows的路径间隔符为"\"，Unity的则为"/"。
 * bundleName示例：Scene/Texture.ab
 * 
 * 项目：自用AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 */

using System;
using System.IO;
using MyABFramework.Global;
using UnityEngine;
using UnityEditor;

namespace MyABFramework.Editor {
	/// <summary>给资源文件添加标签</summary>
	public static class Edt_SetLabels {
		
		/// <summary>给资源文件添加标签</summary>
		[MenuItem(FrameConst.MENU_SetLabels, true, 1)]
		public static void SetLabels() {
			//清空无用的AssetBundle包名
			AssetDatabase.RemoveUnusedAssetBundleNames();
			//得到资源文件的根目录信息
			DirectoryInfo rootDirInfo = new DirectoryInfo(PathTools.GetInputPath());
			//得到根目录下的所有场景目录信息
			DirectoryInfo[] sceneDirInfos = rootDirInfo.GetDirectories();
			//遍历每个场景目录下的所有文件，进行适当的处理
			foreach(var sceneDirInfo in sceneDirInfos)
				HandleAllFiles(sceneDirInfo, sceneDirInfo.Name);
			//刷新
			AssetDatabase.Refresh();
			//打印提示消息
			Debug.Log(FrameConst.PREFIX + FrameConst.INFO_SetLabels);
		}


		/// <summary>遍历指定目录下的所有文件</summary>
		/// <remarks>如果是目录，则递归；如果是.meta文件，则跳过；否则，修改AssetBundle的标签。</remarks>
		/// <param name="dirInfo">指定的目录信息</param>
		/// <param name="sceneName">场景名称</param>
		private static void HandleAllFiles(DirectoryInfo dirInfo, string sceneName) {
			FileInfo[] fileInfos = dirInfo.GetFiles("*", SearchOption.AllDirectories);
			foreach(var info in fileInfos) {
				//跳过为.meta文件的情况
				if(info.Extension == ".meta")
					continue;
				//修改此文件的AssetBundle标签
				SetFileLabel(info, sceneName);
			}
		}


		/// <summary>为指定文件设置标签</summary>
		/// <param name="fileInfo">指定的文件信息</param>
		/// <param name="sceneName">指定的场景名称</param>
		private static void SetFileLabel(FileInfo fileInfo, string sceneName) {
			//得到相对于Unity项目的文件路径，一般以"Assets"开始
			string unityFilePath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)).Replace("\\","/");
			//导入资源文件
			AssetImporter assetImp = AssetImporter.GetAtPath(unityFilePath);
			//设置包名
			assetImp.assetBundleName = GetBundleName(unityFilePath, sceneName);
			//设置扩展名
			assetImp.assetBundleVariant= GetBundleVariant(fileInfo);
		}


		/// <summary>得到AssetBundle包的名称</summary>
		/// <remarks>如果在根目录下，名称 = 场景名称 + 场景名称。</remarks>
		/// <remarks>如果在三级目录下，名称 = 二级目录名称（场景名称）+ 三级目录名称（类型名称）。</remarks>
		/// <param name="unityFilePath">指相对于Unity项目的指定文件路径</param>
		/// <param name="sceneName">指定的场景名称</param>
		private static string GetBundleName(string unityFilePath, string sceneName) {
			//示例：
			//Unity路径：Assets/Scene/Texture/image.jpg，包名：Scene/Texture.ab
			//Unity路径：Assets/Scene/image.jpg，包名：Scene/Scene.u3d
			
			//得到场景目录下的文件相对路径
			string tempFilePath = unityFilePath.Replace($"Asset/{sceneName}/", "");
			string bundleName;
			if(tempFilePath.Contains("/"))
				bundleName = sceneName + "/" + tempFilePath.Split('/')[0];
			else
				bundleName = sceneName + "/" + sceneName;
			return bundleName;
		}


		/// <summary>得到AssetBundle包的扩展名</summary>
		/// <param name="fileInfo">指定的文件信息</param>
		private static string GetBundleVariant(FileInfo fileInfo) {
			string variant;
			if(fileInfo.Extension == ".unity")
				variant = "u3d";
			else
				variant = "ab";
			return variant;
		}
	}
}