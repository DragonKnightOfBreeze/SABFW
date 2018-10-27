namespace MyABFramework.Global {
	/// <summary>框架常量静态类</summary>
	public static class FrameConst {
		
		/// <summary>AssetBundle清单的名称</summary>
		public const string NAME_Manifest = "AssetBundleManifest";

		
		/* Unity菜单标签 */

		public const string MENU_SetLabels = "AssetBundleTools/SetLabels";
		public const string MENU_BuildBundles = "AssetBundleTools/BuildAll";
		public const string MENU_DeleteBundles = "AssetBundleTools/DeleteAll";
		
		
		/* 消息、警告的前缀 */
		
		public const string PREFIX = "自用AssetBundle框架：\t";
		
		/* 消息 */
		
		public const string INFO_SetLabels = "已设置所有的资源标记。";
		public const string INFO_BuildBundles = "已构建所有的AssetBundle包。";
		public const string INFO_DeleteBundles = "已删除所有的AssetBundle包。";
	}
}