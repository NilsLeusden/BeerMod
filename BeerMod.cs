using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BeerMod
{

	[BepInPlugin("NilsThatBoi.BeerMod", "BeerMod", "1.0.0")]

	public class BeerMod : BaseUnityPlugin
	{
		internal static BeerMod Instance { get; private set; } = null!;
		internal new static ManualLogSource Logger => Instance._logger;
		private ManualLogSource _logger => base.Logger;
		internal Harmony? Harmony { get; set; }

		private void Awake()
		{
			Instance = this;

			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;
	
			Settings.Initialize(Config);

		Logger.LogError($"[Config] pourAngle={Settings.pourAngle.Value}, voiceChatPitch={Settings.voiceChatPitch.Value}, "
			+ $"playerSpeed={Settings.playerSpeed.Value}, lookSpeed={Settings.lookSpeed.Value}, …");
			Patch();

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


