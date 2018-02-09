namespace Wsds.DAL.Common
{
    interface ISequencedEntity
    {
        long Id { get; set; }
        long GetNextSeq();
    }
}
