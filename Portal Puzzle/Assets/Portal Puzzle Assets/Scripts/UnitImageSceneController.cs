using UnityEngine;
using UnityEngine.UI;
using MadLevelManager;
using System.Collections;

public class UnitImageSceneController : MonoBehaviour {

    public Texture2D testTexture;

    public GameObject[] sampleUnitsContainer;
    public Button[] editBtnContainer;

    private int currentIndex;

    void Start() 
    {
        // Change Image of all sample Unit on screen into Unit Image from UnitImageController
        for (int i = 0; i < sampleUnitsContainer.Length; i++)
        {
            SpriteRenderer sampleUnitSpriteRend = sampleUnitsContainer[i].GetComponent<SpriteRenderer>();
            SpriteRenderer targetSpriteRend = UnitImageController.Instance.UnitsPrefabsContainer[i].GetComponent<SpriteRenderer>();
            sampleUnitSpriteRend.sprite = targetSpriteRend.sprite;
        }
    }

    public void toMainGameScene()
    {
        //Application.LoadLevel("_MainGameScene");
        //MadLevel.LoadNext(MadLevel.Type.Level);
        MadLevel.Continue();
    }

    public void onBtnEditImageClicked(int value)
    {
        currentIndex = value;
        UM_Camera.instance.OnImagePicked += OnImage;
        UM_Camera.instance.GetImageFromGallery();
    }

    public void OnImage(UM_ImagePickResult result)
    {

        if (result.IsSucceeded)
        {          
            // Get the result texture from gallery
            Texture2D resultTexture = result.image;
            // Get the texture of the choosen Unit
            SpriteRenderer choosenUnitSpriteRend = UnitImageController.Instance.UnitsPrefabsContainer[currentIndex].GetComponent<SpriteRenderer>();

            // Change Image for the choosen Unit in Container based on the size of the result texture
            if (resultTexture.width >= resultTexture.height)
            {
                float startX = ((float)(resultTexture.width - resultTexture.height)) / 2;
                float startY = 0.0f;
                float pixelsPerUnit = ((float)resultTexture.height) / 0.8f;
                //new MobileNativeMessage("Notice", "TestTexture Height :" + resultTexture.height + " Pixel per Unit :" + pixelsPerUnit);
                choosenUnitSpriteRend.sprite = Sprite.Create(resultTexture, 
                                                    new Rect(startX, startY, resultTexture.height, resultTexture.height), 
                                                    new Vector2(0, 0), 
                                                    pixelsPerUnit);
            }
            else
            {
                float startX = 0.0f;
                float startY = ((float)(resultTexture.height - resultTexture.width)) / 2;
                float pixelsPerUnit = ((float)resultTexture.width) / 0.8f;
                //new MobileNativeMessage("Notice", "TestTexture Width :" + resultTexture.width + " Pixel per Unit :" + pixelsPerUnit);
                choosenUnitSpriteRend.sprite = Sprite.Create(resultTexture, 
                                                    new Rect(startX, startY, resultTexture.width, resultTexture.width), 
                                                    new Vector2(0, 0), 
                                                    pixelsPerUnit);
            }

            // Change the Image of Sample Unit on screen
            SpriteRenderer testObjectSpriteRend = sampleUnitsContainer[currentIndex].GetComponent<SpriteRenderer>();
            Texture2D targetTexture = UnitImageController.Instance.UnitsPrefabsContainer[currentIndex].GetComponent<SpriteRenderer>().sprite.texture;
            testObjectSpriteRend.sprite = UnitImageController.Instance.UnitsPrefabsContainer[currentIndex].GetComponent<SpriteRenderer>().sprite;
            //unit1Sprite.sprite = Sprite.Create(resultTexture, new Rect(0, 0, resultTexture.width, resultTexture.height), new Vector2(0, 0), 100.0f);

        }
        UM_Camera.instance.OnImagePicked -= OnImage;
    }
}
