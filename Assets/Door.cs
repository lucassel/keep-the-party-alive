using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
  private void Update()
  {

    if (counter < countdown)
    {
      if (Application.isMobilePlatform)
      {
   
        // TODO: swipe AR
      }
      else
      {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          FlingGuest();
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


  private void FlingGuest()
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


  public static GameObject FindParentWithTag(GameObject childObject, string tag)
  {
    Transform t = childObject.transform;
    while (t.parent != null)
    {
      if (t.parent.tag == tag)
      {
        return t.parent.gameObject;
      }

      t = t.parent.transform;
    }

    return null; // Could not find a parent with given tag.
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