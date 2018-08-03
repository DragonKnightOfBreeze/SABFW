/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 自动给资源文件添加标记
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 1.定义需要打包资源的文件夹根目录
 * 2.遍历每个“场景”文件夹
 *		A.遍历本场景目录下所有的目录或者文件。
 *			如果是目录，则继续“递归”访问里面的内容，知道定位到文件。
 *			如果找到文件，则使用AssetImporter类，标记“包名”
 * 
 * ［用法］
 * 
 */
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace SABFW {
	/// <summary>
	/// 自动给资源文件添加标记
	/// </summary>
	public class Edi_SetLabels {

		/// <summary>
		/// 设置AB包名
		/// </summary>
		[MenuItem("AssetBundleTools"+ "/" + "SetLabels",true,1)]
		public static void SetLabels(){
			//清空无用AB标记
			AssetDatabase.RemoveUnusedAssetBundleNames();

			//得到AB资源的根目录信息
			string rootPath = PathTools.GetABInputPath();
			DirectoryInfo rootDirInfo = new DirectoryInfo(rootPath);

			//得到根目录下的所有目录信息（场景目录）
			DirectoryInfo[] sceneDirInfoArray = rootDirInfo.GetDirectories();
			//遍历每个场景目录信息
			foreach (var sceneDirInfo in sceneDirInfoArray) {
				//递归调用方法，如果找到文件，则使用AssetImport类，标记“包名”
				CheckDirOrFileByRecursive(sceneDirInfo, sceneDirInfo.Name);
			}
			//刷新
			AssetDatabase.Refresh();
			//打印提示消息
			Debug.Log(FWDefine.PREFIX+FWDefine.INFO_SetLabels);
		}


		/// <summary>
		/// 递归判断是否为目录和文件
		/// 如果是目录，则递归；否则修改AssetBundle的标记（Label）
		/// </summary>
		/// <param name="fileSysInfo">指定的文件信息</param>
		/// <param name="sceneName">指定的场景名称</param>
		private static void CheckDirOrFileByRecursive(FileSystemInfo fileSysInfo, string sceneName){
			//得到当前目录下一级的文件信息集合
			//文件信息转化为目录信息
			DirectoryInfo dirInfo = fileSysInfo as DirectoryInfo;
			//空引用检查（不是目录则直接返回）
			if (dirInfo == null)
				return;
			FileSystemInfo[] subFileSysInfoArray = dirInfo.GetFileSystemInfos();
			foreach (var subFileSysInfo in subFileSysInfoArray) {
				FileInfo subFileInfo = subFileSysInfo as FileInfo;
				//如果是文件类型
				if (subFileInfo != null) {
					//跳过是.meta文件的情况
					if (subFileInfo.Extension == ".meta")
						continue;

					//修改此文件的AssetBundle标签
					SetFileLabel(subFileInfo, sceneName);
				}
				//如果是目录类型
				else {
					//递归调用此方法
					CheckDirOrFileByRecursive(subFileSysInfo, sceneName);
				}
			}
		}


		/// <summary>
		/// 对指定的文件，设置AB包名称
		/// </summary>
		/// <param name="fileInfo">指定的文件信息（绝对路径）</param>
		/// <param name="sceneName">指定的场景名称</param>
		private static void SetFileLabel(FileInfo fileInfo, string sceneName){
			//得到AB包的名称
			string packageName = GetPackageName(fileInfo, sceneName);
			//得到以Asset文件夹作为根目录的资源文件的相对路径
			int tempIndex = fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal);
			string assetFilePath = fileInfo.FullName.Substring(tempIndex);

			//给资源文件设置AB包的名称
			AssetImporter assetImp = AssetImporter.GetAtPath(assetFilePath);
			assetImp.assetBundleName = packageName;
			//定义AB包的扩展名
			SetPackageVariant(fileInfo,assetImp);
		}


		/// <summary>
		/// 得到AB包的名称
		/// 文件AB包名称= 所在二级目录名称（场景名称）+三级目录名称（类型名称）
		/// </summary>
		/// <param name="fileInfo">指定的文件信息</param>
		/// <param name="sceneName">指定的场景名称</param>
		private static string GetPackageName(FileInfo fileInfo, string sceneName){
			//要返回的AB包的名称
			string packageName;

			//WIN路径
			string winPath = fileInfo.FullName;
			//Unity路径
			string unityPath = winPath.Replace("\\", "/");
			//得到将场景目录作为根目录的文件相对路径
			int sceneNameIndex = unityPath.IndexOf(sceneName, StringComparison.Ordinal) + sceneName.Length;
			string fileNameArea = unityPath.Substring(sceneNameIndex + 1);
			//如果在子目录下（场景名+三级目录名）
			if (fileNameArea.Contains("/")) {
				packageName = sceneName + "/" + fileNameArea.Split('/')[0];
			}
			//如果在根目录下（对于场景文件，场景名+场景名）
			else {
				packageName = sceneName + "/" + sceneName;
			}
			return packageName;
		}


		/// <summary>
		/// 设置AB包的扩展名
		/// </summary>
		/// <param name="fileInfo">指定的文件信息</param>
		/// <param name="assetImp">对应的资源导入器</param>
		private static void SetPackageVariant(FileInfo fileInfo, AssetImporter assetImp){
			switch (fileInfo.Extension) {
				case ".unity":
					assetImp.assetBundleVariant = "u3d";
					break;
				default:
					assetImp.assetBundleVariant = "ab";
					break;
			}
		}
	}
}