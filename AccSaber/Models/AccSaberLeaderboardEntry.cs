using System;
using AccSaber.Models.Base;
using AccSaber.Models.Reloaded;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models
{
	[UsedImplicitly]
	internal sealed class AccSaberLeaderboardEntry : Model
	{
		[JsonProperty("rank")]
		public int Rank { get; set; }

		[JsonProperty("playerId")]
		public string PlayerId { get; set; } = null!;

		[JsonProperty("playerName")]
		public string PlayerName { get; set; } = null!;

		[JsonProperty("accuracy")]
		public float Accuracy { get; set; }

		[JsonProperty("score")]
		public int Score { get; set; }

		[JsonProperty("ap")]
		public float AP { get; set; }

		[JsonProperty("timeSet")]
		public DateTime TimeSet { get; set; }

		public static AccSaberLeaderboardEntry FromReloaded(AccSaberReloadedScoreResponse score)
		{
			return new AccSaberLeaderboardEntry
			{
				Rank = score.Rank,
				PlayerId = score.UserId,
				PlayerName = score.UserName,
				Accuracy = (float)score.Accuracy,
				Score = score.Score,
				AP = (float)score.Ap,
				TimeSet = score.TimeSet
			};
		}
	}
}
