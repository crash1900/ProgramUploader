//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProgramUploader
{
    using System;
    using System.Collections.Generic;
    
    public partial class Movement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Movement()
        {
            this.ProgramHeaderMovements = new HashSet<ProgramHeaderMovement>();
        }
    
        public int Id { get; set; }
        public string MovementName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProgramHeaderMovement> ProgramHeaderMovements { get; set; }
    }
}
