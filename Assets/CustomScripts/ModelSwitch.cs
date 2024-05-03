using System;
using UnityEngine;
using UnityEngine.UI;

public class ModelSwitch : MonoBehaviour
{
    public GameObject[] objectsToChange;
    public GameObject[] wallObjects;
    public Slider hueSlider;
    public GameObject fillObject;
    public Button confirmButton;
    public Button backButton;
    public Button resetButton;
    public GameObject imagePanel;
    public Button colorMenuButton;
    public Button sofaButton;
    public Button fridgeButton;
    public Button wallButton;
    public Button bedButton;
    private Color[] originalColors;
    private Color[] originalWallColors;
    private Color currentColor;
    private GameObject activeObject;

    void Start()
    {
        imagePanel.SetActive(false);
        backButton.gameObject.SetActive(false);
        hueSlider.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        originalColors = new Color[objectsToChange.Length];
        for (int i = 0; i < objectsToChange.Length; i++)
        {
            Renderer renderer = objectsToChange[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                originalColors[i] = renderer.material.color;
            }
        }
        originalWallColors = new Color[wallObjects.Length];
        for (int i = 0; i < wallObjects.Length; i++)
        {
            Renderer renderer = wallObjects[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                originalWallColors[i] = renderer.material.color;
            }
        }
    }

    void Update()
    {
        confirmButton.onClick.AddListener(ConfirmColor);
        backButton.onClick.AddListener(BackToModelSelection);
        colorMenuButton.onClick.AddListener(ToggleColorMenu);
        resetButton.onClick.AddListener(ResetColor);
        Button[] imageButtons = imagePanel.GetComponentsInChildren<Button>();
        foreach (Button button in imageButtons)
        {
            button.onClick.AddListener(() => OnImageClicked(button));
        }
        UpdateFillObjectColor(hueSlider.value);
    }

    void UpdateFillObjectColor(float hueValue)
    {
        currentColor = Color.HSVToRGB(hueValue, 1f, 1f);
        if (fillObject != null)
        {
            Image fillImage = fillObject.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = currentColor;
            }
        }
    }

    void ToggleColorMenu()
    {
        bool isActive = !imagePanel.activeSelf;
        imagePanel.SetActive(isActive);
    }

    void ChangeModelColor(Color hue)
    {
        if (activeObject != null)
        {
            Renderer[] renderers = activeObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.color = hue;
                }
            }
            if (activeObject == objectsToChange[2])
            {
                foreach (GameObject wallObject in wallObjects)
                {
                    Renderer[] wallRenderers = wallObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer wallRenderer in wallRenderers)
                    {
                        foreach (Material material in wallRenderer.materials)
                        {
                            material.color = hue;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No active object found.");
        }
    }

    void ConfirmColor()
    {
        Debug.Log("Color confirmed!");
        ChangeModelColor(currentColor);
        backButton.gameObject.SetActive(false);
        hueSlider.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
    }

    void BackToModelSelection()
    {
        Debug.Log("Back to model selection!");
        backButton.gameObject.SetActive(false);
        hueSlider.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
    }

    void ResetColor()
    {
        Debug.Log("Color reset!");
        ChangeModelColorToOriginal();
    }

    void ChangeModelColorToOriginal()
    {
        if (activeObject != null)
        {
            int index = System.Array.IndexOf(objectsToChange, activeObject);
            if (index != -1)
            {
                Renderer[] renderers = activeObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.color = originalColors[index];
                    }
                }
                if (activeObject == objectsToChange[2])
                {
                    foreach (GameObject wallObject in wallObjects)
                    {
                        Renderer[] wallRenderers = wallObject.GetComponentsInChildren<Renderer>();
                        foreach (Renderer wallRenderer in wallRenderers)
                        {
                            foreach (Material material in wallRenderer.materials)
                            {
                                material.color = originalWallColors[index];
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No active object found.");
        }
    }

    void OnImageClicked(Button button)
    {
        Debug.Log("Image clicked: " + button.name);
        backButton.gameObject.SetActive(true);
        hueSlider.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(true);
        if (button == sofaButton)
        {
            activeObject = objectsToChange[0];
        }
        else if (button == fridgeButton)
        {
            activeObject = objectsToChange[1];
        }
        else if (button == wallButton)
        {
            activeObject = objectsToChange[2];
        }
        else if (button == bedButton)
        {
            activeObject = objectsToChange[3];
        }
    }
}