using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Globalization;

/*
This script handles the trials during the t-maze experiment
*/

public class Trial_Handler : MonoBehaviour
{
    //////////////////// Define Variables ////////////////////
    [Header("Player-Objects")]
    [Tooltip("A game-object which marks the start position of the player")]
    public GameObject position_Start;
    [Tooltip("The game-object for the player during the screen condition")]
    public GameObject PCPlayer;
    [Tooltip("The game-object for the player during the VR conditions")]
    public GameObject VRPlayer;
    [Tooltip("The VR-Camera game-object")]
    public Camera vrCamera;
    [Tooltip("The VR-Camera collider-object")]
    public GameObject vrCollider;


    [Header("Camera Raycast")]
    [Tooltip("Layers to exclude for raycast of gaze")]
    public LayerMask excludeLayerMask;

    [Header("T-Maze")]
    [Tooltip("The T-Maze game-object")]
    public GameObject Maze;
    [Tooltip("The colliders of the T-Maze")]
    public List<GameObject> MazeCollider;

    [Header("Background")]
    [Tooltip("The background game-object")]
    public GameObject Cube;

    [Header("Event Colliders")]
    [Tooltip("The collider on the left side of the canvas")]
    public Collider LeftCollider;
    [Tooltip("The collider on the right side of the canvas")]
    public Collider RightCollider;
    [Tooltip("The collider on the start position")]
    public Collider StartCollider;

    [Header("Canvases")]
    [Tooltip("The canvas on the left side of the T-Maze")]
    public GameObject leftCanvas;
    [Tooltip("The canvas on the right side of the T-Maze")]
    public GameObject rightCanvas;
    [Tooltip("The canvas on the start position")]
    public GameObject startCanvas;

    [Header("Cues for the start position")]
    [Tooltip("Arrow to display the start position")]
    public GameObject startArrow;
    [Tooltip("Circle to display the start position")]
    public GameObject startCircle;

    [Header("Light Objects")]
    [Tooltip("The Skybox in the current scene")]
    public Material Skybox;
    [Tooltip("The directional light in the current scene")]
    public Light directionalLight;

    [Header("Movement Settings of player")]
    [Tooltip("The speed of the player's movement")]
    public float playerMovementSpeed = 3;
    [Tooltip("The speed of the player's rotation")]
    public float playerRotationSpeed = 90;


    //The current participant
    private string Subject;
    //The current run
    private string Run;

    //The selected navigatoin-type
    private string NavigationType;
    //The selected condition
    private string Condition;

    // A variable for the navigationtype in the csv's name
    string navigationVariable;

    //A variable to remark hits of the raycast of participant's gaze
    private string GazeObject = "None";

    //The position of the VR-camera
    private Vector3 VRCameraPosition;
    //The rotation of the VR-camera
    private Vector3 VRCameraRotation;

    //the start position of the player
    private Vector3 startPosition;

    //The position of the left-canvas
    private Vector3 leftCanvasOriginalPosition;
    //The position of the right-canvas
    private Vector3 rightCanvasOriginalPosition;
    //The rotation of the left-canvas
    private Quaternion leftCanvasOriginalRotation;
    //The rotation of the right-canvas
    private Quaternion rightCanvasOriginalRotation;

    //The current state of collissions with canvases
    private string currentCanvasCollission;


    //The current Trial
    private int Trial;

    //The max number of trials
    private int MaxTrial;

    //The side of the target in the current trials
    private string TargetSide;

    //The correct canvas during the current trials
    private GameObject currentCanvas;



    //A variable to check if a collision is detected
    private bool collisionDetected = false;

    //A rariable to check whether the Side has changed
    private bool sideChanged = false;

    //A variable to check whether the canvases are activated
    private bool canvasActivated = false;

    //A variable to check whether a reset is currently in progress
    private bool resetInProgress = false;


    //A variable for the chosen side
    private string chosenSide = "";

    //Current state during the trials
    private string trialState = "Inactive";


    //Directory of data storage
    private string csvFilePath;


    //Define a class to hold trial data
    private class TrialData
    {
        //The current trial
        public int TrialIndex;
        //The targetside of the respective trial
        public string TargetSide;
        //The rotation of the room during the respective trial
        public float RoomRotation;
    }

    //Define a list to hold trial data
    private List<TrialData> trialDataList = new List<TrialData>();


    //Light settings
    //Initial intensity of the lightbox
    private float initialSkyboxLightIntensityMultiplier = 1.23f;
    //Initial reflection of the lightbox
    private float initialSkyboxReflectionIntensityMultiplier = 1f;
    //Initial intensity of the directoinal light
    private float initialLightInesnity = 1.5f;

    //Define duration of Trial transition
    private float transitionDuration = 2f;

    //Define target Light settings
    private float targetSkyboxLightIntensityMultiplier = 0f;
    private float targetSkyboxReflectionIntensityMultiplier = 0f;
    private float targetLightInesnity = 0f;

    //A gameobject which represents the player
    private GameObject Player;









    //////////////////// At the beginning of the scene ////////////////////
    void Start()
    {
        //Assign the condition if saved in the player-preferences
        if (PlayerPrefs.HasKey("Condition"))
        {Condition = PlayerPrefs.GetString("Condition");}
        else //Define condition as "Normal" if condition is not saved in the player-preferences
        { Condition = "Normal";}


        //Assign the navigationtype if saved in the player-preferences
        if (PlayerPrefs.HasKey("Navigation"))
        {NavigationType = PlayerPrefs.GetString("Navigation");}
        else //Define navigationtype as "Screen" if navigationtype is not saved in the player-preferences
        { NavigationType = "Screen"; }


        //Assign participants ID if saved in the player-preferences
        if (PlayerPrefs.HasKey("Subject"))
        { Subject = PlayerPrefs.GetString("Subject");}
        else //Define participants ID as "Unknown" if Subject is not saved in the player-preferences
        { Subject = "Unknown";}


        //Assign Run if saved in the player-preferences
        if (PlayerPrefs.HasKey("Run"))
        { Run = PlayerPrefs.GetString("Run");}
        else //Define Run as "Unknown" if Run is not saved in the player-preferences
        { Run = "Unknown"; }


        //Get the transform-values of the startCanvas
        RectTransform canvasRectTransform = startCanvas.GetComponent<RectTransform>();



        //If the NavigationType is Screen
        if (NavigationType == "Screen")
        {
            //Set the PCPlayer as Player
            Player = PCPlayer;
            //Deactivate the VRPLayer
            VRPlayer.SetActive(false);


            // Define a variable for the position of the start canvas
            Vector3 newPosition = canvasRectTransform.position;

            // Get the player's y-position
            newPosition.y = Player.transform.position.y;

            // Adjust the position of the start canvas
            canvasRectTransform.position = newPosition;

            // Set the sart canas as a child of the player
            startCanvas.transform.SetParent(Player.transform);
        }



        //If the NavigationType is VR Walking
        if (NavigationType == "VR Walking")
        {
            //Deactivate the camera of the PCPlayer
            PCPlayer.GetComponent<Camera>().enabled = false;
            //Deactivate the audio listener of the PCPlayer
            PCPlayer.GetComponent<AudioListener>().enabled = false;

            //Set the position of the PCPlayer to the vrCamera's position
            PCPlayer.transform.position = vrCamera.transform.position;
            //Set the rotation of the PCPlayer to the vrCamera's position
            PCPlayer.transform.rotation = vrCamera.transform.rotation;

            // Set the PCPlayer as child of the vrCamera
            PCPlayer.transform.SetParent(vrCamera.transform);

            // Set the vrCollider as Player
            Player = vrCollider;


            // Define a variable for the position of the start canvas
            Vector3 newPosition = canvasRectTransform.position;
            // Get the player's y-position
            newPosition.y = vrCamera.transform.position.y;
            // Adjust the position of the start canvas
            canvasRectTransform.position = newPosition;

            // Set the sart canas as a child of the player
            startCanvas.transform.SetParent(vrCamera.transform);
        }


        if (NavigationType == "VR Static")
        {
            // Set the VRPlayer as Player
            Player = VRPlayer;

            //Deactivate the PCPlayer
            PCPlayer.SetActive(false);

            // Define a variable for the position of the start canvas
            Vector3 newPosition = canvasRectTransform.position;
            // Get the player's y-position
            newPosition.y = vrCamera.transform.position.y;
            // Adjust the position of the start canvas
            canvasRectTransform.position = newPosition;

            // Set the sart canas as a child of the player
            startCanvas.transform.SetParent(vrCamera.transform);

        }


        //Call Coroutine activateStartCanvas
        StartCoroutine(activateStartCanvas(3));

        //Call Function ReadCSVFile to read condition file
        ReadCSVFile();

        //Define the number of MaxTrials based on the condition file
        MaxTrial = trialDataList.Count;

        //Save the start-Position by the x- and z-value of the starting-Position and the y-value of the players position
        startPosition = new Vector3(position_Start.transform.position.x, Player.transform.position.y, position_Start.transform.position.z);

        //Move the player to the start position
        Player.transform.position = startPosition;

        //Save the original position of the left canvas
        leftCanvasOriginalPosition = leftCanvas.transform.position;
        //Save the original rotation of the left canvas
        leftCanvasOriginalRotation = leftCanvas.transform.rotation;
        //Save the original position of the right canvas
        rightCanvasOriginalPosition = rightCanvas.transform.position;
        //Save the original rotation of the right canvas
        rightCanvasOriginalRotation = rightCanvas.transform.rotation;


        //Deactivate the Canvases
        leftCanvas.SetActive(false);
        rightCanvas.SetActive(false);
        startCanvas.SetActive(true);


        //Deactivate the StartCues
        startArrow.SetActive(false);
        startCircle.SetActive(false);


        //Save the date and Time
        string dateTime = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm", CultureInfo.InvariantCulture);


        //Define the directory of the data storage based on the player preferences
        string DataStoragePath = PlayerPrefs.GetString("DataStoragePath");

        // Combine participant's ID to get the directory of subfolder
        string folderPath = Path.Combine(DataStoragePath, "sub-" + Subject, "ses-01", "vr");

        //Define a variable for the dataDirectory
        string dataDirectory;


        //Check if there is already a subfolder for this participant
        if (Directory.Exists(folderPath))
        {
            //Save the subfolder as data Directory
            dataDirectory = folderPath;
        }
        //Otherwise create the folder
        else
        {
            Directory.CreateDirectory(folderPath);
            //Save the subfolder as data Directory
            dataDirectory = folderPath;
        }

        // Set the navigationVariable based on NavigationType
        if (NavigationType == "Screen")
        { navigationVariable = "screen"; }
        if (NavigationType == "VR Static")
        { navigationVariable = "static"; }
        if (NavigationType == "VR Walking")
        { navigationVariable = "walking"; }

        // Define the full path for the CSV file output
        csvFilePath = Path.Combine(dataDirectory, $"sub-{Subject}_ses-01_run-{Run}_task-{navigationVariable}.csv");

        // Create the CSV file
        File.WriteAllText(csvFilePath, "subject,navigation,run,task,trial,Target,time,Decision,state,player_position_x,player_position_y,player_position_z,player_rotation_x, player_rotation_y, player_rotation_z, vr_camera_position_x, vr_camera_position_y, vr_camera_position_z, vr_camera_rotation_x, vr_camera_rotation_y, vr_camera_rotation_z,gaze_object\n");

        // Create a debug-Log with the file's path
        Debug.Log("CSV File is at: " + csvFilePath);


        //Adjust intensity of skybox
        RenderSettings.ambientIntensity = initialSkyboxLightIntensityMultiplier;
        //Adjust reflectoin of skybox
        RenderSettings.reflectionIntensity = initialSkyboxReflectionIntensityMultiplier;
        //Adjust intensity of directional light
        directionalLight.intensity = initialLightInesnity;


        //Set state of the trial
        trialState = "Searching";
    }









    //////////////////// Every Frame ////////////////////
    void Update()
    {
        //Move the vrCollider to the vrCamera
        vrCollider.transform.position = vrCamera.transform.position;





        ////////// Set TargetSide and Background Rotation //////////
        // Check if the current trial index is within the range of trialDataList
        if (Trial <= trialDataList.Count)
        {
            // If the condition is Inverted
            if (Condition == "Inverted")
            {
                //If the targetside is not Both
                if (trialDataList[Trial].TargetSide != "Both")
                {
                    // Swap "Left" and "Right" from the condition-file and assign this to the target side
                    TargetSide = trialDataList[Trial].TargetSide == "Left" ? "Right" : "Left";
                }
                else
                {
                    // Otherwise assign "Both" as the TargetSide
                    TargetSide = "Both";
                }
            }
            // If condition is not Inverted
            else
            {
                //Assign the data from the condition-file to the targetside
                TargetSide = trialDataList[Trial].TargetSide;
            }


            // Rotate the background based on RoomRotation from trialDataList
            Cube.transform.rotation = Quaternion.Euler(0, trialDataList[Trial].RoomRotation, 0);
        }





        ////////// Raycast for Gaze //////////
        //If the navigation type is Screen
        if (NavigationType == "Screen")
        {
            // Create a Raycast from the perspective of the PCPlayer
            Ray ray = new Ray(PCPlayer.transform.position, PCPlayer.transform.forward);

            // Send out the raycast
            RaycastHit hit;

            // Check if there is a raycast-colission with an object from the excluded layer
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, excludeLayerMask))
            {
                // Check again for a collission of the Raycast with an object behind the object from the layer mask
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, ~excludeLayerMask);
                //For each hit...
                foreach (var h in hits)
                {
                    //...save the name of the objects with which the Raycast-Collission hits
                    GazeObject = h.collider.gameObject.name;
                }
            }
            //If the raycast collision does not hit with an object from the excluded layer
            else
            {
                // If there is a collission of the raycast...
                if (Physics.Raycast(ray, out hit))
                {
                    //...save the name of the objects with which the Raycast-Collission hits
                    GazeObject = hit.collider.gameObject.name;
                }
                else
                {
                    // Mark if there is no collisison of the raycast
                    GazeObject = "None";
                }
            }
        }



        //If the navigation type is not Screen
        if (NavigationType != "Screen")
        {
            // Create a Raycast from the perspective of the vrCamera
            Ray ray = new Ray(vrCamera.transform.position, vrCamera.transform.forward);

            // Send out the raycast
            RaycastHit hit;

            // Check if there is a raycast-colission with an object from the excluded layer
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, excludeLayerMask))
            {
                // Check again for a collission of the Raycast with an object behind the object from the layer mask
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, ~excludeLayerMask);
                //For each hit...
                foreach (var h in hits)
                {
                    //...save the name of the objects with which the Raycast-Collission hits
                    GazeObject = h.collider.gameObject.name;
                }
            }
            //If the raycast collision does not hit with an object from the excluded layer
            else
            {
                // If there is a collission of the raycast...
                if (Physics.Raycast(ray, out hit))
                {
                    //...save the name of the objects with which the Raycast-Collission hits
                    GazeObject = hit.collider.gameObject.name;
                }
                else
                {
                    // Mark if there is no collisison of the raycast
                    GazeObject = "None";
                }
            }
        }





        ////////// Handle Data and Inputs //////////
        //Call function LogData to save data into CSV-File
        LogData();

        //Call function HandleInput to handle Inputs
        HandleInput();





        ////////// Handle Collissions //////////
        // Check for collision with left collider and whether there is already a reset in progress
        if (LeftCollider.bounds.Intersects(Player.GetComponent<Collider>().bounds) 
            && (TargetSide == "Left" || TargetSide == "Both") 
            && !collisionDetected
            && !canvasActivated)
        {
            //Define the left side as chosen side
            chosenSide = "Left";

            //Define the left canvas as collided canvas
            currentCanvasCollission = "Left";

            //Call function canvasHandler to activate the canvas
            canvasHandler();
        }



        // Check for collision with right collider and whether there is already a reset in progress
        if (RightCollider.bounds.Intersects(Player.GetComponent<Collider>().bounds)
            && (TargetSide == "Right" || TargetSide == "Both")
            && !collisionDetected 
            && !canvasActivated)
        {
            //Define the right side as chosen side
            chosenSide = "Right";

            //Define the right canvas as collided canvas
            currentCanvasCollission = "Right";

            //Call function canvasHandler to activate the canvas
            canvasHandler();
        }



        // Check for collision with start collider when player should return to start position - and check that there is no other reset in progress
        if (StartCollider.bounds.Intersects(Player.GetComponent<Collider>().bounds) && trialState == "Return"
            && !collisionDetected
            && !canvasActivated)
        {
            //Call function StartCoroutine to change the trials
            StartCoroutine(changeTrial());
        }





        ////////// Handle last trial //////////
        // If the current Trial is the fourth Trial (and sides have not been changed)
        if (Trial == 4 && !sideChanged)
        {
            //Start the Coroutine to change the sides
            StartCoroutine(ChangeSide());
        }





        ////////// Handle Keypresses //////////
        // If Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Quit application
            Application.Quit();
        }



        // If Q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Back to Main Menu
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }









    //////////////////// Input Handler-Function ////////////////////
    void HandleInput()
    {
        // Get horizontal input of left joystick
        float leftJoystickHorizontal = Input.GetAxis("LeftJoystickHorizontal");
        // Get vertical input of left joystick
        float leftJoystickVertical = Input.GetAxis("LeftJoystickVertical");
        // Get horizontal input of right joystick
        float rightJoystickHorizontal = Input.GetAxis("RightJoystickHorizontal");
        // Get vertical input of right joystick
        float rightJoystickVertical = Input.GetAxis("RightJoystickVertical");






        //If navigation type is Screen or VR Static
        if (NavigationType == "Screen" || NavigationType == "VR Static")
        {
            // If navigation type is Scren
            if (NavigationType == "Screen")
            {
                // Rotate the player based on the right joystick's horizontal axis
                if (rightJoystickHorizontal != 0)
                {
                    Player.transform.Rotate(0, rightJoystickHorizontal * playerRotationSpeed * Time.deltaTime, 0);
                }

                // Rotate the player based on the right joystick's vertical axis
                if (rightJoystickVertical != 0)
                {
                    Player.transform.Rotate(-rightJoystickVertical * playerRotationSpeed * Time.deltaTime, 0, 0);
                }

                //Reset totation along z-Axis
                Player.transform.rotation = Quaternion.Euler(Player.transform.eulerAngles.x, Player.transform.eulerAngles.y, 0);
            }



            // If navigation type is VR Static
            if (NavigationType == "VR Static")
            {
                //Set vrCamera position to the x- and z-position of the Player 
                vrCamera.transform.position = new Vector3(Player.transform.position.x, vrCamera.transform.position.y, Player.transform.position.z);
            }






            // Calculate a vector for the movement by the joystick's input
            Vector3 movement = new Vector3(leftJoystickHorizontal, 0, leftJoystickVertical).normalized;





            // Only look at movement with a magnitude over 0.1
            if (movement.magnitude >= 0.1f)
            {
                // If Navigation type is "Screen
                if (NavigationType == "Screen")
                {
                    // Calculate the forward position of the player based on Player's position
                    Vector3 forward = new Vector3(Player.transform.forward.x, 0, Player.transform.forward.z).normalized;
                    // Calculate the left/right position of the player based on Player's position
                    Vector3 right = new Vector3(Player.transform.right.x, 0, Player.transform.right.z).normalized;

                    // Ignore backwards movement
                    float forwardMovementFactor = Mathf.Clamp(movement.z, -1, 0);

                    // Add the forward movement to the Player's position
                    Vector3 forwardMovement = forward * -forwardMovementFactor * playerMovementSpeed;
                    // Add the left/right movement to the Player's position
                    Vector3 rightMovement = right * movement.x * playerMovementSpeed;
                    // Calculate the new position of the player
                    Vector3 newPosition = Player.transform.position + (forwardMovement + rightMovement) * Time.deltaTime;

                    // Add the player's height to the calculated position
                    newPosition.y = Player.transform.position.y;

                    // Perform a raycast new position
                    RaycastHit hit;

                    // Set the distance for the raycast
                    float raycastDistance = 10.0f;

                    // If the raycast from the calculated position hits an object downwards
                    if (Physics.Raycast(newPosition, Vector3.down, out hit, raycastDistance))
                    {
                        // Check if the raycast hits a maze-collider
                        if (MazeCollider.Contains(hit.collider.gameObject))
                        {
                            //Set the Player's position to the calculated position
                            Player.transform.position = newPosition;
                        }
                    }
                }



                if (NavigationType == "VR Static")
                {
                    // Calculate the forward position of the player based on Player's position
                    Vector3 forward = new Vector3(vrCamera.transform.forward.x, 0, vrCamera.transform.forward.z).normalized;
                    // Calculate the left/right position of the player based on Player's position
                    Vector3 right = new Vector3(vrCamera.transform.right.x, 0, vrCamera.transform.right.z).normalized;

                    // Ignore backwards movement
                    float forwardMovementFactor = Mathf.Clamp(movement.z, -1, 0);

                    // Add the forward movement to the Player's position
                    Vector3 forwardMovement = forward * -forwardMovementFactor * playerMovementSpeed;
                    // Add the left/right movement to the Player's position
                    Vector3 rightMovement = right * movement.x * playerMovementSpeed;
                    // Calculate the new position of the player
                    Vector3 newPosition = vrCamera.transform.position + (forwardMovement + rightMovement) * Time.deltaTime;

                    // Add the player's height to the calculated position
                    newPosition.y = Player.transform.position.y;

                    // Perform a raycast new position
                    RaycastHit hit;

                    // Set the distance for the raycast
                    float raycastDistance = 10.0f;

                    // If the raycast from the calculated position hits an object downwards
                    if (Physics.Raycast(newPosition, Vector3.down, out hit, raycastDistance))
                    {
                        // Check if the raycast hits a maze-collider
                        if (MazeCollider.Contains(hit.collider.gameObject))
                        {
                            //Set the Player's position to the calculated position
                            Player.transform.position = newPosition;
                        }
                    }
                }
            }
        }
    }









    //////////////////// CanvasHandler-Function ////////////////////
    void canvasHandler()
    {
        // Check if the TargetSide is either Left or Right
        if (TargetSide == "Left" | TargetSide == "Right")
        {
            // Define the respective canvas as currently hitted canvas
            if (TargetSide == "Left")
            { currentCanvas = leftCanvas; }
            if (TargetSide == "Right")
            { currentCanvas = rightCanvas; }

            //Activate the currently hitted canvas
            currentCanvas.SetActive(true);
        }


        // If the current TargetSide is Both
        if (TargetSide == "Both")
        {
            // Activate the left canvas if the collission was with this canvas
            if (currentCanvasCollission == "Left")
            {leftCanvas.SetActive(true); }
            // Activate the right canvas if the collission was with this canvas
            if (currentCanvasCollission == "Right")
            { rightCanvas.SetActive(true); }
        }


        //Activate the arrow and circle at the start position
        startArrow.SetActive(true);
        startCircle.SetActive(true);


        //Start Coroutine ResetCollisionFlag to reset collisionflags 
        StartCoroutine(ResetCollisionFlag());
    }









    //////////////////// ReadCSVFile Function ////////////////////
    private void ReadCSVFile()
    {
        // Get the directory of the condition file from the player preferences
        csvFilePath = PlayerPrefs.GetString("ConditionFilePath");

        // Read all lines from the condition file
        string[] csvLines = File.ReadAllLines(csvFilePath);

        // Loop through each line (starting from index 1 to skip header)
        for (int i = 1; i < csvLines.Length; i++)
        {
            // Split the line by comma to get individual values
            string[] values = csvLines[i].Split(';');

            // Create a new TrialData object and populate its fields
            TrialData trialData = new TrialData();
            // Get the trial index from the first column
            trialData.TrialIndex = int.Parse(values[0]);
            // Get the TargetSide from the second column
            trialData.TargetSide = values[1];
            // Get the room rotation from the third column
            trialData.RoomRotation = float.Parse(values[2]);

            // Add the trialData to the list
            trialDataList.Add(trialData);
        }
    }









    //////////////////// LogData Function ////////////////////
    void LogData()
    {
        // Save the position of the Player
        Vector3 PlayerPosition = Player.transform.position;
        // Save the rotation of the Player
        Vector3 PlayerRotation = Player.transform.eulerAngles;

        //if NavigationType is not Screen
        if (NavigationType != "Screen")
        {
            // Save the position of the vrCamera
            VRCameraPosition = vrCamera.transform.position;
            // Save the rotation of the vrCamera
            VRCameraRotation = vrCamera.transform.eulerAngles;
        }

        //Save the time
        string timeStamp = System.DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

        //if NavigationType is Screen
        if (NavigationType == "Screen")
        {
            //Create a new line for the data output (with "None" for position and rotation of vrCamera
            string logEntry = $"{Subject},{NavigationType},{Run},{Condition},{Trial},{TargetSide},{timeStamp},{trialState},{PlayerPosition.x},{PlayerPosition.y},{PlayerPosition.z},{PlayerRotation.x},{PlayerRotation.y},{PlayerRotation.z},{"None"},{"None"},{"None"},{"None"},{"None"},{"None"},{GazeObject}\n";
            //Append file to csv-output
            File.AppendAllText(csvFilePath, logEntry);
        }
        else
        {
            //Create a new line for the data output
            string logEntry = $"{Subject},{NavigationType},{Condition},{Trial},{TargetSide},{timeStamp},{chosenSide},{trialState},{PlayerPosition.x},{PlayerPosition.y},{PlayerPosition.z},{PlayerRotation.x},{PlayerRotation.y},{PlayerRotation.z},{VRCameraPosition.x},{VRCameraPosition.y},{VRCameraPosition.z},{VRCameraRotation.x},{VRCameraRotation.y},{VRCameraRotation.z},{GazeObject}\n";
            //Append file to csv-output
            File.AppendAllText(csvFilePath, logEntry);
        }
    }









    //////////////////// ChangeSide Function ////////////////////
    private IEnumerator ChangeSide()
    {
        // If the Condition is Left and side has not been changed
        if (Condition == "Left" && sideChanged == false)
        {
            // Wait a second
            yield return new WaitForSeconds(1.0f);

            // Mark that the sides have changed
            sideChanged = true;
        }




        // If the Condition is Right and side has not been changed
        if (Condition == "Right" && sideChanged == false)
        {
            //Wait a second
            yield return new WaitForSeconds(1.0f);

            //Mark that the sides have changed
            sideChanged = true;
        }
    }









    //////////////////// ResetCollisionFlag Function ////////////////////
    private IEnumerator ResetCollisionFlag()
    {
        // Mark that there is a reset in progress
        canvasActivated = true;

        // Change the state of the trial
        trialState = "Return";

        // Wait for 1.5 seconds
        yield return new WaitForSeconds(1.5f);


        // Check if there is a reset in Progress
        if (canvasActivated)
        {
            // Reset the position of the left canvas
            leftCanvas.transform.position = leftCanvasOriginalPosition;
            // Reset the position of the right canvas
            rightCanvas.transform.position = rightCanvasOriginalPosition;
            // Reset the rotation of the left canvas
            leftCanvas.transform.rotation = leftCanvasOriginalRotation;
            // Reset the rotation of the right canvas
            rightCanvas.transform.rotation = rightCanvasOriginalRotation;


            // Deactivate the current canvas
            currentCanvas.SetActive(false);
            // Deactivate the left canvas
            leftCanvas.SetActive(false);
            // Deactivate the right canvas
            rightCanvas.SetActive(false);

            // Reset the value for the chosen side
            chosenSide = "";

            // Reset the value for the activated canvas
            canvasActivated = false;
        }
    }









    //////////////////// activateStartCanvas Function ////////////////////
    private IEnumerator activateStartCanvas(int ActivationDuration)
    {
        // Define a timer
        float CanvasTimer = 0f;

        // Activate the startCanvas
        startCanvas.SetActive(true);

        // Untile the timer is smaller than the activation-duration
        while (CanvasTimer < ActivationDuration)
        {
            // Increase the timer
            CanvasTimer += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Deactivate the startCanvas
        startCanvas.SetActive(false);

        // Wait until the next frame
        yield return null;
    }









    //////////////////// changeTrial Function ////////////////////
    private IEnumerator changeTrial()
    {
        // Deactivate the arrow and circle at the start position
        startArrow.SetActive(false);
        startCircle.SetActive(false);

        // Mark that there is a reset in Progress
        resetInProgress = true;

        // Define a timer
        float elapsedTime = 0f;

        // Change the state of the trial
        trialState = "Trial_Ending";


        // Untile the timer is smaller than the transition-duration
        while (elapsedTime < transitionDuration)
        {
            // Increase the timer
            elapsedTime += Time.deltaTime;

            // Gradually calculate a decreaed value of skybox's intensity
            float currentAmbientIntensity = Mathf.Lerp(initialSkyboxLightIntensityMultiplier, targetSkyboxLightIntensityMultiplier, elapsedTime / transitionDuration);
            // Gradually calculate a decreaed value of skybox's reflection
            float currentReflectionIntensity = Mathf.Lerp(initialSkyboxReflectionIntensityMultiplier, targetSkyboxReflectionIntensityMultiplier, elapsedTime / transitionDuration);
            // Gradually calculate a decreaed value of directoinal light's intensity
            float currentLightIntensity = Mathf.Lerp(initialLightInesnity, targetLightInesnity, elapsedTime / transitionDuration);

            // Assign the calculated value to the skybox's intensity
            RenderSettings.ambientIntensity = currentAmbientIntensity;
            // Assign the calculated value to the skybox's reflection
            RenderSettings.reflectionIntensity = currentReflectionIntensity;
            // Assign the calculated value to the directoinal light's intensity
            directionalLight.intensity = currentLightIntensity;

            // Wait until the next frame
            yield return null;
        }


        // Wait until the next frame
        yield return null;


        // Set the final value to the skybox's intensity
        RenderSettings.ambientIntensity = targetSkyboxLightIntensityMultiplier;
        // Set the final value to the skybox's reflectoin
        RenderSettings.reflectionIntensity = targetSkyboxReflectionIntensityMultiplier;
        // Set the final value to the directional light's intensity
        directionalLight.intensity = targetLightInesnity;


        // Wait until the next frame
        yield return null;


        // Check if there is a reset in progress
        if (resetInProgress)
        {
            // Increase the number of trials
            Trial++;

            // If the current Trials is the last trial (MaxTrial)
            if (Trial==MaxTrial)
            {
                // Load the next scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }

            // Reset the resetInProgress-Flag
            resetInProgress = false;
        }


        // Reset the timer
        elapsedTime = 0f;

        // Change the state of the trial
        trialState = "Trial_Start";

        // Call coroutine activateStartCanvas to activate the start canvas
        StartCoroutine(activateStartCanvas(3));


        // Untile the timer is smaller than the transition-duration
        while (elapsedTime < transitionDuration)
        {
            // Increase the timer
            elapsedTime += Time.deltaTime;

            // Gradually calculate a increased value of skybox's intensity
            float currentAmbientIntensity = Mathf.Lerp(targetSkyboxLightIntensityMultiplier, initialSkyboxLightIntensityMultiplier, elapsedTime / transitionDuration);
            // Gradually calculate a increased value of skybox's reflection
            float currentReflectionIntensity = Mathf.Lerp(targetSkyboxReflectionIntensityMultiplier, initialSkyboxReflectionIntensityMultiplier, elapsedTime / transitionDuration);
            // Gradually calculate a increased value of directional light's intensity
            float currentLightIntensity = Mathf.Lerp(targetLightInesnity, initialLightInesnity, elapsedTime / transitionDuration);

            // Assign the calculated value to the skybox's intensity
            RenderSettings.ambientIntensity = currentAmbientIntensity;
            // Assign the calculated value to the skybox's reflection
            RenderSettings.reflectionIntensity = currentReflectionIntensity;
            // Assign the calculated value to the directoinal light's intensity
            directionalLight.intensity = currentLightIntensity;


            // Wait until the next frame
            yield return null;
        }


        // Wait until the next frame
        yield return null;


        // Set the final value to the skybox's intensity
        RenderSettings.ambientIntensity = initialSkyboxLightIntensityMultiplier;
        // Set the final value to the skybox's reflectoin
        RenderSettings.reflectionIntensity = initialSkyboxReflectionIntensityMultiplier;
        // Set the final value to the directional light's intensity
        directionalLight.intensity = initialLightInesnity;


        // Change the state of the trial
        trialState = "Searching";
    }
}