using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityGenerator : MonoBehaviour
{
    static List<string> possibleNames = null;
    static Dictionary<string, Entity> allEntities = new Dictionary<string, Entity>();

    static string GetRandomName()
    {
        if (possibleNames == null)
        {
            possibleNames = PrefabLink.D.EntityNames.text.Split('\n').OfType<string>().ToList();
        }
        int selectedIndex = Random.Range(0, possibleNames.Count);
        string selectedName = possibleNames[selectedIndex];
        possibleNames.RemoveAt(selectedIndex);
        return selectedName;
    }

    public static Entity GenerateEntity(Faction faction, WorldLocation location)
    {
        Entity ent = new Entity(GetRandomName(), faction, location, Instantiate(PrefabLink.D.EntityPrefab));
        allEntities.Add(ent.entityName, ent);
        return ent;
    }

    public static Entity GetEntity(string entName)
    {
        if (allEntities.TryGetValue(entName, out Entity ent)) return ent;
        return null;
    }

    private void Start()
    {
        WorldSimulator.OnTickReset += ResetPathEncounter;
        WorldSimulator.OnTickEncounter += ProcesEncounter;
    }

    private void ProcesEncounter(int turn)
    {
        // TODO : Handle three person encounter
        // TODO : Handle pass
        // TODO : Handle cross
        foreach (var path in Entity.pathToCheck)
        {
            List<PathMovement> movements = Entity.allPathMovements[path];
            for (int i = 0; i < movements.Count; i++)
            {
                PathMovement mov = movements[i];
                for (int j = i + 1; j < movements.Count; j++)
                {
                    PathMovement movTwo = movements[j];

                    if (mov.goingToA != movTwo.goingToA)
                    {
                        CheckCrossInPath(mov, movTwo);
                    }
                    else
                    {
                        CheckPassInPath(mov, movTwo);
                    }
                }
            }
        }
    }

    private void CheckPassInPath(PathMovement mov, PathMovement movTwo)
    {
        PathMovement fastest = mov.moveValue >= movTwo.moveValue ? mov : movTwo;
        PathMovement slowest = fastest == movTwo ? mov : movTwo;

        if (fastest.pastValue <= slowest.pastValue && fastest.endValue >= slowest.endValue)
        {
            if (fastest.moveValue == slowest.moveValue)
            {
                Debug.Log($"{fastest.ent.entityName} travelled with {slowest.ent.entityName}");
            }
            else
            {
                Debug.Log($"{fastest.ent.entityName} passed beside {slowest.ent.entityName}");
            }
        }
    }

    private void CheckCrossInPath(PathMovement mov, PathMovement movTwo)
    {
        PathMovement theOneGoingToA = mov.goingToA ? mov : movTwo;
        PathMovement theOneGoingToB = mov.goingToA ? movTwo : mov;

        float A_pastValueInverted = theOneGoingToA.path.pathLength - theOneGoingToA.pastValue;
        float A_endValueInverted = theOneGoingToA.path.pathLength - theOneGoingToA.endValue;

        bool pastValueIsInZone = A_pastValueInverted > theOneGoingToB.pastValue && A_pastValueInverted <= theOneGoingToB.endValue;
        bool endValueIsInZone = A_endValueInverted > theOneGoingToB.pastValue && A_endValueInverted <= theOneGoingToB.endValue;

        if (pastValueIsInZone != endValueIsInZone)
        {
            Debug.Log($"{theOneGoingToA.ent.entityName} crossed with {theOneGoingToB.ent.entityName}");
        }
    }

    private void ResetPathEncounter(int turn)
    {
        Entity.allPathMovements = new Dictionary<WorldPathData, List<PathMovement>>();
        Entity.pathToCheck = new List<WorldPathData>();
    }
}
