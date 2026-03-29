using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models.Reloaded
{
	[UsedImplicitly]
	internal sealed class AccSaberReloadedScoreResponse
	{
		[JsonProperty("accuracy")]
		public double Accuracy { get; set; }

		[JsonProperty("ap")]
		public double Ap { get; set; }

		[JsonProperty("rank")]
		public int Rank { get; set; }

		[JsonProperty("score")]
		public int Score { get; set; }

		[JsonProperty("timeSet")]
		public DateTime TimeSet { get; set; }

		[JsonProperty("userId")]
		public string UserId { get; set; } = string.Empty;

		[JsonProperty("userName")]
		public string UserName { get; set; } = string.Empty;
	}
}
