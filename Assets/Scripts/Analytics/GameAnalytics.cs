using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class GameAnalytics {
	
	private const string EditorFilePath = "Assets/StreamingAssets/analytics.csv";
	
	private static string applicationFilePath = Application.streamingAssetsPath + "/analytics.csv";
	private static GameEvents gameEvents;
	private static List<string> keys;
	private static string path;

	public static void StartAnalytics() {
		if(!Directory.Exists("Assets/StreamingAssets"))
		{    
			Directory.CreateDirectory("Assets/StreamingAssets");
		}
		if(!Directory.Exists(Application.streamingAssetsPath))
		{    
			Directory.CreateDirectory(Application.streamingAssetsPath);
		}
		path = Application.isEditor ? EditorFilePath : applicationFilePath;
		keys = GetAllKeys();
		gameEvents = new GameEvents();
	}
	
	public static void AddEvent(PlayerPosition player, string eventKey) {
		gameEvents.Add(player, eventKey);
	}

	public static void FlushEvents() {
		WriteToCsv();
		gameEvents.ClearAll();
	}

	private static List<string> GetAllKeys() {
		var list = new List<string>();
		List<object> fieldValues = typeof(EventKeys).GetFields().Select(field => field.GetValue(typeof(EventKeys))).ToList();
		foreach (object value in fieldValues) {
			list.Add(value.ToString());
		}
		return list;
	}

	private static void WriteToCsv() {
		using (var writer = new StreamWriter(path, true)) {
			string firstLine = DateTime.Now.ToString("d") + ',';
			string secondLine = PlayerPosition.Bottom.ToString().ToUpper() + ',';
			string thirdLine = PlayerPosition.Top.ToString().ToUpper() + ',';
		
			foreach (string key in keys) {
				firstLine += key + ',';
				secondLine += GetValue(gameEvents.bottomPlayerEvents, key) + ',';
				thirdLine += GetValue(gameEvents.topPlayerEvents, key) + ',';
			}
			
			writer.WriteLine(firstLine);
			writer.WriteLine(secondLine);
			writer.WriteLine(thirdLine);
			writer.WriteLine();
		}
	}

	private static string GetValue(Dictionary<string, int> dictionary, string key) {
		return dictionary.ContainsKey(key) ? dictionary[key].ToString() : "0";
	}
}

[Serializable]
public class GameEvents {
	public Dictionary<string, int> topPlayerEvents = new Dictionary<string, int>();
	public Dictionary<string, int> bottomPlayerEvents = new Dictionary<string, int>();

	public void Add(PlayerPosition player, string key) {
		UpdateData(player == PlayerPosition.Bottom ? bottomPlayerEvents : topPlayerEvents, key);
	}

	public void ClearAll() {
		topPlayerEvents.Clear();
		bottomPlayerEvents.Clear();
	}

	private void UpdateData(Dictionary<string, int> data, string key) {
		if (data.ContainsKey(key)) {
			data[key]++;
		}
		else {
			data.Add(key, 1);
		}
	}
}