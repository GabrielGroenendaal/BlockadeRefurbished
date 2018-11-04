using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class is for determining the behavior and movement of the "players", two snakes that move around a grid, resetting when they collide with their own body or each other
// While a lot of this code could have been more easily written, I wanted to simplify the process by storing all the necessary sprites and variables within the code itself
public class PlayerControl : MonoBehaviour {
	
	// These variables are for storing references to the player art assets necessary for the arrows, the snakes, and to identify the sprites
	public Sprite body; public Sprite up; public Sprite down; public Sprite left; public Sprite right; public Sprite empty;
	
	// This variable stores a reference to the opposing player's PlayerControl class, to raise their score if you collide
	public PlayerControl opponent;
	
	// These variables store the audio clips that will play when each character moves
	public bool musicPlay;
	public AudioSource beep;
	
	// These variables are for storing the directional inputs for each player, which are inputted through Unity
	public KeyCode upInput; public KeyCode downInput; public KeyCode rightInput; public KeyCode leftInput;

	// These variables are for other important values: the player's current score and their position on the map
	private int score = 0;
	public Text countText;
	private int Xpos;
	private int Ypos;
	
	// These variables are for storing the player inputs. Comparing inputs is necessary to prevent players from going backwards, to determine the location of the arrows
	// And to determine if a collision will take place without bugs
	private KeyCode input;
	private KeyCode previousInput;
	
	// These variables are for storing the position of the Arrow 
	private int arrowXpos;
	private int arrowYpos;
	
	// This variable stores a reference to the grid class, which is used to call on the "reset" function and access the 2D array gameboard
	public GameController grid;
	
	// This variable stores a timer before any movement takes place. This timer is shorter after the first beat.
	public float targetTime = 2.0f;
 
	// Use this for initialization. Almost all the variables are initialized in the Unity Editor.
	void Start () {
		
	}
	
	// Update is called once per frame, and is used to keep track of shifting player inputs 
	// This code manages the storing of input values and calls upon the arrow() method to place the arrow down
	void Update () {
		
		targetTime -= Time.deltaTime; // Increments the timer

		
		// This checks the input and compares it to the previous input, preventing players from going backwards, a strange way to collide.
		// If the input checks out, then it is stored, and the arrow() method is called to see if an arrow sprite can be placed in the location
		if (Input.GetKeyDown(downInput) && previousInput!=upInput)
		{
			input = downInput;
			grid.board[Xpos, Ypos].GetComponent<Image>().sprite = down;
		}
		
		// This repeats for the remaining other directional inputs
		else if (Input.GetKeyDown(upInput) && previousInput!=downInput)
		{
			input = upInput;
			grid.board[Xpos, Ypos].GetComponent<Image>().sprite = up;
		}
		else if (Input.GetKeyDown(rightInput) && previousInput!=leftInput)
		{
			input = rightInput;
			grid.board[Xpos, Ypos].GetComponent<Image>().sprite = right;
		}
		else if (Input.GetKeyDown(leftInput) && previousInput!=rightInput)
		{
			input = leftInput;
			grid.board[Xpos, Ypos].GetComponent<Image>().sprite = left;
		}
		
		// Now this code will check the inputs every .45 seconds to move the players accordingly 
		if (targetTime <= 0.0f)
		{
			targetTime = .15f; // Reset the Timer

			if (musicPlay == false)
			{
				beep.Play(0);
				beep.loop = true;
				musicPlay = true;
			}
			
			// This code is a bit complicated. First it checks the tile that a player would move to given their current stored input
			if (input == downInput)
			{
				
				// If the sprite stored on that panel is (A) not the determined 'empty' sprite or (B) the appropiate arrow sprite for the input
				// Then this indicates a collision, we first check to see that the other player will not collide.
				// If so, they resetting the board and give the opponent a point
				if (grid.board[Xpos, Ypos - 1].GetComponent<Image>().sprite != empty)
				{
					if (checkOpponent() == false)
					{
						opponent.score++; 
						opponent.setCountText(); // updates the count text.
						grid.BuildBoard();
					}
					else
					{
						grid.BuildBoard();
					}
				}
				// if neither of the above are the case, then that position is now the "head", the current position is the body, and the Xpos and Ypos variables shift
				// Previous input is set to the current input, preventing players from going directly backwards
				// And the arrow position is reset
				else
				{
					grid.board[Xpos, Ypos - 1].GetComponent<Image>().sprite = down;
					grid.board[Xpos, Ypos].GetComponent<Image>().sprite = body;
					previousInput = input;
					Ypos = Ypos - 1;
				}
			}
			
			// This repeats for every other possible input.
			else if (input == upInput)
			{
				if (grid.board[Xpos, Ypos + 1].GetComponent<Image>().sprite != empty)
				{
					if (checkOpponent() == false)
					{
						opponent.score++; 
						opponent.setCountText(); // updates the count text.
						grid.BuildBoard();
					}
					else
					{
						grid.BuildBoard();
					}
				}
				else
				{
					grid.board[Xpos, Ypos + 1].GetComponent<Image>().sprite = up;
					grid.board[Xpos, Ypos].GetComponent<Image>().sprite = body;
					previousInput = input;
					Ypos = Ypos + 1;
				}
			}
			
			else if (input == leftInput)
			{
				if (grid.board[Xpos - 1, Ypos].GetComponent<Image>().sprite != empty)
				{
					if (checkOpponent() == false)
					{
						opponent.score++; 
						opponent.setCountText(); // updates the count text.
						grid.BuildBoard();
					}
					else
					{
						grid.BuildBoard();
					}
				}
				else
				{
					grid.board[Xpos - 1, Ypos].GetComponent<Image>().sprite = left;
					grid.board[Xpos, Ypos].GetComponent<Image>().sprite = body;
					previousInput = input;
					Xpos = Xpos - 1;
				}
			}
			
			else if (input == rightInput)
			{
				if (grid.board[Xpos + 1, Ypos].GetComponent<Image>().sprite != empty)
				{
					if (checkOpponent() == false)
					{
						opponent.score++; 
						opponent.setCountText(); // updates the count text.
						grid.BuildBoard();
					}
					else
					{
						grid.BuildBoard();
					}
				}
				else
				{
					grid.board[Xpos + 1, Ypos].GetComponent<Image>().sprite = right;
					grid.board[Xpos, Ypos].GetComponent<Image>().sprite = body;
					previousInput = input;
					Xpos = Xpos + 1;
				}
			}
		}
	}
	
	// This code checks to see if the opponent's next move will ALSO result in a collision
	// If this is the case, then neither side gets a point and the board resets
	// This ensures that there is no unfair RNG points delivered
	private bool checkOpponent()
	{
		if (opponent.input == opponent.downInput)
			{
				// If the sprite stored on that panel is (A) not the determined 'empty' sprite or (B) the appropiate arrow sprite for the input
				// Then this indicates a collision, resetting the board and lowering the offending players score by 1
				if (grid.board[opponent.Xpos, opponent.Ypos - 1].GetComponent<Image>().sprite != empty)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		// This repeats for every other possible input.
		else if (opponent.input == opponent.upInput)
			{
				if (grid.board[opponent.Xpos, opponent.Ypos + 1].GetComponent<Image>().sprite != empty)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		else if (opponent.input == opponent.leftInput)
			{
				if (grid.board[opponent.Xpos - 1, opponent.Ypos].GetComponent<Image>().sprite != empty)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		else if (opponent.input == opponent.rightInput)
		{
			if (grid.board[opponent.Xpos + 1, opponent.Ypos].GetComponent<Image>().sprite != empty)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		else
		{
			return false;
		}
	}
	
	// Simple code to set the position of the Player
	public void setPosition(int x, int y)
	{
		Xpos = x;
		Ypos = y;
	}
	
	// A simple method for calling the player to change the count text
	public void setCountText()
	{
		countText.text = score.ToString();
	}
	
	// A simple method for setting the score of each player
	public void setScore(int j)
	{
		score = j;
	}
	
	// A simple method for setting the inputs for the players
	// This is used during the "reset" after each round
	public void setInput(KeyCode k)
	{
		previousInput = k;
		input = k;
	}	
	
	// A simple method for retrieving the score, used by the SceneManager to determine if the game is over
	public int getScore()
	{
		return score;
	}
}
