using HarmonyLib;
using Il2CppScheduleOne.Packaging;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	[HarmonyPatch(typeof(PackagingStationMk2), "StartTask")]
	internal static class PackagingStationMk2Patch {
		private static void Postfix(PackagingStationMk2 __instance) {
			if(Prefs.packagingStationMk2Toggle.Value) {
				MelonCoroutines.Start(AutomatePackagingStationMk2Coroutine(__instance));
			} else {
				Melon<Mod>.Logger.Msg("Automate packaging MK2 station disabled in settings");
			}
		}

		static System.Collections.IEnumerator AutomatePackagingStationMk2Coroutine(PackagingStationMk2 packagingStationMk2) {
			PackagingTool packagingTool;
			FunctionalPackaging functionalPackaging = null;
			int maxProductsInPackaging;
			int numFinishedPackaging;
			bool stepComplete;
			float time;

			float _waitBeforeStartingPackagingMk2Task = Prefs.GetTiming(Prefs.waitBeforeStartingPackagingMk2Task);

			Melon<Mod>.Logger.Msg("Packaging station Mk2 task started");

			yield return new WaitForSeconds(_waitBeforeStartingPackagingMk2Task);

			if(Utils.NullCheck(packagingStationMk2, "Can't find packaging station Mk2 - probably exited task"))
				yield break;

			if(!IsPackagingStationMk2InUse(packagingStationMk2)) {
				Melon<Mod>.Logger.Msg("Probably exited task");
				yield break;
			}

			packagingTool = packagingStationMk2.GetComponentInChildren<PackagingTool>();

			if(Utils.NullCheck([packagingTool, packagingTool.PackagingContainer], "Can't find packaging tool - probably exited task"))
				yield break;

			while(packagingTool.ProductInHopper > 0) {
				if(Utils.NullCheck([packagingStationMk2, packagingTool, packagingTool.PackagingContainer], "Can't find packaging tool - probably exited task"))
					yield break;

				if(!IsPackagingStationMk2InUse(packagingStationMk2)) {
					Melon<Mod>.Logger.Msg("Probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Dropping product");

				packagingTool.DropProduct();

				Melon<Mod>.Logger.Msg("Waiting for packaging's contents to update");

				maxProductsInPackaging = functionalPackaging?.GetComponentsInChildren<FunctionalProduct>().Length ?? 0;

				stepComplete = false;
				time = 0;

				//Up to 3 seconds
				while(time < 3) {
					if(Utils.NullCheck([packagingTool, packagingTool.PackagingContainer], "Can't find packaging tool - probably exited task"))
						yield break;

					if(!IsPackagingStationMk2InUse(packagingStationMk2)) {
						Melon<Mod>.Logger.Msg("Probably exited task");
						yield break;
					}

					functionalPackaging = packagingTool.PackagingContainer.GetComponentsInChildren<FunctionalPackaging>().FirstOrDefault(fp => fp.GetComponentsInChildren<FunctionalProduct>().Length > maxProductsInPackaging);

					if(!Utils.NullCheck(functionalPackaging)) {
						Melon<Mod>.Logger.Msg("Packaging's contents incremented");
						stepComplete = true;
						break;
					}

					time += Time.deltaTime;

					yield return null;
				}

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Packaging's contents didn't increment after 3 seconds");
					yield break;
				}

				if(functionalPackaging.IsFull) {
					Melon<Mod>.Logger.Msg("Packaging is full - rotating conveyor");

					functionalPackaging = null;
					maxProductsInPackaging = 0;

					numFinishedPackaging = packagingTool.FinalizedPackaging.Count;

					stepComplete = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						if(Utils.NullCheck([packagingStationMk2, packagingTool, packagingTool.PackagingContainer], "Can't find packaging tool - probably exited task"))
							yield break;

						if(!IsPackagingStationMk2InUse(packagingStationMk2)) {
							Melon<Mod>.Logger.Msg("Probably exited task");
							yield break;
						}

						packagingTool.conveyorVelocity = 1f;

						if(packagingTool.finalizeCoroutine != null) {
							Melon<Mod>.Logger.Msg("Full packaging kicked to hatch");
							stepComplete = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Full packaging wasn't kicked into hatch after 3 seconds");
						yield break;
					}

					stepComplete = false;
					time = 0;

					while(time < 3) {
						if(Utils.NullCheck([packagingStationMk2, packagingTool], "Can't find packaging tool - probably exited task"))
							yield break;

						if(!IsPackagingStationMk2InUse(packagingStationMk2)) {
							Melon<Mod>.Logger.Msg("Probably exited task");
							yield break;
						}

						if(packagingTool.finalizeCoroutine == null) {
							stepComplete = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete) {
						Melon<Mod>.Logger.Msg("Kick animation didn't end after 3 seconds");
						yield break;
					}

					if(packagingTool.PackagingContainer.childCount == 0) {
						Melon<Mod>.Logger.Msg("Done packaging or exited task");
						stepComplete = true;
						break;
					}
				}
			}
		}

		private static bool IsPackagingStationMk2InUse(PackagingStationMk2 packagingStationMk2) {
			return packagingStationMk2.visualsLocked;
		}
	}
}
