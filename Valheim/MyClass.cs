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
		
//		static Color[] RotateMatrix(Color[] matrix, int n)
//		{
//	         Color[] ret = new Color[n * n];
//	         
//	         for (int i = 0; i < n; ++i) {
//	             for (int j = 0; j < n; ++j) {
//	                 ret[i*n + j] = matrix[(n - j - 1) * n + i];
//	             }
//	         }
//	         
//	         return ret;
//	     }
//		static Texture2D convtex(Texture mainTexture)
//		{
//			Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
//  
//			RenderTexture currentRT = RenderTexture.active;
//			
//			RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);
//			Graphics.Blit(mainTexture, renderTexture);
//			
//			RenderTexture.active = renderTexture;
//			texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
//			texture2D.Apply();
//			return texture2D;
//		}
		
		[HarmonyPatch(typeof(Minimap), "AddPin")]
		class PinsUpdate1 { static bool Prefix() { logger.LogWarning("Adding pin!"); return true; }}
		[HarmonyPatch(typeof(Minimap), "ClearPins")]
		class PinsUpdate2 { static bool Prefix() { logger.LogWarning("Clearing pins!"); return true; }}
		[HarmonyPatch(typeof(Minimap), "ForceRegen")]
		class PinsUpdate3 { static bool Prefix() { logger.LogWarning("Forcing regen!"); return true; }}
		
		//
		
		[HarmonyPatch(typeof(Minimap), "IsPointVisible")]
		class MinimapPinYCheck
		{
			static bool Prefix(ref bool __result)
			{
				logger.LogWarning("Setting point to FALSE");
				__result = false;
				return false;
			}
		}
		
		//
		
		[HarmonyPatch(typeof(Minimap), "Update")]
		class MinimapRotation
		{
//			public static Texture2D toTexture2D(ref RenderTexture rTex)
//			{
//			    Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
//			    var old_rt = RenderTexture.active;
//			    RenderTexture.active = rTex;
//			
//			    tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
//			    tex.Apply();
//			
//			    RenderTexture.active = old_rt;
//			    return tex;
//			}
//			
			static private bool initialized = false;
			static public GameObject mycameraObj;
			static public Camera mycamera;
//			static public Camera mycamera = new Camera();
			static public RenderTexture ctex;
			static void init()
			{
				if (initialized)
					return;
				else {
					logger.LogWarning("MOD: Initializing...");
					
					mycameraObj = new GameObject("mycamera", typeof(Camera));
					mycamera = mycameraObj.GetComponent(typeof(Camera)) as Camera;
					ctex = new RenderTexture(1000, 1000, 0, RenderTextureFormat.ARGB32);
					ctex.Create();
					mycamera.targetTexture = ctex;
					
					initialized = true;
					logger.LogWarning("MOD: ...Done!");
				}
			}
			
			static void Postfix(ref UnityEngine.UI.RawImage ___m_mapImageSmall, ref UnityEngine.UI.RawImage ___m_mapImageLarge, ref GameObject ___m_mapSmall, ref GameObject ___m_smallRoot, ref UnityEngine.RectTransform ___m_smallMarker)
			{
				Rect rSmall = ___m_mapImageSmall.uvRect;
				Component imgAsComp = ((Component)(object)___m_mapImageSmall);
//				Material imgMat = ___m_mapImageSmall.material;
				Vector3 a = imgAsComp.transform.eulerAngles;
				RectTransform rectTransform = imgAsComp.transform as RectTransform;
//				Quaternion t = ___m_smallMarker.rotation;
				Vector3 b = ___m_smallMarker.rotation.eulerAngles;
				
				RectTransform rectTransform1 = ((Component)(object)___m_mapImageSmall).transform as RectTransform;
				Vector2 r1s = new Vector2(rectTransform1.rect.width, rectTransform1.rect.height);
				Vector2 r1c = r1s * 0.5f;
				Quaternion r1t = rectTransform1.rotation;
				
				RectTransform rectTransform2 = ((Component)(object)___m_mapImageLarge).transform as RectTransform;
				Vector2 r2s = new Vector2(rectTransform2.rect.width, rectTransform2.rect.height);
				Vector2 r2c = r2s * 0.5f;
				
//				Shader shSmall = ___m_mapImageSmall.material.shader;
//				string fileName = Application.dataPath + "/Resources/RuntimeShaders/mapshader.shader";
//				if (File.Exists(fileName))
//				{
//					string shaderSourceCode = File.ReadAllText(fileName);
////					Debug.Log(fileName + " already exists.");
//					logger.LogWarning(shaderSourceCode);
//				} else {
//					logger.LogWarning(shaderSourceCode);
//				}
////				string[] names = ___m_mapImageSmall.material.GetTexturePropertyNames();
//				logger.LogWarning(shSmall.ToString());
//				foreach (var k in )
//				{
//					 logger.LogWarning(k);
//				}
//				Quaternion rot = Quaternion.Euler(0, 0, Time.time * 0.1f);
//        		Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
//				___m_mapImageSmall.material.SetInt("_Rotation", 36);
//				___m_mapImageSmall.material.SetMatrix("_TextureRotation", m);
//				string[] keys = ___m_mapImageSmall.material.shaderKeywords;
				
//				Texture2D tex2Dlarge = ___m_mapImageLarge.texture as Texture2D;
//				Color[] pixels = tex2Dlarge.GetPixels();
//				pixels = RotateMatrix(pixels, tex2Dlarge.width);
//				tex2Dlarge.SetPixels(pixels);
				
//				Vector3 myVector = new Vector3(0.0f, 0.0f, 0.0f);
//				Vector3 myVector2 = new Vector3(0.0f, 0.0f, 1.0f);
				
				
//				SpriteRenderer r1 = ___m_mapSmall.GetComponent<SpriteRenderer>();
//				SpriteRenderer r2 = ___m_smallRoot.GetComponent<SpriteRenderer>();
//				SpriteRenderer r3 = imgAsComp.GetComponent<SpriteRenderer>();
//				int l = ___m_mapSmall.layer;
//				int l2 = ___m_smallRoot.layer;
				
				Vector3 rot = new Vector3(0f, 0f, 45f * (float)Math.Sin(Time.time));
				Vector3 p = ___m_smallMarker.transform.position;
				p.x += (float)Math.Sin(Time.time);
				p.y += (float)Math.Sin(Time.time);
				p.z += (float)Math.Sin(Time.time);
//				Component whatToRotate = ((Component)(object)___m_mapImageSmall.transform.localRotation);
//				___m_smallMarker.transform.position = p;
				Quaternion q = Quaternion.Euler(rot.x, rot.y, rot.z);
//				q.w = 0.0f;
//				___m_mapSmall.transform.rotation = q;
//				q = ___m_mapSmall.transform.rotation;
				
//				___m_mapSmall.transform.Rotate(new Vector3(0.0f, 0.0f, 0.1f), Space.Self);
//				___m_smallMarker.transform.Rotate(new Vector3(0.0f, 0.0f, -0.1f), Space.Self);
				
//				Vector3 center = new Vector3(rSmall.x + rSmall.width/2.0f, rSmall.y + rSmall.height/2.0f, 0.0f);
//				Vector3 center = new Vector3(rectTransform.rect.center.x, rectTransform.rect.center.y, 0.0f);
//				___m_mapImageSmall.transform.RotateAround(center, new Vector3(0.0f, 0.0f, 1.0f), 0.1f);
//				imgAsComp.transform.RotateAround(new Vector3(0.0f, 0.0f, 0.1f), new Vector3(0.0f, 0.0f, 1.0f), 10);
				
				
//				___m_mapImageLarge.transform.RotateAround(r2c, ___m_mapImageSmall.transform.up, 0.1f);
				___m_mapImageLarge.transform.Rotate(___m_mapImageLarge.transform.up, Space.World);
//				___m_mapImageLarge.transform.Rotate(new Vector3(0.0f, 0.0f, 0.1f), Space.Self);
				
				
				
				
				
//				init();
//				mycamera.enabled = false;
//				mycamera.targetTexture
				
//				Texture2D t = toTexture2D(ref ctex);
				
//				___m_mapImageSmall.texture = ctex;
				
//				GUI.DrawTexture(new Rect(0, 0, 1000, 000), ctex);
				
				
				
				
				
				
				
				
//				___m_smallMarker.transform.eulerAngles.z = 0;
//				logger.LogWarning(String.Format("A: {0} {1} {2} {3}", ___m_mapImageSmall.transform.up, ___m_mapSmall.transform.up, ___m_smallMarker.transform.up, 0));
				
			}
		}
	}
}

























