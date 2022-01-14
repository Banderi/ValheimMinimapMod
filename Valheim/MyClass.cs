/*
 * Created by SharpDevelop.
 * User: Banderi
 * Date: 1/14/2022
 * Time: 12:40 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ValheimMod
{
//	public class NameOf
//    {
//        public static String nameof<T>(Expression<Func<T>> name)
//        {
//            MemberExpression expressionBody = (MemberExpression)name.Body;
//            return expressionBody.Member.Name;
//        }
//    }
	
	[BepInPlugin("Banderi.MinimapMod", "Rotating Minimap", "1.0.0")]
	[BepInProcess("valheim.exe")]
	
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public class ValheimMod_RotatingMinimap : BaseUnityPlugin
	{
		public static BepInEx.Logging.ManualLogSource logger;
        
		private readonly Harmony harmony = new Harmony("Banderi.MinimapMod");
		void Awake()
		{
			logger = Logger;
			logger.LogWarning("MOD: Awaking...");
			harmony.PatchAll();
		}
		
		// [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
		[HarmonyPatch(typeof(Character), "Jump")]
		class Jump_Patch
		{
			static void Prefix(ref float ___m_jumpForce)
			{
				logger.LogWarning(String.Format("Jump force: {0}",___m_jumpForce));
				___m_jumpForce = 1;
				logger.LogWarning(String.Format("New jump force: {0}",___m_jumpForce));
			}
		}
	}
}