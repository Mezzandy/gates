using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public GameObject SceneCamera;
	public TileMap MainTileMap;

	public float Speed;
	public float JumpHeight;
	public bool Grounded;
	public bool Jumped;
	public bool Falling;
	public Vector3 PlayerGroundPoint;
	public Vector3 Velocity;
	public Vector2 Direction;
	public Vector2 CollisionDirection;
	public Vector2 BoxScale;

	public int PlayerTileX;
	public int PlayerTileY;

	public int NextTileX;
	public int NextTileY;

	public float Gravity;

	// Use this for initialization
	void Start () {

		UpdateBox();
	}

	// Update is called once per frame
	void Update () {

		GetInput ();
	}

	public void UpdateBox(){
		Vector3 p = gameObject.transform.position;
	
		PlayerTileX = (int)p.x;
		PlayerTileY = (int)p.y;
	}

	public bool Collided(){
		bool HasCollided = false;

		Vector3 PlayerPosition = gameObject.transform.position;

		int CurrentTileX = (int)(PlayerPosition.x);
		int CurrentTileY = (int)(PlayerPosition.y);

		//Find Desired Direction 
		if (Velocity.x > 0) {
			Direction.x = 1;
		} 
		else if (Velocity.x == 0) {
			Direction.x = 0;
		} 
		else {
			Direction.x = -1;
		}

		if (Velocity.y > 0) {
			Direction.y = 1;
		} 
		else if (Velocity.y == 0) {
			Direction.y = 0;
		} 
		else {
			Direction.y = -1;
		}
			
		Vector3 NewPlayerPosition = gameObject.transform.position;
		NewPlayerPosition.x += Velocity.x * Speed * Time.deltaTime;

		NextTileX = (int)(CurrentTileX + Direction.x);

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
			}
		}

		NextTileY = (int)(CurrentTileY + Direction.y);

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

		Velocity.x = Input.GetAxis("Horizontal");

		if (Jumped == false && Grounded == true) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				Jumped = true;
				Velocity.y = JumpHeight;
				Grounded = false;
			}
		} 
			
		if (Jumped == true) {
			if (Velocity.y <= 0.0f) {
				Jumped = false;
				Falling = true;
			}
			if (Collided () == true) {
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

			//position.x += h * Speed * Time.deltaTime;
			position.x += Velocity.x * Speed * Time.deltaTime;

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
