namespace Megatokyo.Domain.Exceptions
{
    public class UpdateEntityException(string entityname, object entityid) : Exception($"{entityname} with id [{entityid}] could not be updated.")
    {
    }
}
