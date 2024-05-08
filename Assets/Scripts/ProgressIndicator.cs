using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour
{
    public Transform player; // the player
    public float enableDistance = 5; // distance to grab screwdriver

    private PlayerController playerVars; // access to PlayerController variables
    [SerializeField] private float duration = 3.0f;
    [SerializeField] private Image progressIndicator;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerVars = player.GetComponent<PlayerController>();
        playerVars.screwdriver.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= enableDistance) {
            playerVars.canCollectScrewdriver = true;
            if (Input.GetKey(KeyCode.F)) {
                timer += Time.deltaTime;
                progressIndicator.enabled = true;
                progressIndicator.fillAmount = timer / duration;
                if (progressIndicator.fillAmount >= 1) {
                    playerVars.hasScrewdriver = true;
                    playerVars.screwdriver.enabled = true;
                    Destroy(gameObject);
                }
            } else {
                timer = 0;
                progressIndicator.enabled = false;
                progressIndicator.fillAmount = 0;
            }
        } else {
            playerVars.canCollectScrewdriver = false;
            timer = 0;
            progressIndicator.enabled = false;
            progressIndicator.fillAmount = 0;
        }        
    }
}
