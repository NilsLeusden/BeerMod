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
			if (BeerValuable.BeerValuableClass.InvertInputActive && BeerValuable.BeerValuableClass.InvertX)
			{
				// Debug.Log("EDITING LOCAL INPUTMOVEMENT X");
				__result = -__result;
			}
		}
		[HarmonyPatch(typeof(SemiFunc), "InputMovementY")]
		[HarmonyPostfix]
		static void Post_InputMovementY(ref float __result)
		{
			if (BeerValuable.BeerValuableClass.InvertInputActive && BeerValuable.BeerValuableClass.InvertY)
			{
				// Debug.Log("EDITING LOCAL INPUTMOVEMENT Y");
				__result = -__result;
			}
		}
	}
	[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Update))]
	static class Patch_PlayerController_Look
	{
		static void Postfix(PlayerController __instance)
		{
			if (BeerValuable.BeerValuableClass.InvertInputActive)
			{
				float mx = Input.GetAxis("Mouse X");
				float my = Input.GetAxis("Mouse Y");
				if (BeerValuable.BeerValuableClass.InvertMouseX)
					mx = -mx;
				if (BeerValuable.BeerValuableClass.InvertMouseY)
					my = -my;
				__instance.cameraGameObject.transform.Rotate(-my, mx, 0f);
				if (BeerValuable.BeerValuableClass.TiltActive)
				{
					Transform cam = __instance.cameraGameObject.transform;
					Vector3 current = cam.localEulerAngles;
					if (current.x > 180f)
						current.x -= 360f;
					if (current.z > 180f)
						current.z -= 360f;
					Vector3 targetEuler = new Vector3(
						current.x + BeerValuable.BeerValuableClass.TiltAngleX,
						__instance.cameraGameObject.transform.localEulerAngles.y,
						current.z + BeerValuable.BeerValuableClass.TiltAngleZ
					);
					float tiltSpeed = BeerValuable.BeerValuableClass.TiltSpeed;
					cam.localEulerAngles = Vector3.Lerp(
						current,
						targetEuler,
						Time.deltaTime * tiltSpeed
					);
				}
			}
		}
	}
}