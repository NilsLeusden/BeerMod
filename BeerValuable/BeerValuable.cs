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
			"Burp",
			"I love this beer"
		};

		private bool stateStart;
		private PhysGrabObject physGrabObject;

		private PhotonView photonView;

		private float minInvertInterval = Settings.SettingsClass.minInvert.Value;
		private float maxInvertInterval = Settings.SettingsClass.maxInvert.Value;

		private float invertDuration = Settings.SettingsClass.invertDuration.Value;

		public Transform particleSystemTransform;

		public Sound soundTimeGlassLoop;

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
				stateStart = false;
			}
			if (!particleSystemTransform)
				Debug.LogError("ParticleSystemTransform not found!");
			if (particleSystemTransform != null && particleSystemTransform.gameObject.activeSelf)
			{

				List<PhysGrabber> playerGrabbing = physGrabObject.playerGrabbing;
				if (playerGrabbing.Count > particleFocus)
				{
					PhysGrabber physGrabber = playerGrabbing[particleFocus];
					if ((bool)physGrabber)
					{
						Transform headLookAtTransform = physGrabber.playerAvatar.playerAvatarVisuals.headLookAtTransform;
						if ((bool)headLookAtTransform)
						{
							particleSystemTransform.LookAt(headLookAtTransform);
						}
						particleFocus++;
					}
					else
					{
						particleFocus = 0;
					}
				}
				else
				{
					particleFocus = 0;
				}
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
				//physGrabObject.OverrideDrag(1f);
				//physGrabObject.OverrideAngularDrag(0.5f);
				if (!physGrabObject.grabbed)
				{
					SetState(States.Idle);
				}
			}
			if (physGrabObject.grabbedLocal)
			{
				PlayerAvatar instance = PlayerAvatar.instance;
				if ((bool)instance.voiceChat)
				{
					instance.voiceChat.OverridePitch(Settings.SettingsClass.voiceChatPitch.Value, 1f, 2f);
					instance.voiceChat.
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
	}
}
