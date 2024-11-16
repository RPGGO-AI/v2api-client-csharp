﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


using Microsoft.Extensions.Logging;

namespace v2api_client_csharp
{
    public class RPGGOClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _apiEndpoint = "https://api.rpggo.ai";

        public RPGGOClient(string apiKey)
        {
            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
            //_httpClient.DefaultRequestHeaders..Add("Content-Type", "application/json");
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = factory.CreateLogger("RPGGOClient");
        }

        public async Task<GameMetadataResponse> GetGameMetadataAsync(string gameId)
        {
            var requestBody = new
            {
                game_id = gameId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync( _apiEndpoint + "/v2/open/game/gamemetadata", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var gameMetadataResponse = JsonConvert.DeserializeObject<GameMetadataResponse>(responseContent)!;

            return gameMetadataResponse;
        }

        // Start a game
        public async Task<GameOngoingResponse> StartGameAsync(string gameId, string sessionId)
        {
            var requestBody = new
            {
                game_id = gameId,
                session_id = sessionId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiEndpoint + "/v2/open/game/startgame", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var startGameResponse = JsonConvert.DeserializeObject<GameOngoingResponse>(responseContent)!;

            return startGameResponse;
        }

        // Resume a game session
        public async Task<GameOngoingResponse> ResumeSessionAsync(string gameId, string sessionId)
        {
            var requestBody = new
            {
                game_id = gameId,
                session_id = sessionId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiEndpoint + "/v2/open/game/resumesession", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var resumeSessionResponse = JsonConvert.DeserializeObject<GameOngoingResponse>(responseContent)!;

            return resumeSessionResponse;
        }

        public async Task<GameOngoingResponse> SwitchChapterAsync(string gameId, string chapterId, string sessionId)
        {
            var requestBody = new
            {
                game_id = gameId,
                chapter_id = chapterId,
                session_Id = sessionId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiEndpoint + "/v2/open/game/changechapter", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var changeChapterResponse = JsonConvert.DeserializeObject<GameOngoingResponse>(responseContent)!;

            return changeChapterResponse;
        }


        // Method to monitor SSE stream
        public async Task ChatSseAsync(
            string characterId,
            string gameId,
            string message,
            string messageId,
            string sessionId,
            Action<string, string> onChatMessageReceived,
            Action<string> onImageMessageReceived = null,
            Action<string, GameOngoingResponse> onChapterSwitchMessageReceived = null,
            Action<string> onGameEndingMessageReceived = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestBody = new
            {
                character_id = characterId,
                game_id = gameId,
                message = message,
                message_id = messageId,
                session_id = sessionId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiEndpoint + "/v2/open/game/chatsse", requestContent);
            response.EnsureSuccessStatusCode();

            // Get the stream to read SSE messages
            var stream = await response.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                string line;
                string completeMessage = string.Empty;

                // Continuously read the stream
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    //Console.WriteLine("message is " + line);
                    // SSE events are separated by empty lines
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (!string.IsNullOrWhiteSpace(completeMessage))
                        {
                            // Trigger callback when the message is complete
                            var sseMsg = JsonConvert.DeserializeObject<SSEResponse>(completeMessage);
                            if (sseMsg == null)
                            {
                                throw new Exception("json deserialize issue:" + completeMessage);
                            }

                            if (sseMsg.Data.Result != null)
                            {

                                if (sseMsg.Data.Result.CharacterType == "common_npc")
                                {
                                    onChatMessageReceived(sseMsg.Data.Result.CharacterId, sseMsg.Data.Result.Text);
                                }
                                else if (sseMsg.Data.Result.CharacterType == "async_npc")
                                {
                                    if (sseMsg.Data.Result.Text != "")
                                    {
                                        onChatMessageReceived(sseMsg.Data.Result.CharacterId, sseMsg.Data.Result.Text);
                                    }
                                }
                                else if (sseMsg.Data.Result.CharacterType == "picture_produce_dm")
                                {
                                    onImageMessageReceived?.Invoke(sseMsg.Data.Result.Image);
                                }

                                if (sseMsg.Data.GameStatus != null)
                                {
                                    if (sseMsg.Data.Result.CharacterType == "goal_check_dm" && sseMsg.Data.GameStatus.Action == 2)
                                    {
                                        var next_chapter_id = sseMsg.Data.GameStatus.ChapterId;
                                        var new_meta_response = await SwitchChapterAsync(gameId, next_chapter_id, sessionId);
                                        _logger.LogInformation($"switch to new chapter {next_chapter_id}");
                                        // switch to new chapter and pass new metadata for application processing
                                        onChapterSwitchMessageReceived?.Invoke(sseMsg.Data.GameStatus.ActionMessage, new_meta_response);
                                        
                                    }
                                    else if (sseMsg.Data.Result.CharacterType == "goal_check_dm" && sseMsg.Data.GameStatus.Action == 3)
                                    {
                                        onGameEndingMessageReceived?.Invoke(sseMsg.Data.GameStatus.ActionMessage);
                                    }
                                }
                                  
 
                            }

                            completeMessage = string.Empty;
                        }
                    }
                    else if (line.StartsWith("data:"))
                    {
                        // Extract data after the 'data:' field
                        completeMessage += line.Substring(5).Trim();
                    }
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation("ChatSseAsync executed in {elapsedMs} ms.", elapsedMs);
        }

    }

}
