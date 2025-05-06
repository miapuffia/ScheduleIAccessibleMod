using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.StationFramework;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	[HarmonyPatch(typeof(CauldronCanvas), "BeginButtonPressed")]
	internal static class CauldronCanvasPatch {
		private static void Postfix(CauldronCanvas __instance) {
			if(Prefs.cauldronToggle.Value) {
				MelonCoroutines.Start(AutomateCauldronCoroutine());
			} else {
				Melon<Mod>.Logger.Msg("Automate cauldron disabled in settings");
			}
		}

		static System.Collections.IEnumerator AutomateCauldronCoroutine() {
			Cauldron cauldron;
			PourableModule gasoline;
			Vector3 moveToPosition;
			Vector3 moveBackToPosition;
			Vector3 rotateToAngles;
			bool stepComplete;
			bool callbackError;
			float time;

			float _waitBeforeStartingCauldronTask = Prefs.GetTiming(Prefs.waitBeforeStartingCauldronTask);
			float _timeToMoveGasolineToPot = Prefs.GetTiming(Prefs.timeToMoveGasolineToPot);
			float _timeToRotateGasolineToPot = Prefs.GetTiming(Prefs.timeToRotateGasolineToPot);
			float _timeToRotateAndMoveGasolineFromPotBack = Prefs.GetTiming(Prefs.timeToRotateAndMoveGasolineFromPotBack);
			float _waitBeforeMovingProductsToPot = Prefs.GetTiming(Prefs.waitBeforeMovingProductsToPot);
			float _timeToMoveProductToPot = Prefs.GetTiming(Prefs.timeToMoveProductToPot);
			float _waitBetweenMovingProductsToPot = Prefs.GetTiming(Prefs.waitBetweenMovingProductsToPot);
			float _waitBeforePressingCauldronStartButton = Prefs.GetTiming(Prefs.waitBeforePressingCauldronStartButton);

			Melon<Mod>.Logger.Msg("Cauldron task started");

			yield return new WaitForSeconds(_waitBeforeStartingCauldronTask);

			cauldron = GameObject.FindObjectsOfType<Cauldron>().FirstOrDefault(c => c.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

			if(Utils.NullCheck([cauldron, cauldron.ItemContainer], "Can't find the cauldron the player is using"))
				yield break;

			Melon<Mod>.Logger.Msg("Moving gasoline to pot");

			gasoline = cauldron.ItemContainer.GetComponentInChildren<PourableModule>();

			if(Utils.NullCheck(gasoline, "Can't find gasoline - probably exited task"))
				yield break;

			if(Utils.NullCheck(cauldron.CauldronFillable, "Can't find pot - probably exited task"))
				yield break;

			moveBackToPosition = gasoline.transform.position;
			moveBackToPosition.y += 0.4f;

			moveToPosition = gasoline.transform.position.Between(cauldron.CauldronFillable.transform.position, 0.5f);
			moveToPosition.y += 0.4f;

			gasoline.transform.localEulerAngles = Vector3.zero;

			callbackError = false;

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(gasoline.transform, moveToPosition, Vector3.zero, _timeToMoveGasolineToPot, () => callbackError = true);

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find gasoline - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Rotating gasoline");

			gasoline.transform.localEulerAngles = Vector3.zero;
			rotateToAngles = new Vector3(90, 0, 0);

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(gasoline.transform, moveToPosition, rotateToAngles, _timeToRotateGasolineToPot, () => callbackError = true);

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find gasoline to move and rotate - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Holding gasoline");

			stepComplete = false;
			time = 0;

			//Up to 5 seconds
			while(time < 5) {
				if(Utils.NullCheck(gasoline, "Can't find gasoline - probably exited task"))
					yield break;

				if(gasoline.LiquidLevel == 0) {
					Melon<Mod>.Logger.Msg("Done pouring gasoline");
					stepComplete = true;
					break;
				}

				gasoline.transform.position = moveToPosition;
				gasoline.transform.localEulerAngles = rotateToAngles;

				time += Time.deltaTime;

				yield return null;
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Pouring gasoline didn't complete after 5 seconds");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Moving gasoline out of the way");

			gasoline.transform.localEulerAngles = new Vector3(90, 0, 0);

			callbackError = false;

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(gasoline.transform, moveBackToPosition, Vector3.zero, _timeToRotateAndMoveGasolineFromPotBack, () => callbackError = true);

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find gasoline to move and rotate - probably exited task");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeMovingProductsToPot);

			if(Utils.NullCheck([cauldron, cauldron.ItemContainer], "Can't find the cauldron the player is using"))
				yield break;

			Melon<Mod>.Logger.Msg("Moving solid ingredients");

			foreach(IngredientPiece ingredientPiece in cauldron.ItemContainer.GetComponentsInChildren<IngredientPiece>()) {
				Melon<Mod>.Logger.Msg("Moving ingredient to pot");

				if(Utils.NullCheck(cauldron.CauldronFillable, "Can't find pot - probably exited task"))
					yield break;

				moveToPosition = ingredientPiece.transform.position.Between(cauldron.CauldronFillable.transform.position, 0.8f);
				moveToPosition.y += 0.4f;

				callbackError = false;

				yield return Utils.SinusoidalLerpPositionCoroutine(ingredientPiece.transform, moveToPosition, _timeToMoveProductToPot, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find ingredient - probably exited task");
					yield break;
				}

				yield return new WaitForSeconds(_waitBetweenMovingProductsToPot);
			}

			yield return new WaitForSeconds(_waitBeforePressingCauldronStartButton);

			if(Utils.NullCheck([cauldron, cauldron.StartButtonClickable], "Can't find mixing station start button - probably exited task"))
				yield break;

			if(!IsCauldronInUse(cauldron)) {
				Melon<Mod>.Logger.Msg("Probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Pressing start button");

			cauldron.StartButtonClickable.StartClick(new RaycastHit());

			Melon<Mod>.Logger.Msg("Done mixing");
		}

		private static bool IsCauldronInUse(Cauldron cauldron) {
			return (cauldron.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false) && (!GameObject.FindObjectOfType<CauldronCanvas>()?.Canvas?.enabled ?? false);
		}
	}
}
