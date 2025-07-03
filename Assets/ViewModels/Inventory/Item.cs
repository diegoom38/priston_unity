using Assets.Enums;
using System;


namespace Assets.ViewModels.Inventory
{
	[Serializable]
	public class Item
	{
		public int id;
		public bool equipavel;
		public string recursoNomeItem;
		public string recursoNomePrefab;
		public string nome;
		public InventorySlotType slotTipo;
	}
}
