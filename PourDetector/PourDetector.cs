using System.Collections;
using UnityEngine;

namespace BeerMod.PourDetector
{
	public class PourDetector : MonoBehaviour
	{
		private int pourThreshold;
		public Transform Origin = null;
		public GameObject PourStreamPrefab = null;

		private bool isPouring;

		private PourStream.PourStreamClass currentPourStream = null;


        private void Awake()
        {
            pourThreshold = Settings.SettingsClass.pourAngle.Value;
        }
		private void Update()
		{
			bool pourCheck = CalculatePourAngle() > pourThreshold;
			if (isPouring != pourCheck)
			{
				isPouring = pourCheck;
				if (isPouring)
					StartPour();
				else
					EndPour();
			}

		}

		private void StartPour()
		{
			currentPourStream = CreatePourStream();
			currentPourStream.Begin();
		}

		private void EndPour()
		{
			currentPourStream.End();
			currentPourStream = null;
		}

	private float CalculatePourAngle()
	{
		return Vector3.Angle(transform.up, Vector3.up);
	}
		private PourStream.PourStreamClass CreatePourStream()
		{
			GameObject PourStreamObject = Instantiate(PourStreamPrefab, Origin.position, Quaternion.identity, transform);
			return (PourStreamObject.GetComponent<PourStream.PourStreamClass>());
		}
	}
}