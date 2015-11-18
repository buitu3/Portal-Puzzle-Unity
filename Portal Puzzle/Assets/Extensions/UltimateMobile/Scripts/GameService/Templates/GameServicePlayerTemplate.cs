using UnityEngine;
using System.Collections;

public class GameServicePlayerTemplate  {



	private GameCenterPlayerTemplate gc_player;
	private GooglePlayerTemplate ps_player;



	
	//--------------------------------------
	// init
	//--------------------------------------

	public GameServicePlayerTemplate(GameCenterPlayerTemplate gc, GooglePlayerTemplate ps) {
		gc_player = gc;
		ps_player = ps;

	}
	
	//--------------------------------------
	// get / set
	//--------------------------------------



	public string PlayerId {
		get {
			switch(Application.platform) {

			case RuntimePlatform.Android:
				return ps_player.playerId;
			case RuntimePlatform.IPhonePlayer:
				return gc_player.PlayerId;

			}

			return string.Empty;
		}
	}


	public string Name {
		get {
			switch(Application.platform) {
				
			case RuntimePlatform.Android:
				return ps_player.name;
			case RuntimePlatform.IPhonePlayer:
				return gc_player.Alias;
			}

			return string.Empty;
		}
	}

	public Texture2D Avatar {
		get {
			switch(Application.platform) {
				
			case RuntimePlatform.Android:
				return ps_player.image;
			case RuntimePlatform.IPhonePlayer:
				return gc_player.Avatar;
			}

			return null;
		}
	}


	public GameCenterPlayerTemplate GameCenterPlayer {
		get {
			return gc_player;
		}
	}
	
	
	
	public GooglePlayerTemplate GooglePlayPlayer {
		get {
			return ps_player;
		}
	}


	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------




} 
