using HarmonyLib;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Property.Utilities.Water;
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

		static System.Collections.IEnumerator AutomateSinkCoroutine(Tap tap) {
			float _waitBeforeStartingSinkTask = Prefs.GetTiming(Prefs.waitBeforeStartingSinkTask);

			Melon<Mod>.Logger.Msg("Sink task started");

			yield return new WaitForSeconds(_waitBeforeStartingSinkTask);

			if(Utils.NullCheck(tap, "Can't find the tap the player is using"))
				yield break;

			if(!tap.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
				Melon<Mod>.Logger.Msg("Tap isn't associated with player - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Holding open tap");
			tap.IsHeldOpen = true;
		}
	}
}
