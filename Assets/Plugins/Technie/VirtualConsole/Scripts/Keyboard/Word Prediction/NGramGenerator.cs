using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.Text;
using System.Linq;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System.Text.RegularExpressions;

namespace Technie.VirtualConsole
{

	public class NGramGenerator : MonoBehaviour
	{
		// Public Properties

		public TextAsset biGramAsset;
		public TextAsset levenshteinAsset;

		public Text[] ButtonLabels;

		// Internal State

		private Dictionary<string, int> biGramDict = new Dictionary<string, int>();
		private Dictionary<string, int> levenshteinDict = new Dictionary<string, int>();
		private List<string> biGramPredictionCorpus = new List<string>();

		private List<string> levenshteinCorpus = new List<string>();

		private void Awake()
		{
			biGramDict = LoadDictionary(biGramAsset);
			levenshteinDict = LoadDictionary(levenshteinAsset);

			levenshteinCorpus = levenshteinDict.Keys.ToList();
		}

		public List<string> GetLevenshteinCorpus()
		{
			return this.levenshteinCorpus;
		}

		private static Dictionary<string, int> LoadDictionary(TextAsset asset)
		{
			Dictionary<string, int> d = new Dictionary<string, int>();

			string[] tokens = asset.text.Split(new char[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < tokens.Length; i += 2)
			{
				string name = tokens[i];
				string freq = tokens[i + 1];

			//	Debug.Log(name + "|" + freq);

				try
				{
					int count = int.Parse(freq);
					if (d.ContainsKey(name))
					{
						d[name] += count;
					}
					else
					{
						d.Add(name, count);
					}
				}
				catch (FormatException fe)
				{
					Debug.LogException(fe);

					string prevTokens = "";
					for (int j=0; j<32; j++)
					{
						string t = tokens[i - j];
						prevTokens += t + "\n";
					}
					Debug.LogError("Previous tokens: " + prevTokens);
					break;
				}
			}

			return d;
		}

		public void PredictNextWords(string input)
		{
			Profiler.BeginSample(string.Format("Build prediction corpus for {0} keys", biGramDict.Count));
			string toFind = input.ToLower() + " ";
			foreach (KeyValuePair<string, int> kvp in biGramDict)
			{
				if (kvp.Key.Contains(toFind))
				{
					biGramPredictionCorpus.Add(kvp.Key.Split(' ')[1]); // input "fish" matched with key "fish sauce" so need to split on the space and just add "sauce" TODO: Pre-calc the second token ("sauce") rather than splitting here
				}
			}
			Profiler.EndSample();

			if (biGramPredictionCorpus.Count < ButtonLabels.Length)
			{
				Profiler.BeginSample("Populate button labels (predictions and leftover levenshtein values)");
				for (int i = 0; i < biGramPredictionCorpus.Count; i++)
				{
					ButtonLabels[i].text = biGramPredictionCorpus[i];
				}

				for (int i = biGramPredictionCorpus.Count; i < ButtonLabels.Length; i++)
				{
					// Don't forget to filter repeating stuff like "to" "to" etc.
					ButtonLabels[i].text = levenshteinCorpus[i - biGramPredictionCorpus.Count];
				}
				Profiler.EndSample();
			}
			else
			{
				Profiler.BeginSample("Populate button labels");
				for (int i = 0; i < ButtonLabels.Length; i++)
				{
					ButtonLabels[i].text = biGramPredictionCorpus[i];
				}
				Profiler.EndSample();
			}
			biGramPredictionCorpus.Clear();
		}
	}
}
