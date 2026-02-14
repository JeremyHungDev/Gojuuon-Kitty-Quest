using System;
using System.Collections.Generic;

[Serializable]
public class WordData
{
    public string id;
    public string kana;
    public string katakana;
    public string chinese;
    public string category;
    public int difficulty;
    public string sprite;
}

[Serializable]
public class WordDatabaseWrapper
{
    public List<WordData> words;
}
