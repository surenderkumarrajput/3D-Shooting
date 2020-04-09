using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    
    public float XSensitivity;
    [HideInInspector]
    float CurrentMoveSpeed;
    [HideInInspector]
    public float Speed;
    public float jumpSpeed;
    float dummySpeed;
    [HideInInspector]
    public float RunningSpeed;
    public float YSensitivity;
    float XRotation = 0f;

    int selectedWeapon = 4;

    public Transform Trigger;

    Animator anim;

    public GameObject[] Weapons;

    Inventory inventory;

    bool canRun;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        CurrentMoveSpeed = Speed;
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        inventory = GetComponent<Inventory>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Finish"))
        {
            var item = hit.gameObject.GetComponent<ItemScript>();
            inventory.Add(item.item,1);
            Destroy(hit.gameObject);
        }
    }
    void Update()
    {
        #region PlayerMovements
        //Inputs
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical   = Input.GetAxisRaw("Vertical");
        //Movement Vectors
        Vector3 move_horizontal = transform.right * horizontal;
        Vector3 move_z = transform.forward * vertical;
        Vector3 movedir = (move_horizontal + move_z).normalized*CurrentMoveSpeed*Time.deltaTime;
        //Gravity
        movedir.y = -9.8f * Time.deltaTime;
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            movedir.y = jumpSpeed;
        }
       
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
        transform.Rotate(0,Input.GetAxis("MouseX")* XSensitivity* Time.deltaTime,0);

        #endregion

        #region CameraRotation
        float Xroattion = Input.GetAxis("MouseY") * YSensitivity * Time.deltaTime;
        XRotation -= Xroattion;
        Xroattion = Mathf.Clamp(XRotation, -90, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(Xroattion, 0, 0);
        #endregion

        #region Shooting
        if (Input.GetMouseButtonDown(0))
        {
           StartCoroutine(Shooting(10));
        }
        IEnumerator Shooting(float Range)
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            Ray ray = new Ray(Trigger.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider);
            }
            anim.ResetTrigger("Attack");
        }
        #endregion

        #region Weapons
        if(Input.GetAxis("Mouse ScrollWheel")>0f)
        {
            if(selectedWeapon>=Weapons.Length-1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = Weapons.Length - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }
        HolsterWeapon();
        #endregion

        #region Animations
        dummySpeed = characterController.velocity.magnitude;
        if (!canRun)
        {
            if (dummySpeed > 0.5f)
            {
                dummySpeed = 0.5f;
            }
        }
        anim.SetFloat("Speed", dummySpeed);
        #endregion
    }
    public void HolsterWeapon()
    {
        int i = 0;
        foreach (var item in Weapons)
        {
            if(i==selectedWeapon)
            {
                Weapons[i].SetActive(true);
                anim = Weapons[i].GetComponent<Animator>();
            }
            else
            {
                Weapons[i].SetActive(false);
            }
            i++;
        }
    }
}
