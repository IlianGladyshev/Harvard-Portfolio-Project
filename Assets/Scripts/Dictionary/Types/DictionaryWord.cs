public class DictionaryWord
{
    public string Name { get; set; }
    public string Meaning { get; set; }
    public string Example { get; set; }

    public DictionaryWord(string name, string meaning, string example)
    {
        Name = name;
        Meaning = meaning;
        Example = example;
    }
}
