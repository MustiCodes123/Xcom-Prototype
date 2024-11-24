using UnityEngine;

public static class Extension
{
    public static void DestroyChilds(Transform transform)
    {
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static string FormatNumber(long number)
    {
        if (number >= 1000000000)
            return (number / 1000000000D).ToString("0.#") + "b";
        if (number >= 1000000)
            return (number / 1000000D).ToString("0.#") + "m";
        if (number >= 10000)
            return (number / 1000D).ToString("0.#") + "k";
        return number.ToString();
    }
}
