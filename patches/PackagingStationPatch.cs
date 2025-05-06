using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Packaging;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	[HarmonyPatch(typeof(PackagingStation), "StartTask")]
	internal static class PackagingStationPatch {
		private static void Postfix(PackagingStation __instance) {
			if(Prefs.packagingStationToggle.Value) {
				MelonCoroutines.Start(AutomatePackagingStationCoroutine(__instance));
			} else {
				Melon<Mod>.Logger.Msg("Automate packaging station disabled in settings");
			}
		}

		static System.Collections.IEnumerator AutomatePackagingStationCoroutine(PackagingStation packagingStation) {
			FunctionalPackaging packaging;
			Vector3 moveToPosition;
			bool stepComplete;
			bool callbackError;
			float time;

			float _waitBeforeStartingPackagingTask = Prefs.GetTiming(Prefs.waitBeforeStartingPackagingTask);
			float _timeToMoveProductToPackaging = Prefs.GetTiming(Prefs.timeToMoveProductToPackaging);
			float _waitBeforeMovingPackagingToHatch = Prefs.GetTiming(Prefs.waitBeforeMovingPackagingToHatch);
			float _timeToMovePackagingToHatch = Prefs.GetTiming(Prefs.timeToMovePackagingToHatch);
			float _waitAfterMovingPackagingToHatch = Prefs.GetTiming(Prefs.waitAfterMovingPackagingToHatch);

			Melon<Mod>.Logger.Msg("Packaging station task started");

			yield return new WaitForSeconds(_waitBeforeStartingPackagingTask);

			if(Utils.NullCheck([packagingStation, packagingStation.Container], "Can't find packaging station - probably exited task"))
				yield break;

			if(!IsPackagingStationInUse(packagingStation)) {
				Melon<Mod>.Logger.Msg("Probably exited task");
				yield break;
			}

			int productsInPackaging;

			while(IsPackagingStationInUse(packagingStation)) {
				foreach(FunctionalProduct product in packagingStation.Container.GetComponentsInImmediateChildren<FunctionalProduct>()) {
					if(Utils.NullCheck([packagingStation, packagingStation.Container])) {
						Melon<Mod>.Logger.Msg("Can't find packaging station - probably exited task");
						yield break;
					}

					if(!IsPackagingStationInUse(packagingStation)) {
						Melon<Mod>.Logger.Msg("Probably exited task");
						yield break;
					}

					packaging = packagingStation.Container.GetComponentInChildren<FunctionalPackaging>();

					if(Utils.NullCheck(packaging, "Can't find packaging - probably exited task"))
						yield break;

					Melon<Mod>.Logger.Msg("Moving product to packaging");

					moveToPosition = packaging.gameObject.transform.position;
					moveToPosition.y += 0.3f;

					productsInPackaging = packaging.PackedProducts.Count;

					callbackError = false;

					yield return Utils.SinusoidalLerpPositionCoroutine(product.gameObject.transform, moveToPosition, _timeToMoveProductToPackaging, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Updating packaging's contents");

					stepComplete = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						if(Utils.NullCheck([packagingStation, packaging], "Can't find packaging - probably exited task"))
							yield break;

						if(!IsPackagingStationInUse(packagingStation)) {
							Melon<Mod>.Logger.Msg("Probably exited task");
							yield break;
						}

						if(packaging.PackedProducts.Count > productsInPackaging) {
							if(packaging.IsFull) {
								Melon<Mod>.Logger.Msg("Packaging is full - closing packaging");
								packaging.Seal();
							} else {
								Melon<Mod>.Logger.Msg("Packaging's contents increased");
							}

							stepComplete = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Packaging's contents didn't increase after 3 seconds");
						yield break;
					}

					yield return new WaitForSeconds(_waitBeforeMovingPackagingToHatch);

					if(Utils.NullCheck([packagingStation, packagingStation.OutputCollider, packaging], "Can't find packaging - probably exited task"))
						yield break;

					if(!IsPackagingStationInUse(packagingStation)) {
						Melon<Mod>.Logger.Msg("Probably exited task");
						yield break;
					}

					if(packaging.IsSealed) {
						Melon<Mod>.Logger.Msg("Moving packaging to hatch");

						moveToPosition = new Vector3(packagingStation.OutputCollider.transform.position.x, packaging.gameObject.transform.position.y, packagingStation.OutputCollider.transform.position.z);

						callbackError = false;

						yield return Utils.SinusoidalLerpPositionCoroutine(packaging.gameObject.transform, moveToPosition, _timeToMovePackagingToHatch, () => callbackError = true);

						if(callbackError) {
							Melon<Mod>.Logger.Msg("Can't find packaging to move - probably exited task");
							yield break;
						}

						yield return new WaitForSeconds(_waitAfterMovingPackagingToHatch);

						if(Utils.NullCheck([packagingStation, packagingStation.Container], "Can't find packaging station - probably exited task"))
							yield break;

						if(!IsPackagingStationInUse(packagingStation)) {
							Melon<Mod>.Logger.Msg("Done packaging");
							yield break;
						}
					}
				}

				yield return null;
			}
		}

		private static bool IsPackagingStationInUse(PackagingStation packagingStation) {
			return packagingStation.Container.childCount > 0;
		}
	}
}
