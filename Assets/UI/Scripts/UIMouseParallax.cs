using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIImageFollowMouse : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField, Range(1f, 10f)]
    private float movimientoMaximo = 5f; // Máximo desplazamiento en píxeles

    [SerializeField, Range(0.01f, 0.3f)]
    private float suavizado = 0.1f; // Qué tan suave es el movimiento

    private RectTransform rectTransform;
    private Vector3 posicionInicial;
    private Vector3 velocidad = Vector3.zero;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        posicionInicial = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Normalizar la posición del mouse (-1 a 1)
        float x = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float y = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Calcular el desplazamiento sutil
        Vector3 objetivo = posicionInicial + new Vector3(x, y, 0) * movimientoMaximo;

        // Movimiento suave hacia la posición objetivo
        rectTransform.anchoredPosition = Vector3.SmoothDamp(
            rectTransform.anchoredPosition,
            objetivo,
            ref velocidad,
            suavizado
        );
    }

    void OnDisable()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = posicionInicial;
    }
}
