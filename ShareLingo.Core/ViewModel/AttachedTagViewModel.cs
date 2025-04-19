using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model;

namespace ShareLingo.Core.ViewModel
{
    public class AttachedTagViewModel : ObservableObject
    {
        #region Constructors
        public AttachedTagViewModel(string tagName, ExerciseSubItemViewModelBase exerciseItem)
        {
            Tag = new() { Name = tagName };
            TagRelation = new() { ExerciseItem = exerciseItem.Item, RelatedTag = Tag };
            ExerciseItem = exerciseItem;
        }
        public AttachedTagViewModel(Tag tag, TagRelation tagRelation, ExerciseSubItemViewModelBase exerciseItem)
        {
            Tag = tag;
            TagRelation = tagRelation;
            ExerciseItem = exerciseItem;
        }
        #endregion

        #region Properties
        public Tag Tag { get; }
        public TagRelation TagRelation { get; }
        public ExerciseSubItemViewModelBase ExerciseItem { get; }
        #endregion
    }
}
