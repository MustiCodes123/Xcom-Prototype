public class ArtifactRecipeItem : BaseItem
{
    public RecipeData RecipeData { get; set; }
    public ItemTemplate TargetItem { get; set; }
    public int Price { get; set; }
    public GameCurrencies Currency { get; set; }
}