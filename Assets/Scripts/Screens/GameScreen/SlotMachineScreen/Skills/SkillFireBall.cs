﻿using UnityEngine;
using System.Collections;

public class SkillFireBall : Skill {

	public Transform fireBallPrefab;
	public Transform explodePrefab;
		
	public override void Init(int level, int damage, BossManager bossManager) {
		this.bossManager = bossManager;
		this.damage = damage;
		explodePrefab.gameObject.SetActive(false);
		SpawnParticle();
		base.Init();
		// transform.position = ScreenManager.Instance.CurrentSlotScreen.userAvatarPanel.position;

	}
	
	void SpawnParticle() {
		fireBallPrefab.position = ScreenManager.Instance.CurrentSlotScreen.userAvatarPanel.position;
		fireBallPrefab.gameObject.SetActive(true);
		LeanTween.move(fireBallPrefab.gameObject, bossManager.GetBossMiddlePoint(), 0.8f).setEase(LeanTweenType.easeInQuad).setOnComplete(PreExplode);
	}
	
	void PreExplode() {
		StartCoroutine(Explode());
	}
	
	IEnumerator Explode() {
		explodePrefab.position = bossManager.GetBossMiddlePoint();
		explodePrefab.gameObject.SetActive(true);
		yield return null;
		fireBallPrefab.gameObject.SetActive(false);
		bossManager.Shake();
		bossManager.GetHit(damage);
		// GameObject.Destroy(this.gameObject);
	}
}
