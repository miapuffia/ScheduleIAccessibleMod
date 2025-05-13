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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace AutomatedTasksMod {
	internal class Utils {
		internal static System.Collections.IEnumerator LerpPositionCoroutine(Transform transform, Vector3 targetPosition, float duration, Action onError = null) {
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

			if(transform == null) {
				onError?.Invoke();
				yield break;
			}

			transform.position = targetPosition;
		}

		internal static System.Collections.IEnumerator SinusoidalLerpPositionCoroutine(Transform transform, Vector3 targetPosition, float duration, Action onError = null) {
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

			if(transform == null) {
				onError?.Invoke();
				yield break;
			}

			transform.position = targetPosition;
		}

		internal static System.Collections.IEnumerator SinusoidalLerpPositionsCoroutine(Transform[] transforms, Vector3 positionModifier, float duration, Action onError = null) {
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

			for(int i = 0; i < transforms.Length; i++) {
				if(transforms[i] == null) {
					onError?.Invoke();
					yield break;
				}

				transforms[i].position = new Vector3(startPositions[i].x + positionModifier.x, startPositions[i].y + positionModifier.y, startPositions[i].z + positionModifier.z);
			}
		}

		internal static System.Collections.IEnumerator LerpRotationCoroutine(Transform transform, Vector3 targetAngle, float duration, Action onError = null) {
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

			if(transform == null) {
				onError?.Invoke();
				yield break;
			}

			transform.localEulerAngles = targetAngle;
		}

		internal static System.Collections.IEnumerator SinusoidalLerpRotationCoroutine(Transform transform, Vector3 targetAngle, float duration, Action onError = null) {
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

			if(transform == null) {
				onError?.Invoke();
				yield break;
			}

			transform.localEulerAngles = targetAngle;
		}

		internal static System.Collections.IEnumerator SinusoidalLerpPositionAndRotationCoroutine(Transform transform, Vector3 targetPosition, Vector3 targetAngle, float duration, Action onError = null) {
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

			if(transform == null) {
				onError?.Invoke();
				yield break;
			}

			transform.position = targetPosition;
			transform.localEulerAngles = targetAngle;
		}

		internal static System.Collections.IEnumerator LerpFloatCallbackCoroutine(float start, float end, float duration, Func<float, bool> body) {
			float time = 0;
			float delta = end - start;

			while(time < duration) {
				float t = time / duration;

				if(!body.Invoke(start + (delta * t)))
					yield break;

				time += Time.deltaTime;
				yield return null;
			}

			body.Invoke(end);
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
				if(NullCheck(o)) {
					if(message != null)
						Melon<Mod>.Logger.Msg(message);

					return true;
				}
			}

			return false;
		}

		internal static System.Collections.IEnumerator SimulateKeyPress(KeyControl keyControl) {
			InputEventPtr eventPtr;

			StateEvent.From(Keyboard.current.device, out eventPtr);
			keyControl.WriteValueIntoEvent(0f, eventPtr);
			InputSystem.QueueEvent(eventPtr);

			yield return new WaitForSeconds(0.1f);

			StateEvent.From(Keyboard.current.device, out eventPtr);
			keyControl.WriteValueIntoEvent(1f, eventPtr);
			InputSystem.QueueEvent(eventPtr);
		}
	}
}
