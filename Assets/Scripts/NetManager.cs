using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetManager : Singleton<NetManager>
{
    public string nickname;
    public string address;
    public GameObject playerPrefab;

    public List<GameObject> enemyPrefabs;
    Dictionary<int, NetPlayer> players = new Dictionary<int, NetPlayer>();
    Dictionary<int, NetEnemy> entities = new Dictionary<int, NetEnemy>();
    protected NetManager() { }

    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Demo")
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)NetMessage.Nickname);
                var s = Encoding.UTF8.GetBytes(nickname);
                bw.Write(s.Length);
                bw.Write(s);
                s = Encoding.UTF8.GetBytes(address);
                bw.Write(s.Length);
                bw.Write(s);
                Networking.Instance.SendReliable(ms.ToArray());
            }
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.GetPlayers));
                Networking.Instance.SendReliable(ms.ToArray());
            }
            /*using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.GetEnemies));
                Networking.Instance.SendReliable(ms.ToArray());
            }*/
        }
    }

    void Update()
    {
        while (Networking.Instance.queue.Count > 0)
        {
            Message msg;
            if (Networking.Instance.queue.TryDequeue(out msg))
            {
                using (MemoryStream ms = new MemoryStream(msg.buffer, 0, msg.length))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    var netMessage = (NetMessage)(br.ReadInt32());

                    switch (netMessage)
                    {
                        case NetMessage.Nickname:
                            {
                                int id = br.ReadInt32();
                                int count = br.ReadInt32();
                                var nickname = Encoding.UTF8.GetString(br.ReadBytes(count));
                                players[id].Nickname = nickname;
                            }
                            break;
                        case NetMessage.GetPlayers:
                            {
                                int count = br.ReadInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    int id = br.ReadInt32();
                                    if (!players.ContainsKey(id))
                                    {
                                        int ncount = br.ReadInt32();
                                        string nickname = Encoding.UTF8.GetString(br.ReadBytes(ncount));
                                        Vector3 position = br.ReadVector3();
                                        SpawnPlayer(id, nickname, position);
                                    }
                                }
                                using (MemoryStream ms2 = new MemoryStream())
                                using (BinaryWriter bw = new BinaryWriter(ms2))
                                {
                                    bw.Write((int)(NetMessage.GetEnemies));
                                    Networking.Instance.SendReliable(ms2.ToArray());
                                }
                            }
                            break;
                        case NetMessage.PlayerConnected:
                            {
                                int id = br.ReadInt32();
                                SpawnPlayer(id, "", Vector3.zero);
                                Debug.Log("Player connected");
                            }
                            break;
                        case NetMessage.PlayerDisconnected:
                            {
                                int id = br.ReadInt32();
                                Destroy(players[id].gameObject);
                                players.Remove(id);
                            }
                            break;
                        case NetMessage.UpdatePosition:
                            {
                                int id = br.ReadInt32();
                                if (players.ContainsKey(id))
                                {
                                    Vector3 position = br.ReadVector3();
                                    players[id].UpdatePosition(position);
                                }
                            }
                            break;
                        case NetMessage.SpawnEnemy:
                            {
                                int id = br.ReadInt32();
                                int gameID = br.ReadInt32();
                                SpawnEnemy(id, gameID, Vector3.zero);
                            }
                            break;
                        case NetMessage.EnemyDied:
                            {
                                int id = br.ReadInt32();
                                Destroy(entities[id].gameObject);
                                entities.Remove(id);
                            }
                            break;
                        case NetMessage.GetEnemies:
                            {
                                int count = br.ReadInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    int id = br.ReadInt32();
                                    if (!entities.ContainsKey(id))
                                    {
                                        int gameID = br.ReadInt32();
                                        Vector3 position = br.ReadVector3();
                                        SpawnEnemy(id, gameID, position);
                                    }
                                }
                            }
                            break;         
                        case NetMessage.UpdateEnemyPosition:
                            {
                                int id = br.ReadInt32();
                                if (entities.ContainsKey(id)){
                                    Vector3 position = br.ReadVector3();
                                    entities[id].UpdatePosition(position);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    void SpawnPlayer(int id, string nickname, Vector3 position)
    {
        var go = Instantiate(playerPrefab, position, Quaternion.identity) as GameObject;
        var netPlayer = go.GetComponent<NetPlayer>();
        netPlayer.id = id;
        netPlayer.Nickname = nickname;
        players.Add(id, netPlayer);
    }
    void SpawnEnemy(int id, int gameID, Vector3 position){
        var go = Instantiate(enemyPrefabs[gameID], position, Quaternion.identity) as GameObject;
        var netEnemy = go.GetComponent<NetEnemy>();
        netEnemy.id = id;
        entities.Add(id, netEnemy);
    }
}
