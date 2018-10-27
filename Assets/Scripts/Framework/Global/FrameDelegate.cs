namespace MyABFramework.Global {
	/// <summary>框架委托静态类</summary>
	public static class FrameDelegate { }

	/// <summary>委托：包加载完成</summary>
	public delegate void LoadCompleteDelegate(string bundleName);

	/// <summary>委托：场景中的全部包加载完成</summary>
	public delegate void LoadAllCompleteDelegate();
}