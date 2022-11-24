using Cinemachine;
using StarterAssets;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelBasicStart : MonoBehaviour
{
    public GameObject zombieTypeToSpawn;
    public ZombieController zombieController;
    public GameObject prefabPlayer;
    private List<GameObject> players;
    public List<GameObject> weapons;
    public Material bulletTrailMaterial;
    public GameObject cinemachineUpOverrideObject;
    private InputManager inputManager;
    // Start is called before the first frame update
    void Start()
    {
        cinemachineUpOverrideObject = new GameObject("CinemachineUpOverrideObject");
        cinemachineUpOverrideObject.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);

        int totalPlayers = 2;
        if (MenuManager.persistence != null)
        {
            totalPlayers = MenuManager.persistence.GetComponent<GameLoader>().totalPlayers;
        }

        CreatePlayers(totalPlayers);

        zombieController = new ZombieController();
        zombieController.Start();
        ZombieController.typeToSpawn = zombieTypeToSpawn;
        this.gameObject.AddComponent<MapCreator>();
        ZombieController.CreateZombieSpawner(new Vector3(-10, 0, 5));
        inputManager = InputManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreatePlayers(int nbrPlayers)
    {
        for (int i = 0; i < nbrPlayers; i++)
        {
            GameObject.Instantiate(prefabPlayer);
        }

        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        int playerNumber = 0;
        foreach (var player in players)
        {
            player.transform.parent.name = "Player_" + (playerNumber + 1) + "_Container";
            player.name = "Player_" + (playerNumber + 1);

            GameObject camera = player.transform.parent.Find("MainCamera").gameObject;
            camera.GetComponent<Camera>().depth = playerNumber;
            if (playerNumber > 0)
            {
                camera.GetComponent<AudioListener>().enabled = false;
            }

            SetupPlayerCameraLayerAndMask(player, playerNumber);

            PlayableCharacter playableCharacter = player.AddComponent<PlayableCharacter>();
            playableCharacter.cinemachineUpOverrideObjectTransform = cinemachineUpOverrideObject.transform;
            playableCharacter.SetupCameraTopDown();
            playableCharacter.weapons = weapons;
            playableCharacter.playerNumber = playerNumber + 1;
            playableCharacter.totalNumberOfPlayers = nbrPlayers;
            StarterAssetsInputs starterAssetsInputs = player.GetComponent<StarterAssetsInputs>();
            PauseMenuController pauseMenuController = player.AddComponent<PauseMenuController>();
            pauseMenuController.playerInput = player.GetComponent<PlayerInput>();             
            player.AddComponent<HealthBarManager>().playerNumber = playerNumber + 1;
            PlayerInputController playerInputController = player.AddComponent<PlayerInputController>();
            playerInputController.playableCharacter = playableCharacter;
            playerInputController.pauseMenuController = pauseMenuController;
            playerInputController.starterAssetsInputs = starterAssetsInputs;
            playerInputController.playerKeyCodes = InputManager.GetPlayerKeyCodes(playerNumber + 1);

            playerNumber++;
        }
    }

    //Set the layer and bitmask of the camera and its required parts for it to follow its designed player
    private void SetupPlayerCameraLayerAndMask(GameObject player, int playerNumber)
    {
        GameObject virtualPlayerCam = player.transform.parent.Find("PlayerFollowCamera").gameObject;
        GameObject playerCameraRoot = player.transform.Find("PlayerCameraRoot").gameObject;
        GameObject camera = player.transform.parent.Find("MainCamera").gameObject;

        int layer = playerNumber + 10;

        virtualPlayerCam.layer = layer;

        var bitMask = (1 << layer)
            | (1 << 0)
            | (1 << 1)
            | (1 << 2)
            | (1 << 4)
            | (1 << 5)
            | (1 << 6)
            | (1 << 7);

        camera.GetComponent<Camera>().cullingMask = bitMask;
        camera.layer = layer;
        virtualPlayerCam.layer = layer;
        playerCameraRoot.layer = layer;
    }
}
