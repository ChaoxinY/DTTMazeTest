using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

#region RecursiveBacktracking
public static class RecursiveBacktrackingDirections
{
	//Basic 2D directions.
	//Multiplying with 2 instead of 1 is because the wallcells themselves also take up a spot in the list.
	//Using Dictionary to increase readability when accesing the values.
	public static readonly Dictionary<string, Vector2> directions = new Dictionary<string, Vector2>() { { "Up", Vector2.up*2 },{ "Down",Vector2.down*2},{ "Left", Vector2.left*2 },{ "Right", Vector2.right*2} };
}

public struct BaseCell
{
	public BaseCell(Vector2 position)
	{
		Position = position;
	}
	public Vector2 Position { get; }
}

public class RecursiveBacktrackingCalculationUnit
{
	public BaseCell BaseCell { get; private set; }
	//Use to backtrack the direction and find the wall between this unit and the unit that
	//reached this unit.
	public Vector2 ThroughWhatDirectionWasIFound;
	public bool isVisited;
	//Whether the unit should be spawned or not.
	public bool isWall;
	//No need to do the math and trying to find a neighbour that either doenst exsist or 
	//is on a wrong spot. Just check for the bool.
	private bool[] availableNeighbourDirections = new bool[] { true, true, true, true };

	public bool[] AvailableNeighbourDirections
	{
		get
		{
			return availableNeighbourDirections;
		}
	}

	public RecursiveBacktrackingCalculationUnit(BaseCell baseCell,int mazeWidth, int mazeHeight)
	{
		BaseCell = baseCell;
		isVisited = false;
		if(!isWall)
		{
			AddAvailableNeighbourDirection(mazeWidth, mazeHeight);
		}
	}

	private void AddAvailableNeighbourDirection(int mazeWidth, int mazeHeight)
	{
		int i = 0;
		foreach(KeyValuePair<string, Vector2> direction in RecursiveBacktrackingDirections.directions)
		{
			//Up
			if(i == 1 && BaseCell.Position.y == 0)
			{
				availableNeighbourDirections[i] = false;
			}
			//Down
			else if(i == 0 && BaseCell.Position.y == mazeHeight - 1)
			{
				availableNeighbourDirections[i] = false;
			}
			//Left
			else if(i == 2 && BaseCell.Position.x == 0)
			{
				availableNeighbourDirections[i] = false;
			}
			//Right
			else if(i == 3 && BaseCell.Position.x == mazeWidth - 1)
			{
				availableNeighbourDirections[i] = false;
			}
			i++;
		}
	}
}

//Most algorithms exsist out of huge chuncks of code. Thats why I use "partial" to allow adding different algorthims through different scripts. Increasing readability while other classes can still access these algorithms through the same class. 
public static partial class MazeCalculatingAlgorithms
{
	//using await Task.Delay to prevent algortihm freezing the whole programme when the calculation takes to long to process in a single frame.
	public async static Task<List<Vector2Int>> CalculateRecursiveBacktrackingMaze(Vector2Int mazeDimensions)
	{
		List<RecursiveBacktrackingCalculationUnit> allUnits = new List<RecursiveBacktrackingCalculationUnit>();
		List<RecursiveBacktrackingCalculationUnit> pathUnits = new List<RecursiveBacktrackingCalculationUnit>();
		FillUnitLists(mazeDimensions.x, mazeDimensions.y, allUnits, pathUnits);

		//Make the initial unit the current unit and mark it as visited
		RecursiveBacktrackingCalculationUnit startingUnit = pathUnits[(UnityEngine.Random.Range(0, (int)mazeDimensions.y))];
		RecursiveBacktrackingCalculationUnit currentUnit = startingUnit;
		Stack<RecursiveBacktrackingCalculationUnit> pathUnitStack = new Stack<RecursiveBacktrackingCalculationUnit>();

		currentUnit.isVisited = true;

		//While there are unvisited units
		while(pathUnits.Count != 0)
		{   //If the current unit has any neighbours which have not been visited
			if(ReturnUnvistedNeighbours(currentUnit, allUnits, ToolMethods.RowColumnSearch, (int)mazeDimensions.x).Count != 0)
			{
				//Randomly choose one of the unvistedNeightbours
				List<RecursiveBacktrackingCalculationUnit> unvistedNeightbours = ReturnUnvistedNeighbours(currentUnit, allUnits, ToolMethods.RowColumnSearch, (int)mazeDimensions.x);
				RecursiveBacktrackingCalculationUnit RandomlyChosenNeighbourUnit = unvistedNeightbours[UnityEngine.Random.Range(0, unvistedNeightbours.Count)];
				pathUnitStack.Push(currentUnit);
				currentUnit.isWall = false;
				RandomlyChosenNeighbourUnit.isWall = false;
				//Make the wall between the chosen unit and the current unit a pathUnit.
				RecursiveBacktrackingCalculationUnit wallCell = FindTheInBetweenWall(RandomlyChosenNeighbourUnit, allUnits, ToolMethods.RowColumnSearch, (int)mazeDimensions.x);
				wallCell.isWall = false;
				pathUnits.Remove(currentUnit);
				currentUnit = RandomlyChosenNeighbourUnit;
				currentUnit.isVisited = true;
			}
			else if(pathUnitStack.Count != 0)
			{
				currentUnit = pathUnitStack.Pop();
			}
			// If there are somehow units that couldn't be reached by the process above.
			else
			{
				//Choose one of the remaining unit and repeat the process
				RecursiveBacktrackingCalculationUnit reviveUnit = pathUnits[UnityEngine.Random.Range(0, pathUnits.Count)];
				currentUnit = reviveUnit;
				pathUnits.Remove(reviveUnit);
				currentUnit.isVisited = true;
			}
			await Task.Delay(1);
		}
		List<Vector2Int> positions = new List<Vector2Int>();
		foreach(RecursiveBacktrackingCalculationUnit cellToSpawn in allUnits)
		{
			if(cellToSpawn.isWall)
			{
				positions.Add(new Vector2Int((int)cellToSpawn.BaseCell.Position.x, (int)cellToSpawn.BaseCell.Position.y));
			}
		}
		return positions;
	}

	public static void FillUnitLists(int width, int height, List<RecursiveBacktrackingCalculationUnit> allUnitList, List<RecursiveBacktrackingCalculationUnit> pathUnitList)
	{
		//Total number of cells that needs to be initialized and added.
		int limit = width * height;

		for(int i = 0; i < limit; i++)
		{
			int cellWidth = i % width;
			int cellHeight = i / width;
			int oddOrEven = ((i / width) % 2 == 0) ? 0 : 1;

			BaseCell baseCell = new BaseCell(new Vector2 (cellWidth, cellHeight));
			
			RecursiveBacktrackingCalculationUnit unit = new RecursiveBacktrackingCalculationUnit(baseCell, width, height)
			{
				isVisited = false,
				isWall = true
			};
			if(cellHeight % 2 == 1)
			{
				allUnitList.Add(unit);
			}
			//Every unit on a even spot is going to be a path unit.
			else if(i % 2 == oddOrEven)
			{		
				unit.isWall = false;
				allUnitList.Add(unit);
				pathUnitList.Add(unit);
			}
			else
			{
				allUnitList.Add(unit);
			}
		}
	}

	public static List<RecursiveBacktrackingCalculationUnit> ReturnUnvistedNeighbours(RecursiveBacktrackingCalculationUnit currentcell, List<RecursiveBacktrackingCalculationUnit> cellCollectionToSearchFrom,
		Func<int, int, int, int> listSearchAlgorithmToUse, int mazeWidth)
	{
		List<RecursiveBacktrackingCalculationUnit> unvistedNeighbours = new List<RecursiveBacktrackingCalculationUnit>();
		int i = 0;
	
		foreach(KeyValuePair<string, Vector2> direction in RecursiveBacktrackingDirections.directions)
		{		
			if(currentcell.AvailableNeighbourDirections[i])
			{
				Vector2 neighbourPosition = currentcell.BaseCell.Position + direction.Value;
				RecursiveBacktrackingCalculationUnit NeighbourCell = cellCollectionToSearchFrom[listSearchAlgorithmToUse(mazeWidth, (int)neighbourPosition.x, (int)neighbourPosition.y)];
				if(!NeighbourCell.isVisited)
				{
					unvistedNeighbours.Add(NeighbourCell);
					NeighbourCell.ThroughWhatDirectionWasIFound = direction.Value;
				}
			}
			i++;
		}
		return unvistedNeighbours;
	}

	public static RecursiveBacktrackingCalculationUnit FindTheInBetweenWall(RecursiveBacktrackingCalculationUnit cellOnTheOtherSide, List<RecursiveBacktrackingCalculationUnit> cellCollectionToSearchFrom,
		Func<int, int, int, int> listSearchAlgorithmToUse, int mazeWidth)
	{
		RecursiveBacktrackingCalculationUnit wallCell = null;

		int directionCellX = (int)cellOnTheOtherSide.ThroughWhatDirectionWasIFound.x;
		if(Mathf.Abs(directionCellX) > 0)
		{
			directionCellX = directionCellX / 2;
		}
		int directionCellY = (int)cellOnTheOtherSide.ThroughWhatDirectionWasIFound.y;

		if(Mathf.Abs(directionCellY) > 0)
		{
			directionCellY = directionCellY / 2;
		}
		wallCell = cellCollectionToSearchFrom[listSearchAlgorithmToUse(mazeWidth, (int)cellOnTheOtherSide.BaseCell.Position.x - directionCellX,
			(int)cellOnTheOtherSide.BaseCell.Position.y - directionCellY)];
		return wallCell;
	}
}
#endregion