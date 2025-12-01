using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script changes the textures of the maze and the background based on the current navigation type
*/

public class TextureChanger : MonoBehaviour
{
    //////////////////// Define Variables ////////////////////
    [Header("Materials for screen condition")]
    [Tooltip("Material for the background during the screen condition")]
    public Material BackgroundScreenMaterial;
    [Tooltip("Material for the t-maze during the screen condition")]
    public Material MazeScreenMaterial;
    [Tooltip("Material for the t-maze floor during the screen condition")]
    public Material FloorScreenMaterial;

    [Header("Materials for VR-controller condition")]
    [Tooltip("Material for the background during the VR-controller condition")]
    public Material BackgroundVRControllerMaterial;
    [Tooltip("Material for the t-maze during the VR-controller condition")]
    public Material MazeVRControllerMaterial;
    [Tooltip("Material for the t-maze floor during the VR-controller condition")]
    public Material FloorVRControllerMaterial;

    [Header("Materials for VR-walking condition")]
    [Tooltip("Material for the background during the VR-walking condition")]
    public Material BackgroundVRWalkingMaterial;
    [Tooltip("Material for the t-maze during the VR-walking condition")]
    public Material MazeVRWalkingMaterial;
    [Tooltip("Material for the t-maze floor during the VR-walking condition")]
    public Material FloorVRWalkingMaterial;


    [Header("Gameobjects for which materials are changed")]
    [Tooltip("Parts of the t-maze")]
    public List<GameObject> MazeParts;
    [Tooltip("Parts of the background")]
    public List<GameObject> BackgroundParts;
    [Tooltip("Parts of the t-maze floor")]
    public List<GameObject> FloorParts;


    //Define an internal variable for the navigation-type
    private string NavigationType;









    //////////////////// Every Frame ////////////////////
    void Start()
    {
        //Check if player-preferences have the Key "Navigation"
        if (PlayerPrefs.HasKey("Navigation"))
        {
            //Assign the NavigationType from the player-preferences
            NavigationType = PlayerPrefs.GetString("Navigation");
        }
        else //Define NavigationType as "Screen" if NavigationType is not saved in the player-preferences
        { NavigationType = "Screen"; }





        // Call function AssignMaterials to assign the materials to the respective objects
        if (NavigationType == "Screen")
        {
            AssignMaterials(BackgroundParts, BackgroundScreenMaterial);
            AssignMaterials(MazeParts, MazeScreenMaterial);
            AssignMaterials(FloorParts, FloorScreenMaterial);
        }

        if (NavigationType == "VR Walking")
        {
            AssignMaterials(BackgroundParts, BackgroundVRWalkingMaterial);
            AssignMaterials(MazeParts, MazeVRWalkingMaterial);
            AssignMaterials(FloorParts, FloorVRWalkingMaterial);
        }

        if (NavigationType == "VR Static")
        {
            AssignMaterials(BackgroundParts, BackgroundVRControllerMaterial);
            AssignMaterials(MazeParts, MazeVRControllerMaterial);
            AssignMaterials(FloorParts, FloorVRControllerMaterial);
        }
    }









    //////////////////// AssignMaterials-Function ////////////////////
    void AssignMaterials(List<GameObject> gameObjects, Material material)
    {
        // For each game-object in the list of game-objects
        foreach (GameObject obj in gameObjects)
        {
            //Get the renderer of the object
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Assign the material to the object's renderer
                renderer.material = material;
            }
        }
    }
}
