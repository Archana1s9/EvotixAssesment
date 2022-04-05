using System;
using System.Collections.Generic;
using System.Text;

namespace SSD.UI.Tests.Models
{
    public class EnvironmentLandContamination
    {
      
            //public string RecNo { get; set; }
            public string OrgUnit { get; set; }
            public string Reference { get; set; }
            public string EnvironmentalAssessmentReference { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public string Locality { get; set; }
            public string SampleDate { get; set; }
            public string SelectfromPersonregister { get; set; }
            public string PersonReference { get; set; }
            public string Forenames { get; set; }
            public string Surname { get; set; }
            public string SelectEquipmentfromRegister { get; set; }
            public string EquipmentReference { get; set; }
            public string EquipmentName { get; set; }
            public string Delete { get; set; }
        
       /* public partial class SampleDetails
        {
            public string RecNo { get; set; }
            public string Date { get; set; }
            public string Test { get; set; }
            public string UnitofMeasure { get; set; }
            public string NumericQuantity { get; set; }
            public string Location { get; set; }
            public string TestReference { get; set; }
            public string Result { get; set; }
           
        }
        public partial class Conclusion
        {
            public string RecNo { get; set; }
            public string Comments { get; set; }
            public string TestPassed { get; set; }
           

        }*/
        public override string ToString()
        {
            return Description;
        }
    }
}
