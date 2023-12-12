namespace Pronia.Utilities.Exceptions
{
    public class WrongRequestException:Exception
    {
        public WrongRequestException(string message="Wrong request"):base(message)
        {

        }
    }
}
