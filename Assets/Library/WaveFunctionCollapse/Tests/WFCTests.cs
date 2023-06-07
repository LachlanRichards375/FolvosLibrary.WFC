using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class WFCTests
{
    protected WFCManager_2D manager;
    protected BeachImporter importer;
    protected BeachWFCExporter exporter;
    protected IWFCPosition size = new IWFCPosition(3, 3);
    public WFCTile[] GetDomain()
    {
        WFCTile[] Domain = new WFCTile[3]{
            WFCTile.CreateTile(new TileData("Grass", 1), new WFCRule[1]{
                new MultiCellIsNotTarget2D()
            }),
            WFCTile.CreateTile(new TileData("Sand", 1), new WFCRule[2]{
                new MultiCellIsNotTarget2D(),
                new MultiCellIsNotTarget2D(),
            }),
            WFCTile.CreateTile(new TileData("Water", 1), new WFCRule[1]{
                new MultiCellIsNotTarget2D()
            }),
        };

        //Grass
        //Doesn't want water Anywhere
        MultiCellIsNotTarget2D currentRule = (Domain[0].Rules[0] as MultiCellIsNotTarget2D);
        currentRule.goal = Domain[2];
        SetAllFlags(ref currentRule.direction);

        //Sand
        //Doesn't want water west
        currentRule = (Domain[1].Rules[0] as MultiCellIsNotTarget2D);
        currentRule.goal = Domain[2];
        SetFlag(ref currentRule.direction, CellDirection.Direction.West);

        //Sand
        //Doesn't want grass east
        currentRule = (Domain[1].Rules[1] as MultiCellIsNotTarget2D);
        currentRule.goal = Domain[2];
        SetFlag(ref currentRule.direction, CellDirection.Direction.East);

        //Water
        //Doesn't want Grass anywhere
        currentRule = (Domain[2].Rules[0] as MultiCellIsNotTarget2D);
        currentRule.goal = Domain[0];
        SetAllFlags(ref currentRule.direction);

        return Domain;
    }

    public void SetAllFlags(ref CellDirection.Direction flags)
    {
        SetFlag(ref flags, CellDirection.Direction.North);
        SetFlag(ref flags, CellDirection.Direction.NorthEast);
        SetFlag(ref flags, CellDirection.Direction.East);
        SetFlag(ref flags, CellDirection.Direction.SouthEast);
        SetFlag(ref flags, CellDirection.Direction.South);
        SetFlag(ref flags, CellDirection.Direction.SouthWest);
        SetFlag(ref flags, CellDirection.Direction.West);
        SetFlag(ref flags, CellDirection.Direction.NorthWest);
    }

    public void SetFlag(ref CellDirection.Direction flags, CellDirection.Direction direction)
    {
        FlagsHelper.Set<CellDirection.Direction>(ref flags, direction);
    }

    public WFCManager_2D GetManager()
    {
        WFCManager_2D manager = (WFCManager_2D)ScriptableObject.CreateInstance(typeof(WFCManager_2D));
        
        if (importer is null)
        {
            importer = (BeachImporter)ScriptableObject.CreateInstance(typeof(BeachImporter));
        }

        if (exporter is null)
        {
            exporter = (BeachWFCExporter)ScriptableObject.CreateInstance(typeof(BeachWFCExporter));
        }

        importer.returner = GetDomain();

        manager.SetImporter(importer);
        manager.SetExporter(exporter);
        manager.SetSize(size);
        manager.Initialize();

        return manager;
    }

    public WFCTile GetGrass(){
        if(importer is null){
            Debug.LogError("Importer was not initialized idiot");
            return null;
        }

        return importer.returner[0];
    }

    public WFCTile GetSand(){
        if(importer is null){
            Debug.LogError("Importer was not initialized idiot");
            return null;
        }

        return importer.returner[1];
    }

    public WFCTile GetWater(){
        if(importer is null){
            Debug.LogError("Importer was not initialized idiot");
            return null;
        }

        return importer.returner[2];
    }
}
