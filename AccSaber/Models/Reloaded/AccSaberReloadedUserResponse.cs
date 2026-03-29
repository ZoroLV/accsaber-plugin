using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models.Reloaded
{
	[UsedImplicitly]
	internal sealed class AccSaberReloadedUserResponse
	{
		[JsonProperty("avatarUrl")]
		public string AvatarUrl { get; set; } = string.Empty;

		[JsonProperty("country")]
		public string Country { get; set; } = string.Empty;

		[JsonProperty("id")]
		public string Id { get; set; } = string.Empty;

		[JsonProperty("hmd")]
		public string Hmd { get; set; } = string.Empty;

		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("statistics")]
		public List<AccSaberReloadedUserCategoryStatisticsResponse> Statistics { get; set; } = new();
	}
}
