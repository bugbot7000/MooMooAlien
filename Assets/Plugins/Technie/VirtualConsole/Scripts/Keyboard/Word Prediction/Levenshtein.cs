using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Profiling;

namespace Technie.VirtualConsole
{
	public class Levenshtein : MonoBehaviour
	{
		// Inspector Properties

		public NGramGenerator NGramHandler;
		public Text[] ButtonLabels;

		// Internal State

		private const int maxWordLength = 15;
		private const int maxLevenshteinCost = 7;
		private const int minLevenshteinCost = 1;
		private List<string> corpus = new List<string>();
		private bool isUppercase = false;

		public static class LevenshteinDistance
		{
			public static int Compute(string s, string t)
			{
				int n = s.Length;
				int m = t.Length;

				if (n == 0)
				{
					return m;
				}

				if (m == 0)
				{
					return n;
				}

				int[,] d = new int[n + 1, m + 1];

				for (int i = 0; i <= n; d[i, 0] = i++) {}

				for (int j = 0; j <= m; d[0, j] = j++) {}

				for (int i = 1; i <= n; i++)
				{
					for (int j = 1; j <= m; j++)
					{
						int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
						d[i, j] = Math.Min(
							Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
							d[i - 1, j - 1] + cost);
					}
				}
				return d[n, m];
			}
		}

		void Start()
		{
			corpus = NGramHandler.GetLevenshteinCorpus(); ;
			for (int i = 0; i < ButtonLabels.Length; i++)
			{
				ButtonLabels[i].text = corpus[i];
			}
		}

		// Triggered from the output text field every time it's changed, so we can reevaluate the auto-complete suggestions based on the new letter
		public void RunAutoComplete(string input)
		{
			if (input.Length > 0)
			{
				char[] lastChar = input.Substring(input.Length - 1).ToCharArray();
				string lastWord = input.Split(' ').Last();
				
				bool isFirstLetterUpper = IsFirstCharUppercase(lastWord);

				if (!char.IsWhiteSpace(lastChar[0]))
				{
					if (lastWord.Length < maxWordLength)
					{
						if (input.Length >= 0)
						{
							Dictionary<int, int> dict = new Dictionary<int, int>();

							Profiler.BeginSample(string.Format("Build cost dictionary for corpus of {0} entries", corpus.Count));
							string lastWordLower = lastWord.ToLower();
							for (int i = 0; i < corpus.Count; i++)
							{
								int cost = LevenshteinDistance.Compute(lastWordLower, corpus[i]);
								if (cost >= minLevenshteinCost && cost <= maxLevenshteinCost)
								{
									dict.Add(i, cost);
								}
							}
							Profiler.EndSample();

							if (lastWord.All(char.IsUpper))
							{
								isUppercase = true;
							}
							if (lastWord.Any(char.IsLower))
							{
								isUppercase = false;
							}

							Profiler.BeginSample(string.Format("Sort dictionary by distance - dictionary has {0} entries", dict.Count));
							List<int> distanceOrder = dict.OrderBy(kp => kp.Value).Select(kp => kp.Key).ToList();
							Profiler.EndSample();

							// Push the top three entries to the auto-complete button labels
							for (int i = 0; i < distanceOrder.Count; i++)
							{
								if (i < ButtonLabels.Length)
								{
									if (isUppercase)
									{
										ButtonLabels[i].text = corpus[distanceOrder[i]].ToUpper();
									}
									else if (isFirstLetterUpper && isUppercase == false)
									{
										ButtonLabels[i].text = char.ToUpper(corpus[distanceOrder[i]][0]) + corpus[distanceOrder[i]].Substring(1);
									}
									else if (!isUppercase && isFirstLetterUpper == false)
									{
										ButtonLabels[i].text = corpus[distanceOrder[i]].ToLower();
									}
								}
							}

						}
					}
				}
			}
		}

		private static bool IsFirstCharUppercase(string input)
		{
			bool isUpper = false;

			char[] firstCharOfLastWord = input.Substring(0).ToCharArray();
			if (firstCharOfLastWord.Length >= 1)
			{
				if (firstCharOfLastWord[0].ToString().Any(char.IsUpper))
				{
					isUpper = true;
				}
				else
				{
					isUpper = false;
				}
			}

			return isUpper;
		}
	}
}