# v2api-client-csharp

This is an demo c# library for game developer to integrate with RPGGO v2 api.

## 📖 Brief
If you're dreaming of making your own AI-powered game, well, you'll need more than just game dev skills. You'll also have to tackle some seriously brainy AI challenges, like auto-generating storylines, managing memory stacks, handling NPC interactions, and dealing with memory systems—basically, all the techy stuff that turns solo game development into a next-level boss fight!

But now, there's a smarter solution! With the RPGGO API, you can free yourself from the tangled web of AI logic and focus on crafting the game itself. RPGGO will infuse your game with an AI-powered soul, taking care of all the complex AI magic for you!

This demo project tries to show you how the v2 api will work.

if you have any thoughst, feel free to tell us here: [Discuss, feedback and bug report](https://github.com/RPGGO-AI/v2api-client-csharp/issues/1)

<br>

## 📂 File structure

v2api-client-csharp/ <br>
├── public/                # Static files of game assets, like map, npc sprites <br>
├── v2api-client-csharp/   # The real library of connecting with v2 api                
├── TestCode/              # The test code of library
└── README.md              # readme file <br>


## ❓ How it works

![whiteboard_exported_image (3)](https://github.com/user-attachments/assets/24b8aba6-db17-4dba-a5db-53d42a2c09ce)
This graph tells the exact magic about how the system works.

Basically, RPGGO covers the end2end pipeline from building a game to rendering a game in real time. As a game developer, all you need to do is very simple:
1. find a game you want to make it live in 2D graphic. Either, go to https://creator.rpggo.ai to build your own game if you are a good game designer, or go to https://rpggo.ai game lobby to find a game you like. Remember the game id.
2. fill an [API Key Application form](https://developer.rpggo.ai/dev-docs/support/apply-your-test-key) with the Game ID you chose, or contact them via email at [dev@rpggo.ai](mailto:dev@rpggo.ai)
3. After you get your key, you can use it in RPGGOClient.cs to access the game data and make this lib work. This project is also a good example to tell how the integration code will be.

<br>


## ⚡️ Quick Start in local
The best way is to use visual studio 2022, where you can easily manage coding, debugging in one station.

But you can also use visual studio code with the extension "C# Dev Kit"



## 📚 Publish to Nuget

dotnet nuget push $path\v2api-client-csharp\bin\Release\v2api-client-csharp.1.0.8.nupkg --api-key $api-key --source https://api.nuget.org/v3/index.json


## ⚖️ License

This project is under MIT license, which means you can do anything you want.

<br>


## 📬 Contact

If you want to make this repo even better, feel free to submit your pr.

If you have any questions, feedback, or would like to get in touch, send me email or message. 
