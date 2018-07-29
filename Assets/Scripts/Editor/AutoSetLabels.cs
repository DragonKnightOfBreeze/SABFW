/*******
 * ［标题］
 * 项目：AssetBundle框架设计
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
	public class AutoSetLabels {

		/// <summary>
		/// 设置AB包名
		/// </summary>
		[MenuItem("AssetBundleTools"+ "/" + "SetLabels",true,1)]
		public static void SetABLabel(){
			/*定义局部变量*/
			//需要给AB做标记的根目录
			string strNeedSetLabelRoot = string.Empty;
			//目录信息（场景目录信息数组，表示所有根目录下场景目录）
			DirectoryInfo[] dirSceneDirArray = null;

			//清空无用AB标记
			AssetDatabase.RemoveUnusedAssetBundleNames();

			//定义需要打包资源的文件夹根目录
			strNeedSetLabelRoot = PathTools.GetABInputPath();
			DirectoryInfo dirTempInfo = new DirectoryInfo(strNeedSetLabelRoot);
			//得到根目录下所有的目录的信息
			dirSceneDirArray = dirTempInfo.GetDirectories();

			//遍历每个“场景”文件夹
			foreach (var curDir in dirSceneDirArray) {
				//得到对于根目录的绝对路径
				string tempSceneDir = strNeedSetLabelRoot + "/" + curDir.Name;
				//得到临时场景名称
				int tempIndex = tempSceneDir.LastIndexOf('/');
				string tempSceneName = tempSceneDir.Substring(tempIndex + 1);
				//递归调用方法，如果找到文件，则使用AssetImport类，标记“包名”
				CheckDirOrFileByRecursive(curDir, tempSceneName);
			}
			//刷新
			AssetDatabase.Refresh();
			//提示信息，标记完成
			Debug.Log("SABFW：设置标记完成");
		}

		/// <summary>
		/// 递归判断是否为目录和文件
		/// 如果是目录，则递归；否则修改AssetBundle的标记（Label）
		/// </summary>
		/// <param name="fileSysInfo">当前的文件信息</param>
		/// <param name="curSceneName">当前的场景名称</param>
		private static void CheckDirOrFileByRecursive(FileSystemInfo fileSysInfo, string curSceneName){
			//参数检查
			if (!fileSysInfo.Exists)
				throw new FileNotFoundException("文件或目录不存在！"+ "\t" + nameof(fileSysInfo));

			//得到当前目录下一级的文件信息集合
			//文件信息转化为目录信息
			DirectoryInfo dirInfoObj = fileSysInfo as DirectoryInfo;
			//空引用检查
			if (dirInfoObj == null)
				return;

			FileSystemInfo[] fileSysArray = dirInfoObj.GetFileSystemInfos();
			foreach (var info in fileSysArray) {
				FileInfo fileInfoObj = info as FileInfo;
				//如果是文件类型
				if (fileInfoObj != null) {
					//参数检查：排除是.meta文件的情况
					if (fileInfoObj.Extension == ".meta")
						continue;
					//修改此文件的AssetBundle标签
					SetFileABLabel(fileInfoObj, curSceneName);
				}
				//如果是目录类型
				else {
					//递归调用此方法
					CheckDirOrFileByRecursive(info, curSceneName);
				}
			}
		}

		/// <summary>
		/// 对指定的文件，设置AB包名称
		/// </summary>
		/// <param name="fileInfo">指定的文件信息（绝对路径）</param>
		/// <param name="sceneName">指定的场景名称</param>
		private static void SetFileABLabel(FileInfo fileInfo, string sceneName){
			/*定义局部变量*/
			//AB包路径
			string strABName = string.Empty;
			//文件路径（相对路径）
			string strAssetFilePath = string.Empty;

			//得到AB包的名称
			strABName = GetABName(fileInfo, sceneName);
			//得到资源文件的相对路径
			//将“Assets”以及后面的字符串全部截取下来
			int tempIndex = fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal);
			strAssetFilePath = fileInfo.FullName.Substring(tempIndex);

			//给资源文件设置AB包的名称
			AssetImporter tempImporterObj = AssetImporter.GetAtPath(strAssetFilePath);
			if (tempImporterObj == null)
				return;
			tempImporterObj.assetBundleName = strABName;

			//定义AB包的扩展名
			if (fileInfo.Extension == ".unity") {
				tempImporterObj.assetBundleVariant = "u3d";
			}
			else {
				tempImporterObj.assetBundleVariant = "ab";

			}
		}

		/// <summary>
		/// 得到AB包的名称
		///
		/// AB包形成规则：
		///		文件AB包名称= 所在二级目录名称（场景名称）+三级目录名称（类型名称）
		/// </summary>
		/// <param name="fileInfo">指定的文件信息</param>
		/// <param name="sceneName">指定的场景名称</param>
		private static string GetABName(FileInfo fileInfo, string sceneName){
			//测试
			//Debug.Log(fileInfo.FullName);

			//要返回的AB包的名称
			string strABName = string.Empty;

			//WIN路径
			string tempWinPath = fileInfo.FullName;
			//Unity路径
			string tempUnityPath = tempWinPath.Replace("\\", "/");

			//定位“场景名称”后面字符位置
			int tempSceneNameIndex = tempUnityPath.IndexOf(sceneName, StringComparison.Ordinal) + sceneName.Length;
			//LastIndexOf()???
			string strABFileNameArea = tempUnityPath.Substring(tempSceneNameIndex + 1);

			if (strABFileNameArea.Contains("/")) {
				//确定AB包的名字：场景名+三级目录名
				strABName = sceneName + "/" + strABFileNameArea.Split('/')[0];
			}
			else {
				//对于场景文件
				strABName = sceneName + "/" + sceneName;
			}
			return strABName;
		}



		///// <summary>
		///// 设置AB包的扩展名
		///// </summary>
		///// <param name="fileInfo"></param>
		///// <param name="aImporter"></param>
		//private static void SetABVariant(FileInfo fileInfo, AssetImporter aImporter){ }
	}
}