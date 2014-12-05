﻿using UnityEngine;
using System.Collections;

public class ClickToMove_lvl2 : MonoBehaviour {
	// Parameters publics
	public float speed = 30;
	public GameObject attack_range;
	public float one_atk_time = 0.30f;

	private Player_Attack_System_lvl2 atk_script;
	private CharacterController controller;
	private Music_Engine_Script music;

	//Movements variables
	private RaycastHit getObjectScene;
	private Vector3 destinationPosition;
	private Vector3 targetPoint;
	private bool moving = false;
	private float disToDestination = 0.0f;
	private Plane playerPlane;
	private Ray ray;
	private float hitdist = 0.0f;
	
	//MODIFICACIO PER MANTENIRLO SOBRE EL TERRA
	private float gravity;
	private float atk_time = -1.0f;
	private bool attacking = false;
	private float atk_cd = 1.0f;

	// Use this for initialization
	void Start () {
		atk_script = attack_range.GetComponent<Player_Attack_System_lvl2> ();
		music = GameObject.FindGameObjectWithTag("music_engine").GetComponent<Music_Engine_Script> ();
		controller = this.gameObject.GetComponent<CharacterController> ();
		animation["metarig|Caminar"].speed = 2.75f;
		animation ["metarig|Atacar"].speed = 1.25f;
		destinationPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.deltaTime != 0) {
			disToDestination = Mathf.Abs (transform.position.x - destinationPosition.x) + Mathf.Abs(destinationPosition.z - transform.position.z);

			//Si llegamos a la position del click +-0.5f de distancia para evitar que se quede corriendo
			if(disToDestination  < .5f){
				if (!isAttacking()) {
					animation.Stop ("metarig|Caminar");
					animation.Stop ("metarig|Atacar");
				}
				else animation.Play ("metarig|Atacar");
				speed = 0.0f;
			} else {
				moveToPosition (destinationPosition);
				if (!isAttacking()) animation.Play ("metarig|Caminar");
				else animation.Play ("metarig|Atacar");
				speed = 30.0f;
			}

			// Si hacemos click o dejamos presionado el botón izquierdo del mouse, nos movemos al punto del mouse
			if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetMouseButton(0)) { 			
				playerPlane = new Plane(Vector3.up, transform.position);
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				hitdist = 0.0f;
				
				if (playerPlane.Raycast(ray, out hitdist)) {
					targetPoint = ray.GetPoint(hitdist);
					destinationPosition = ray.GetPoint(hitdist);
					rotateToMouse();
				}
			}

			//Bloque para atacar
			if (!isAttacking()) { // Si no esta haciendo un ataque
				if (Input.GetKeyDown (KeyCode.Space)) { // Al puslar espacio
					// Si ha pasado mas de 1 segundo del inicio del ultimo ataque
					if (canAttack()) {
						atk_script.makeAttack();
						atk_time = Time.time;
						music.play_Player_Sword_Attack ();
					}
				}

				if (Input.GetKeyDown (KeyCode.Mouse1)) {
				
				}
			}
		}
	}
	

	public void rotateToMouse () {
		Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
		transform.rotation = targetRotation;
	}

	void moveToPosition (Vector3 position) {
		Vector3 dir = position - transform.position;
		gravity -= 9.81f * Time.deltaTime;
		if (controller.isGrounded)	gravity = 0.0f;
		dir.y = gravity;
		Vector3 movement = dir.normalized * speed * Time.deltaTime;
		if (movement.magnitude > dir.magnitude) movement = dir;
		controller.Move(movement);
	}


	public void teleport(Vector3 position) {
		destinationPosition = position;
	}

	private bool isAttacking() {
		if (Time.time - atk_time < one_atk_time) return true;
		else return false;
	}

	private bool canAttack() {
		if (Time.time - atk_time > atk_cd) return true;
		else return false;
	}
}
