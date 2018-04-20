namespace Business
{
    /// <summary>
    /// Serialization and deserialization interface.
    /// </summary>
    public interface ISerializer
    {
        // get extension used in files
        string GetExtension { get; }

        // serialize object
        string Serialize(QuestionsSet questionSet);

        // deserialize object from provided data
        QuestionsSet Deserialize(string data);

    }
}
