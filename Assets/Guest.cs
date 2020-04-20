using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{


  public SkinnedMeshRenderer SKR { get; private set; }

  /// <summary>
  /// Amount of cash the guest will averagely spend in the club.
  /// </summary>
  [Range(0, 1f)] public float BigSpender;

  /// <summary>
  /// Amount of mood the guest will add or subtract from the club.
  /// </summary>
  [Range(-1f, 1f)] public float MoodFactor;

  /// <summary>
  /// If guest is picky about the mood, they will leave sooner than a normal guest.
  /// </summary>
  public bool PickyAboutMood;

  public Rigidbody Center;

  private void Start()
  {
    Center.detectCollisions = false;
    Center.mass = 0f;
    SKR = GetComponentInChildren<SkinnedMeshRenderer>();
  }


  public void StartDespawnTimer()
  {
    StartCoroutine(CountdownDeath());
  }

  private IEnumerator CountdownDeath()
  {
    yield return new WaitForSecondsRealtime(3f);
    Destroy(this);
  }


  public void EnterClub()
  {
    // TODO: add subtract mood
    // start coroutine spending money

    // start checking to leave if mood drops below a certain number
  }



}