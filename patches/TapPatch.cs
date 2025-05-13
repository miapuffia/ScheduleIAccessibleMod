using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Property.Utilities.Water;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	[HarmonyPatch(typeof(Tap), "Interacted")]
	internal static class TapPatch {
		private static void Postfix(Tap __instance) {
			if(Prefs.sinkToggle.Value) {
				MelonCoroutines.Start(AutomateSinkCoroutine(__instance));
			} else {
				Melon<Mod>.Logger.Msg("Automate sink tap disabled in settings");
			}
		}

		private static System.Collections.IEnumerator AutomateSinkCoroutine(Tap tap) {
			bool isInUse;
			bool isError = false;

			float _waitBeforeStartingSinkTask = Prefs.GetTiming(Prefs.waitBeforeStartingSinkTask);

			Melon<Mod>.Logger.Msg("Sink task started");

			yield return new WaitForSeconds(_waitBeforeStartingSinkTask);

			Melon<Mod>.Logger.Msg("Holding open tap");

			GetIsTapInUse(tap, out isInUse, ref isError);

			if(isError || !isInUse) {
				Melon<Mod>.Logger.Msg("Can't find tap - probably exited task");
				yield break;
			}

			tap.IsHeldOpen = true;
		}

		private static void GetIsTapInUse(Tap tap, out bool isInUse, ref bool isError) {
			if(Utils.NullCheck([tap, tap?.PlayerUserObject])) {
				isError = true;
				isInUse = false;
				return;
			}

			isError = false;
			isInUse = tap.PlayerUserObject.GetComponent<Player>()?.IsLocalPlayer ?? false;
		}
	}
}
