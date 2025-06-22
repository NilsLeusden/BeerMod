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

		private static readonly string[] beerPhrases =
		{
			"Burppp",
			"I love this beer!",
			"Get me my car keys!",
			"REPO beer is the best!",
			"It's 5 PM somewhere!",
			"Why is the room spinning?",
			"This one's for the boys!",
			"I'm totally fine, trust me.",
			"Another round!",
			"This better not be non-alcoholic.",
			"Suddenly, I can dance.",
			"Hold my beer and watch this.",
			"That hit different...",
			"Is this FDA approved?",
			"Was that a cop?",
			"Let’s make poor decisions!",
			"Who put a steering wheel on the ceiling?",
			"Chug! Chug! Chug!",
			"Wait... was that glass?",
			"I feel powerful. And confused.",
			"This tastes like victory.",
			"Gimme another!",
			"Wh-where'd my hands go?",
			"I ain’t even that drunk...",
			"This beer... it gets me.",
			"Isss jus' one beer, officer.",
			"Lemme tell you somethin’...",
			"I can totally drive. No, really.",
			"You’re my best friend now.",
			"Wait, did I drink it or drop it?",
			"I swear I saw a unicorn.",
			"Why is everything upside down?",
			"I'm walkin' perfectly fine!",
			"Who turned down gravity?",
			"Beer makes you fast—science!",
			"Shhh... I’m hiding from the floor.",
			"Is this my hand or yours?",
			"Time is fake. Beer is real.",
			"Y’all ever hear colors?",
			"I’m not drunk, you're drunk.",
			"Heh... gravity’s overrated anyway.",
			"I love you, random object."
		};

		private bool stateStart;
		private PhysGrabObject physGrabObject;

		private PhotonView photonView;

		private float minInvertInterval = Settings.SettingsClass.minInvert.Value;
		private float maxInvertInterval = Settings.SettingsClass.maxInvert.Value;

		private float invertDuration = Settings.SettingsClass.invertDuration.Value;

		public Transform particleSystemTransform;

		private float soundPitchLerp;

		private Renderer beerValuableRenderer;

		private States currentState;

		private int particleFocus;

		public static bool InvertInputActive { get; private set; }

		private Coroutine? _invertRoutine;

		private void Start()
		{
			physGrabObject = GetComponent<PhysGrabObject>();
			beerValuableRenderer = GetComponentInChildren<MeshRenderer>(true);
			photonView = GetComponent<PhotonView>();

			if (physGrabObject == null)
				Debug.LogError("PhysGrabObject not found!");
			if (beerValuableRenderer == null)
				Debug.LogError("MeshRenderer not found!");
		}
		private void Update()
		{
			if (beerValuableRenderer == null)
				return;
			if (SemiFunc.IsMultiplayer())
			{
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
				photonView.RPC("SetStateRPC", RpcTarget.All, state);
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
			}
			if (SemiFunc.IsMasterClientOrSingleplayer() && physGrabObject.grabbed)
			{
				SetState(States.Active);
			}
			soundPitchLerp = Mathf.Lerp(soundPitchLerp, 0f, Time.deltaTime * 10f);
		}
		private void StateActive()
		{
			if (stateStart)
			{
				_invertRoutine = StartCoroutine(InversionCycle());
				SendMessage();
				stateStart = false;
			}

			soundPitchLerp = Mathf.Lerp(soundPitchLerp, 1f, Time.deltaTime * 2f);
			foreach (PhysGrabber item in physGrabObject.playerGrabbing)
			{
				if ((bool)item && !item.isLocal)
				{
					item.playerAvatar.voiceChat.OverridePitch(Settings.SettingsClass.voiceChatPitch.Value, 1f, 2f);
				}
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (!physGrabObject.grabbed)
				{
					SetState(States.Idle);
				}
			}
			if (physGrabObject.grabbedLocal)
			{
				GrabEffects();
			}
		}

		private void GrabEffects()
		{
			//physGrabObject.OverrideDrag(1f);
			//physGrabObject.OverrideAngularDrag(0.5f);
			PlayerAvatar instance = PlayerAvatar.instance;
			if ((bool)instance.voiceChat)
			{
				instance.voiceChat.OverridePitch(Settings.SettingsClass.voiceChatPitch.Value, 1f, 2f);
			}
			instance.OverridePupilSize(Settings.SettingsClass.pupilSize.Value, 4, 1f, 1f, 5f, 0.5f);
			PlayerController.instance.OverrideSpeed(Settings.SettingsClass.playerSpeed.Value);
			PlayerController.instance.OverrideLookSpeed(Settings.SettingsClass.lookSpeed.Value, 2f, 1f);
			PlayerController.instance.OverrideAnimationSpeed(Settings.SettingsClass.animationSpeed.Value, 1f, 2f);
			PlayerController.instance.OverrideTimeScale(Settings.SettingsClass.timeScale.Value);
			physGrabObject.OverrideTorqueStrength(Settings.SettingsClass.torqueStrength.Value);
			CameraZoom.Instance.OverrideZoomSet(Settings.SettingsClass.zoomSize.Value, 0.1f, 0.5f, 1f, base.gameObject, 0);
			PostProcessing.Instance.SaturationOverride(Settings.SettingsClass.saturation.Value, 0.1f, 0.5f, 0.1f, base.gameObject);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.5f, 1f, 1f, 0.5f, 0.1f, base.gameObject);
		}

		private IEnumerator InversionCycle()
		{
			while (physGrabObject != null && physGrabObject.grabbedLocal)
			{
				float delay = Random.Range(minInvertInterval, maxInvertInterval);
				yield return new WaitForSeconds(delay);

				InvertInputActive = true;
				yield return new WaitForSeconds(invertDuration);
				InvertInputActive = false;
			}
		}
		public void ResetInversion()
		{
			StopAllCoroutines();
			InvertInputActive = false;
		}
		private void SendMessage()
		{
			string message = beerPhrases[Random.Range(0, beerPhrases.Length)];
			Color textColor =new Color(0.95f, 0.8f, 0.1f, 1f);
			ChatManager.instance.PossessChatScheduleStart(10);
			ChatManager.instance.PossessChat(ChatManager.PossessChatID.LovePotion, message, 1f, textColor);
			ChatManager.instance.PossessChatScheduleEnd();
			return;
		}

	}

}
