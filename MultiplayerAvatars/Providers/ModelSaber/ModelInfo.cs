using Newtonsoft.Json;
using System;

namespace MultiplayerAvatars.Providers.ModelSaber
{
    internal class ModelInfo
    {
        [JsonProperty("tags")]
        public string[] Tags { get; set; } = Array.Empty<string>();

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("author")]
        public string Author { get; set; } = string.Empty;

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; } = string.Empty;

        [JsonProperty("bsaber")]
        public string BSaber { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("discordid")]
        public string DiscordID { get; set; } = string.Empty;

        [JsonProperty("discord")]
        public string Discord { get; set; } = string.Empty;

        [JsonProperty("variationid")]
        public int? VariationID { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; } = string.Empty;

        [JsonProperty("download")]
        public string DownloadUrl { get; set; } = string.Empty;

        [JsonProperty("install_link")]
        public string InstallUrl { get; set; } = string.Empty;

        [JsonProperty("date")]
        public string Date { get; set; } = string.Empty;
    }
}
