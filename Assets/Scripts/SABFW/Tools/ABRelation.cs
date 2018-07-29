/*******
 * ［标题］
 * 项目：AssetBundle框架设计
 * 作者：微风的龙骑士 风游迩
 * 
 * 辅助类：AB包关系类
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
using System;
using System.Collections.Generic;

namespace SABFW {
	/// <summary>
	/// AB包关系类
	/// </summary>
	public class ABRelation {

		/// <summary>当前AssetBundle名称</summary>
		public string ABName;
		/// <summary>所有依赖包的集合</summary>
		private List<string> _ABDependenceList;
		/// <summary>所有引用包的集合</summary>
		private List<string> _ABReferenceList;


		public ABRelation(string abName){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			//开始初始化
			ABName = abName;
			_ABDependenceList = new List<string>();
			_ABReferenceList = new List<string>();
		}



		#region ［依赖关系处理］

		/// <summary>
		/// 增加依赖关系
		/// </summary>
		public bool AddABDependence(string abName){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			if (_ABDependenceList.Contains(abName))
				return false;
			_ABDependenceList.Add(abName);
			return true;
		}


		/// <summary>
		/// 移除依赖关系
		/// </summary>
		public bool RemoveDependence(string abName){
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			if (!_ABDependenceList.Contains(abName))
				return false;
			_ABDependenceList.Remove(abName);
			return true;
		}


		/// <summary>
		/// 查询所有依赖关系
		/// </summary>
		/// <returns></returns>
		public List<string> RetrieveDependencies(){
			return _ABDependenceList;
		}


		/// <summary>
		/// 是否有依赖关系
		/// </summary>
		public bool HasDependence => _ABDependenceList.Count != 0;

		#endregion


		#region ［引用关系处理］

		/// <summary>
		/// 增加引用关系
		/// </summary>
		public bool AddABReference(string abName) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			if (_ABReferenceList.Contains(abName))
				return false;
			_ABReferenceList.Add(abName);
			return true;
		}


		/// <summary>
		/// 移除引用关系
		/// </summary>
		public bool RemoveReference(string abName) {
			if (string.IsNullOrEmpty(abName))
				throw new ArgumentException(nameof(abName));

			if (!_ABReferenceList.Contains(abName))
				return false;
			_ABReferenceList.Remove(abName);
			return true;
		}


		/// <summary>
		/// 查询所有引用关系关系
		/// </summary>
		/// <returns></returns>
		public List<string> RetrieveReferences(){
			return _ABReferenceList;
		}


		/// <summary>
		/// 是否有引用关系
		/// </summary>
		public bool HasReference => _ABReferenceList.Count != 0;

		#endregion
	}
}