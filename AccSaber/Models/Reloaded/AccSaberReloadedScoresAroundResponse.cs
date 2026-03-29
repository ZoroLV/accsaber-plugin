using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models.Reloaded
{
	[UsedImplicitly]
	internal sealed class AccSaberReloadedScoresAroundResponse
	{
		[JsonProperty("playerScore")]
		public AccSaberReloadedScoreResponse? PlayerScore { get; set; }

		[JsonProperty("scoresAbove")]
		public List<AccSaberReloadedScoreResponse> ScoresAbove { get; set; } = new();

		[JsonProperty("scoresBelow")]
		public List<AccSaberReloadedScoreResponse> ScoresBelow { get; set; } = new();
	}
}
