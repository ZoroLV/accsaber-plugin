using System;
using System.Collections.Generic;
using System.Linq;
using AccSaber.Managers;
using AccSaber.Models.Base;
using AccSaber.Models.Reloaded;
using AccSaber.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models
{
	[UsedImplicitly]
	internal sealed class AccSaberRankedMap : Model
	{
		[JsonProperty("songName")]
		public string SongName { get; set; } = null!;

		[JsonProperty("songSubName")]
		public string SongSubName { get; set; } = null!;

		[JsonProperty("songAuthorName")]
		public string SongAuthorName { get; set; } = null!;

		[JsonProperty("levelAuthorName")]
		public string LevelAuthorName { get; set; } = null!;

		[JsonProperty("difficulty")]
		public string Difficulty { get; set; } = null!;

		[JsonProperty("leaderboardId")]
		public string LeaderboardId { get; set; } = null!;

		[JsonIgnore]
		public string AlternateLeaderboardId { get; set; } = string.Empty;

		[JsonProperty("beatSaverKey")]
		public string BeatSaverKey { get; set; } = null!;

		[JsonProperty("songHash")]
		public string SongHash { get; set; } = null!;

		[JsonProperty("complexity")]
		public float Complexity { get; set; }

		[JsonProperty("categoryDisplayName")]
		public string CategoryDisplayName { get; set; } = null!;

		[JsonProperty("dateRanked")]
		public DateTime DateRanked { get; set; }

		[JsonIgnore]
		public AccSaberStore.AccSaberMapCategories Category;

		public IEnumerable<string> GetLeaderboardIds()
		{
			if (!string.IsNullOrWhiteSpace(LeaderboardId))
			{
				yield return LeaderboardId;
			}

			if (!string.IsNullOrWhiteSpace(AlternateLeaderboardId) &&
			    !string.Equals(AlternateLeaderboardId, LeaderboardId, StringComparison.Ordinal))
			{
				yield return AlternateLeaderboardId;
			}
		}

		public static AccSaberRankedMap? FromReloaded(AccSaberReloadedMapResponse mapResponse, string difficulty)
		{
			var normalizedDifficulty = NormalizeDifficulty(difficulty);
			var rankedDifficulty = mapResponse.Difficulties.FirstOrDefault(x =>
				x.Active &&
				string.Equals(x.Status, "RANKED", StringComparison.OrdinalIgnoreCase) &&
				string.Equals(x.Characteristic, "Standard", StringComparison.OrdinalIgnoreCase) &&
				NormalizeDifficulty(x.Difficulty) == normalizedDifficulty);

			if (rankedDifficulty is null || !TryGetCategory(rankedDifficulty.CategoryId, out var category, out var categoryDisplayName))
			{
				return null;
			}

			var primaryLeaderboardId = !string.IsNullOrWhiteSpace(rankedDifficulty.SsLeaderboardId)
				? rankedDifficulty.SsLeaderboardId
				: rankedDifficulty.BlLeaderboardId;
			var alternateLeaderboardId = !string.IsNullOrWhiteSpace(rankedDifficulty.BlLeaderboardId) &&
			                            !string.Equals(rankedDifficulty.BlLeaderboardId, primaryLeaderboardId, StringComparison.Ordinal)
				? rankedDifficulty.BlLeaderboardId
				: string.Empty;

			if (string.IsNullOrWhiteSpace(primaryLeaderboardId))
			{
				return null;
			}

			return new AccSaberRankedMap
			{
				SongName = mapResponse.SongName,
				SongSubName = mapResponse.SongSubName ?? string.Empty,
				SongAuthorName = mapResponse.SongAuthor,
				LevelAuthorName = mapResponse.MapAuthor,
				Difficulty = rankedDifficulty.Difficulty,
				LeaderboardId = primaryLeaderboardId,
				AlternateLeaderboardId = alternateLeaderboardId,
				BeatSaverKey = mapResponse.BeatSaverCode ?? string.Empty,
				SongHash = mapResponse.SongHash,
				Complexity = (float)rankedDifficulty.Complexity,
				CategoryDisplayName = categoryDisplayName,
				DateRanked = rankedDifficulty.RankedAt,
				Category = category
			};
		}

		public static string CreateLookupKey(string songHash, string difficulty)
		{
			return $"{songHash.ToLowerInvariant()}/{NormalizeDifficulty(difficulty)}";
		}

		public static string NormalizeDifficulty(string difficulty)
		{
			return difficulty.Replace("_", string.Empty).Replace(" ", string.Empty).ToLowerInvariant() switch
			{
				"easy" => "easy",
				"normal" => "normal",
				"hard" => "hard",
				"expert" => "expert",
				"expertplus" => "expertplus",
				_ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
			};
		}

		public static string ToReloadedDifficulty(string difficulty)
		{
			return NormalizeDifficulty(difficulty) switch
			{
				"easy" => "EASY",
				"normal" => "NORMAL",
				"hard" => "HARD",
				"expert" => "EXPERT",
				"expertplus" => "EXPERT_PLUS",
				_ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
			};
		}

		private static bool TryGetCategory(string categoryId, out AccSaberStore.AccSaberMapCategories category, out string displayName)
		{
			switch (categoryId)
			{
				case AccSaberReloadedApi.TrueAccCategoryId:
					category = AccSaberStore.AccSaberMapCategories.True;
					displayName = "True Acc";
					return true;
				case AccSaberReloadedApi.StandardAccCategoryId:
					category = AccSaberStore.AccSaberMapCategories.Standard;
					displayName = "Standard Acc";
					return true;
				case AccSaberReloadedApi.TechAccCategoryId:
					category = AccSaberStore.AccSaberMapCategories.Tech;
					displayName = "Tech Acc";
					return true;
				default:
					category = default;
					displayName = string.Empty;
					return false;
			}
		}
	}
}
