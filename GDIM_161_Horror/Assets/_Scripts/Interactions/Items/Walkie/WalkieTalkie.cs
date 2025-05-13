using Dissonance;
using Interactions;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class WalkieTalkie : NetworkPickableItem
{
    [Header("FMOD Events (2D)")]
    [SerializeField] private EventReference walkieStart;   // one-shot “press” sound
    [SerializeField] private EventReference walkieEnd;     // one-shot “release” sound
    [SerializeField] private EventReference radioStatic;   // looping static (2D)

    private EventInstance radioStaticInstance;
    private bool isLoopPlaying = false;
    private bool isPossessed = false;

    protected override void PickItem(int playerId)
    {
        base.PickItem(playerId);

        isPossessed = true;

        // Add your Dissonance token
        var local = FindFirstObjectByType<DissonanceComms>();
        local.AddToken("AddWalkie");

        // Create the 2D FMOD loop instance—but don't start it yet
        radioStaticInstance = RuntimeManager.CreateInstance(radioStatic);
        // No set3DAttributes() call here
        isLoopPlaying = false;
    }

    public override void UnPossessItem(Vector3 throwDir, int playerID)
    {
        base.UnPossessItem(throwDir, playerID);

        isPossessed = false;

        var local = FindFirstObjectByType<DissonanceComms>();
        local.RemoveToken("AddWalkie");

        if (radioStaticInstance.isValid())
        {
            // Immediately stop and release your 2D loop
            radioStaticInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            radioStaticInstance.release();
        }

        isLoopPlaying = false;
    }

    private void Update()
    {
        if (!isPossessed)
            return;

        // Start talking
        if (Input.GetKeyDown(KeyCode.V))
        {
            // 2D one-shot
            RuntimeManager.PlayOneShot(walkieStart);

            if (!isLoopPlaying)
            {
                radioStaticInstance.start();
                isLoopPlaying = true;
            }
        }

        // Stop talking
        if (Input.GetKeyUp(KeyCode.V))
        {
            RuntimeManager.PlayOneShot(walkieEnd);

            if (isLoopPlaying)
            {
                radioStaticInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                isLoopPlaying = false;
            }
        }
    }
}
