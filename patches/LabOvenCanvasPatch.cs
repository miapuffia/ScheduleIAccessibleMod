using HarmonyLib;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerTasks;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

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

		private static void Prefix(LabOvenCanvas __instance) {
			if(Prefs.labOvenToggle.Value) {
				MelonCoroutines.Start(AutomateLabOvenCoroutine(__instance));
			} else {
				Melon<Mod>.Logger.Msg("Automate lab oven disabled in settings");
			}
		}

		private static System.Collections.IEnumerator AutomateLabOvenCoroutine(LabOvenCanvas labOvenCanvas) {
			LabOven labOven;
			LabOvenHammer hammer;
			Draggable hammerDraggable;
			Vector3 moveToPosition;
			bool stepComplete;
			bool isInUse;
			bool isError = false;
			float time;

			float _waitBeforeStartingLabOvenTask = Prefs.GetTiming(Prefs.waitBeforeStartingLabOvenTask);
			float _timeToOpenLabOvenDoor = Prefs.GetTiming(Prefs.timeToOpenLabOvenDoor);
			float _timeToCloseLabOvenDoor = Prefs.GetTiming(Prefs.timeToCloseLabOvenDoor);
			float _waitBeforeMovingProductsToTray = Prefs.GetTiming(Prefs.waitBeforeMovingProductsToTray);
			float _timeToMoveProductToTray = Prefs.GetTiming(Prefs.timeToMoveProductToTray);
			float _waitBetweenMovingProductsToTray = Prefs.GetTiming(Prefs.waitBetweenMovingProductsToTray);
			float _waitBeforeClosingLabOvenDoorCocaine = Prefs.GetTiming(Prefs.waitBeforeClosingLabOvenDoorCocaine);
			float _waitBeforePressingLabOvenStartButton = Prefs.GetTiming(Prefs.waitBeforePressingLabOvenStartButton);
			float _waitBeforeMovingHammerOverTray = Prefs.GetTiming(Prefs.waitBeforeMovingHammerOverTray);
			float _timeToMoveHammerOverTray = Prefs.GetTiming(Prefs.timeToMoveHammerOverTray);

			Melon<Mod>.Logger.Msg("Lab oven task started");

			if(Utils.NullCheck([labOvenCanvas, labOvenCanvas?.Oven], "Can't find lab oven - probably exited task"))
				yield break;

			labOven = labOvenCanvas.Oven;

			yield return new WaitForSeconds(_waitBeforeStartingLabOvenTask);

			GetIsLabOvenInUse(labOven, out isInUse, ref isError);

			if(isError || !isInUse) {
				Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
				yield break;
			}

			if(!labOven.IsReadyForHarvest()) {
				Melon<Mod>.Logger.Msg("Opening lab oven door");

				if(Utils.NullCheck([labOven, labOven?.Door, labOven?.Button], "Can't find lab oven - probably exited task"))
					yield break;

				isError = false;

				yield return Utils.LerpFloatCallbackCoroutine(0, 1, _timeToOpenLabOvenDoor, f => {
					GetIsLabOvenInUse(labOven, out isInUse, ref isError);

					if(isError || !isInUse) {
						isError = true;
						return false;
					}

					labOven.Door.TargetPosition = f;

					return true;
				});

				if(isError) {
					Melon<Mod>.Logger.Msg("Can't find lab door to open - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Done opening lab oven door");

				if(Utils.NullCheck([labOven, labOven?.PourableContainer, labOven?.ItemContainer], "Can't find lab oven components - probably exited task"))
					yield break;

				if(labOven.PourableContainer.childCount > 0) {
					Melon<Mod>.Logger.Msg("Lab oven has pourable");
					Melon<Mod>.Logger.Msg("Waiting for lab oven door to be closable");

					stepComplete = false;
					time = 0;

					//Up to 5 seconds
					while(time < 5) {
						GetIsLabOvenInUse(labOven, out isInUse, ref isError);

						if(isError || Utils.NullCheck(labOven.LiquidMesh) || !isInUse) {
							Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
							yield break;
						}

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

					isError = false;

					yield return Utils.LerpFloatCallbackCoroutine(labOven.Door.ActualPosition, 0, _timeToCloseLabOvenDoor, f => {
						GetIsLabOvenInUse(labOven, out isInUse, ref isError);

						if(isError || !isInUse) {
							isError = true;
							return false;
						}

						labOven.Door.TargetPosition = f;

						return true;
					});

					if(isError) {
						Melon<Mod>.Logger.Msg("Can't find lab oven door to close - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Done closing lab oven door");

					yield return new WaitForSeconds(_waitBeforePressingLabOvenStartButton);

					Melon<Mod>.Logger.Msg("Pressing lab oven button");

					GetIsLabOvenInUse(labOven, out isInUse, ref isError);

					if(isError || Utils.NullCheck(labOven.Button) || !isInUse) {
						Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
						yield break;
					}

					labOven.Button.Press(new RaycastHit());

					Melon<Mod>.Logger.Msg("Done with lab oven");
				} else if(labOven.ItemContainer.childCount > 0) {
					Melon<Mod>.Logger.Msg("Lab oven has products");
					Melon<Mod>.Logger.Msg("Waiting for tray to be ready");

					stepComplete = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						GetIsLabOvenInUse(labOven, out isInUse, ref isError);

						if(isError || Utils.NullCheck(labOven.WireTray) || !isInUse) {
							Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
							yield break;
						}

						if(labOven.WireTray.ActualPosition > 0.99f) {
							Melon<Mod>.Logger.Msg("Lab oven tray is ready");
							stepComplete = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Lab oven door wasn't ready after 3 seconds");
						yield break;
					}

					yield return new WaitForSeconds(_waitBeforeMovingProductsToTray);

					if(Utils.NullCheck([labOven, labOven?.ItemContainer, labOven?.SquareTray], "Can't find lab oven components - probably exited task"))
						yield break;

					int i = 0;

					foreach(Draggable product in labOven.ItemContainer.GetComponentsInChildren<Draggable>()) {
						Melon<Mod>.Logger.Msg("Moving product to tray");

						GetIsLabOvenInUse(labOven, out isInUse, ref isError);

						if(isError || Utils.NullCheck(labOven.SquareTray) || !isInUse) {
							Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
							yield break;
						}

						moveToPosition = new Vector3(labOven.SquareTray.transform.position.x + labOvenTrayPositionOffsets[i % labOvenTrayPositionOffsets.Length].x, labOven.SquareTray.transform.position.y + labOvenTrayPositionOffsets[i % labOvenTrayPositionOffsets.Length].y, labOven.SquareTray.transform.position.z + labOvenTrayPositionOffsets[i % labOvenTrayPositionOffsets.Length].z);
						moveToPosition.y += 0.3f;

						isError = false;

						yield return Utils.SinusoidalLerpPositionCoroutine(product.transform, moveToPosition, _timeToMoveProductToTray, () => isError = true);

						if(isError) {
							Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
							yield break;
						}

						i++;

						yield return new WaitForSeconds(_waitBetweenMovingProductsToTray);
					}

					Melon<Mod>.Logger.Msg("Waiting for door to be closable");

					stepComplete = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						GetIsLabOvenInUse(labOven, out isInUse, ref isError);

						if(isError || !isInUse) {
							Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
							yield break;
						}

						if(labOven.Door.Interactable) {
							Melon<Mod>.Logger.Msg("Lab oven door is closable");
							stepComplete = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Lab oven door wasn't closable after 3 seconds");
						yield break;
					}

					yield return new WaitForSeconds(_waitBeforeClosingLabOvenDoorCocaine);
					
					Melon<Mod>.Logger.Msg("Closing lab oven door");

					if(Utils.NullCheck([labOven, labOven?.Door], "Can't find lab oven door - probably exited task"))
						yield break;

					isError = false;

					yield return Utils.LerpFloatCallbackCoroutine(labOven.Door.ActualPosition, 0, _timeToCloseLabOvenDoor, f => {
						GetIsLabOvenInUse(labOven, out isInUse, ref isError);

						if(isError || !isInUse) {
							isError = true;
							return false;
						}

						labOven.Door.TargetPosition = f;

						return true;
					});

					if(isError) {
						Melon<Mod>.Logger.Msg("Can't find lab oven door to close - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Done closing lab oven door");

					yield return new WaitForSeconds(_waitBeforePressingLabOvenStartButton);

					Melon<Mod>.Logger.Msg("Pressing lab oven button");

					GetIsLabOvenInUse(labOven, out isInUse, ref isError);

					if(isError || Utils.NullCheck(labOven.Button) || !isInUse) {
						Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
						yield break;
					}

					labOven.Button.Press(new RaycastHit());

					Melon<Mod>.Logger.Msg("Done with lab oven");
				} else {
					Melon<Mod>.Logger.Msg("Can't find pourable or products - probably exited task");
					yield break;
				}

			} else {
				Melon<Mod>.Logger.Msg("Waiting for lab oven door to open");

				stepComplete = false;
				time = 0;

				//Up to 5 seconds
				while(time < 5) {
					GetIsLabOvenInUse(labOven, out isInUse, ref isError);

					if(isError || Utils.NullCheck(labOven.WireTray) || !isInUse) {
						Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
						yield break;
					}

					if(labOven.WireTray.ActualPosition < 0.001f) {
						Melon<Mod>.Logger.Msg("Lab oven door is open");
						stepComplete = true;
						break;
					}

					time += Time.deltaTime;

					yield return null;
				}

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Lab oven door wasn't open after 5 seconds");
					yield break;
				}

				yield return new WaitForSeconds(_waitBeforeMovingHammerOverTray);

				Melon<Mod>.Logger.Msg("Moving hammer over tray");

				GetIsLabOvenInUse(labOven, out isInUse, ref isError);

				hammer = labOven.GetComponentInChildren<LabOvenHammer>();
				hammerDraggable = hammer?.GetComponent<Draggable>();

				if(isError || Utils.NullCheck([labOven.HammerContainer, hammerDraggable]) || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
					yield break;
				}

				hammerDraggable.idleUpForce = 9.81f;

				moveToPosition = labOven.ShardSpawnPoints[0].position;
				moveToPosition.y += 0.4f;

				isError = false;

				yield return Utils.SinusoidalLerpPositionCoroutine(hammer.transform, moveToPosition, _timeToMoveHammerOverTray, () => isError = true);

				if(isError) {
					Melon<Mod>.Logger.Msg("Can't find hammer to move - probably exited task");
					yield break;
				}

				if(Utils.NullCheck(hammerDraggable)) {
					Melon<Mod>.Logger.Msg("Can't find lab oven hammer - probably exited task");
					yield break;
				}

				hammerDraggable.idleUpForce = 0;

				Melon<Mod>.Logger.Msg("Waiting for hammer to hit tray");

				if(Utils.NullCheck(labOven)) {
					Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
					yield break;
				}

				stepComplete = false;
				time = 0;

				//Up to 3 seconds
				while(time < 3) {
					GetIsLabOvenInUse(labOven, out isInUse, ref isError);

					if(isError || Utils.NullCheck([labOven.CookedLiquidMesh, hammer, hammer?.ImpactPoint]) || !isInUse) {
						Melon<Mod>.Logger.Msg("Can't find lab oven hammer - probably exited task");
						yield break;
					}

					if(hammer.transform.position.y - labOven.CookedLiquidMesh.transform.position.y < 0.03f) {
						Melon<Mod>.Logger.Msg("Hammer hit tray");
						stepComplete = true;
						break;
					}

					time += Time.deltaTime;

					yield return null;
				}

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Hammer didn't hit tray after 3 seconds");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Simulating shattering product");

				GetIsLabOvenInUse(labOven, out isInUse, ref isError);

				if(isError || Utils.NullCheck([
					labOven.CurrentOperation,
					labOven.CurrentOperation?.Cookable,
					labOven.CurrentOperation?.Cookable?.Product,
					labOven.CurrentOperation?.Cookable?.ProductShardPrefab,
					labOven.CookedLiquidMesh,
					labOven.OutputSlot,
				]) || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
					yield break;
				}

				int quantity = labOven.CurrentOperation.IngredientQuantity * labOven.CurrentOperation.Cookable.ProductQuantity;

				QualityItemInstance product = labOven.CurrentOperation.Cookable.Product.GetDefaultInstance(quantity).Cast<QualityItemInstance>();
				product.Quality = labOven.CurrentOperation.IngredientQuality;

				labOven.CookedLiquidMesh.transform.parent.gameObject.SetActive(false);
				labOven.Shatter(quantity, labOven.CurrentOperation.Cookable.ProductShardPrefab.gameObject);

				for(int i = 0; i < labOven.shards.Count; i++) {
					if(i >= labOven.ShardSpawnPoints.Count) {
						Melon<Mod>.Logger.Msg("Too many shards - continuing since this is visual");
						break;
					}

					GameObject.Destroy(labOven.shards[i].GetComponent<Rigidbody>());
					labOven.shards[i].transform.position = labOven.ShardSpawnPoints[i].position;
				}

				labOven.OutputSlot.InsertItem(product);

				yield return new WaitForSeconds(0.8f);

				GetIsLabOvenInUse(labOven, out isInUse, ref isError);

				if(isError || Utils.NullCheck(labOven.CookedLiquidMesh) || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find lab oven - probably exited task");
					yield break;
				}

				labOven.SetCookOperation(null, null, false);
				labOven.ResetSquareTray();
				labOven.CookedLiquidMesh.transform.parent.gameObject.SetActive(true);

				yield return Utils.SimulateKeyPress(Keyboard.current.escapeKey);

				Melon<Mod>.Logger.Msg("Done with lab oven");
			}
		}

		private static void GetIsLabOvenInUse(LabOven labOven, out bool isInUse, ref bool isError) {
			if(Utils.NullCheck([labOven, labOven?.PourableContainer, labOven?.ItemContainer, labOven?.Door, labOven?.WireTray])) {
				isError = true;
				isInUse = false;
				return;
			}

			isError = false;
			isInUse = !labOven.isOpen && (labOven.PourableContainer.childCount > 0 || labOven.ItemContainer.childCount > 0 || labOven.Door.ActualPosition > 0 || labOven.WireTray.TargetPosition > 0);
		}
	}
}
