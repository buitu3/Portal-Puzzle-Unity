using UnityEngine;
using System.Collections;

public class UnitImageController : MonoBehaviour {

    public static UnitImageController Instance;

    public GameObject[] UnitsOriginalPrefabsContainer;
    public GameObject[] UnitsPrefabsContainer;

    public Texture2D testTexture;

    void Awake()
    {
        // Check if this is the First created Instance
        // Destroy it if not
        if (Instance == null)
        {
            Instance = this;

            // Copy all original Unit Image into current play Unit Image container
            for (int i = 0; i < UnitsPrefabsContainer.Length; i++)
            {
                SpriteRenderer originalUnitSprite = UnitsOriginalPrefabsContainer[i].GetComponent<SpriteRenderer>();
                Texture2D originalTexture = originalUnitSprite.sprite.texture;
                SpriteRenderer unitSprite = UnitsPrefabsContainer[i].GetComponent<SpriteRenderer>();
                float pixelsPerUnit = originalUnitSprite.sprite.pixelsPerUnit;

                unitSprite.sprite = Sprite.Create(originalTexture, new Rect(0, 0, originalTexture.width, originalTexture.height), new Vector2(0, 0), pixelsPerUnit);
            }
        }

        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    //print("destroy");
        //}

        else if (Instance != this)
        {
            Destroy(gameObject);
            //print("destroy");
        }

        DontDestroyOnLoad(gameObject);
    }
}
