using Assets.Scripts.Core.Services.Inventory;
using Assets.Utils.Inventory;
using Assets.ViewModels.Inventory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.ViewModels
{
    public static class EnemyData
    {
        public static List<Enemy> AllEnemies { get; private set; } = new();

        public static async Task InitializeAsync()
        {
            AllEnemies = new List<Enemy>
            {
                await CreateEnemyAsync(1, "Esfera", new List<int>() { 0, 1, 4, 6}),
                await CreateEnemyAsync(2, "Cubo", new List<int>() { 8, 9, 4, 15 })
            };
        }

        public async static Task<Enemy> GetEnemyById(int id)
        {   
            await InitializeAsync();

            return AllEnemies.FirstOrDefault(e => e.Id == id);
        }

        private static async Task<Enemy> CreateEnemyAsync(int id, string name, List<int> itemIds)
        {
            var drops = new List<Enemy.AvailableDrop>();
            var items = await ItemService.GetItems(itemIds);

            foreach (var itemId in itemIds)
            {
                var item = items.FirstOrDefault(i => i.id == itemId);
                if (item != null)
                {
                    drops.Add(new Enemy.AvailableDrop
                    {
                        DropItem = item
                    });
                }
            }

            return new Enemy
            {
                Id = id,
                Name = name,
                Drops = drops
            };
        }
    }

    public class Enemy
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<AvailableDrop> Drops { get; set; }

        public class AvailableDrop
        {
            public float Percentage { get; set; }
            public Item DropItem { get; set; }
        }
    }
}