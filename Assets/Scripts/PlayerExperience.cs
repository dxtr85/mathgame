using UnityEngine;
using System.Collections;

public class PlayerExperience : MonoBehaviour {

	private int totalExperience;// = 0;
	private int minLevelExp;// = 0;
	private int nextLevelExp;
	private int level;// = 0; 
	private PlayerMovement playerMvt;

	// Use this for initialization
	void Start () {
		//nextLevelExp = 10000;
		playerMvt = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<PlayerMovement> ();
	}

	public void AddExperiencePionts(int points){
		totalExperience += points;
		if (totalExperience >= nextLevelExp) {
			level +=1;
			minLevelExp = nextLevelExp;
			nextLevelExp += (int)(1000 + 200*level);
			playerMvt.IncreaseMoveSpeed();
		}
	}

	public void SubstractExperiencePionts(int points){
		if (totalExperience - points >= minLevelExp) {
			totalExperience -= points;
		}
	}

	public int Level{
		get{ return level;}
		set{ level = value;}
	}
	public int NextLevelExp{
		get{ return nextLevelExp;}
		set{ nextLevelExp = value;}
	}
	public int MinLevelExp{
		get{ return minLevelExp;}
		set{ minLevelExp = value;}
	}

	public int TotalExperience{
		get{
			return totalExperience;
		}
		set { totalExperience = value;}
	}
	public int ExperiencePerc{
		get{ return (int)(100 * (totalExperience - minLevelExp) / (nextLevelExp - minLevelExp));}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
