﻿using TFramework;
using System;
using Utils;
using UnityEngine;
using LitJson;

[Serializable]
public class UserInfo {
    public string name;
    public int aid;
    public int uid;
    public int star;
    public bool isVIP;
    public int travelTimes;
    public int avatarNum;
    public int mapNum;
    public EquipInfo equipInfo = new EquipInfo();
}

public interface IUserModel : IModel {
    BindableProperty<string> Name { get; }
    BindableProperty<string> Password { get; }
    BindableProperty<int> Aid { get; }
    BindableProperty<int> Uid { get; }
    BindableProperty<int> Star { get; }
    BindableProperty<bool> IsVIP { get; }
    BindableProperty<int> TravelTimes { get; }
    BindableProperty<int> AvatarNum { get; }
    BindableProperty<int> MapNum { get; }
    BindableProperty<EquipInfo> EquipInfo { get; }
    BindableProperty<int> RememberPassword { get; set; }

    void SetUserInfoPart(UserInfo userInfo);

    void ParseUserInfo(JsonData jsonData);

    UserInfo GetUserInfoPart();
}

public class UserModel : AbstractModel, IUserModel {
    public BindableProperty<string> Name { get; set; } = new BindableProperty<string>();
    public BindableProperty<string> Password { get; set; } = new BindableProperty<string>();
    public BindableProperty<int> Aid { get; set; } = new BindableProperty<int>();
    public BindableProperty<int> Uid { get; set; } = new BindableProperty<int>();
    public BindableProperty<int> Star { get; set; } = new BindableProperty<int>();
    public BindableProperty<bool> IsVIP { get; set; } = new BindableProperty<bool>();
    public BindableProperty<int> TravelTimes { get; set; } = new BindableProperty<int>();
    public BindableProperty<int> AvatarNum { get; set; } = new BindableProperty<int>();
    public BindableProperty<int> MapNum { get; set; } = new BindableProperty<int>();
    public BindableProperty<EquipInfo> EquipInfo { get; set; } = new BindableProperty<EquipInfo>();
    public BindableProperty<int> RememberPassword { get; set; } = new BindableProperty<int>();

    public UserInfo GetUserInfoPart() {
        UserInfo userInfo = new UserInfo();
        userInfo.aid = Aid.Value;
        userInfo.avatarNum = AvatarNum.Value;
        userInfo.equipInfo = EquipInfo.Value;
        userInfo.isVIP = IsVIP;
        userInfo.mapNum = MapNum.Value;
        userInfo.name = Name.Value;
        userInfo.star = Star.Value;
        userInfo.uid = Uid.Value;
        userInfo.travelTimes = TravelTimes.Value;
        return userInfo;
    }

    public void ParseUserInfo(JsonData jsonData) {
        UserInfo userInfo = new UserInfo();
        Name.Value = (string)jsonData["user_info"]["name"];
        Uid.Value = (int)jsonData["user_info"]["uid"];
        Aid.Value = (int)jsonData["user_info"]["aid"];
        Star.Value = (int)jsonData["user_info"]["star"];
        IsVIP.Value = (bool)jsonData["user_info"]["is_vip"];
        AvatarNum.Value = (int)jsonData["user_info"]["avatar_num"];
        TravelTimes.Value = (int)jsonData["user_info"]["travel_times"];
        MapNum.Value = (int)jsonData["user_info"]["map_num"];

        JsonData data = jsonData["user_info"]["equip_info"];
        EquipInfo info = new EquipInfo();
        info.eid = (int)data["eid"];
        info.rid = (string)data["rid"];
        info.equipName = (string)data["equip_name"];
        info.star = (int)data["star"];
        info.mass = (int)data["mass"];
        info.speed = (int)data["speed"];
        info.maxSpeed = (int)data["max_speed"];
        info.isHas = (bool)data["is_has"];
        info.isShow = (bool)data["is_show"];
        EquipInfo.Value = info;
    }

    public void SetUserInfoPart(UserInfo userInfo) {
        Name.Value = userInfo.name;
        Aid.Value = userInfo.aid;
        Uid.Value = userInfo.uid;
        Star.Value = userInfo.star;
        IsVIP.Value = userInfo.isVIP;
        TravelTimes.Value = userInfo.travelTimes;
        AvatarNum.Value = userInfo.avatarNum;
        MapNum.Value = userInfo.mapNum;
        EquipInfo.Value = userInfo.equipInfo;
    }

    protected override void OnInit() {
        var storage = this.GetUtility <IPlayerPrefsStorage>();
        Name.Value = storage.LoadString(PrefKeys.userName);
        Name.Register((v) => { 
            if(PlayerPrefs.GetInt(PrefKeys.rememberPassword) == 1) {
                storage.SaveString(PrefKeys.userName, v);
            }
        });
        Password.Value = storage.LoadString(PrefKeys.password);
        Password.Register((v) => {
            if (PlayerPrefs.GetInt(PrefKeys.rememberPassword) == 1) {
                storage.SaveString(PrefKeys.password, v);
            }           
        });
        RememberPassword.Value = storage.LoadInt(PrefKeys.rememberPassword);
        RememberPassword.Register(v =>
            storage.SaveInt(PrefKeys.rememberPassword, v)
        );
    }
   
}
