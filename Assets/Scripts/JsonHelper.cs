
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonHelper
{

    //! this is probably terrible. but hey, it works!
    public static string SerializeArray<T>(T[] array)
    {
        string json = "[";
        foreach (var item in array)
        {
            json += JsonUtility.ToJson(item) + ",";
        }
        json = json.Substring(0, json.Length - 1);
        json += "]";
        return json;
    }

    public static string SerializeDictionary<A, B>(Dictionary<A, B> dictionary)
    {
        B[] array = new B[dictionary.Count];
        dictionary.Values.CopyTo(array, 0);

        string json = JsonHelper.SerializeArray<B>(array);
        return json;
    }

    public static T[] DeserializeArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}