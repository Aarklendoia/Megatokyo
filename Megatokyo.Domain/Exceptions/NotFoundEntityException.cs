namespace Megatokyo.Domain.Exceptions
{
    public class NotFoundEntityException : Exception
    {

        public NotFoundEntityException(string entityname, object entityid)
            : base($"{entityname} with id [{entityid}] not found.")
        {

        }
    }
}
