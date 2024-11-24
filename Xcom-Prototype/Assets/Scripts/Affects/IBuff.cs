
public interface IBuff
{
    public BaseCharacerView Owner { get; set; }
    public BuffsEnum buffType { get; set; }
    public void Apply(BaseCharacerView target);
    public void Remove(BaseCharacerView target);
    public void Tick();

}
