namespace UKParliament.CodeTest.Services.Exceptions
{
    //[Serializable]
    public class DuplicateException : Exception
    {
        public DuplicateException()
        {
        }

        public DuplicateException(string? message) : base(message)
        {
        }
    }
}