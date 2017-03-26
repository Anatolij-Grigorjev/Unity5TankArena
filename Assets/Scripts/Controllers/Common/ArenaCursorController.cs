using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Controllers
{
    public class ArenaCursorController : MonoBehaviour
    {


        public Color neutralColor;
        public Color enemyColor;
        public SpriteRenderer spriteRenderer;
        // Use this for initialization
        private RaycastHit2D[] allocHits;
        void Start()
        {
            //hide cursor
            Cursor.visible = false;
            spriteRenderer.color = neutralColor;
            allocHits = new RaycastHit2D[1];
        }

        void Update()
        {
            //match cursor position
            var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(newPos.x, newPos.y, 11.0f);

            //check if enemy under cursor
            Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hits = Physics2D.CircleCastNonAlloc(origin, 10.0f, Vector2.zero, allocHits, 0.0f, LayerMasks.LM_ENEMY);

            // set sprite color if hovering over enemy
            spriteRenderer.color = (hits > 0 && allocHits[0].collider != null && allocHits[0].collider.gameObject != null) ? enemyColor : neutralColor;

        }
    }

}
