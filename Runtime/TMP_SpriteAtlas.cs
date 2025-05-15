using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
using UnityEngine.TextCore;

namespace TMPro_Extension
{
    /// <summary>
    /// TMP_SpriteAtlas is a subclass of TMP_SpriteAsset that allows for the use of Unity's SpriteAtlas.
    /// It provides methods to update the sprite asset from the atlas and retrieve sprites by name or index.
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteAtlas", menuName = "TextMeshProExtend/SpriteAtlas")]
    public class TMP_SpriteAtlas : TMP_SpriteAsset
    {
        [SerializeField] private SpriteAtlas atlas;

        public SpriteAtlas Atlas => atlas;

        /// <summary>
        /// Updates the TMP_SpriteAsset with sprites from the assigned SpriteAtlas
        /// </summary>
        public void UpdateSpriteAssetFromAtlas()
        {
            if (atlas == null)
            {
                Debug.LogError("SpriteAtlas is not assigned");
                return;
            }

            // Get all sprites from the atlas
            List<Sprite> sprites = new List<Sprite>();
            Sprite[] spriteArray = new Sprite[atlas.spriteCount];
            atlas.GetSprites(spriteArray);
            sprites.AddRange(spriteArray);

            if (sprites.Count == 0)
            {
                Debug.LogError("No sprites found in the SpriteAtlas");
                return;
            }

            // Clear existing data
            spriteGlyphTable.Clear();
            spriteCharacterTable.Clear();

            // Get the texture from the first sprite's texture
            Texture mainTexture = sprites[0].texture;
            spriteSheet = mainTexture;

            if (material == null)
            {
                material = new Material(Shader.Find("TextMeshPro/Sprite"));
            }
            material.SetTexture(ShaderUtilities.ID_MainTex, mainTexture);

            // Add sprites to the asset
            uint glyphIndex = 0;

            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite sprite = sprites[i];
                if (sprite == null) continue;

                // Create sprite glyph
                TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph();
                spriteGlyph.index = glyphIndex;

                // Get sprite rect relative to texture
                Rect rect = sprite.rect;
                spriteGlyph.glyphRect = new GlyphRect(
                    (int)rect.x,
                    (int)rect.y,
                    (int)rect.width,
                    (int)rect.height);

                // Set glyph metrics
                float pixelsPerUnit = sprite.pixelsPerUnit;
                spriteGlyph.metrics = new GlyphMetrics(
                    rect.width / pixelsPerUnit,
                    rect.height / pixelsPerUnit,
                    sprite.pivot.x / pixelsPerUnit - (rect.width / pixelsPerUnit / 2),
                    sprite.pivot.y / pixelsPerUnit,
                    rect.width / pixelsPerUnit);

                spriteGlyph.scale = 1.0f;
                spriteGlyph.atlasIndex = 0;

                // Create sprite character
                TMP_SpriteCharacter spriteCharacter = new TMP_SpriteCharacter();
                spriteCharacter.glyph = spriteGlyph;

                // Use hash of name for unicode to ensure uniqueness
                uint unicode = (uint)sprite.name.GetHashCode();
                spriteCharacter.unicode = unicode;
                spriteCharacter.name = sprite.name;
                spriteCharacter.scale = 1.0f;
                spriteCharacter.glyphIndex = glyphIndex;

                // Add to the sprite asset's tables
                spriteGlyphTable.Add(spriteGlyph);
                spriteCharacterTable.Add(spriteCharacter);

                glyphIndex++;
            }

            // Update lookup tables
            UpdateLookupTables();

            // We should also have a spriteInfoList for backward compatibility
            if (spriteInfoList == null)
                spriteInfoList = new List<TMP_Sprite>();
            else
                spriteInfoList.Clear();

            // Populate spriteInfoList (old format)
            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite sprite = sprites[i];
                if (sprite == null) continue;

                TMP_Sprite tmpSprite = new TMP_Sprite();
                tmpSprite.name = sprite.name;
                tmpSprite.unicode = (int)spriteCharacterTable[i].unicode;
                tmpSprite.x = sprite.rect.x;
                tmpSprite.y = sprite.rect.y;
                tmpSprite.width = sprite.rect.width;
                tmpSprite.height = sprite.rect.height;
                tmpSprite.xOffset = spriteGlyphTable[i].metrics.horizontalBearingX;
                tmpSprite.yOffset = spriteGlyphTable[i].metrics.horizontalBearingY;
                tmpSprite.xAdvance = spriteGlyphTable[i].metrics.horizontalAdvance;
                tmpSprite.scale = 1.0f;
                tmpSprite.sprite = sprite;

                spriteInfoList.Add(tmpSprite);
            }
        }

        public Sprite GetSpriteByName(string name)
        {
            if (atlas == null) return null;

            Sprite sprite = atlas.GetSprite(name);
            return sprite;
        }

        public Sprite GetSpriteByIndex(int index)
        {
            if (spriteInfoList == null || index < 0 || index >= spriteInfoList.Count)
                return null;

            return spriteInfoList[index].sprite;
        }
    }
}