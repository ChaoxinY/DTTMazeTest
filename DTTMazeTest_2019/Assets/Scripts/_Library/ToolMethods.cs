using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class ToolMethods
{
	public static int SetEvenNumberToOdd(int numberInQuestion)
	{
		int processedNumber = 0;
		processedNumber = numberInQuestion % 2 == 0 ? numberInQuestion - 1 : numberInQuestion;
		return processedNumber;
	}

	public static int GetIntValueFromInputField(InputField inputField)
	{
		if(int.TryParse(inputField.text, out int value))
		{
			value = int.Parse(inputField.text);
		}
		return value;
	}
	//Width for how many cells are on the row
	//y for how deep in the column
	//x for the exact position on the row
	public static int RowColumnSearch(int rowCount, int x, int y)
	{
		int indexCell = (rowCount * y) + x;
		return indexCell;
	}

	public static Vector3 CalculateTransformCenterpoint(Vector3 position)
	{
		Vector3 centerPointPosition = new Vector3(position.x / 2f, position.y, position.z / 2f);
		return centerPointPosition;
	}

	public static void CameraFocusOnGameObject(Camera camera, Vector3 focusPosition, Vector2 objectDimension)
	{
		float highestMeasureMentValue = objectDimension.x > objectDimension.y ? objectDimension.x : objectDimension.y;
		camera.transform.position = new Vector3(focusPosition.x, focusPosition.y + highestMeasureMentValue + 5f, focusPosition.z);
	}

}
