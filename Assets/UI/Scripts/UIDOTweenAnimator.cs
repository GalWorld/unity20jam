using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class UIDOTweenAnimator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform targetRect;
    private CanvasGroup canvasGroup;

    [Header("Configuración General")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease easeEnter = Ease.OutBack;
    [SerializeField] private Ease easeExit = Ease.InBack;

    [Header("Tipo de Animación")]
    [SerializeField] private AnimationType animationType = AnimationType.Fade;

    [Header("Opciones de Animación")]
    [SerializeField] private float fadeFrom = 0f;
    [SerializeField] private float fadeTo = 1f;
    [SerializeField] private float scaleFrom = 0.8f;
    [SerializeField] private float scaleTo = 1f;
    [SerializeField] private float slideDistance = 100f;
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Bottom;

    private Vector2 originalPosition;
    private Sequence currentSequence;

    public enum AnimationType { Fade, Scale, Slide, FadeAndScale, FadeAndSlide }
    public enum SlideDirection { Top, Right, Bottom, Left }

    public System.Action OnEnterComplete;
    public System.Action OnExitComplete;

    private void Awake()
    {
        if (targetRect == null)
            targetRect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalPosition = targetRect.anchoredPosition;
    }

    private void OnDisable()
    {
        if (currentSequence != null && currentSequence.IsActive())
            currentSequence.Kill();
    }

    // ✦ Animación de entrada
    public void PlayEnter(System.Action callback = null)
    {
        PrepareForEntry();
        PlayEntryInternal(() =>
        {
            OnEnterComplete?.Invoke();
            callback?.Invoke();
        });
    }

    // ✦ Animación de salida
    public void PlayExit(System.Action callback = null)
    {
        PlayExitInternal(() =>
        {
            OnExitComplete?.Invoke();
            callback?.Invoke();
        });
    }

    private void PrepareForEntry()
    {
        switch (animationType)
        {
            case AnimationType.Fade:
                canvasGroup.alpha = fadeFrom;
                break;

            case AnimationType.Scale:
                targetRect.localScale = Vector3.one * scaleFrom;
                break;

            case AnimationType.Slide:
                targetRect.anchoredPosition = GetSlideStart();
                break;

            case AnimationType.FadeAndScale:
                canvasGroup.alpha = fadeFrom;
                targetRect.localScale = Vector3.one * scaleFrom;
                break;

            case AnimationType.FadeAndSlide:
                canvasGroup.alpha = fadeFrom;
                targetRect.anchoredPosition = GetSlideStart();
                break;
        }
    }

    private Vector2 GetSlideStart()
    {
        Vector2 offset = Vector2.zero;
        switch (slideDirection)
        {
            case SlideDirection.Top: offset = new Vector2(0, slideDistance); break;
            case SlideDirection.Right: offset = new Vector2(slideDistance, 0); break;
            case SlideDirection.Bottom: offset = new Vector2(0, -slideDistance); break;
            case SlideDirection.Left: offset = new Vector2(-slideDistance, 0); break;
        }
        return originalPosition + offset;
    }

    private Vector2 GetSlideEnd()
    {
        Vector2 offset = Vector2.zero;
        switch (slideDirection)
        {
            case SlideDirection.Top: offset = new Vector2(0, -slideDistance); break;
            case SlideDirection.Right: offset = new Vector2(-slideDistance, 0); break;
            case SlideDirection.Bottom: offset = new Vector2(0, slideDistance); break;
            case SlideDirection.Left: offset = new Vector2(slideDistance, 0); break;
        }
        return originalPosition + offset;
    }

    private void PlayEntryInternal(System.Action onComplete)
    {
        if (currentSequence != null && currentSequence.IsActive())
            currentSequence.Kill();

        currentSequence = DOTween.Sequence();

        switch (animationType)
        {
            case AnimationType.Fade:
                currentSequence.Append(canvasGroup.DOFade(fadeTo, animationDuration).SetEase(easeEnter));
                break;

            case AnimationType.Scale:
                currentSequence.Append(targetRect.DOScale(scaleTo, animationDuration).SetEase(easeEnter));
                break;

            case AnimationType.Slide:
                currentSequence.Append(targetRect.DOAnchorPos(originalPosition, animationDuration).SetEase(easeEnter));
                break;

            case AnimationType.FadeAndScale:
                currentSequence.Join(canvasGroup.DOFade(fadeTo, animationDuration).SetEase(easeEnter));
                currentSequence.Join(targetRect.DOScale(scaleTo, animationDuration).SetEase(easeEnter));
                break;

            case AnimationType.FadeAndSlide:
                currentSequence.Join(canvasGroup.DOFade(fadeTo, animationDuration).SetEase(easeEnter));
                currentSequence.Join(targetRect.DOAnchorPos(originalPosition, animationDuration).SetEase(easeEnter));
                break;
        }

        currentSequence.OnComplete(() => onComplete?.Invoke());
    }

    private void PlayExitInternal(System.Action onComplete)
    {
        if (currentSequence != null && currentSequence.IsActive())
            currentSequence.Kill();

        currentSequence = DOTween.Sequence();

        switch (animationType)
        {
            case AnimationType.Fade:
                currentSequence.Append(canvasGroup.DOFade(fadeFrom, animationDuration).SetEase(easeExit));
                break;

            case AnimationType.Scale:
                currentSequence.Append(targetRect.DOScale(scaleFrom, animationDuration).SetEase(easeExit));
                break;

            case AnimationType.Slide:
                currentSequence.Append(targetRect.DOAnchorPos(GetSlideEnd(), animationDuration).SetEase(easeExit));
                break;

            case AnimationType.FadeAndScale:
                currentSequence.Join(canvasGroup.DOFade(fadeFrom, animationDuration).SetEase(easeExit));
                currentSequence.Join(targetRect.DOScale(scaleFrom, animationDuration).SetEase(easeExit));
                break;

            case AnimationType.FadeAndSlide:
                currentSequence.Join(canvasGroup.DOFade(fadeFrom, animationDuration).SetEase(easeExit));
                currentSequence.Join(targetRect.DOAnchorPos(GetSlideEnd(), animationDuration).SetEase(easeExit));
                break;
        }

        currentSequence.OnComplete(() =>
        {
            targetRect.anchoredPosition = originalPosition;
            onComplete?.Invoke();
        });
    }

    // Restaurar estado original
    public void ResetUI()
    {
        targetRect.anchoredPosition = originalPosition;
        targetRect.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
    }
}
