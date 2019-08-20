using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

#region RecursiveBacktracking
public static class Directions
{
	//Basic 2D directions.
	//Using Vector2 because its just easier make use of it.
	//Using an addition of 2 instead of 1 is because the wallcells themselves 
	//also take up a spot in the list.
	//Enum
	public static readonly Vector2[] directions = new Vector2[] { new Vector2(0, 2), new Vector2(0, -2),
		new Vector2(-2, 0),new Vector2(2, 0)  };
}

public abstract class BaseCell
{
	protected Vector2 position;

	public Vector2 Position
	{
		get
		{
			return position;
		}
	}

	//Whether the cell should be spawned or not.
	public bool isWall;
}

public class RecursiveBacktrackingCell : BaseCell
{
	public bool isVisited;
	//No need to do the math and trying to find a neighbour that either doenst exsist or 
	//is on a wrong spot. Just check for the bool.
	private bool[] availableNeighbourDirections = new bool[] { true, true, true, true };
	//Use to backtrack the direction and find the wall between this cell and the cell that
	//choose this cell.
	public Vector2 ThroughWhatDirectionWasIFound;

	public bool[] AvailableNeighbourDirections
	{
		get
		{
			return availableNeighbourDirections;
		}
	}

	public RecursiveBacktrackingCell(int horizontalPosition, int verticalPosition, int mazeWidth, int mazeHeight)
	{
		isVisited = false;
		position = new Vector2(horizontalPosition, verticalPosition);
		if(!isWall)
		{
			AddAvailableNeighbourDirection(this, mazeWidth, mazeHeight);
		}
	}

	public void AddAvailableNeighbourDirection(RecursiveBacktrackingCell currentCell, int mazeWidth, int mazeHeight)
	{
		int i = 0;

		foreach(Vector2 direction in Directions.directions)
		{
			//Up
			if(i == 1 && (currentCell.position.y == 0 || currentCell.position.y == 1))
			{
				availableNeighbourDirections[i] = false;
			}
			//Down
			else if(i == 0 && (currentCell.position.y == mazeHeight - 1 || currentCell.position.y == mazeHeight - 2))
			{
				availableNeighbourDirections[i] = false;
			}
			//Left
			else if(i == 2 && (currentCell.position.x == 0 || currentCell.position.x == 1))
			{
				availableNeighbourDirections[i] = false;
			}
			//Right
			else if(i == 3 && (currentCell.position.x == mazeWidth - 1 || currentCell.position.x == mazeWidth - 2))
			{
				availableNeighbourDirections[i] = false;
			}
			i++;
		}
	}
}

public static partial class MazeCalculatingAlgorithms
{
	public async static Task<List<Vector2Int>> CalculateRecursiveBacktrackingMaze(Vector2Int mazeDimensions)
	{
		List<RecursiveBacktrackingCell> allCells = new List<RecursiveBacktrackingCell>();
		List<RecursiveBacktrackingCell> pathCells = new List<RecursiveBacktrackingCell>();
		FillCellLists(mazeDimensions.x, mazeDimensions.y, allCells, pathCells);

		//Make the initial cell the current cell and mark it as visited
		RecursiveBacktrackingCell initialCell = pathCells[(UnityEngine.Random.Range(0, (int)mazeDimensions.y))];
		RecursiveBacktrackingCell currentCell = initialCell;
		Stack<RecursiveBacktrackingCell> pathCellStack = new Stack<RecursiveBacktrackingCell>();

		currentCell.isVisited = true;

		//While there are unvisited cells
		while(pathCells.Count != 0)
		{   //If the current cell has any neighbours which have not been visited
			if(ReturnUnvistedNeighbours(currentCell, allCells, ToolMethods.RowColumnSearch, (int)mazeDimensions.x).Count != 0)
			{
				//Choose randomly one of the unvisited neighbours
				List<RecursiveBacktrackingCell> unvistedNeightbours = ReturnUnvistedNeighbours(currentCell, allCells, ToolMethods.RowColumnSearch, (int)mazeDimensions.x);
				RecursiveBacktrackingCell RandomlyChosenNeighbourCell = unvistedNeightbours[UnityEngine.Random.Range(0, unvistedNeightbours.Count)];
				pathCellStack.Push(currentCell);
				currentCell.isWall = false;
				RandomlyChosenNeighbourCell.isWall = false;
				//Make the wall between the chosen cell and the current cell a pathcell.
				RecursiveBacktrackingCell wallCell = FindTheInBetweenWall(RandomlyChosenNeighbourCell, allCells, ToolMethods.RowColumnSearch, (int)mazeDimensions.x);
				wallCell.isWall = false;
				pathCells.Remove(currentCell);
				currentCell = RandomlyChosenNeighbourCell;
				currentCell.isVisited = true;
			}
			else if(pathCellStack.Count != 0)
			{
				currentCell = pathCellStack.Pop();
			}
			// If there are somehow cells that couldnt be reached by the process above.
			else
			{
				//Choose one of the remaining cell and repeat the process
				RecursiveBacktrackingCell reviveCell = pathCells[UnityEngine.Random.Range(0, pathCells.Count)];
				currentCell = reviveCell;
				pathCells.Remove(reviveCell);
				currentCell.isVisited = true;
			}
			await Task.Delay(1);
		}
		List<Vector2Int> positions = new List<Vector2Int>();
		foreach(RecursiveBacktrackingCell cellToSpawn in allCells)
		{
			if(cellToSpawn.isWall)
			{
				positions.Add(new Vector2Int((int)cellToSpawn.Position.x,(int)cellToSpawn.Position.y));
			}
		}
		return positions;
	}

	public static void FillCellLists(int width, int height, List<RecursiveBacktrackingCell> allCellList, List<RecursiveBacktrackingCell> pathCellList)
	{
		//Total number of cells that needs to be initialized and added.
		int limit = width * height;

		for(int i = 0; i < limit; i++)
		{
			int x = i % width;
			int y = i / width;
			int oddOrEven = ((i / width) % 2 == 0) ? 0 : 1;

			RecursiveBacktrackingCell cell = new RecursiveBacktrackingCell(x, y, width, height)
			{
				isVisited = false,
				isWall = true
			};
			//Every cell on an odd numbered row should be a wall from the start.
			if(y % 2 == 1)
			{
				allCellList.Add(cell);
			}
			//Every other cell on a even spot is going to be a path cell.
			else if(i % 2 == oddOrEven)
			{
				cell.isWall = false;
				allCellList.Add(cell);
				pathCellList.Add(cell);
			}
			else
			{
				allCellList.Add(cell);
			}
		}
	}

	//Using delegate parameter here so that if I have come up with a more effcient search algorithm
	//I can just past it in instead of having to change the method itself.
	//Still a bit inefficient because now I have to use the same signature for the upcoming 
	//algorithms.
	public static List<RecursiveBacktrackingCell> ReturnUnvistedNeighbours(RecursiveBacktrackingCell currentcell, List<RecursiveBacktrackingCell> cellCollectionToSearchFrom,
		Func<int, int, int, int> listSearchAlgorithmToUse, int mazeWidth)
	{
		List<RecursiveBacktrackingCell> unvistedNeighbours = new List<RecursiveBacktrackingCell>();
		int i = 0;
		foreach(Vector2 direction in Directions.directions)
		{
			if(currentcell.AvailableNeighbourDirections[i])
			{
				Vector2 neighbourPosition = currentcell.Position + direction;
				RecursiveBacktrackingCell NeighbourCell = cellCollectionToSearchFrom[listSearchAlgorithmToUse(mazeWidth, (int)neighbourPosition.x, (int)neighbourPosition.y)];
				if(!NeighbourCell.isVisited)
				{
					unvistedNeighbours.Add(NeighbourCell);
					NeighbourCell.ThroughWhatDirectionWasIFound = direction;
				}
			}
			i++;
		}
		return unvistedNeighbours;
	}

	public static RecursiveBacktrackingCell FindTheInBetweenWall(RecursiveBacktrackingCell cellOnTheOtherSide, List<RecursiveBacktrackingCell> cellCollectionToSearchFrom,
		Func<int, int, int, int> listSearchAlgorithmToUse, int mazeWidth)
	{
		RecursiveBacktrackingCell wallCell = null;

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
		wallCell = cellCollectionToSearchFrom[listSearchAlgorithmToUse(mazeWidth, (int)cellOnTheOtherSide.Position.x - directionCellX,
			(int)cellOnTheOtherSide.Position.y - directionCellY)];
		return wallCell;
	}
}
#endregion