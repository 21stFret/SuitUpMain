using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTextWithImage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] public Image inlineImage;  // Reference to the actual image we'll reuse
    
    private const string IMAGE_TAG = "<image>";
    private const string IMAGE_END_TAG = "</image>";
    
    private void Awake()
    {
        // Ensure the image starts hidden
        if (inlineImage != null)
        {
            inlineImage.gameObject.SetActive(false);
        }
    }
    
    // Call this method to set up your tutorial text with an image
    public void SetTutorialText(string text, Sprite image)
    {
        bool hasImage = image != null;

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (!hasImage)
        {
            inlineImage.gameObject.SetActive(false);
            tutorialText.text = text;
        }
        else
        {
            // Find the position of the image tag
            int imageTagStart = text.IndexOf(IMAGE_TAG);
            int imageTagEnd = text.IndexOf(IMAGE_END_TAG);

            if (imageTagStart == -1 || imageTagEnd == -1)
            {
                //Debug.LogError("Image tags not found in text!" + "\n Bad Text :  " + text);
                inlineImage.gameObject.SetActive(false);
            }

            Vector3 imageSize = new Vector3(inlineImage.rectTransform.sizeDelta.x, inlineImage.rectTransform.sizeDelta.y, 0);

            // Update the existing image
            inlineImage.gameObject.SetActive(true);
            inlineImage.sprite = image;

            // Replace the image tags with a space for the inline image
            string beforeImage = text.Substring(0, imageTagStart);
            string afterImage = text.Substring(imageTagEnd + IMAGE_END_TAG.Length);

            // Add a space for the image using TMP's sprite asset system
            string finalText = beforeImage + "         " + afterImage;
            tutorialText.text = finalText;

            // Wait for text to update before positioning the image
            Canvas.ForceUpdateCanvases();
            tutorialText.ForceMeshUpdate();

            // Calculate the position for the image
            TMP_TextInfo textInfo = tutorialText.textInfo;
            if (textInfo.characterCount > 0)
            {
                int charIndex = beforeImage.Length;
                if (charIndex < textInfo.characterCount)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
                    Vector3 charPosition = charInfo.bottomLeft + (imageSize / 2);
                    charPosition.y = 0;

                    // Position the image at the character position
                    inlineImage.rectTransform.position = tutorialText.transform.TransformPoint(charPosition);
                }
            }
        }
    }

    // Clear the tutorial text and hide the image
    public void Clear()
    {
        tutorialText.text = "";
        inlineImage.gameObject.SetActive(false);
    }

    public void UpdateImage(Sprite sprite)
    {
        inlineImage.sprite = sprite;
    }
}
