namespace SharpPusher.Services
{
    public class Response<T>
    {
        private readonly ErrorCollection errors = new ErrorCollection();

        public ErrorCollection Errors
        {
            get { return errors; }
        }

        public T Result { get; set; }
    }
}
