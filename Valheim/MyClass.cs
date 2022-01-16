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
		
		//
		
		static private bool initialized = false;
		static GameObject newPinsParent;
		static Quaternion lastPlayerRot;
		static Vector2 origin;
		static Vector2 playerMinimapPinPos;
		
		static void reparentPins(GameObject map) {
			foreach (Transform child in map.transform) {
				child.gameObject.transform.SetParent(newPinsParent.transform);
			}
		}
		static void init(GameObject map) {
			if (initialized)
				return;
			else {
				logger.LogWarning("MOD: Initializing...");
				
				origin = new Vector2(0.0f, 0.0f);
				newPinsParent = new GameObject();
				newPinsParent.transform.position = map.transform.position;
				newPinsParent.transform.SetParent(map.transform.parent);
				
//				reparentPins(map);
				
				initialized = true;
				logger.LogWarning("MOD: ...Done!");
			}
		}
		
		[HarmonyPatch(typeof(Minimap), "ClearPins")]
		class PinsUpdate2 { static bool Prefix() { logger.LogWarning("Clearing pins!"); return true; }}
		[HarmonyPatch(typeof(Minimap), "ForceRegen")]
		class PinsUpdate3 { static bool Prefix() { logger.LogWarning("Forcing regen!"); return true; }}
		
		//
		[HarmonyPatch(typeof(Minimap), "AddPin")]
		class PinsUpdate1
		{
			static void Postfix(ref GameObject ___m_mapSmall, Vector3 pos, Minimap.PinType type, string name)
			{
				init(___m_mapSmall);
				reparentPins(___m_mapSmall);
				logger.LogWarning(String.Format("Adding pin! {0} {1} {2}", pos, name, type));
			}
		}
		[HarmonyPatch(typeof(Minimap), "UpdatePins")]
		class MinimapRotation
		{
			static void Postfix(ref GameObject ___m_mapSmall, ref GameObject ___m_mapLarge, ref UnityEngine.RectTransform ___m_smallMarker)
			{
				___m_mapSmall.transform.rotation = Quaternion.Euler(0, 0, lastPlayerRot.eulerAngles.y);
				return;
				logger.LogWarning(String.Format("A: {0} {1} {2} {3} {4}",
				                                ___m_mapSmall.transform.position,
				                                newPinsParent.transform.position,
//				                                ___m_largeMarker.rotation.eulerAngles,
				                                0,
				                                ___m_smallMarker.position,
				                                ___m_smallMarker.localPosition));
			}
		}
		[HarmonyPatch(typeof(Minimap), "WorldToMapPoint")]
		class MinimapCenterPos
		{
			static private void Postfix(Vector3 p, float mx, float my)
			{
				Player localPlayer = Player.m_localPlayer;
				Vector3 position = localPlayer.transform.position;
				if (p == position) { // this is the local player pin's RELATIVE (UV) position used to calculating the anchor points later
					playerMinimapPinPos.x = mx;
					playerMinimapPinPos.y = my;
				}
			}
		}
		[HarmonyPatch(typeof(Minimap), "UpdatePlayerMarker")]
		class MinimapCenterRot
		{
			static private void Postfix(Quaternion playerRot, ref UnityEngine.RectTransform ___m_smallMarker)
			{
				lastPlayerRot = playerRot;
				___m_smallMarker.transform.rotation = Quaternion.Euler(0, 0, 0);
			}
		}
		[HarmonyPatch(typeof(Minimap), "MapPointToLocalGuiPos")]
		class MinimapIcons
		{
			static private void Prefix(Minimap.MapMode ___m_mode, ref float mx, ref float my, UnityEngine.UI.RawImage img)
			{
				if (___m_mode != Minimap.MapMode.Large) {
					
					Vector2 A = new Vector2(mx, my);
					Vector2 a = A - playerMinimapPinPos;
					Vector2 b = Quaternion.Euler(0, 0, lastPlayerRot.eulerAngles.y) * a;
					Vector2 B = playerMinimapPinPos + b;
					
					mx = B.x;
					my = B.y;
				}
			}
		}
		[HarmonyPatch(typeof(Minimap), "UpdateWindMarker")]
		class MinimapWind
		{
			static private void Postfix(ref RectTransform ___m_windMarker)
			{
				Quaternion windRotation = ___m_windMarker.rotation;
				___m_windMarker.rotation = Quaternion.Euler(0, 0, lastPlayerRot.eulerAngles.y) * windRotation;
			}
		}
	}
}

























