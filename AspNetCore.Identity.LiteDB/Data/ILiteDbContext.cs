using LiteDB;

namespace AspNetCore.Identity.LiteDB.Data
{
   public interface ILiteDbContext
   {
      ILiteDatabase LiteDatabase { get; }
   }
}