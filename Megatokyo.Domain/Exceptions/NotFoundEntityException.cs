namespace Megatokyo.Domain.Exceptions
{
    public class NotFoundEntityException(string entityname, object entityid) : Exception($"{entityname} with id [{entityid}] not found.")
    {
    }
}
