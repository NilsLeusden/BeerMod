using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BeerMod.PourStream
{
	public class PourStreamClass : MonoBehaviour
	{
		private LineRenderer lineRenderer = null;

		[Header("Pour audio from Sound.cs")]
		public Sound pourSound;

		private ParticleSystem splashParticle = null;
		private ParticleSystem.EmissionModule splashEmission;


		private Coroutine pourRoutine = null;

		private Vector3 targetPosition = Vector3.zero;

		private int segmentCount;
		private float arcHeight;
		private void Awake()
		{
			if (pourSound == null)
				Debug.LogError("pourStream wasn't assigned/found in the inspector!");
			segmentCount = Settings.SettingsClass.segmentCount.Value;
			arcHeight = Settings.SettingsClass.arcHeight.Value;
			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.positionCount = segmentCount;
			splashParticle = GetComponentInChildren<ParticleSystem>();
			splashEmission = splashParticle.emission;
			splashEmission.enabled = false;
		}

		private void Start()
		{
			for (int i = 0; i < segmentCount; i++)
				lineRenderer.SetPosition(i, transform.position);
		}

		public void Begin()
		{
			pourRoutine = StartCoroutine(BeginPour());
			StartCoroutine(UpdateParticle());
		}

		private IEnumerator BeginPour()
		{
			while (gameObject.activeSelf)
			{
				if (pourSound != null)
				{
					pourSound.PlayLoop(true, fadeInSpeed: 0.1f, fadeOutSpeed: 0.1f);
				}
				targetPosition = FindEndPoint();
				DrawParabolicArc(transform.position, targetPosition);
				yield return null;
			}
		}
		private void DrawParabolicArc(Vector3 start, Vector3 end)
		{
			float totalDistance = Vector3.Distance(start, end);
			float maxSafeArc = totalDistance * 0.5f;
			float height = Mathf.Min(arcHeight, maxSafeArc);
			Vector3 streamDirection = end - start;
			Vector3 arcDirection = Vector3.Cross(Vector3.Cross(streamDirection, Vector3.down), streamDirection).normalized;
			for (int i = 0; i < segmentCount; i++)
			{
				float normalizedI = i / (float)(segmentCount - 1);
				Vector3 point = Vector3.Lerp(start, end, normalizedI);
				float heightOffset = 4f * height * normalizedI * (1f - normalizedI);
				point += arcDirection * heightOffset;
				lineRenderer.SetPosition(i, point);
			}
		}

		public void End()
		{
			if (pourSound != null)
			{
				pourSound.PlayLoop(playing: false, fadeInSpeed: 0.1f, fadeOutSpeed: 0.1f);
			}
			if (pourRoutine != null)
					StopCoroutine(pourRoutine);
			pourRoutine = StartCoroutine(EndPour());
		}

		private IEnumerator EndPour()
		{
			Vector3 start = transform.position;
			while (true)
			{
				bool allAtEnd = true;
				for (int i = 0; i < segmentCount; i++)
				{
					Vector3 current = lineRenderer.GetPosition(i);
					Vector3 next = Vector3.MoveTowards(current, targetPosition, Time.deltaTime * 2f);
					lineRenderer.SetPosition(i, next);
					if (next != targetPosition)
						allAtEnd = false;
				}
				if (allAtEnd)
					break;
				yield return null;
			}
			Destroy(gameObject);
		}

		private Vector3 FindEndPoint()
		{
			RaycastHit hit;
			Ray ray = new Ray(transform.position, Vector3.down);
			Physics.Raycast(ray, out hit, 2.0f);
			Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2.0f);
			return (endPoint);
		}

		private void MoveToPosition(int index, Vector3 targetPosition)
		{
			lineRenderer.SetPosition(index, targetPosition);
		}

		private void AnimateToPosition(int index, Vector3 targetPosition)
		{
			Vector3 currentPoint = lineRenderer.GetPosition(index);
			Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * 1.75f);
			lineRenderer.SetPosition(index, newPosition);
		}

		private bool HasReachedPosition(int index, Vector3 targetPosition)
		{
			Vector3 currentPosition = lineRenderer.GetPosition(index);

			return (currentPosition == targetPosition);
		}

		private IEnumerator UpdateParticle()
		{
			var emission = splashParticle.emission;
			splashParticle.Play();
			while (gameObject.activeSelf)
			{
				splashParticle.gameObject.transform.position = targetPosition;
				bool isHitting = HasReachedPosition(segmentCount - 1, targetPosition);
				emission.enabled = isHitting;
				yield return null;
			}
		}
	}
}
