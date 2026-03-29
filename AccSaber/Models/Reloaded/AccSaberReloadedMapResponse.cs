using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models.Reloaded
{
	[UsedImplicitly]
	internal sealed class AccSaberReloadedMapResponse
	{
		[JsonProperty("beatsaverCode")]
		public string BeatSaverCode { get; set; } = string.Empty;

		[JsonProperty("difficulties")]
		public List<AccSaberReloadedMapDifficultyResponse> Difficulties { get; set; } = new();

		[JsonProperty("mapAuthor")]
		public string MapAuthor { get; set; } = string.Empty;

		[JsonProperty("songAuthor")]
		public string SongAuthor { get; set; } = string.Empty;

		[JsonProperty("songHash")]
		public string SongHash { get; set; } = string.Empty;

		[JsonProperty("songName")]
		public string SongName { get; set; } = string.Empty;

		[JsonProperty("songSubName")]
		public string SongSubName { get; set; } = string.Empty;
	}
}
