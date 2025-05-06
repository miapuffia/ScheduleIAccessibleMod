using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerTasks;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	[HarmonyPatch(typeof(LabOvenCanvas), "BeginButtonPressed")]
	internal static class LabOvenCanvasPatch {
		private static readonly Vector3[] labOvenTrayPositionOffsets = [
			new Vector3(0, 0, 0),
			new Vector3(0, 0, 0.1f),
			new Vector3(0, 0, -0.1f),
			new Vector3(0.1f, 0, 0),
			new Vector3(0.1f, 0, 0.1f),
			new Vector3(0.1f, 0, -0.1f),
			new Vector3(-0.1f, 0, 0),
			new Vector3(-0.1f, 0, 0.1f),
			new Vector3(-0.1f, 0, -0.1f),
		];

		private static void Postfix(LabOvenCanvas __instance) {
			if(Prefs.labOvenToggle.Value) {
				MelonCoroutines.Start(AutomateLabOvenCoroutine());
			} else {
				Melon<Mod>.Logger.Msg("Automate lab oven disabled in settings");
			}
		}

		static System.Collections.IEnumerator AutomateLabOvenCoroutine() {
			LabOven labOven;
			Vector3 moveToPosition;
			bool stepComplete;
			bool callbackError;
			float time;

			float _waitBeforeStartingLabOvenTask = Prefs.GetTiming(Prefs.waitBeforeStartingLabOvenTask);
			float _timeToOpenLabOvenDoor = Prefs.GetTiming(Prefs.timeToOpenLabOvenDoor);
			float _timeToCloseLabOvenDoor = Prefs.GetTiming(Prefs.timeToCloseLabOvenDoor);
			float _waitBeforeMovingProductsToTray = Prefs.GetTiming(Prefs.waitBeforeMovingProductsToTray);
			float _timeToMoveProductToTray = Prefs.GetTiming(Prefs.timeToMoveProductToTray);
			float _waitBetweenMovingProductsToTray = Prefs.GetTiming(Prefs.waitBetweenMovingProductsToTray);
			float _waitBeforeClosingLabOvenDoorCocaine = Prefs.GetTiming(Prefs.waitBeforeClosingLabOvenDoorCocaine);
			float _waitBeforePressingLabOvenStartButton = Prefs.GetTiming(Prefs.waitBeforePressingLabOvenStartButton);

			Melon<Mod>.Logger.Msg("Lab oven task started");

			yield return new WaitForSeconds(_waitBeforeStartingLabOvenTask);

			labOven = GameObject.FindObjectsOfType<LabOven>().FirstOrDefault(m => m.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

			if(Utils.NullCheck(labOven, "Can't find the lab oven the player is using"))
				yield break;

			if(!labOven.IsReadyForHarvest()) {
				if(Utils.NullCheck([labOven.Door, labOven.Button], "Can't find lab oven - probably exited task"))
					yield break;

				Melon<Mod>.Logger.Msg("Opening lab oven door");

				callbackError = false;

				yield return Utils.LerpFloatCallbackCoroutine(0, 1, _timeToOpenLabOvenDoor, f => {
					if(Utils.NullCheck([labOven, labOven?.Door, labOven?.PourableContainer, labOven?.ItemContainer]) || !IsLabOvenInUse(labOven)) {
						callbackError = true;
						return false;
					}

					labOven.Door.TargetPosition = f;

					return true;
				});

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find lab door to open - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Done opening lab oven door");

				if(Utils.NullCheck([labOven, labOven?.PourableContainer, labOven?.ItemContainer], "Can't find lab oven door - probably exited task"))
					yield break;

				if(labOven.PourableContainer.childCount > 0) {
					Melon<Mod>.Logger.Msg("Lab oven has pourable");
					Melon<Mod>.Logger.Msg("Waiting for lab oven door to be closable");

					stepComplete = false;
					time = 0;

					if(Utils.NullCheck([labOven.LiquidMesh, labOven.Door], "Can't find lab oven - probably exited task"))
						yield break;

					//Up to 5 seconds
					while(time < 5) {
						if(Utils.NullCheck([labOven, labOven?.LiquidMesh, labOven?.Door], "Can't find lab oven - probably exited task"))
							yield break;

						if(labOven.LiquidMesh.gameObject.activeSelf && labOven.Door.Interactable) {
							Melon<Mod>.Logger.Msg("Lab oven door is closable");
							stepComplete = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Lab oven door wasn't closable after 5 seconds");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Closing lab oven door");

					if(Utils.NullCheck([labOven, labOven?.Door], "Can't find lab oven door - probably exited task"))
						yield break;

					callbackError = false;

					yield return Utils.LerpFloatCallbackCoroutine(labOven.Door.ActualPosition, 0, _timeToCloseLabOvenDoor, f => {
						if(Utils.NullCheck([labOven, labOven?.Door, labOven?.PourableContainer, labOven?.ItemContainer]) || !IsLabOvenInUse(labOven)) {
							callbackError = true;
							return false;
						}

						labOven.Door.TargetPosition = f;

						return true;
					});

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find lab door to close - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Done closing lab oven door");

					yield return new WaitForSeconds(_waitBeforePressingLabOvenStartButton);

					Melon<Mod>.Logger.Msg("Pressing lab oven button");

					if(Utils.NullCheck([labOven, labOven?.Button], "Can't find lab oven button - probably exited task"))
						yield break;

					labOven.Button.Press(new RaycastHit());

					Melon<Mod>.Logger.Msg("Done with lab oven");
				} else if(labOven.ItemContainer.childCount > 0) {
					Melon<Mod>.Logger.Msg("Lab oven has products");

					yield return new WaitForSeconds(_waitBeforeMovingProductsToTray);

					if(Utils.NullCheck([labOven, labOven?.ItemContainer, labOven?.SquareTray], "Can't find lab oven - probably exited task"))
						yield break;

					int i = 0;

					foreach(Draggable product in labOven.ItemContainer.GetComponentsInChildren<Draggable>()) {
						moveToPosition = new Vector3(labOven.SquareTray.transform.position.x + labOvenTrayPositionOffsets[i % labOvenTrayPositionOffsets.Length].x, labOven.SquareTray.transform.position.y + labOvenTrayPositionOffsets[i % labOvenTrayPositionOffsets.Length].y, labOven.SquareTray.transform.position.z + labOvenTrayPositionOffsets[i % labOvenTrayPositionOffsets.Length].z);
						moveToPosition.y += 0.3f;

						Melon<Mod>.Logger.Msg("Moving product to tray");

						callbackError = false;

						yield return Utils.SinusoidalLerpPositionCoroutine(product.transform, moveToPosition, _timeToMoveProductToTray, () => callbackError = true);

						if(callbackError) {
							Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
							yield break;
						}

						i++;

						yield return new WaitForSeconds(_waitBetweenMovingProductsToTray);
					}

					yield return new WaitForSeconds(_waitBeforeClosingLabOvenDoorCocaine);

					Melon<Mod>.Logger.Msg("Closing lab oven door");

					if(Utils.NullCheck([labOven, labOven?.Door], "Can't find lab oven door - probably exited task"))
						yield break;

					callbackError = false;

					yield return Utils.LerpFloatCallbackCoroutine(labOven.Door.ActualPosition, 0, _timeToCloseLabOvenDoor, f => {
						if(Utils.NullCheck([labOven, labOven?.Door, labOven?.PourableContainer, labOven?.ItemContainer]) || !IsLabOvenInUse(labOven)) {
							callbackError = true;
							return false;
						}

						labOven.Door.TargetPosition = f;

						return true;
					});

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find lab door to close - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Done closing lab oven door");

					yield return new WaitForSeconds(_waitBeforePressingLabOvenStartButton);

					Melon<Mod>.Logger.Msg("Pressing lab oven button");

					if(Utils.NullCheck([labOven, labOven?.Button], "Can't find lab oven button - probably exited task"))
						yield break;

					labOven.Button.Press(new RaycastHit());

					Melon<Mod>.Logger.Msg("Done with lab oven");
				} else {
					Melon<Mod>.Logger.Msg("Can't find pourable or products - probably exited task");
					yield break;
				}

			} else {
				//Couldn't figure out how to automate smashing with the hammer :(
			}
		}

		private static bool IsLabOvenInUse(LabOven labOven) {
			return labOven.PourableContainer.childCount > 0 || labOven.ItemContainer.childCount > 0 || labOven.Door.ActualPosition > 0;
		}
	}
}
