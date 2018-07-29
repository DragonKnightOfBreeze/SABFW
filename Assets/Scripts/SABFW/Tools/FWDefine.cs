/*******
 * ［标题］
 * 项目：AssetBundle框架设计
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
		public const string NAME_AssetBundleManifest = "AssetBundleManifest";

		#endregion

	}


	#region ［委托］

	/// <summary>委托：加载完成</summary>
	public delegate void DelLoadComplete(string name);

	#endregion


}