namespace CloudsdaleLib.Models {
    /// <summary>
    /// A model which must do some pre-processing before it is added to a cache
    /// </summary>
    public interface IPreProcessable {
        /// <summary>
        /// Notify the model that it should perform its pre-processing
        /// </summary>
        void PreProcess();
    }
}
