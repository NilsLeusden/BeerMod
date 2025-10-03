using BepInEx.Configuration;

namespace BeerMod.Settings
{
    internal static class SettingsClass
    {
		public static ConfigEntry<byte>? PourAngle { get; private set; }
		public static ConfigEntry<float>? VoiceChatPitch { get; private set; }
		public static ConfigEntry<float>? PlayerSpeed { get; private set; }
		public static ConfigEntry<float>? LookSpeed { get; private set; }
		public static ConfigEntry<float>? AnimationSpeed { get; private set; }
		public static ConfigEntry<float>? TimeScale { get; private set; }
		public static ConfigEntry<float>? TorqueStrength { get; private set; }
		public static ConfigEntry<float>? PupilSize { get; private set; }
		public static ConfigEntry<float>? ZoomSize { get; private set; }
		public static ConfigEntry<float>? Saturation { get; private set; }
		public static ConfigEntry<int>? SegmentCount { get; private set; }
		public static ConfigEntry<float>? ArcHeight { get; private set; }
		public static ConfigEntry<float>? MinInvert { get; private set; }
		public static ConfigEntry<float>? MaxInvert { get; private set; }
		public static ConfigEntry<float>? InvertDuration { get; private set; }
		public static ConfigEntry<float>? TiltChance { get; private set; }
		public static ConfigEntry<float>? TiltDegreeX { get; private set; }
		public static ConfigEntry<float>? TiltDegreeZ { get; private set; }
		public static ConfigEntry<float>? TiltSpeed { get; private set; }
		public static ConfigEntry<float>? Vignette { get; private set; }
		public static ConfigEntry<float>? MotionBlur { get; private set; }
		public static ConfigEntry<float>? BloomIntensity { get; private set; }
		public static ConfigEntry<float>? BloomThreshold { get; private set; }
		public static ConfigEntry<float>? Contrast { get; private set; }


		public static void Initialize(ConfigFile config)
		{
			PourAngle = config.Bind<byte>(
				"Pouring",
				"Pouring angle for bottle",
				85,
				new ConfigDescription(
				"The pouring angle for liquid animation",
				new AcceptableValueRange<byte>(1, 180)
				));
			VoiceChatPitch = config.Bind<float>(
				"Pitch",
				"Voice chat pitch",
				0.75f,
				new ConfigDescription(
				"Overriding the pitch of player when beer is picked up",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			PlayerSpeed = config.Bind<float>(
				"Speed",
				"Player speed",
				0.8f,
				new ConfigDescription(
				"Speed that the player moves when holding the beer",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			LookSpeed = config.Bind<float>(
				"Speed",
				"Player look speed",
				0.8f,
				new ConfigDescription(
				"Speed that the player looks around when holding the beer",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			AnimationSpeed = config.Bind<float>(
				"Speed",
				"Player animation speed",
				0.8f,
				new ConfigDescription(
				"Speed of the player's animation when holding the beer",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			TimeScale = config.Bind<float>(
				"Speed",
				"Player time scale speed",
				0.8f,
				new ConfigDescription(
				"Speed of the player's time scale when holding the beer",
				new AcceptableValueRange<float>(0.1f, 1f)
				));
			TorqueStrength = config.Bind<float>(
				"Torque",
				"Player torque strength",
				0.9f,
				new ConfigDescription(
				"Players torque strength when holding the bottle",
				new AcceptableValueRange<float>(0.1f, 2f)
				));
			PupilSize = config.Bind<float>(
				"Pupil",
				"pupil size",
				0.9f,
				new ConfigDescription(
				"Players pupil size when holdin the beer",
				new AcceptableValueRange<float>(0.1f, 4f)
				));
			ZoomSize = config.Bind<float>(
				"Zoom",
				"Zoom size",
				50f,
				new ConfigDescription(
				"Players camera zoom size when holdin the beer",
				new AcceptableValueRange<float>(1f, 50f)
				));
			Saturation = config.Bind<float>(
				"Saturation",
				"Saturation difference",
				2f,
				new ConfigDescription(
				"Saturation difference when holding the beer",
				new AcceptableValueRange<float>(1f, 50f)
				));
			SegmentCount = config.Bind<int>(
				"Animation",
				"Beer pour animation quality",
				12,
				new ConfigDescription(
				"Beer pouring animation quality set by segments in the stream",
				new AcceptableValueRange<int>(1, 36)
				));
			ArcHeight = config.Bind<float>(
				"Animation",
				"arc height",
				1.00f,
				new ConfigDescription(
				"The curve of the stream as it falls",
				new AcceptableValueRange<float>(0f, 2f)
				));
			MinInvert = config.Bind<float>(
				"Invert",
				"minimum time",
				2f,
				new ConfigDescription(
				"minimum time until inverted movement",
				new AcceptableValueRange<float>(1f, 20f)
				));
			MaxInvert = config.Bind<float>(
				"Invert",
				"maximum time",
				5f,
				new ConfigDescription(
				"maximum time until inverted movement",
				new AcceptableValueRange<float>(2f, 20f)
				));
			InvertDuration = config.Bind<float>(
				"Invert",
				"invert time duration",
				2f,
				new ConfigDescription(
				"inverted movement duration",
				new AcceptableValueRange<float>(2f, 20f)
				));
			TiltChance = config.Bind<float>(
				"Invert",
				"Tilt chance",
				0.75f,
				new ConfigDescription(
				"Chance of a camera tilt while inverted",
				new AcceptableValueRange<float>(0f, 1f)
				));
			TiltDegreeX = config.Bind<float>(
				"Invert",
				"Verticle tilt range",
				3f,
				new ConfigDescription(
					"The range of the verticle camera tilt, tied to Invert durations",
				new AcceptableValueRange<float>(0f, 20f)
				));
			TiltDegreeZ = config.Bind<float>(
				"Invert",
				"Horizontal tilt range",
				10f,
				new ConfigDescription(
					"The range of the horizontal camera tilt, tied to Invert durations",
				new AcceptableValueRange<float>(0f, 20f)
				));
			TiltSpeed = config.Bind<float>(
				"Invert",
				"Tilt speed",
				2f,
				new ConfigDescription(
					"the speed at which the camera tilts, tied to Invert durations",
				new AcceptableValueRange<float>(0f, 20f)
				));
			Vignette = config.Bind<float>(
				"Vignette",
				"Vignette intensity",
				0.5f,
				new ConfigDescription(
				"player's vision Vignette intensity",
				new AcceptableValueRange<float>(0f, 200f)
				));
			MotionBlur = config.Bind<float>(
				"MotionBlur",
				"MotionBlur intensity",
				0.5f,
				new ConfigDescription(
				"player's MotionBlur intensity",
				new AcceptableValueRange<float>(0f, 20f)
				));
			BloomIntensity = config.Bind<float>(
				"Bloom",
				"bloom Intensity",
				1f,
				new ConfigDescription(
				"player's bloom intensity",
				new AcceptableValueRange<float>(0f, 20f)
				));
			BloomThreshold = config.Bind<float>(
				"Bloom",
				"bloom Threshold",
				0.5f,
				new ConfigDescription(
				"player's bloom Threshold",
				new AcceptableValueRange<float>(0f, 20f)
				));
			Contrast = config.Bind<float>(
				"Contrast",
				"Contrast intensity",
				0.5f,
				new ConfigDescription(
				"player's Contrast intensity",
				new AcceptableValueRange<float>(0f, 20f)
				));
		}
	}
}
