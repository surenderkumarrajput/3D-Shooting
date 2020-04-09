using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunsway : MonoBehaviour 
{
	public Transform root;


	public float speed = 3f;
	float x;
	float y;
	private float oldY;
	private float oldX;


	void Start () 
	{
		
		oldX = root.transform.eulerAngles.x;
		oldY = root.transform.eulerAngles.y;
	}


	void Update () 
	{
		//x = root.transform.localRotation.x - oldrotation.x;

		//x *= root.transform.localRotation.w * rotationAmount;

		x = -(Mathf.DeltaAngle(root.transform.eulerAngles.x, oldX) /100f);
		y = -(Mathf.DeltaAngle(root.transform.eulerAngles.y, oldY) /150f);

		Quaternion temp = new Quaternion (x,y,transform.localRotation.z,transform.localRotation.w);
		transform.localRotation = Quaternion.Slerp(transform.localRotation,temp,Time.deltaTime * speed );



		oldY = root.transform.eulerAngles.y;
		oldX = root.transform.eulerAngles.x;

	}




}
