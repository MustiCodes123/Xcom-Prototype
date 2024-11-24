using System.Threading.Tasks;
using UnityEngine;

public interface IStorageItemData
{
   public Task<Sprite> GetItemIcon();
}
