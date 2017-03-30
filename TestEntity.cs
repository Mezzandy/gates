using UnityEngine;
using System.Collections;

public class TestEntity : MonoBehaviour {

	public GameObject SceneCamera;
	public TileMap MainTileMap;

	public bool CollisionDetected;
	public float Speed;
	public float JumpHeight;
	public bool Grounded;
	public bool Jumped;
	public bool Falling;
	public Vector3 PlayerGroundPoint;
	public Vector3 Velocity;
	public int layer;
	public Vector2 BoxScale;
	public Vector2 BoxBR;
	public Vector2 BoxBL;
	public Vector2 BoxTR;
	public Vector2 BoxTL;

	public int PlayerTileX;
	public int PlayerTileY;

	public int NextTileX;
	public int NextTileY;

	public float Gravity;

	// Use this for initialization
	void Start () {

		//SceneCamera.transform.SetParent (gameObject.transform);
		UpdateBox();
	}

	// Update is called once per frame
	void Update () {

		GetInput ();
	}

	public void UpdateBox(){
		Vector3 p = gameObject.transform.position;
		BoxBR = new Vector2 (p.x + BoxScale.x, p.y - BoxScale.y);
		BoxBL = new Vector2 (p.x - BoxScale.x, p.y - BoxScale.y);
		BoxTR = new Vector2 (p.x + BoxScale.x, p.y + BoxScale.y);
		BoxTL = new Vector2 (p.x - BoxScale.x, p.y + BoxScale.y);

		PlayerTileX = (int)p.x;
		PlayerTileY = (int)p.y;
	}

	public bool Collided(){
		bool HasCollided = false;
		CollisionDetected = false;

		Vector3 PlayerPosition = gameObject.transform.position;

		int CurrentTileX = (int)(PlayerPosition.x);
		int CurrentTileY = (int)(PlayerPosition.y);

		int nx = 0;
		int ny = 0;

		if (Velocity.x > 0) {
			nx = 1;
		} 
		else if (Velocity.x == 0) {
			nx = 0;
		} 
		else {
			nx = -1;
		}

		if (Velocity.y > 0) {
			ny = 1;
		} 
		else if (Velocity.y == 0) {
			ny = 0;
		} 
		else {
			ny = -1;
		}

		//Vector3 NewPlayerPosition = new Vector3 (PlayerPosition.x + hDirection * Speed * Time.deltaTime, PlayerPosition.y + vDirection * Speed * Time.deltaTime, 0.0f);
		//Vector3 NewPlayerPosition = PlayerPosition;
		Vector3 NewPlayerPosition = gameObject.transform.position;
		NewPlayerPosition.x += Velocity.x * Speed * Time.deltaTime;

		 NextTileX = CurrentTileX + nx;

		//X Axis
		if (MainTileMap.CollisionMap [CurrentTileY * 20 + NextTileX] == 1) {
			float cRight = NextTileX + 1;
			float cLeft = NextTileX;
			float cTop = CurrentTileY + 1;
			float cBottom = CurrentTileY;

			float pRight = NewPlayerPosition.x + BoxScale.x;
			float pLeft = NewPlayerPosition.x - BoxScale.x;
			float pTop = NewPlayerPosition.y + BoxScale.y;
			float pBottom = NewPlayerPosition.y - BoxScale.y;

			if (pLeft > cRight || pRight < cLeft || pTop < cBottom || pBottom > cTop) {
				HasCollided = false;
			} else {
				HasCollided = true;
				CollisionDetected = true;
			}
		}

		NextTileY = CurrentTileY + ny;
		float newVelocity = Velocity.y - (Gravity * Time.deltaTime);
		NewPlayerPosition.y += Velocity.y * Time.deltaTime;

		//Y Axis
		if (MainTileMap.CollisionMap [NextTileY * 20 + CurrentTileX] == 1) {
			float cRight = CurrentTileX + 1;
			float cLeft = CurrentTileX;
			float cTop = NextTileY + 1;
			float cBottom = NextTileY;

			float pRight = NewPlayerPosition.x + BoxScale.x;
			float pLeft = NewPlayerPosition.x - BoxScale.x;
			float pTop = NewPlayerPosition.y + BoxScale.y;
			float pBottom = NewPlayerPosition.y - BoxScale.y;

			if (pLeft > cRight || pRight < cLeft || pTop < cBottom || pBottom > cTop) {
				HasCollided = false;
			} else {
				HasCollided = true;
				CollisionDetected = true;
			}
		}

		return HasCollided;
	}

	public void CheckIfGrounded(){
		//Check for ground
		Vector3 PlayerPosition = gameObject.transform.position;

		int CurrentTileX = (int)(PlayerPosition.x);
		int CurrentTileY = (int)(PlayerPosition.y);

		if (MainTileMap.CollisionMap [(CurrentTileY - 1) * 20 + CurrentTileX] == 1) {
			float cRight = CurrentTileX + 1;
			float cLeft = CurrentTileX;
			float cTop = (CurrentTileY);
			float cBottom = (CurrentTileY - 1);

			float pRight = PlayerPosition.x + BoxScale.x;
			float pLeft = PlayerPosition.x - BoxScale.x;
			float pTop = PlayerPosition.y + BoxScale.y;
			float pBottom = PlayerPosition.y - BoxScale.y;

			if (pLeft > cRight || pRight < cLeft || pTop < cBottom || pBottom > cTop) {
				Grounded = false;
			} else {
				Grounded = true;
				CollisionDetected = true;
				Velocity.y = 0.0f;
				Falling = false;
				if ((PlayerPosition.y-BoxScale.y) < (float)CurrentTileY) {
					PlayerPosition.y = BoxScale.y + (float)CurrentTileY;
				}
			}
		} 
		else {
			Grounded = false;
		}
	}

	public void GetInput(){

		float h = Input.GetAxis ("Horizontal");
		//float v = Input.GetAxis ("Vertical");


		if (Jumped == false && Grounded == true) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				Jumped = true;
				Velocity.y = JumpHeight;
				Grounded = false;
			}
		} 

		//CheckIfGrounded ();

		if (Jumped == true) {
			if (Velocity.y <= 0.0f) {
				Jumped = false;
				Falling = true;
			}
		} 

		if (Jumped == false) {
			CheckIfGrounded ();
		}

		//If no collision, move character
		if (Collided () == false) {
			Vector3 position = gameObject.transform.position;

			position.x += h * Speed * Time.deltaTime;

			if (Grounded == false && Jumped == false) {		//Falling
				//position.y += Gravity * Time.deltaTime;
				Falling = true;
				Velocity.y += Gravity * Time.deltaTime;
				position.y += Velocity.y * Time.deltaTime;
			}

			if (Jumped == true) {
				float newVelocity = Velocity.y + (Gravity * Time.deltaTime);
				Velocity.y = newVelocity;
				position.y += Velocity.y * Time.deltaTime;
			}


			Vector3 camerapos = SceneCamera.transform.position;
			camerapos.x = position.x;
			SceneCamera.transform.position = camerapos;

			gameObject.transform.position = position;


			UpdateBox ();
		} 
		else {
			Vector3 position = gameObject.transform.position;

			if (Grounded == false && Jumped == false) {		//Falling
				//float newVelocity = Velocity.y + (Gravity * Time.deltaTime);
				Falling = true;
				Velocity.y += Gravity * Time.deltaTime;
				position.y += Velocity.y * Time.deltaTime;
				//position.y += Gravity * Time.deltaTime;
				if ((position.y-BoxScale.y) < (float)PlayerTileY) {
					position.y = (float)PlayerTileY + BoxScale.y;
				}
			}
				
			if (Jumped == true) {
				float newVelocity = Velocity.y + (Gravity * Time.deltaTime);
				Velocity.y = newVelocity;
				position.y += Velocity.y * Time.deltaTime;
			}



			Vector3 camerapos = SceneCamera.transform.position;
			camerapos.x = position.x;
			SceneCamera.transform.position = camerapos;

			gameObject.transform.position = position;

			UpdateBox ();
		}


			
		//Camera Control
		if (Input.GetKey (KeyCode.LeftBracket)) {
			float s = SceneCamera.GetComponent<Camera> ().orthographicSize;
			s -= 1.0f * Time.deltaTime;
			SceneCamera.GetComponent<Camera> ().orthographicSize = s;
		}

		if (Input.GetKey (KeyCode.RightBracket)) {
			float s = SceneCamera.GetComponent<Camera> ().orthographicSize;
			s += 1.0f * Time.deltaTime;
			SceneCamera.GetComponent<Camera> ().orthographicSize = s;
		}
			
	}


}
