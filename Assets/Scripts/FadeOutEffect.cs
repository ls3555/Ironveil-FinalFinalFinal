using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class FadeOutEffect : MonoBehaviour
{
    public void FadeOutBackground(List<CanvasGroup> ListCanvasList)
    {
        StartCoroutine(FadeBackgroundRoutine(ListCanvasList));
    }

    private IEnumerator FadeBackgroundRoutine(List<CanvasGroup> canvasGroups)
    {        
        
        float time = 0f;
        float duration = 2f;

        while (time < duration)
        {
            time += Time.deltaTime;
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / duration);
            }
            yield return null; // Wait until the next frame
        }
        foreach (CanvasGroup canvasGroup in canvasGroups)
            {
            canvasGroup.alpha = 0; 
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false; // Disable interaction with the faded-out panel
    
            }
        
        
    }
}