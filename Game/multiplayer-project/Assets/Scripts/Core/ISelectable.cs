public interface ISelectable
{
    void Select(ulong selector);
    void Unselect();
    bool IsSelected();
}
