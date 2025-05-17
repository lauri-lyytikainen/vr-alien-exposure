using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;


public class ComputerScript : MonoBehaviour
{
    private Photo[] photos;
    private int pictureIndex;
    [SerializeField]
    private RawImage previewImage;
    [SerializeField]
    private TextMeshProUGUI imageCountText;
    [SerializeField]
    private TextMeshProUGUI plantNameText;
    [SerializeField]
    private TextMeshProUGUI plantDescriptionText;
    [SerializeField]
    private TextMeshProUGUI plantHabitatText;
    [SerializeField]
    private TextMeshProUGUI plantCountText;
    [SerializeField]
    private int plantTotal = 6;
    [SerializeField]
    private CameraScript cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        pictureIndex = 0;
        photos = new Photo[0]; 
        UpdateImageText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EmptyCamera(HoverEnterEventArgs args) {
        Photo[] cameraPictures = cameraScript.GetPictures();
        int newLength = cameraPictures.Length + photos.Length;
        Photo[] newPictures = new Photo[newLength];
        for (int i = 0; i < photos.Length; i++) {
            newPictures[i] = photos[i];
        }
        for (int i = 0; i < cameraPictures.Length; i++) {
            newPictures[i + photos.Length] = cameraPictures[i];
        }
        photos = newPictures;
        pictureIndex = photos.Length - 1; // Set to the last picture taken
        if (photos.Length > 0)
        {
            SetPreviewImage(photos[pictureIndex].image);
        }
        UpdateImageText();
        cameraScript.ClearPictures(); // Clear the pictures from the camera
    }

    private void SetPreviewImage(RenderTexture texture) {
        previewImage.texture = texture;
    }

    private void UpdateImageText()
    {
        imageCountText.text = photos.Length + " Images stored \n Currently viewing image " + (pictureIndex + 1);
        if (photos.Length > 0)
        {
            plantNameText.text = "Plant name: " + photos[pictureIndex].species;
            plantDescriptionText.text = "Description: " + photos[pictureIndex].description;
            plantHabitatText.text = "Habitat: " + photos[pictureIndex].habitat;
        }
        else
        {
            plantNameText.text = "Plant name: ";
            plantDescriptionText.text = "Description: ";
            plantHabitatText.text = "Habitat: ";
        }
        int uniquePlants = 0;
        Dictionary<string, int> plantCounts = new Dictionary<string, int>();
        for (int i = 0; i < photos.Length; i++)
        {
            if (plantCounts.ContainsKey(photos[i].species))
            {
                plantCounts[photos[i].species]++;
            }
            else
            {
                plantCounts[photos[i].species] = 1;
                uniquePlants++;
            }
        }
        if (uniquePlants == plantTotal)
        {
            plantCountText.text = "All plant species discovered!";
        }
        else
        {
             plantCountText.text = plantTotal - uniquePlants + " plant species left to discover";
        }
    }

    public void NextImage() {
        if (photos.Length == 0) return; // No photos to view
        pictureIndex = (pictureIndex + 1) % photos.Length; // Loop back to the start
        SetPreviewImage(photos[pictureIndex].image);
        UpdateImageText();
    }

    public void PreviousImage() {
        if (photos.Length == 0) return; // No photos to view
        pictureIndex = (pictureIndex - 1 + photos.Length) % photos.Length; // Loop back to the end
        SetPreviewImage(photos[pictureIndex].image);
        UpdateImageText();
    }

    public void DeleteImage() {
        if (photos.Length == 0) return; // No photos to delete
        Photo[] newphotos = new Photo[photos.Length - 1];
        for (int i = 0, j = 0; i < photos.Length; i++) {
            if (i != pictureIndex) {
                newphotos[j++] = photos[i];
            }
        }
        photos = newphotos;
        pictureIndex = Mathf.Clamp(pictureIndex - 1, 0, photos.Length - 1); // Adjust index after deletion
        if (photos.Length > 0) {
            SetPreviewImage(photos[pictureIndex].image);
        } else {
            previewImage.texture = null; // Clear the preview if no images left
        }
        UpdateImageText();
    }        
}
