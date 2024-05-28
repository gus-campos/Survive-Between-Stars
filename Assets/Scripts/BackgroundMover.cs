using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using System.Runtime.CompilerServices;

public class BackgroundMover : MonoBehaviour {

    [SerializeField] private GameObject baseBackground;
    [SerializeField] private GameObject rocket;
    private Sprite sprite;

    private Vector2 size;
    private Vector2 sizeX;
    private Vector2 sizeY;
    private List<GameObject> backgrounds = new List<GameObject>();

    private Vector2 lastPosic;
    
    private Vector2 gridIndex;


    private void Awake() {

        rocket = GameObject.Find("Rocket");

        sprite = baseBackground.GetComponent<SpriteRenderer>().sprite;
        size = sprite.rect.size / sprite.pixelsPerUnit;

        sizeX = new Vector2(size.x, 0.0f);
        sizeY = new Vector2(0.0f, size.y);
    }

    private void updateSprites(Vector2 increment) {

        lastPosic += increment;

        foreach (GameObject background in backgrounds) {

            background.transform.position += new Vector3(increment.x, increment.y, 0.0f);
        }
    }

    public static Vector2 Snap(Vector2 vec, float gridSize = 1.0f) {

        return new Vector2(
            Mathf.Round(vec.x / gridSize) * gridSize,
            Mathf.Round(vec.y / gridSize) * gridSize);
    }

    private void Start() {

        lastPosic = rocket.transform.position;

        // Gerando sprites
        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {

                backgrounds.Add(Object.Instantiate(baseBackground, lastPosic + i*sizeX + j*sizeY, Quaternion.identity, transform));
            }
        }
    }

    private void Update() {

        // Achar índice da célula em que está
        gridIndex = Snap((new Vector2(rocket.transform.position.x, rocket.transform.position.y) - lastPosic) / size);

        // Incrementar a última posição
        updateSprites(size * gridIndex);

        // Reposicionar os sprits


    }
}
