using HarmonyLib;
using Il2CppGameKit.Utilities;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.Interaction;
using Il2CppScheduleOne.Networking;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.ObjectScripts.Soil;
using Il2CppScheduleOne.Packaging;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerTasks;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Property.Utilities.Water;
using Il2CppScheduleOne.StationFramework;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Stations;
using Il2CppSteamworks;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[assembly: MelonInfo(typeof(AutomatedTasksMod.Mod), "AutomatedTasksMod", "0.2.0", "Robert Rioja")]
[assembly: MelonColor(1, 255, 20, 147)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace AutomatedTasksMod {
	public class Mod : MelonMod {
		[HarmonyPatch(typeof(InputPromptsCanvas), "LoadModule", [typeof(string)])]
		public static class HarvestPatch {
			private static void Postfix(InputPromptsCanvas __instance) {
				switch(__instance.currentModuleLabel) {
					case "pourable":
						MelonCoroutines.Start(AutomatePourTask());
						break;
					case "harvestplant":
						MelonCoroutines.Start(AutomateHarvestingCoroutine());
						break;
				}
			}

			static System.Collections.IEnumerator AutomatePourTask() {
				yield return new WaitForSeconds(0.5f);

				PourableSoil soil = GameObject.FindObjectsOfType<PourableSoil>().FirstOrDefault(p => p.TargetPot?.PlayerUserObject.GetComponent<Player>()?.IsLocalPlayer ?? false);

				if(!NullCheck(soil)) {
					yield return AutomatePouringSoilCoroutine(soil);
					yield break;
				}

				FunctionalSeed seed = GameObject.FindObjectOfType<FunctionalSeed>();

				if(!NullCheck(seed)) {
					yield return AutomateSowingSeedCoroutine(seed);
					yield break;
				}

				FunctionalWateringCan wateringCan = GameObject.FindObjectOfType<FunctionalWateringCan>();

				if(!NullCheck(wateringCan)) {
					yield return AutomatePouringWaterCoroutine(wateringCan);
					yield break;
				}

				PourableAdditive fertilizer = GameObject.FindObjectsOfType<PourableAdditive>().FirstOrDefault(m => m.TargetPot?.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

				if(!NullCheck(fertilizer)) {
					yield return AutomatePouringFertilizerCoroutine(fertilizer);
					yield break;
				}

				Melon<Mod>.Logger.Msg("Probably exited task");
			}

			static System.Collections.IEnumerator AutomatePouringSoilCoroutine(PourableSoil soil) {
				bool stepComplete;
				bool callbackError;

				stepComplete = false;

				Melon<Mod>.Logger.Msg("Pour soil task started");

				//It shouldn't take more than 10 cuts so this is a failsafe
				for(int i = 0; i < 10; i++) {
					if(NullCheck([soil, soil.TargetPot], "Can't find soil - probably exited task"))
						yield break;

					if(!soil.TargetPot.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
						Melon<Mod>.Logger.Msg("Soil isn't associated with player - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Cutting open soil");
					soil.Cut();

					if(soil.IsOpen) {
						Melon<Mod>.Logger.Msg("Done opening soil");
						stepComplete = true;
						break;
					}

					yield return new WaitForSeconds(0.1f);
				}

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Cutting open soil didn't complete after 20 attempts");
					yield break;
				}

				yield return new WaitForSeconds(0.2f);

				Melon<Mod>.Logger.Msg("Pouring soil");

				if(NullCheck([soil, soil.TargetPot], "Can't find soil - probably exited task"))
					yield break;

				if(!soil.TargetPot.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
					Melon<Mod>.Logger.Msg("Soil isn't associated with player - probably exited task");
					yield break;
				}

				callbackError = false;

				yield return SinusoidalLerpRotationCoroutine(soil.transform, new Vector3(soil.transform.localEulerAngles.x, soil.transform.localEulerAngles.y, soil.transform.localEulerAngles.z - 180), 1.5f, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find soil to rotate - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Done pouring soil");
			}

			static System.Collections.IEnumerator AutomateSowingSeedCoroutine(FunctionalSeed seed) {
				Pot pot;
				Vector3 moveToPosition;
				bool callbackError;

				Melon<Mod>.Logger.Msg("Sow seed task started");
				Melon<Mod>.Logger.Msg("Moving and rotating seed vial");

				if(NullCheck(seed.Vial, "Can't find seed vial - probably exited task"))
					yield break;

				moveToPosition = seed.Vial.transform.position;
				moveToPosition.y -= 0.1f;

				seed.Vial.transform.localEulerAngles = Vector3.zero;

				callbackError = false;

				yield return SinusoidalLerpPositionAndRotationCoroutine(seed.Vial.transform, moveToPosition, new Vector3(seed.Vial.transform.localEulerAngles.x + 180, seed.Vial.transform.localEulerAngles.y, seed.Vial.transform.localEulerAngles.z), 1.5f, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find seed vial to move and rotate - probably exited task");
					yield break;
				}

				yield return new WaitForSeconds(0.2f);

				if(NullCheck([seed, seed.Cap], "Can't find seed cap - probably exited task"))
					yield break;

				Melon<Mod>.Logger.Msg("Popping seed cap");
				seed.Cap.Pop();

				yield return new WaitForSeconds(0.5f);

				pot = GameObject.FindObjectsOfType<Pot>().FirstOrDefault(p => p.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

				if(NullCheck(pot, "Can't find the pot the player is using")) {
					yield break;
				}

				foreach(SoilChunk soilChunk in pot.SoilChunks) {
					if(NullCheck(pot, "Can't find pot - probably exited task"))
						yield break;

					if(NullCheck(soilChunk, "Can't find soil chunk - probably exited task"))
						yield break;

					if(!pot.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
						Melon<Mod>.Logger.Msg("Pot isn't associated with player - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Moving soil chunk");
					soilChunk.StartClick(new RaycastHit());

					yield return new WaitForSeconds(0.5f);
				}

				Melon<Mod>.Logger.Msg("Done sowing seed");
			}

			static System.Collections.IEnumerator AutomatePouringWaterCoroutine(FunctionalWateringCan wateringCan) {
				Vector3 moveToPosition;
				Pot pot;
				bool stepComplete;
				bool stepComplete2;
				bool callbackError;
				float time;

				Melon<Mod>.Logger.Msg("Water soil task started");
				Melon<Mod>.Logger.Msg("Rotating watering can");

				callbackError = false;

				yield return SinusoidalLerpRotationCoroutine(wateringCan.transform, new Vector3(wateringCan.transform.localEulerAngles.x, wateringCan.transform.localEulerAngles.y, wateringCan.transform.localEulerAngles.z - 90), 0.8f, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find watering can to rotate - probably exited task");
					yield break;
				}

				stepComplete = false;
				Vector3 targetPosition;

				//There shouldn't be more than 10 watering spots so this is a failsafe
				for(int i = 0; i < 10; i++) {
					if(NullCheck(wateringCan, "Can't find watering can - probably exited task"))
						yield break;

					pot = wateringCan.TargetPot;

					if(NullCheck(pot, "Can't find watering can's pot - probably exited task"))
						yield break;

					if(!pot.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
						Melon<Mod>.Logger.Msg("Watering can's pot isn't associated with player - probably exited task");
						yield break;
					}

					if(NullCheck(wateringCan.PourPoint, "Can't find watering can pour point - probably exited task"))
						yield break;

					if(NullCheck(pot.Target, "Can't find watering can's pot's target - probably exited task"))
						yield break;

					Melon<Mod>.Logger.Msg("Moving watering can");

					targetPosition = pot.Target.position;

					moveToPosition = new Vector3(wateringCan.transform.position.x - (wateringCan.PourPoint.position.x - targetPosition.x), wateringCan.transform.position.y, wateringCan.transform.position.z - (wateringCan.PourPoint.position.z - targetPosition.z));

					callbackError = false;

					yield return SinusoidalLerpPositionCoroutine(wateringCan.transform, moveToPosition, 0.8f, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find watering can to move - probably exited task");
						yield break;
					}

					time = 0;
					stepComplete2 = false;

					//Up to 5 seconds
					while(time < 5) {
						if(NullCheck([pot, pot.Target], "Can't find watering can's pot's target - probably exited task"))
							yield break;

						if(pot.Target.position != targetPosition) {
							Melon<Mod>.Logger.Msg("Done watering target");
							stepComplete2 = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete2) {
						Melon<Mod>.Logger.Msg("Watering target didn't complete after 5 seconds");
						yield break;
					}

					if(pot.WaterLevel > pot.WaterCapacity - pot.WaterDrainPerHour) {
						Melon<Mod>.Logger.Msg("Done watering");
						stepComplete = true;
						yield break;
					}
				}

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Watering didn't complete after 10 attempts");
					yield break;
				}
			}

			static System.Collections.IEnumerator AutomatePouringFertilizerCoroutine(PourableAdditive fertilizer) {
				Pot pot;
				Vector3 targetPosition;
				Vector3 moveToPosition;
				bool stepComplete;
				bool callbackError;

				Melon<Mod>.Logger.Msg("Pour fertilizer task started");
				Melon<Mod>.Logger.Msg("Rotating fertilizer");

				callbackError = false;

				yield return SinusoidalLerpRotationCoroutine(fertilizer.transform, new Vector3(fertilizer.transform.localEulerAngles.x, fertilizer.transform.localEulerAngles.y, fertilizer.transform.localEulerAngles.z - 180), 1, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find fertilizer to rotate - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Pouring fertilizer");

				if(NullCheck([fertilizer, fertilizer.TargetPot], "Can't find fertilizer - probably exited task"))
					yield break;

				pot = fertilizer.TargetPot;

				int angle = 0;
				stepComplete = false;

				for(float r = 0.01f; r < pot.PotRadius; r *= Mathf.Max(0.18f / (r + 0.1f), 1.02f)) {
					if(NullCheck([fertilizer, pot], "Can't find fertilizer - probably exited task"))
						yield break;

					if(!pot.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
						Melon<Mod>.Logger.Msg("Fertilizer's pot isn't associated with player - probably exited task");
						yield break;
					}

					targetPosition = new Vector3(pot.PourableStartPoint.position.x + (Mathf.Sin(angle * Mathf.Deg2Rad) * r), 0, pot.PourableStartPoint.position.z + (Mathf.Cos(angle * Mathf.Deg2Rad) * r));

					moveToPosition = new Vector3(fertilizer.transform.position.x - (fertilizer.PourPoint.position.x - targetPosition.x), fertilizer.transform.position.y, fertilizer.transform.position.z - (fertilizer.PourPoint.position.z - targetPosition.z));

					callbackError = false;

					yield return LerpPositionCoroutine(fertilizer.transform, moveToPosition, 0.10f, () => callbackError = true);

					if(callbackError) {
						if(NullCheck(pot, "Can't find fertilizer's pot - probably exited task"))
							yield break;

						if(pot.AppliedAdditives.Count > 0) {
							Melon<Mod>.Logger.Msg("Done pouring fertilizer");
						} else {
							Melon<Mod>.Logger.Msg("Can't find fertilizer to move - probably exited task");
						}

						yield break;
					}

					angle += 20;
				}

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Pouring fertilizer did not complete after reaching the pot's radius");
					yield break;
				}
			}

			static System.Collections.IEnumerator AutomateHarvestingCoroutine() {
				Pot usingPot;
				PlantHarvestable harvestable;

				Melon<Mod>.Logger.Msg("Harvest task started");

				yield return new WaitForSeconds(0.5f);

				usingPot = GameObject.FindObjectsOfType<Pot>().FirstOrDefault(p => p.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

				if(NullCheck(usingPot, "Can't find the pot the player is using - probably exited task"))
					yield break;

				//We can never have more than 20 harvestables so this is a failsafe
				for(int i = 0; i < 20; i++) {
					if(!usingPot.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
						Melon<Mod>.Logger.Msg("Pot isn't associated with player - probably exited task");
						yield break;
					}

					harvestable = usingPot.GetComponentInChildren<PlantHarvestable>();

					//This should never trigger but we check to be safe
					if(NullCheck(harvestable, "Done harvesting"))
						yield break;

					Melon<Mod>.Logger.Msg("Harvesting plant piece");
					harvestable.Harvest();

					if(NullCheck(usingPot.GetComponentInChildren<PlantHarvestable>(), "Done harvesting"))
						yield break;

					yield return new WaitForSeconds(0.5f);
				}
			}
		}

		[HarmonyPatch(typeof(Tap), "Interacted")]
		public static class TapPatch {
			private static void Postfix(Tap __instance) {
				MelonCoroutines.Start(AutomateTapCoroutine(__instance));
			}

			static System.Collections.IEnumerator AutomateTapCoroutine(Tap tap) {
				Melon<Mod>.Logger.Msg("Sink task started");

				yield return new WaitForSeconds(0.5f);

				if(NullCheck(tap, "Can't find the tap the player is using"))
					yield break;

				if(!tap.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? true) {
					Melon<Mod>.Logger.Msg("Tap isn't associated with player - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Holding open tap");
				tap.IsHeldOpen = true;
			}
		}

		[HarmonyPatch(typeof(PackagingStation), "StartTask")]
		public static class PackagingStationPatch {
			private static void Postfix(PackagingStation __instance) {
				MelonCoroutines.Start(AutomatePackagingStationCoroutine(__instance));
			}

			static System.Collections.IEnumerator AutomatePackagingStationCoroutine(PackagingStation packagingStation) {
				int numProduct, numBaggies;
				FunctionalProduct product;
				FunctionalPackaging packaging;
				Vector3 moveToPosition;
				bool stepComplete;
				bool callbackError;
				float time;

				Melon<Mod>.Logger.Msg("Packaging station task started");

				yield return new WaitForSeconds(0.5f);

				int productsInPackaging;

				//We can only ever have 20 products so this is a failsafe
				for(int i = 0; i < 20; i++) {
					if(NullCheck([packagingStation, packagingStation.Container], "Can't find packaging station - probably exited task"))
						yield break;

					if(!IsPackagingStationInUse(packagingStation)) {
						Melon<Mod>.Logger.Msg("Probably exited task");
						yield break;
					}

					packaging = packagingStation.Container.GetComponentInChildren<FunctionalPackaging>();

					if(NullCheck(packaging, "Can't find packaging - probably exited task"))
						yield break;

					numProduct = packagingStation.GetComponentsInChildren<FilledPackagingVisuals>().Count;
					//Add 1 for the active packaging
					numBaggies = packagingStation.transform.Find("PackagingAlignments").GetComponentsInChildren<Rigidbody>().Count + 1;

					if(ZeroCheck(numProduct, "Ran out of product - stopping"))
						yield break;

					if(ZeroCheck(numBaggies, "Ran out of packaging - stopping"))
						yield break;

					Melon<Mod>.Logger.Msg("Moving product to packaging");

					product = packagingStation.Container.GetComponentInImmediateChildren<FunctionalProduct>();

					if(NullCheck(product, "Can't find product - probably exited task"))
						yield break;

					moveToPosition = packaging.gameObject.transform.position;
					moveToPosition.y += 0.3f;

					productsInPackaging = packaging.PackedProducts.Count;

					callbackError = false;

					yield return SinusoidalLerpPositionCoroutine(product.gameObject.transform, moveToPosition, 0.5f, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Updating packaging's contents");

					stepComplete = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						if(NullCheck(packaging, "Can't find packaging - probably exited task"))
							yield break;

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

					yield return new WaitForSeconds(0.2f);

					if(NullCheck([packagingStation, packagingStation.OutputCollider, packaging], "Can't find packaging - probably exited task"))
						yield break;

					if(!IsPackagingStationInUse(packagingStation)) {
						Melon<Mod>.Logger.Msg("Probably exited task");
						yield break;
					}

					if(packaging.IsSealed) {
						Melon<Mod>.Logger.Msg("Moving packaging to hatch");

						moveToPosition = new Vector3(packagingStation.OutputCollider.transform.position.x, packaging.gameObject.transform.position.y, packaging.gameObject.transform.position.z);

						callbackError = false;

						yield return SinusoidalLerpPositionCoroutine(packaging.gameObject.transform, moveToPosition, 0.3f, () => callbackError = true);

						if(callbackError) {
							Melon<Mod>.Logger.Msg("Can't find packaging to move - probably exited task");
							yield break;
						}

						yield return new WaitForSeconds(0.8f);

						if(!IsPackagingStationInUse(packagingStation)) {
							Melon<Mod>.Logger.Msg("Done packaging");
							yield break;
						}
					}
				}
			}
		}

		[HarmonyPatch(typeof(MixingStationCanvas), "BeginButtonPressed")]
		public static class MixingStationCanvasPatch {
			private static void Postfix(MixingStationCanvas __instance) {
				MelonCoroutines.Start(AutomateMixingStationCoroutine());
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

				Melon<Mod>.Logger.Msg("Mixing station task started");

				yield return new WaitForSeconds(0.5f);

				mixingStation = GameObject.FindObjectsOfType<MixingStation>().FirstOrDefault(m => m.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

				if(NullCheck(mixingStation, "Can't find the mixing station the player is using"))
					yield break;

				if(!IsMixingStationInUse(mixingStation)) {
					Melon<Mod>.Logger.Msg("Probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Moving products");

				//We can only ever have up to 20 products so we want to loop one extra to trigger the done message
				for(int i = 0; i < 21; i++) {
					if(NullCheck([mixingStation, mixingStation.ItemContainer, mixingStation.BowlFillable], "Can't find mixing station - probably exited task"))
						yield break;

					if(!IsMixingStationInUse(mixingStation)) {
						Melon<Mod>.Logger.Msg("Probably exited task");
						yield break;
					}

					if(mixingStation.ItemContainer.childCount <= i) {
						Melon<Mod>.Logger.Msg("Done moving products");
						break;
					}

					product = mixingStation.ItemContainer.GetChild(i);

					if(NullCheck(product, "Can't find product - probably exited task"))
						yield break;

					productPiece = product.GetComponentInChildren<IngredientPiece>();
					productBeaker = product.GetComponentInChildren<Beaker>();

					if(!NullCheck(productPiece)) {
						moveToPosition = mixingStation.BowlFillable.transform.position;
						moveToPosition.y += 0.3f;

						Melon<Mod>.Logger.Msg("Moving product to mixer");

						callbackError = false;

						yield return SinusoidalLerpPositionCoroutine(productPiece.transform, moveToPosition, 0.5f, () => callbackError = true);

						if(callbackError) {
							Melon<Mod>.Logger.Msg("Can't find product piece to move - probably exited task");
							yield break;
						}

						yield return new WaitForSeconds(0.3f);
					} else if(!NullCheck(productBeaker)) {
						Melon<Mod>.Logger.Msg("Moving beaker to mixer");

						moveBackToPosition = productBeaker.transform.position;

						moveToPosition = productBeaker.transform.position.Between(mixingStation.BowlFillable.transform.position, 0.3f);
						moveToPosition.y += 0.35f;

						callbackError = false;

						yield return SinusoidalLerpPositionCoroutine(productBeaker.transform, moveToPosition, 0.8f, () => callbackError = true);

						if(callbackError) {
							Melon<Mod>.Logger.Msg("Can't find beaker - probably exited task");
							yield break;
						}

						Melon<Mod>.Logger.Msg("Rotating beaker");

						productBeaker.transform.localEulerAngles = Vector3.zero;
						rotateToAngles = new Vector3(productBeaker.transform.localEulerAngles.x, productBeaker.transform.localEulerAngles.x, productBeaker.transform.localEulerAngles.y + 90);

						yield return SinusoidalLerpPositionAndRotationCoroutine(productBeaker.transform, moveToPosition, rotateToAngles, 2f, () => callbackError = true);

						if(callbackError) {
							Melon<Mod>.Logger.Msg("Can't find beaker to move and rotate - probably exited task");
							yield break;
						}

						Melon<Mod>.Logger.Msg("Holding beaker");

						stepComplete = false;
						time = 0;

						//Up to 5 seconds
						while(time < 5) {
							if(NullCheck(productBeaker, "Can't find beaker - probably exited task"))
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

						yield return SinusoidalLerpPositionAndRotationCoroutine(productBeaker.transform, moveBackToPosition, Vector3.zero, 0.8f, () => callbackError = true);

						if(callbackError) {
							Melon<Mod>.Logger.Msg("Can't find beaker to move and rotate - probably exited task");
							yield break;
						}

						yield return new WaitForSeconds(0.3f);
					} else {
						Melon<Mod>.Logger.Msg("Can't find product piece or beaker - probably exited task");
						yield break;
					}
				}

				yield return new WaitForSeconds(0.5f);

				if(NullCheck([mixingStation, mixingStation.StartButton], "Can't find mixing station start button - probably exited task"))
					yield break;

				if(!IsMixingStationInUse(mixingStation)) {
					Melon<Mod>.Logger.Msg("Probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Pressing start button");

				mixingStation.StartButton.StartClick(new RaycastHit());

				Melon<Mod>.Logger.Msg("Done mixing");
			}
		}

		[HarmonyPatch(typeof(ChemistryStationCanvas), "BeginButtonPressed")]
		public static class ChemistryStationCanvasPatch {
			private static void Postfix(ChemistryStationCanvas __instance) {
				MelonCoroutines.Start(AutomateChemistryStationCoroutine());
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

				Melon<Mod>.Logger.Msg("Chemistry station task started");

				yield return new WaitForSeconds(0.5f);

				chemistryStation = GameObject.FindObjectsOfType<ChemistryStation>().FirstOrDefault(m => m.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

				if(NullCheck(chemistryStation, "Can't find the chemistry station the player is using"))
					yield break;

				beaker = chemistryStation.GetComponentInChildren<Beaker>();

				if(NullCheck(beaker, "Can't find beaker - probably exited task"))
					yield break;

				Melon<Mod>.Logger.Msg("Moving ingredients to beaker");

				foreach(IngredientPiece ingredient in chemistryStation.ItemContainer.transform.GetComponentsInChildren<IngredientPiece>()) {
					if(NullCheck(ingredient, "Can't find ingredient - probably exited task"))
						yield break;

					if(NullCheck(beaker, "Can't find beaker - probably exited task"))
						yield break;

					moveToPosition = beaker.transform.position;
					moveToPosition.y += 0.4f;

					Melon<Mod>.Logger.Msg("Moving ingredient to beaker");

					callbackError = false;

					yield return SinusoidalLerpPositionCoroutine(ingredient.transform, moveToPosition, 0.5f, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find ingredient to move - probably exited task");
						yield break;
					}

					yield return new WaitForSeconds(0.3f);
				}

				Melon<Mod>.Logger.Msg("Done moving ingredients to beaker");
				Melon<Mod>.Logger.Msg("Pouring ingredients");

				foreach(PourableModule pourable in chemistryStation.ItemContainer.transform.GetComponentsInChildren<PourableModule>()) {
					if(NullCheck(pourable, "Can't find pourable - probably exited task"))
						yield break;

					if(NullCheck(beaker, "Can't find beaker - probably exited task"))
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

					yield return SinusoidalLerpPositionCoroutine(pourable.transform, moveToPosition, 0.8f, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find pourable to move - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Rotating pourable");

					yield return SinusoidalLerpPositionAndRotationCoroutine(pourable.transform, moveToPosition, new Vector3(pourable.transform.localEulerAngles.x, pourable.transform.localEulerAngles.x, pourable.transform.localEulerAngles.y + 180), 1.5f, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find pourable to move and rotate - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Holding pourable");

					stepComplete = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						if(NullCheck(pourable, "Can't find pourable - probably exited task"))
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

					yield return SinusoidalLerpPositionAndRotationCoroutine(pourable.transform, moveBackToPosition, Vector3.zero, 0.8f, () => callbackError = true);

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find pourable to move and rotate - probably exited task");
						yield break;
					}

					yield return new WaitForSeconds(0.3f);
				}

				Melon<Mod>.Logger.Msg("Moving stirring rod");

				if(NullCheck(chemistryStation, "Can't find chemistry station - probably exited task"))
					yield break;

				stirringRod = chemistryStation.GetComponentInChildren<StirringRod>();

				if(NullCheck(stirringRod, "Can't find stirring rod - probably exited task"))
					yield break;

				stepComplete = false;
				time = 0;
				float maxTime = 8;

				//Up to 8 seconds
				while(maxTime > 0) {
					if(NullCheck(stirringRod)) {
						if(NullCheck(chemistryStation) || !IsChemistryStationInUse(chemistryStation)) {
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

						MelonCoroutines.Start(LerpRotationCoroutine(stirringRod.transform, new Vector3(stirringRod.transform.localEulerAngles.x, stirringRod.transform.localEulerAngles.y + 40, stirringRod.transform.localEulerAngles.z), 0.1f, () => callbackError = true));

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

				yield return new WaitForSeconds(0.5f);

				Melon<Mod>.Logger.Msg("Moving lab stand");

				if(NullCheck(chemistryStation, "Can't find chemistry station - probably exited task"))
					yield break;

				if(NullCheck(chemistryStation.LabStand, "Can't find lab stand - probably exited task"))
					yield break;

				callbackError = false;

				yield return LerpFloatCallbackCoroutine(1, 0, 0.5f, f => {
					if(NullCheck(chemistryStation) || NullCheck(chemistryStation.LabStand) || !IsChemistryStationInUse(chemistryStation)) {
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

				yield return new WaitForSeconds(0.5f);

				Melon<Mod>.Logger.Msg("Moving beaker to funnel");

				if(NullCheck(beaker, "Can't find beaker - probably exited station"))
					yield break;

				if(NullCheck([chemistryStation, chemistryStation.LabStand, chemistryStation.LabStand.Funnel], "Can't find lab stand - probably exited task"))
					yield break;

				moveToPosition = beaker.transform.position.Between(chemistryStation.LabStand.Funnel.transform.position, 0.3f);
				moveToPosition.y += 0.4f;

				callbackError = false;

				yield return SinusoidalLerpPositionCoroutine(beaker.transform, moveToPosition, 0.8f, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find beaker - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Rotating beaker");

				beaker.transform.localEulerAngles = Vector3.zero;
				rotateToAngles = new Vector3(beaker.transform.localEulerAngles.x, beaker.transform.localEulerAngles.x, beaker.transform.localEulerAngles.y + 90);

				yield return SinusoidalLerpPositionAndRotationCoroutine(beaker.transform, moveToPosition, rotateToAngles, 3f, () => callbackError = true);

				if(callbackError) {
					Melon<Mod>.Logger.Msg("Can't find beaker to move and rotate - probably exited task");
					yield break;
				}

				Melon<Mod>.Logger.Msg("Holding beaker");

				stepComplete = false;
				time = 0;

				//Up to 5 seconds
				while(time < 5) {
					if(NullCheck(beaker, "Can't find beaker - probably exited task"))
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

				yield return new WaitForSeconds(0.5f);

				Melon<Mod>.Logger.Msg("Moving lab stand back");

				if(NullCheck([chemistryStation, chemistryStation.LabStand], "Can't find chemistry station - probably exited task"))
					yield break;

				callbackError = false;

				yield return LerpFloatCallbackCoroutine(0, 1, 0.5f, f => {
					if(NullCheck(chemistryStation) || NullCheck(chemistryStation.LabStand) || !IsChemistryStationInUse(chemistryStation)) {
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

				yield return new WaitForSeconds(0.5f);

				Melon<Mod>.Logger.Msg("Handling burner");

				stepComplete = false;
				time = 0;

				//Up to 8 seconds
				while(time < 8) {
					if(NullCheck([chemistryStation, chemistryStation.BoilingFlask, chemistryStation.Burner], "Can't find chemistry station - probably exited task"))
						yield break;

					if(!IsChemistryStationInUse(chemistryStation)) {
						if(NullCheck(chemistryStation.CurrentCookOperation, "Probably exited task"))
							yield break;
						else {
							Melon<Mod>.Logger.Msg("Finished preparing recipe");
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
					yield break;
				}
			}
		}

		[HarmonyPatch(typeof(LabOvenCanvas), "BeginButtonPressed")]
		public static class LabOvenCanvasPatch {
			private static void Postfix(LabOvenCanvas __instance) {
				MelonCoroutines.Start(AutomateLabOvenCoroutine());
			}

			static System.Collections.IEnumerator AutomateLabOvenCoroutine() {
				LabOven labOven;
				LabOvenDoor labOvenDoor;
				LabOvenButton labOvenButton;
				bool callbackError;

				Melon<Mod>.Logger.Msg("Lab oven task started");

				yield return new WaitForSeconds(0.5f);

				labOven = GameObject.FindObjectsOfType<LabOven>().FirstOrDefault(m => m.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

				if(NullCheck(labOven, "Can't find the lab oven the player is using"))
					yield break;

				if(!labOven.IsReadyForHarvest()) {
					labOvenDoor = labOven.Door;

					if(NullCheck(labOvenDoor, "Can't find lab oven door - probably exited task"))
						yield break;

					labOvenButton = labOven.Button;

					if(NullCheck(labOvenButton, "Can't find lab oven button - probably exited task"))
						yield break;

					Melon<Mod>.Logger.Msg("Opening lab oven door");

					callbackError = false;

					yield return LerpFloatCallbackCoroutine(0, 1, 0.5f, f => {
						if(NullCheck(labOvenDoor) || NullCheck(labOven) || !IsLabOvenInUse(labOven)) {
							callbackError = true;
							return false;
						}

						labOvenDoor.TargetPosition = f;

						return true;
					});

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find lab door to open - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Done opening lab oven door");

					yield return new WaitForSeconds(3f);

					Melon<Mod>.Logger.Msg("Closing lab oven door");

					if(NullCheck(labOvenDoor, "Can't find lab oven door - probably exited task"))
						yield break;

					callbackError = false;

					yield return LerpFloatCallbackCoroutine(labOvenDoor.ActualPosition, 0, 0.5f, f => {
						if(NullCheck(labOvenDoor) || NullCheck(labOven) || !IsLabOvenInUse(labOven)) {
							callbackError = true;
							return false;
						}

						labOvenDoor.TargetPosition = f;

						return true;
					});

					if(callbackError) {
						Melon<Mod>.Logger.Msg("Can't find lab door to close - probably exited task");
						yield break;
					}

					Melon<Mod>.Logger.Msg("Done closing lab oven door");

					yield return new WaitForSeconds(0.5f);

					Melon<Mod>.Logger.Msg("Pressing lab oven button");

					if(NullCheck(labOvenButton, "Can't find lab oven button - probably exited task"))
						yield break;

					labOvenButton.Press(new RaycastHit());
				} else {
					//Couldn't figure out how to automate smashing with the hammer :(
				}
			}
		}

		[HarmonyPatch(typeof(PackagingStationMk2), "StartTask")]
		public static class PackagingStationMk2Patch {
			private static void Postfix(PackagingStationMk2 __instance) {
				MelonCoroutines.Start(AutomatePackagingStationMk2Coroutine(__instance));
			}

			static System.Collections.IEnumerator AutomatePackagingStationMk2Coroutine(PackagingStationMk2 packagingStationMk2) {
				PackagingTool packagingTool;
				FunctionalPackaging functionalPackaging = null;
				int maxProductsInPackaging;
				int numFinishedPackaging;
				bool stepComplete;
				bool stepComplete2;
				float time;

				Melon<Mod>.Logger.Msg("Packaging station MK2 task started");

				yield return new WaitForSeconds(0.5f);

				if(NullCheck(packagingStationMk2, "Can't find packaging station MK2 - probably exited task"))
					yield break;

				packagingTool = packagingStationMk2.GetComponentInChildren<PackagingTool>();

				if(NullCheck([packagingTool, packagingTool.PackagingContainer], "Can't find packaging tool - probably exited task"))
					yield break;

				stepComplete = false;

				//We can only ever have up to 20 products so this is a failsafe
				for(int i = 0; i < 20; i++) {
					Melon<Mod>.Logger.Msg("Dropping product");

					packagingTool.DropProduct();

					Melon<Mod>.Logger.Msg("Waiting for packaging's contents to update");

					maxProductsInPackaging = functionalPackaging?.GetComponentsInChildren<FunctionalProduct>().Length ?? 0;

					stepComplete2 = false;
					time = 0;

					//Up to 3 seconds
					while(time < 3) {
						if(NullCheck([packagingTool, packagingTool.PackagingContainer], "Can't find packaging tool - probably exited task"))
							yield break;

						functionalPackaging = packagingTool.PackagingContainer.GetComponentsInChildren<FunctionalPackaging>().FirstOrDefault(fp => fp.GetComponentsInChildren<FunctionalProduct>().Length > maxProductsInPackaging);

						if(!NullCheck(functionalPackaging)) {
							Melon<Mod>.Logger.Msg("Packaging's contents incremented");
							stepComplete2 = true;
							break;
						}

						time += Time.deltaTime;

						yield return null;
					}

					if(!stepComplete2) {
						Melon<Mod>.Logger.Msg("Packaging's contents didn't increment after 3 seconds");
						yield break;
					}

					if(functionalPackaging.IsFull) {
						Melon<Mod>.Logger.Msg("Packaging is full - rotating conveyor");

						functionalPackaging = null;
						maxProductsInPackaging = 0;

						numFinishedPackaging = packagingTool.FinalizedPackaging.Count;

						stepComplete2 = false;
						time = 0;

						//Up to 3 seconds
						while(time < 3) {
							if(NullCheck([packagingStationMk2, packagingTool, packagingTool.PackagingContainer], "Can't find packaging tool - probably exited task"))
								yield break;

							packagingTool.conveyorVelocity = 1f;

							if(packagingTool.finalizeCoroutine != null) {
								Melon<Mod>.Logger.Msg("Full packaging kicked to hatch");
								stepComplete2 = true;
								break;
							}

							time += Time.deltaTime;

							yield return null;
						}

						if(!stepComplete2) {
							Melon<Mod>.Logger.Msg("Full packaging wasn't kicked into hatch after 3 seconds");
							yield break;
						}

						stepComplete2 = false;
						time = 0;

						while(time < 3) {
							if(packagingTool.finalizeCoroutine == null) {
								stepComplete2 = true;
								break;
							}

							time += Time.deltaTime;

							yield return null;
						}

						if(!stepComplete2) {
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

				if(!stepComplete) {
					Melon<Mod>.Logger.Msg("Packaging didn't complete after 20 attempts");
					yield break;
				}
			}
		}

		static bool IsPackagingStationInUse(PackagingStation packagingStation) {
			return packagingStation.Container.childCount > 0;
		}

		static bool IsMixingStationInUse(MixingStation mixingStation) {
			return (mixingStation.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false) && (!GameObject.FindObjectOfType<MixingStationCanvas>()?.Canvas?.enabled ?? false);
		}

		static bool IsChemistryStationInUse(ChemistryStation chemistryStation) {
			return chemistryStation.ItemContainer.childCount > 0;
		}

		static bool IsLabOvenInUse(LabOven labOven) {
			return labOven.PourableContainer.childCount > 0;
		}

		static System.Collections.IEnumerator LerpPositionCoroutine(Transform transform, Vector3 targetPosition, float duration, Action onError = null) {
			float time = 0;
			Vector3 startPosition = transform.position;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				Vector3 pos = startPosition;

				pos.y = Mathf.Lerp(startPosition.y, targetPosition.y, t);
				pos.x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
				pos.z = Mathf.Lerp(startPosition.z, targetPosition.z, t);

				transform.position = pos;

				time += Time.deltaTime;
				yield return null;
			}
		}

		static System.Collections.IEnumerator SinusoidalLerpPositionCoroutine(Transform transform, Vector3 targetPosition, float duration, Action onError = null) {
			float time = 0;
			Vector3 startPosition = transform.position;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				float yT = Mathf.Sin(t * Mathf.PI / 2);
				float xT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);
				Vector3 pos = startPosition;

				pos.y = Mathf.Lerp(startPosition.y, targetPosition.y, yT);
				pos.x = Mathf.Lerp(startPosition.x, targetPosition.x, xT);
				pos.z = Mathf.Lerp(startPosition.z, targetPosition.z, yT);

				transform.position = pos;

				time += Time.deltaTime;
				yield return null;
			}
		}

		static System.Collections.IEnumerator LerpRotationCoroutine(Transform transform, Vector3 targetAngle, float duration, Action onError = null) {
			float time = 0;
			Vector3 startRotation = transform.localEulerAngles;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				Vector3 rot = startRotation;

				rot.x = Mathf.Lerp(startRotation.x, targetAngle.x, t);
				rot.y = Mathf.Lerp(startRotation.y, targetAngle.y, t);
				rot.z = Mathf.Lerp(startRotation.z, targetAngle.z, t);

				transform.localEulerAngles = rot;

				time += Time.deltaTime;
				yield return null;
			}
		}

		static System.Collections.IEnumerator SinusoidalLerpRotationCoroutine(Transform transform, Vector3 targetAngle, float duration, Action onError = null) {
			float time = 0;
			Vector3 startRotation = transform.localEulerAngles;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				float aT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);
				Vector3 rot = startRotation;
				
				rot.x = Mathf.Lerp(startRotation.x, targetAngle.x, aT);
				rot.y = Mathf.Lerp(startRotation.y, targetAngle.y, aT);
				rot.z = Mathf.Lerp(startRotation.z, targetAngle.z, aT);

				transform.localEulerAngles = rot;

				time += Time.deltaTime;
				yield return null;
			}
		}

		static System.Collections.IEnumerator SinusoidalLerpPositionAndRotationCoroutine(Transform transform, Vector3 targetPosition, Vector3 targetAngle, float duration, Action onError = null) {
			float time = 0;
			Vector3 startPosition = transform.position;
			Vector3 startRotation = transform.localEulerAngles;

			while(time < duration) {
				if(transform == null) {
					onError?.Invoke();
					yield break;
				}

				float t = time / duration;
				float yT = Mathf.Sin(t * Mathf.PI / 2);
				float xT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);
				float aT = ((Mathf.Cos(t * Mathf.PI) / -2) + 0.5f);

				Vector3 pos = startPosition;
				Vector3 rot = startRotation;

				pos.y = Mathf.Lerp(startPosition.y, targetPosition.y, yT);
				pos.x = Mathf.Lerp(startPosition.x, targetPosition.x, xT);
				pos.z = Mathf.Lerp(startPosition.z, targetPosition.z, yT);

				rot.x = Mathf.Lerp(startRotation.x, targetAngle.x, aT);
				rot.y = Mathf.Lerp(startRotation.y, targetAngle.y, aT);
				rot.z = Mathf.Lerp(startRotation.z, targetAngle.z, aT);

				transform.position = pos;
				transform.localEulerAngles = rot;

				time += Time.deltaTime;
				yield return null;
			}
		}

		static System.Collections.IEnumerator LerpFloatCallbackCoroutine(float start, float end, float duration, Func<float, bool> body) {
			float time = 0;
			float delta = end - start;

			while(time < duration) {
				float t = time / duration;

				if(!body.Invoke(start + (delta * t)))
					yield break;

				time += Time.deltaTime;
				yield return null;
			}
		}

		public static bool NullCheck(object obj, string message = null) {
			bool isNull;

			if(obj is GameObject) {
				isNull = (obj as GameObject) == null || (obj as GameObject).IsDestroyed() || (obj as GameObject).WasCollected;
			} else if(obj is Component) {
				isNull = (obj as Component) == null || (obj as Component).WasCollected || (obj as Component).gameObject == null || (obj as Component).gameObject.IsDestroyed() || (obj as Component).gameObject.WasCollected;
			} else {
				isNull = obj == null;
			}

			if(isNull && message != null) {
				Melon<Mod>.Logger.Msg(message);
			}

			return isNull;
		}

		public static bool NullCheck(object[] obj, string message = null) {
			foreach(object o in obj) {
				if(o == null) {
					if(message != null)
						Melon<Mod>.Logger.Msg(message);

					return true;
				}
			}

			return false;
		}

		public static bool ZeroCheck(long value, string message = null) {
			if(value == 0) {
				if(message != null)
					Melon<Mod>.Logger.Msg(message);

				return true;
			}

			return false;
		}
	}
}
