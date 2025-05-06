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
	[HarmonyPatch(typeof(MixingStationCanvas), "BeginButtonPressed")]
	internal static class MixingStationCanvasPatch {
		private static void Postfix(MixingStationCanvas __instance) {
			if(Prefs.mixingStationToggle.Value) {
				MelonCoroutines.Start(AutomateMixingStationCoroutine());
			} else {
				Melon<Mod>.Logger.Msg("Automate mixing station disabled in settings");
			}
		}

		static System.Collections.IEnumerator AutomateMixingStationCoroutine() {
			MixingStation mixingStation;
			Transform product;
			IngredientPiece productPiece;
			Beaker productBeaker;
			Vector3 moveToPosition;
			Vector3 moveBackToPosition;
			Vector3 rotateToAngles;
			bool stepComplete;
			bool callbackError;
			float time;

			float _waitBeforeStartingMixingStationTask = Prefs.GetTiming(Prefs.waitBeforeStartingMixingStationTask);
			float _timeToMoveProductToMixer = Prefs.GetTiming(Prefs.timeToMoveProductToMixer);
			float _timeToMovePourableToMixer = Prefs.GetTiming(Prefs.timeToMovePourableToMixer);
			float _timeToRotatePourableToMixer = Prefs.GetTiming(Prefs.timeToRotatePourableToMixer);
			float _timeToRotateAndMovePourableFromMixerBack = Prefs.GetTiming(Prefs.timeToRotateAndMovePourableFromMixerBack);
			float _waitBetweenMovingItemsToMixer = Prefs.GetTiming(Prefs.waitBetweenMovingItemsToMixer);
			float _waitBeforePressingMixerStartButton = Prefs.GetTiming(Prefs.waitBeforePressingMixerStartButton);

			Melon<Mod>.Logger.Msg("Mixing station task started");

			yield return new WaitForSeconds(_waitBeforeStartingMixingStationTask);

			mixingStation = GameObject.FindObjectsOfType<MixingStation>().FirstOrDefault(m => m.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

			if(Utils.NullCheck(mixingStation, "Can't find the mixing station the player is using"))
				yield break;

			if(!IsMixingStationInUse(mixingStation)) {
				Melon<Mod>.Logger.Msg("Probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Moving products");

			for(int i = 0; i < mixingStation.ItemContainer.childCount; i++) {
				if(Utils.NullCheck([mixingStation, mixingStation.ItemContainer, mixingStation.BowlFillable], "Can't find mixing station - probably exited task"))
					yield break;

				if(!IsMixingStationInUse(mixingStation)) {
					Melon<Mod>.Logger.Msg("Probably exited task");
					yield break;
				}

				product = mixingStation.ItemContainer.GetChild(i);

				if(Utils.NullCheck(product, "Can't find product - probably exited task"))
					yield break;

				productPiece = product.GetComponentInChildren<IngredientPiece>();
				productBeaker = product.GetComponentInChildren<Beaker>();

				if(!Utils.NullCheck(productPiece)) {
					moveToPosition = mixingStation.BowlFillable.transform.position;
					moveToPosition.y += 0.3f;

					Melon<Mod>.Logger.Msg("Moving product to mixer");

					callbackError = false;

					yield return Utils.SinusoidalLerpPositionCoroutine(productPiece.transform, moveToPosition, _timeToMoveProductToMixer, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find product piece to move - probably exited task");
						yield break;
					}
				} else if(!Utils.NullCheck(productBeaker)) {
					Melon<Mod>.Logger.Msg("Moving beaker to mixer");

					moveBackToPosition = productBeaker.transform.position;

					moveToPosition = productBeaker.transform.position.Between(mixingStation.BowlFillable.transform.position, 0.3f);
					moveToPosition.y += 0.35f;

					callbackError = false;

					yield return Utils.SinusoidalLerpPositionCoroutine(productBeaker.transform, moveToPosition, _timeToMovePourableToMixer, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find beaker - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Rotating beaker");

					productBeaker.transform.localEulerAngles = Vector3.zero;
					rotateToAngles = new Vector3(productBeaker.transform.localEulerAngles.x, productBeaker.transform.localEulerAngles.x, productBeaker.transform.localEulerAngles.y + 90);

					yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(productBeaker.transform, moveToPosition, rotateToAngles, _timeToRotatePourableToMixer, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find beaker to move and rotate - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Holding beaker");

					stepComplete = false;
					time = 0;

					//Up to 5 seconds
					while(time < 5) {
						if(Utils.NullCheck(productBeaker, "Can't find beaker - probably exited task"))
							yield break;

						if(productBeaker.Pourable.LiquidLevel == 0) {
							Melon<Mod>.Logger.Msg("Done pouring beaker");
							stepComplete = true;
							break;
						}

						productBeaker.transform.position = moveToPosition;
						productBeaker.transform.localEulerAngles = rotateToAngles;

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Pouring beaker didn't complete after 5 seconds");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Moving beaker out of the way");

					callbackError = false;

					yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(productBeaker.transform, moveBackToPosition, Vector3.zero, _timeToRotateAndMovePourableFromMixerBack, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find beaker to move and rotate - probably exited task");
						yield break;
					}
				} else {
					Melon<Mod>.Logger.Msg("Can't find product piece or beaker - probably exited task");
					yield break;
				}

				yield return new WaitForSeconds(_waitBetweenMovingItemsToMixer);
			}

			yield return new WaitForSeconds(_waitBeforePressingMixerStartButton);

			if(Utils.NullCheck([mixingStation, mixingStation.StartButton], "Can't find mixing station start button - probably exited task"))
				yield break;

			if(!IsMixingStationInUse(mixingStation)) {
				Melon<Mod>.Logger.Msg("Probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Pressing start button");

			mixingStation.StartButton.StartClick(new RaycastHit());

			Melon<Mod>.Logger.Msg("Done mixing");
		}

		private static bool IsMixingStationInUse(MixingStation mixingStation) {
			return (mixingStation.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false) && (!GameObject.FindObjectOfType<MixingStationCanvas>()?.Canvas?.enabled ?? false);
		}
	}
}
