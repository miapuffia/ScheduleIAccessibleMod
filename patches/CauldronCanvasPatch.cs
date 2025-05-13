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
		private static void Prefix(CauldronCanvas __instance) {
			if(Prefs.cauldronToggle.Value) {
				MelonCoroutines.Start(AutomateCauldronCoroutine(__instance));
			} else {
				Melon<Mod>.Logger.Msg("Automate cauldron disabled in settings");
			}
		}

		private static System.Collections.IEnumerator AutomateCauldronCoroutine(CauldronCanvas cauldronCanvas) {
			Cauldron cauldron;
			PourableModule gasoline;
			Vector3 moveToPosition;
			Vector3 moveBackToPosition;
			Vector3 rotateToAngles;
			bool stepComplete;
			bool isInUse;
			bool isError = false;
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

			if(Utils.NullCheck([cauldronCanvas, cauldronCanvas?.Cauldron], "Can't find cauldron - probably exited task"))
				yield break;

			cauldron = cauldronCanvas.Cauldron;

			yield return new WaitForSeconds(_waitBeforeStartingCauldronTask);

			Melon<Mod>.Logger.Msg("Moving gasoline to pot");

			if(Utils.NullCheck([cauldron, cauldron?.ItemContainer], "Can't find cauldron container - probably exited task"))
				yield break;

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

			isError = false;

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(gasoline.transform, moveToPosition, Vector3.zero, _timeToMoveGasolineToPot, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find gasoline - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Rotating gasoline");

			gasoline.transform.localEulerAngles = Vector3.zero;
			rotateToAngles = new Vector3(90, 0, 0);

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(gasoline.transform, moveToPosition, rotateToAngles, _timeToRotateGasolineToPot, () => isError = true);

			if(isError) {
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

			isError = false;

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(gasoline.transform, moveBackToPosition, Vector3.zero, _timeToRotateAndMoveGasolineFromPotBack, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find gasoline to move and rotate - probably exited task");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeMovingProductsToPot);

			Melon<Mod>.Logger.Msg("Moving solid ingredients");

			GetIsCauldronInUse(cauldron, cauldronCanvas, out isInUse, ref isError);

			if(isError || Utils.NullCheck(cauldron.ItemContainer) || !isInUse) {
				Melon<Mod>.Logger.Msg("Can't find the cauldron the player is using");
				yield break;
			}

			foreach(IngredientPiece ingredientPiece in cauldron.ItemContainer.GetComponentsInChildren<IngredientPiece>()) {
				Melon<Mod>.Logger.Msg("Moving ingredient to pot");

				GetIsCauldronInUse(cauldron, cauldronCanvas, out isInUse, ref isError);

				if(isError || Utils.NullCheck([cauldron.CauldronFillable]) || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find the cauldron the player is using");
					yield break;
				}

				moveToPosition = ingredientPiece.transform.position.Between(cauldron.CauldronFillable.transform.position, 0.8f);
				moveToPosition.y += 0.4f;

				isError = false;

				yield return Utils.SinusoidalLerpPositionCoroutine(ingredientPiece.transform, moveToPosition, _timeToMoveProductToPot, () => isError = true);

				if(isError) {
					Melon<Mod>.Logger.Msg("Can't find ingredient - probably exited task");
					yield break;
				}

				yield return new WaitForSeconds(_waitBetweenMovingProductsToPot);
			}

			Melon<Mod>.Logger.Msg("Waiting for cauldron to be startable");

			stepComplete = false;
			time = 0;

			//Up to 3 seconds
			while(time < 3) {
				if(Utils.NullCheck([cauldron, cauldron?.StartButtonClickable], "Can't find cauldron start button - probably exited task"))
					yield break;

				if(cauldron.StartButtonClickable.ClickableEnabled) {
					Melon<Mod>.Logger.Msg("Cauldron is startable");
					stepComplete = true;
					break;
				}

				time += Time.deltaTime;

				yield return null;
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Cauldron didn't become startable after 3 seconds");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforePressingCauldronStartButton);

			Melon<Mod>.Logger.Msg("Pressing start button");

			if(Utils.NullCheck([cauldron, cauldron?.StartButtonClickable], "Can't find mixing station start button - probably exited task"))
				yield break;

			GetIsCauldronInUse(cauldron, cauldronCanvas, out isInUse, ref isError);

			if(isError || !isInUse) {
				Melon<Mod>.Logger.Msg("Probably exited task");
				yield break;
			}

			cauldron.StartButtonClickable.StartClick(new RaycastHit());

			Melon<Mod>.Logger.Msg("Done mixing");
		}

		private static void GetIsCauldronInUse(Cauldron cauldron, CauldronCanvas cauldronCanvas, out bool isInUse, ref bool isError) {
			if(Utils.NullCheck([cauldron, cauldron?.PlayerUserObject, cauldronCanvas, cauldronCanvas?.Canvas])) {
				isError = true;
				isInUse = false;
				return;
			}

			isError = false;
			isInUse = (cauldron.PlayerUserObject.GetComponent<Player>()?.IsLocalPlayer ?? false) && !cauldronCanvas.Canvas.enabled;
		}
	}
}
