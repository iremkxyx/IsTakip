//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IsTakip.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Loglar
    {
        public int logId { get; set; }
        public string logAciklama { get; set; }
        public string actionAd { get; set; }
        public string controllerAd { get; set; }
        public Nullable<System.DateTime> tarih { get; set; }
        public Nullable<int> personelId { get; set; }
    
        public virtual Personeller Personeller { get; set; }
    }
}
