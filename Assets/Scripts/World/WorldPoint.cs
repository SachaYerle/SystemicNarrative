using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WorldPoint : WorldLocation
{
    // --- VISUAL
    public TextMeshPro nameText = null;
    public TextMeshPro factionText = null;
    public TextMeshPro nbInText = null;
    public SpriteRenderer mainSpriteRender = null;
    public GameObject houseRoot = null;

    public Faction faction { get; protected set; }

    [HideInInspector] public List<WorldPathData> paths = new List<WorldPathData>();
    [HideInInspector] public List<WorldPathData> validPaths = new List<WorldPathData>();


    private void Start()
    {
        WorldSimulator.OnTick += DoTick;
    }

    private void OnDestroy()
    {
        WorldSimulator.OnTick -= DoTick;
    }

    public void DoTick(int tick)
    {

    }

    public void InitWorldLocation(string locationName)
    {
        gameObject.name = locationName;
        nameText.text = locationName;

        InitPopulation();

    }

    private void Update()
    {
        nbInText.text = EntitiesIn.Count.ToString();
    }


    private void InitPopulation()
    {
        // --- POPULATION
        SetFaction(FactionGenerator.GenerateFaction());
        int nbEntity = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < nbEntity; i++)
        {
            Entity entity = EntityGenerator.GenerateEntity(faction, this);
        }
    }

    private void SetFaction(Faction faction)
    {
        this.faction = faction;
        if (faction != null)
        {
            factionText.text = faction.factionName;
        }
        else
        {
            factionText.text = "";
        }
    }
}
