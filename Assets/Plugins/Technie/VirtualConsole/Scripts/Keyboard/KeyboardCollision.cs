using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.VirtualConsole
{
	public class KeyboardCollision : MonoBehaviour
	{
		public Transform[] collisionTips;
		public float tipRadius = 0.02f;

		public float maxDepressionDepth = 0.1f;

		public float keyReturnSpeed = 0.2f;

		public bool showDebug;

		// Internal State

		private Key[] keys;

		private Vector3[] restPositions;
		private Vector3[] depressDir;
		private Vector3[] forwardDir;
		private Quaternion[] rotation;
		private float[] depressionAmount;
		private bool[] hasCollision;

		void Start()
		{
			keys = GetComponentsInChildren<Key>(true);

			restPositions = new Vector3[keys.Length];
			depressDir = new Vector3[keys.Length];
			forwardDir = new Vector3[keys.Length];
			rotation = new Quaternion[keys.Length];
			depressionAmount = new float[keys.Length];
			hasCollision = new bool[keys.Length];

			for (int i = 0; i < keys.Length; i++)
			{
				restPositions[i] = keys[i].transform.position;
				depressDir[i] = keys[i].transform.up * -1;
				forwardDir[i] = keys[i].transform.forward;
				rotation[i] = keys[i].transform.rotation;
			}
		}

		void Update()
		{
			for (int i = 0; i < keys.Length; i++)
			{
				depressionAmount[i] = Mathf.Clamp(depressionAmount[i] - (keyReturnSpeed * Time.deltaTime), 0, maxDepressionDepth);
			}

			for (int i = 0; i < keys.Length; i++)
			{
				Key key = keys[i];

				bool hit = false;
				float newDepth = depressionAmount[i];

				foreach (Transform tip in collisionTips)
				{
					float hitDepth;
					hit = HasCollision(i, key, tip, out hitDepth);
					if (hit)
					{
						newDepth = Mathf.Max(newDepth, hitDepth);
					}
				}

				hasCollision[i] = hit;
				depressionAmount[i] = Mathf.Clamp(newDepth, 0, maxDepressionDepth);
			}

			// Push new position to key objects
			for (int i = 0; i < keys.Length; i++)
			{
				Key key = keys[i];
				Vector3 currPos = restPositions[i] + depressDir[i] * depressionAmount[i];
				key.transform.position = currPos;
			}
		}

		private bool HasCollision(int i, Key key, Transform tip, out float hitDepth)
		{
			bool hit = false;
			hitDepth = 0f;

			if (key.collisionType == Key.CollisionType.Cylinder)
			{
				Vector3 currPos = restPositions[i] + depressDir[i] * depressionAmount[i];
				Matrix4x4 basis = Matrix4x4.TRS(currPos, rotation[i], Vector3.one);
				Matrix4x4 inverse = basis.inverse;

				float sqrMinSeparation = Mathf.Pow(tipRadius + key.cylinderRadius, 2);
				float minVertSeparation = (key.cylinderDepth * 0.5f) + tipRadius;

				Vector3 localTip = inverse.MultiplyPoint3x4(tip.position);
				float sqrDist = (localTip.x * localTip.x) + (localTip.z * localTip.z);
				float yDist = Mathf.Abs(localTip.y);
				if (sqrDist < sqrMinSeparation && yDist < minVertSeparation)
				{
					Vector3 surfacePos = restPositions[i] - (depressDir[i] * ((key.cylinderDepth * 0.5f) + tipRadius));
					float dot = Vector3.Dot(depressDir[i], tip.position - surfacePos);
					if (dot > 0f)
					{
						float newDepth = dot;
						hitDepth = newDepth;
					}
					hit = true;
				}
			}
			else if (key.collisionType == Key.CollisionType.Box)
			{
				Vector3 currPos = restPositions[i] + depressDir[i] * depressionAmount[i];
				Matrix4x4 basis = Matrix4x4.TRS(currPos, rotation[i], Vector3.one);
				Matrix4x4 inverse = basis.inverse;

				Vector3 localTip = inverse.MultiplyPoint3x4(tip.position);
				float dx = Mathf.Abs(localTip.x);
				float dy = Mathf.Abs(localTip.y);
				float dz = Mathf.Abs(localTip.z);
				float maxX = (key.boxSize.x * 0.5f) + tipRadius;
				float maxY = (key.boxSize.y * 0.5f) + tipRadius;
				float maxZ = (key.boxSize.z * 0.5f) + tipRadius;
				if (dx < maxX && dy < maxY && dz < maxZ)
				{
					Vector3 surfacePos = restPositions[i] - (depressDir[i] * ((key.boxSize.y * 0.5f) + tipRadius));
					float dot = Vector3.Dot(depressDir[i], tip.position - surfacePos);
					if (dot > 0f)
					{
						float newDepth = dot;
						hitDepth = newDepth;
					}
					hit = true;
				}
			}

			return hit;
		}

		private void OnDrawGizmos()
		{
			if (!showDebug)
				return;

			if (depressionAmount == null || depressionAmount.Length == 0)
				return;
			
			for (int i = 0; i < keys.Length; i++)
			{
				Gizmos.color = hasCollision[i] ? Color.red : Color.green;

				Key key = keys[i];
				if (key.collisionType == Key.CollisionType.Cylinder)
				{
					float halfDepth = key.cylinderDepth * 0.5f;

					Vector3 currPos = restPositions[i] + depressDir[i] * depressionAmount[i];
					Vector3 top = currPos + (depressDir[i] * halfDepth);
					Vector3 bottom = currPos - (depressDir[i] * halfDepth);

					Gizmos.matrix = Matrix4x4.identity;

					DrawCylinder(top, bottom, key.cylinderRadius);
					Gizmos.DrawWireSphere(currPos, 0.005f);
				}
				else if (key.collisionType == Key.CollisionType.Box)
				{
					Vector3 currPos = restPositions[i] + depressDir[i] * depressionAmount[i];

					Gizmos.matrix = Matrix4x4.TRS(currPos, rotation[i], Vector3.one);
					Gizmos.DrawWireCube(Vector3.zero, key.boxSize);
					Gizmos.matrix = Matrix4x4.identity;
				}
			}

			for (int i = 0; i < collisionTips.Length; i++)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(collisionTips[i].position, tipRadius);
			}
		}

		private static void DrawCylinder(Vector3 top, Vector3 bottom, float radius)
		{
			Vector3 axis = (top - bottom).normalized;
			Vector3 tangent = (Mathf.Abs(Vector3.Dot(axis, Vector3.up)) > 0.9 ? Vector3.Cross(axis, Vector3.right) : Vector3.Cross(axis, Vector3.up)).normalized;
			Vector3 arcTangent = Vector3.Cross(axis, tangent);

			DrawCircle(top, radius, tangent, arcTangent);
			DrawCircle(bottom, radius, tangent, arcTangent);

			Gizmos.DrawLine(top + tangent * radius, bottom + tangent * radius);
			Gizmos.DrawLine(top - tangent * radius, bottom - tangent * radius);
			Gizmos.DrawLine(top + arcTangent * radius, bottom + arcTangent * radius);
			Gizmos.DrawLine(top - arcTangent * radius, bottom - arcTangent * radius);
		}

		private static void DrawCircle(Vector3 position, float radius, Vector3 planeX, Vector3 planeY)
		{
			int numSegments = 32;
			for (int i = 0; i < numSegments; i++)
			{
				float a0 = ((float)i / (float)numSegments) * Mathf.PI * 2.0f;
				float a1 = ((float)(i + 1) / (float)numSegments) * Mathf.PI * 2.0f;

				float x0 = Mathf.Cos(a0) * radius;
				float y0 = Mathf.Sin(a0) * radius;

				float x1 = Mathf.Cos(a1) * radius;
				float y1 = Mathf.Sin(a1) * radius;

				Vector3 p0 = (x0 * planeX) + (y0 * planeY);
				Vector3 p1 = (x1 * planeX) + (y1 * planeY);

				Gizmos.DrawLine(position + p0, position + p1);
			}
		}
	}
}
