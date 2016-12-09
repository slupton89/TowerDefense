﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    private Transform exitPoint;
    [SerializeField]
    private Transform[] waypoints;
    [SerializeField]
    private float navigationUpdate;
    [SerializeField]
    private float health;
    [SerializeField]
    private int rewardAmount;

    private int target = 0;
    private Transform enemy;
    private Collider2D enemyCollider;
    private Animator anim;
    private float navigationTime = 0;
    private bool isDead = false;

    public bool IsDead {
        get {
            return isDead;
        }
    }
	// Use this for initialization
	void Start () {
        enemy = GetComponent<Transform>();
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        GameManager.Instance.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (waypoints != null && !isDead) {
            navigationTime += Time.deltaTime;
            if (navigationTime > navigationUpdate) {
                if (target < waypoints.Length) {
                    enemy.position = Vector2.MoveTowards(enemy.position, waypoints[target].position, navigationTime);
                } else {
                    enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
                }
                navigationTime = 0;
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Checkpoint") {
            target += 1;
        } else if (collision.tag == "Finish") {
            GameManager.Instance.RoundEscaped += 1;
            GameManager.Instance.TotalEscaped += 1;
            GameManager.Instance.UnregisterEnemy(this);
            GameManager.Instance.IsWaveOver();
        } else if (collision.tag == "Projectile") {
            Projectile newP = collision.gameObject.GetComponent<Projectile>();
            EnemyHit(newP.AttackStrength);
            Destroy(collision.gameObject);
            
        }
    }

    public void EnemyHit(int hitpoints) {
        if (health - hitpoints > 0) {
            health -= hitpoints;
            anim.Play("Hurt");
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
        } else {
            anim.SetTrigger("DidDie");
            Die();
        }
    }

    public void Die() {
        isDead = true;
        enemyCollider.enabled = false;
        GameManager.Instance.TotalKilled += 1;
        GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
        GameManager.Instance.IsWaveOver();
        GameManager.Instance.AddMoney(rewardAmount);
    }
}
