using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Packaging;
using Il2CppScheduleOne.PlayerScripts;
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
		private static void Postfix(CauldronCanvas __instance) {
			if(Prefs.brickPressToggle.Value) {
				MelonCoroutines.Start(AutomateBrickPressCoroutine());
			} else {
				Melon<Mod>.Logger.Msg("Automate brick press station disabled in settings");
			}
		}

		static System.Collections.IEnumerator AutomateBrickPressCoroutine() {
			BrickPress brickPress;
			Vector3 positionModifier;
			bool callbackError;

			float _waitBeforeStartingBrickPressTask = Prefs.GetTiming(Prefs.waitBeforeStartingBrickPressTask);
			float _timeToMoveProductsToMoldUp = Prefs.GetTiming(Prefs.timeToMoveProductsToMoldUp);
			float _timeToMoveProductsToMoldRight = Prefs.GetTiming(Prefs.timeToMoveProductsToMoldRight);
			float _waitBeforePullingDownHandle = Prefs.GetTiming(Prefs.waitBeforePullingDownHandle);
			float _timeToPullDownHandle = Prefs.GetTiming(Prefs.timeToPullDownHandle);

			Melon<Mod>.Logger.Msg("Brick press task started");

			yield return new WaitForSeconds(_waitBeforeStartingBrickPressTask);

			brickPress = GameObject.FindObjectsOfType<BrickPress>().FirstOrDefault(b => b.PlayerUserObject?.GetComponent<Player>().IsLocalPlayer ?? false);

			if(Utils.NullCheck([brickPress, brickPress?.ContainerSpawnPoint, brickPress?.MouldDetection], "Can't find the brick press the player is using"))
				yield break;

			Melon<Mod>.Logger.Msg("Moving products up");

			IEnumerable<FunctionalProduct> products = GameObject.FindObjectsOfType<FunctionalProduct>().Where(d => d.transform.position.MaxComponentDifference(brickPress.ContainerSpawnPoint.transform.position) < 1f);

			if(!products.Any()) {
				Melon<Mod>.Logger.Msg("Can't find products - probably exited task");
				yield break;
			}

			foreach(FunctionalProduct product in products) {
				product.GetComponent<Rigidbody>().useGravity = false;
			}

			positionModifier = new Vector3(0, 0.3f, 0);

			callbackError = false;

			yield return Utils.SinusoidalLerpPositionsCoroutine([.. products.Select(f => f.transform)], positionModifier, _timeToMoveProductsToMoldUp, () => callbackError = true);

			if(callbackError) {
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

			callbackError = false;

			yield return Utils.SinusoidalLerpPositionsCoroutine([.. products.Select(f => f.transform)], positionModifier, _timeToMoveProductsToMoldRight, () => callbackError = true);

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find product to move - probably exited task");
				yield break;
			}

			foreach(FunctionalProduct product in products) {
				product.GetComponent<Rigidbody>().useGravity = true;
			}

			yield return new WaitForSeconds(_waitBeforePullingDownHandle);

			Melon<Mod>.Logger.Msg("Pulling down handle");

			callbackError = false;

			yield return Utils.LerpFloatCallbackCoroutine(0, 1, _timeToPullDownHandle, f => {
				if(Utils.NullCheck([brickPress, brickPress?.Handle])) {
					callbackError = true;
					return false;
				}

				brickPress.Handle.CurrentPosition = f;

				return true;
			});

			if(callbackError) {
				Melon<Mod>.Logger.Msg("Can't find handle to move - probably exited task");
				yield break;
			}

			if(Utils.NullCheck([brickPress, brickPress?.Handle], "Can't find handle - probably exited task"))
				yield break;

			brickPress.Handle.CurrentPosition = 2;

			Melon<Mod>.Logger.Msg("Done with brick press");
		}
	}
}
