using UnityEngine;
using UnionAssets.FLE;
using System.Collections;

public class WP8ProductTemplate : EventDispatcherBase {


	private Texture2D _texture;

	public string ImgURL { get; set; }
	public string Name { get; set; }
	public string ProductId { get; set; }
	public string Price { get; set; }
	public WP8PurchaseProductType Type { get; set; }
	public string Description { get; set; }
	public bool isPurchased { get; set; }

	public const string PRODUCT_IMAGE_LOADED = "product_image_loaded";


	public void LoadProductImage() {
		
		if(_texture != null) {
			dispatch(PRODUCT_IMAGE_LOADED);
			return;
		}
		
		
		WPN_TextureLoader loader = WPN_TextureLoader.Create();
		loader.addEventListener(BaseEvent.LOADED, OnTextureLoad);
		loader.LoadTexture(ImgURL);
	}


	public Texture2D texture {
		get {
			return _texture;
		}
	}


	private void OnTextureLoad(CEvent e) {
		e.dispatcher.removeEventListener(BaseEvent.LOADED, OnTextureLoad);
		_texture = e.data as Texture2D;
		
		dispatch(PRODUCT_IMAGE_LOADED);
	}

}
