using System;
using System.Threading.Tasks;
using AccSaber.UI.ViewControllers;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using Zenject;

namespace AccSaber.UI
{
	internal sealed class AccSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable
	{
		private readonly CustomLeaderboardManager _customLeaderboardManager;
		private readonly AccSaberPanelViewController _accSaberPanelViewController;
		private readonly AccSaberLeaderboardViewController _accSaberLeaderboardViewController;
		private bool _disposed;

		public AccSaberCustomLeaderboard(CustomLeaderboardManager customLeaderboardManager, AccSaberPanelViewController accSaberPanelViewController, AccSaberLeaderboardViewController accSaberLeaderboardViewController)
		{
			_customLeaderboardManager = customLeaderboardManager;
			_accSaberPanelViewController = accSaberPanelViewController;
			_accSaberLeaderboardViewController = accSaberLeaderboardViewController;
		}

		protected override ViewController panelViewController => _accSaberPanelViewController;
		protected override ViewController leaderboardViewController => _accSaberLeaderboardViewController;

		public async void Initialize()
		{
			// LeaderboardCore tab order follows registration timing, so register slightly
			// after the other built-in leaderboard plugins to keep AccSaber later in the row.
			await Task.Delay(1500);
			if (_disposed)
			{
				return;
			}

			_customLeaderboardManager.Register(this);
		}

		public void Dispose()
		{
			_disposed = true;
			_customLeaderboardManager.Unregister(this);
		}
	}
}
