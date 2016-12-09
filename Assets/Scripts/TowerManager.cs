﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerManager : Singleton<TowerManager> {

	public TowerButton towerButtonPressed {get; set;}

	private SpriteRenderer spriteRenderer;
    private List<Tower> TowerList = new List<Tower>();
    private List<Collider2D> BuildList = new List<Collider2D>();
    private Collider2D buildTile;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
			if (hit.collider.tag == "BuildSite") {
                buildTile = hit.collider;
				buildTile.tag = "BuildSiteFull";
                RegisterBuildSite(buildTile);
                
				placeTower(hit);
			}
		}
		if (spriteRenderer.enabled) {
				followMouse();
			}
	}

    public void RegisterBuildSite(Collider2D buildTag) {
        BuildList.Add(buildTag);
    }

    public void RegisterTower(Tower tower) {
        TowerList.Add(tower);
    }

    public void RenameTagsBuildSites() {
        foreach (Collider2D buildTag in BuildList) {
            buildTag.tag = "BuildSite";
        }
        BuildList.Clear();
    }

    public void DestroyAllTower() {
        foreach(Tower tower in TowerList) {
            Destroy(tower.gameObject);
        }
        TowerList.Clear();
    }

	public void placeTower(RaycastHit2D hit) {
		if (!EventSystem.current.IsPointerOverGameObject() && towerButtonPressed != null) {
			Tower newTower = Instantiate(towerButtonPressed.TowerObject) as Tower;
			newTower.transform.position = hit.transform.position;
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
            BuyTower(towerButtonPressed.TowerPrice);
            RegisterTower(newTower);
            disableDragSprite();
		}
	}

    public void BuyTower(int price) {
        GameManager.Instance.SubtractMoney(price);
    }

	public void selectedTower(TowerButton towerSelected) {
        if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney) {
            towerButtonPressed = towerSelected;
		    enableDragSprite(towerButtonPressed.DragSprite);
        }
	}

	public void followMouse() {
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector2(transform.position.x, transform.position.y);
	}

	public void enableDragSprite(Sprite sprite) {
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void disableDragSprite() {
		spriteRenderer.enabled = false;
	}
}
