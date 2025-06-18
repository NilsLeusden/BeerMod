using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using HarmonyLib;

namespace BeerMod.MovementInfluence
{
	[HarmonyPatch]
	static class Patch_SemiFunc_Input
	{
		[HarmonyPatch(typeof(SemiFunc), "InputMovementX")]
		[HarmonyPostfix]
		static void PostInputMovementX(ref float __result)
		{
			if (BeerValuable.BeerValuableClass.InvertInputActive)
			{
				Debug.Log("EDITING LOCAL INPUTMOVEMENT X");
				__result = -__result;
			}
		}
		[HarmonyPatch(typeof(SemiFunc), "InputMovementY")]
		[HarmonyPostfix]
		static void Post_InputMovementY(ref float __result)
		{
			if (BeerValuable.BeerValuableClass.InvertInputActive)
			{
				 Debug.Log("EDITING LOCAL INPUTMOVEMENT Y");
				__result = -__result;
			}
		}
	}
	[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Update))]
	static class Patch_PlayerController_Look
	{
		static void Postfix(PlayerController __instance)
		{
			if (!BeerValuable.BeerValuableClass.InvertInputActive) return;

			float mx = Input.GetAxis("Mouse X");
			float my = Input.GetAxis("Mouse Y");
			__instance.cameraGameObject.transform.Rotate(-my, -mx, 0f);
		}
	}
}