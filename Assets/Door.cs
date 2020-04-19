using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
  private List<Vector3> Points;
  public List<GameObject> Guests;
  private List<GameObject> _spawned;
  private Queue<GameObject> _queue;


  private Animator _animator;
  private static readonly int Bam = Animator.StringToHash("BAM");

  private int _counter;

  private void Start()
  {
    _animator = GetComponent<Animator>();
    Points = new List<Vector3>();
    _spawned = new List<GameObject>();
    _queue = new Queue<GameObject>();


    for (int i = 0; i < 9; i++)
    {
      Points.Add(new Vector3(transform.position.x + .5f, 0, -1f - i));
      var guest = Guests[Random.Range(0, Guests.Count)];
      var spawn = Instantiate(guest, Points[i], Quaternion.identity);
      _spawned.Add(spawn);
      _queue.Enqueue(spawn);
    }
  }


  private void Update()
  {
    // TODO: fix for AR
    if (Input.GetKeyDown(KeyCode.Space))
    {
      FlingGuest();
    }
  }


  private void FlingGuest()
  {
    Debug.Log("BAM");
    _animator.SetTrigger(Bam);
    var guest = _queue.Dequeue();
    StartCoroutine(PushGuest(guest));
    // MOVE QUEUE

    foreach (var dude in _queue)
    {
      StartCoroutine(MoveGuestUpQueue(dude));
    }


    // ENQUEUE AT END
    var new_dude = Instantiate(Guests[Random.Range(0, Guests.Count)], Points[Points.Count - 1], Quaternion.identity);
    _queue.Enqueue(new_dude);
  }


  private IEnumerator MoveGuestUpQueue(GameObject guest)
  {
    var t = guest.transform.position + Vector3.forward;
    
    while (Vector3.Distance(guest.transform.position, t) > .01f)
    {
      guest.transform.position = Vector3.MoveTowards(guest.transform.position, t, Time.deltaTime);
      yield return null;
    }
  }

  private IEnumerator PushGuest(GameObject guest)
  {
    var anim = guest.GetComponent<Animator>();
    anim.enabled = false;
    var target = transform.forward * 4f;
    var g = guest.GetComponent<Guest>();
    g.Center.detectCollisions = true;
    g.Center.mass = 10f;
    g.Center.AddForce(guest.transform.forward *800f, ForceMode.Impulse);
    
    while (Vector3.Distance(guest.transform.position, target) > .1f)
    {
      Debug.Log($"moving {guest.name}");
      yield return null;
    }


    //anim.enabled = true;
  }


  private void OnDrawGizmos()
  {
    if (Points != null)
    {
      foreach (var p in _spawned)
      {
        Gizmos.DrawSphere(p.transform.position, .25f);
      }
    }
  }
}