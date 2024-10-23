using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using v2api_client_csharp;


namespace RPGGoApiExample
{
    class Program
    {
        static async Task TestGetMeta(RPGGOClient client, string gameId)
        {
            var gameMetadata = await client.GetGameMetadataAsync(gameId);
            Console.WriteLine($"Game Name: {gameMetadata.Data.Name}");
            Console.WriteLine($"Intro: {gameMetadata.Data.Intro}");
            Console.WriteLine($"Chapters: {gameMetadata.Data.Chapters.Count}");
            Console.WriteLine($"Character id: {gameMetadata.Data.Chapters[0].Characters[0].Id}");
            Console.WriteLine();
        }

        static async Task TestStartSession(RPGGOClient client, string gameId, string sessionId)
        {
            // Start a new game session
            var startGameResponse = await client.StartGameAsync(gameId, sessionId);
            Console.WriteLine($"Started Game: {startGameResponse.Data.Name}");
            Console.WriteLine();
        }

        static async Task TestResumeSession(RPGGOClient client, string gameId, string sessionId)
        {
            // Resume an existing game session
            var resumeSessionResponse = await client.ResumeSessionAsync(gameId, sessionId);
            Console.WriteLine($"Resumed Game: {resumeSessionResponse.Data.Name}");
            Console.WriteLine();
        }

        static async Task TestChatSSE(RPGGOClient client, string gameId, string sessionId, string msg)
        {
            var gameMetadata = await client.GetGameMetadataAsync(gameId);

            string msgId = Guid.NewGuid().ToString().Substring(0, 5);
            string characterId = gameMetadata.Data.Chapters[0].Characters[0].Id;

            // Callback function to handle new SSE messages
            void OnChatMessageReceived(string chatMessage)
            {
                Console.WriteLine($"New chat message received: {chatMessage}");
                Console.WriteLine();
                // You can further parse and process the received message here
            }

            // 
            void OnImageMessageReceived(string imageUrl)
            {
                Console.WriteLine($"New image message received: {imageUrl}");
                Console.WriteLine();
                // You can further parse and process the received message here
            }

            // Monitor SSE stream
            await client.ChatSseAsync(characterId, gameId, "hello", msgId, sessionId, OnChatMessageReceived, OnImageMessageReceived);

        }


        static async Task Main(string[] args)
        {
            var apiKey = ""; // Replace with your actual API key
            var gameId = "7411057c-43a0-4fbb-b4b8-f0b02ba3cb02";
            string sessionId = Guid.NewGuid().ToString();
            

            var rpgGoClient = new v2api_client_csharp.RPGGOClient(apiKey);

            Console.WriteLine("session_id is " + sessionId);

            try
            {
                Console.WriteLine("Testing GetGameMetadataAsync ... ");
                await TestGetMeta(rpgGoClient, gameId);

                Console.WriteLine("Testing StartGameAsync ... ");
                await TestStartSession(rpgGoClient, gameId, sessionId);

                Console.WriteLine("Testing ResumeSessionAsync ... ");
                await TestResumeSession(rpgGoClient, gameId, sessionId);

                Console.WriteLine("Testing ChatSseAsync ... ");
                await TestChatSSE(rpgGoClient, gameId, sessionId, "hello world!");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching game metadata: {ex.Message}");
            }
        }
    }
}

