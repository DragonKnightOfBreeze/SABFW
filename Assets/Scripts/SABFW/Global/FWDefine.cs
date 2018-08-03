/*******
 * ［标题］
 * 项目：简单的AssetBundle框架
 * 作者：微风的龙骑士 风游迩
 * 
 * 框架全局定义类
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

namespace SABFW {
	/// <summary>
	/// 框架全局定义类
	/// </summary>                          
	public static class FWDefine {

		#region ［常量］

		/// <summary>AB清单的名称</summary>
		public const string NAME_ABManifest = "AssetBundleManifest";

		/// <summary>消息前缀</summary>
		public const string PREFIX = "SABFW框架：";

		public const string INFO_SetLabels = "已设置所有资源的标记。";
		public const string INFO_BuildAssetBundles = "已构建所有的AssetBundle包。";
		public const string INFO_DeleteAssetBundles = "已删除所有的AssetBundle包";

		#endregion

	}


	#region ［委托］

	/// <summary>委托：加载完成</summary>
	public delegate void DelLoadComplete(string name);

	#endregion


}