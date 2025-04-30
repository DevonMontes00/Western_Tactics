using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [SerializeField] private RoundOutcomeUI roundOutcomeUI;
    [SerializeField] private UnitActionSystemUI unitActionSystemUI;
    [SerializeField] private GameObject enemyUnitPrefab;

    private enum EnemyType
    {
        BasicEnemy,
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one RoundManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ContinueButtonPressed()
    {
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        //TO:DO - Genereate Next Level Randomly

        int width = LevelGrid.Instance.GetWidth();
        int height = LevelGrid.Instance.GetHeight();
        float cellSize = 2;

        //Pathfinding.Instance.Setup(width, height, cellSize);

        List<GridPosition> allWalakableGridPositions = Pathfinding.Instance.GetAllWalkableGridPositions();

        OpenAllDoorsOnMapForPathfinding();

        List<GridPosition> allSpawnableLocations = GetAllSpawnableLocations(allWalakableGridPositions);

        CloseAllDoorsOnMapForPathfinding();

        List<EnemyType> levelEnemies = GenerateListOfLevelEnemies();

        Debug.Log(levelEnemies.Count);

        SpawnEnemies(levelEnemies, allSpawnableLocations);

        
    }

    private List<GridPosition> GetAllSpawnableLocations(List<GridPosition> allWalakableGridPositions)
    {
        List<GridPosition> allSpawnableLocations = new List<GridPosition>();
        GridPosition selectedUnitGridPosition = UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition();

        foreach (GridPosition walkableGridPosition in allWalakableGridPositions)
        {

            GridObject gridObjectAtGridPosition = LevelGrid.Instance.GetGridObject(walkableGridPosition);

            if(!Pathfinding.Instance.HasPath(selectedUnitGridPosition, walkableGridPosition))
            {
                continue;
            }

            if(gridObjectAtGridPosition.HasAnyUnit())
            {
                continue;
            }

            if (gridObjectAtGridPosition.GetInteractable() != null)
            {
                continue;
            }

            allSpawnableLocations.Add(walkableGridPosition);
        }

        return allSpawnableLocations;
    }

    private List<EnemyType> GenerateListOfLevelEnemies()
    {
        List<EnemyType> unitEnemies = new List<EnemyType>();

        System.Random rnd = new System.Random();

        int numOfEnemies = rnd.Next(1,6);

        for (int i = 0; i < numOfEnemies; i++)
        {
            EnemyType newEnemy = EnemyType.BasicEnemy;
            unitEnemies.Add(newEnemy);
        }

        return unitEnemies;

    }

    private void SpawnEnemies(List<EnemyType> levelEnemies, List<GridPosition> allSpawnableLocations)
    {
        int spawnedEnemies = 0;
        System.Random rnd = new System.Random();

        while (spawnedEnemies != levelEnemies.Count)
        {
            foreach (GridPosition gridPosition in allSpawnableLocations)
            {
                int chance = rnd.Next(1, 11); //10% chance to spawn enemy at this position

                if(chance == 1)
                {
                    Vector3 spawnPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    Instantiate(enemyUnitPrefab, spawnPosition, Quaternion.identity);

                    spawnedEnemies++;
                }

                if(spawnedEnemies == levelEnemies.Count)
                {
                    break;
                }
            }
        }
    }

    private void OpenAllDoorsOnMapForPathfinding()
    {
        List<Door> doors = LevelGrid.Instance.GetListOfAllDoors();

        foreach (Door door in doors)
        {
            door.OpenForPathfinding();
        }
    }

    private void CloseAllDoorsOnMapForPathfinding()
    {
        List<Door> doors = LevelGrid.Instance.GetListOfAllDoors();

        foreach (Door door in doors)
        {
            door.CloseForPathfinding();
        }
    }
}
