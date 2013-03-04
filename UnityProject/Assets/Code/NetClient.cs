using UnityEngine;
using System.Collections;
using Empire.Core;
using Illisian.UnityUtil.Logging;
using Empire.Core.Messages;
using System.Collections.Generic;
using System;
using System.Threading;
using Illisian.UnityUtil;

public class NetClient : MonoBehaviour
{
    public GameObject CurrentPlayer;
    Guid _myid;
    Vector3 _myPostion;
    Quaternion _myRotation;

    Dictionary<Guid, GameObject> _players = new Dictionary<Guid, GameObject>();



    Client _client;

    // Use this for initialization
    void Start()
    {
        _myid = Guid.NewGuid();
        _myPostion = CurrentPlayer.transform.position;
        _myRotation = CurrentPlayer.transform.rotation;


        LogManager.Context.LogEvent += Context_LogEvent;
        _client = new Client();
        Log.Info("Initialising the Network Client");

        _client.OnPlayerConnect += _client_OnPlayerConnect;
        _client.OnPlayerDisconnect += _client_OnPlayerDisconnect;
        _client.OnPlayerMove += _client_OnPlayerMove; 
        
        _client.Initialise();
        
        var pos = ConvertVector3ToVec3(_myPostion);
        var rot = ConvertQuaternionToQuat(_myRotation);
        _client.SendMessage(new PlayerConnect() { id = _myid, Position = pos, Rotation = rot });
       
    }

    void _client_OnPlayerConnect(PlayerConnect msg)
    {
        Log.Info("_client_OnPlayerConnect");

        if (msg.id != _myid && !_players.ContainsKey(msg.id))
        {
            var playerClone = (GameObject)Instantiate(Resources.Load("BaseMale/Prefabs/baseMale"));

            // GameObject playerClone = (GameObject)Instantiate(CurrentPlayer, transform.position, transform.rotation);
            _players.Add(msg.id, playerClone);
            _client_OnPlayerMove(msg as PlayerMove);
        }
    }
    void _client_OnPlayerDisconnect(PlayerDisconnect msg)
    {
        Log.Info("_client_OnPlayerDisconnect");

        if (_players.ContainsKey(msg.id))
        {
            Destroy(_players[msg.id]);
            _players.Remove(msg.id);
        }
    }

    void _client_OnPlayerMove(PlayerMove msg)
    {
        
        if (msg.id != _myid)
        {
            if (_players.ContainsKey(msg.id))
            {
                Log.Info("_client_OnPlayerMove");
                if (msg.Position != null)
                {
                    _players[msg.id].transform.position.Set(msg.Position.x, msg.Position.y, msg.Position.z);
                }
                if (msg.Rotation != null)
                {
                    _players[msg.id].transform.rotation.Set(msg.Rotation.x, msg.Rotation.y, msg.Rotation.z, msg.Rotation.w);
                }
            }
            else
            {
                _client_OnPlayerConnect(new PlayerConnect() { id = msg.id, Position = msg.Position, Rotation = msg.Rotation });
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        _client.ProcessMessage();
        CheckMove();
    }
    private void CheckMove()
    {
        PlayerMove _move = new PlayerMove()
        {
            id = _myid
        };
        if (Vector3.Distance(CurrentPlayer.transform.position, _myPostion) != 0)
        {
            _move.Position = ConvertVector3ToVec3(CurrentPlayer.transform.position);
            _myPostion = CurrentPlayer.transform.position;
        }
        if (Quaternion.Angle(CurrentPlayer.transform.rotation, _myRotation) != 0)
        {
            _move.Rotation = ConvertQuaternionToQuat(CurrentPlayer.transform.rotation);
            _myRotation = CurrentPlayer.transform.rotation;

        }
        if (_move.Position != null || _move.Rotation != null)
        {
            _client.SendMessage(_move);
        }
    }



    void OnDestroy()
    {
        _client.SendMessage(new PlayerDisconnect() { id = _myid });
        _client.Shutdown();
    }
    void Context_LogEvent(Illisian.UnityUtil.Logging.LogType type, string message, params object[] objects)
    {
        switch (type)
        {
            case Illisian.UnityUtil.Logging.LogType.Information:
                Debug.Log(string.Format("[INFO] {0}", message));
                break;
            case Illisian.UnityUtil.Logging.LogType.Warning:
                Debug.LogWarning(string.Format("[WARN] {0}", message));
                break;
            case Illisian.UnityUtil.Logging.LogType.Critical:
                Debug.LogWarning(string.Format("[CRITICAL] {0}", message));
                break;
            case Illisian.UnityUtil.Logging.LogType.Debug:
                Debug.Log(string.Format("[DEBUG] {0}", message));
                break;
        }
    }

    public Vec3 ConvertVector3ToVec3(Vector3 pos)
    {
        return new Vec3() { x = pos.x, y = pos.y, z = pos.z };
    }
    public Quat ConvertQuaternionToQuat(Quaternion quat)
    {
        return new Quat() { w = quat.w, x = quat.x, y = quat.y, z = quat.z };
    }
}
