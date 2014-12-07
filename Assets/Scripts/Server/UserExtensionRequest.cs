﻿using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using Sfs2X.Entities.Data;

public class UserExtensionRequest : MonoBehaviour {

  public static UserExtensionRequest Instance { get; private set; }

  void Awake() {
    Instance = this;
  }
  
  public void LoadLeaderboardData(LeaderboardScreen.Tab selectedTab) {
    JSONObject data = new JSONObject();
    data.Add("type", (int)selectedTab);
    SmartfoxClient.Instance.HandleServerRequest(CreateExtensionRequest(Command.USER.LOAD_LEADERBOARD, "LoadLeaderboardDataSuccess", data));
  }
  
  void LoadLeaderboardDataSuccess(JSONObject data) {
    LeaderboardScreen.Tab selectedTab = (LeaderboardScreen.Tab)data.GetInt("type");
    LeaderboardScreen.SetData( data.GetArray("users"), selectedTab);
    Debug.Log("LoadLeaderboardDataSuccess " + data.ToString());
    if (ScreenManager.Instance.LeaderboardScreen != null) {
      ScreenManager.Instance.LeaderboardScreen.ShowTopPlayer(selectedTab);
    }
    PopupManager.Instance.CloseLoadingPopup();
  }
  
  public void LoadUserInfo(string username) {
    JSONObject data = new JSONObject();
    data.Add("username", username);
    SmartfoxClient.Instance.HandleServerRequest(CreateExtensionRequest(Command.USER.LOAD_USER_INFO, "LoadUserInfoSuccess", data));
  }
  
  void LoadUserInfoSuccess(JSONObject data) {
    PopupManager.Instance.HideLoadingIndicator();
    JSONObject user = data.GetObject("user");
		if (user.ContainsKey("errorCode")) {
	    ErrorCode.USER errorCode = (ErrorCode.USER)user.GetInt("errorCode");
      HUDManager.Instance.AddFlyText(errorCode.ToString(), Vector3.zero, 40, Color.red);
		} else {
	    if (PopupManager.Instance.PopupUserInfo != null) {
	      PopupManager.Instance.PopupUserInfo.DisplayUserInfo(user);
	    } else {
	      PopupUserInfo.SetUser(user);
	    }
		}
  }
  
  public void AddFriend(string fUsername) {
    JSONObject data = new JSONObject();
    data.Add("username", AccountManager.Instance.username);
    data.Add("fUsername", fUsername);
    SmartfoxClient.Instance.HandleServerRequest(CreateExtensionRequest(Command.USER.ADD_FRIEND, "AddFriendSuccess", data));
  }
  
  void AddFriendSuccess(JSONObject data) {
    ErrorCode.USER errorCode = (ErrorCode.USER)data.GetInt("errorCode");
    if (errorCode == ErrorCode.USER.NULL) {
      string fUsername = data.GetString("fUsername");
      // AccountManager.Instance.friends.Add(fUsername);
      if (PopupManager.Instance.PopupUserInfo != null) {
        PopupManager.Instance.PopupUserInfo.AddFriendSuccess(fUsername);
      }
    } else {
      HUDManager.Instance.AddFlyText(errorCode.ToString(), Vector3.zero, 40, Color.red);
    }
  }
  
  public void InviteToGame(JSONArray inviteUsernames, BaseSlotMachineScreen.GameType gameType, string roomName) {
	  JSONObject jsonData = new JSONObject();
		jsonData.Add("gameType", SlotMachineClient.GetCommandByGameType(gameType));
		jsonData.Add("message", AccountManager.Instance.displayName + " invite you to play " + SlotMachineClient.GetCommandByGameType(gameType) + " with him.");
		jsonData.Add("roomName", roomName);
		jsonData.Add("invitees", inviteUsernames);
    SmartfoxClient.Instance.HandleServerRequest(CreateExtensionRequest(Command.USER.INVITE_TO_GAME, "InviteToGameSuccess", jsonData));
  }
	
	void InviteToGameSuccess(JSONObject data) {
		
	}
	
  void HandleErrorCode(ErrorCode.USER errorCode) {
    PopupManager.Instance.CloseLoadingPopup();
    PopupManager.Instance.HideLoadingIndicator();
    
    Debug.Log("HandleErrorCode " + errorCode);
  }
  
  private ISFSObject CreateExtensionObject(JSONObject jsonData) {
      ISFSObject objOut = new SFSObject();
      objOut.PutByteArray("jsonData", Utils.ToByteArray(jsonData.ToString()));
      return objOut;
  }
  
  private ServerRequest CreateExtensionRequest(string commandId, string successCallback, JSONObject jsonData = null) {
   if (jsonData == null) {
     jsonData = new JSONObject();
   }
  
   ISFSObject requestData = CreateExtensionObject(jsonData);
   ServerRequest serverRequest = new ServerRequest(ServerRequest.Type.EXTENSION,
                           Command.Create(GameId.USER, commandId),
                           requestData,
                           gameObject,
                           successCallback);
   return serverRequest;
  }
}
