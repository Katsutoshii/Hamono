using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{


    private TextTyper storyText;
    [TextArea(3, 10)]
    public string[] text;
    public Sprite[] sprites;

    public float readingTime;

    private int scriptIndex;
    private int imageIndex;

    private Image storyImage;

    public bool completedSpeech;
    private bool startedSpeech;
    // private bool dialogStarted;
    private bool clicked;

    private Animator anim;
    private bool fade;
    private float animationWait = 1f;

    private GameObject fadeToBlackEffect;
    private GameObject fadeToBlackEffectScreen;
    private AudioSource musicAudio;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        fadeToBlackEffect = GameObject.Find("FadeToBlack");
        fadeToBlackEffectScreen = GameObject.Find("FadeToBlack_Screen");
        fadeToBlackEffect.SetActive(false);
        fadeToBlackEffectScreen.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {

        completedSpeech = false;
        startedSpeech = false;

        storyImage = GameObject.Find("StoryImage").GetComponent<Image>();
        anim = fadeToBlackEffect.transform.GetChild(0).GetComponent<Animator>();
        musicAudio = GameObject.Find("Music Player").GetComponent<AudioSource>();
        storyText = GameObject.Find("StoryText").GetComponent<TextTyper>();

        StartStory();
    }

    // Update is called once per frame
    void Update()
    {
        ClearScript();
        if (Input.GetMouseButtonDown(0) && scriptIndex < text.Length) {
            clicked = true;
        }
    }

    // if player doesn't click, the next script will automatically start
    IEnumerator AutoNextScript()
    {
        if (!clicked) {
            StartStory();
        } else {
            NextSection();
        }
        yield return new WaitForSeconds(readingTime);
    }

    // clears the current script
    private void ClearScript() {
        if (completedSpeech) {
            // clearing script
            // dialogStarted = false;
            completedSpeech = false;
            startedSpeech = false;
            StartCoroutine(AutoNextScript());
        } else if (clicked) {
            clicked = false;
            StartCoroutine(AutoNextScript());
        }
    }

    // Flips to the next image and text
    void NextSection() {
        StartCoroutine(NextImage());
        
        animationWait = 2f;
    }

    // moves to the next script
    void StartStory()
    {
        // dialogStarted = true;
        Debug.Log("starting story");
        Debug.Log("completed speech?: " + completedSpeech);
        anim.speed = 0.3f;
        if (!startedSpeech && !completedSpeech && scriptIndex < text.Length)
        {
            NextSection();
        }
        else if (startedSpeech && !completedSpeech)
        {
            animationWait = 0f;
            storyText.Skip();
        }
        else if (scriptIndex >= text.Length)
        {
            anim = fadeToBlackEffectScreen.transform.GetChild(0).GetComponent<Animator>();
            fadeToBlackEffectScreen.SetActive(true);
            StartCoroutine(MoveToNextScene());
        }
    }

    // moves to the next image
    IEnumerator NextImage()
    {
        if (imageIndex < sprites.Length)
        {
            yield return new WaitForSeconds(animationWait);
            storyText.TypeText(text[scriptIndex]);
            completedSpeech = false;
            startedSpeech = true;
            scriptIndex++;

            fadeToBlackEffect.SetActive(true);
            yield return new WaitForSeconds(0f);


            storyImage.sprite = sprites[imageIndex];
            imageIndex++;

            yield return new WaitForSeconds(3f);
            fadeToBlackEffect.SetActive(false);
        }
    }

    // starts entire screen fade and moves to next scene
    IEnumerator MoveToNextScene()
    {
        while (musicAudio.volume > 0)
        {
            musicAudio.volume = musicAudio.volume - .01f;
            yield return new WaitForSeconds(.1f);
        }
        SceneManager.LoadScene(2); // takes player to first level
    }
}
