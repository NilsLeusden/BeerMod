using HarmonyLib;
using UnityEngine;
using BeerMod.BeerValuable;

namespace BeerMod.BeerValuableBreak
{
    [HarmonyPatch(typeof(PhysGrabObject), nameof(PhysGrabObject.DestroyPhysGrabObject))]
	static class Patch_PhysGrabObject_Break
	{
		static void Postfix(PhysGrabObject __instance)
		{
			var beer = __instance.GetComponent<BeerValuableClass>();
			if (beer != null)
			{
				beer.ResetInversion();
				Debug.Log("[BeerMod] Input inversion cleared on beer break.");
			}
		}
	}
}