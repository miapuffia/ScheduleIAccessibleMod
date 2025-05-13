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
	[HarmonyPatch(typeof(BrickPressCanvas), "BeginButtonPressed")]
	internal static class BrickPressCanvasPatch {
		private static void Prefix(BrickPressCanvas __instance) {
			if(Prefs.brickPressToggle.Value) {
				MelonCoroutines.Start(AutomateBrickPressCoroutine(__instance));
			} else {
				Melon<Mod>.Logger.Msg("Automate brick press station disabled in settings");
			}
		}

		private static System.Collections.IEnumerator AutomateBrickPressCoroutine(BrickPressCanvas brickPressCanvas) {
			BrickPress brickPress;
			Vector3 positionModifier;
			bool stepComplete;
			bool isInUse;
			bool isError = false;
			float time;

			float _waitBeforeStartingBrickPressTask = Prefs.GetTiming(Prefs.waitBeforeStartingBrickPressTask);
			float _timeToMoveProductsToMoldUp = Prefs.GetTiming(Prefs.timeToMoveProductsToMoldUp);
			float _timeToMoveProductsToMoldRight = Prefs.GetTiming(Prefs.timeToMoveProductsToMoldRight);
			float _waitBeforePullingDownHandle = Prefs.GetTiming(Prefs.waitBeforePullingDownHandle);
			float _timeToPullDownHandle = Prefs.GetTiming(Prefs.timeToPullDownHandle);

			Melon<Mod>.Logger.Msg("Brick press task started");

			if(Utils.NullCheck([brickPressCanvas, brickPressCanvas?.Press], "Can't find brick press - probably exited task"))
				yield break;

			brickPress = brickPressCanvas.Press;

			yield return new WaitForSeconds(_waitBeforeStartingBrickPressTask);

			Melon<Mod>.Logger.Msg("Moving products up");

			if(Utils.NullCheck([brickPress, brickPress?.ContainerSpawnPoint, brickPress?.MouldDetection], "Can't find brick press requirements"))
				yield break;

			IEnumerable<FunctionalProduct> products = GameObject.FindObjectsOfType<FunctionalProduct>().Where(d => d.transform.position.MaxComponentDifference(brickPress.ContainerSpawnPoint.transform.position) < 1f);

			if(!products.Any()) {
				Melon<Mod>.Logger.Msg("Can't find products - probably exited task");
				yield break;
			}

			foreach(FunctionalProduct product in products) {
				product.GetComponent<Rigidbody>().useGravity = false;
			}

			positionModifier = new Vector3(0, 0.3f, 0);

			isError = false;

			yield return Utils.SinusoidalLerpPositionsCoroutine([.. products.Select(f => f.transform)], positionModifier, _timeToMoveProductsToMoldUp, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
				yield break;
			}

			Melon<Mod>.Logger.Msg("Moving products right");

			if(Utils.NullCheck([brickPress, brickPress?.ContainerSpawnPoint, brickPress?.MouldDetection], "Can't find mold - probably exited task"))
				yield break;

			if(!products.Any()) {
				Melon<Mod>.Logger.Msg("Can't find products - probably exited task");
				yield break;
			}

			positionModifier = new Vector3(brickPress.MouldDetection.transform.position.x - brickPress.ContainerSpawnPoint.position.x, 0, brickPress.MouldDetection.transform.position.z - brickPress.ContainerSpawnPoint.position.z);

			isError = false;

			yield return Utils.SinusoidalLerpPositionsCoroutine([.. products.Select(f => f.transform)], positionModifier, _timeToMoveProductsToMoldRight, () => isError = true);

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
				yield break;
			}

			foreach(FunctionalProduct product in products) {
				product.GetComponent<Rigidbody>().useGravity = true;
			}

			Melon<Mod>.Logger.Msg("Waiting for products to be in the mold");

			stepComplete = false;
			time = 0;

			//Up to 3 seconds
			while(time < 3) {
				GetIsBrickPressInUse(brickPress, out isInUse, ref isError);

				if(isError || !isInUse) {
					Melon<Mod>.Logger.Msg("Can't find brick press - probably exited task");
					yield break;
				}

				if(brickPress.GetProductInMould().Count >= 20) {
					Melon<Mod>.Logger.Msg("Products in mold");
					stepComplete = true;
					break;
				}

				time += Time.deltaTime;

				yield return null;
			}

			if(!stepComplete) {
				Melon<Mod>.Logger.Msg("Products are not in the mold after 3 seconds");
				yield break;
			}

			yield return new WaitForSeconds(_waitBeforePullingDownHandle);

			Melon<Mod>.Logger.Msg("Pulling down handle");

			isError = false;

			yield return Utils.LerpFloatCallbackCoroutine(0, 1, _timeToPullDownHandle, f => {
				GetIsBrickPressInUse(brickPress, out isInUse, ref isError);

				if(isError || Utils.NullCheck(brickPress?.Handle) || !isInUse) {
					isError = true;
					return false;
				}

				brickPress.Handle.CurrentPosition = f;

				return true;
			});

			if(isError) {
				Melon<Mod>.Logger.Msg("Can't find handle to move - probably exited task");
				yield break;
			}

			if(Utils.NullCheck([brickPress, brickPress?.Handle], "Can't find handle - probably exited task"))
				yield break;

			brickPress.Handle.CurrentPosition = 2;

			Melon<Mod>.Logger.Msg("Done with brick press");
		}

		private static void GetIsBrickPressInUse(BrickPress brickPress, out bool isInUse, ref bool isError) {
			if(Utils.NullCheck([brickPress, brickPress?.Container1])) {
				isError = true;
				isInUse = false;
				return;
			}

			isError = false;
			isInUse = !brickPress.Container1.gameObject.activeSelf;
		}
	}
}
