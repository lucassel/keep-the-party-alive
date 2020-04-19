using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{
  /// <summary>
  /// Amount of cash the guest will averagely spend in the club.
  /// </summary>
  public float BigSpender;

  /// <summary>
  /// Amount of mood the guest will add or subtract from the club.
  /// </summary>
  public float MoodFactor;

  /// <summary>
  /// If guest is picky about the mood, they will leave sooner than a normal guest.
  /// </summary>
  public bool PickAboutMood;

  public Rigidbody Center;

  private void Start()
  {
    Center.detectCollisions = false;
    Center.mass = 0f;
  }

  public void EnterClub()
  {
    // TODO: add subtract mood
    // start coroutine spending money

    // start checking to leave if mood drops below a certain number
  }
}