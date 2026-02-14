using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadePanel;

    private Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPanel(string panelName, GameObject panel)
    {
        panels[panelName] = panel;
    }

    public void ShowPanel(string panelName)
    {
        if (panels.TryGetValue(panelName, out var panel))
        {
            panel.SetActive(true);
        }
    }

    public void HidePanel(string panelName)
    {
        if (panels.TryGetValue(panelName, out var panel))
        {
            panel.SetActive(false);
        }
    }

    public void HideAllPanels()
    {
        foreach (var panel in panels.Values)
        {
            panel.SetActive(false);
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(0f, 1f, 0.3f));
        }

        SceneManager.LoadScene(sceneName);

        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(1f, 0f, 0.3f));
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        fadePanel.alpha = to;
        if (to == 0f) fadePanel.gameObject.SetActive(false);
    }
}
