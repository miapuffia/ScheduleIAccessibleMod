using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedTasksMod {
	internal class Prefs {
		//Task toggles
		internal static MelonPreferences_Category toggles;
		internal static MelonPreferences_Entry<bool> pouringSoilToggle;
		internal static MelonPreferences_Entry<bool> sowingSeedToggle;
		internal static MelonPreferences_Entry<bool> pouringWaterToggle;
		internal static MelonPreferences_Entry<bool> pouringFertilizerToggle;
		internal static MelonPreferences_Entry<bool> harvestingToggle;
		internal static MelonPreferences_Entry<bool> sinkToggle;
		internal static MelonPreferences_Entry<bool> packagingStationToggle;
		internal static MelonPreferences_Entry<bool> packagingStationMk2Toggle;
		internal static MelonPreferences_Entry<bool> brickPressToggle;
		internal static MelonPreferences_Entry<bool> mixingStationToggle;
		internal static MelonPreferences_Entry<bool> chemistryStationToggle;
		internal static MelonPreferences_Entry<bool> labOvenToggle;
		internal static MelonPreferences_Entry<bool> cauldronToggle;

		//General timings
		internal static MelonPreferences_Category taskTimings;
		internal static MelonPreferences_Entry<SpeedEnum> timingsPreset;

		//Pouring soil
		internal static MelonPreferences_Category pouringSoilTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingPouringSoilTask;
		internal static MelonPreferences_Entry<float> waitBetweenSoilCuts;
		internal static MelonPreferences_Entry<float> waitBeforeRotatingSoil;
		internal static MelonPreferences_Entry<float> timeToRotateSoil;

		//Sowing seed
		internal static MelonPreferences_Category sowingSeedTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingSowingSeedTask;
		internal static MelonPreferences_Entry<float> timeToMoveAndRotateSeedVial;
		internal static MelonPreferences_Entry<float> waitBeforePoppingSeedVialCap;
		internal static MelonPreferences_Entry<float> waitBeforeMovingDirtChunks;
		internal static MelonPreferences_Entry<float> waitBetweenMovingSoilChunks;

		//Pouring water
		internal static MelonPreferences_Category pouringWaterTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingPouringWaterTask;
		internal static MelonPreferences_Entry<float> timeToRotateWateringCan;
		internal static MelonPreferences_Entry<float> timeToMoveWateringCan;

		//Pouring fertilizer
		internal static MelonPreferences_Category pouringFertilizerTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingPouringFertilizerTask;
		internal static MelonPreferences_Entry<float> timeToRotateFertilizer;
		internal static MelonPreferences_Entry<float> timeToMoveFertilizer;

		//Harvesting
		internal static MelonPreferences_Category harvestingTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingHarvestingTask;
		internal static MelonPreferences_Entry<float> waitBetweenHarvestingPieces;
		internal static MelonPreferences_Entry<float> waitBetweenHarvestingPiecesElectric;

		//Sink
		internal static MelonPreferences_Category sinkTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingSinkTask;

		//Packaging station
		internal static MelonPreferences_Category packagingStationTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingPackagingTask;
		internal static MelonPreferences_Entry<float> timeToMoveProductToPackaging;
		internal static MelonPreferences_Entry<float> waitBeforeMovingPackagingToHatch;
		internal static MelonPreferences_Entry<float> timeToMovePackagingToHatch;
		internal static MelonPreferences_Entry<float> waitBetweenMovingPackagingToHatch;

		//Packaging station Mk2
		internal static MelonPreferences_Category packagingStationMk2Timings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingPackagingMk2Task;

		//Brick press
		internal static MelonPreferences_Category brickPressTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingBrickPressTask;
		internal static MelonPreferences_Entry<float> timeToMoveProductsToMoldUp;
		internal static MelonPreferences_Entry<float> timeToMoveProductsToMoldRight;
		internal static MelonPreferences_Entry<float> waitBeforePullingDownHandle;
		internal static MelonPreferences_Entry<float> timeToPullDownHandle;

		//Mixing station
		internal static MelonPreferences_Category mixingStationTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingMixingStationTask;
		internal static MelonPreferences_Entry<float> timeToMoveProductToMixer;
		internal static MelonPreferences_Entry<float> waitBetweenMovingItemsToMixer;
		internal static MelonPreferences_Entry<float> timeToMovePourableToMixer;
		internal static MelonPreferences_Entry<float> timeToRotatePourableToMixer;
		internal static MelonPreferences_Entry<float> timeToRotateAndMovePourableFromMixerBack;
		internal static MelonPreferences_Entry<float> waitBeforePressingMixerStartButton;

		//Chemistry station
		internal static MelonPreferences_Category chemistryStationTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingChemistryStationTask;
		internal static MelonPreferences_Entry<float> timeToMoveProductToBeaker;
		internal static MelonPreferences_Entry<float> waitBetweenMovingProductsToBeaker;
		internal static MelonPreferences_Entry<float> timeToMovePourableToBeaker;
		internal static MelonPreferences_Entry<float> timeToRotatePourableToBeaker;
		internal static MelonPreferences_Entry<float> timeToRotateAndMovePourableFromBeakerBack;
		internal static MelonPreferences_Entry<float> waitBetweenMovingPourablesToBeaker;
		internal static MelonPreferences_Entry<float> timeToRotateStirRod;
		internal static MelonPreferences_Entry<float> waitBeforeMovingLabStandDown;
		internal static MelonPreferences_Entry<float> timeToMoveLabStandDown;
		internal static MelonPreferences_Entry<float> waitBeforeMovingBeakerToFunnel;
		internal static MelonPreferences_Entry<float> timeToMoveBeakerToFunnel;
		internal static MelonPreferences_Entry<float> timeToRotateBeakerToFunnel;
		internal static MelonPreferences_Entry<float> waitBeforeMovingLabStandUp;
		internal static MelonPreferences_Entry<float> timeToMoveLabStandUp;
		internal static MelonPreferences_Entry<float> waitBeforeHandlingBurner;

		//Lab oven
		internal static MelonPreferences_Category labOvenTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingLabOvenTask;
		internal static MelonPreferences_Entry<float> timeToOpenLabOvenDoor;
		internal static MelonPreferences_Entry<float> timeToCloseLabOvenDoor;
		internal static MelonPreferences_Entry<float> waitBeforeMovingProductsToTray;
		internal static MelonPreferences_Entry<float> timeToMoveProductToTray;
		internal static MelonPreferences_Entry<float> waitBetweenMovingProductsToTray;
		internal static MelonPreferences_Entry<float> waitBeforeClosingLabOvenDoorCocaine;
		internal static MelonPreferences_Entry<float> waitBeforePressingLabOvenStartButton;
		internal static MelonPreferences_Entry<float> waitBeforeMovingHammerOverTray;
		internal static MelonPreferences_Entry<float> timeToMoveHammerOverTray;

		//Cauldron
		internal static MelonPreferences_Category cauldronTimings;
		internal static MelonPreferences_Entry<float> waitBeforeStartingCauldronTask;
		internal static MelonPreferences_Entry<float> timeToMoveGasolineToPot;
		internal static MelonPreferences_Entry<float> timeToRotateGasolineToPot;
		internal static MelonPreferences_Entry<float> timeToRotateAndMoveGasolineFromPotBack;
		internal static MelonPreferences_Entry<float> waitBeforeMovingProductsToPot;
		internal static MelonPreferences_Entry<float> timeToMoveProductToPot;
		internal static MelonPreferences_Entry<float> waitBetweenMovingProductsToPot;
		internal static MelonPreferences_Entry<float> waitBeforePressingCauldronStartButton;

		internal static void SetupPrefs() {
			PrettyInt categoryIndex = new(0);
			PrettyInt entryIndex = new(0);

			//Task toggles
			CreateCategory(ref toggles, "AutomatedTasksMod", ++categoryIndex, "Task Toggles");
			CreateToggleEntry(toggles, ref pouringSoilToggle, "automate", ++entryIndex, "Automate pouring soil");
			CreateToggleEntry(toggles, ref sowingSeedToggle, "automate", ++entryIndex, "Automate sowing seed");
			CreateToggleEntry(toggles, ref pouringWaterToggle, "automate", ++entryIndex, "Automate pouring water");
			CreateToggleEntry(toggles, ref pouringFertilizerToggle, "automate", ++entryIndex, "Automate pouring fertilizer");
			CreateToggleEntry(toggles, ref harvestingToggle, "automate", ++entryIndex, "Automate harvesting");
			CreateToggleEntry(toggles, ref sinkToggle, "automate", ++entryIndex, "Automate sink tap");
			CreateToggleEntry(toggles, ref packagingStationToggle, "automate", ++entryIndex, "Automate packaging station");
			CreateToggleEntry(toggles, ref packagingStationMk2Toggle, "automate", ++entryIndex, "Automate packaging MK2 station");
			CreateToggleEntry(toggles, ref brickPressToggle, "automate", ++entryIndex, "Automate brick press station");
			CreateToggleEntry(toggles, ref mixingStationToggle, "automate", ++entryIndex, "Automate mixing station");
			CreateToggleEntry(toggles, ref chemistryStationToggle, "automate", ++entryIndex, "Automate chemistry station");
			CreateToggleEntry(toggles, ref labOvenToggle, "automate", ++entryIndex, "Automate lab oven");
			CreateToggleEntry(toggles, ref cauldronToggle, "automate", ++entryIndex, "Automate cauldron");

			entryIndex = new(0);

			CreateCategory(ref taskTimings, "AutomatedTasksMod", ++categoryIndex, "Task Timings (in seconds)");
			CreateGenericEntry(taskTimings, ref timingsPreset, "timings", ++entryIndex, SpeedEnum.Custom_Values_Below, "Apply timings preset to every value");

			entryIndex = new(0);

			//Pouring soil
			CreateCategory(ref pouringSoilTimings, "AutomatedTasksMod", ++categoryIndex, "Pouring Soil");
			CreateTimingEntry(pouringSoilTimings, ref waitBeforeStartingPouringSoilTask, "timing", ++entryIndex, 0.5f, "Wait before starting pouring soil task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(pouringSoilTimings, ref waitBetweenSoilCuts, "timing", ++entryIndex, 0.1f, "Wait between cutting each soil bag segment", TimingTypeEnum.WaitBetween);
			CreateTimingEntry(pouringSoilTimings, ref waitBeforeRotatingSoil, "timing", ++entryIndex, 0.2f, "Wait before rotating soil", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(pouringSoilTimings, ref timeToRotateSoil, "timing", ++entryIndex, 1.5f, "Time it takes to rotate soil", TimingTypeEnum.Rotate);

			entryIndex = new(0);

			//Sowing seed
			CreateCategory(ref sowingSeedTimings, "AutomatedTasksMod", ++categoryIndex, "Sowing Seed");
			CreateTimingEntry(sowingSeedTimings, ref waitBeforeStartingSowingSeedTask, "timing", ++entryIndex, 0.5f, "Wait before starting sowing seed task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(sowingSeedTimings, ref timeToMoveAndRotateSeedVial, "timing", ++entryIndex, 1.5f, "Time it takes to move and rotate seed vial", TimingTypeEnum.MoveRotate);
			CreateTimingEntry(sowingSeedTimings, ref waitBeforePoppingSeedVialCap, "timing", ++entryIndex, 0.2f, "Wait before popping seed vial cap", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(sowingSeedTimings, ref waitBeforeMovingDirtChunks, "timing", ++entryIndex, 0.2f, "Wait before moving dirt chunks", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(sowingSeedTimings, ref waitBetweenMovingSoilChunks, "timing", ++entryIndex, 0.5f, "Wait between moving each soil chunk", TimingTypeEnum.WaitBetween);

			entryIndex = new(0);

			//Pouring water
			CreateCategory(ref pouringWaterTimings, "AutomatedTasksMod", ++categoryIndex, "Pouring Water");
			CreateTimingEntry(pouringWaterTimings, ref waitBeforeStartingPouringWaterTask, "timing", ++entryIndex, 0.5f, "Wait before starting pouring water task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(pouringWaterTimings, ref timeToRotateWateringCan, "timing", ++entryIndex, 0.8f, "Time it takes to rotate watering can", TimingTypeEnum.Rotate);
			CreateTimingEntry(pouringWaterTimings, ref timeToMoveWateringCan, "timing", ++entryIndex, 0.8f, "Time it takes to move watering can to target", TimingTypeEnum.Move);

			entryIndex = new(0);

			//Pouring fertilizer
			CreateCategory(ref pouringFertilizerTimings, "AutomatedTasksMod", ++categoryIndex, "Pouring Fertilizer");
			CreateTimingEntry(pouringFertilizerTimings, ref waitBeforeStartingPouringFertilizerTask, "timing", ++entryIndex, 0.5f, "Wait before starting pouring fertilizer task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(pouringFertilizerTimings, ref timeToRotateFertilizer, "timing", ++entryIndex, 1f, "Time it takes to rotate fertilizer", TimingTypeEnum.Rotate);
			CreateTimingEntry(pouringFertilizerTimings, ref timeToMoveFertilizer, "timing", ++entryIndex, 0.1f, "Time it takes to move fertilizer along each line segment", TimingTypeEnum.Move);

			entryIndex = new(0);

			//Harvesting
			CreateCategory(ref harvestingTimings, "AutomatedTasksMod", ++categoryIndex, "Harvesting");
			CreateTimingEntry(harvestingTimings, ref waitBeforeStartingHarvestingTask, "timing", ++entryIndex, 0.5f, "Wait before starting harvesting task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(harvestingTimings, ref waitBetweenHarvestingPieces, "timing", ++entryIndex, 0.5f, "Wait between harvesting each plant piece with non-electric trimmers", TimingTypeEnum.WaitBetween);
			CreateTimingEntry(harvestingTimings, ref waitBetweenHarvestingPiecesElectric, "timing", ++entryIndex, 0.25f, "Wait between harvesting each plant piece with electric trimmers", TimingTypeEnum.WaitBetween);

			entryIndex = new(0);

			//Sink
			CreateCategory(ref sinkTimings, "AutomatedTasksMod", ++categoryIndex, "Sink");
			CreateTimingEntry(sinkTimings, ref waitBeforeStartingSinkTask, "timing", ++entryIndex, 0.5f, "Wait before starting sink task", TimingTypeEnum.WaitInitial);

			entryIndex = new(0);

			//Packaging station
			CreateCategory(ref packagingStationTimings, "AutomatedTasksMod", ++categoryIndex, "Packaging Station");
			CreateTimingEntry(packagingStationTimings, ref waitBeforeStartingPackagingTask, "timing", ++entryIndex, 0.5f, "Wait before starting packaging station task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(packagingStationTimings, ref timeToMoveProductToPackaging, "timing", ++entryIndex, 0.5f, "Time it takes to move product to packaging", TimingTypeEnum.Move);
			CreateTimingEntry(packagingStationTimings, ref waitBeforeMovingPackagingToHatch, "timing", ++entryIndex, 0.2f, "Wait before moving packaging to hatch", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(packagingStationTimings, ref timeToMovePackagingToHatch, "timing", ++entryIndex, 0.3f, "Time it takes to move packaging to hatch", TimingTypeEnum.Move);
			CreateTimingEntry(packagingStationTimings, ref waitBetweenMovingPackagingToHatch, "timing", ++entryIndex, 0.8f, "Wait between moving packaging to hatch", TimingTypeEnum.WaitBetween);

			entryIndex = new(0);

			//Packaging station Mk2
			CreateCategory(ref packagingStationMk2Timings, "AutomatedTasksMod", ++categoryIndex, "Packaging Station Mk2");
			CreateTimingEntry(packagingStationMk2Timings, ref waitBeforeStartingPackagingMk2Task, "timing", ++entryIndex, 0.5f, "Wait before starting packaging station Mk2 task", TimingTypeEnum.WaitInitial);

			entryIndex = new(0);

			//Brick press
			CreateCategory(ref brickPressTimings, "AutomatedTasksMod", ++categoryIndex, "Brick Press");
			CreateTimingEntry(brickPressTimings, ref waitBeforeStartingBrickPressTask, "timing", ++entryIndex, 0.5f, "Wait before starting brick press task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(brickPressTimings, ref timeToMoveProductsToMoldUp, "timing", ++entryIndex, 1f, "Time it takes to move products to mold (up portion)", TimingTypeEnum.Move);
			CreateTimingEntry(brickPressTimings, ref timeToMoveProductsToMoldRight, "timing", ++entryIndex, 1f, "Time it takes to move products to mold (right portion)", TimingTypeEnum.Move);
			CreateTimingEntry(brickPressTimings, ref waitBeforePullingDownHandle, "timing", ++entryIndex, 0.2f, "Wait before pulling down handle", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(brickPressTimings, ref timeToPullDownHandle, "timing", ++entryIndex, 1f, "Time it takes to pull down handle", TimingTypeEnum.ChangeValue);

			entryIndex = new(0);

			//Mixing station
			CreateCategory(ref mixingStationTimings, "AutomatedTasksMod", ++categoryIndex, "Mixing Station");
			CreateTimingEntry(mixingStationTimings, ref waitBeforeStartingMixingStationTask, "timing", ++entryIndex, 0.5f, "Wait before starting mixing station task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(mixingStationTimings, ref timeToMoveProductToMixer, "timing", ++entryIndex, 0.5f, "Time it takes to move product to mixer", TimingTypeEnum.Move);
			CreateTimingEntry(mixingStationTimings, ref waitBetweenMovingItemsToMixer, "timing", ++entryIndex, 0.3f, "Wait between moving each item to mixer", TimingTypeEnum.WaitBetween);
			CreateTimingEntry(mixingStationTimings, ref timeToMovePourableToMixer, "timing", ++entryIndex, 0.8f, "Time it takes to move pourable to mixer", TimingTypeEnum.Move);
			CreateTimingEntry(mixingStationTimings, ref timeToRotatePourableToMixer, "timing", ++entryIndex, 2f, "Time it takes to rotate pourable", TimingTypeEnum.Rotate);
			CreateTimingEntry(mixingStationTimings, ref timeToRotateAndMovePourableFromMixerBack, "timing", ++entryIndex, 0.8f, "Time it takes to move and rotate pourable back", TimingTypeEnum.MoveRotateBack);
			CreateTimingEntry(mixingStationTimings, ref waitBeforePressingMixerStartButton, "timing", ++entryIndex, 0.5f, "Wait before pressing start button", TimingTypeEnum.WaitBefore);

			entryIndex = new(0);

			//Chemistry station
			CreateCategory(ref chemistryStationTimings, "AutomatedTasksMod", ++categoryIndex, "Chemistry Station");
			CreateTimingEntry(chemistryStationTimings, ref waitBeforeStartingChemistryStationTask, "timing", ++entryIndex, 0.5f, "Wait before starting chemistry station task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(chemistryStationTimings, ref timeToMoveProductToBeaker, "timing", ++entryIndex, 0.5f, "Time it takes to move product to beaker", TimingTypeEnum.Move);
			CreateTimingEntry(chemistryStationTimings, ref waitBetweenMovingProductsToBeaker, "timing", ++entryIndex, 0.3f, "Wait between moving each product to beaker", TimingTypeEnum.WaitBetween);
			CreateTimingEntry(chemistryStationTimings, ref timeToMovePourableToBeaker, "timing", ++entryIndex, 0.8f, "Time it takes to move pourable to beaker", TimingTypeEnum.Move);
			CreateTimingEntry(chemistryStationTimings, ref timeToRotatePourableToBeaker, "timing", ++entryIndex, 1.5f, "Time it takes to rotate pourable", TimingTypeEnum.Rotate);
			CreateTimingEntry(chemistryStationTimings, ref timeToRotateAndMovePourableFromBeakerBack, "timing", ++entryIndex, 0.8f, "Time it takes to move and rotate pourable back", TimingTypeEnum.MoveRotateBack);
			CreateTimingEntry(chemistryStationTimings, ref waitBetweenMovingPourablesToBeaker, "timing", ++entryIndex, 0.3f, "Wait between moving each pourable to beaker", TimingTypeEnum.WaitBetween);
			CreateTimingEntry(chemistryStationTimings, ref timeToRotateStirRod, "timing", ++entryIndex, 0.1f, "Time it takes to rotate sir rod (only effects visuals)", TimingTypeEnum.Rotate);
			CreateTimingEntry(chemistryStationTimings, ref waitBeforeMovingLabStandDown, "timing", ++entryIndex, 0.5f, "Wait before moving lab stand down", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(chemistryStationTimings, ref timeToMoveLabStandDown, "timing", ++entryIndex, 0.5f, "Time it takes to move lab stand down", TimingTypeEnum.ChangeValue);
			CreateTimingEntry(chemistryStationTimings, ref waitBeforeMovingBeakerToFunnel, "timing", ++entryIndex, 0.5f, "Wait before moving beaker to funnel", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(chemistryStationTimings, ref timeToMoveBeakerToFunnel, "timing", ++entryIndex, 0.8f, "Time it takes to move beaker to funnel", TimingTypeEnum.Move);
			CreateTimingEntry(chemistryStationTimings, ref timeToRotateBeakerToFunnel, "timing", ++entryIndex, 3f, "Time it takes to rotate beaker", TimingTypeEnum.Rotate);
			CreateTimingEntry(chemistryStationTimings, ref waitBeforeMovingLabStandUp, "timing", ++entryIndex, 0.5f, "Wait before moving lab stand up", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(chemistryStationTimings, ref timeToMoveLabStandUp, "timing", ++entryIndex, 0.5f, "Time it takes to move lab stand up", TimingTypeEnum.ChangeValue);
			CreateTimingEntry(chemistryStationTimings, ref waitBeforeHandlingBurner, "timing", ++entryIndex, 0.5f, "Wait before handling burner", TimingTypeEnum.WaitBefore);

			entryIndex = new(0);

			//Lab oven
			CreateCategory(ref labOvenTimings, "AutomatedTasksMod", ++categoryIndex, "Lab Oven");
			CreateTimingEntry(labOvenTimings, ref waitBeforeStartingLabOvenTask, "timing", ++entryIndex, 0.5f, "Wait before starting lab oven task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(labOvenTimings, ref timeToOpenLabOvenDoor, "timing", ++entryIndex, 0.5f, "Time it takes to open lab oven door", TimingTypeEnum.ChangeValue);
			CreateTimingEntry(labOvenTimings, ref timeToCloseLabOvenDoor, "timing", ++entryIndex, 0.5f, "Time it takes to close lab oven door", TimingTypeEnum.ChangeValue);
			CreateTimingEntry(labOvenTimings, ref waitBeforeMovingProductsToTray, "timing", ++entryIndex, 1f, "Wait before moving products to tray", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(labOvenTimings, ref timeToMoveProductToTray, "timing", ++entryIndex, 0.5f, "Time it takes to move product to tray", TimingTypeEnum.Move);
			CreateTimingEntry(labOvenTimings, ref waitBetweenMovingProductsToTray, "timing", ++entryIndex, 0.3f, "Wait between moving each product to tray", TimingTypeEnum.WaitBetween);
			CreateTimingEntry(labOvenTimings, ref waitBeforeClosingLabOvenDoorCocaine, "timing", ++entryIndex, 0.5f, "Wait before closing lab oven door when making cocaine", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(labOvenTimings, ref waitBeforePressingLabOvenStartButton, "timing", ++entryIndex, 0.5f, "Wait before pressing start button", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(labOvenTimings, ref waitBeforeMovingHammerOverTray, "timing", ++entryIndex, 0.5f, "Wait before moving hammer over tray", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(labOvenTimings, ref timeToMoveHammerOverTray, "timing", ++entryIndex, 0.5f, "Time it takes to move hammer over tray", TimingTypeEnum.Move);

			entryIndex = new(0);

			//Cauldron
			CreateCategory(ref cauldronTimings, "AutomatedTasksMod", ++categoryIndex, "Cauldron");
			CreateTimingEntry(cauldronTimings, ref waitBeforeStartingCauldronTask, "timing", ++entryIndex, 0.5f, "Wait before starting cauldron task", TimingTypeEnum.WaitInitial);
			CreateTimingEntry(cauldronTimings, ref timeToMoveGasolineToPot, "timing", ++entryIndex, 1f, "Time it takes to move gasoline to pot", TimingTypeEnum.Move);
			CreateTimingEntry(cauldronTimings, ref timeToRotateGasolineToPot, "timing", ++entryIndex, 2f, "Time it takes to rotate gasoline", TimingTypeEnum.Rotate);
			CreateTimingEntry(cauldronTimings, ref timeToRotateAndMoveGasolineFromPotBack, "timing", ++entryIndex, 0.8f, "Time it takes to move and rotate gasoline back", TimingTypeEnum.MoveRotateBack);
			CreateTimingEntry(cauldronTimings, ref waitBeforeMovingProductsToPot, "timing", ++entryIndex, 0.5f, "Wait before moving products to pot", TimingTypeEnum.WaitBefore);
			CreateTimingEntry(cauldronTimings, ref timeToMoveProductToPot, "timing", ++entryIndex, 0.5f, "Time it takes to move product to pot", TimingTypeEnum.Move);
			CreateTimingEntry(cauldronTimings, ref waitBetweenMovingProductsToPot, "timing", ++entryIndex, 0.3f, "Wait between moving each product to pot", TimingTypeEnum.WaitBetween);
			CreateTimingEntry(cauldronTimings, ref waitBeforePressingCauldronStartButton, "timing", ++entryIndex, 0.5f, "Wait before pressing start button", TimingTypeEnum.WaitBefore);
		}

		private static void CreateCategory(ref MelonPreferences_Category category, string prefix, PrettyInt index, string displayName, [CallerArgumentExpression(nameof(category))] string categoryName = "") {
			category = MelonPreferences.CreateCategory($"{prefix}_{index}_{categoryName}", displayName);
		}

		private static void CreateToggleEntry(MelonPreferences_Category category, ref MelonPreferences_Entry<bool> entry, string prefix, PrettyInt index, string displayName, [CallerArgumentExpression(nameof(entry))] string entryName = "") {
			entry = category.CreateEntry($"{prefix}_{index}_{entryName}", true, displayName);
		}

		private static void CreateGenericEntry<T>(MelonPreferences_Category category, ref MelonPreferences_Entry<T> entry, string prefix, PrettyInt index, T defaultValue, string displayName, [CallerArgumentExpression(nameof(entry))] string entryName = "") {
			entry = category.CreateEntry($"{prefix}_{index}_{entryName}", defaultValue, displayName);
		}

		private static void CreateTimingEntry(MelonPreferences_Category category, ref MelonPreferences_Entry<float> entry, string prefix, PrettyInt index, float defaultValue, string displayName, TimingTypeEnum timingType, [CallerArgumentExpression(nameof(entry))] string entryName = "") {
			entry = category.CreateEntry($"{prefix}_{index}_{entryName}", defaultValue, displayName);
			entry.Comment = timingType.ToString();
		}

		internal static float GetTiming(MelonPreferences_Entry<float> timingPref) {
			float fastDefault = MathF.Min(timingPref.DefaultValue, 0.2f);
			float fastestWait = 0;
			float slowerTime = MathF.Min(timingPref.DefaultValue, 0.5f);

			return timingsPreset.Value switch {
				SpeedEnum.Custom_Values_Below => timingPref.Value,
				SpeedEnum.Default_Values => timingPref.DefaultValue,
				SpeedEnum.Fast => (Enum.TryParse(timingPref.Comment, out TimingTypeEnum timingType)
					? timingType switch {
						TimingTypeEnum.WaitInitial => fastDefault,
						TimingTypeEnum.WaitBefore => fastestWait,
						TimingTypeEnum.WaitBetween => fastestWait,
						TimingTypeEnum.Move => fastDefault,
						TimingTypeEnum.MoveRotate => slowerTime,
						TimingTypeEnum.MoveRotateBack => slowerTime,
						TimingTypeEnum.Rotate => slowerTime,
						TimingTypeEnum.ChangeValue => slowerTime,
						_ => fastDefault,
					} : fastDefault),
				SpeedEnum.Slow => timingPref.DefaultValue * 1.5f,
				_ => timingPref.Value,
			};
		}
	}
}
