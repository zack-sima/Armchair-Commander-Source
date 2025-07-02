using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public Vector2[] lowBounds, highBounds;
	[HideInInspector]
	public Vector2 lowBound, highBound;
	[HideInInspector]
	public Vector2 lowerCornerBound, upperCornerBound;
	Camera cam;
	Vector3 lastMousePos;
	Vector3 currentMousePos;
	Vector3 movePos;
	bool stoppedDrag;
	Controller controller;

	//map sizes
	public float[] bounds;
	float upperBound;
	public float lowerBound;
	public float minOrtho = -1f;
	public float maxOrtho = -1f;
	bool notched = false;
	private void Start() {
		notched = Screen.safeArea.width != Screen.width;

		lowerCornerBound = new Vector2(-100f, -100f);
		upperCornerBound = new Vector2(120f, 110f);

		cam = GetComponent<Camera>();
		controller = Controller.instance;
	}

#if UNITY_EDITOR
	bool editorLocked = false;
#endif

	bool deltaLocked = false;
	void Update() {
		//for trailer purposes lock screen to prevent movement
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.L)) {
			editorLocked = !editorLocked;
		}
		if (editorLocked) return;
#endif

		if (controller.editMode && controller.mouseInUI()) {
			deltaLocked = true;
			return;
		}
		if (controller.diplomacyInstance != null)
			return;

		lowBound = lowBounds[controller.mapSize];
		highBound = highBounds[controller.mapSize];

		if (notched) {
			lowBound = new Vector2(lowBound.x - 2, lowBound.y);
			highBound = new Vector2(highBound.x + 2, highBound.y);
		}

		upperBound = bounds[controller.mapSize];
		if ((!controller.editMode || controller.moveToggle.isOn) && !controller.paused && !controller.isTutorial && !controller.hasDialogue) {
			lastMousePos = currentMousePos;
			currentMousePos = Input.mousePosition;
			if (Input.GetMouseButtonUp(0)) {
				stoppedDrag = true;
			}

			float speed = (1f + (cam.orthographicSize - 1.5f) / 10f) / 8f * (1.5f + MyPlayerPrefs.instance.GetFloat("sensitivity") + (CustomFunctions.getIsMobile() ? 1f : 0f));
			if (Input.touchCount < 2 && Input.GetMouseButton(0)) {
				if (stoppedDrag) {
					lastMousePos = currentMousePos;
					stoppedDrag = false;
				}
				if (!deltaLocked)
					movePos = (new Vector3(lastMousePos.x, lastMousePos.y, 0f) - new Vector3(currentMousePos.x, currentMousePos.y, 0f)) / 29f;
			} else {
				stoppedDrag = true;
			}
			transform.position += new Vector3(movePos.x * speed, movePos.y * speed, 0f);
			checkBoundViolation(false);
			movePos *= 0.8f;
			if (Input.touchCount == 2) {
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				float actualMag = Mathf.Abs(deltaMagnitudeDiff);
				if (actualMag > 125f)
					actualMag = 123f;
				if (deltaMagnitudeDiff > 0f)
					cam.orthographicSize += actualMag * Time.deltaTime * 0.2f * (1.5f + MyPlayerPrefs.instance.GetFloat("sensitivity"));
				else if (deltaMagnitudeDiff < 0f)
					cam.orthographicSize -= actualMag * Time.deltaTime * 0.2f * (1.5f + MyPlayerPrefs.instance.GetFloat("sensitivity"));
				checkBoundViolation(false);

			}

			if (!CustomFunctions.getIsMobile()) {
				if (Input.mouseScrollDelta.y < 0f && (cam.orthographicSize < maxOrtho * (controller.editMode ? 2 : 1) || maxOrtho < 0f) ||
					Input.mouseScrollDelta.y > 0f && (cam.orthographicSize > minOrtho || minOrtho < 0f)) {
					cam.orthographicSize -= Input.mouseScrollDelta.y * Time.deltaTime * 6f;
				}
				checkBoundViolation(false);
				//keyboard movement
				float mult = 1.5f;
				if (Input.GetKey(KeyCode.LeftShift)) {
					mult = 3.5f;
				}
				float sense = 1f + MyPlayerPrefs.instance.GetFloat("sensitivity") / 2f;
				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
					transform.Translate(5 * mult * sense * Time.deltaTime * Vector3.up);
				}
				if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
					transform.Translate(5 * mult * sense * Time.deltaTime * Vector3.down);
				}
				if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
					transform.Translate(5 * mult * sense * Time.deltaTime * Vector3.left);
				}
				if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
					transform.Translate(5 * mult * sense * Time.deltaTime * Vector3.right);
				}
			}

			if (cam.orthographicSize < minOrtho && minOrtho > 0f)
				cam.orthographicSize = minOrtho;
			else if (cam.orthographicSize > maxOrtho * (controller.editMode ? 2 : 1) && maxOrtho > 0f)
				cam.orthographicSize = maxOrtho * (controller.editMode ? 2 : 1);
		}
		checkBoundViolation(false);
		deltaLocked = false;
	}

	void checkBoundViolation(bool settingLimits) {

		Vector2 lowCorner = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Vector2.zero), Vector2.zero).point;
		Vector2 highCorner = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)), Vector2.zero).point;

		if (settingLimits || maxOrtho < 0) {
			if (Mathf.Abs(highCorner.x - lowCorner.x) > upperBound) {
				cam.orthographicSize -= 0.05f;
				checkBoundViolation(true);
			} else if (settingLimits && Mathf.Abs(highCorner.x - lowCorner.x) > upperBound - 1f) {
				maxOrtho = cam.orthographicSize;
			}
		}
		if (settingLimits || minOrtho < 0) {
			if (Mathf.Abs(highCorner.x - lowCorner.x) < lowerBound) {
				cam.orthographicSize += 0.05f;
				checkBoundViolation(true);
			} else if (settingLimits && Mathf.Abs(highCorner.x - lowCorner.x) < lowerBound + 1f) {
				minOrtho = cam.orthographicSize;
			}
		}
		if (!controller.editMode && !settingLimits) {
			if (lowCorner.x < lowBound.x)
				cam.transform.position = new Vector3(cam.transform.position.x + lowBound.x - lowCorner.x, cam.transform.position.y, cam.transform.position.z);
			if (lowCorner.y < lowBound.y)
				cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + lowBound.y - lowCorner.y, cam.transform.position.z);
			if (highCorner.x > highBound.x)
				cam.transform.position = new Vector3(cam.transform.position.x + highBound.x - highCorner.x, cam.transform.position.y, cam.transform.position.z);
			if (highCorner.y > highBound.y)
				cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + highBound.y - highCorner.y, cam.transform.position.z);
		}
	}
}