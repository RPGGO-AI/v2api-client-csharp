using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace v2api_client_csharp
{

    public class GameMetadataResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public GameData Data { get; set; }
    }

    public class GameData
    {
        public string Name { get; set; }
        public string GameId { get; set; }
        public string Background { get; set; }
        public string Intro { get; set; }
        public string Image { get; set; }
        public string Genre { get; set; }
        public List<Chapter> Chapters { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Chapter
    {
        public string Name { get; set; }
        public string ChapterId { get; set; }
        public string Background { get; set; }
        public string Intro { get; set; }
        public string Image { get; set; }
        public string BackgroundAudio { get; set; }
        public string EndingAudio { get; set; }
    }


}
