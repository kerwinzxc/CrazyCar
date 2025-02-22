﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class PlayerStateMsg {
    public int cid;
    public UserInfo userInfo = new UserInfo();
    public Vector3 pos = new Vector3();
    public Vector3 speed;
    public long timestamp;
}

public interface IPlayerManagerSystem : ISystem {
    MPlayer SelfPlayer { get; set; }
    MPlayer GetPlayerByUid(int uid);
    Dictionary<int, MPlayer> peers { get; set; }
    void RespondAction(PlayerStateMsg playerStateMsg);
    void RemovePlayer(int uid);
}

public class PlayerManagerSystem : AbstractSystem, IPlayerManagerSystem {
    public Dictionary<int, MPlayer> peers { get; set; } = new Dictionary<int, MPlayer>();

    public MPlayer SelfPlayer { get; set; }

    public MPlayer GetPlayerByUid(int uid) {
        if (uid == SelfPlayer.userInfo.uid) {
            return SelfPlayer;
        }

        MPlayer player;
        if (peers.TryGetValue(uid, out player)) {
            return player;
        }

        return null;
    }

    public void RespondAction(PlayerStateMsg playerStateMsg) {
        if (playerStateMsg.userInfo.uid == this.GetModel<IUserModel>().Uid.Value) {
            this.GetSystem<IPlayerManagerSystem>().SelfPlayer.ConfirmStatus(playerStateMsg);
        } else {
            AdjustPeerPlayer(playerStateMsg);
        }
    }

    private void AdjustPeerPlayer(PlayerStateMsg playerStateMsg) {
        MPlayer peer = null;
        if (!this.GetSystem<IPlayerManagerSystem>().peers.TryGetValue(playerStateMsg.userInfo.uid, out peer)) {
            this.SendEvent(new MakeNewPlayerEvent(playerStateMsg));
        } else {
            if (playerStateMsg.timestamp > peer.lastRecvStatusStamp) {
                peer.AdjustPlayerPosition(playerStateMsg.pos, playerStateMsg.speed);
            }
        }
    }

    public void RemovePlayer(int uid) {
        if (uid == SelfPlayer.userInfo.uid) {
            return;
        }

        MPlayer b = null;
        if (!peers.TryGetValue(uid, out b)) {
            return;
        }

        peers.Remove(uid);
        b.DestroySelf();
    }

    protected override void OnInit() {
        
    }
}
