using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

// This code is used to create and store a grid gameboard comprised of various UI panels while providing them gameplay functionality
// These notes will walk you through exactly how that is accomplished.

public class GameController : MonoBehaviour {
	
	// This two dimensional array stores the BASELINE version of the gameboard, with all the tiles set to 'empty' and the border tiles intact.
	// This board is stored for safekeeping for when we need to reinitialize the game during a "reset"
	public GameObject[,] board_base = new GameObject[20,16];
	
	// This multidimensional array stores the updated board that actually utilized during play. This is reset with each round. 
	public GameObject[,] board = new GameObject[20,16];
	
	// These variables store references to the two Player GameObjects, allowing it to access their public methods 
	public PlayerControl player_one;
	public PlayerControl player_two;

	// These variables store references to sprites used to initialize the game board
	public Sprite player1; 
	public Sprite player2;
	public Sprite empty;
	public AudioSource beep;
	
	// This code is to manage the Score Text
	public Text player1score;
	public Text player2score;
	
	// Use this for initialization of the game. It sets the players scores and sets up the basic board that will be used
	void Start ()
	{
		player_one.setScore(0);  
		player_two.setScore(0);
		player_one.setCountText();
		player_two.setCountText();
		BuildBaseBoard();
		BuildBoard();
	}
	
	// This timer will be to fade the text out
	public float targetTime = 2.0f;
	
	// Update is called once per frame
	// It is mostly used to set a timer on how long the score is set up
	void Update () {
		targetTime -= Time.deltaTime;
		if (targetTime <= 0.0f)
		{
			player1score.enabled = false;
			player2score.enabled = false;
		}
	}

	// This code will populate the "board_base" array that will be used to form the instantiated arrays going forward
	public void BuildBaseBoard()
	{
		int count = 1;
		
		for (int i = 0; i < 16; i++)
		{
			for (int k = 0; k < 20; k++)
			{
				GameObject g = GameObject.Find("Tile (" + count + ")");
				board_base[k, i] = g;
				count++;
			}
		}
	}
	
	// This code will populate the 20 x 16 array with the contents of the board_base array, forming the game grid
	// This code will be called at the start of the game and whenever they need to reset the game
	public void BuildBoard()
	{
		for (int i = 0; i < 16; i++)
		{
			for (int k = 0; k < 20; k++)
			{
				board[k, i] = board_base[k, i];
	
			}
		}
		
		// Just a double check to make sure everything is empty
		for (int i = 1; i < 15; i++)
		{
			for (int k = 1; k < 19; k++)
			{
				board[k, i].GetComponent<Image>().sprite = empty;
			}
		}

		player_one.beep.Stop();
		player_two.beep.Stop();
		
		// We then set the positions of the players and set their inputs to the default position
		board[4, 10].GetComponent<Image>().sprite = player1;
		board[15, 4].GetComponent<Image>().sprite = player2;
		player_one.setPosition(4, 10);
		player_two.setPosition(15, 4);
		player_one.setInput(player_one.downInput);
		player_two.setInput(player_two.upInput);
		
		// We then pass each player the updated gridobject 
		player_one.grid = this;
		player_two.grid = this;
		
		// And finally we reset all the timers for the players
		// These timers determine how long the first "beat" takes, and how long the score stays up
		player1score.enabled = true;
		player2score.enabled = true;
		player_one.targetTime = 2.0f;
		player_two.targetTime = 2.0f;
		targetTime = 2.0f;
		player_one.musicPlay = false;
		player_two.musicPlay = false;
		beep.Play(0);
	}
	
	
	
}
