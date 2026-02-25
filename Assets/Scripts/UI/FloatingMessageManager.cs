using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.WSA;

public class FloatingMessageManager : MonoBehaviour
{
    public static FloatingMessageManager Instance;

    [Header("References")]
    [SerializeField]
    private GameObject floatingMessagePrefab;
    [SerializeField]
    private Transform floatingMessageParent;

    [Header("Settings")]
    [SerializeField]
    private float floatingDuration = 2f;
    [SerializeField]
    private float fadeDuration = 0.5f;
    [SerializeField]
    private float moveUpDistance = 40f;
    [SerializeField]
    private float coolDown = 1.5f;
    private float lastShowTime = -1f;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowMessage(string message, FloatingMessageType type)
    {
        if (Time.time - lastShowTime < coolDown)
        {
            return;
        }
        lastShowTime = Time.time;
        FloatingMessageUI ui = floatingMessageParent.GetComponent<FloatingMessageUI>();
        ui.SetupUI(message, type);
        GameObject floatingMessage = Instantiate(floatingMessagePrefab, floatingMessageParent);
        StartCoroutine(AnimateFloatingMessage(floatingMessage));
    }

    private IEnumerator AnimateFloatingMessage(GameObject floatingMessage)
    {
        RectTransform rect = floatingMessage.GetComponent<RectTransform>();

        Vector2 startPos = rect.anchoredPosition + new Vector2(0, 300f);
        Vector2 endPos = startPos + Vector2.up * moveUpDistance;

        CanvasGroup cg = floatingMessage.GetComponent<CanvasGroup>();
        if (cg == null) cg = floatingMessage.AddComponent<CanvasGroup>();

        float startAlpha = 1f;

        float timer = 0f;

        while (timer < floatingDuration)
        {
            timer += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, timer / floatingDuration);
            yield return null;
        }
       

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
            yield return null;
        }
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(floatingMessage);
    }
}

public enum FloatingMessageType { 
    Warning,
    Error,
    Info
}
