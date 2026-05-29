using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class PropsAltar : MonoBehaviour
    {
        public List<SpriteRenderer> runes;
        public float lerpSpeed;

        public bool IsLit { get; private set; } = false;

        private Color curColor;
        private Color targetColor;

        private void Awake()
        {
            targetColor = runes[0].color;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            targetColor.a = 1.0f;
            IsLit = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            targetColor.a = 0.0f;
            IsLit = false;
        }

        private void Update()
        {
            curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);
            foreach (var r in runes)
                r.color = curColor;
        }
    }
}