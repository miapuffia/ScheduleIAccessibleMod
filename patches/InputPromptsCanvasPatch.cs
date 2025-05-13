using HarmonyLib;
using Il2CppMTAssets.UltimateLODSystem.MeshSimplifier;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.ObjectScripts.Soil;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerTasks;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Stations;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomatedTasksMod {
	[HarmonyPatch(typeof(InputPromptsCanvas), "LoadModule", [typeof(string)])]
	public static class InputPromptsCanvasPatch {
		private static void Postfix(InputPromptsCanvas __instance) {
			switch(__instance.currentModuleLabel) {
				case "pourable":
					MelonCoroutines.Start(AutomatePouringSoilCoroutine());
					MelonCoroutines.Start(AutomateSowingSeedCoroutine());
					MelonCoroutines.Start(AutomatePouringWaterCoroutine());
					MelonCoroutines.Start(AutomatePouringFertilizerCoroutine());
					break;
				case "harvestplant":
					if(Prefs.harvestingToggle.Value) {
						MelonCoroutines.Start(AutomateHarvestingCoroutine());
					} else {
						Melon<Mod>.Logger.Msg("Automate harvesting disabled in settings");
					}
					break;
			}
		}

		private static System.Collections.IEnumerator AutomatePouringSoilCoroutine() {
			bool stepComplete;
			bool isInUse;
			bool isError = false;

			float _waitBeforeStartingPouringSoilTask = Prefs.GetTiming(Prefs.waitBeforeStartingPouringSoilTask);
			bool _pouringSoilToggle = Prefs.pouringSoilToggle.Value;
			float _waitBetweenSoilCuts = Prefs.GetTiming(Prefs.waitBetweenSoilCuts);
			float _waitBeforeRotatingSoil = Prefs.GetTiming(Prefs.waitBeforeRotatingSoil);
			float _timeToRotateSoil = Prefs.GetTiming(Prefs.timeToRotateSoil);

			yield return new WaitForSeconds(_waitBeforeStartingPouringSoilTask);

			PourableSoil soil = GetPourableInUse<PourableSoil>();

			if(Utils.NullCheck(soil)) {
				//Don't print error message because we might not even be trying to do this task
				yield break;
			} else if(!_pouringSoilToggle) {
				Melon<Mod>.Logger.Msg("Automate pouring soil disabled in settings");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Pour soil task started");

			stepComplete = false;

			//It shouldn't take more than 10 cuts so this is a failsafe
			for(int i = 0; i < 10; i++) {
				Melon<Mod>.Logger.Msg("Cutting open soil");

				GetIsPotInUse(soil, out isInUse, ref isError);

				if(isError || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find soil - probably exited task");
					yield break;
				}

				soil.Cut();

				if(soil.IsOpen) {
					Melon<Mod>.Logger.Msg("Done opening soil");
					stepComplete = true;
					break;
				}

				yield return new WaitForSeconds(_waitBetweenSoilCuts);
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Cutting open soil didn't complete after 10 attempts");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeRotatingSoil);

			Melon<Mod>.Logger.Msg("Pouring soil");

			GetIsPotInUse(soil, out isInUse, ref isError);

			if(isError || !isInUse) {
				Melon<Mod>.Logger.Msg("Can't find soil - probably exited task");
				yield break;
			}

			isError = false;

			yield return Utils.SinusoidalLerpRotationCoroutine(soil.transform, new Vector3(soil.transform.localEulerAngles.x, soil.transform.localEulerAngles.y, soil.transform.localEulerAngles.z - 180), _timeToRotateSoil, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find soil to rotate - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Done pouring soil");
		}

		private static System.Collections.IEnumerator AutomateSowingSeedCoroutine() {
			Pot pot;
			Vector3 moveToPosition;
			bool stepComplete;
			bool isInUse;
			bool isError = false;
			float time;

			float _waitBeforeStartingSowingSeedTask = Prefs.GetTiming(Prefs.waitBeforeStartingSowingSeedTask);
			bool _sowingSeedToggle = Prefs.sowingSeedToggle.Value;
			float _timeToMoveAndRotateSeedVial = Prefs.GetTiming(Prefs.timeToMoveAndRotateSeedVial);
			float _waitBeforePoppingSeedVialCap = Prefs.GetTiming(Prefs.waitBeforePoppingSeedVialCap);
			float _waitBeforeMovingDirtChunks = Prefs.GetTiming(Prefs.waitBeforeMovingDirtChunks);
			float _waitBetweenMovingSoilChunks = Prefs.GetTiming(Prefs.waitBetweenMovingSoilChunks);

			yield return new WaitForSeconds(_waitBeforeStartingSowingSeedTask);

			FunctionalSeed seed = GameObject.FindObjectOfType<FunctionalSeed>();

			if(Utils.NullCheck(seed)) {
				//Don't print error message because we might not even be trying to do this task
				yield break;
			} else if(!_sowingSeedToggle) {
				Melon<Mod>.Logger.Msg("Automate sowing seeds disabled in settings");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Sow seed task started");
			Melon<Mod>.Logger.Msg("Moving and rotating seed vial");

			pot = GetPotInUse();

			if(Utils.NullCheck(pot, "Can't find the pot the player is using"))
				yield break;

			if(Utils.NullCheck(seed.Vial, "Can't find seed vial - probably exited task"))
				yield break;

			moveToPosition = seed.Vial.transform.position;
			moveToPosition.y -= 0.1f;

			seed.Vial.transform.localEulerAngles = Vector3.zero;

			isError = false;

			yield return Utils.SinusoidalLerpPositionAndRotationCoroutine(seed.Vial.transform, moveToPosition, new Vector3(seed.Vial.transform.localEulerAngles.x + 180, seed.Vial.transform.localEulerAngles.y, seed.Vial.transform.localEulerAngles.z), _timeToMoveAndRotateSeedVial, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find seed vial to move and rotate - probably exited task");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforePoppingSeedVialCap);

			Melon<Mod>.Logger.Msg("Popping seed cap");

			if(Utils.NullCheck([seed, seed?.Cap], "Can't find seed cap - probably exited task"))
				yield break;

			seed.Cap.Pop();

			Melon<Mod>.Logger.Msg("Waiting for seed to fall into position");

			time = 0;
			stepComplete = false;

			//Up to 3 seconds
			while(time < 3) {
				GetIsPotInUse(pot, out isInUse, ref isError);

				if(isError || !isInUse || pot.SoilChunks.Length == 0 || Utils.NullCheck(pot.SoilChunks[0])) {
					Melon<Mod>.Logger.Msg("Can't find soil chunks - probably exited task");
					yield break;
				}

				if(pot.SoilChunks[0].ClickableEnabled) {
					Melon<Mod>.Logger.Msg("Seed is in position");
					stepComplete = true;
					break;
				}

				time += Time.deltaTime;

				yield return null;
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Seed didn't fall into place after 3 seconds");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforeMovingDirtChunks);

			foreach(SoilChunk soilChunk in pot.SoilChunks) {
				Melon<Mod>.Logger.Msg("Moving soil chunk");

				GetIsPotInUse(pot, out isInUse, ref isError);

				if(isError || Utils.NullCheck(soilChunk) || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find soil chunk - probably exited task");
					yield break;
				}

				soilChunk.StartClick(new RaycastHit());

				yield return new WaitForSeconds(_waitBetweenMovingSoilChunks);
			}

			Melon<Mod>.Logger.Msg("Done sowing seed");
		}

		private static System.Collections.IEnumerator AutomatePouringWaterCoroutine() {
			Vector3 moveToPosition;
			bool stepComplete;
			bool stepComplete2;
			bool isInUse;
			bool isError = false;
			float time;

			float _waitBeforeStartingPouringWaterTask = Prefs.GetTiming(Prefs.waitBeforeStartingPouringWaterTask);
			bool _pouringWaterToggle = Prefs.pouringWaterToggle.Value;
			float _timeToRotateWateringCan = Prefs.GetTiming(Prefs.timeToRotateWateringCan);
			float _timeToMoveWateringCan = Prefs.GetTiming(Prefs.timeToMoveWateringCan);

			yield return new WaitForSeconds(_waitBeforeStartingPouringWaterTask);

			FunctionalWateringCan wateringCan = GameObject.FindObjectOfType<FunctionalWateringCan>();

			if(Utils.NullCheck(wateringCan)) {
				//Don't print error message because we might not even be trying to do this task
				yield break;
			} else if(!_pouringWaterToggle) {
				Melon<Mod>.Logger.Msg("Automate pouring water disabled in settings");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Water soil task started");
			Melon<Mod>.Logger.Msg("Rotating watering can");

			isError = false;

			yield return Utils.SinusoidalLerpRotationCoroutine(wateringCan.transform, new Vector3(wateringCan.transform.localEulerAngles.x, wateringCan.transform.localEulerAngles.y, wateringCan.transform.localEulerAngles.z - 90), _timeToRotateWateringCan, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find watering can to rotate - probably exited task");
				yield break;
			}

			stepComplete = false;
			Vector3 targetPosition;

			//There shouldn't be more than 10 watering spots so this is a failsafe
			for(int i = 0; i < 10; i++) {
				Melon<Mod>.Logger.Msg("Moving watering can");

				GetIsPotInUse(wateringCan, out isInUse, ref isError);

				if(isError || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find watering can - probably exited task");
					yield break;
				}

				if(Utils.NullCheck([wateringCan.PourPoint, wateringCan.TargetPot.Target], "Can't find watering can components - probably exited task"))
					yield break;

				targetPosition = wateringCan.TargetPot.Target.position;

				moveToPosition = new Vector3(wateringCan.transform.position.x - (wateringCan.PourPoint.position.x - targetPosition.x), wateringCan.transform.position.y, wateringCan.transform.position.z - (wateringCan.PourPoint.position.z - targetPosition.z));

				isError = false;

				yield return Utils.SinusoidalLerpPositionCoroutine(wateringCan.transform, moveToPosition, _timeToMoveWateringCan, () => isError = true);

				if(isError) {
					Melon<Mod>.Logger.Msg("Can't find watering can to move - probably exited task");
					yield break;
				}

				time = 0;
				stepComplete2 = false;

				//Up to 5 seconds
				while(time < 5) {
					if(Utils.NullCheck([wateringCan, wateringCan?.TargetPot, wateringCan?.TargetPot?.Target], "Can't find watering can pot target - probably exited task"))
						yield break;

					if(wateringCan.TargetPot.Target.position != targetPosition) {
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

				if(wateringCan.TargetPot.WaterLevel > wateringCan.TargetPot.WaterCapacity - wateringCan.TargetPot.WaterDrainPerHour) {
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

		private static System.Collections.IEnumerator AutomatePouringFertilizerCoroutine() {
			Vector3 targetPosition;
			Vector3 moveToPosition;
			bool stepComplete;
			bool isInUse;
			bool isError = false;

			float _waitBeforeStartingPouringFertilizerTask = Prefs.GetTiming(Prefs.waitBeforeStartingPouringFertilizerTask);
			bool _pouringFertilizerToggle = Prefs.pouringFertilizerToggle.Value;
			float _timeToRotateFertilizer = Prefs.GetTiming(Prefs.timeToRotateFertilizer);
			float _timeToMoveFertilizer = Prefs.GetTiming(Prefs.timeToMoveFertilizer);

			yield return new WaitForSeconds(_waitBeforeStartingPouringFertilizerTask);

			PourableAdditive fertilizer = GetPourableInUse<PourableAdditive>();

			if(Utils.NullCheck(fertilizer)) {
				//Don't print error message because we might not even be trying to do this task
				yield break;
			} else if(!_pouringFertilizerToggle) {
				Melon<Mod>.Logger.Msg("Automate pouring fertilizer disabled in settings");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Pour fertilizer task started");
			Melon<Mod>.Logger.Msg("Rotating fertilizer");

			isError = false;

			yield return Utils.SinusoidalLerpRotationCoroutine(fertilizer.transform, new Vector3(fertilizer.transform.localEulerAngles.x, fertilizer.transform.localEulerAngles.y, fertilizer.transform.localEulerAngles.z - 180), _timeToRotateFertilizer, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find fertilizer to rotate - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Pouring fertilizer");

			if(Utils.NullCheck([fertilizer, fertilizer?.TargetPot], "Can't find fertilizer - probably exited task"))
				yield break;

			float angle = 0;
			int numSpiralRevolutions = 4;
			float maxAngle = 360 * numSpiralRevolutions;
			bool spiralingOut = true;
			stepComplete = false;

			for(float r = 0f; r >= 0; r = (-Math.Abs((angle / maxAngle) - 1) + 1) * fertilizer.TargetPot.PotRadius) {
				GetIsPotInUse(fertilizer, out isInUse, ref isError);

				if(isError || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find fertilizer - probably exited task");
					yield break;
				}

				targetPosition = new Vector3(fertilizer.TargetPot.PourableStartPoint.position.x + (Mathf.Sin(angle * Mathf.Deg2Rad) * r), 0, fertilizer.TargetPot.PourableStartPoint.position.z + (Mathf.Cos(angle * Mathf.Deg2Rad) * r));

				moveToPosition = new Vector3(fertilizer.transform.position.x - (fertilizer.PourPoint.position.x - targetPosition.x), fertilizer.transform.position.y, fertilizer.transform.position.z - (fertilizer.PourPoint.position.z - targetPosition.z));

				isError = false;

				yield return Utils.LerpPositionCoroutine(fertilizer.transform, moveToPosition, _timeToMoveFertilizer, () => isError = true);

				if(isError) {
					if(Utils.NullCheck([fertilizer, fertilizer?.TargetPot], "Can't find fertilizer pot - probably exited task"))
						yield break;

					if(fertilizer.TargetPot.AppliedAdditives.Count > 0) {
						Melon<Mod>.Logger.Msg("Done pouring fertilizer");
					} else {
						Melon<Mod>.Logger.Msg("Can't find fertilizer to move - probably exited task");
					}

					yield break;
				}

				if(Utils.NullCheck([fertilizer, fertilizer?.TargetPot], "Can't find fertilizer pot - probably exited task"))
					yield break;

				angle += 10 / (float) Math.Max(r / fertilizer.TargetPot.PotRadius, 0.1);

				if(spiralingOut && angle > maxAngle) {
					Melon<Mod>.Logger.Msg("Pouring fertilizer did not complete after reaching the pot's radius - going back to center");
					spiralingOut = false;
				}
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Pouring fertilizer did not complete after reaching the pot's radius and back to center");
				yield break;
			}
		}

		private static System.Collections.IEnumerator AutomateHarvestingCoroutine() {
			Pot pot;
			bool isInUse;
			bool isError = false;

			float _waitBeforeStartingHarvestingTask = Prefs.GetTiming(Prefs.waitBeforeStartingHarvestingTask);

			Melon<Mod>.Logger.Msg("Harvest task started");

			yield return new WaitForSeconds(_waitBeforeStartingHarvestingTask);

			pot = GetPotInUse();

			if(Utils.NullCheck(pot, "Can't find the pot the player is using - probably exited task"))
				yield break;

			float harvestCooldown = IsUsingElectricTrimmers(pot.PlayerUserObject.GetComponent<Player>()) ? Prefs.GetTiming(Prefs.waitBetweenHarvestingPiecesElectric) : Prefs.GetTiming(Prefs.waitBetweenHarvestingPieces);

			foreach(PlantHarvestable harvestable in pot.GetComponentsInChildren<PlantHarvestable>()) {
				Melon<Mod>.Logger.Msg("Harvesting plant piece");

				GetIsPotInUse(pot, out isInUse, ref isError);

				if(isError || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find the pot the player is using - probably exited task");
					yield break;
				}

				if(!CanHarvestableFitInInventory(pot)) {
					Melon<Mod>.Logger.Msg("Harvestable can't fit in inventory - exiting");
					yield break;
				}

				harvestable.Harvest();

				yield return new WaitForSeconds(harvestCooldown);
			}

			Melon<Mod>.Logger.Msg("Done harvesting");
		}

		private static T GetPourableInUse<T>() where T : Pourable {
			return GameObject.FindObjectsOfType<T>().FirstOrDefault(p => p.TargetPot?.PlayerUserObject.GetComponent<Player>()?.IsLocalPlayer ?? false);
		}

		private static Pot GetPotInUse() {
			return GameObject.FindObjectsOfType<Pot>().FirstOrDefault(p => p.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);
		}

		private static void GetIsPotInUse(Pourable pourable, out bool isInUse, ref bool isError) {
			if(Utils.NullCheck([pourable])) {
				isError = true;
				isInUse = false;
				return;
			}

			GetIsPotInUse(pourable.TargetPot, out isInUse, ref isError);
		}

		private static void GetIsPotInUse(Pot pot, out bool isInUse, ref bool isError) {
			if(Utils.NullCheck([pot, pot?.PlayerUserObject])) {
				isError = true;
				isInUse = false;
				return;
			}

			isError = false;
			isInUse = pot.PlayerUserObject.GetComponent<Player>()?.IsLocalPlayer ?? false;
		}

		private static bool IsUsingElectricTrimmers(Player player) {
			GameObject playerGO = player.LocalGameObject;

			if(Utils.NullCheck(playerGO)) {
				Melon<Mod>.Logger.Msg("Can't find player to determine what trimmers are being used - continuing");
				return false;
			}

			PlayerInventory playerInventory = playerGO.GetComponent<PlayerInventory>();

			if(Utils.NullCheck(playerInventory)) {
				Melon<Mod>.Logger.Msg("Can't find player inventory to determine what trimmers are being used - continuing");
				return false;
			}

			if(playerInventory.PriorEquippedSlotIndex >= player.Inventory.Count) {
				Melon<Mod>.Logger.Msg("Invalid equipped item index - continuing");
				return false;
			}

			ItemSlot itemSlot = player.Inventory[playerInventory.PriorEquippedSlotIndex];

			if(Utils.NullCheck(itemSlot)) {
				Melon<Mod>.Logger.Msg("Can't find item slot to determine what trimmers are being used - continuing");
				return false;
			}

			ItemInstance itemInstance = itemSlot.ItemInstance;

			if(Utils.NullCheck(itemInstance)) {
				Melon<Mod>.Logger.Msg("Can't find item instance to determine what trimmers are being used - continuing");
				return false;
			}

			if(itemInstance.ID == "electrictrimmers") {
				return true;
			}

			return false;
		}

		private static bool CanHarvestableFitInInventory(Pot pot) {
			if(Utils.NullCheck([pot, pot?.Plant])) {
				Melon<Mod>.Logger.Msg("Can't find pot plant");
				return false;
			}

			return PlayerInventory.Instance.CanItemFitInInventory(pot.Plant.GetHarvestedProduct(1));
		}
	}
}
