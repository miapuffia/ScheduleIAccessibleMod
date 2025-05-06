using Il2CppGameKit.Utilities;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Packaging;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	internal class Utils {
		internal static System.Collections.IEnumerator LerpPositionCoroutine(Transform transform, Vector3 targetPosition, float duration, Action onError = null) {
			if(duration == 0) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				transform.position = targetPosition;
				yield break;
			}

			float time = 0;
			Vector3 startPosition = transform.position;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				Vector3 pos = startPosition;

				pos.y = Mathf.Lerp(startPosition.y, targetPosition.y, t);
				pos.x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
				pos.z = Mathf.Lerp(startPosition.z, targetPosition.z, t);

				transform.position = pos;

				time += Time.deltaTime;
				yield return null;
			}
		}

		internal static System.Collections.IEnumerator SinusoidalLerpPositionCoroutine(Transform transform, Vector3 targetPosition, float duration, Action onError = null) {
			if(duration == 0) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				transform.position = targetPosition;
				yield break;
			}

			float time = 0;
			Vector3 startPosition = transform.position;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				float yT = Mathf.Sin(t * Mathf.PI / 2);
				float xT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);
				Vector3 pos = startPosition;

				pos.x = Mathf.Lerp(startPosition.x, targetPosition.x, xT);
				pos.y = Mathf.Lerp(startPosition.y, targetPosition.y, yT);
				pos.z = Mathf.Lerp(startPosition.z, targetPosition.z, yT);

				transform.position = pos;

				time += Time.deltaTime;
				yield return null;
			}
		}

		internal static System.Collections.IEnumerator SinusoidalLerpPositionsCoroutine(Transform[] transforms, Vector3 positionModifier, float duration, Action onError = null) {
			if(duration == 0) {
				for(int i = 0; i < transforms.Length; i++) {
					if(transforms[i] == null) {
						onError?.Invoke();
						yield break;
					}

					transforms[i].position = new Vector3(transforms[i].position.x + positionModifier.x, transforms[i].position.y + positionModifier.y, transforms[i].position.z + positionModifier.z);
				}
				yield break;
			}

			float time = 0;

			Vector3[] startPositions = [.. transforms.Select(t => t.position)];
			Vector3 startPosition;

			while(time < duration) {
				for(int i = 0; i < transforms.Length; i++) {
					if(transforms[i] == null) {
						onError?.Invoke();
						yield break;
					}

					float t = time / duration;
					float yT = Mathf.Sin(t * Mathf.PI / 2);
					float xT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);

					startPosition = startPositions[i];
					Vector3 pos = startPosition;

					pos.x = Mathf.Lerp(startPosition.x, startPosition.x + positionModifier.x, xT);
					pos.y = Mathf.Lerp(startPosition.y, startPosition.y + positionModifier.y, yT);
					pos.z = Mathf.Lerp(startPosition.z, startPosition.z + positionModifier.z, yT);

					transforms[i].position = pos;
				}

				time += Time.deltaTime;
				yield return null;
			}
		}

		internal static System.Collections.IEnumerator LerpRotationCoroutine(Transform transform, Vector3 targetAngle, float duration, Action onError = null) {
			if(duration == 0) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				transform.localEulerAngles = targetAngle;
				yield break;
			}

			float time = 0;
			Vector3 startRotation = transform.localEulerAngles;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				Vector3 rot = startRotation;

				rot.x = Mathf.Lerp(startRotation.x, targetAngle.x, t);
				rot.y = Mathf.Lerp(startRotation.y, targetAngle.y, t);
				rot.z = Mathf.Lerp(startRotation.z, targetAngle.z, t);

				transform.localEulerAngles = rot;

				time += Time.deltaTime;
				yield return null;
			}
		}

		internal static System.Collections.IEnumerator SinusoidalLerpRotationCoroutine(Transform transform, Vector3 targetAngle, float duration, Action onError = null) {
			if(duration == 0) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				transform.localEulerAngles = targetAngle;
				yield break;
			}

			float time = 0;
			Vector3 startRotation = transform.localEulerAngles;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				float aT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);
				Vector3 rot = startRotation;

				rot.x = Mathf.Lerp(startRotation.x, targetAngle.x, aT);
				rot.y = Mathf.Lerp(startRotation.y, targetAngle.y, aT);
				rot.z = Mathf.Lerp(startRotation.z, targetAngle.z, aT);

				transform.localEulerAngles = rot;

				time += Time.deltaTime;
				yield return null;
			}
		}

		internal static System.Collections.IEnumerator SinusoidalLerpPositionAndRotationCoroutine(Transform transform, Vector3 targetPosition, Vector3 targetAngle, float duration, Action onError = null) {
			if(duration == 0) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				transform.position = targetPosition;
				transform.localEulerAngles = targetAngle;
				yield break;
			}

			float time = 0;
			Vector3 startPosition = transform.position;
			Vector3 startRotation = transform.localEulerAngles;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				float yT = Mathf.Sin(t * Mathf.PI / 2);
				float xT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);
				float aT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);

				Vector3 pos = startPosition;
				Vector3 rot = startRotation;

				pos.x = Mathf.Lerp(startPosition.x, targetPosition.x, xT);
				pos.y = Mathf.Lerp(startPosition.y, targetPosition.y, yT);
				pos.z = Mathf.Lerp(startPosition.z, targetPosition.z, yT);

				rot.x = Mathf.Lerp(startRotation.x, targetAngle.x, aT);
				rot.y = Mathf.Lerp(startRotation.y, targetAngle.y, aT);
				rot.z = Mathf.Lerp(startRotation.z, targetAngle.z, aT);

				transform.position = pos;
				transform.localEulerAngles = rot;

				time += Time.deltaTime;
				yield return null;
			}
		}

		internal static System.Collections.IEnumerator LerpFloatCallbackCoroutine(float start, float end, float duration, Func<float, bool> body) {
			if(duration == 0) {
				body.Invoke(end);
				yield break;
			}

			float time = 0;
			float delta = end - start;

			while(time < duration) {
				float t = time / duration;

				if(!body.Invoke(start + (delta * t)))
					yield break;

				time += Time.deltaTime;
				yield return null;
			}
		}

		internal static bool NullCheck(object obj, string message = null) {
			bool isNull;

			if(obj is GameObject) {
				isNull = (obj as GameObject) == null || (obj as GameObject).IsDestroyed() || (obj as GameObject).WasCollected;
			} else if(obj is Component) {
				isNull = (obj as Component) == null || (obj as Component).WasCollected || (obj as Component).gameObject == null || (obj as Component).gameObject.IsDestroyed() || (obj as Component).gameObject.WasCollected;
			} else {
				isNull = obj == null;
			}

			if(isNull && message != null) {
				Melon<Mod>.Logger.Msg(message);
			}

			return isNull;
		}

		internal static bool NullCheck(object[] obj, string message = null) {
			foreach(object o in obj) {
				if(o == null) {
					if(message != null)
						Melon<Mod>.Logger.Msg(message);

					return true;
				}
			}

			return false;
		}

		internal static bool ZeroCheck(long value, string message = null) {
			if(value == 0) {
				if(message != null)
					Melon<Mod>.Logger.Msg(message);

				return true;
			}

			return false;
		}
	}
}
