using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;

    public float XSensitivity;
    [HideInInspector]
    float CurrentMoveSpeed;
    public float Speed;
    public float jumpSpeed;
    float dummySpeed;
    public float RunningSpeed;
    public float YSensitivity;
    float XRotation = 0f;
    float ElapsedTime=0f, FixedTime = 5f;

    public int selectedWeapon = 0;

    public delegate void OnplayerDeath();
    public static OnplayerDeath Playerdeath;

    public Image HealthImage;

    Animator anim;

    public List<GameObject> WeaponList=new List<GameObject>();

    public Transform CameraTransform;

    public TextMeshProUGUI BulletCount;

    Inventory inventory;
    HealthSystems healthSystems;
    bool canRun;

    Color FadeColor;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        CurrentMoveSpeed = Speed;
        inventory = GetComponent<Inventory>();
        transform.GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(0, 0, 0);
        healthSystems = GetComponent<HealthSystems>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Weapon")|| hit.gameObject.CompareTag("Medkits"))
        {
            var item = hit.gameObject.GetComponent<ItemScript>();
            inventory.Add(item.item, item.item.Amount);
            Destroy(hit.gameObject);
        }
        if(hit.gameObject.CompareTag("Ammo"))
        {
            var item = hit.gameObject.GetComponent<ItemScript>();
            inventory.Add(item.item, item.item.Amount);
            Destroy(hit.gameObject);
        }
    }
    void Update()
    {
        HealthImage.fillAmount = healthSystems.CurrentHealth / healthSystems.MaxHealth;
        if (healthSystems.CurrentHealth<=0)
        {
            Playerdeath();
        }
        #region Movements
        //Inputs
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        //Movement Vectors
        Vector3 move_horizontal = transform.right * horizontal;
        Vector3 move_z = transform.forward * vertical;
        Vector3 movedir = (move_horizontal + move_z).normalized * CurrentMoveSpeed * Time.deltaTime;
        //Gravity
     //   movedir.y = -7f * Time.deltaTime;
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            var _jump = jumpSpeed;
            do
            {
                movedir.y = _jump;
                _jump -= Time.deltaTime;
            }   while (!characterController.isGrounded);
        }
        else
        {
            movedir += Physics.gravity * 1 * Time.deltaTime;
        }
        //Setting speed for animation
        var magnitude = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
        dummySpeed = magnitude;
        //Player Running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            canRun = true;
            CurrentMoveSpeed = RunningSpeed;
        }
        else
        {
            canRun = false;
            CurrentMoveSpeed = Speed;
        }

        //CharacterController Move 
        characterController.Move(movedir);
        //Rotation of Player
        if (!canRun)
        {
            if (dummySpeed > 0.5f)
            {
                dummySpeed = 0.5f;
            }
        }
        transform.Rotate(0, Input.GetAxisRaw("MouseX") * XSensitivity * Time.deltaTime, 0);

        float Xroattion = Input.GetAxis("MouseY") * YSensitivity * Time.deltaTime;
        XRotation -= Xroattion;
        XRotation = Mathf.Clamp(XRotation, -90, 90);
        CameraTransform.localRotation = Quaternion.Euler(XRotation, 0, 0);

        #endregion
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
                if (selectedWeapon >= WeaponList.Count - 1)
                {
                    selectedWeapon = 0;
                }
                else
                {
                    selectedWeapon++;
                }
            HolsterWeapon();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
          if (selectedWeapon <= 0)
          {
             selectedWeapon = WeaponList.Count - 1;
          }
          else
          {
             selectedWeapon--;
          }
            HolsterWeapon();
        }
        BulletCountUpdate();
    }

    private void BulletCountUpdate()
    {
        if(WeaponList[selectedWeapon].GetComponentInChildren<Shooting>()!=null)
        {
            BulletCount.text = WeaponList[selectedWeapon].GetComponentInChildren<Shooting>().CurrentAmmo.ToString() + " / " + WeaponList[selectedWeapon].GetComponentInChildren<Shooting>().weapons.TotalBullets.ToString();
        }
        else
        {
            BulletCount.text = "0 / 0";
        }
    }
    public void Spawn(GameObject obj)
    {
        var go = Instantiate(obj as GameObject);
        var spawnpoint = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().localPosition + (GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().forward * 10);
        spawnpoint.y += 1000;
        var ray = new Ray(spawnpoint, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            spawnpoint.y = hit.point.y + go.transform.localScale.y * 0.5f;
        }
        go.transform.position = spawnpoint;
    }
    private void HolsterWeapon()
    {
        int i = 0;
        foreach (var item in WeaponList)
        {
            if (i == selectedWeapon)
            {
                WeaponList[i].SetActive(true);
                WeaponList[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                anim = WeaponList[i].GetComponent<Animator>();
            }
            else
            {
                WeaponList[i].SetActive(false);
            }
            i++;
        }
    }
}
