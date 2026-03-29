using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models.Reloaded
{
	[UsedImplicitly]
	internal sealed class PageResponse<T>
	{
		[JsonProperty("content")]
		public List<T> Content { get; set; } = new();
	}
}
