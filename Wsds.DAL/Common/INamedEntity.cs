namespace Wsds.DAL.Common
{
    interface INamedEntity
    {
        string NAME { get; set; }
        string GetLocalizedName(int localId);
    }
}
