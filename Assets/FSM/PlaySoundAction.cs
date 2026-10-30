using UnityEngine;

// plays a one-off sound on the controller's AudioSource, e.g. as a state's setup or exit action
[CreateAssetMenu(menuName = "PluggableSM/Actions/PlaySound")]
public class PlaySoundAction : Action
{
    public AudioClip clip;

    public override void Act(StateController controller)
    {
        AudioSource source = controller.GetComponent<AudioSource>();
        if (source != null && clip != null) source.PlayOneShot(clip);
    }
}
