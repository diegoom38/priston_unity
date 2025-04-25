using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobSpawn : MonoBehaviourPunCallbacks
{
    public List<GameObject> Mobs; // Lista de prefabs dos mobs
    public float spawnInterval = 5f;
    public int maxMobs = 10;
    public float spawnRadius = 5f; // Novo campo para definir o raio da área de spawn

    private float timer;
    private List<GameObject> activeMobs = new List<GameObject>();

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval && activeMobs.Count < maxMobs)
        {
            SpawnMob();
            timer = 0f;
        }

        activeMobs.RemoveAll(mob => mob == null);
    }

    private void SpawnMob()
    {
        if (Mobs.Count == 0) return;

        GameObject mobPrefab = Mobs[Random.Range(0, Mobs.Count)];
        string prefabPath = $"Monsters/{mobPrefab.name}";

        // Gera um ponto aleatório no plano XZ dentro do raio definido
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        GameObject mobInstance = PhotonNetwork.Instantiate(prefabPath, spawnPosition, Quaternion.identity);
        mobInstance.name = mobPrefab.name;
        mobInstance.transform.SetParent(transform);

        activeMobs.Add(mobInstance);
    }
}
