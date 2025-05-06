using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
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
		internal static MelonPreferences_Entry<float> waitAfterMovingPackagingToHatch;

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

			toggles = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_toggles", "Task Toggles");

			//Task toggles
			pouringSoilToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_pouringSoilToggle", true, "Automate pouring soil");
			sowingSeedToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_sowingSeedToggle", true, "Automate sowing seed");
			pouringWaterToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_pouringWaterToggle", true, "Automate pouring water");
			pouringFertilizerToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_pouringFertilizerToggle", true, "Automate pouring fertilizer");
			harvestingToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_harvestingToggle", true, "Automate harvesting");
			sinkToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_sinkToggle", true, "Automate sink tap");
			packagingStationToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_packagingStationToggle", true, "Automate packaging station");
			packagingStationMk2Toggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_packagingStationMk2Toggle", true, "Automate packaging MK2 station");
			brickPressToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_brickPressToggle", true, "Automate brick press station");
			mixingStationToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_mixingStationToggle", true, "Automate mixing station");
			chemistryStationToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_chemistryStationToggle", true, "Automate chemistry station");
			labOvenToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_labOvenToggle", true, "Automate lab oven");
			cauldronToggle = toggles.CreateEntry<bool>($"automate_{++entryIndex}_cauldronToggle", true, "Automate cauldron");

			entryIndex = new(0);

			taskTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_timings", "Task Timings (in seconds)");
			timingsPreset = taskTimings.CreateEntry<SpeedEnum>($"timings_{++entryIndex}_timingsPreset", SpeedEnum.Custom_Values_Below, "Apply timings preset to every value");

			entryIndex = new(0);

			//Pouring soil
			pouringSoilTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_pouringSoilTimings", "Pouring Soil");
			waitBeforeStartingPouringSoilTask = pouringSoilTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingPouringSoilTask", 0.5f, "Wait before starting pouring soil task");
			waitBetweenSoilCuts = pouringSoilTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenSoilCuts", 0.1f, "Wait between cutting each soil bag segment");
			waitBeforeRotatingSoil = pouringSoilTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeRotatingSoil", 0.2f, "Wait before rotating soil");
			timeToRotateSoil = pouringSoilTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateSoil", 1.5f, "Time it takes to rotate soil");

			entryIndex = new(0);

			//Sowing seed
			sowingSeedTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_sowingSeedTimings", "Sowing Seed");
			waitBeforeStartingSowingSeedTask = sowingSeedTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingSowingTask", 0.5f, "Wait before starting sowing seed task");
			timeToMoveAndRotateSeedVial = sowingSeedTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveAndRotateSeedVial", 1.5f, "Time it takes to rotate and move seed vial");
			waitBeforePoppingSeedVialCap = sowingSeedTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforePoppingSeedVialCap", 0.2f, "Wait before popping seed vial cap");
			waitBeforeMovingDirtChunks = sowingSeedTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeMovingDirtChunks", 0.5f, "Wait before moving dirt chunks");
			waitBetweenMovingSoilChunks = sowingSeedTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenMovingSoilChunks", 0.5f, "Wait between moving each soil chunk");

			entryIndex = new(0);

			//Pouring water
			pouringWaterTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_pouringWaterTimings", "Pouring Water");
			waitBeforeStartingPouringWaterTask = pouringWaterTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingPouringWaterTask", 0.5f, "Wait before starting pouring water task");
			timeToRotateWateringCan = pouringWaterTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateWateringCan", 0.8f, "Time it takes to rotate watering can");
			timeToMoveWateringCan = pouringWaterTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveWateringCan", 0.8f, "Time it takes to move watering can to target");

			entryIndex = new(0);

			//Pouring fertilizer
			pouringFertilizerTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_pouringFertilizerTimings", "Pouring Fertilizer");
			waitBeforeStartingPouringFertilizerTask = pouringFertilizerTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingPouringFertilizerTask", 0.5f, "Wait before starting pouring fertilizer task");
			timeToRotateFertilizer = pouringFertilizerTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateFertilizer", 1f, "Time it takes to rotate fertilizer");
			timeToMoveFertilizer = pouringFertilizerTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveFertilizer", 0.1f, "Time it takes to move fertilizer along each line segment");

			entryIndex = new(0);

			//Harvesting
			harvestingTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_harvestingTimings", "Harvesting");
			waitBeforeStartingHarvestingTask = harvestingTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingHarvestingTask", 0.5f, "Wait before starting harvesting task");
			waitBetweenHarvestingPieces = harvestingTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenHarvestingPieces", 0.5f, "Wait between harvesting each plant piece with non-electric trimmers");
			waitBetweenHarvestingPiecesElectric = harvestingTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenHarvestingPiecesElectric", 0.25f, "Wait between harvesting each plant piece with electric trimmers");

			entryIndex = new(0);

			//Sink
			sinkTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_sinkTimings", "Sink");
			waitBeforeStartingSinkTask = sinkTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingSinkTask", 0.5f, "Wait before starting sink task");

			entryIndex = new(0);

			//Packaging station
			packagingStationTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_packagingStationTimings", "Packaging Station");
			waitBeforeStartingPackagingTask = packagingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingPackagingTask", 0.5f, "Wait before starting packaging station task");
			timeToMoveProductToPackaging = packagingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveProductToPackaging", 0.5f, "Time it takes to move product to packaging");
			waitBeforeMovingPackagingToHatch = packagingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeMovingPackagingToHatch", 0.2f, "Wait before moving packaging to hatch");
			timeToMovePackagingToHatch = packagingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMovePackagingToHatch", 0.3f, "Time it takes to move packaging to hatch");
			waitAfterMovingPackagingToHatch = packagingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitAfterMovingPackagingToHatch", 0.8f, "Wait after moving packaging to hatch");

			entryIndex = new(0);

			//Packaging station Mk2
			packagingStationMk2Timings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_packagingStationMk2Timings", "Packaging Station Mk2");
			waitBeforeStartingPackagingMk2Task = packagingStationMk2Timings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingPackagingMk2Task", 0.5f, "Wait before starting packaging station Mk2 task");

			entryIndex = new(0);

			//Brick press
			brickPressTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_brickPressTimings", "Brick Press");
			waitBeforeStartingBrickPressTask = brickPressTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingBrickPressTask", 0.5f, "Wait before starting brick press task");
			timeToMoveProductsToMoldUp = brickPressTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveProductsToMoldUp", 1f, "Time it takes to move products to mold (up portion)");
			timeToMoveProductsToMoldRight = brickPressTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveProductsToMoldRight", 1f, "Time it takes to move products to mold (right portion)");
			waitBeforePullingDownHandle = brickPressTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforePullingDownHandle", 1f, "Wait before pulling down handle");
			timeToPullDownHandle = brickPressTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToPullDownHandle", 1f, "Time it takes to pull down handle");

			entryIndex = new(0);

			//Mixing station
			mixingStationTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_mixingStationTimings", "Mixing Station");
			waitBeforeStartingMixingStationTask = mixingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingMixingStationTask", 0.5f, "Wait before starting mixing station task");
			timeToMoveProductToMixer = mixingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveProductToMixer", 0.5f, "Time it takes to move product to mixer");
			waitBetweenMovingItemsToMixer = mixingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenMovingItemsToMixer", 0.3f, "Wait between moving each item to mixer");
			timeToMovePourableToMixer = mixingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMovePourableToMixer", 0.8f, "Time it takes to move pourable to mixer");
			timeToRotatePourableToMixer = mixingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotatePourableToMixer", 2f, "Time it takes to rotate pourable");
			timeToRotateAndMovePourableFromMixerBack = mixingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateAndMovePourableFromMixerBack", 0.8f, "Time it takes to move and rotate pourable back");
			waitBeforePressingMixerStartButton = mixingStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforePressingMixerStartButton", 0.5f, "Wait before pressing start button");

			entryIndex = new(0);

			//Chemistry station
			chemistryStationTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_chemistryStationTimings", "Chemistry Station");
			waitBeforeStartingChemistryStationTask = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingChemistryStationTask", 0.5f, "Wait before starting chemistry station task");
			timeToMoveProductToBeaker = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveProductToBeaker", 0.5f, "Time it takes to move product to beaker");
			waitBetweenMovingProductsToBeaker = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenMovingProductsToBeaker", 0.3f, "Wait between moving each product to beaker");
			timeToMovePourableToBeaker = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMovePourableToBeaker", 0.8f, "Time it takes to move pourable to beaker");
			timeToRotatePourableToBeaker = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotatePourableToBeaker", 1.5f, "Time it takes to rotate pourable");
			timeToRotateAndMovePourableFromBeakerBack = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateAndMovePourableFromBeakerBack", 0.8f, "Time it takes to move and rotate pourable back");
			waitBetweenMovingPourablesToBeaker = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenMovingPourablesToBeaker", 0.3f, "Wait between moving each pourable to beaker");
			timeToRotateStirRod = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateStirRod", 0.1f, "Time it takes to rotate sir rod (only effects visuals)");
			waitBeforeMovingLabStandDown = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeMovingLabStandDown", 0.5f, "Wait before moving lab stand down");
			timeToMoveLabStandDown = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveLabStandDown", 0.5f, "Time it takes to move lab stand down");
			waitBeforeMovingBeakerToFunnel = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeMovingBeakerToFunnel", 0.5f, "Wait before moving beaker to funnel");
			timeToMoveBeakerToFunnel = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveBeakerToFunnel", 0.8f, "Time it takes to move beaker to funnel");
			timeToRotateBeakerToFunnel = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateBeakerToFunnel", 3f, "Time it takes to rotate beaker");
			waitBeforeMovingLabStandUp = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeMovingLabStandUp", 0.5f, "Wait before moving lab stand up");
			timeToMoveLabStandUp = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveLabStandUp", 0.5f, "Time it takes to move lab stand up");
			waitBeforeHandlingBurner = chemistryStationTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeHandlingBurner", 0.5f, "Wait before handling burner");

			entryIndex = new(0);

			//Lab oven
			labOvenTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_labOvenTimings", "Lab Oven");
			waitBeforeStartingLabOvenTask = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingLabOvenTask", 0.5f, "Wait before starting lab oven task");
			timeToOpenLabOvenDoor = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToOpenLabOvenDoor", 0.5f, "Time it takes to open lab oven door");
			timeToCloseLabOvenDoor = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToCloseLabOvenDoor", 0.5f, "Time it takes to close lab oven door");
			waitBeforeMovingProductsToTray = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeMovingProductsToTray", 1f, "Wait before moving products to tray");
			timeToMoveProductToTray = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveProductToTray", 0.5f, "Time it takes to move product to tray");
			waitBetweenMovingProductsToTray = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenMovingProductsToTray", 0.3f, "Wait between moving each product to tray");
			waitBeforeClosingLabOvenDoorCocaine = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeClosingLabOvenDoorCocaine", 0.5f, "Wait before closing lab oven door when making cocaine");
			waitBeforePressingLabOvenStartButton = labOvenTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforePressingLabOvenStartButton", 0.5f, "Wait before pressing start button");

			entryIndex = new(0);

			//Cauldron
			cauldronTimings = MelonPreferences.CreateCategory($"AutomatedTasksMod_{++categoryIndex}_cauldronTimings", "Cauldron");
			waitBeforeStartingCauldronTask = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeStartingCauldronTask", 0.5f, "Wait before starting cauldron task");
			timeToMoveGasolineToPot = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveGasolineToPot", 1f, "Time it takes to move gasoline to pot");
			timeToRotateGasolineToPot = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateGasolineToPot", 2f, "Time it takes to rotate gasoline");
			timeToRotateAndMoveGasolineFromPotBack = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToRotateAndMoveGasolineFromPotBack", 0.8f, "Time it takes to move and rotate gasoline back");
			waitBeforeMovingProductsToPot = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforeMovingProductsToPot", 0.5f, "Wait before moving products to pot");
			timeToMoveProductToPot = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_timeToMoveProductToPot", 0.5f, "Time it takes to move product to pot");
			waitBetweenMovingProductsToPot = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBetweenMovingProductsToPot", 0.3f, "Wait between moving each product to pot");
			waitBeforePressingCauldronStartButton = cauldronTimings.CreateEntry<float>($"timing_{++entryIndex}_waitBeforePressingCauldronStartButton", 0.5f, "Wait before pressing start button");
		}

		internal static float GetTiming(MelonPreferences_Entry<float> timingPref) {
			return timingsPreset.Value switch {
				SpeedEnum.Custom_Values_Below => timingPref.Value,
				SpeedEnum.Default_Values => timingPref.DefaultValue,
				SpeedEnum.Fast => MathF.Min(timingPref.DefaultValue, 0.2f),
				SpeedEnum.Slow => timingPref.DefaultValue * 1.5f,
				_ => timingPref.Value,
			};
		}
	}
}
