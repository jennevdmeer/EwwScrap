﻿using System;
using System.Collections.Generic;
using AwwScrap.Helpers;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;

namespace AwwScrap.Networking
{
	public static class ChatMessages
	{
		internal const string ChatCommandPrefix = "/eemdev";
		private const string HelpPrefix = "help";
		private const string GetCivlStandingsPrefix = "getcivlstandings";
		private const string ShowDebugLogPrefix = "showdebuglog";
		private const string ShowProfilingLogPrefix = "showprofilinglog";
		private const string ShowGeneralLogPrefix = "showgenerallog";
		private const string SpawnTestPrefabPrefix = "spawntestprefab";
		private const string DespawnTestPrefabPrefix = "despawntestprefab";
		private const string ProfilePrefabsPrefix = "profileprefabs";

		private static readonly Dictionary<string, Action<string>> ChatAction = new Dictionary<string, Action<string>>
		{
			{HelpPrefix, PrintHelpCommands},
			{ShowDebugLogPrefix, ShowDebugLog}, {ShowGeneralLogPrefix, ShowGeneralLog},
			{ShowProfilingLogPrefix, ShowProfilingLog}
		};

		public static void HandleChatMessage(string message)
		{

			IMyPlayer localPlayer = MyAPIGateway.Session.Player;

			if (localPlayer.PromoteLevel < MyPromoteLevel.Admin)
			{
				Messaging.ShowLocalNotification($"You must be an Administrator to invoke EEM Chat Commands.  Current Rank: {localPlayer.PromoteLevel.ToString()}");
				return;
			}

			string[] chatCommand = message.Split(' ');

			if (chatCommand.Length < 2)
			{
				PrintHelpCommands("");
				return;
			}

			Action<string> action;
			string actionText = null;

			if (chatCommand.Length > 2)
				actionText = chatCommand[2];

			if (ChatAction.TryGetValue(chatCommand[1], out action))
				action?.Invoke(actionText);
			else PrintHelpCommands("");
		}

		/// <summary>
		/// Prints a list of available commands
		/// </summary>  
		private static void PrintHelpCommands(string s)
		{
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {HelpPrefix}' will show this message");
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {GetCivlStandingsPrefix}' will show the standings between CIVL and all other lawful factions");
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {ShowDebugLogPrefix}' will show the last 20 lines of the Debug Log");
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {ShowProfilingLogPrefix}' will show the last 20 lines of the Profiling Log");
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {ShowGeneralLogPrefix}' will show the last 20 lines of the General Log");
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {SpawnTestPrefabPrefix}' will spawn a test prefab 1k away from the player (not yet implemented)");
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {DespawnTestPrefabPrefix}' will despawn the test prefab (not yet implemented)");
			Messaging.ShowLocalNotification($"'{ChatCommandPrefix} {ProfilePrefabsPrefix}' will profile all loaded prefabs - WARNING! This is not quick (not yet implemented)");
		}

		private static void ShowDebugLog(string s)
		{
			if (Constants.DebugMode)
				AwwScrapSession.DebugLog.GetTailMessages();
			else Messaging.ShowLocalNotification("Debug mode is not enabled");
		}

		private static void ShowGeneralLog(string s)
		{
			if (Constants.EnableGeneralLog)
				AwwScrapSession.GeneralLog.GetTailMessages();
			else Messaging.ShowLocalNotification("General logging is not enabled");
		}

		private static void ShowProfilingLog(string s)
		{
			if (Constants.EnableProfilingLog)
				AwwScrapSession.ProfilingLog.GetTailMessages();
			else Messaging.ShowLocalNotification("Profiling is not enabled");
		}
	}
}