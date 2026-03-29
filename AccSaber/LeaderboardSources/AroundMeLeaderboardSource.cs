using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.Models.Reloaded;
using AccSaber.Utils;
using UnityEngine;

namespace AccSaber.LeaderboardSources
{
	internal sealed class AroundMeLeaderboardSource : ILeaderboardSource
	{
		private readonly List<List<AccSaberLeaderboardEntry>> _cachedEntries = new();

		private readonly WebUtils _webUtils;
		private readonly AccSaberStore _accSaberStore;

		public AroundMeLeaderboardSource(WebUtils webUtils, AccSaberStore accSaberStore)
		{
			_webUtils = webUtils;
			_accSaberStore = accSaberStore;
		}

		public string HoverHint => "Around Me";

		public Task<Sprite> Icon => BeatSaberMarkupLanguage.Utilities.LoadSpriteFromAssemblyAsync("AccSaber.Resources.PlayerIcon.png");

		public bool Scrollable => false;
		public async Task<List<AccSaberLeaderboardEntry>?> GetScoresAsync(AccSaberRankedMap rankedMap, CancellationToken cancellationToken = default, int page = 0)
		{
			if (_cachedEntries.Count >= page + 1)
			{
				return _cachedEntries[page];
			}

			var userInfo = await _accSaberStore.GetPlatformUserInfo();
			if (userInfo is null)
			{
				return null;
			}

			AccSaberReloadedScoresAroundResponse? response = null;
			foreach (var leaderboardId in rankedMap.GetLeaderboardIds())
			{
				response = await _webUtils.GetAsync<AccSaberReloadedScoresAroundResponse>(
					AccSaberReloadedApi.GetScoresAround(leaderboardId, userInfo.platformUserId),
					cancellationToken,
					allowNotFound: true);
				if (response is not null)
				{
					break;
				}
			}

			if (response?.PlayerScore is null)
			{
				return new List<AccSaberLeaderboardEntry>();
			}

			var mappedScores = response.ScoresAbove
				.Concat(new[] { response.PlayerScore })
				.Concat(response.ScoresBelow)
				.Select(AccSaberLeaderboardEntry.FromReloaded)
				.OrderBy(x => x.Rank)
				.ToList();

			_cachedEntries.Add(mappedScores);
			return mappedScores;
		}

		public List<AccSaberLeaderboardEntry>? GetCachedScore(int page)
		{
			return page >= 0 && page < _cachedEntries.Count ? _cachedEntries[page] : null;
		}

		public List<AccSaberLeaderboardEntry>? GetLatestCachedScore()
		{
			return _cachedEntries.LastOrDefault();
		}

		public void ClearCache()
		{
			_cachedEntries.Clear();
		}
	}
}
