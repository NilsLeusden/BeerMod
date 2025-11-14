using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using HarmonyLib;
using System.Collections;


namespace BeerMod.BeerValuable
{
	public class BeerValuableClass : MonoBehaviour
	{
		public enum States
		{
			Idle = 0,
			Active = 1
		}
		public static bool InvertInputActive { get; private set; }
		public static bool InvertX { get; private set; }
		public static bool InvertY { get; private set; }
		public static bool InvertMouseX { get; private set; }
		public static bool InvertMouseY { get; private set; }
		public static bool TiltActive { get; private set; } = false;
		public static float TiltAngleX { get; private set; } = 0f;
		public static float TiltAngleZ { get; private set; } = 0f;
		public static float TiltSpeed { get; private set; } = Settings.SettingsClass.TiltSpeed!.Value;

		private static readonly string[] beerPhrases =
		{
			"Burppp",
			"WHOOOHOOOO",
			"I love this beer!",
			"Get me my car keys!",
			"REPO beer is the best!",
			"It's 5 PM somewhere!",
			"Heyyy I'm drinking hereeee!",
			"Why is the room spinning?",
			"This one's for the boys!",
			"I'm totally fine, trust me.",
			"Another round!",
			"This better not be non alcoholic!",
			"Suddenly, I can dance.",
			"Hold my beer and watch this.",
			"That hit different...",
			"Is this F D A approved?",
			"Was that a cop?",
			"Let’s make poor decisions!",
			"My liver just rage quit.",
			"Who put a steering wheel on the ceiling?",
			"Chug! Chug! Chug!",
			"Wait... was that glass?",
			"I feel powerful. And confused.",
			"This tastes like victory.",
			"Gimme another!",
			"Trust me, I have a P h D in Beerology.",
			"1 million beers please",
			"Wh where'd my hands go?",
			"I ain’t even that drunk...",
			"This beer... it gets me.",
			"Issss juss one beer, officer.",
			"Lemme tell you somethin...",
			"I can totally drive. No, really.",
			"You’re my best friend now.",
			"Wait, did I drink it or drop it?",
			"I swear I saw a unicorn.",
			"Why is everything upside down?",
			"I'm walking perfectly fine!",
			"Who turned down gravity?",
			"Beer makes you fast—science!",
			"Shhh I’m hiding from the floor.",
			"Is this my hand or yours?",
			"Time is fake. Beer is real.",
			"Y’all ever hear colors?",
			"My sanity left the bar hours ago!",
			"I’m not drunk, you're drunk.",
			"Heh gravity’s overrated anyway.",
			"I love you, random object.",
			"What seems to be the officer, problem?",
			"KOWABUNGAA!",
			"I dare you to steal my beer!",
			"Chat watch this!"
		};

		private bool stateStart;

		private bool localMsgHasPlayed;

		private PhysGrabObject? physGrabObject;

		private PhotonView? photonView;

		private float tiltDegreeX;
		private float tiltDegreeZ;
		private float tiltChance;
		private float minInvertInterval;
		private float maxInvertInterval;
		private float invertDuration;
		private float defaultBloomIntensity;
		private float defaultBloomThreshold;
		private float voiceChatPitch;
		private float playerSpeed;
		private float lookSpeed;
		private float animationSpeed;
		private float timeScale;
		private float torqueStrength;
		private float pupilSize;
		private float zoomSize;
		private float saturation;
		private float vignette;
		private float motionBlur;
		private float bloomIntensity;
		private float bloomThreshold;
		private float contrast;

		private float soundPitchLerp;

		private Renderer? beerValuableRenderer;

		private float _normalShutterAngle;

		private States currentState;


		private Coroutine? _invertRoutine;

		private void Awake()
		{
			minInvertInterval = Settings.SettingsClass.MinInvert!.Value;
			maxInvertInterval = Settings.SettingsClass.MaxInvert!.Value;
			invertDuration = Settings.SettingsClass.InvertDuration!.Value;
			defaultBloomIntensity = LevelGenerator.Instance.Level.BloomIntensity;
			defaultBloomThreshold = LevelGenerator.Instance.Level.BloomThreshold;
			voiceChatPitch = Settings.SettingsClass.VoiceChatPitch!.Value;
			playerSpeed = Settings.SettingsClass.PlayerSpeed!.Value;
			lookSpeed = Settings.SettingsClass.PlayerSpeed.Value;
			animationSpeed = Settings.SettingsClass.AnimationSpeed!.Value;
			timeScale = Settings.SettingsClass.TimeScale!.Value;
			torqueStrength = Settings.SettingsClass.TorqueStrength!.Value;
			pupilSize = Settings.SettingsClass.PupilSize!.Value;
			zoomSize = Settings.SettingsClass.ZoomSize!.Value;
			saturation = Settings.SettingsClass.Saturation!.Value;
			vignette = Settings.SettingsClass.Vignette!.Value;
			motionBlur = Settings.SettingsClass.MotionBlur!.Value;
			bloomIntensity = Settings.SettingsClass.BloomIntensity!.Value;
			bloomThreshold = Settings.SettingsClass.BloomThreshold!.Value;
			contrast = Settings.SettingsClass.Contrast!.Value;
			tiltChance = Settings.SettingsClass.TiltChance!.Value;
			tiltDegreeX = Settings.SettingsClass.TiltDegreeX!.Value;
			tiltDegreeZ = Settings.SettingsClass.TiltDegreeZ!.Value;
			_normalShutterAngle = PostProcessing.Instance.motionBlur.shutterAngle.value;
		}
		private void Start()
		{
			physGrabObject = GetComponent<PhysGrabObject>();
			beerValuableRenderer = GetComponentInChildren<MeshRenderer>(true);
			photonView = GetComponent<PhotonView>();
			if (physGrabObject == null)
			{
				Debug.LogError("PhysGrabObject not found!");
				enabled = false;
				return;
			}
			if (beerValuableRenderer == null)
			{
				Debug.LogError("MeshRenderer not found!");
				enabled = false;
				return;
			}
			if (photonView == null)
			{
				Debug.LogError("photonView not found!");
				enabled = false;
				return;
			}
		}
		private void Update()
		{
			if (beerValuableRenderer == null)
				return;
			switch (currentState)
			{
				case States.Active:
					StateActive();
					break;
				case States.Idle:
					StateIdle();
					break;
			}
		}

		[PunRPC]
		public void SetStateRPC(States state)
		{
			currentState = state;
			stateStart = true;
		}

		private void SetState(States state)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (!SemiFunc.IsMultiplayer())
				{
					SetStateRPC(state);
					return;
				}
				photonView!.RPC("SetStateRPC", RpcTarget.All, state);
			}
		}

		private void StateIdle()
		{
			if (stateStart)
			{
				stateStart = false;
				if (_invertRoutine != null)
					StopCoroutine(_invertRoutine);
				_invertRoutine = null;
				InvertInputActive = false;
				PostProcessing.Instance.motionBlur.shutterAngle.value = _normalShutterAngle;
				LevelGenerator.Instance.Level.BloomIntensity = defaultBloomIntensity;
				LevelGenerator.Instance.Level.BloomThreshold = defaultBloomThreshold;
			}
			if (SemiFunc.IsMasterClientOrSingleplayer() && physGrabObject!.grabbed)
			{
				SetState(States.Active);
			}
			soundPitchLerp = Mathf.Lerp(soundPitchLerp, 0f, Time.deltaTime * 10f);
		}
		private void StateActive()
		{
			if (stateStart && physGrabObject!.grabbedLocal)
			{
				_invertRoutine = StartCoroutine(InversionCycle());
				localMsgHasPlayed = false;
				stateStart = false;
			}
			soundPitchLerp = Mathf.Lerp(soundPitchLerp, 1f, Time.deltaTime * 2f);
			foreach (PhysGrabber item in physGrabObject!.playerGrabbing)
			{
				if ((bool)item && !item.isLocal)
				{
					item.playerAvatar.voiceChat.OverridePitch(voiceChatPitch, 1f, 2f);
				}
			}
			if (SemiFunc.IsMasterClientOrSingleplayer() && !physGrabObject.grabbed)
			{
				SetState(States.Idle);
			}
			if (physGrabObject.grabbedLocal)
			{
				GrabEffects();
				TrySendLocalMessage();
			}
		}

		private void TrySendLocalMessage()
		{
			if (!localMsgHasPlayed)
			{
				SendMessage();
				localMsgHasPlayed = true;
			}
		}

		private void GrabEffects()
		{
			PlayerAvatar instance = PlayerAvatar.instance;
			if ((bool)instance.voiceChat)
			{
				instance.voiceChat.OverridePitch(voiceChatPitch, 1f, 2f);
			}
			instance.OverridePupilSize(pupilSize, 4, 1f, 1f, 5f, 0.5f);
			PlayerController.instance.OverrideSpeed(playerSpeed);
			PlayerController.instance.OverrideLookSpeed(lookSpeed, 2f, 1f);
			PlayerController.instance.OverrideAnimationSpeed(animationSpeed, 1f, 2f);
			PlayerController.instance.OverrideTimeScale(timeScale);
			physGrabObject!.OverrideTorqueStrength(torqueStrength);
			CameraZoom.Instance.OverrideZoomSet(zoomSize, 0.1f, 0.5f, 1f, base.gameObject, 0);
			PostProcessing.Instance.SaturationOverride(saturation, 0.1f, 0.5f, 0.1f, base.gameObject);
			PostProcessing.Instance.VignetteOverride(Color.black, vignette, 1f, 1f, 0.5f, 0.1f, base.gameObject);
			PostProcessing.Instance.motionBlur.shutterAngle.value = motionBlur;
			PostProcessing.Instance.ContrastOverride(contrast, 1f, 1f, 1f, base.gameObject);
			LevelGenerator.Instance.Level.BloomIntensity = bloomIntensity;
			LevelGenerator.Instance.Level.BloomThreshold = bloomThreshold;
		}

		private IEnumerator InversionCycle()
		{
			while (physGrabObject != null && physGrabObject.grabbedLocal)
			{
				float delay = Random.Range(minInvertInterval, maxInvertInterval);
				yield return new WaitForSeconds(delay);

				InvertInputActive = true;
				InvertX = Random.value > 0.5f;
				InvertY = Random.value > 0.5f;
				InvertMouseX = Random.value > 0.5f;
				InvertMouseY = Random.value > 0.5f;
				if (Random.value < tiltChance)
				{
					TiltActive = true;
					TiltAngleX = Random.Range(-tiltDegreeX, tiltDegreeX);
					TiltAngleZ = Random.Range(-tiltDegreeZ, tiltDegreeZ);
				}
				yield return new WaitForSeconds(invertDuration);
				if (TiltActive)
				{
					TiltActive = false;
					TiltAngleX = 0f;
					TiltAngleZ = 0f;
				}
				InvertInputActive = false;
				InvertX = false;
				InvertY = false;
				InvertMouseX = false;
				InvertMouseY = false;
			}
		}

		public void ResetInversion()
		{
			StopAllCoroutines();
			InvertInputActive = false;
			InvertX = false;
			InvertY = false;
			InvertMouseX = false;
			InvertMouseY = false;
			TiltActive = false;
			TiltAngleX = 0f;
			TiltAngleZ = 0f;
		}
		private void SendMessage()
		{
			string message = beerPhrases[Random.Range(0, beerPhrases.Length)];
			Color textColor = new Color(0.95f, 0.8f, 0.1f, 1f);
			ChatManager.instance.PossessChatScheduleStart(10);
			ChatManager.instance.PossessChat(ChatManager.PossessChatID.None, message, 1f, textColor);
			ChatManager.instance.PossessChatScheduleEnd();
			return;
		}
	}
}
