using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BeerMod.Core
{

	[BepInPlugin("NilsThatBoi.BeerMod", "BeerMod", "1.0.4")]

	public class BeerModClass : BaseUnityPlugin
	{
		internal static BeerModClass Instance { get; private set; } = null!;
		internal new static ManualLogSource Logger => Instance._logger;
		private ManualLogSource _logger => base.Logger;
		internal Harmony? Harmony { get; set; }

		private void Awake()
		{
			Instance = this;

			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;
			Settings.SettingsClass.Initialize(Config);

			Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
		}

		internal void Patch()
		{
			Harmony ??= new Harmony(Info.Metadata.GUID);
			Harmony.PatchAll();
		}

		internal void Unpatch()
		{
			Harmony?.UnpatchSelf();
		}

		private void Update()
		{
		}
	}
}
