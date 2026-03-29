using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models.Reloaded
{
	[UsedImplicitly]
	internal sealed class AccSaberReloadedMapDifficultyResponse
	{
		[JsonProperty("active")]
		public bool Active { get; set; }

		[JsonProperty("blLeaderboardId")]
		public string BlLeaderboardId { get; set; } = string.Empty;

		[JsonProperty("categoryId")]
		public string CategoryId { get; set; } = string.Empty;

		[JsonProperty("characteristic")]
		public string Characteristic { get; set; } = string.Empty;

		[JsonProperty("complexity")]
		public double Complexity { get; set; }

		[JsonProperty("difficulty")]
		public string Difficulty { get; set; } = string.Empty;

		[JsonProperty("rankedAt")]
		public DateTime RankedAt { get; set; }

		[JsonProperty("ssLeaderboardId")]
		public string SsLeaderboardId { get; set; } = string.Empty;

		[JsonProperty("status")]
		public string Status { get; set; } = string.Empty;
	}
}
