namespace AudioTexts
{
    public class Story
    {
        public string Name { get; set; }
        public string[] Content { get; set; }
        public string Author { get; set; }

        public Story(string name, string[] content, string author)
        {
            Name = name;
            Content = content;
            Author = author;
        }
    }
}