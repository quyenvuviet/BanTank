using UnityEngine;

public static class GamePrefData
{
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
    public static float GetFloat(string key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
    public static bool GetBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, null);
    }

    public static void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
