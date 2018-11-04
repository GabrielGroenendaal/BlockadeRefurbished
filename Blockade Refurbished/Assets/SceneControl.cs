using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Application = UnityEngine.Application;

public class SceneControl : MonoBehaviour {
	
	// These variables store references to the player GameObjects, and acts to detect a Lose State when either reaches 0 score
	public PlayerControl player1;
	public PlayerControl player2;
	
	// Variables that store assets to be used in the game over screen
	private static string winningPlayerName = "";
	private static string losingPlayerName = "";
	private static Sprite winningPlayer;
	private static Sprite losingPlayer;
	public Image winningPlayerHead;
	public Image winningPlayerHead2;
	public Image losingPlayerHead;
	public Text playerVictory;
	public Text playerLoss;

	// Use this for initialization
	void Start ()
	{
		
	}
    

	// Update is called once per frame
	void Update () {
		// Determines what scene it is, and if the appropiate input is made, moves to the next Scene
        
		Scene m_scene = SceneManager.GetActiveScene();
        
		// Checks for the WinCondition to be fufilled in the PlayerController Script
		if (m_scene.name == "MainMenu")
		{
			//GameObject player = GameObject.Find("player");
			//PlayerController p = player.GetComponent<PlayerController>();
        
        
			//if (Input.GetKeyDown(KeyCode.Space) && p.WinState)
			if (Input.GetKeyDown(KeyCode.Space))
			{
				SceneManager.LoadScene(1);
			}
		}
                
		// This code checks the game every frame to during the Game scene to see if the lose condition has been activated.
		// If it has, it store the name and sprite of the winning / losing player and change to the game over screen
		else if (m_scene.name == "Game" && player2.getScore()==5)
		{
			winningPlayerName = "Player 1";
			losingPlayerName = "Player 2";
			winningPlayer = player2.happy;
			losingPlayer = player1.unhappy;
			SceneManager.LoadScene(2);
		}
        
		else if (m_scene.name == "Game" && player1.getScore()==5)
		{
			winningPlayerName = "Player 2";
			losingPlayerName = "Player 1";
			winningPlayer = player1.happy;
			losingPlayer = player2.unhappy;
			SceneManager.LoadScene(2);      
		}
                
		// This code stores a few situational conditions in the game over screen, altering the images depending on who won. 
		else if (m_scene.name == "GameOver")
		{
			// This code sets the images properly for the game over screen
			winningPlayerHead.sprite = winningPlayer; 
			winningPlayerHead2.sprite = winningPlayer;
			losingPlayerHead.sprite = losingPlayer;
			
			// This code sets the text and color of the game over screen middle
			playerVictory.text = winningPlayerName;
			playerLoss.text = losingPlayerName;
			
			if (winningPlayerName == "Player 1")
			{
				playerVictory.color = Color.magenta;
				playerLoss.color = Color.yellow;	
			}
			else
			{
				playerVictory.color = Color.yellow;
				playerLoss.color = Color.magenta;
			}
			
			// Finally, this last piece of coat allows us to return to the main menu
			if (Input.GetKeyDown(KeyCode.Space))
			{
				SceneManager.LoadScene(0);
			}
		}
        
		// This gives the game a "quit" button by pressing Escape. If it is pressed outside of main menu, it returns you to main menu
		if (Input.GetKey("escape"))
		{
			if (m_scene.name == "MainMenu")
			{
				Application.Quit();
			}
			
			else
			{
				SceneManager.LoadScene(0);
			}
		}
	}
}
