using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Photo {
    public RenderTexture image;
    public string species;
    public string description;
    public string habitat;
}

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private int maxStoredImages = 3;
    [SerializeField]
    private float maxDetectionDistance = 1f; // Maximum distance for plant detection
    private int currentStoredImages = 0;
    [SerializeField]
    private GameObject plantHolder;

    private Camera cameraComponent;
    // private RenderTexture[] snapshots;
    private Photo[] photos;
    [SerializeField]
    private RawImage previewImage;
    [SerializeField]
    private AudioSource shutterSound;
    private TextMeshProUGUI imageCountText;
    private TextMeshProUGUI plantNameText;
    private TextMeshProUGUI statusText;

    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GameObject.Find("Snapshot Camera").GetComponent<Camera>();
        photos = new Photo[0];
        imageCountText = GameObject.Find("Picture count text").GetComponent<TextMeshProUGUI>();
        imageCountText.text = "Pictures taken: " + currentStoredImages + "/" + maxStoredImages;
        plantNameText = GameObject.Find("Plant text").GetComponent<TextMeshProUGUI>();
        plantNameText.text = "Plant detected:\n - None";
        statusText = GameObject.Find("Status text").GetComponent<TextMeshProUGUI>();
        statusText.text = "Status: Ready";
        previewImage.texture = null; // Initialize with no texture
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shutter()
    {
        CaptureImage();
    }

    private void CaptureImage()
    {
        if (currentStoredImages >= maxStoredImages)
        {
            return;
        }
        // Play the shutter sound
        if (shutterSound != null)
        {
            shutterSound.Play();
        }

        // Create and activate the render texture
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
        renderTexture.Create();
        
        // Render the camera to the texture
        cameraComponent.targetTexture = renderTexture;
        cameraComponent.Render();
        cameraComponent.targetTexture = null; // Reset target texture
        
        // Store the snapshot
        Photo newPhoto = new Photo();
        newPhoto.image = renderTexture;
        IdentifyPlant(newPhoto); // Call the plant identification function

        if (currentStoredImages >= photos.Length)
        {
            // Expand the array size
            Photo[] newPhotos = new Photo[currentStoredImages + 1];
            for (int i = 0; i < currentStoredImages; i++)
            {
                newPhotos[i] = photos[i];
            }
            photos = newPhotos;
        }

        photos[currentStoredImages] = newPhoto; // Store the new photo
        
        // Update the preview - make sure we check if previewImage exists
        if (previewImage != null)
        {
            previewImage.texture = renderTexture; 
            previewImage.enabled = true; // Make sure the image is visible
        }
        
        currentStoredImages++;
        imageCountText.text = "Pictures taken: " + currentStoredImages + "/" + maxStoredImages;
        plantNameText.text = "Plant detected:\n - " + newPhoto.species; // Update the plant name text
        if (currentStoredImages >= maxStoredImages)
        {
            statusText.text = "Status: Full!";
        }

    }

    public Photo[] GetPictures() {
        Photo[] deepCopy = new Photo[photos.Length];
        for (int i = 0; i < photos.Length; i++)
        {
            deepCopy[i] = new Photo
            {
                image = CopyRenderTexture(photos[i].image), // Create a new RenderTexture and copy the data
                species = photos[i].species, // Direct assignment as strings are immutable
                description = photos[i].description, // Direct assignment as strings are immutable
                habitat = photos[i].habitat // Direct assignment as strings are immutable
                
            };
        }
        return deepCopy;
    }

    public void ClearPictures() {
        photos = new Photo[0];
        currentStoredImages = 0;
        imageCountText.text = "Pictures taken: " + currentStoredImages + "/" + maxStoredImages;
        previewImage.texture = null; // Clear the preview image
        plantNameText.text = "Plant detected:\n - None";
        statusText.text = "Status: Ready";
    }

    private void IdentifyPlant(Photo photo) {

        // Loop through the plantHolder children to find the ones that are in the camera view and within maxDetectionDistance
        foreach (Transform child in plantHolder.transform) {
            // Check if the child is within the camera's view frustum
            Vector3 screenPoint = cameraComponent.WorldToViewportPoint(child.position);
            if (screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0) {
                // Check if the child is within the maximum detection distance
                float distance = Vector3.Distance(cameraComponent.transform.position, child.position);
                if (distance <= maxDetectionDistance) {
                    // If the child is in view and within range, return its name
                    photo.species = child.GetComponent<PlantScript>().species; // Assuming the PlantScript has a species field
                    photo.description = child.GetComponent<PlantScript>().description; // Assuming the PlantScript has a species field
                    photo.habitat = child.GetComponent<PlantScript>().habitat; // Assuming the PlantScript has a species field
                    return; // Exit the loop after finding the first plant
                }
            }
        }

        photo.species = "No plant detected"; // Default value if no plant is found
        photo.description = "";
        photo.habitat = "";
    }

    private RenderTexture CopyRenderTexture(RenderTexture source)
    {
        RenderTexture copy = new RenderTexture(source.width, source.height, source.depth, source.format);
        Graphics.Blit(source, copy);
        return copy;
    }
}
