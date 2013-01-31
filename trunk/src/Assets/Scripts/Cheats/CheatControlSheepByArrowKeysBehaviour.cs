﻿using System.Collections.Generic;
using UnityEngine;

public sealed class CheatControlSheepByArrowKeysBehaviour : MonoBehaviour {
    private static readonly Dictionary<KeyCode, Vector3> MovementMultipliers = new Dictionary<KeyCode, Vector3>() {
            {KeyCode.I, new Vector3(0, 0, 1)},
            {KeyCode.K, new Vector3(0, 0, -1)},
    };

    private static readonly Dictionary<KeyCode, int> RotationMultipliers = new Dictionary<KeyCode, int>() {
            {KeyCode.J,-1},
            {KeyCode.L,1},
    };

    private const int Button = 2;
    private const float MovementSpeed = 10f;
    private const float RotationSpeed = 90f;

    private GameObject marker;
    private bool isMouseOver;
    private bool isSelected;

    public void SetMarker(GameObject projectorToSet) {
        GameObject copy = (GameObject)Object.Instantiate(projectorToSet);

        copy.transform.parent = this.transform;
        copy.transform.localPosition = projectorToSet.transform.localPosition;
        copy.transform.localRotation = projectorToSet.transform.localRotation;
        copy.SetActive(false);

        this.marker = copy;
    }

    void OnMouseOver() {
        this.isMouseOver = true;
        this.ShowMarker = true;
    }

    void OnMouseExit() {
        this.isMouseOver = false;
        this.ShowMarker = false;
    }

    private void Update() {
        // check for select / deselect
        if (Input.GetMouseButtonUp(Button) && this.isMouseOver) {
            this.isSelected = !this.isSelected;
        }

        // don't continue when not selected
        if (!this.isSelected) {
            return;
        }
        this.ShowMarker = this.isSelected;

        // control: movement
        foreach (KeyValuePair<KeyCode, Vector3> controlPair in MovementMultipliers) {
            KeyCode code = controlPair.Key;

            if (Input.GetKey(code)) {
                Vector3 movementSpeed = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * controlPair.Value;
                movementSpeed = movementSpeed * CheatControlSheepByArrowKeysBehaviour.MovementSpeed * Time.deltaTime;
                movementSpeed.y = 0;

                this.transform.Translate(movementSpeed, Space.World);
            }
        }

        // control: rotation
        foreach (KeyValuePair<KeyCode, int> controlPair in RotationMultipliers) {
            KeyCode code = controlPair.Key;

            if (Input.GetKey(code)) {
                this.transform.Rotate(Vector3.up, controlPair.Value * Time.deltaTime * RotationSpeed, Space.Self);
            }
        }
    }

    private bool ShowMarker {
        set {
            if (this.marker != null) {
                this.marker.SetActive(value);
            }
        }
    }
}