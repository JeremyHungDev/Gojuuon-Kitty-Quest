using System;
using System.Collections.Generic;

[Serializable]
public class KanaData
{
    public string id;
    public string hiragana;
    public string katakana;
    public string romaji;
    public string origin;
    public string originMeaning;
    public string audioFile;
    public List<int> strokeOrder;
    public string group;
    public int level;
}

[Serializable]
public class KanaDatabaseWrapper
{
    public List<KanaData> kanaList;
}
