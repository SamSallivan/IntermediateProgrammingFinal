using System.Collections;
using System.Collections.Generic; 
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerDecapitate : MonoBehaviour
{
	private PlayerController player;

	private Transform t;

	private Rigidbody rb;


	private void Awake()
	{
		t = base.transform;
		rb = GetComponent<Rigidbody>();
		player = GetComponentInParent<PlayerController>();
	}

    void Update()
    {
        if(Input.GetKey(KeyCode.R)){
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }
    }

    public void Decapitate(Transform pos, Vector3 dir){
        if ((bool)transform.parent)
		{
			transform.SetParent(null);
		}
		transform.SetPositionAndRotation(pos.position, pos.rotation);
		rb.AddForce(dir * 5f, ForceMode.Impulse);
		rb.AddTorque(Vector3.one * 10f, ForceMode.Impulse);

		// PostProcessVolume volume = FindObjectOfType<PostProcessVolume>();
		// Bloom bloom;
		// ChromaticAberration ca;
		// ColorGrading cg;

		// volume.profile.TryGetSettings(out bloom);
		// volume.profile.TryGetSettings(out ca);
		// volume.profile.TryGetSettings(out cg);

		// bloom.intensity.value = 10;
		// ca.intensity.value = 1;
		// cg.mixerGreenOutRedIn.value = -150;
    }
}
