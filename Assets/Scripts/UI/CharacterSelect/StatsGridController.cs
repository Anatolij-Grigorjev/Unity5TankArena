using System;
using System.Collections;
using System.Collections.Generic;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI.Characters
{
    public class StatsGridController : MonoBehaviour
    {

        public Image[] atkCells;
        public Image[] movCells;
        public Image[] regCells;
		private int ATK, MOV, REG;

        // Use this for initialization
        void Start()
        {
			ATK = MOV = REG = 0;
        }

		public void SetStats(CharacterStats stats)
		{
			ATK = stats.ATK;
			MOV = stats.MOV;
			REG = stats.REG;

			RefreshUI();
		}

        // Update is called once per frame
        private void RefreshUI()
        {
            //redo stats
            atkCells.ForEachWithIndex(clearCellAction);
            movCells.ForEachWithIndex(clearCellAction);
            regCells.ForEachWithIndex(clearCellAction);
            //APPLY STATS
            fillCellsWithColor(atkCells, Color.red, ATK);
            fillCellsWithColor(movCells, Color.blue, MOV);
            fillCellsWithColor(regCells, Color.green, REG);
        }
		private Action<Image, int> clearCellAction = (cell, index) => { cell.color = Color.black; };
		private void fillCellsWithColor(Image[] array, Color fill, int n) 
		{
			for (int i = 0; i < n; i++)
			{
				array[i].color = fill;
			}
		}
    }

}
