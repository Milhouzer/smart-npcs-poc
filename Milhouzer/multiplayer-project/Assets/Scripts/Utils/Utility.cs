using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
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
