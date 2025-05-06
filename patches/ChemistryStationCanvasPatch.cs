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
	[HarmonyPatch(typeof(ChemistryStationCanvas), "BeginButtonPressed")]
	internal static class ChemistryStationCanvasPatch {
		private static void Postfix(ChemistryStationCanvas __instance) {
			if(Prefs.chemistryStationToggle.Value) {
				MelonCoroutines.Start(AutomateChemistryStationCoroutine());
			} else {
				Melon<Mod>.Logger.Msg("Automate chemistry station disabled in settings");
			}
		}

		static System.Collections.IEnumerator AutomateChemistryStationCoroutine() {
			ChemistryStation chemistryStation;
			Beaker beaker;
			StirringRod stirringRod;
			Vector3 moveToPosition;
			Vector3 moveBackToPosition;
			Vector3 rotateToAngles;
			bool stepComplete;
			bool callbackError;
			float time;

			float _waitBeforeStartingChemistryStationTask = Prefs.GetTiming(Prefs.waitBeforeStartingChemistryStationTask);
			float _timeToMoveProductToBeaker = Prefs.GetTiming(Prefs.timeToMoveProductToBeaker);
			float _waitBetweenMovingProductsToBeaker = Prefs.GetTiming(Prefs.waitBetweenMovingProductsToBeaker);
			float _timeToMovePourableToBeaker = Prefs.GetTiming(Prefs.timeToMovePourableToBeaker);
			float _timeToRotatePourableToBeaker = Prefs.GetTiming(Prefs.timeToRotatePourableToBeaker);
			float _timeToRotateAndMovePourableFromBeakerBack = Prefs.GetTiming(Prefs.timeToRotateAndMovePourableFromBeakerBack);
			float _waitBetweenMovingPourablesToBeaker = Prefs.GetTiming(Prefs.waitBetweenMovingPourablesToBeaker);
			float _timeToRotateStirRod = Prefs.GetTiming(Prefs.timeToRotateStirRod);
			float _waitBeforeMovingLabStandDown = Prefs.GetTiming(Prefs.waitBeforeMovingLabStandDown);
			float _timeToMoveLabStandDown = Prefs.GetTiming(Prefs.timeToMoveLabStandDown);
			float _waitBeforeMovingBeakerToFunnel = Prefs.GetTiming(Prefs.waitBeforeMovingBeakerToFunnel);
			float _timeToMoveBeakerToFunnel = Prefs.GetTiming(Prefs.timeToMoveBeakerToFunnel);
			float _timeToRotateBeakerToFunnel = Prefs.GetTiming(Prefs.timeToRotateBeakerToFunnel);
			float _waitBeforeMovingLabStandUp = Prefs.GetTiming(Prefs.waitBeforeMovingLabStandUp);
			float _timeToMoveLabStandUp = Prefs.GetTiming(Prefs.timeToMoveLabStandUp);
			float _waitBeforeHandlingBurner = Prefs.GetTiming(Prefs.waitBeforeHandlingBurner);

			Melon<Mod>.Logger.Msg("Chemistry station task started");

			yield return new WaitForSeconds(_waitBeforeStartingChemistryStationTask);

			chemistryStation = GameObject.FindObjectsOfType<ChemistryStation>().FirstOrDefault(m => m.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

			if(Utils.NullCheck(chemistryStation, "Can't find the chemistry station the player is using"))
				yield break;

			beaker = chemistryStation.GetComponentInChildren<Beaker>();

			if(Utils.NullCheck(beaker, "Can't find beaker - probably exited task"))
				yield break;

			Melon<Mod>.Logger.Msg("Moving ingredients to beaker");

			foreach(IngredientPiece ingredient in chemistryStation.ItemContainer.transform.GetComponentsInChildren<IngredientPiece>()) {
				if(Utils.NullCheck(ingredient, "Can't find ingredient - probably exited task"))
					yield break;

				if(Utils.NullCheck(beaker, "Can't find beaker - probably exited task"))
					yield break;

				moveToPosition = beaker.transform.position;
				moveToPosition.y += 0.4f;

				Melon<Mod>.Logger.Msg("Moving ingredient to beaker");

				callbackError = false;

				yield return Utils.SinusoidalLerpPositionCoroutine(ingredient.transform, moveToPosition, _timeToMoveProductToBeaker, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find ingredient to move - probably exited task");
					yield break;
				}

				yield return new WaitForSeconds(_waitBetweenMovingProductsToBeaker);
			}

			Melon<Mod>.Logger.Msg("Done moving ingredients to beaker");
			Melon<Mod>.Logger.Msg("Pouring ingredients");

			foreach(PourableModule pourable in chemistryStation.ItemContainer.transform.GetComponentsInChildren<PourableModule>()) {
				if(Utils.NullCheck(pourable, "Can't find pourable - probably exited task"))
					yield break;

				if(Utils.NullCheck(beaker, "Can't find beaker - probably exited task"))
					yield break;

				if(!pourable.IsModuleActive) {
					Melon<Mod>.Logger.Msg("Pourable is not active - skipping");
					continue;
				}

				moveBackToPosition = pourable.transform.position;

				moveToPosition = beaker.transform.position;
				moveToPosition.y += 0.4f;

				Melon<Mod>.Logger.Msg("Moving pourable to beaker");

				callbackError = false;

				yield return Utils.SinusoidalLerpPositionCoroutine(pourable.transform, moveToPosition, _timeToMovePourableToBeaker, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find pourable to move - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Rotating pourable");

				yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(pourable.transform, moveToPosition, new Vector3(pourable.transform.localEulerAngles.x, pourable.transform.localEulerAngles.x, pourable.transform.localEulerAngles.y + 180), _timeToRotatePourableToBeaker, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find pourable to move and rotate - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Holding pourable");

				stepComplete = false;
				time = 0;

				//Up to 3 seconds
				while(time < 3) {
					if(Utils.NullCheck(pourable, "Can't find pourable - probably exited task"))
						yield break;

					if(pourable.LiquidLevel == 0) {
						Melon<Mod>.Logger.Msg("Done pouring");
						stepComplete = true;
						break;
					}

					pourable.transform.position = moveToPosition;

					time += Time.deltaTime;

					yield return null;
				}

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Pouring didn't complete after 3 seconds");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Moving pourable out of the way");

				callbackError = false;

				yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(pourable.transform, moveBackToPosition, Vector3.zero, _timeToRotateAndMovePourableFromBeakerBack, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find pourable to move and rotate - probably exited task");
					yield break;
				}

				yield return new WaitForSeconds(_waitBetweenMovingPourablesToBeaker);
			}

			Melon<Mod>.Logger.Msg("Moving stirring rod");

			if(Utils.NullCheck(chemistryStation, "Can't find chemistry station - probably exited task"))
				yield break;

			stirringRod = chemistryStation.GetComponentInChildren<StirringRod>();

			if(Utils.NullCheck(stirringRod, "Can't find stirring rod - probably exited task"))
				yield break;

			stepComplete = false;
			time = 0;
			float maxTime = 8;

			//Up to 8 seconds
			while(maxTime > 0) {
				if(Utils.NullCheck(stirringRod)) {
					if(Utils.NullCheck(chemistryStation) || !IsChemistryStationInUse(chemistryStation)) {
						Melon<Mod>.Logger.Msg("Can't find chemistry station - probably exited task");
						yield break;
					} else { //Chemistry station is still being interacted with but stir rod is gone
						Melon<Mod>.Logger.Msg("Done stirring");
						stepComplete = true;
						break;
					}
				}

				if(time > 0.1) {
					Melon<Mod>.Logger.Msg("Simulating stir rod");

					stirringRod.CurrentStirringSpeed = 4f;
					stirringRod.enabled = false;

					callbackError = false;

					MelonCoroutines.Start(Utils.LerpRotationCoroutine(stirringRod.transform, new Vector3(stirringRod.transform.localEulerAngles.x, stirringRod.transform.localEulerAngles.y + 40, stirringRod.transform.localEulerAngles.z), _timeToRotateStirRod, () => callbackError = true));

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find stir rod to rotate - probably exited task");
						yield break;
					}

					maxTime -= time;
					time = 0;
				} else {
					stirringRod.enabled = true;
				}

				time += Time.deltaTime;

				yield return null;
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Stirring didn't complete after 8 seconds");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeMovingLabStandDown);

			Melon<Mod>.Logger.Msg("Moving lab stand down");

			if(Utils.NullCheck(chemistryStation, "Can't find chemistry station - probably exited task"))
				yield break;

			if(Utils.NullCheck(chemistryStation.LabStand, "Can't find lab stand - probably exited task"))
				yield break;

			callbackError = false;

			yield return Utils.LerpFloatCallbackCoroutine(1, 0, _timeToMoveLabStandDown, f => {
				if(Utils.NullCheck([chemistryStation, chemistryStation?.LabStand]) || !IsChemistryStationInUse(chemistryStation)) {
					callbackError = true;
					return false;
				}

				chemistryStation.LabStand.CurrentPosition = f;

				return true;
			});

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find lab stand to move - probably exited task");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeMovingBeakerToFunnel);

			Melon<Mod>.Logger.Msg("Moving beaker to funnel");

			if(Utils.NullCheck(beaker, "Can't find beaker - probably exited station"))
				yield break;

			if(Utils.NullCheck([chemistryStation, chemistryStation?.LabStand, chemistryStation?.LabStand?.Funnel], "Can't find lab stand - probably exited task"))
				yield break;

			moveToPosition = beaker.transform.position.Between(chemistryStation.LabStand.Funnel.transform.position, 0.3f);
			moveToPosition.y += 0.4f;

			callbackError = false;

			yield return Utils.SinusoidalLerpPositionCoroutine(beaker.transform, moveToPosition, _timeToMoveBeakerToFunnel, () => callbackError = true);

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find beaker - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Rotating beaker");

			beaker.transform.localEulerAngles = Vector3.zero;
			rotateToAngles = new Vector3(beaker.transform.localEulerAngles.x, beaker.transform.localEulerAngles.x, beaker.transform.localEulerAngles.y + 90);

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(beaker.transform, moveToPosition, rotateToAngles, _timeToRotateBeakerToFunnel, () => callbackError = true);

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find beaker to move and rotate - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Holding beaker");

			stepComplete = false;
			time = 0;

			//Up to 5 seconds
			while(time < 5) {
				if(Utils.NullCheck(beaker, "Can't find beaker - probably exited task"))
					yield break;

				if(beaker.Pourable.LiquidLevel == 0) {
					Melon<Mod>.Logger.Msg("Done pouring beaker");
					stepComplete = true;
					break;
				}

				beaker.transform.position = moveToPosition;
				beaker.transform.localEulerAngles = rotateToAngles;

				time += Time.deltaTime;

				yield return null;
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Pouring beaker didn't complete after 5 seconds");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeMovingLabStandUp);

			Melon<Mod>.Logger.Msg("Moving lab stand up");

			if(Utils.NullCheck([chemistryStation, chemistryStation?.LabStand], "Can't find chemistry station - probably exited task"))
				yield break;

			callbackError = false;

			yield return Utils.LerpFloatCallbackCoroutine(0, 1, _timeToMoveLabStandUp, f => {
				if(Utils.NullCheck([chemistryStation, chemistryStation?.LabStand]) || !IsChemistryStationInUse(chemistryStation)) {
					callbackError = true;
					return false;
				}

				chemistryStation.LabStand.CurrentPosition = f;

				return true;
			});

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find lab stand to move - probably exited task");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeHandlingBurner);

			Melon<Mod>.Logger.Msg("Handling burner");

			stepComplete = false;
			time = 0;

			//Up to 8 seconds
			while(time < 8) {
				if(Utils.NullCheck([chemistryStation, chemistryStation?.BoilingFlask, chemistryStation?.Burner], "Can't find chemistry station - probably exited task")) {
					TryToTurnBurnerOff(chemistryStation);
					yield break;
				}

				if(!IsChemistryStationInUse(chemistryStation)) {
					if(Utils.NullCheck(chemistryStation.CurrentCookOperation, "Probably exited task")) {
						TryToTurnBurnerOff(chemistryStation);
						yield break;
					} else {
						Melon<Mod>.Logger.Msg("Finished preparing recipe");
						TryToTurnBurnerOff(chemistryStation);
						stepComplete = true;
						break;
					}
				}

				if(chemistryStation.BoilingFlask.CurrentTemperature + chemistryStation.BoilingFlask.CurrentTemperatureVelocity < 250) {
					chemistryStation.Burner.IsDialHeld = true;
				} else {
					chemistryStation.Burner.IsDialHeld = false;
				}

				time += Time.deltaTime;

				yield return null;
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Handling burner didn't complete after 8 seconds");
				TryToTurnBurnerOff(chemistryStation);
				yield break;
			}
		}

		private static void TryToTurnBurnerOff(ChemistryStation chemistryStation) {
			if(Utils.NullCheck([chemistryStation, chemistryStation?.Burner])) {
				return;
			}

			chemistryStation.Burner.IsDialHeld = false;
			chemistryStation.Burner.CurrentHeat = 0;
		}

		private static bool IsChemistryStationInUse(ChemistryStation chemistryStation) {
			return chemistryStation.ItemContainer.childCount > 0;
		}
	}
}
