using System;

namespace AccSaber.Utils
{
	internal static class AccSaberReloadedApi
	{
		public const string BaseUrl = "https://api.accsaberreloaded.com/v1";
		public const string TrueAccCategoryId = "b0000000-0000-0000-0000-000000000001";
		public const string StandardAccCategoryId = "b0000000-0000-0000-0000-000000000002";
		public const string TechAccCategoryId = "b0000000-0000-0000-0000-000000000003";
		public const string OverallCategoryId = "b0000000-0000-0000-0000-000000000005";

		public static string GetMapByHash(string songHash, string difficulty) =>
			$"{BaseUrl}/maps/hash/{songHash}?difficulty={Models.AccSaberRankedMap.ToReloadedDifficulty(difficulty)}";

		public static string GetUser(string userId) =>
			$"{BaseUrl}/users/{userId}?statistics=true";

		public static string GetLeaderboardScores(string leaderboardId, int page, int size) =>
			$"{BaseUrl}/maps/difficulties/leaderboard/{Uri.EscapeDataString(leaderboardId)}/scores?page={page}&size={size}";

		public static string GetScoresAround(string leaderboardId, string userId, int above = 4, int below = 5) =>
			$"{BaseUrl}/maps/difficulties/leaderboard/{Uri.EscapeDataString(leaderboardId)}/scores-around/{userId}?above={above}&below={below}";
	}
}
