namespace ShareLingo.WinUI.ViewModel.Component
{
    public struct MediaData
    {
        #region Properties
        /// <summary>
        /// The absolute file path to the media file located in local storage.
        /// </summary>
        public string AbsolutePath { get; init; }

        /// <summary>
        /// The relative file path to the media file located in local strorage that starts with media directory.
        /// </summary>
        public string RelativePath { get; init; }
        #endregion
    }
}
