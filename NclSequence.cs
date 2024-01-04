public class NclSequence
{
    public string Name { get; set; }
    public string FeatureNumber { get; set; }
    public int LineCount => Lines.Count;
    public bool HasToolCall { get; set; }
    public List<NclItemBase> Lines { get; set; }

    public NclSequence()
    {
        Lines = new List<NclItemBase>();
    }
}
