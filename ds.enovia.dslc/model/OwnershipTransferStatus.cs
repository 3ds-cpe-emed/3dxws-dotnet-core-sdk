using System;
using System.Collections.Generic;
using System.Text;

namespace ds.enovia.dslc.model
{
   public class OwnershipTransferStatus : IOwnershipTransferStatus
   {
      public int status      { get ; set ; }
      
      public string owner        { get ; set ; }
      public string organization { get ; set ; }
      public string collabspace  { get ; set ; }
      
      public string id           { get ; set ; }
   }
}
