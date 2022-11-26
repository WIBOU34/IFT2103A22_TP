using Cinemachine;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    private InputManager inputManager;
    public int playerNumber;
    public int totalNumberOfPlayers;
    public List<GameObject> weapons;
    public Transform cinemachineUpOverrideObjectTransform;
    private GameObject playerMainCamera;
    private GameObject virtualPlayerCam;
    private GameObject playerCameraRoot;

    // Start is called before the first frame update
    void Start()
    {
        playerMainCamera = this.transform.parent.Find("MainCamera").gameObject;
        virtualPlayerCam = this.transform.parent.Find("PlayerFollowCamera").gameObject;
        playerCameraRoot = this.transform.Find("PlayerCameraRoot").gameObject;

        SetupCameraTopDown();

        inputManager = InputManager.Instance;

        this.gameObject.AddComponent<Damageable>();
        this.gameObject.AddComponent<WeaponManager>().parent = this.gameObject.transform
            .Find("Skeleton/Hips/Spine/Chest/UpperChest/Right_Shoulder/Right_UpperArm/Right_LowerArm/Right_Hand").gameObject;
        for (int i = 0; i < weapons.Count; i++)
        {
            this.gameObject.GetComponent<WeaponManager>().weapons.Add(i, weapons[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnKilled()
    {
        ZombieController.PlayerKilled(this.gameObject);
    }

    public void OnGameOver()
    {
        Debug.Log("Game Over");
    }

    public void SetupCameraTopDown()
    {
        playerMainCamera.GetComponent<CinemachineBrain>().m_WorldUpOverride = cinemachineUpOverrideObjectTransform;
        CinemachineVirtualCamera cmvc = virtualPlayerCam.GetComponent<CinemachineVirtualCamera>();
        cmvc.Follow = playerCameraRoot.transform;
        cmvc.LookAt = playerCameraRoot.transform;
        CinemachineTransposer body = cmvc.AddCinemachineComponent<CinemachineTransposer>();
        body.m_FollowOffset = new Vector3(0, 10, 0);
        body.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
        virtualPlayerCam.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
        SetTopDownSettingForMovement();

    }

    public void SetupCamera3rdPerson()
    {
        playerMainCamera.GetComponent<CinemachineBrain>().m_WorldUpOverride = null;
        CinemachineVirtualCamera cmvc = virtualPlayerCam.GetComponent<CinemachineVirtualCamera>();
        cmvc.Follow = playerCameraRoot.transform;
        cmvc.LookAt = playerCameraRoot.transform;
        Cinemachine3rdPersonFollow body = cmvc.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
        body.Damping = new Vector3(0.1f, 0.25f, 0.3f);
        body.ShoulderOffset = new Vector3(2, 0, 0);
        body.VerticalArmLength = 0;
        body.CameraSide = 0.6f;
        body.CameraDistance = 4;
        body.CameraRadius = 0.15f;
        body.IgnoreTag = "Player";
        Set3rdPersonSettingForMovement();
    }

    private void SetTopDownSettingForMovement()
    {
        this.gameObject.GetComponent<StarterAssetsInputs>().cursorInputForLook = false;
        this.gameObject.GetComponent<StarterAssetsInputs>().topDownViewCamera = true;
        this.gameObject.GetComponent<StarterAssetsInputs>().LookInput(Vector2.zero);
    }

    private void Set3rdPersonSettingForMovement()
    {
        this.gameObject.GetComponent<StarterAssetsInputs>().cursorInputForLook = true;
        this.gameObject.GetComponent<StarterAssetsInputs>().topDownViewCamera = false;
        this.gameObject.GetComponent<StarterAssetsInputs>().LookInput(Vector2.zero);
    }

    public void ZoomMainCamera()
    {
        CinemachineTransposer cinemachineTransposer = virtualPlayerCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();

        if (cinemachineTransposer.m_FollowOffset.y > 10)
        {
            cinemachineTransposer.m_FollowOffset = new Vector3(cinemachineTransposer.m_FollowOffset.x, cinemachineTransposer.m_FollowOffset.y - 1, cinemachineTransposer.m_FollowOffset.z);
        }
    }

    public void UnZoomMainCamera()
    {
        CinemachineTransposer cinemachineTransposer = virtualPlayerCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        if (cinemachineTransposer.m_FollowOffset.y < 50)
        {
            cinemachineTransposer.m_FollowOffset = new Vector3(cinemachineTransposer.m_FollowOffset.x, cinemachineTransposer.m_FollowOffset.y + 1, cinemachineTransposer.m_FollowOffset.z);
        }
    }
}
