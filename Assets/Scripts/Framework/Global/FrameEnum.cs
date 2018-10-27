using System;

namespace MyABFramework.Global {
	/// <summary>框架枚举静态类</summary>
	public static class FrameEnum {
		/// <summary>枚举值转化为字符串</summary>
		public static string S(this Enum e) {
			return e.ToString();
		}

		/// <summary>枚举值转化为字符串（路径形式）</summary>
		public static string PS(this Enum e) {
			return e.ToString().Replace("__", "/");
		}
	}


//	/// <summary>Unity菜单标签结构</summary>
//	public enum MenuLabel {
//		AssetBundleTools__SetLabels,
//		AssetBundleTools__BuildAll,
//		AssetBundleTools__DeleteBundles
//	}


	/// <summary>场景类型</summary>
	/// <remarks>注意：请自行添加场景类型。</remarks>
	public enum SceneType {
		None,
		PleaseAddScenes
	}
}