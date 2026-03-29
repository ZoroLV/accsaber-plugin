using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Models.Reloaded;
using AccSaber.Utils;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Managers
{
	internal sealed class AccSaberStore : IInitializable
	{
		private readonly SiraLog _log;
		private readonly WebUtils _webUtils;
		private readonly IPlatformUserModel _platformUserModel;
		private readonly SemaphoreSlim _userRefreshSemaphore = new(1, 1);

		public event Action<AccSaberRankedMap?>? OnAccSaberRankedMapUpdated;
		public event Action? OnUpdatingFromAccSaberAPI;
		public event Action<bool>? OnUpdatedFromAccSaberAPI;

		public Dictionary<string, AccSaberRankedMap> RankedMaps = new();
		private AccSaberUser _currentUserOverall = new();
		private AccSaberUser _currentUserTrue = new();
		private AccSaberUser _currentUserStandard = new();
		private AccSaberUser _currentUserTech = new();
		public DateTime LastLocalUpdateTime { get; private set; } = DateTime.MinValue;

		private AccSaberRankedMap? _currentRankedMap;

		public AccSaberStore(SiraLog log, WebUtils webUtils, IPlatformUserModel platformUserModel)
		{
			_log = log;
			_webUtils = webUtils;
			_platformUserModel = platformUserModel;
		}

		public enum AccSaberMapCategories
		{
			True,
			Standard,
			Tech
		}

		public AccSaberRankedMap? CurrentRankedMap
		{
			get => _currentRankedMap;
			set
			{
				_currentRankedMap = value;
				OnAccSaberRankedMapUpdated?.Invoke(_currentRankedMap);
			}
		}

		public async Task<AccSaberRankedMap?> GetRankedMapAsync(string songHash, string difficulty, CancellationToken cancellationToken = default)
		{
			var lookupKey = AccSaberRankedMap.CreateLookupKey(songHash, difficulty);
			if (RankedMaps.TryGetValue(lookupKey, out var cachedMap))
			{
				return cachedMap;
			}

			var response = await _webUtils.GetAsync<AccSaberReloadedMapResponse>(
				AccSaberReloadedApi.GetMapByHash(songHash, difficulty),
				cancellationToken,
				allowNotFound: true);
			if (response is null)
			{
				return null;
			}

			var rankedMap = AccSaberRankedMap.FromReloaded(response, difficulty);
			if (rankedMap is null)
			{
				return null;
			}

			RankedMaps[lookupKey] = rankedMap;
			return rankedMap;
		}

		private async Task<AccSaberReloadedUserResponse?> GetUserProfile(string id, CancellationToken cancellationToken = default)
		{
			return await _webUtils.GetAsync<AccSaberReloadedUserResponse>(AccSaberReloadedApi.GetUser(id), cancellationToken, allowNotFound: true);
		}

		private async Task<bool> UpdateAccSaberInfo()
		{
			await _userRefreshSemaphore.WaitAsync();
			try
			{
				OnUpdatingFromAccSaberAPI?.Invoke();

				var platformUser = await GetPlatformUserInfo();
				if (platformUser is null)
				{
					_log.Error("platformUser is null");
					OnUpdatedFromAccSaberAPI?.Invoke(false);
					return false;
				}

				var response = await GetUserProfile(platformUser.platformUserId);
				if (response is null)
				{
					_log.Error($"Failed to get user {platformUser.platformUserId} from AccSaber Reloaded API");
					OnUpdatedFromAccSaberAPI?.Invoke(false);
					return false;
				}

				var newOverall = AccSaberUser.FromReloaded(response);
				var newTrue = AccSaberUser.FromReloaded(response, AccSaberMapCategories.True);
				var newStandard = AccSaberUser.FromReloaded(response, AccSaberMapCategories.Standard);
				var newTech = AccSaberUser.FromReloaded(response, AccSaberMapCategories.Tech);
				var isUpdated = LastLocalUpdateTime == DateTime.MinValue || HasUserChanged(_currentUserOverall, newOverall);

				_currentUserOverall = newOverall;
				_currentUserTrue = newTrue;
				_currentUserStandard = newStandard;
				_currentUserTech = newTech;
				LastLocalUpdateTime = DateTime.UtcNow;

				OnUpdatedFromAccSaberAPI?.Invoke(isUpdated);
				return isUpdated;
			}
			finally
			{
				_userRefreshSemaphore.Release();
			}
		}

		public Task<AccSaberUser> GetCurrentUser(AccSaberMapCategories? category = null)
		{
			return Task.FromResult(category switch
			{
				AccSaberMapCategories.True => _currentUserTrue,
				AccSaberMapCategories.Standard => _currentUserStandard,
				AccSaberMapCategories.Tech => _currentUserTech,
				null => _currentUserOverall,
				_ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
			});
		}

		public async Task<AccSaberUser> GetUserFromId(string id, AccSaberMapCategories? category = null)
		{
			var response = await GetUserProfile(id);
			if (response != null)
			{
				return AccSaberUser.FromReloaded(response, category);
			}

			_log.Error($"Failed to get user {id} from AccSaber Reloaded API");
			return new AccSaberUser();
		}

		public async Task<UserInfo?> GetPlatformUserInfo()
		{
			return await _platformUserModel.GetUserInfo(CancellationToken.None);
		}

		public async Task<AccSaberUser> GetCurrentCategoryUserAsync()
		{
			return _currentRankedMap?.Category switch
			{
				AccSaberMapCategories.True => await GetCurrentUser(AccSaberMapCategories.True),
				AccSaberMapCategories.Standard => await GetCurrentUser(AccSaberMapCategories.Standard),
				AccSaberMapCategories.Tech => await GetCurrentUser(AccSaberMapCategories.Tech),
				_ => await GetCurrentUser()
			};
		}

		public AccSaberUser GetCurrentCategoryUser()
		{
			return _currentRankedMap?.Category switch
			{
				AccSaberMapCategories.True => _currentUserTrue,
				AccSaberMapCategories.Standard => _currentUserStandard,
				AccSaberMapCategories.Tech => _currentUserTech,
				_ => _currentUserOverall
			};
		}

		public AccSaberUser GetCurrentOverallUser()
		{
			return _currentUserOverall;
		}

		public async Task<bool> HasAccSaberUpdated()
		{
			if (LastLocalUpdateTime != DateTime.MinValue && DateTime.UtcNow < LastLocalUpdateTime.AddMinutes(15))
			{
				return false;
			}

			return await UpdateAccSaberInfo();
		}

		public void Initialize()
		{
		}

		private static bool HasUserChanged(AccSaberUser current, AccSaberUser next)
		{
			return !string.Equals(current.PlayerId, next.PlayerId, StringComparison.Ordinal) ||
			       !string.Equals(current.PlayerName, next.PlayerName, StringComparison.Ordinal) ||
			       current.Rank != next.Rank ||
			       Math.Abs(current.AP - next.AP) > 0.01f ||
			       current.RankedPlays != next.RankedPlays;
		}
	}
}
