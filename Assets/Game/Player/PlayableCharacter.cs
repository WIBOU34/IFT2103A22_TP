using Cinemachine;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    private InputManager inputManager;
    public int playerNumber;
    public int totalNumberOfPlayers;
    public List<GameObject> weapons;
    public Transform cinemachineUpOverrideObjectTransform;

    // Start is called before the first frame update
    void Start()
    {
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
        this.OnGameOver();
    }

    public void OnGameOver()
    {
        Debug.Log("Game Over");
    }

    public void SetupCameraTopDown()
    {
        GameObject virtualPlayerCam = this.transform.parent.Find("PlayerFollowCamera").gameObject;
        GameObject playerCameraRoot = this.transform.Find("PlayerCameraRoot").gameObject;
        GameObject camera = this.transform.parent.Find("MainCamera").gameObject;
        camera.GetComponent<CinemachineBrain>().m_WorldUpOverride = cinemachineUpOverrideObjectTransform;
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
        GameObject virtualPlayerCam = this.transform.parent.Find("PlayerFollowCamera").gameObject;
        GameObject playerCameraRoot = this.transform.Find("PlayerCameraRoot").gameObject;
        GameObject camera = this.transform.parent.Find("MainCamera").gameObject;
        camera.GetComponent<CinemachineBrain>().m_WorldUpOverride = null;
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
        //this.gameObject.GetComponent<StarterAssetsInputs>().cursorLocked = false;
        this.gameObject.GetComponent<StarterAssetsInputs>().topDownViewCamera = true;
        this.gameObject.GetComponent<StarterAssetsInputs>().LookInput(Vector2.zero);
    }

    private void Set3rdPersonSettingForMovement()
    {
        this.gameObject.GetComponent<StarterAssetsInputs>().cursorInputForLook = true;
        //this.gameObject.GetComponent<StarterAssetsInputs>().cursorLocked = false;
        this.gameObject.GetComponent<StarterAssetsInputs>().topDownViewCamera = false;
        this.gameObject.GetComponent<StarterAssetsInputs>().LookInput(Vector2.zero);
    }
}
