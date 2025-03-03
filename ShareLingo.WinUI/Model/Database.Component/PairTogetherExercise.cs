namespace ShareLingo.WinUI.Model.Database.Component
{
    public class PairTogetherExercise
    {
        public string Descriptin { get; set; } = string.Empty;
        public PairTogetherSubItem[] Items { get; set; } = Array.Empty<PairTogetherSubItem>();
    }

    public struct PairTogetherSubItem
    {
        public PairTogetherSubItem()
        {
            Id = default;
            LeftItem = string.Empty;
            RightItem = string.Empty;
            Target = PairTogetherSubItemTarget.Left;
        }
        public int Id { get; set; }
        public string LeftItem { get; set; }
        public string RightItem { get; set; }
        public PairTogetherSubItemTarget Target { get; set; }
    }
    public enum PairTogetherSubItemTarget
    {
        Left, Right
    }
}
