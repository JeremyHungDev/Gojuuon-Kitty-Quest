using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KanaDatabaseService
{
    private List<KanaData> kanaList = new List<KanaData>();
    private Dictionary<string, KanaData> kanaById = new Dictionary<string, KanaData>();

    public void LoadFromJson(string json)
    {
        var wrapper = JsonUtility.FromJson<KanaDatabaseWrapper>(json);
        kanaList = wrapper.kanaList;
        kanaById.Clear();
        foreach (var kana in kanaList)
        {
            kanaById[kana.id] = kana;
        }
    }

    public KanaData GetKanaById(string id)
    {
        kanaById.TryGetValue(id, out var kana);
        return kana;
    }

    public List<KanaData> GetKanaByGroup(string group)
    {
        return kanaList.Where(k => k.group == group).ToList();
    }

    public List<KanaData> GetKanaByLevel(int level)
    {
        return kanaList.Where(k => k.level == level).ToList();
    }

    public KanaData GetRandomKana(int level)
    {
        var candidates = GetKanaByLevel(level);
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    public List<KanaData> GetAllKana()
    {
        return new List<KanaData>(kanaList);
    }
}
