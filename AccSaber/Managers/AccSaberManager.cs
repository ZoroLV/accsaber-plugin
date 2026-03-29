using System;
using System.Threading;
using System.Threading.Tasks;
using LeaderboardCore.Interfaces;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Managers
{
	internal class AccSaberManager : INotifyLeaderboardSet
	{
		private readonly SiraLog _log;
		private readonly AccSaberStore _accSaberStore;
		private readonly BeatmapLevelsModel _beatmapLevelsModel;
		private int _lookupVersion;

		public AccSaberManager(SiraLog log, AccSaberStore accSaberStore, BeatmapLevelsModel beatmapLevelsModel)
		{
			_log = log;
			_accSaberStore = accSaberStore;
			_beatmapLevelsModel = beatmapLevelsModel;
		}

		public void OnLeaderboardSet(BeatmapKey beatmapKey)
		{
			_ = HandleLeaderboardSetAsync(beatmapKey);
		}

		private async Task HandleLeaderboardSetAsync(BeatmapKey beatmapKey)
		{
			try
			{
				var lookupVersion = Interlocked.Increment(ref _lookupVersion);
				BeatmapLevel? level = _beatmapLevelsModel.GetBeatmapLevel(beatmapKey.levelId);
				if (level is null)
				{
					if (lookupVersion == _lookupVersion)
					{
						_accSaberStore.CurrentRankedMap = null;
					}

					return;
				}

				var hash = SongCore.Utilities.Hashing.ComputeCustomLevelHash(level);
				var mapInfo = await _accSaberStore.GetRankedMapAsync(hash, beatmapKey.difficulty.ToString());
				if (lookupVersion != _lookupVersion)
				{
					return;
				}

				_accSaberStore.CurrentRankedMap = mapInfo;
			}
			catch (Exception ex)
			{
				_log.Critical(ex);
			}
		}
	}
}
