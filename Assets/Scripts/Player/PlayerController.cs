using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    public float XSensitivity;
    [HideInInspector]
    private float CurrentMoveSpeed;
    public float Speed;
    public float jumpSpeed;
    private float dummySpeed;
    public float RunningSpeed;
    private float ElapsedTime = 1.3f;
    private float TimeBwtweenSwitching=1.3f;
    public float YSensitivity;
    private float XRotation = 0f;

    [HideInInspector]
    public int selectedWeapon = 0;

    public delegate void OnplayerDeath();
    public static OnplayerDeath Playerdeath;

    public TextMeshProUGUI BulletCount;
    public TextMeshProUGUI Mag;
    public TextMeshProUGUI Gun_Name;

    public Image Stamina_Bar;
    public Image HealthImage;

    private Animator anim;

    public List<GameObject> WeaponList=new List<GameObject>();

    public Transform CameraTransform;

    private Inventory inventory;
    private HealthSystems healthSystems;
    private Stamina Stamina_Ref;
    [HideInInspector]
    public WayPoint Waypoint_Ref;
    public Objectives Objectives_Ref;
    public List<Collider> DialogueSoundTriggers=new List<Collider>();


    public GameObject Cam_Holder;
    private bool canRun;
    private bool isDead = false;
    private bool isPlayed=false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        CurrentMoveSpeed = Speed;
        inventory = GetComponent<Inventory>();
        transform.GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(0, 0, 0);
        healthSystems = GetComponent<HealthSystems>();
        Stamina_Ref = GetComponent<Stamina>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      
        if(hit.gameObject.CompareTag("WayPoint"))
        {
            Destroy(hit.gameObject);
            if (Waypoint_Ref.i+1<Waypoint_Ref.Target.Count)
            {
                Waypoint_Ref.i++;
            }
            if (Objectives_Ref.i<Objectives_Ref.Objective.Count)
            {
                Objectives_Ref.i++;
            }
            else
            {
                return;
            }
        }
        for (int i = 0; i < DialogueSoundTriggers.Count; i++)
        {
            if (hit.collider.name == DialogueSoundTriggers[i].name)
            {
                FindObjectOfType<DialogAudioManager>().Play(DialogueSoundTriggers[i].name);
                DialogueSoundTriggers.Remove(DialogueSoundTriggers[i]);
                Destroy(hit.collider.gameObject);
            }
        }
        
    }
    void Update()
    {
        //Setting Health value to UI.
        HealthImage.fillAmount = healthSystems.CurrentHealth / healthSystems.MaxHealth;
        //Setting Stamina value to UI.
        Stamina_Bar.fillAmount = Stamina_Ref.Current_Stamina / Stamina_Ref.Max_Stamina;
        if (healthSystems.CurrentHealth <= 0)
        {
            Playerdeath();
            isDead = true;
            SceneChangeManager.instance.SceneChangeFunction("Lose");
        }
        if (!isDead)
        {
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
                } while (!characterController.isGrounded);
            }
            else
            {
                movedir += Physics.gravity * Time.deltaTime;
            }
            //Setting speed for animation
            var magnitude = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
            dummySpeed = magnitude;
            //Player Running
            if (Input.GetKey(KeyCode.LeftShift) && Stamina_Ref.Current_Stamina > 0)
            {
                canRun = true;
                Stamina_Ref.StopCoroutine(Stamina_Ref.Recover_Stamina(2));
                CurrentMoveSpeed = RunningSpeed;
                if (characterController.velocity.magnitude >= RunningSpeed)
                {
                    Stamina_Ref.Current_Stamina -= 20 * Time.deltaTime;
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                canRun = false;
                CurrentMoveSpeed = Speed;
            }
            if (Stamina_Ref.Current_Stamina <= Stamina_Ref.Max_Stamina && !canRun)
            {
                Stamina_Ref.StartCoroutine(Stamina_Ref.Recover_Stamina(2));
            }
            if (Stamina_Ref.Current_Stamina <= 0)
            {
                CurrentMoveSpeed = Speed;
            }
            if (!canRun)
            {
                if (dummySpeed > 0.5f)
                {
                    dummySpeed = 0.5f;
                }
            }
            //CharacterController Move 
            characterController.Move(movedir);
            //Rotation of Player
            transform.Rotate(0, Input.GetAxisRaw("MouseX") * XSensitivity * Time.deltaTime, 0);

            float Xroattion = Input.GetAxis("MouseY") * YSensitivity * Time.deltaTime;
            XRotation -= Xroattion;
            XRotation = Mathf.Clamp(XRotation, -90, 90);
            CameraTransform.localRotation = Quaternion.Euler(XRotation, 0, 0);

            #endregion

            #region Weapon Switching
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && ElapsedTime > TimeBwtweenSwitching)
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
                ElapsedTime = 0f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && ElapsedTime > TimeBwtweenSwitching)
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
                ElapsedTime = 0f;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha1) && ElapsedTime > TimeBwtweenSwitching && selectedWeapon != 0)
            {
                selectedWeapon = 0;
                HolsterWeapon();
                ElapsedTime = 0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && ElapsedTime > TimeBwtweenSwitching && Cam_Holder.GetComponent<Transform>().transform.childCount>=3 && selectedWeapon!=1)
            {
                selectedWeapon = 1;
                HolsterWeapon();
                ElapsedTime = 0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && ElapsedTime > TimeBwtweenSwitching && selectedWeapon != 2)
            {
                selectedWeapon = 2;
                HolsterWeapon();
                ElapsedTime = 0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && ElapsedTime > TimeBwtweenSwitching && selectedWeapon != 3)
            {
                selectedWeapon = 3;
                HolsterWeapon();
                ElapsedTime = 0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5) && ElapsedTime > TimeBwtweenSwitching && selectedWeapon != 4)
            {
                selectedWeapon = 4;
                HolsterWeapon();
                ElapsedTime = 0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6) && ElapsedTime > TimeBwtweenSwitching && selectedWeapon != 5)
            {
                selectedWeapon = 5;
                HolsterWeapon();
                ElapsedTime = 0f;
            }
            else
            {
                ElapsedTime += Time.deltaTime;
            }
            #endregion

            BulletCountUpdate();
        }
        else
        {
            WeaponList[selectedWeapon].SetActive(false);
            if(!isPlayed)
            {
                isPlayed = true;
                FindObjectOfType<AudioManager>().Play("Death");
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z), 2 * Time.deltaTime); ;
        }
    }
    public void AddtoInventory(Item item)
    {
        inventory.Add(item, item.Amount);
    }
    private void BulletCountUpdate()
    {
        if(WeaponList[selectedWeapon].GetComponentInChildren<Shooting>()!=null)
        {
            BulletCount.text = WeaponList[selectedWeapon].GetComponentInChildren<Shooting>().CurrentAmmo.ToString(); 
            Mag.text = WeaponList[selectedWeapon].GetComponentInChildren<Shooting>().weapons.TotalBullets.ToString();
            Gun_Name.text = WeaponList[selectedWeapon].GetComponentInChildren<Shooting>().weapons.name;
        }
        else if (WeaponList[selectedWeapon].GetComponentInChildren<Melee>() != null)
        {
            BulletCount.text = "0";
            Mag.text = "0";
            Gun_Name.text = WeaponList[selectedWeapon].GetComponentInChildren<Melee>().name;
        }
        else
        {
            BulletCount.text = "0";
            Mag.text = "0";
            Gun_Name.text = "Fist";
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
                  if (WeaponList[i].GetComponentInChildren<Shooting>() != null)
                  {
                    FindObjectOfType<AudioManager>().Play(WeaponList[i].GetComponentInChildren<Shooting>().weapons.ReadySound);
                  }
            }
            else
            {
                   WeaponList[i].SetActive(false);
            }
                   i++;
       }
    }
}
