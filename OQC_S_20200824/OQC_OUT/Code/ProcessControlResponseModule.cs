using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OQC_OUT
{
    public class ProcessControlResponseModule
    {
        public bool Pass { get; set; }
        public List<string> Choice_ids { get; set; }
        public string Control_id { get; set; }
        public List<ProcessesModel> Processes { get; set; }
    }

    public class ProcessesModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Pass { get; set; }
        public string Event { get; set; }
        public AfterModel After { get; set; }
    }
    public class AfterModel
    {
        public DateTime Got { get; set; }
        public DateTime Want { get; set; }
    }
}
