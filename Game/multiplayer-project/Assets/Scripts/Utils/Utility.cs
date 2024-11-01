using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Utility
{
    private static readonly System.Random random = new System.Random();
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    const int length = 8;
    
    /// <summary>
    /// Utility method to shuffle an array using Fisher-Yates algorithm
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    public static void Shuffle<T>(ref List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    /// <summary>
    /// Random username generator
    /// </summary>
    /// <returns></returns>
    public static string GetRandomUsername()
    {
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }
    
    public static void Toggle(this CanvasGroup group) {
        if(group.alpha == 0f)   group.Show();
        else                    group.Hide();
    }

    public static void Show(this CanvasGroup group) {
        group.alpha = 1f;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public static void Hide(this CanvasGroup group) {
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }
}
