using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using v2api_client_csharp;


namespace RPGGoApiExample
{

    class FunctionProcessor
    {
        // Callback function to handle new SSE messages
        public static void OnChatMessageReceived(string characterId, string chatMessage)
        {
            Console.WriteLine($"{characterId} has new chat message received: {chatMessage}");
            Console.WriteLine();
            // You can further parse and process the received message here
        }

        // 
        public static void OnImageMessageReceived(string imageUrl)
        {
            Console.WriteLine($"New image message received: {imageUrl}");
            Console.WriteLine();
            // You can further parse and process the received message here
        }

        public static void onChapterSwitchMessageReceived(string chapterEndingMsg, GameOngoingResponse metaDataResponse)
        {
            Console.WriteLine($"Current chapter ends.");
            Console.WriteLine($"{chapterEndingMsg}");
            Console.WriteLine($"New chapter starts.");
            Console.WriteLine();
            // You can fetch the character from next chapter in the GameMetaData.
            // for example, print the new chapter info
            Console.WriteLine($"New chapter Name:{metaDataResponse.Data.Chapter.Name}");
            Console.WriteLine(metaDataResponse.Data.Chapter.Characters);
        }

        public static void onGameEndingMessageReceived(string gameEndingMsg)
        {
            Console.WriteLine($"Game is Over.");
            Console.WriteLine($"{gameEndingMsg}");
            Console.WriteLine();
            // You can do some post-game logic below
        }
    }

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
            var gameMeta = await client.StartGameAsync(gameId, sessionId);
            Console.WriteLine($"Started Game: {gameMeta.Data.Name}");
            Console.WriteLine();
        }

        static async Task TestResumeSession(RPGGOClient client, string gameId, string sessionId)
        {
            // Resume an existing game session
            var gameMeta = await client.ResumeSessionAsync(gameId, sessionId);
            Console.WriteLine($"Resumed Game: {gameMeta.Data.Name}");
            Console.WriteLine();
        }

        static async Task TestChatSSE(RPGGOClient client, string gameId, string sessionId, string msg)
        {
            var gameMetadata = await client.GetGameMetadataAsync(gameId);

            string msgId = Guid.NewGuid().ToString().Substring(0, 5);
            string characterId = gameMetadata.Data.Chapters[0].Characters[0].Id;



            // Monitor SSE stream
            await client.ChatSseAsync(
                characterId, 
                gameId, 
                "hello", 
                msgId, 
                sessionId, 
                FunctionProcessor.OnChatMessageReceived
             );
        }

        static async Task TestSyncedGame(RPGGOClient client, string gameId, string sessionId)
        {
            var gameMetadata = await client.GetGameMetadataAsync(gameId);
            string choosedCharacterId = gameMetadata.Data.Chapters[0].Characters[0].Id;

            Console.Write("Start to play with agent and input exit to quite the game.");
            Console.WriteLine();
            while (true)
            {
                string msgId = Guid.NewGuid().ToString().Substring(0, 5);
                Console.Write("Your input:");
                
               var input = Console.ReadLine();
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // Monitor SSE stream
                await client.ChatSseAsync(
                    choosedCharacterId, 
                    gameId,
                    input,
                    msgId,
                    sessionId,
                    FunctionProcessor.OnChatMessageReceived,
                    FunctionProcessor.OnImageMessageReceived,
                    FunctionProcessor.onChapterSwitchMessageReceived,
                    FunctionProcessor.onGameEndingMessageReceived
                 );
                Console.WriteLine();
            }
        }


        static async Task TestAsyncChatGame(RPGGOClient client, string gameId, string sessionId)
        {
            var gameMetadata = await client.GetGameMetadataAsync(gameId);

            string msgId = Guid.NewGuid().ToString().Substring(0, 5);
            string choosedCharacterId = gameMetadata.Data.Chapters[0].Characters[0].Id;
            string choosedCharacterName = gameMetadata.Data.Chapters[0].Characters[0].Name;

            // for check Affection
            string dm_id = "CCT67P3C0";

            await client.ChatSseAsync(
                choosedCharacterId,
                gameId,
                "hello",
                msgId,
                sessionId,
                FunctionProcessor.OnChatMessageReceived,
                FunctionProcessor.OnImageMessageReceived,
                FunctionProcessor.onChapterSwitchMessageReceived,
                FunctionProcessor.onGameEndingMessageReceived
            );

            await client.ChatSseAsync(
                choosedCharacterId,
                gameId,
                $"Give me {choosedCharacterName}'s Affection",
                msgId,
                sessionId,
                FunctionProcessor.OnChatMessageReceived,
                FunctionProcessor.OnImageMessageReceived,
                FunctionProcessor.onChapterSwitchMessageReceived,
                FunctionProcessor.onGameEndingMessageReceived
            );
        }


        static async Task Main(string[] args)
        {
            var apiKey = "Bearer "; // Replace with your actual API key
            var gameId = "gG2xmvEDS";
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

                //Console.WriteLine("Testing ChatSseAsync ... ");
                //await TestChatSSE(rpgGoClient, gameId, sessionId, "hello world!");

                
                Console.WriteLine("Testing TestSyncedGame ... ");
                await TestSyncedGame(rpgGoClient, gameId, sessionId);


                //Console.WriteLine("Testing TestAsyncChatGame ... ");
                //await TestAsyncChatGame(rpgGoClient, gameId, sessionId);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during running game: {ex.Message}");
            }
        }
    }
}

