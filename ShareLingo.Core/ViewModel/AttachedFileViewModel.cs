using NetForge.Core;

namespace ShareLingo.Core.ViewModel
{
    public class AttachedFileViewModel : IDisposable, ICloneable<AttachedFileViewModel>, IMergable<AttachedFileViewModel>
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
        public AttachedFileViewModel Clone()
        {
            // Create a new stream from the original stream
            Stream clonedStream = new MemoryStream();
            Stream.CopyTo(clonedStream);
            clonedStream.Position = 0; // Reset position for reading
            return new AttachedFileViewModel(Name, clonedStream);
        }
        public void Merge(AttachedFileViewModel other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other.Name != Name) throw new ArgumentException("Cannot merge files with different names.");
            Stream.SetLength(0);
            other.Stream.Position = 0; // Ensure the other stream is at the beginning
            other.Stream.CopyTo(Stream);
            Stream.Position = 0; // Reset position for reading
        }
        public void Dispose()
        {
            if (Stream != null) Stream.Dispose();
        }
        #endregion
    }
}
