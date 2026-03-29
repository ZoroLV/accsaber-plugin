using System;
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
	internal class AccSaberUser : Model
	{
		[JsonProperty("rank")]
		public int Rank { get; set; }

		[JsonProperty("playerId")]
		public string PlayerId { get; set; } = null!;

		[JsonProperty("playerName")]
		public string PlayerName { get; set; } = null!;

		[JsonProperty("avatarUrl")]
		public string AvatarUrl { get; set; } = null!;

		[JsonProperty("country")]
		public string Country { get; set; } = string.Empty;

		[JsonProperty("hmd")]
		public string Hmd { get; set; } = string.Empty;

		[JsonProperty("averageAcc")]
		public float AverageAcc { get; set; }

		[JsonProperty("ap")]
		public float AP { get; set; }

		[JsonProperty("rankedPlays")]
		public int RankedPlays { get; set; }

		public static AccSaberUser FromReloaded(AccSaberReloadedUserResponse profile, AccSaberStore.AccSaberMapCategories? category = null)
		{
			var statistics = profile.Statistics.FirstOrDefault(x => string.Equals(x.CategoryId, GetCategoryId(category), StringComparison.OrdinalIgnoreCase));

			return new AccSaberUser
			{
				PlayerId = profile.Id,
				PlayerName = profile.Name,
				AvatarUrl = profile.AvatarUrl,
				Country = profile.Country ?? string.Empty,
				Hmd = profile.Hmd ?? string.Empty,
				AverageAcc = statistics is null ? 0 : (float)statistics.AverageAcc,
				AP = statistics is null ? 0 : (float)statistics.Ap,
				RankedPlays = statistics?.RankedPlays ?? 0,
				Rank = statistics?.Ranking ?? 0
			};
		}

		private static string GetCategoryId(AccSaberStore.AccSaberMapCategories? category)
		{
			return category switch
			{
				AccSaberStore.AccSaberMapCategories.True => AccSaberReloadedApi.TrueAccCategoryId,
				AccSaberStore.AccSaberMapCategories.Standard => AccSaberReloadedApi.StandardAccCategoryId,
				AccSaberStore.AccSaberMapCategories.Tech => AccSaberReloadedApi.TechAccCategoryId,
				null => AccSaberReloadedApi.OverallCategoryId,
				_ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
			};
		}
	}
}
