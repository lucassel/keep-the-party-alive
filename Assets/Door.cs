using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
  private List<Vector3> Points;
  public List<GameObject> Guests;
  private List<GameObject> _spawned;
  private Queue<GameObject> _queue;
  public Material CulledMaterial;


  private Animator _animator;
  private static readonly int Bam = Animator.StringToHash("BAM");


  private Guest _dudeAboutToBeFlung;


  private void Start()
  {
    _animator = GetComponent<Animator>();
    Points = new List<Vector3>();
    _spawned = new List<GameObject>();
    _queue = new Queue<GameObject>();


    for (int i = 0; i < 9; i++)
    {
      Points.Add(new Vector3(transform.position.x + .5f - i, 0, -1f));
      var guest = Guests[Random.Range(0, Guests.Count)];
      var spawn = Instantiate(guest, Points[i], Quaternion.identity);
      spawn.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
      _spawned.Add(spawn);
      _queue.Enqueue(spawn);
    }
  }



  private int countdown = 60;
  private int counter;
  
  
  
  private Vector2 fingerDown;
  private Vector2 fingerUp;
  public bool detectSwipeOnlyAfterRelease = false;

  public float SWIPE_THRESHOLD = 20f;
  

  
  private void Update()
  {

    if (counter < countdown)
    {
      if (Application.isMobilePlatform)
      {
        foreach (Touch touch in Input.touches)
        {
          if (touch.phase == TouchPhase.Began)
          {
            fingerUp = touch.position;
            fingerDown = touch.position;
          }

          //Detects Swipe while finger is still moving
          if (touch.phase == TouchPhase.Moved)
          {
            if (!detectSwipeOnlyAfterRelease)
            {
              fingerDown = touch.position;
              checkSwipe();
            }
          }

          //Detects swipe after finger is released
          if (touch.phase == TouchPhase.Ended)
          {
            fingerDown = touch.position;
            checkSwipe();
          }
        }
      }
      else
      {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          AcceptGuest();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
          DenyGuestEntry();
        }
      }

      counter++;
      
    }
    else
    {
      counter = 0;
    }

  }

  
  
  void checkSwipe()
  {
    //Check if Vertical swipe
    if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
    {
      //Debug.Log("Vertical");
      if (fingerDown.y - fingerUp.y > 0)//up swipe
      {
        OnSwipeUp();
      }
      else if (fingerDown.y - fingerUp.y < 0)//Down swipe
      {
        OnSwipeDown();
      }
      fingerUp = fingerDown;
    }

    //Check if Horizontal swipe
    else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
    {
      //Debug.Log("Horizontal");
      if (fingerDown.x - fingerUp.x > 0)//Right swipe
      {
        OnSwipeRight();
      }
      else if (fingerDown.x - fingerUp.x < 0)//Left swipe
      {
        OnSwipeLeft();
      }
      fingerUp = fingerDown;
    }

    //No Movement at-all
    else
    {
      //Debug.Log("No Swipe!");
    }
  }

  float verticalMove()
  {
    return Mathf.Abs(fingerDown.y - fingerUp.y);
  }

  float horizontalValMove()
  {
    return Mathf.Abs(fingerDown.x - fingerUp.x);
  }

  //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
  void OnSwipeUp()
  {
    Debug.Log("Swipe UP");
  }

  void OnSwipeDown()
  {
    Debug.Log("Swipe Down");
  }

  void OnSwipeLeft()
  {
    DenyGuestEntry();
  }

  void OnSwipeRight()
  {
    AcceptGuest();
  }
  
  
  
  private void MoveQueueUpAndInsertAtEnd()
  {
    var temp = _queue.ToList();
    for (int i = 0; i < _queue.Count; i++)
    {
      StartCoroutine(MoveGuestUpQueue(temp[i], Points[i]));
    }


    // ENQUEUE AT END
    var new_dude = Instantiate(Guests[Random.Range(0, Guests.Count)], Points[Points.Count - 1], Quaternion.identity);
    new_dude.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    _queue.Enqueue(new_dude);
  }


  private void AcceptGuest()
  {
    Debug.Log("BAM");
    _animator.SetTrigger(Bam);
    var guest = _queue.Dequeue();
    StartCoroutine(PushGuest(guest));
    MoveQueueUpAndInsertAtEnd();
  }


  private IEnumerator MoveGuestUpQueue(GameObject guest, Vector3 newSpot)
  {
    while (Vector3.Distance(guest.transform.position, newSpot) > .01f)
    {
      guest.transform.position = Vector3.MoveTowards(guest.transform.position, newSpot, Time.deltaTime);
      yield return null;
    }
  }


  private void DenyGuestEntry()
  {
    var guest = _queue.Dequeue();


    var anim = guest.GetComponent<Animator>();
    anim.enabled = false;
    var g = guest.GetComponent<Guest>();
    g.Center.detectCollisions = true;
    g.Center.mass = 60f;
    g.Center.AddForce(guest.transform.right * 800f, ForceMode.Impulse);
    g.StartDespawnTimer();
    MoveQueueUpAndInsertAtEnd();
  }


  private IEnumerator PushGuest(GameObject guest)
  {
    var anim = guest.GetComponent<Animator>();
    anim.enabled = false;
    var target = transform.forward * 4f;
    _dudeAboutToBeFlung = guest.GetComponent<Guest>();
    _dudeAboutToBeFlung.Center.detectCollisions = true;
    _dudeAboutToBeFlung.Center.mass = 60f;
    _dudeAboutToBeFlung.Center.AddForce(guest.transform.right * -750f, ForceMode.Impulse);

    while (Vector3.Distance(guest.transform.position, target) > .1f)
    {
      //Debug.Log($"moving {guest.name}");
      yield return null;
    }


    //anim.enabled = true;
  }



  private void OnTriggerEnter(Collider other)
  {
    if (_dudeAboutToBeFlung != null)
    {
      _dudeAboutToBeFlung.SKR.sharedMaterial = CulledMaterial;
      _dudeAboutToBeFlung = null;
    }
  }

  private void OnDrawGizmos()
  {
    if (Points != null)
    {
      foreach (var p in Points)
      {
        Gizmos.DrawSphere(p, .25f);
      }
    }
  }
}