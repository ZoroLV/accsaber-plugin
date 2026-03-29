using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models.Reloaded
{
	[UsedImplicitly]
	internal sealed class AccSaberReloadedUserCategoryStatisticsResponse
	{
		[JsonProperty("ap")]
		public double Ap { get; set; }

		[JsonProperty("averageAcc")]
		public double AverageAcc { get; set; }

		[JsonProperty("averageAp")]
		public double AverageAp { get; set; }

		[JsonProperty("categoryId")]
		public string CategoryId { get; set; } = string.Empty;

		[JsonProperty("rankedPlays")]
		public int RankedPlays { get; set; }

		[JsonProperty("ranking")]
		public int Ranking { get; set; }
	}
}
