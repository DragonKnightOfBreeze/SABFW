/*******
 * ［标题］
 * 项目：AssetBundle框架设计
 * 作者：微风的龙骑士 风游迩
 * 
 * 相关的游戏枚举
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
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

namespace Global {

	/// <summary>
	/// 场景类型
	/// TODO：请添加
	/// </summary>
	public enum SceneType {
		None,
		PleaseAddScenes
	}


	/// <summary>
	/// 游戏枚举管理类
	/// </summary>
	public static class GameEnum{
		/// <summary>
		/// 根据场景类型枚举，得到场景名称
		/// </summary>
		public static string Get(SceneType sceneType){
			return sceneType.ToString();
		}
	}

}