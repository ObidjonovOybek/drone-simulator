using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PartInfoUI : MonoBehaviour
{
    public static PartInfoUI Instance;

    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image partImage;

    public float animationDuration = 0.25f;

    private ClickToAssemble currentPart;
    private CanvasGroup canvasGroup;
    private RectTransform panelRect;
    private Coroutine currentAnimation;

    private void Awake()
    {
        Instance = this;

        canvasGroup = panel.GetComponent<CanvasGroup>();
        panelRect = panel.GetComponent<RectTransform>();

        panel.SetActive(false);
    }

    public void ShowPartInfo(DronePartInfo info, ClickToAssemble clickedPart)
    {
        currentPart = clickedPart;

        titleText.text = info.partName;
        descriptionText.text = info.description;
        partImage.sprite = info.partImage;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(ShowAnimation());
    }

    public void ConfirmAssemble()
    {
        if (currentPart != null)
        {
            currentPart.AssemblePart();
        }

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(HideAnimation());
    }

    public void ClosePanel()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(HideAnimation());
    }

    IEnumerator ShowAnimation()
    {
        panel.SetActive(true);

        float time = 0f;
        canvasGroup.alpha = 0f;
        panelRect.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            panelRect.localScale = Vector3.Lerp(
                new Vector3(0.8f, 0.8f, 0.8f),
                Vector3.one,
                t
            );

            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.localScale = Vector3.one;
    }

    IEnumerator HideAnimation()
    {
        float time = 0f;
        canvasGroup.alpha = 1f;
        panelRect.localScale = Vector3.one;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            panelRect.localScale = Vector3.Lerp(
                Vector3.one,
                new Vector3(0.8f, 0.8f, 0.8f),
                t
            );

            yield return null;
        }

        canvasGroup.alpha = 0f;
        panel.SetActive(false);
        currentPart = null;
    }
}