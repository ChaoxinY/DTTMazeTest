using UnityEngine;
using System.Collections;

public static class ToolMethods
{
	public static int SetEvenNumberToOdd(int numberInQuestion)
	{
		int processedNumber = 0;
		processedNumber = numberInQuestion % 2 == 0 ? numberInQuestion - 1 : numberInQuestion;
		return processedNumber;
	}

	//Width for how many cells are on the row
	//y for how deep in the column
	//x for the exact position on the row
	public static int RowColumnSearch(int rowCount, int x, int y)
	{
		int indexCell = (rowCount * y) + x;
		return indexCell;
	}

	public static Vector3 CalculateCenterpoint(float width, float height)
	{
		Vector3 centerPointPosition = new Vector3(width / 2f - 0.5f, 0, height / 2f - 0.2f);
		return centerPointPosition;
	}

	public static void CameraOverViewFocusOnGameObject(Camera camera, Vector3 focusPosition, float objectWidth)
	{
		camera.transform.position = new Vector3(focusPosition.x, focusPosition.y + objectWidth + 3f, focusPosition.z);
	}

}
