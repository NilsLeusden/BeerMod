using BepInEx.Configuration;

namespace BeerMod
{
    internal class Settings
    {
		public static ConfigEntry<byte> pourAngle { get; private set; }
		public static ConfigEntry<float> voiceChatPitch { get; private set; }
		public static ConfigEntry<float> playerSpeed { get; private set; }
		public static ConfigEntry<float> lookSpeed { get; private set; }
		public static ConfigEntry<float> animationSpeed { get; private set; }
		public static ConfigEntry<float> timeScale { get; private set; }
		public static ConfigEntry<float> torqueStrength { get; private set; }
		public static ConfigEntry<float> pupilSize { get; private set; }
		public static ConfigEntry<float> zoomSize { get; private set; }
		public static ConfigEntry<float> saturation { get; private set; }
		public static ConfigEntry<int> segmentCount { get; private set; }
		public static ConfigEntry<float> arcHeight { get; private set; }

		public static void Initialize(ConfigFile config)
		{
			pourAngle = config.Bind<byte>(
				"Pouring",
				"Pouring angle for bottle",
				85,
				new ConfigDescription(
				"The pouring angle for liquid animation",
				new AcceptableValueRange<byte>(1, 180)
				));
			voiceChatPitch = config.Bind<float>(
				"Pitch",
				"Voice chat pitch",
				0.75f,
				new ConfigDescription(
				"Overriding the pitch of player when beer is picked up",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			playerSpeed = config.Bind<float>(
				"Speed",
				"Player speed",
				0.8f,
				new ConfigDescription(
				"Speed that the player moves when holding the beer",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			lookSpeed = config.Bind<float>(
				"Speed",
				"Player look speed",
				0.8f,
				new ConfigDescription(
				"Speed that the player looks around when holding the beer",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			animationSpeed = config.Bind<float>(
				"Speed",
				"Player animation speed",
				0.8f,
				new ConfigDescription(
				"Speed of the player's animation when holding the beer",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			timeScale = config.Bind<float>(
				"Speed",
				"Player time scale speed",
				0.8f,
				new ConfigDescription(
				"Speed of the player's time scale when holding the beer",
				new AcceptableValueRange<float>(0.1f, 1f)
				));
			torqueStrength = config.Bind<float>(
				"Torque",
				"Player torque strength",
				0.9f,
				new ConfigDescription(
				"Players torque strength when holding the bottle",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			pupilSize = config.Bind<float>(
				"Pupil",
				"pupil size",
				0.9f,
				new ConfigDescription(
				"Players pupil size when holdin the beer",
				new AcceptableValueRange<float>(0.1f, 4f)
				));
			zoomSize = config.Bind<float>(
				"Zoom",
				"Zoom size",
				15f,
				new ConfigDescription(
				"Players camera zoom size when holdin the beer",
				new AcceptableValueRange<float>(1f, 50f)
				));
			saturation = config.Bind<float>(
				"Saturation",
				"Saturation difference",
				15f,
				new ConfigDescription(
				"Saturation difference when holding the beer",
				new AcceptableValueRange<float>(1f, 50f)
				));
			segmentCount = config.Bind<int>(
				"Animation",
				"Beer pour animation quality",
				12,
				new ConfigDescription(
				"Beer pouring animation quality set by segments in the stream",
				new AcceptableValueRange<int>(1, 36)
				));
			arcHeight = config.Bind<float>(
				"Animation",
				"arc height",
				0.5f,
				new ConfigDescription(
				"The curve of the stream as it falls",
				new AcceptableValueRange<float>(0f, 1f)
				));
		}
	}
}


			//instance.voiceChat.OverridePitch(0.65f, 1f, 2f);

			//physGrabObject.OverrideDrag(1f);

			//physGrabObject.OverrideAngularDrag(0.5f);

			//CameraZoom.Instance.OverrideZoomSet(20f, 0.1f, 0.5f, 1f, base.gameObject, 0);

			//PostProcessing.Instance.SaturationOverride(30f, 0.1f, 0.5f, 0.1f, base.gameObject);

			//PlayerController.instance.OverrideSpeed(0.8f);

			//PlayerController.instance.OverrideLookSpeed(0.8f, 2f, 1f);

			//PlayerController.instance.OverrideAnimationSpeed(0.4f, 1f, 2f);

			//PlayerController.instance.OverrideTimeScale(0.6f);

			//physGrabObject.OverrideTorqueStrength(1f);

			//instance.OverridePupilSize(2f, 4, 1f, 1f, 5f, 0.5f);
