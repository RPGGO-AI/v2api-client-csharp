﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace v2api_client_csharp
{
    public class RPGGOClient
    {
        private readonly HttpClient _httpClient;

        public RPGGOClient(string apiKey)
        {
            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
            //_httpClient.DefaultRequestHeaders..Add("Content-Type", "application/json");
        }

        public async Task<GameMetadataResponse> GetGameMetadataAsync(string gameId)
        {
            var requestBody = new
            {
                game_id = gameId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.rpggo.ai/v2/open/game/gamemetadata", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var gameMetadataResponse = JsonConvert.DeserializeObject<GameMetadataResponse>(responseContent);

            return gameMetadataResponse;
        }

        // Start a game
        public async Task<StartGameResponse> StartGameAsync(string gameId, string sessionId)
        {
            var requestBody = new
            {
                game_id = gameId,
                session_id = sessionId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.rpggo.ai/v2/open/game/startgame", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var startGameResponse = JsonConvert.DeserializeObject<StartGameResponse>(responseContent);

            return startGameResponse;
        }

        // Resume a game session
        public async Task<StartGameResponse> ResumeSessionAsync(string gameId, string sessionId)
        {
            var requestBody = new
            {
                game_id = gameId,
                session_id = sessionId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.rpggo.ai/v2/open/game/resumesession", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var resumeSessionResponse = JsonConvert.DeserializeObject<StartGameResponse>(responseContent);

            return resumeSessionResponse;
        }
    }

}
