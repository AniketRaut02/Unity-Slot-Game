using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float moveSpeed = 75f;
    [SerializeField] private float fadeDuration = 1.2f;

    public void Setup(string message, Color color)
    {
        textMesh.text = message;
        textMesh.color = color;
        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Color startColor = textMesh.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Move slightly upwards every frame
            rect.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

            // Fade the alpha channel smoothly
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

            yield return null;
        }

        // Clean up the object once the animation is done
        Destroy(gameObject);
    }
}