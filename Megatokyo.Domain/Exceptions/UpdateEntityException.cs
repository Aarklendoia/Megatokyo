namespace Megatokyo.Domain.Exceptions
{
    public class UpdateEntityException : Exception
    {

        public UpdateEntityException(string entityname, object entityid)
            : base($"{entityname} with id [{entityid}] could not be updated.")
        {

        }
    }
}
