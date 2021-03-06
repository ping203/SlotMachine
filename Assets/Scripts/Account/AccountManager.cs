using UnityEngine;
using System;
using System.Collections;
using Boomlagoon.JSON;
using Sfs2X.Entities.Data;

public class AccountManager : MonoBehaviour {
  
  public static AccountManager Instance { get; private set; }
	
  private int randomNumb = new System.Random().Next();
	
	private string mUsername = string.Empty;
	private string mPassword = string.Empty;
	private string mDisplayName = string.Empty;
	private long mCash = 0;
	private int mGem = 0;
	private int mBossKilled = 0;
	private bool mIsGuest = false;
	
	public long lastClaimedDaily = 0;
	public long lastReadInboxTime = 0;
	public long lastInboxTime = 0;
	
	// facebook data
	public string fbId = string.Empty;
	public string avatarLink = string.Empty;
	public string email = string.Empty;
	// private JSONArray mFriends;
	
	public string username {
	  get { return mUsername; }
	  set { mUsername = value; }
	}

	public string password {
	  get { return mPassword; }
	  set { mPassword = value; }
	}
	
	public string displayName {
	  get { return mDisplayName; }
	  set { mDisplayName = value; }
	}
	
	public long cash {
	  get { return mCash ^ randomNumb; }
	  set { mCash = value ^ randomNumb; }
	}
	
	public int gem {
	  get { return mGem ^ randomNumb; }
	  set { mGem = value ^ randomNumb; }
	}
	
	public int bossKilled {
	  get { return mBossKilled ^ randomNumb; }
	  set { mBossKilled = value ^ randomNumb; }
	}
	
	public bool isGuest {
	  get { return mIsGuest; }
	  set { mIsGuest = value; }
	}
	
	// public JSONArray friends {
	//   get { return mFriends; }
	//   set { mFriends = value; }
	// }
	
	void Awake() {
		Instance = this;
	}

	public string GetUserId() {
		return "Guest";
	}
	
	public void SetUser(JSONObject user) {
	  username = user.GetString("username");
		password = user.GetString("password");
		displayName = user.GetString("displayName");
		cash = user.GetLong("cash");
		gem = user.GetInt("gem");
		bossKilled = user.GetInt("bossKill");
		lastClaimedDaily = user.GetLong("lastDaily");
		fbId = user.GetString("facebookId");
		avatarLink = user.GetString("avatar");
		lastReadInboxTime = user.GetLong("lastReadInboxTime");
		lastInboxTime = user.GetLong("lastInboxTime");
		Debug.Log("SetUser " + user.ToString());
		// friends = user.GetArray("friends");
	}
	
	public void LogOut() {
	  username = string.Empty;
		password = string.Empty;
		displayName = string.Empty;
		cash = 0;
		gem = 0;
		bossKilled = 0;
		lastClaimedDaily = 0;
		lastReadInboxTime = 0;
		lastInboxTime = 0;
		fbId = string.Empty;
		avatarLink = string.Empty;
	}
	
	public void UpdateUserCash(long updateVal) {
	  cash = Math.Max(0, cash + updateVal);
	}
	
	public void UpdateUserGem(int updateVal) {
	  gem = Math.Max(0, gem + updateVal);
	}
	
	public bool IsYou(string username) {
	  return username == this.username;
	}
	
	public bool IsFriend(string username) {
		return SmartfoxClient.Instance.IsContainBuddy(username);
	}
	
	private bool CanAddFriend(string username) {
		int count = SmartfoxClient.Instance.GetBuddyList().Count;
	  return count < Global.MAX_FRIEND_NUMB;
	}
	
	public void AddFriend(string username) {
	  if (CanAddFriend(username)) {
	    UserExtensionRequest.Instance.AddFriend(username);
	  } else {
      HUDManager.Instance.AddFlyText(Localization.Get(ErrorCode.USER.MAX_FRIENDS.ToString()), Vector3.zero, 40, Color.red, 0, 4f);
	  }
	}
	
	public void Register(JSONObject jsonData) {
	  SmartfoxClient.Instance.Register(jsonData);
	}
	
	public void RegisterAsGuest() {
	  SmartfoxClient.Instance.RegisterAsGuest();
	}
		
	// Login or register using facebook
	public void LoginUsingFB(string mFBId, string mDisplayName, string mAvatarLink, string mEmail) {
		SmartfoxClient.Instance.LoginUsingFB(mFBId, mDisplayName, mAvatarLink, mEmail);
	}
	
  // void HandleErrorCode(ErrorCode.USER errorCode) {
  //   Debug.Log("HandleErrorCode " + errorCode);
  // }
  // 
  // private ISFSObject CreateExtensionObject(JSONObject jsonData) {
  //     ISFSObject objOut = new SFSObject();
  //     objOut.PutByteArray("jsonData", Utils.ToByteArray(jsonData.ToString()));
  //     return objOut;
  // }
  // 
  // private ServerRequest CreateExtensionRequest(string commandId, string successCallback, JSONObject jsonData = null) {
  //  if (jsonData == null) {
  //    jsonData = new JSONObject();
  //  }
  // 
  //  ISFSObject requestData = CreateExtensionObject(jsonData);
  //  ServerRequest serverRequest = new ServerRequest(ServerRequest.Type.EXTENSION,
  //                          Command.Create(GameId.USER, commandId),
  //                          requestData,
  //                          gameObject,
  //                          successCallback);
  //  return serverRequest;
  // }
}