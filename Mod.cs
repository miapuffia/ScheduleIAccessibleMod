using HarmonyLib;
using Il2CppGameKit.Utilities;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.Interaction;
using Il2CppScheduleOne.ItemFramework;
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

[assembly: MelonInfo(typeof(AutomatedTasksMod.Mod), "AutomatedTasksMod", "1.0.0", "Robert Rioja")]
[assembly: MelonColor(1, 255, 20, 147)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace AutomatedTasksMod {
	public class Mod : MelonMod {
		public override void OnInitializeMelon() {
			Prefs.SetupPrefs();
		}
	}
}
