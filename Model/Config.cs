using System;
using System.IO;
using Newtonsoft.Json;

namespace easysave.Model
{
	public class Config
	{
		private string jsonFilePath;

		public Config()
		{
			string solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName; // Get the solution directory
            jsonFilePath = Path.Combine(solutionDir, @"Config\AppConfig.json"); // Combine the solution directory with the path to the JSON file
		}

		public AppConfigData LoadFromFile()
		{
			if (File.Exists(jsonFilePath))
			{
				Console.WriteLine("Loading AppConfigData from file: " + jsonFilePath);
				string jsonString = File.ReadAllText(jsonFilePath);
				return JsonConvert.DeserializeObject<AppConfigData>(jsonString);
			}
			else
			{
				throw new FileNotFoundException("Le fichier JSON spécifié est introuvable.", jsonFilePath);
			}
		}
	}
}