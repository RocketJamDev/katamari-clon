using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariManager : MonoBehaviour
{
    public GameObject debugCube;
    public GameObject debugSphere;
    public AudioClip[] stickySounds;
    public AudioClip[] impactSounds;

    public float volumeFactor = 3.0f;
    public float thresholdGrowthFactor = 0.66f;
    public float growFactor = 0.6f;
    public float stunTime;
    public float forceImpulse = 10000.0f;
    public float torqueImpulse = 1000.0f;
    public float forceObjectThrow = 300.0f;
    public float torqueObjectThrow = 100.0f;
    public int minNumItems;
    public int maxNumItems;
    bool played = false;

    private double katamariVolume = 0;
    private float growthSphereRadius = 0;
    private double growthSphereVolume = 0;
    private double itemsSumVolume = 0;
    private Rigidbody sphereRb;
    private SphereCollider sphereCollider;
    private AudioSource audioSource;
    private ParticleSystem particleSys;
    

    private void Start()
    {
        // Cache components.
        sphereCollider = GetComponent<SphereCollider>();
        sphereRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        particleSys = GetComponent<ParticleSystem>();

        katamariVolume = SphereVolume(sphereCollider.radius);
        growthSphereRadius = GetBoundSphere();
        growthSphereVolume = SphereVolume(growthSphereRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject item = collision.gameObject;

        if (!item.CompareTag("notStickable")) 
        {
            ItemManager itemManager = item.GetComponent<ItemManager>();

            double itemVolume = itemManager.Volume;
            double thresholdVolume = itemVolume * volumeFactor;

            if (katamariVolume >= thresholdVolume)
            {
                ManageStick(item);
                AudioClip clip = stickySounds[UnityEngine.Random.Range(0, stickySounds.Length - 1)];
                audioSource.PlayOneShot(clip);
                ManageGrowth(itemVolume);
            }
            else
            {
                ManageImpact(collision);
            }
        }
    }

    private void ManageStick(GameObject item)
    {
        item.transform.parent = transform;

        Destroy(item.GetComponent<Rigidbody>());
        Destroy(item.GetComponent<BoxCollider>());
    }

    private void ManageGrowth(double itemVolume)
    {
        itemsSumVolume += itemVolume;

        float newBoundSphere = GetBoundSphere();
        growthSphereRadius = newBoundSphere;
        growthSphereVolume = SphereVolume(growthSphereRadius);

        double outerRingVolume = growthSphereVolume - katamariVolume;
                
        if (itemsSumVolume >= outerRingVolume * thresholdGrowthFactor)
        {
            Grow();
            Debug.Log("Grow");
        }
    }

    private void Grow()
    {
        // Reset Counter
        itemsSumVolume = 0;

        // Grow Katamari
        Bounds bounds = GetBounds();
        sphereCollider.radius = growthSphereRadius * growFactor;
        katamariVolume = SphereVolume(sphereCollider.radius);
    }

    private void ManageImpact(Collision collision)
    {
        ThrowItems();
        
        // Play sounds and Particles
        if(played == false)
        {
            AudioClip clip = impactSounds[UnityEngine.Random.Range(0, stickySounds.Length - 1)];
            audioSource.PlayOneShot(clip);
            played = true;
        }

        // Add force and torque to the katamari.
        Vector3 impulseDir = collision.contacts[0].point - transform.position;
        impulseDir = -impulseDir.normalized;
        sphereRb.Sleep();
        sphereRb.AddForce(impulseDir * forceImpulse);
        sphereRb.AddTorque(new Vector3(torqueImpulse, torqueImpulse, torqueImpulse));

        // Stop katamari torque rotation after 1 second.
        Invoke("StopRotation", 0.5f);
    }
    
    private void ThrowItems()
    {
        int numItemsToThrow = UnityEngine.Random.Range(minNumItems, maxNumItems);

        if (numItemsToThrow > transform.childCount)
        {
            numItemsToThrow = transform.childCount;
        }

        for (int i = transform.childCount - 1; i >= transform.childCount - numItemsToThrow; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            // Calculates random position for X and Y.
            float xPos = UnityEngine.Random.Range(0.0f, 0.25f);
            float yPos = UnityEngine.Random.Range(0.0f, 0.25f);

            Vector3 spawnPoint = transform.position + new Vector3(xPos, sphereCollider.radius, yPos);
            
            GameObject throwedObject = Instantiate(child, spawnPoint, Quaternion.identity);
            Rigidbody trowedObjectRb = throwedObject.AddComponent<Rigidbody>();
            
            // Calculates random direction for X and Z.
            float xDir = UnityEngine.Random.Range(0, 70) / 100.0f;
            float zDir = UnityEngine.Random.Range(0, 70) / 100.0f;

            // Add force and torque to throwed object.
            Vector3 throwDir = new Vector3(xDir, 1, zDir);
            trowedObjectRb.AddForce(forceObjectThrow * throwDir);
            trowedObjectRb.AddTorque(new Vector3(torqueObjectThrow, torqueObjectThrow, torqueObjectThrow));
            
            // Destroy both objects
            Destroy(child);
            Destroy(throwedObject, 1.0f);
        }
    }

    private void StopRotation()
    {
        sphereRb.angularVelocity = Vector3.zero;
        played = false;
    }

    float GetBoundSphere()
    {
        Bounds b = GetBounds();
        return Math.Min(Math.Min(b.size.x, b.size.y), b.size.z) / 2;
    }

    Bounds GetBounds()
    {
        Bounds combinedBounds = GetComponent<Renderer>().bounds;
        
        foreach(var r in GetComponentsInChildren<Renderer>())
        {
            if (!r.gameObject.CompareTag("notmesh"))
            {
                combinedBounds.Encapsulate(r.bounds);
            }
        }
       
        return combinedBounds;
    }

    private double SphereVolume(float radius)
    {
        double r = Convert.ToDouble(radius);
        double scaledRadius = r * transform.localScale.x;
        return 4 * Math.PI * Math.Pow(scaledRadius, 3) / 3;
    }
}   
