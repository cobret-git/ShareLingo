using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace ShareLingo.Core.Model
{
    public class TagRelation : ObservableObject
    {
        #region Consts
        public const string COLLECTION_NAME = "TagRelations";
        #endregion

        #region Fields
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        [BsonRef(Tag.COLLECTION_NAME)] public Tag RelatedTag { get; set; } = null!;
        [BsonRef(ExerciseBase.COLLECTION_NAME)] public ExerciseBase Exercise { get; set; } = null!;
        [BsonRef(ExerciseSubItemBase.COLLECTION_NAME)] public ExerciseSubItemBase ExerciseItem { get; set; } = null!;
        #endregion
    }
}
