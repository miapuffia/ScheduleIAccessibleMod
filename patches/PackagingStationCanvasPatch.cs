using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Packaging;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	[HarmonyPatch(typeof(PackagingStationCanvas), "BeginButtonPressed")]
	internal static class PackagingStationCanvasPatch {
		private static void Prefix(PackagingStationCanvas __instance) {
			if(!Utils.NullCheck(__instance.PackagingStation) && __instance.PackagingStation.TryCast<PackagingStationMk2>() is null) {
				if(Prefs.mixingStationToggle.Value) {
					MelonCoroutines.Start(AutomatePackagingStationCoroutine(__instance));
				} else {
					Melon<Mod>.Logger.Msg("Automate mixing station disabled in settings");
				}
			}
		}

		private static System.Collections.IEnumerator AutomatePackagingStationCoroutine(PackagingStationCanvas packagingStationCanvas) {
			PackagingStation packagingStation;
			FunctionalPackaging packaging;
			Vector3 moveToPosition;
			bool stepComplete;
			bool isInUse;
			bool isError = false;
			float time;

			float _waitBeforeStartingPackagingTask = Prefs.GetTiming(Prefs.waitBeforeStartingPackagingTask);
			float _timeToMoveProductToPackaging = Prefs.GetTiming(Prefs.timeToMoveProductToPackaging);
			float _waitBeforeMovingPackagingToHatch = Prefs.GetTiming(Prefs.waitBeforeMovingPackagingToHatch);
			float _timeToMovePackagingToHatch = Prefs.GetTiming(Prefs.timeToMovePackagingToHatch);
			float _waitBetweenMovingPackagingToHatch = Prefs.GetTiming(Prefs.waitBetweenMovingPackagingToHatch);

			Melon<Mod>.Logger.Msg("Packaging station task started");

			if(Utils.NullCheck([packagingStationCanvas, packagingStationCanvas?.PackagingStation], "Can't find packaging station - probably exited task"))
				yield break;

			packagingStation = packagingStationCanvas.PackagingStation;

			yield return new WaitForSeconds(_waitBeforeStartingPackagingTask);

			GetIsPackagingStationInUse(packagingStation, out isInUse, ref isError);

			if(isError || !isInUse) {
				Melon<Mod>.Logger.Msg("Can't find packaging station - probably exited task");
				yield break;
			}

			int productsInPackaging;

			while(true) {
				GetIsPackagingStationInUse(packagingStation, out isInUse, ref isError);

				if(isError) {
					Melon<Mod>.Logger.Msg("Can't find packaging station - probably exited task");
					yield break;
				}

				if(!isInUse)
					break;

				foreach(FunctionalProduct product in packagingStation.Container.GetComponentsInImmediateChildren<FunctionalProduct>()) {
					Melon<Mod>.Logger.Msg("Moving product to packaging");

					GetIsPackagingStationInUse(packagingStation, out isInUse, ref isError);

					if(isError || !isInUse) {
						Melon<Mod>.Logger.Msg("Can't find packaging station - probably exited task");
						yield break;
					}

					packaging = packagingStation.Container.GetComponentInChildren<FunctionalPackaging>();

					if(Utils.NullCheck(packaging, "Can't find packaging - probably exited task"))
						yield break;

					moveToPosition = packaging.gameObject.transform.position;
					moveToPosition.y += 0.3f;

					productsInPackaging = packaging.PackedProducts.Count;

					isError = false;

					yield return Utils.SinusoidalLerpPositionCoroutine(product.gameObject.transform, moveToPosition, _timeToMoveProductToPackaging, () => isError = true);

					if(isError) {
						Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Waiting for packaging's contents to update");

					stepComplete = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						GetIsPackagingStationInUse(packagingStation, out isInUse, ref isError);

						if(isError || Utils.NullCheck(packaging) || !isInUse) {
							Melon<Mod>.Logger.Msg("Can't find packaging - probably exited task");
							yield break;
						}

						if(packaging.PackedProducts.Count > productsInPackaging) {
							if(packaging.IsFull) {
								Melon<Mod>.Logger.Msg("Packaging is full - closing packaging");
								packaging.Seal();
							} else {
								Melon<Mod>.Logger.Msg("Packaging's contents updated");
							}

							stepComplete = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Packaging's contents didn't update after 3 seconds");
						yield break;
					}

					yield return new WaitForSeconds(_waitBeforeMovingPackagingToHatch);

					GetIsPackagingStationInUse(packagingStation, out isInUse, ref isError);

					if(isError || Utils.NullCheck([packagingStation.OutputCollider, packaging]) || !isInUse) {
						Melon<Mod>.Logger.Msg("Can't find packaging - probably exited task");
						yield break;
					}

					if(packaging.IsSealed) {
						Melon<Mod>.Logger.Msg("Moving packaging to hatch");

						moveToPosition = new Vector3(packagingStation.OutputCollider.transform.position.x, packaging.gameObject.transform.position.y, packagingStation.OutputCollider.transform.position.z);

						isError = false;

						yield return Utils.SinusoidalLerpPositionCoroutine(packaging.gameObject.transform, moveToPosition, _timeToMovePackagingToHatch, () => isError = true);

						if(isError) {
							Melon<Mod>.Logger.Msg("Can't find packaging to move - probably exited task");
							yield break;
						}

						yield return new WaitForSeconds(_waitBetweenMovingPackagingToHatch);

						GetIsPackagingStationInUse(packagingStation, out isInUse, ref isError);

						if(isError || Utils.NullCheck([packagingStationCanvas, packagingStationCanvas?.BeginButton])) {
							Melon<Mod>.Logger.Msg("Can't find packaging station - probably exited task");
							yield break;
						}

						if(!isInUse) {
							if(packagingStationCanvas.BeginButton.currentSelectionState == UnityEngine.UI.Selectable.SelectionState.Normal) {
								Melon<Mod>.Logger.Msg("Probably exited task");
							} else {
								Melon<Mod>.Logger.Msg("Done packaging");
							}

							yield break;
						}
					}
				}

				yield return null;
			}
		}

		private static void GetIsPackagingStationInUse(PackagingStation packagingStation, out bool isInUse, ref bool isError) {
			if(Utils.NullCheck([packagingStation, packagingStation?.Container])) {
				isError = true;
				isInUse = false;
				return;
			}

			isError = false;
			isInUse = packagingStation.Container.childCount > 0;
		}
	}
}
