using Teleport.Services;

namespace Teleport.Entities
{
    public class PttArticle
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }

        public string ToPttLink()
        {
            return $"{PttService.PttUrl}{Link}";
        }
    }
}