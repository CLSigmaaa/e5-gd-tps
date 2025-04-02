using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    [Header("Configuration des Zombies")]
    [SerializeField] private GameObject normalZombiePrefab;
    [SerializeField] private GameObject mutantZombiePrefab;
    [SerializeField] private GameObject shooterZombiePrefab;

    [Header("Configuration des Manches")]
    [SerializeField] private int baseZombiesPerWave = 5;
    [SerializeField] private float zombieIncreasePerWave = 1.5f;
    [SerializeField] private float timeBetweenWaves = 10f;

    [Header("Points d'Apparition")]
    [SerializeField] private Transform defaultZombieSpawnPointsParent;
    [SerializeField] private Transform mutantZombieSpawnPointsParent;
    [SerializeField] private Transform shooterZombieSpawnPointsParent;
    [SerializeField] private Transform itemSpawnPointsParent;

    [Header("Interface Utilisateur")]
    [SerializeField] private Text waveText;

    private List<Transform> defaultZombieSpawnPoints = new List<Transform>();
    private List<Transform> mutantZombieSpawnPoints = new List<Transform>();
    private List<Transform> shooterZombieSpawnPoints = new List<Transform>();
    private List<Transform> itemSpawnPoints = new List<Transform>();

    private List<GameObject> activeZombies = new List<GameObject>();
    private int currentWave = 0;
    private float waveTimer = 0f;

    void Start()
    {
        InitializeSpawnPoints();
        StartNextWave();
    }

    private void Update()
    {
        waveTimer += Time.deltaTime;
        if (waveTimer >= timeBetweenWaves && activeZombies.Count == 0)
        {
            StartNextWave();
        }
    }

    private void InitializeSpawnPoints()
    {
        defaultZombieSpawnPoints.AddRange(defaultZombieSpawnPointsParent.GetComponentsInChildren<Transform>());
        mutantZombieSpawnPoints.AddRange(mutantZombieSpawnPointsParent.GetComponentsInChildren<Transform>());
        shooterZombieSpawnPoints.AddRange(shooterZombieSpawnPointsParent.GetComponentsInChildren<Transform>());
        itemSpawnPoints.AddRange(itemSpawnPointsParent.GetComponentsInChildren<Transform>());

        // Remove the parent transforms from the lists
        defaultZombieSpawnPoints.Remove(defaultZombieSpawnPointsParent);
        mutantZombieSpawnPoints.Remove(mutantZombieSpawnPointsParent);
        shooterZombieSpawnPoints.Remove(shooterZombieSpawnPointsParent);
        itemSpawnPoints.Remove(itemSpawnPointsParent);
    }

    private void StartNextWave()
    {
        currentWave++;
        waveText.text = currentWave.ToString();
        waveTimer = 0f;
        int zombiesToSpawn = Mathf.RoundToInt(baseZombiesPerWave + (currentWave - 1) * zombieIncreasePerWave);
        StartCoroutine(SpawnWave(zombiesToSpawn));
    }

    private IEnumerator SpawnWave(int zombiesToSpawn)
    {
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(1f); // Attendre 1 seconde entre chaque apparition de zombie
        }

        // Apparition des objets
        //SpawnItem(ammunitionPrefab);
        //SpawnItem(medkitPrefab);
    }

    private void SpawnZombie()
    {
        GameObject zombiePrefab = GetRandomZombiePrefab();
        List<Transform> spawnPoints = GetSpawnPointsForZombie(zombiePrefab);

        if (spawnPoints.Count > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
            activeZombies.Add(zombie);
        }
        else
        {
            Debug.LogWarning("No spawn points available for " + zombiePrefab.name);
        }
    }

    private GameObject GetRandomZombiePrefab()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                return normalZombiePrefab;
            case 1:
                return mutantZombiePrefab;
            case 2:
                return shooterZombiePrefab;
            default:
                return normalZombiePrefab;
        }
    }

    private List<Transform> GetSpawnPointsForZombie(GameObject zombiePrefab)
    {
        if (zombiePrefab == normalZombiePrefab)
        {
            return defaultZombieSpawnPoints;
        }
        else if (zombiePrefab == mutantZombiePrefab)
        {
            return mutantZombieSpawnPoints;
        }
        else if (zombiePrefab == shooterZombiePrefab)
        {
            return shooterZombieSpawnPoints;
        }
        return defaultZombieSpawnPoints;
    }

    private void SpawnItem(GameObject itemPrefab)
    {
        if (itemSpawnPoints.Count > 0)
        {
            Transform spawnPoint = itemSpawnPoints[Random.Range(0, itemSpawnPoints.Count)];
            Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("No spawn points available for items");
        }
    }

    public void OnZombieKilled(GameObject zombie)
    {
        Debug.Log(zombie.name + " has been killed!");
        activeZombies.Remove(zombie);
    }
}
