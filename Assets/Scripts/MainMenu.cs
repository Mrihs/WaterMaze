using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

/*
This script saves the variables of the experiment and handles the main menu
*/

public class MainMenu : MonoBehaviour
{
    //////////////////// Define Variables ////////////////////
    [Header("Canvases")]
    [Tooltip("Canvas for the main menu")]
    public GameObject MainMenuCanvas;
    [Tooltip("Canvas which instructs to reset headset's callibrations")]
    public GameObject ResetVRCanvas;

    [Header("Condition Selection")]
    [Tooltip("Dropdown to select the condition")]
    public TMP_Dropdown dropdownCondition;
    [Tooltip("Dropdown to select the navigation-type")]
    public TMP_Dropdown dropdownNavigation;

    [Header("Input Field")]
    [Tooltip("Input field to assign the current participant")]
    public TMP_InputField SubjInputField;
    [Tooltip("Input field to assign the current run")]
    public TMP_InputField RunInputField;

    [Header("Inputs for Data Directories")]
    [Tooltip("Input field to define directory of current condition")]
    public TMP_InputField ConditionFileInputField;
    [Tooltip("Input field to define directory of data storage")]
    public TMP_InputField DataStoragePathInputField;

    [Header("Error Messages")]
    [Tooltip("Text-object to display error-message regarding the data directories")]
    public TMP_Text FilesErrorText;
    [Tooltip("Text-object to display error-message regarding participant's infos")]
    public TMP_Text InputErrorText;

    [Header("Recalibration Instructions")]
    [Tooltip("Text-object to give instructions on recalibrating the VR-headset")]
    public TMP_Text ResetInstruction;



    //The current participant
    private string Subject;
    //The current run
    private string Run;

    //The selected condition
    private string selectedCondition;
    //The selected navigation type
    private string selectedNavigation;

    //The directory of the condition-file
    private string ConditionFilePath;
    //The directory of the data storage
    private string DataStoragePath;

    //A mark to check if the condition-file is found
    private bool ConditionFileCorrect;
    //A mark to check if the directories are found
    private bool DatastoragePathCorrect;
    //A mark to check if participant's infos are assigned
    private bool InputsCorrect;









    //////////////////// At the beginning of the scene ////////////////////
    private void Start()
    {
        //Activate the canvas of the main menu
        MainMenuCanvas.SetActive(true);
        //Deactivate the canvas with recalibration instructoins
        ResetVRCanvas.SetActive(false);


        //Check if the directory of the condition-file is saved in the player preferences
        if (PlayerPrefs.HasKey("ConditionFilePath"))
        {
            //Assign the directory of the condition-file to the respective text-field
            ConditionFileInputField.text = PlayerPrefs.GetString("ConditionFilePath");
        }

        //Check if the directory of the data storage path is saved in the player preferences
        if (PlayerPrefs.HasKey("DataStoragePath"))
        {
            //Assign the directory of the data storage to the respective text-field
            DataStoragePathInputField.text = PlayerPrefs.GetString("DataStoragePath");
        }
    }









    //////////////////// Every Frame ////////////////////
    public void Update()
    {
        //Assign the input of the DataStorage-textfield to the respective variable
        DataStoragePath = DataStoragePathInputField.text;
        //Assign the input of the Conditoinfile to the respective variable
        ConditionFilePath = ConditionFileInputField.text;


        // Check if the condition-file exists
        if (File.Exists(ConditionFilePath))
        {
            //If it exists: Check if the condition-file is a CSV-File
            if (Path.GetExtension(ConditionFilePath).ToLower() == ".csv")
            {
                //Remark that the condition-file is correct
                ConditionFileCorrect = true;
            }
            else
            {
                //Display an error-message if the condition-file is not a csv-file
                FilesErrorText.text = "Condition-File is not in CSV-Format!";
                //Remark that the condition-file is not correct
                ConditionFileCorrect = false;
            }
        }
        else
        {
            //Display an error-message that the correct path of the condition-file should be added
            FilesErrorText.text = "Add correct path of the condition-file";
            //Remark that the condition-file is not correct
            ConditionFileCorrect = false;
        }


        // Check if the directory for the datastorage does exist
        if (!Directory.Exists(DataStoragePath))
        {
            //Display an error-message that the directory for the data storage does not exist
            FilesErrorText.text = "Directory for data storage does not exist!";
            //Remark that the data storage directory is not correct
            DatastoragePathCorrect = false;
            return;
        }
        else
        {
            //Remark that the data storage directory is correct
            DatastoragePathCorrect = true;
        }



        //Remove text from error-message if both pathes are correct
        if (DatastoragePathCorrect==true && ConditionFileCorrect == true)
        {
            FilesErrorText.text = "";
        }


        //Check if participant's ID and run have been assigned
        if (SubjInputField.text != "" && RunInputField.text != "")
        {
            //Remark if participant's ID and run have been assigned 
            InputsCorrect = true;
        }
        //Remark if participant's ID and run have not been assigned 
        else
        {
            InputsCorrect = false;
        }






        // If Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Call function QuitGame
            QuitGame();
        }
    }









    //////////////////// Function to Start Experiment ////////////////////
    public void StartGame()
    {
        //Check if both pathes are correct
        if (DatastoragePathCorrect == true && ConditionFileCorrect == true)
        {
            // Check if the inputs are correct
            if (InputsCorrect == true)
            {
                // Save the option of the condition-dropdown as the selected condition
                selectedCondition = dropdownCondition.options[dropdownCondition.value].text;
                // Save the option of the navigation-dropdown as the selected navigation
                selectedNavigation = dropdownNavigation.options[dropdownNavigation.value].text;

                // Save the assigned ID for the current participan
                Subject = SubjInputField.text;
                // Save the assigned run
                Run = RunInputField.text;

                // Save the input from the conditionfile-path input field
                ConditionFilePath = ConditionFileInputField.text;

                // Save the input from the data storage path input field
                DataStoragePath = DataStoragePathInputField.text;

                // Set the variables in the player preferences
                PlayerPrefs.SetString("Condition", selectedCondition);
                PlayerPrefs.SetString("Navigation", selectedNavigation);
                PlayerPrefs.SetString("Subject", Subject);
                PlayerPrefs.SetString("Run", Run);
                PlayerPrefs.SetString("ConditionFilePath", ConditionFilePath);
                PlayerPrefs.SetString("DataStoragePath", DataStoragePath);

                //Save PlayerPrefs
                PlayerPrefs.Save();





                // If the selected navigation-type is not Screen
                if (selectedNavigation!= "Screen")
                {
                    //Define reset-instructions if the navigation-type is VR Static
                    if (selectedNavigation == "VR Static")
                    {ResetInstruction.text = "Please put the VR - Headset on the floor at the start position and reset the headset in SteamVR \n \n Afterward, put headset on participant's head.";}

                    //Define reset-instructions if the navigation-type is VR Walking
                    if (selectedNavigation == "VR Walking")
                    {ResetInstruction.text = "Please put the VR-Headset on the floor at the start position and reset the headset in SteamVR \n \n Afterward, put headset on participant's head.";}

                    //Deactivate the canvas for the main menu
                    MainMenuCanvas.SetActive(false);
                    //Activate the canvas with the instructions to recalibrate the VR headset
                    ResetVRCanvas.SetActive(true);
                }

                // If the selected navigation-type is Screen
                if (selectedNavigation == "Screen")
                {
                    // Load the next Scene
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
            // Print an error message if participant's ID or the run are not assigned
            else
            {InputErrorText.text = "Define Participant's ID and Run";}
        }
    }









    //////////////////// Function to Confirm Recalibration of VR Headset ////////////////////
    public void CofnfirmStart()
    {
        // Load the next Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }









    //////////////////// Function to Return to Main Menu ////////////////////
    public void Back()
    {
        //Activate the canvas of the main menu
        MainMenuCanvas.SetActive(true);
        //Deactivate the canvas with recalibration instructoins
        ResetVRCanvas.SetActive(false);
    }









    //////////////////// Function to Quit the Application ////////////////////
    public void QuitGame()
    {
        //Debug log that the application should quit
        Debug.Log("Application Quitted");
        //Quit application
        Application.Quit();
    }
}
