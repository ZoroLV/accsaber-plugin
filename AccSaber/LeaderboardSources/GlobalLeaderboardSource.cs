using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Models.Reloaded;
using AccSaber.Utils;
using UnityEngine;

namespace AccSaber.LeaderboardSources
{
	internal sealed class GlobalLeaderboardSource : ILeaderboardSource
	{
		private readonly List<List<AccSaberLeaderboardEntry>> _cachedEntries = new();

		private readonly WebUtils _webUtils;

		public GlobalLeaderboardSource(WebUtils webUtils)
		{
			_webUtils = webUtils;
		}

		public string HoverHint => "Global";

		public Task<Sprite> Icon => BeatSaberMarkupLanguage.Utilities.LoadSpriteFromAssemblyAsync("AccSaber.Resources.GlobalIcon.png");

		public bool Scrollable => true;

		public async Task<List<AccSaberLeaderboardEntry>?> GetScoresAsync(AccSaberRankedMap rankedMap, CancellationToken cancellationToken = default, int page = 0)
		{
			if (_cachedEntries.Count >= page + 1)
			{
				return _cachedEntries[page];
			}

			PageResponse<AccSaberReloadedScoreResponse>? response = null;
			foreach (var leaderboardId in rankedMap.GetLeaderboardIds())
			{
				response = await _webUtils.GetAsync<PageResponse<AccSaberReloadedScoreResponse>>(
					AccSaberReloadedApi.GetLeaderboardScores(leaderboardId, page, 10),
					cancellationToken);
				if (response is not null)
				{
					break;
				}
			}

			if (response?.Content is null)
			{
				return null;
			}

			var mappedScores = response.Content.Select(AccSaberLeaderboardEntry.FromReloaded).ToList();
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
