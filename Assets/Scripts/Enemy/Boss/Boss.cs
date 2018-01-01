using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0219  

/// Base class for bosses
public class Boss : MonoBehaviour {
  protected Player player;
  protected GameObject healthBar;
  public State state;
  public AudioSource audioSource;

  protected bool died;

  private float maxHealthAmount;
  public float healthAmount;

  public enum State {
    idle,
    walking,
    noticed,
    attacking,
    damaged,
    blocking,
    dead,
  }

  public bool stunned;
  public float KP;

  public virtual void Start() {
    audioSource = GetComponent<AudioSource>();

    try {
      healthBar = transform.Find("BossHealthBar").gameObject;
      healthBar.SetActive(true);
    } catch {
      Debug.Log("There is no available boss healthbar");
    }

    maxHealthAmount = healthAmount;

    player = FindObjectOfType<Player>();

    died = false;
  }

  public virtual void Update() {
    HandleState();
  }

  protected virtual void Move() {
    AutoPath();
  }

  public float attackRange;
  protected virtual void AutoPath() {
    float xDist = player.transform.position.x - transform.position.x;

    if (Mathf.Abs(xDist) < attackRange) {
      // StartCoroutine(Attack());
      return;
    }

    if (Mathf.Abs(xDist) >= 0.6f)
      transform.position = new Vector2(transform.position.x + (xDist * KP), transform.position.y);
  }

  public virtual void UpdateHealthBar() {
    Image bar = healthBar.transform.Find("HealthBar/Bar").GetComponent<Image>();

    bar.fillAmount = healthAmount / maxHealthAmount;
    if (bar.fillAmount <= .4)
      bar.color = new Color(1, 0, 0, 1);
    else if (bar.fillAmount <= .7)
      bar.color = new Color(1, 0.39f, 0, 1);
    healthBar.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
  }

  public void Dying() {
    stunned = true;
    healthBar.SetActive(false);
    
    state = State.dead;
    gameObject.layer = LayerMask.NameToLayer("Debris");
    // TODO: disable hurtboxes
  }

  public virtual void Kill() {
    // deletes the game object
    // for (int i = 0; i < 4; i++)
      // PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);
    Destroy(gameObject);
  }

  protected virtual void HandleState() {
    // Move();
    if (stunned) transform.position = transform.position;
  }

  // method to play sounds from animator
	public void PlayOneShot(AudioClip sound) {
		audioSource.PlayOneShot(sound);
	}

}
