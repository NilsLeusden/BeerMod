using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using HarmonyLib;

namespace BeerMod
{
	public class BeerValuable : MonoBehaviour
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

		public Transform particleSystemTransform;


		public Sound soundTimeGlassLoop;

		private float soundPitchLerp;

		private Renderer beerValuableRenderer;

		private States currentState;

		private int particleFocus;

		public static bool InvertInputActive { get; private set; }

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
			}
			InvertInputActive = false;
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
				stateStart = false;
			}
			InvertInputActive = true;
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
					item.playerAvatar.voiceChat.OverridePitch(Settings.voiceChatPitch.Value, 1f, 2f);
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
					instance.voiceChat.OverridePitch(Settings.voiceChatPitch.Value, 1f, 2f);
				}
				instance.OverridePupilSize(Settings.pupilSize.Value, 4, 1f, 1f, 5f, 0.5f);
				PlayerController.instance.OverrideSpeed(Settings.playerSpeed.Value);
				PlayerController.instance.OverrideLookSpeed(Settings.lookSpeed.Value, 2f, 1f);
				PlayerController.instance.OverrideAnimationSpeed(Settings.animationSpeed.Value, 1f, 2f);
				PlayerController.instance.OverrideTimeScale(Settings.timeScale.Value);
				physGrabObject.OverrideTorqueStrength(Settings.torqueStrength.Value);
				CameraZoom.Instance.OverrideZoomSet(Settings.zoomSize.Value, 0.1f, 0.5f, 1f, base.gameObject, 0);
				PostProcessing.Instance.SaturationOverride(Settings.saturation.Value, 0.1f, 0.5f, 0.1f, base.gameObject);
			}
		}
	}

	[HarmonyPatch(typeof(Input), nameof(Input.GetAxis), new[] { typeof(string) })]
	static class Patch_Input_GetAxis
	{
		static void Postfix(string axisName, ref float __result)
		{
			if (BeerValuable.InvertInputActive && (axisName == "Horizontal"))
			{
				__result = -__result;
			}
		}
	}
}
