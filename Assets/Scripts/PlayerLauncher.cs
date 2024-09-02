using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerLauncher : MonoBehaviour
{
    [SerializeField]
    private Transform playerStartPosition; // Reference to the starting position of the player object
    public Player player; // Reference to the player object (assuming 'Player' is a custom class)
    private Vector3 launchDirection;
    private bool holdingPlayer; // Boolean flag to check if the player is currently being held
    private Camera cam; // Reference to the main camera
    public GameObject dotPrefab; //for creating trajectory
    private List<GameObject> dots = new List<GameObject>();  // pool of dots
    public int numberOfDots; //no. of dots in trajectory
    public static PlayerLauncher Instance; // Singleton instance for easy access to this script

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Initialize the Singleton instance
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get and cache the main camera reference
        cam = Camera.main;
        for(int i = 0; i < numberOfDots; i++)
        {
            GameObject dot = Instantiate(dotPrefab, playerStartPosition.position, Quaternion.identity);
            dot.SetActive(false);
            dots.Add(dot);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If there's no player assigned, exit the update loop
        if (player == null)
        {
            return;
        }

        // Check if the input is detected and the player hasn't been launched yet
        if (InputDown() && player.isLaunched == false)
        {
            Vector3 touchWorldPosition;

            // Determine the position of the touch or mouse in world coordinates
            if (Input.touchCount > 0)
            {

                touchWorldPosition = cam.ScreenToWorldPoint(Input.touches[0].position);
            }
            else
            {
                touchWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            // Set the Z-axis to 0 because we are working in 2D space
            touchWorldPosition.z = 0;

            // If the touch/mouse position is close enough to the player, set holdingPlayer to true
            if (Vector3.Distance(touchWorldPosition, player.transform.position) <= 3.0f)
            {
                holdingPlayer = true;
            }
        }

        // If the input is released and the player is being held, launch the player
        if (InputUp() && holdingPlayer == true)
        {
            holdingPlayer = false; // Stop holding the player

            // Launch the player in the direction opposite to the start position
            launchDirection = playerStartPosition.position - player.transform.position;
            player.Launch(launchDirection);
            player.isLaunched = true; // Mark the player as launched
        }

        // If the player is being held and hasn't been launched, update the player's position
        if (holdingPlayer && player.isLaunched == false)
        {
            Vector3 newPos;

            // Get the new position from the touch or mouse input
            if (Input.touchCount > 0)
            {
                newPos = cam.ScreenToWorldPoint(Input.touches[0].position);
            }
            else
            {
                newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            // Set the Z-axis to 0 to keep the player in the 2D plane
            newPos.z = 0;
            player.transform.position = newPos; // Update the player's position

            //Calculate launch position for trajectory
            launchDirection = playerStartPosition.position - player.transform.position;
            ShowTrajectory(launchDirection);
        }

        if(player.isLaunched == true)
        {
            HideTrajectory();
        }
    }

    // Method to detect if input (touch or mouse) is pressed down
    bool InputDown()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
           
            return true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }

    // Method to detect if input (touch or mouse) is released
    bool InputUp()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
            return true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            return true;
        }
        else { return false; }
    }

    // Method to set a new player object in the game
    public void SetNewPlayer(GameObject playerPrefab)
    {
        player = Instantiate(playerPrefab, playerStartPosition.position, Quaternion.identity).GetComponent<Player>();
        CameraController.instance.SetPlayer(player);

    }


    private void ShowTrajectory(Vector3 launchDirection)
    {
        Vector3 startingPosition = player.transform.position;
        Vector3 startingVelocity = launchDirection * 5f;
        Vector3 gravity = (Vector3)Physics2D.gravity;

        for (int i = 0; i < numberOfDots; i++)
        {
            float time = i * 0.1f;
            Vector3 position = startingPosition + (startingVelocity * time) + 0.5f * gravity * time * time;
            position.z = 0;

            dots[i].SetActive(true);
            dots[i].transform.position = position;
        }
    }

    private void HideTrajectory()
    {
        foreach (GameObject dot in dots)
        {
            dot.SetActive(false);
        }
    }
}