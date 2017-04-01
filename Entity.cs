using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public GameObject SceneCamera;
	public TileMap MainTileMap;

	public float Speed;
	public float CurrentSpeed;
	public float MaxSpeed;
	public float Acceleration;
	public float JumpHeight;		//Need to add acceleration to jump, avoid this and use another value
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

	public GameObject DebugQuad;

	// Use this for initialization
	void Start () {

		UpdateCurrentTile();
	}

	// Update is called once per frame
	void Update () {

		//GetInput ();

		//Find Current Tile/Position
		UpdateCurrentTile();

		//Get Input
		GetInput();

		//Check For Collision
		CheckForCollision();

		//Update Position
		UpdatePosition();
	}

	public void UpdatePosition(){

		Vector2 NewPlayerPosition = gameObject.transform.position;

		if (CollisionDirection.x == 0) {
			CurrentSpeed = GetCurrentSpeed ();
			NewPlayerPosition.x += CurrentSpeed * Time.deltaTime;
		}

		if (CollisionDirection.y == 0) {
			if (Jumped == true) {
				if (Velocity.y <= 0.0f) {
					Jumped = false;
					Falling = true;
				} else {
					float newVelocity = Velocity.y + (Gravity * Time.deltaTime);
					Velocity.y = newVelocity;
					NewPlayerPosition.y += Velocity.y * Time.deltaTime;
				}
			} 

			if (Grounded == false && Jumped == false) {		//Falling
				Falling = true;
				Velocity.y += Gravity * Time.deltaTime;
				NewPlayerPosition.y += Velocity.y * Time.deltaTime;

//				if ((NewPlayerPosition.y - BoxScale.y) < PlayerTileY) {
//					NewPlayerPosition.y = BoxScale.y + PlayerTileY;
//					Grounded = true;
//				}
			}
		} 
		else if (CollisionDirection.y == -1) {
			Grounded = true;
			Falling = false;
			Jumped = false;
			if ((NewPlayerPosition.y - BoxScale.y) < PlayerTileY) {
				NewPlayerPosition.y = BoxScale.y + PlayerTileY;
				//Grounded = true;
			}
		}
	
		gameObject.transform.position = NewPlayerPosition;

	}

	public void GetInput(){

		//Player Control
		Velocity.x = Input.GetAxis("Horizontal");

		if(Jumped == false && Grounded == true){
			if(Input.GetKeyDown(KeyCode.Space)){
				Jumped = true;
				Grounded = false;
				Velocity.y = JumpHeight;
			}
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

	public void CheckForCollision(){
		Vector3 PlayerPosition = gameObject.transform.position;

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

		//Get potential new speed
		float NewSpeed = GetCurrentSpeed();

		//Check X Axis
		//Move to potential position
		PlayerPosition.x += NewSpeed * Time.deltaTime;

		//Get Potential Bounding Box
		NextTileX = PlayerTileX + (int)(Direction.x);

		//X Axis
		if (MainTileMap.CollisionMap [PlayerTileY * 20 + NextTileX] == 1) {
			//Next Tile
			float cRight = NextTileX + 1;
			float cLeft = NextTileX;
			float cTop =  PlayerTileY+ 1;
			float cBottom = PlayerTileY;

			//Player
			float pRight = PlayerPosition.x + BoxScale.x;
			float pLeft = PlayerPosition.x - BoxScale.x;
			float pTop = PlayerPosition.y + BoxScale.y;
			float pBottom = PlayerPosition.y - BoxScale.y;

			if (pLeft < cRight) {
				CollisionDirection.x = -1;
			} 
			else if (pRight > cLeft) {
				CollisionDirection.x = 1;
			} 
			else {
				CollisionDirection.x = 0;
			}
		}

		//Check Y Axis
		//Move to potential position
		float newVelocity = Velocity.y - (Gravity * Time.deltaTime);
		PlayerPosition.y += Velocity.y * Time.deltaTime;

		NextTileY = PlayerTileY + (int)(Direction.y);

		//Y Axis
		if (MainTileMap.CollisionMap [NextTileY * 20 + PlayerTileX] == 1) {
			//Next Tile
			float cRight = PlayerTileX + 1;
			float cLeft = PlayerTileX;
			float cTop = NextTileY + 1;
			float cBottom = NextTileY;

			//Player
			float pRight = PlayerPosition.x + BoxScale.x;
			float pLeft = PlayerPosition.x - BoxScale.x;
			float pTop = PlayerPosition.y + BoxScale.y;
			float pBottom = PlayerPosition.y - BoxScale.y;

			if (pBottom < cTop) {
				CollisionDirection.y = -1;
				Grounded = true;
				Falling = false;
				Jumped = false;
				Velocity.y = 0.0f;
			} 
			else if (pTop > cBottom) {
				CollisionDirection.y = 1;
				Falling = true;
				Jumped = false;
				Grounded = false;
			} 
			else {
				CollisionDirection.y = 0;
			}
		}	

		Vector3 dPos = DebugQuad.transform.position;
		dPos.x = NextTileX + 0.5f;
		dPos.y = NextTileY + 0.5f;
		DebugQuad.transform.position = dPos;
	}

	public float GetCurrentSpeed(){
		float NewSpeed = 0.0f;

		NewSpeed = Velocity.x * Acceleration * MaxSpeed + (1.0f - Acceleration) * (CurrentSpeed);

		if (NewSpeed > 0.0f) {
			if (NewSpeed > MaxSpeed) {
				NewSpeed = MaxSpeed;
			}
		}

		if (NewSpeed < 0.0f) {
			if (-NewSpeed < -MaxSpeed) {
				NewSpeed = -MaxSpeed;
			}
		}

		if (NewSpeed < 0.1f && NewSpeed > -0.1f) {
			NewSpeed = 0.0f;
		}

		return NewSpeed;
	}

	public void UpdateCurrentTile(){
		Vector3 p = gameObject.transform.position;

		PlayerTileX = (int)p.x;
		PlayerTileY = (int)p.y;
	}
}