/**
 * ［概述］
 * AssetBundle包关系类
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
using System.Collections.Generic;
using MyABFramework.Global;

namespace MyABFramework {
	/// <summary>AssetBundle包关系类</summary>
	public class BundleRelation {
		
		#region ［字段］

		/// <summary>当前AssetBundle包的名称</summary>
		private string bundleName;
		/// <summary>所有依赖包的列表</summary>
		private readonly List<string> abDependenceList;
		/// <summary>所有引用包的列表</summary>
		private readonly List<string> abReferenceList;

		#endregion


		#region ［构造器］

		public BundleRelation(string name) {
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(name));
			
			bundleName = name;
			abDependenceList = new List<string>();
			abReferenceList = new List<string>();
		}

		#endregion


		#region ［依赖关系处理］
		
		/// <summary>是否有依赖关系</summary>
		public bool HasDependence() {
			return abDependenceList.Count != 0;
		}


		/// <summary>增加依赖关系</summary>
		public bool AddABDependence(string name) {
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(name));

			if(abDependenceList.Contains(name))
				return false;
			abDependenceList.Add(name);
			return true;
		}


		/// <summary>移除依赖关系</summary>
		public bool RemoveDependence(string name) {
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(name));

			if(!abDependenceList.Contains(name))
				return false;
			abDependenceList.Remove(name);
			return true;
		}


		/// <summary>查询所有依赖关系</summary>
		public List<string> RetrieveDependencies() {
			return abDependenceList;
		}

		#endregion

		
		#region ［引用关系处理］
		
		/// <summary>是否有依赖关系</summary>
		public bool HasReference() {
			return abReferenceList.Count != 0;
		}
		

		/// <summary>增加引用关系</summary>
		public bool AddReference(string name) {
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(name));

			if(abReferenceList.Contains(name))
				return false;
			abReferenceList.Add(name);
			return true;
		}


		/// <summary>移除引用关系</summary>
		public bool RemoveReference(string name) {
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException(FrameConst.PREFIX + "非法的字符串！", nameof(name));

			if(!abReferenceList.Contains(name))
				return false;
			abReferenceList.Remove(name);
			return true;
		}


		/// <summary>查询所有引用关系关系</summary>
		/// <returns></returns>
		public List<string> RetrieveReferences() {
			return abReferenceList;
		}

		#endregion
	}
}