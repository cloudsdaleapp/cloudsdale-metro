namespace CloudsdaleLib.Models {
    public interface IMergable {
        void Merge(CloudsdaleModel other);
        bool CanMerge(CloudsdaleModel other);
    }
}
