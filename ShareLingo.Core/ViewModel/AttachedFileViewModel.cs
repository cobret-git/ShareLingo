namespace ShareLingo.Core.ViewModel
{
    public class AttachedFileViewModel : IDisposable
    {
        #region Constructors
        public AttachedFileViewModel(string name, Stream stream)
        {
            Name = name;
            Stream = stream;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The file's name with extension.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The file's read-only stream.
        /// </summary>
        public Stream Stream { get; }
        #endregion

        #region Methods
        public void Dispose()
        {
            if (Stream != null) Stream.Dispose();
        }
        #endregion
    }
}
