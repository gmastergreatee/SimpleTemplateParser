namespace SimpleTokenParser.Models
{
    public interface ITokenParserModel<T> where T : class
    {
        string ApplyModel(T model);
    }
}
