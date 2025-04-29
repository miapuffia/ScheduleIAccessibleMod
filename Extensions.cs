using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	public static class Extensions {
		public static T GetComponentInImmediateChildren<T>(this Transform parent) {
			for(int i = 0; i < parent.childCount; i++) {
				if(parent.GetChild(i).TryGetComponent(out T component)) {
					return component;
				}
			}

			return default;
		}

		public static T[] GetComponentsInImmediateChildren<T>(this Transform parent) {
			List<T> childrenWithComponent = [];

			for(int i = 0; i < parent.childCount; i++) {
				if(parent.GetChild(i).TryGetComponent(out T component)) {
					childrenWithComponent.Add(component);
				}
			}

			return childrenWithComponent.ToArray();
		}

		public static Vector3 Between(this Vector3 a, Vector3 b, float amount) {
			return new Vector3(a.x + ((b.x - a.x) * amount), a.y + ((b.y - a.y) * amount), a.z + ((b.z - a.z) * amount));
		}

		public static float MaxComponentDifference(this Vector3 a, Vector3 b) {
			return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
		}
	}
}
