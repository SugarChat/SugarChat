using Google.Cloud.Translation.V2;
using SugarChat.Core.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.ThirdParty
{
    public interface IGoogleService : IService
    {
        Task<string> TranslateText(string targetLanguage, string text);

        string MapLanguageCode(string language);
    }

    public class GoogleService : IGoogleService
    {
        private readonly GoogleTranslateApiKeySetting _googleTranslateApiKey;

        public GoogleService(GoogleTranslateApiKeySetting googleTranslateApiKey)
        {
            _googleTranslateApiKey = googleTranslateApiKey;
        }

        public Dictionary<string, string> GoogleLanguageCodeMapping = new Dictionary<string, string>
        {
            {"zh-TW", "zh-TW"},
            {"en-US", "en"},
            {"es-ES", "es"}
        };

        public async Task<string> TranslateText(string targetLanguage, string text)
        {
            var client = TranslationClient.CreateFromApiKey(_googleTranslateApiKey.Value);
            var response = await client.TranslateTextAsync(text, targetLanguage);
            return response.TranslatedText;
        }

        public string MapLanguageCode(string language)
        {
            GoogleLanguageCodeMapping.TryGetValue(language, out var googleLanguageCode);

            return googleLanguageCode;
        }
    }
}
